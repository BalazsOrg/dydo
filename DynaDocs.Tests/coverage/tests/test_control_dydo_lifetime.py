"""Real Windows job ownership: blocked descendants, interruption and retained evidence."""
import contextlib
import ctypes
import io
import json
import os
from pathlib import Path
import shutil
import signal
import subprocess
import sys
import tempfile
import unittest
from unittest.mock import patch
from types import SimpleNamespace

ROOT = Path(__file__).resolve().parents[3]
sys.path.insert(0, str(ROOT / "skills/engineering/verify-dydo/scripts"))
import control_dydo as control
import owned_process


class PlatformDispatchTests(unittest.TestCase):
    def test_windows_keeps_the_native_job_route(self):
        with patch.object(control.sys, "platform", "win32"), patch.object(owned_process, "run_owned") as native:
            control.run_owned(["tool"], cwd=ROOT, env={}, input="payload", output=ROOT, timeout=7)
        native.assert_called_once_with(["tool"], cwd=ROOT, env={}, input="payload", output=ROOT, timeout=7)

    def test_darwin_selects_posix_without_claiming_native_proof(self):
        with patch.object(control.sys, "platform", "darwin"), patch.object(control, "posix_owned") as native:
            control.run_owned(["tool"], cwd=ROOT, env={}, input="payload", output=ROOT, timeout=7)
        native.assert_called_once_with(["tool"], cwd=ROOT, env={}, input="payload", output=ROOT, timeout=7)

    def test_darwin_membership_parses_only_the_owned_group(self):
        snapshot = subprocess.CompletedProcess([], 0, "10 10 Z\n11 10 S+\n12 12 R\n", "")
        with patch.object(control.sys, "platform", "darwin"):
            with patch.object(control.subprocess, "run", return_value=snapshot):
                self.assertEqual({10: "Z", 11: "S+"}, control.posix_members(10))


@unittest.skipUnless(sys.platform in ("linux", "darwin"), "POSIX session owner")
class PosixLifetimeTests(unittest.TestCase):
    def setUp(self):
        temporary = tempfile.TemporaryDirectory(prefix="dydo-posix-lifetime-")
        self.addCleanup(temporary.cleanup)
        root = patch.object(control, "RUNS", Path(temporary.name) / "runs")
        root.start()
        self.addCleanup(root.stop)
        self.run = control.allocate()
        self.addCleanup(self.retain)

    def retain(self):
        destination = os.environ.get("DYDO_LIFETIME_EVIDENCE")
        if destination:
            shutil.copytree(self.run, Path(destination) / self._testMethodName)

    def fixture(self, exit_root=False, detached_streams=False):
        child = ("import os, signal, threading; signal.signal(signal.SIGTERM, signal.SIG_IGN); "
                 "print('descendant-ready:' + str(os.getpid()), flush=True); threading.Event().wait()")
        streams = "subprocess.DEVNULL" if detached_streams else "sys.stdout"
        ending = "" if exit_root else "import threading; threading.Event().wait()"
        script = ("import subprocess, sys; "
                  f"subprocess.Popen([sys.executable, '-c', {child!r}], stdin=subprocess.DEVNULL, "
                  f"stdout={streams}, stderr={streams}); " + ending)
        return [sys.executable, "-c", script]

    def evidence(self, label):
        receipt = json.loads(next((self.run / "evidence").glob(label + '-*.json')).read_text())
        result = json.loads(Path(receipt["lifecycle"]).read_text())
        self.assertTrue(result["cleanup_confirmed"])
        self.assertTrue(result["root_reaped"])
        self.assertTrue(result["group_quiescent"])
        self.assertTrue(result["streams_complete"])
        self.assertEqual(result["root_identity"]["pid"], result["root_identity"]["pgid"])
        self.assertEqual(result["root_identity"]["pid"], result["root_identity"]["sid"])
        self.assertEqual(str(control.ROOT), result["source"]["checkout"])
        self.assertEqual(control.digest(Path(control.__file__)), result["source"]["sha256"])
        self.assertEqual(sys.platform, result["runtime"]["platform"])
        self.assertIn("escaped sessions", result["containment_limits"])
        return receipt, result

    def test_timeout_terminates_owned_descendants_and_foreign_process_survives(self):
        foreign = subprocess.Popen([sys.executable, '-c', 'import threading; threading.Event().wait()'])
        try:
            with self.assertRaises(subprocess.TimeoutExpired):
                control.execute(self.run, "hung", self.fixture(), self.run, timeout=1)
            receipt, result = self.evidence("hung")
            self.assertEqual("timeout", result["failure_category"])
            self.assertIsNone(foreign.poll())
            self.assertIn("descendant-ready:", Path(receipt["stdout_path"]).read_text())
            control.cleanup(self.run)
            self.assertFalse((self.run / "scratch").exists())
            self.assertTrue(Path(receipt["lifecycle"]).is_file())
        finally:
            foreign.kill()
            foreign.wait(timeout=5)

    def test_exited_root_does_not_leave_blocked_descendant(self):
        with self.assertRaises(subprocess.TimeoutExpired):
            control.execute(self.run, "orphan", self.fixture(exit_root=True), self.run, timeout=1)
        receipt, result = self.evidence("orphan")
        self.assertEqual(0, result["subject_status"])
        self.assertIn("descendant-ready:", Path(receipt["stdout_path"]).read_text())

    def test_exited_root_and_closed_streams_do_not_hide_a_descendant(self):
        with self.assertRaises(subprocess.TimeoutExpired):
            control.execute(self.run, "silent", self.fixture(exit_root=True, detached_streams=True),
                            self.run, timeout=1)
        _, result = self.evidence("silent")
        self.assertEqual(0, result["subject_status"])
        self.assertEqual("timeout", result["failure_category"])

    def test_real_interrupt_cleans_before_releasing_busy(self):
        snapshot = control.posix_members
        interrupted = False

        def interrupt_after_output(pgid):
            nonlocal interrupted
            members = snapshot(pgid)
            outputs = (self.run / "evidence").glob("interrupt-*/stdout.log")
            if not interrupted and any(b"descendant-ready:" in path.read_bytes() for path in outputs):
                interrupted = True
                os.kill(os.getpid(), signal.SIGINT)
            return members

        def launch(run):
            control.execute(run, "interrupt", self.fixture(), run, timeout=5)

        with patch.object(control, "allocate", return_value=self.run), patch.object(control, "launch", launch):
            with patch.object(control, "posix_members", side_effect=interrupt_after_output):
                with contextlib.redirect_stdout(io.StringIO()), self.assertRaises(KeyboardInterrupt):
                    control.main(["launch"])
        _, result = self.evidence("interrupt")
        self.assertEqual("KeyboardInterrupt", result["failure_category"])
        self.assertFalse((self.run / "busy").exists())
        self.assertFalse((self.run / "scratch").exists())

    def test_interrupt_during_spawn_still_owns_and_reaps_the_root(self):
        spawn = subprocess.Popen
        launched = []

        def interrupt_on_return(*args, **options):
            process = spawn(*args, **options)
            launched.append(process)
            os.kill(os.getpid(), signal.SIGINT)
            return process

        try:
            with patch.object(control.subprocess, "Popen", side_effect=interrupt_on_return):
                with self.assertRaises(KeyboardInterrupt):
                    control.execute(self.run, "spawn-interrupt", self.fixture(), self.run, timeout=1)
            _, result = self.evidence("spawn-interrupt")
            self.assertEqual("KeyboardInterrupt", result["failure_category"])
        finally:
            # A red run must not leak the child whose return was interrupted.
            for process in launched:
                if process.poll() is None:
                    os.killpg(process.pid, signal.SIGKILL)
                    process.wait(timeout=5)

    def test_interrupt_during_teardown_finishes_cleanup_then_reports_interrupt(self):
        kill = os.killpg

        def interrupt_before_kill(pgid, number):
            os.kill(os.getpid(), signal.SIGINT)
            kill(pgid, number)

        with patch.object(control.os, "killpg", side_effect=interrupt_before_kill):
            with self.assertRaises(KeyboardInterrupt):
                control.execute(self.run, "teardown-interrupt", [sys.executable, "-c", "pass"], self.run)
        _, result = self.evidence("teardown-interrupt")
        self.assertEqual("KeyboardInterrupt", result["failure_category"])

    def test_stdin_raw_streams_environment_cwd_and_nonzero_exit(self):
        script = ("import os, sys; assert os.getcwd() == sys.argv[1]; assert os.environ['DYDO_PROBE'] == 'yes'; "
                  "sys.stdout.buffer.write(sys.stdin.buffer.read() + b'\\xff'); "
                  "sys.stderr.buffer.write(b'error\\r\\n'); sys.exit(7)")
        result = control.execute(self.run, "streams", [sys.executable, "-c", script, str(self.run)],
                                 self.run, "payload\n", env={**os.environ, "DYDO_PROBE": "yes"})
        self.assertEqual((7, "payload\n\udcff", "error\r\n"), (result.returncode, result.stdout, result.stderr))
        receipt, lifecycle = self.evidence("streams")
        self.assertEqual(7, lifecycle["subject_status"])
        self.assertEqual(b"payload\n\xff", (Path(receipt["lifecycle"]).parent / "stdout.log").read_bytes())
        self.assertEqual("payload\n", (Path(receipt["lifecycle"]).parent / "stdin.txt").read_text())

    def test_large_stdout_and_stderr_drain_without_pipe_deadlock(self):
        script = "import sys; sys.stdout.buffer.write(b'o' * 200000); sys.stderr.buffer.write(b'e' * 200000)"
        result = control.execute(self.run, "large", [sys.executable, "-c", script], self.run, timeout=5)
        self.assertEqual((0, "o" * 200000, "e" * 200000), (result.returncode, result.stdout, result.stderr))
        self.evidence("large")

    def test_escaped_session_holding_streams_retains_scratch(self):
        pid_path = self.run / "escaped.pid"
        child = "import threading; threading.Event().wait()"
        script = ("import subprocess, sys; from pathlib import Path; "
                  f"p = subprocess.Popen([sys.executable, '-c', {child!r}], start_new_session=True); "
                  f"Path({str(pid_path)!r}).write_text(str(p.pid))")
        try:
            with self.assertRaisesRegex(RuntimeError, "cleanup unconfirmed"):
                control.execute(self.run, "escaped", [sys.executable, "-c", script], self.run, timeout=1)
            receipt = json.loads(next((self.run / "evidence").glob("escaped-*.json")).read_text())
            result = json.loads(Path(receipt["lifecycle"]).read_text())
            self.assertFalse(result["cleanup_confirmed"])
            self.assertFalse(result["streams_complete"])
            self.assertTrue(result["group_quiescent"])
            self.assertTrue(result["root_reaped"])
            os.kill(int(pid_path.read_text()), 0)
            with self.assertRaisesRegex(ValueError, "cleanup unconfirmed"):
                control.cleanup(self.run)
            self.assertTrue((self.run / "scratch").is_dir())
            self.assertTrue(Path(receipt["lifecycle"]).is_file())
        finally:
            # The adapter cannot contain an escaped session. The fixture owns this PID.
            if pid_path.exists():
                os.kill(int(pid_path.read_text()), signal.SIGKILL)

    def test_nondefault_sigchld_refuses_before_child_launch(self):
        with patch.object(control.signal, "getsignal", return_value=signal.SIG_IGN):
            with self.assertRaisesRegex(ValueError, "default SIGCHLD"):
                control.execute(self.run, "sigchld", [sys.executable, "-c", "pass"], self.run)
        control.cleanup(self.run)

    def test_unconfirmed_cleanup_retains_scratch_and_evidence(self):
        # Lost membership observation must not be converted to a cleanup success.
        with patch.object(control, "posix_members", side_effect=OSError("membership unavailable")):
            with self.assertRaisesRegex(RuntimeError, "cleanup unconfirmed"):
                control.execute(self.run, "unconfirmed", [sys.executable, "-c", "pass"], self.run)
        with self.assertRaisesRegex(ValueError, "cleanup unconfirmed"):
            control.cleanup(self.run)
        self.assertTrue((self.run / "scratch").is_dir())
        receipt = next((self.run / "evidence").glob("unconfirmed-*.json"))
        self.assertTrue(Path(json.loads(receipt.read_text())["lifecycle"]).is_file())

    def test_missing_executable_retains_command_receipt(self):
        with self.assertRaises(FileNotFoundError):
            control.execute(self.run, "absent", [str(self.run / "absent")], self.run)
        receipt = json.loads(next((self.run / "evidence").glob("absent-*.json")).read_text())
        self.assertEqual("FileNotFoundError", receipt["error_type"])
        control.cleanup(self.run)


class OwnedProcessSupervisionTests(unittest.TestCase):
    def test_finished_native_job_is_successful_with_the_shared_wait_contract(self):
        def query(_job, kind, pointer, _size, _length):
            if kind == 1:
                pointer._obj.active = 0
                pointer._obj.total = 1
            return True

        def exit_code(_process, pointer):
            pointer._obj.value = 7
            return True

        kernel = SimpleNamespace(ResumeThread=lambda _thread: 1,
                                 QueryInformationJobObject=query,
                                 WaitForSingleObject=lambda _handle, _milliseconds: 0,
                                 GetExitCodeProcess=exit_code,
                                 TerminateJobObject=lambda _job, _code: self.fail('completed job was terminated'))
        with contextlib.ExitStack() as stack:
            result = owned_process.supervise(kernel, 1, SimpleNamespace(thread=2, process=3), [],
                                             {'execution_seconds': 1, 'teardown_seconds': 1}, stack)
        self.assertIsNone(result['failure_category'])
        self.assertTrue(result['cleanup_confirmed'])
        self.assertEqual(7, result['subject_status'])


@unittest.skipUnless(os.name == "nt" and sys.version_info[:2] == (3, 14), "Windows CPython 3.14 owner")
class ControlDydoLifetimeTests(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory(prefix="dydo-lifetime-")
        self.addCleanup(self.temporary.cleanup)
        root = patch.object(control, "RUNS", Path(self.temporary.name) / "runs")
        root.start()
        self.addCleanup(root.stop)
        self.run = control.allocate()
        self.addCleanup(self.retain)

    def retain(self):
        destination = os.environ.get("DYDO_LIFETIME_EVIDENCE")
        if destination:
            shutil.copytree(self.run, Path(destination) / self._testMethodName)

    def fixture(self, exit_root=False):
        descendant = "import threading; print('descendant-ready', flush=True); threading.Event().wait()"
        ending = "" if exit_root else "import threading; threading.Event().wait()"
        script = ("import subprocess, sys; "
                  f"subprocess.Popen([sys.executable, '-c', {descendant!r}], stdin=subprocess.DEVNULL, "
                  "stdout=sys.stdout, stderr=sys.stderr, creationflags=subprocess.CREATE_NO_WINDOW); "
                  + ending)
        return [sys._base_executable, "-c", script]

    def evidence(self, label):
        receipt = json.loads(next((self.run / "evidence").glob(label + '-*.json')).read_text())
        result = json.loads(Path(receipt["lifecycle"]).read_text())
        self.assertTrue(result["cleanup_confirmed"])
        self.assertTrue(result["root_signaled"])
        self.assertTrue(result["job_empty"])
        self.assertTrue(result["streams_complete"])
        self.assertGreaterEqual(result["job_total"], 2)
        self.assertTrue(all(row["signaled"] for row in result["held_handles"]))
        self.assertIn("descendant-ready", Path(receipt["stdout_path"]).read_text())
        return receipt, result

    def test_timeout_reaps_root_and_descendant_and_preserves_foreign_process(self):
        with subprocess.Popen([sys._base_executable, '-c', 'import threading; threading.Event().wait()'],
                              creationflags=subprocess.CREATE_NO_WINDOW) as foreign:
            try:
                with self.assertRaises(subprocess.TimeoutExpired):
                    control.execute(self.run, "hung", self.fixture(), self.run, timeout=3)
                receipt, result = self.evidence("hung")
                self.assertEqual("TimeoutExpired", receipt["error_type"])
                self.assertEqual("timeout", result["failure_category"])
                self.assertIsNone(foreign.poll())
                control.cleanup(self.run)
                self.assertFalse((self.run / "scratch").exists())
                self.assertTrue(Path(receipt["lifecycle"]).exists())
            finally:
                foreign.terminate()
                foreign.wait(timeout=10)

    def test_exited_root_does_not_leave_blocked_descendant(self):
        with self.assertRaises(subprocess.TimeoutExpired):
            control.execute(self.run, "orphan", self.fixture(exit_root=True), self.run, timeout=3)
        _, result = self.evidence("orphan")
        self.assertEqual(0, result["subject_status"])
        control.cleanup(self.run)

    def test_interrupt_after_descendant_output_cleans_before_releasing_busy(self):
        original = owned_process.jobs.poll_pipe
        interrupted = False

        def interrupt_after_output(kernel, reader):
            nonlocal interrupted
            original(kernel, reader)
            stream = Path(reader["target"].name)
            if interrupted or stream.name != "stdout.log":
                return
            # Interpreter startup diagnostics on stderr do not establish readiness.
            reader["target"].flush()
            if b"descendant-ready" in stream.read_bytes().splitlines():
                interrupted = True
                raise KeyboardInterrupt()

        def launch(run):
            control.execute(run, "interrupt", self.fixture(), run, timeout=10)

        with patch.object(control, "allocate", return_value=self.run), patch.object(control, "launch", launch):
            with patch.object(owned_process.jobs, "poll_pipe", side_effect=interrupt_after_output):
                with contextlib.redirect_stdout(io.StringIO()), self.assertRaises(KeyboardInterrupt):
                    control.main(["launch"])
        self.evidence("interrupt")
        self.assertFalse((self.run / "busy").exists())
        self.assertFalse((self.run / "scratch").exists())
        failure = json.loads(next((self.run / "evidence").glob("failure-*.json")).read_text())
        self.assertEqual("KeyboardInterrupt", failure["type"])

    def test_stdin_stdout_stderr_and_nonzero_exit_are_preserved(self):
        kernel = owned_process.jobs.bindings()
        set_limits = kernel.SetInformationJobObject
        launch = owned_process.jobs.launch

        def checked_limits(job, kind, pointer, size):
            self.assertEqual((9, 144), (kind, size))
            self.assertEqual(bytes(16) + b"\x00\x20\x00\x00" + bytes(124),
                             ctypes.string_at(pointer, size))
            return set_limits(job, kind, pointer, size)

        def checked_startup(api, request, environment, startup, process):
            # Win64 STARTUPINFOEXW: cb at 0, dwFlags at 60, attribute list at 104.
            raw = bytes(startup)
            self.assertEqual(112, len(raw))
            self.assertEqual(112, int.from_bytes(raw[:4], "little"))
            self.assertEqual(0x100, int.from_bytes(raw[60:64], "little"))
            self.assertNotEqual(bytes(8), raw[104:112])
            return launch(api, request, environment, startup, process)

        script = "import sys; print(sys.stdin.read(), end=''); print('error', file=sys.stderr); sys.exit(7)"
        with patch.object(owned_process.jobs, "bindings", return_value=kernel):
            with patch.object(kernel, "SetInformationJobObject", side_effect=checked_limits) as limits:
                with patch.object(owned_process.jobs, "launch", side_effect=checked_startup) as startup:
                    result = control.execute(self.run, "streams", [sys.executable, "-c", script],
                                             self.run, "payload\n")
        limits.assert_called_once()
        startup.assert_called_once()
        self.assertEqual((7, "payload\r\n", "error\r\n"), (result.returncode, result.stdout, result.stderr))
        control.cleanup(self.run)

    def test_unconfirmed_cleanup_retains_scratch(self):
        output = self.run / "evidence/unconfirmed"
        output.mkdir()
        control.write_json(output / "result.json", {"cleanup_confirmed": False})
        with self.assertRaisesRegex(ValueError, "cleanup unconfirmed"):
            control.cleanup(self.run)
        self.assertTrue((self.run / "scratch").exists())

    def test_missing_executable_retains_command_receipt(self):
        with self.assertRaises(FileNotFoundError):
            control.execute(self.run, "absent", [str(self.run / 'absent.exe')], self.run)
        receipt = json.loads(next((self.run / "evidence").glob("absent-*.json")).read_text())
        self.assertEqual("FileNotFoundError", receipt["error_type"])
        control.cleanup(self.run)

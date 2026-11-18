"""Inherited facade cases; collected through the two public facade classes."""
import json
import os
import queue
import shutil
import signal
import subprocess
import sys
import tempfile
import threading
import time
import unittest
from pathlib import Path
from test_testing_facade_support import (has_worktree_identity, first_adapter_line, next_line, configured, stack, manifest, ROOT, RUNNER, SCRIPT_PATH)


class Cases:
    """Only the public facade classes collect these inherited unittest cases."""

    class FacadeCases06(unittest.TestCase):
        def test_interrupting_the_real_dotnet_adapter_cleans_its_worktree(self):
            with tempfile.TemporaryDirectory(prefix='dydo-interrupt-spelling-') as temporary:
                target, alias = Path(temporary) / 'target', Path(temporary) / 'alias'
                target.mkdir()
                if os.name == 'nt':
                    created = subprocess.run(
                        [os.environ['COMSPEC'], '/d', '/c', 'mklink', '/J', str(alias), str(target)],
                        capture_output=True, text=True, encoding='utf-8')
                    self.assertEqual(0, created.returncode, created.stdout + created.stderr)
                else:
                    alias.symlink_to(target, target_is_directory=True)
                environment = os.environ.copy()
                environment.update({'TEMP': str(alias), 'TMP': str(alias), 'TMPDIR': str(alias)})
                command = [sys.executable, '-u', str(RUNNER), 'test', '--stack', 'dotnet', '--', '--filter', 'FullyQualifiedName~ConsoleCaptureTests.Stderr_RestoresConsoleError_WhenActionSucceeds']
                facade = subprocess.Popen(command, cwd=ROOT, env=environment,
                                          stdout=subprocess.PIPE, stderr=subprocess.STDOUT,
                                          text=True, encoding='utf-8', errors='replace',
                                          creationflags=subprocess.CREATE_NEW_PROCESS_GROUP if os.name == 'nt' else 0,
                                          start_new_session=os.name != 'nt')
                lines, output = queue.Queue(), []
                def collect():
                    for line in facade.stdout:
                        output.append(line); lines.put(line)
                reader = threading.Thread(target=collect, daemon=True); reader.start()
                try:
                    announced = next_line(lines, lambda line: line.strip().startswith('Worktree: '), 60)
                    self.assertIsNotNone(announced, ''.join(output))
                    worktree = Path(announced.strip()[10:])
                    listing = subprocess.run(['git', 'worktree', 'list', '--porcelain'], cwd=ROOT, text=True, capture_output=True, check=True).stdout
                    self.assertTrue(has_worktree_identity(listing, worktree))
                    self.assertTrue(has_worktree_identity(listing, ROOT))
                    # Interrupt with the build in flight, the window where build processes started
                    # without a console outlive dotnet and hold the worktree or files in their TEMP.
                    self.assertIsNotNone(next_line(lines, lambda line: ' -> ' in line, 180), ''.join(output))
                    started = time.monotonic()
                    facade.send_signal(signal.CTRL_BREAK_EVENT if os.name == 'nt' else signal.SIGINT)
                    facade.wait(timeout=35); reader.join(timeout=5)
                    self.assertEqual(130, facade.returncode, ''.join(output))
                    self.assertLess(time.monotonic() - started, 35)
                    self.assertFalse(worktree.exists(), ''.join(output))
                    self.assertFalse((target / 'VBCSCompiler').exists(),
                                     'the isolated run started a compiler server that outlives it in its TEMP')
                    after = subprocess.run(['git', 'worktree', 'list', '--porcelain'], cwd=ROOT, text=True, capture_output=True, check=True).stdout
                    self.assertFalse(has_worktree_identity(after, worktree))
                    self.assertTrue(has_worktree_identity(after, ROOT))
                    result_line = next(x.strip()[8:] for x in output if x.startswith('Result: '))
                    payload = json.loads(Path(result_line).read_text(encoding='utf-8'))
                    self.assertEqual(130, payload['aggregateExit'])
                    row = payload['results'][0]
                    self.assertEqual(('interrupted', 130), (row['state'], row['resultExit']))
                    self.assertEqual(130, row['childExit'])
                    self.assertLess(next(i for i,x in enumerate(output) if 'Cleaning up worktree' in x), next(i for i,x in enumerate(output) if x.startswith('Result: ')))
                finally:
                    if facade.poll() is None:
                        facade.send_signal(signal.CTRL_BREAK_EVENT if os.name == 'nt' else signal.SIGINT)
                        facade.wait(timeout=35)
                    reader.join(timeout=5)
                    facade.stdout.close()


        def test_an_interrupt_during_worktree_cleanup_does_not_abort_it(self):
            adapter = ROOT / 'DynaDocs.Tests/coverage/run_tests.py'
            with tempfile.TemporaryDirectory(prefix='dydo-cleanup-signal-') as temporary:
                harness = (
                    "import importlib.util,signal,sys\n"
                    + SCRIPT_PATH + f"spec=importlib.util.spec_from_file_location('run_tests_probe', {str(adapter)!r})\n"
                    "module=importlib.util.module_from_spec(spec); spec.loader.exec_module(module)\n"
                    f"module.tempfile.gettempdir=lambda: {temporary!r}\n"
                    "module.test_command=lambda extra: [sys.executable, '-c', 'pass']\n"
                    "original=module._git\n"
                    "def interrupted_git(*args, capture=False):\n"
                    "    if args[:2] == ('worktree', 'remove'):\n"
                    "        module._git=original\n"
                    "        print('INTERRUPTED_CLEANUP', flush=True)\n"
                    "        signal.raise_signal(signal.SIGBREAK if sys.platform == 'win32' else signal.SIGINT)\n"
                    "    return original(*args, capture=capture)\n"
                    "module._git=interrupted_git\n"
                    "sys.argv=[sys.argv[0]]\n"
                    "module.main()\n"
                )
                process = subprocess.run([sys.executable, '-u', '-c', harness], cwd=ROOT,
                                         capture_output=True, text=True, encoding='utf-8',
                                         errors='replace', timeout=60)
                output = process.stdout + process.stderr
                worktree = next((Path(line.strip()[10:]) for line in process.stdout.splitlines()
                                 if line.strip().startswith('Worktree: ')), None)
                try:
                    self.assertIsNotNone(worktree, output)
                    self.assertIn('INTERRUPTED_CLEANUP', output)
                    self.assertEqual(130, process.returncode, output)
                    self.assertFalse(worktree.exists(), output)
                    listing = subprocess.run(['git', 'worktree', 'list', '--porcelain'], cwd=ROOT,
                                             text=True, capture_output=True, check=True).stdout
                    self.assertFalse(has_worktree_identity(listing, worktree))
                finally:
                    if worktree is not None:
                        subprocess.run(['git', 'worktree', 'remove', '--force', str(worktree)], cwd=ROOT,
                                       capture_output=True, text=True)
                        subprocess.run(['git', 'worktree', 'prune'], cwd=ROOT, capture_output=True, text=True)


        @unittest.skipUnless(os.name == 'nt', 'Only Windows refuses to delete a directory a live process runs in')
        def test_a_process_the_dotnet_run_leaves_detached_cannot_keep_its_worktree(self):
            adapter = ROOT / 'DynaDocs.Tests/coverage/run_tests.py'
            with tempfile.TemporaryDirectory(prefix='dydo-detached-build-') as temporary:
                # Like an MSBuild task host: started without a console, so no interrupt reaches it, and
                # left running in the worktree after the dotnet run returns.
                detached = ("import subprocess,sys\n"
                            "subprocess.Popen([sys.executable,'-c','import time; time.sleep(20)'],"
                            " creationflags=subprocess.CREATE_NO_WINDOW|subprocess.CREATE_NEW_PROCESS_GROUP,"
                            " stdin=subprocess.DEVNULL, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)\n"
                            "print('DETACHED_STARTED', flush=True)\n")
                harness = (
                    "import importlib.util,sys\n"
                    + SCRIPT_PATH + f"spec=importlib.util.spec_from_file_location('run_tests_probe', {str(adapter)!r})\n"
                    "module=importlib.util.module_from_spec(spec); spec.loader.exec_module(module)\n"
                    f"module.tempfile.gettempdir=lambda: {temporary!r}\n"
                    f"module.test_command=lambda extra: [sys.executable, '-c', {detached!r}]\n"
                    "sys.argv=[sys.argv[0]]\n"
                    "module.main()\n"
                )
                process = subprocess.run([sys.executable, '-u', '-c', harness], cwd=ROOT,
                                         capture_output=True, text=True, encoding='utf-8',
                                         errors='replace', timeout=60)
                output = process.stdout + process.stderr
                worktree = next((Path(line.strip()[10:]) for line in process.stdout.splitlines()
                                 if line.strip().startswith('Worktree: ')), None)
                self.assertIsNotNone(worktree, output)
                self.assertIn('DETACHED_STARTED', output)
                self.assertEqual(0, process.returncode, output)
                self.assertNotIn('Failed to remove test worktree', output)
                self.assertFalse(worktree.exists(), output)
                listing = subprocess.run(['git', 'worktree', 'list', '--porcelain'], cwd=ROOT,
                                         text=True, capture_output=True, check=True).stdout
                self.assertFalse(has_worktree_identity(listing, worktree))


        def test_interrupt_during_real_worktree_registration_cleans_the_attributed_path(self):
            adapter = ROOT / 'DynaDocs.Tests/coverage/run_tests.py'
            for registration_only in [False, True]:
                with self.subTest(registration_only=registration_only):
                    harness = (
                        "import importlib.util,json,shutil,sys\nfrom pathlib import Path\n"
                        + SCRIPT_PATH + f"spec=importlib.util.spec_from_file_location('run_tests_probe', {str(adapter)!r})\n"
                        "module=importlib.util.module_from_spec(spec); spec.loader.exec_module(module)\n"
                        "original=module._git\n"
                        "def delayed_git(*args, capture=False):\n"
                        "    if args[:3] == ('worktree', 'add', '--detach'):\n"
                        "        assert Path(args[3]).is_dir() and not list(Path(args[3]).iterdir())\n"
                        "    result=original(*args, capture=capture)\n"
                        "    if args[:3] == ('worktree', 'add', '--detach') and result.returncode == 0:\n"
                        "        listing,rc=original('worktree', 'list', '--porcelain', capture=True)\n"
                        "        assert rc == 0\n"
                        f"        if {registration_only!r}: shutil.rmtree(args[3])\n"
                        "        print('REGISTERED: ' + args[3], flush=True)\n"
                        "        print('REGISTERED_LISTING: ' + json.dumps(listing), flush=True)\n"
                        "        raise KeyboardInterrupt\n"
                        "    return result\n"
                        "module._git=delayed_git\n"
                        "module.main()\n"
                    )
                    process = subprocess.run([sys.executable, '-u', '-c', harness], cwd=ROOT,
                                             capture_output=True, text=True, encoding='utf-8',
                                             errors='replace', timeout=30)
                    output = process.stdout + process.stderr
                    registered = next((line[12:] for line in process.stdout.splitlines()
                                       if line.startswith('REGISTERED: ')), None)
                    registered_listing = next((json.loads(line[20:]) for line in process.stdout.splitlines()
                                               if line.startswith('REGISTERED_LISTING: ')), None)
                    attributed = Path(registered) if registered else None
                    try:
                        self.assertIsNotNone(attributed, output)
                        self.assertIsNotNone(registered_listing, output)
                        self.assertTrue(has_worktree_identity(registered_listing, attributed), output)
                        self.assertTrue(has_worktree_identity(registered_listing, ROOT), output)
                        self.assertFalse(any(line.strip().startswith('Worktree: ')
                                             for line in process.stdout.splitlines()), output)
                        self.assertEqual(130, process.returncode, output)
                        self.assertFalse(attributed.exists(), output)
                        listing = subprocess.run(['git', 'worktree', 'list', '--porcelain'], cwd=ROOT,
                                                 text=True, capture_output=True, check=True).stdout
                        self.assertFalse(has_worktree_identity(listing, attributed))
                        self.assertTrue(has_worktree_identity(listing, ROOT))
                    finally:
                        if attributed is not None:
                            subprocess.run(['git', 'worktree', 'remove', '--force', str(attributed)], cwd=ROOT,
                                           capture_output=True, text=True)
                            if attributed.exists(): shutil.rmtree(attributed)


        def test_interrupt_stops_later_capabilities_and_stacks(self):
            for operation, block_result in [('all', False), ('--force-run', False), ('all', True)]:
                with self.subTest(operation=operation, block_result=block_result):
                    data = manifest(stack('first'), stack('later'))
                    root = self.fixture(data)
                    (root / 'wait.py').write_text(
                        "import signal,sys,time\nfrom pathlib import Path\n"
                        "def interrupted(signum, frame):\n    raise KeyboardInterrupt\n"
                        "signal.signal(signal.SIGBREAK if sys.platform == 'win32' else signal.SIGINT, interrupted)\n"
                        "try:\n    print('WAITING_FOR_INTERRUPT', flush=True)\n    deadline=time.monotonic()+60\n"
                        "    while time.monotonic()<deadline:\n        time.sleep(0.05)\n"
                        "except KeyboardInterrupt:\n    print('ADAPTER_INTERRUPTED', flush=True)\n    sys.exit(130)\n"
                        "finally:\n    Path('cleanup.txt').write_text('complete')\n"
                        + ("    for p in Path('results').glob('run-*/result.tmp'):\n        p.unlink()\n        p.mkdir()\n"
                           if block_result else ''), encoding='utf-8')
                    data['stacks'][0]['capabilities']['test'] = configured(['-u', 'wait.py'])
                    (root / 'gap_check.json').write_text(json.dumps(data), encoding='utf-8')
                    facade = subprocess.Popen([sys.executable, '-u', str(root / 'gap_check.py'), operation],
                        cwd=root, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True, encoding='utf-8',
                        creationflags=subprocess.CREATE_NEW_PROCESS_GROUP if os.name == 'nt' else 0,
                        start_new_session=os.name != 'nt')
                    lines, output = queue.Queue(), []
                    def collect():
                        for line in facade.stdout:
                            output.append(line); lines.put(line)
                    reader = threading.Thread(target=collect, daemon=True); reader.start()
                    try:
                        self.assertEqual('WAITING_FOR_INTERRUPT', first_adapter_line(lines).strip())
                        interrupted_at = time.monotonic()
                        facade.send_signal(signal.CTRL_BREAK_EVENT if os.name == 'nt' else signal.SIGINT)
                        facade.wait(timeout=35); reader.join(timeout=5)
                        self.assertLess(time.monotonic() - interrupted_at, 10)
                        self.assertEqual(130, facade.returncode, ''.join(output))
                        self.assertIn('ADAPTER_INTERRUPTED', ''.join(output))
                        self.assertEqual('complete', (root / 'cleanup.txt').read_text())
                        self.assertEqual(['cleanup.txt'], [p.name for p in root.glob('*.txt')])
                        if block_result:
                            self.assertFalse(any(line.startswith('Result: ') for line in output))
                            self.assertIn('artifact destination', ''.join(output))
                            self.assertNotIn('Traceback', ''.join(output))
                            continue
                        result_path = next(line.strip()[8:] for line in output if line.startswith('Result: '))
                        payload = json.loads(Path(result_path).read_text(encoding='utf-8'))
                        self.assert_interrupted_selection(payload, operation)
                        self.assertEqual(130, payload['aggregateExit'])
                        self.assertEqual(['first', 'later'], payload['selectedStacks'])
                    finally:
                        if facade.poll() is None:
                            facade.send_signal(signal.CTRL_BREAK_EVENT if os.name == 'nt' else signal.SIGINT)
                            facade.wait(timeout=35)
                        reader.join(timeout=5)
                        facade.stdout.close()

"""Inherited facade cases; collected through the two public facade classes."""
import json
import os
import subprocess
import sys
import time
import unittest
from pathlib import Path
from test_testing_facade_support import (configured, stack, manifest, verdict_stack)


class Cases:
    """Only the public facade classes collect these inherited unittest cases."""

    class FacadeCases07(unittest.TestCase):
        def test_first_interrupt_during_launch_tracks_and_reaps_the_adapter(self):
            self.assert_launch_interrupt(False)


        def test_first_interrupt_from_caller_handler_during_launch_tracks_the_adapter(self):
            self.assert_launch_interrupt(True)


        def test_first_interrupt_during_reporting_or_publication_preserves_finished_rows(self):
            for moment in ('prepare', 'static report', 'coverage report', 'serialize', 'write', 'replace', 'aggregate', 'result', 'halted result'):
                with self.subTest(moment=moment):
                    first_stack = verdict_stack()
                    if moment == 'halted result':
                        first_stack['capabilities']['static']['command']['argv'][1] += '; raise SystemExit(130)'
                    root = self.fixture(manifest(first_stack, stack('later')))
                    (root / 'publication_interrupt.py').write_text(
                        "import builtins,json,pathlib,runpy,signal,sys\n"
                        f"moment={moment!r}\n"
                        "report,serialize,write,replace,opened=builtins.print,json.dumps,pathlib.Path.write_text,pathlib.Path.replace,pathlib.Path.open\n"
                        "fired=False\n"
                        "def fire(at):\n"
                        "    global fired\n"
                        "    if at == moment and not fired:\n"
                        "        fired=True\n"
                        "        report('FIRST_INTERRUPT', flush=True)\n"
                        "        signal.raise_signal(signal.SIGBREAK if sys.platform == 'win32' else signal.SIGINT)\n"
                        "def reported(*values, **options):\n"
                        "    text=str(values[0]) if values else ''\n"
                        "    at=next((at for prefix,at in [('first static:', 'static report'), ('first coverage:', 'coverage report'), ('Aggregate:', 'aggregate'), ('Result:', 'result')] if text.startswith(prefix)), '')\n"
                        "    fire('halted result' if at == 'result' and moment == 'halted result' else at)\n    report(*values, **options)\n"
                        "def serialized(value, *args, **kwargs):\n"
                        "    if isinstance(value, dict) and 'aggregateExit' in value:\n        fire('serialize')\n"
                        "    return serialize(value, *args, **kwargs)\n"
                        "def written(path, *args, **kwargs):\n"
                        "    if path.name == 'result.tmp':\n        fire('write')\n"
                        "    return write(path, *args, **kwargs)\n"
                        "def opening(path, *args, **kwargs):\n"
                        "    if path.name == 'result.tmp':\n        fire('prepare')\n"
                        "    return opened(path, *args, **kwargs)\n"
                        "def replaced(path, target):\n"
                        "    if path.name == 'result.tmp':\n"
                        "        fire('replace')\n"
                        "        if fired:\n"
                        "            report('FURTHER_INTERRUPT', flush=True)\n"
                        "            signal.raise_signal(signal.SIGBREAK if sys.platform == 'win32' else signal.SIGINT)\n"
                        "    return replace(path, target)\n"
                        "builtins.print,json.dumps,pathlib.Path.write_text,pathlib.Path.replace=reported,serialized,written,replaced\n"
                        "pathlib.Path.open=opening\n"
                        "runpy.run_path('run_gap_check.py', run_name='__main__')\n", encoding='utf-8')
                    process = subprocess.run([sys.executable, '-u', 'publication_interrupt.py', '--force-run'],
                                             cwd=root, capture_output=True, text=True, encoding='utf-8', timeout=60)
                    self.assert_exit(process, 130)
                    self.assertIn('FIRST_INTERRUPT', process.stdout)
                    self.assertNotIn('Traceback', process.stderr)
                    payload = json.loads(next((root / 'results').glob('*/result.json')).read_text(encoding='utf-8'))
                    self.assertEqual(130, payload['aggregateExit'])
                    first = ['interrupted', 'passed', 'interrupted'] if moment == 'static report' else ['passed'] * 3
                    later = ['interrupted'] * 3 if moment.endswith('report') else ['passed'] * 3
                    if moment in ('prepare', 'halted result'):
                        first = later = ['interrupted'] * 3
                    self.assertEqual(first + later, [row['state'] for row in payload['results']])
                    if moment == 'static report':
                        self.assertIn('interrupted before the coverage row', payload['results'][0]['reason'])
                    self.assertFalse((root / 'first-test.txt').exists(), 'coverage must remain the only suite execution')


        def test_first_interrupt_at_final_return_publishes_130(self):
            for target in ('write_result', 'run_operation', 'main'):
                for event in ('return', 'opcode'):
                    for custom in (False, True):
                        with self.subTest(target=target, event=event, custom=custom):
                            self.assert_final_return_interrupt(target, event, custom)


        def test_first_interrupt_from_caller_handler_never_redispatches_finished_adapters(self):
            for moment in ('first test:', 'Aggregate:', 'Result:'):
                with self.subTest(moment=moment):
                    self.assert_caller_reporting(moment, 'raise')


        def test_ignored_or_returning_caller_handler_keeps_its_signal_semantics(self):
            for handler in ('ignore', 'return'):
                with self.subTest(handler=handler):
                    self.assert_caller_reporting('first test:', handler)


        def test_first_interrupt_after_main_returns_retains_publication_ownership(self):
            for unwritable in (False, True):
                with self.subTest(unwritable=unwritable):
                    root = self.fixture(manifest(stack('first'), stack('later')))
                    launcher = root / 'run_gap_check.py'
                    launcher.write_text(launcher.read_text(encoding='utf-8').replace(
                        'raise SystemExit(module.main())',
                        "code=module.main()\n"
                        "import signal\nfrom pathlib import Path\n"
                        "replace=Path.replace\n"
                        "sig=signal.SIGBREAK if sys.platform == 'win32' else signal.SIGINT\n"
                        "def replacing(path, target):\n"
                        "    signal.raise_signal(sig)\n"
                        f"    if {unwritable!r}: raise OSError('late publication unavailable')\n"
                        "    return replace(path, target)\n"
                        "Path.replace=replacing\n"
                        "signal.raise_signal(sig)\n"
                        "raise SystemExit(code)"), encoding='utf-8')
                    process, _, payload = self.invoke(['all'], directory=root)
                    self.assert_exit(process, 130)
                    self.assertNotIn('Traceback', process.stderr)
                    self.assertEqual(0 if unwritable else 130, payload['aggregateExit'])
                    self.assertEqual(unwritable, 'late publication unavailable' in process.stderr)
                    self.assert_rows(payload, [('first', 'test', 'passed'), ('later', 'test', 'passed')])


        def test_first_interrupt_with_unwritable_publication_retry_retains_exit_130(self):
            root = self.fixture()
            (root / 'unwritable_retry.py').write_text(
                "import pathlib,runpy,signal,sys\n"
                "replace=pathlib.Path.replace\n"
                "def replacing(path, target):\n"
                "    if path.name == 'result.tmp' and not target.exists():\n"
                "        target.mkdir()\n"
                "        signal.raise_signal(signal.SIGBREAK if sys.platform == 'win32' else signal.SIGINT)\n"
                "    return replace(path, target)\n"
                "pathlib.Path.replace=replacing\n"
                "runpy.run_path('run_gap_check.py', run_name='__main__')\n", encoding='utf-8')
            process = subprocess.run([sys.executable, '-u', 'unwritable_retry.py', 'all'], cwd=root,
                                     capture_output=True, text=True, encoding='utf-8', timeout=60)
            self.assert_exit(process, 130)
            self.assertIn('artifact destination', process.stderr)
            self.assertNotIn('Traceback', process.stderr)


        @unittest.skipIf(os.name == 'nt', 'Windows Popen reaps through its process handle and takes no waitpid lock')
        def test_an_interrupt_landing_while_popen_holds_its_waitpid_lock_still_confirms_the_adapter_exit(self):
            # The facade's SIGINT is delivered as soon as Popen has taken its waitpid lock, before the
            # try that releases it: a facade that lets the interrupt surface there can no longer reap
            # its adapter, waits out the cleanup window and records no child exit.
            cleanup_seconds = 15
            root = self.fixture(cleanup_seconds=cleanup_seconds)
            (root / 'adapter.py').write_text(
                "import signal,sys,time\nfrom pathlib import Path\n"
                "def interrupted(signum, frame):\n    raise KeyboardInterrupt\n"
                "signal.signal(signal.SIGINT, interrupted)\n"
                "try:\n    Path('ready.txt').write_text('ready')\n    deadline=time.monotonic()+60\n"
                "    while time.monotonic()<deadline:\n        time.sleep(0.05)\n"
                "except KeyboardInterrupt:\n    Path('cleanup.txt').write_text('complete')\n    sys.exit(130)\n"
                "sys.exit(99)\n", encoding='utf-8')
            data = manifest(stack('first'))
            data['stacks'][0]['capabilities']['test'] = configured(['-u', 'adapter.py'])
            (root / 'gap_check.json').write_text(json.dumps(data), encoding='utf-8')
            (root / 'trapped_gap_check.py').write_text(
                "import runpy,signal,subprocess\nfrom pathlib import Path\n"
                "class InterruptedAfterAcquire:\n"
                "    def __init__(self, lock):\n        self.lock, self.armed = lock, True\n"
                "    def acquire(self, blocking=True, timeout=-1):\n"
                "        acquired = self.lock.acquire(blocking, timeout)\n"
                "        if acquired and self.armed and Path('ready.txt').exists():\n"
                "            self.armed = False\n"
                "            print('INTERRUPT_UNDER_WAITPID_LOCK', flush=True)\n"
                "            signal.raise_signal(signal.SIGINT)\n"
                "        return acquired\n"
                "    def release(self):\n        self.lock.release()\n"
                "    def __enter__(self):\n        self.acquire()\n        return self\n"
                "    def __exit__(self, *details):\n        self.release()\n"
                "class TrappedPopen(subprocess.Popen):\n"
                "    def __init__(self, args, *rest, **options):\n"
                "        super().__init__(args, *rest, **options)\n"
                "        if 'adapter.py' in args:\n"
                "            self._waitpid_lock = InterruptedAfterAcquire(self._waitpid_lock)\n"
                "subprocess.Popen = TrappedPopen\n"
                "runpy.run_path('run_gap_check.py', run_name='__main__')\n", encoding='utf-8')
            started = time.monotonic()
            process = subprocess.run([sys.executable, '-u', 'trapped_gap_check.py', 'test', '--stack', 'first'],
                                     cwd=root, capture_output=True, text=True, encoding='utf-8', timeout=60)
            elapsed = time.monotonic() - started
            output = process.stdout + process.stderr
            self.assertIn('INTERRUPT_UNDER_WAITPID_LOCK', output)
            self.assert_exit(process, 130)
            self.assertEqual('complete', (root / 'cleanup.txt').read_text(encoding='utf-8'))
            result_path = next(line[8:] for line in process.stdout.splitlines() if line.startswith('Result: '))
            row = json.loads(Path(result_path).read_text(encoding='utf-8'))['results'][0]
            self.assertEqual(('interrupted', 130), (row['state'], row['resultExit']))
            self.assertEqual(130, row['childExit'], row['reason'])
            self.assertNotIn('not reaped', row['reason'])
            self.assertLess(elapsed, cleanup_seconds, output)

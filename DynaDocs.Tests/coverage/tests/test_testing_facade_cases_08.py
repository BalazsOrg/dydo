"""Inherited facade cases; collected through the two public facade classes."""
import json
import os
import signal
import subprocess
import sys
import time
import unittest
from unittest import mock
import runpy
from pathlib import Path
from test_testing_facade_support import (configured, stack, manifest, FakeClock, FakeChild, fake_killpg)


class Cases:
    """Only the public facade classes collect these inherited unittest cases."""

    class FacadeCases08(unittest.TestCase):
        def test_a_further_interrupt_while_the_facade_stops_its_adapter_or_publishes_the_result_does_not_abort_it(self):
            # The first interrupt decides the outcome. A further one, while the facade waits on or reaps
            # the adapter it is stopping, skips the later rows or writes the result, must not abort it:
            # the row keeps the adapter's real exit and the result is published.
            cleanup_seconds = 15
            for moment in ('cleanup wait', 'after reap', 'row reported', 'result publication'):
                with self.subTest(moment=moment):
                    data = manifest(stack('first'), stack('later'))
                    root = self.fixture(data, cleanup_seconds=cleanup_seconds)
                    (root / 'adapter.py').write_text(
                        "import signal,sys,time\nfrom pathlib import Path\n"
                        "def interrupted(signum, frame):\n    raise KeyboardInterrupt\n"
                        "signal.signal(signal.SIGBREAK if sys.platform == 'win32' else signal.SIGINT, interrupted)\n"
                        "try:\n    Path('ready.txt').write_text('ready')\n    deadline=time.monotonic()+60\n"
                        "    while time.monotonic()<deadline:\n        time.sleep(0.05)\n"
                        "except KeyboardInterrupt:\n    Path('stopping.txt').write_text('stopping')\n"
                        f"    deadline=time.monotonic()+{cleanup_seconds}\n"
                        f"    while {moment == 'cleanup wait'} and not Path('released.txt').exists() and time.monotonic()<deadline:\n"
                        "        time.sleep(0.05)\n"
                        "    Path('cleanup.txt').write_text('complete')\n    sys.exit(130)\n"
                        "sys.exit(99)\n", encoding='utf-8')
                    data['stacks'][0]['capabilities']['test'] = configured(['-u', 'adapter.py'])
                    (root / 'gap_check.json').write_text(json.dumps(data), encoding='utf-8')
                    (root / 'interrupted_gap_check.py').write_text(
                        "import builtins,pathlib,runpy,signal,subprocess,sys\nfrom pathlib import Path\n"
                        f"MOMENT = {moment!r}\n"
                        "INTERRUPT = signal.SIGBREAK if sys.platform == 'win32' else signal.SIGINT\n"
                        "report, publish = builtins.print, pathlib.Path.replace\n"
                        "def further():\n"
                        "    report('FURTHER_INTERRUPT', flush=True)\n"
                        "    signal.raise_signal(INTERRUPT)\n"
                        "class InterruptedPopen(subprocess.Popen):\n"
                        "    def __init__(self, args, *rest, **options):\n"
                        "        super().__init__(args, *rest, **options)\n"
                        "        self.adapter, self.first, self.further = 'adapter.py' in args, False, False\n"
                        "    def poll(self):\n"
                        "        code = super().poll()\n"
                        "        if self.adapter and not self.first and Path('ready.txt').exists():\n"
                        "            self.first = True\n"
                        "            report('FIRST_INTERRUPT', flush=True)\n"
                        "            signal.raise_signal(INTERRUPT)\n"
                        "        elif self.adapter and not self.further and (\n"
                        "                MOMENT == 'cleanup wait' and code is None and Path('stopping.txt').exists()\n"
                        "                or MOMENT == 'after reap' and code == 130):\n"
                        "            self.further = True\n"
                        "            further()\n"
                        "            Path('released.txt').write_text('released')\n"
                        "        return code\n"
                        "def reported(*values, **options):\n"
                        "    report(*values, **options)\n"
                        "    if MOMENT == 'row reported' and values and str(values[0]).startswith('first test: INTERRUPTED'):\n"
                        "        further()\n"
                        "def published(self, target):\n"
                        "    if MOMENT == 'result publication' and self.name == 'result.tmp':\n"
                        "        further()\n"
                        "    return publish(self, target)\n"
                        "subprocess.Popen, builtins.print, pathlib.Path.replace = InterruptedPopen, reported, published\n"
                        "runpy.run_path('run_gap_check.py', run_name='__main__')\n", encoding='utf-8')
                    started = time.monotonic()
                    process = subprocess.run([sys.executable, '-u', 'interrupted_gap_check.py', 'all'],
                                             cwd=root, capture_output=True, text=True, encoding='utf-8', timeout=60)
                    elapsed = time.monotonic() - started
                    output = process.stdout + process.stderr
                    self.assertIn('FIRST_INTERRUPT', output)
                    self.assertIn('FURTHER_INTERRUPT', output)
                    self.assert_exit(process, 130)
                    self.assertNotIn('Traceback', output)
                    self.assertEqual('complete', (root / 'cleanup.txt').read_text(encoding='utf-8'))
                    self.assertFalse((root / 'later-test.txt').exists())
                    result_path = next(line[8:] for line in process.stdout.splitlines() if line.startswith('Result: '))
                    payload = json.loads(Path(result_path).read_text(encoding='utf-8'))
                    self.assert_rows(payload, [('first', 'test', 'interrupted'), ('later', 'test', 'interrupted')])
                    self.assertEqual(130, payload['aggregateExit'])
                    row = payload['results'][0]
                    self.assertEqual(130, row['childExit'], row['reason'])
                    self.assertNotIn('not reaped', row['reason'])
                    self.assertLess(elapsed, cleanup_seconds, output)


        @unittest.skipUnless(hasattr(signal, 'pthread_sigmask'), 'SIGINT is held through pthread_sigmask, absent here')
        def test_held_blocks_sigint_only_when_it_set_the_block_and_defers_an_interrupt_until_the_call_returns(self):
            held = runpy.run_path(str(self.runner))['held']
            sigint = signal.SIGINT

            def interrupted(signum, frame):
                raise KeyboardInterrupt

            def raise_inside_call():
                masks.append(signal.pthread_sigmask(signal.SIG_BLOCK, ()))
                signal.raise_signal(sigint)
                masks.append(signal.pthread_sigmask(signal.SIG_BLOCK, ()))
                returned.append(True)
                return 'result'

            previous = signal.signal(sigint, interrupted)
            original = signal.pthread_sigmask(signal.SIG_BLOCK, ())
            try:
                signal.pthread_sigmask(signal.SIG_UNBLOCK, (sigint,))
                masks, returned = [], []
                with self.assertRaises(KeyboardInterrupt):
                    held(raise_inside_call)
                self.assertEqual([True], returned, 'the interrupt surfaced before the held call returned')
                self.assertTrue(all(sigint in mask for mask in masks), 'SIGINT was not blocked during the call')
                self.assertNotIn(sigint, signal.pthread_sigmask(signal.SIG_BLOCK, ()),
                                 'held left SIGINT blocked after it blocked it')
                self.assertEqual('result', held(lambda: 'result'))
                self.assertNotIn(sigint, signal.pthread_sigmask(signal.SIG_BLOCK, ()))

                # The caller's own block is not the facade's to lift.
                signal.pthread_sigmask(signal.SIG_BLOCK, (sigint,))
                masks, returned = [], []
                self.assertEqual('result', held(raise_inside_call))
                self.assertEqual([True], returned)
                self.assertIn(sigint, signal.pthread_sigmask(signal.SIG_BLOCK, ()),
                              'held unblocked a SIGINT block it did not set')
                self.assertEqual('again', held(lambda first, second: first + second, 'ag', 'ain'))
                self.assertIn(sigint, signal.pthread_sigmask(signal.SIG_BLOCK, ()))
            finally:
                # The interrupt raised under the caller's block is still pending: deliver it to a handler that drops it.
                signal.signal(sigint, lambda signum, frame: None)
                signal.pthread_sigmask(signal.SIG_SETMASK, original)
                signal.signal(sigint, previous)


        def test_absolute_row_deadline_cleans_cooperative_child_before_publication(self):
            data = manifest(stack('first'), stack('later'))
            root = self.fixture(data, execution_seconds=.15, cleanup_seconds=.25)
            (root / 'deadline.py').write_text(
                "import signal,sys,time\nfrom pathlib import Path\n"
                "def stop(signum, frame):\n    Path('cleanup.txt').write_text('complete')\n    raise SystemExit(130)\n"
                "signal.signal(signal.SIGBREAK if sys.platform == 'win32' else signal.SIGINT, stop)\n"
                "Path('ready.txt').write_text('ready')\n"
                "deadline=time.monotonic()+1\n"
                "while time.monotonic()<deadline:\n    print('wake', flush=True)\n    time.sleep(.005)\n"
                "raise SystemExit(99)\n", encoding='utf-8')
            data['stacks'][0]['capabilities']['test'] = configured(['-I', '-S', '-u', 'deadline.py'])
            (root / 'gap_check.json').write_text(json.dumps(data), encoding='utf-8')
            process, _, payload = self.invoke(['all'], directory=root)
            self.assert_exit(process, 130)
            self.assertEqual('ready', (root / 'ready.txt').read_text(encoding='utf-8'))
            self.assertEqual('complete', (root / 'cleanup.txt').read_text(encoding='utf-8'))
            self.assertFalse((root / 'later-test.txt').exists())
            self.assert_rows(payload, [('first', 'test', 'interrupted')])
            row = payload['results'][0]
            self.assertEqual(130, row['childExit'])
            self.assertIn('DYDO_ROW_DEADLINE', row['environment'])
            self.assertGreater(process.stdout.count('wake'), 5)


        def test_absolute_row_deadline_force_terminates_uncooperative_child(self):
            data = manifest(stack('first'), stack('later'))
            root = self.fixture(data, execution_seconds=.1, cleanup_seconds=.15)
            (root / 'deadline.py').write_text(
                "import signal,time\n"
                "signal.signal(signal.SIGINT, signal.SIG_IGN)\n"
                + ("signal.signal(signal.SIGBREAK, signal.SIG_IGN)\n" if os.name == 'nt' else '')
                + "deadline=time.monotonic()+1\n"
                "while time.monotonic()<deadline:\n    time.sleep(.005)\n"
                "raise SystemExit(99)\n", encoding='utf-8')
            data['stacks'][0]['capabilities']['test'] = configured(['-u', 'deadline.py'])
            (root / 'gap_check.json').write_text(json.dumps(data), encoding='utf-8')
            started = time.monotonic()
            process, _, payload = self.invoke(['all'], directory=root)
            self.assertLess(time.monotonic() - started, 2)
            self.assert_exit(process, 130)
            self.assertFalse((root / 'later-test.txt').exists())
            self.assert_rows(payload, [('first', 'test', 'interrupted')])
            self.assertIn('deadline', payload['results'][0]['reason'])


        def test_a_killed_child_that_is_never_reaped_releases_the_facade_within_the_reap_bound(self):
            namespace = runpy.run_path(str(self.runner))['stop_child'].__globals__
            for platform in ('linux', 'win32'):
                with self.subTest(platform=platform):
                    clock, children = FakeClock(), []
                    children.append(FakeChild(clock, reapable=False))
                    with (mock.patch.dict(namespace, {'time': clock}), mock.patch.object(sys, 'platform', platform),
                          mock.patch.object(signal, 'SIGKILL', getattr(signal, 'SIGKILL', 9), create=True),
                          mock.patch.object(signal, 'CTRL_BREAK_EVENT', getattr(signal, 'CTRL_BREAK_EVENT', 1), create=True),
                          mock.patch.object(os, 'killpg', fake_killpg(children), create=True)):
                        namespace['stop_child'](children[0], 30)
                    self.assertEqual(['kill'], children[0].signals[1:])
                    self.assertIsNone(children[0].returncode)
                    self.assertLessEqual(clock.now, 30 + namespace['REAP_SECONDS'])


        def test_a_deadline_row_whose_killed_child_is_never_reaped_stays_interrupted_without_a_child_exit(self):
            clock = FakeClock()
            rows, results, launched, namespace = self.fake_rows(clock, FakeChild(clock, reapable=False))
            self.assertEqual([('first', 'interrupted', None)], rows)
            self.assertEqual(1, len(launched))
            self.assertIn('deadline', results[0]['reason'])
            self.assertIn('not reaped', results[0]['reason'])
            self.assertLessEqual(clock.now, 1830 + namespace['REAP_SECONDS'])


        def test_an_interrupted_row_whose_killed_child_is_never_reaped_records_no_child_exit(self):
            clock = FakeClock()
            rows, results, launched, _ = self.fake_rows(clock, FakeChild(clock, reapable=False, interrupt=True))
            self.assertEqual([('first', 'interrupted', None)], rows)
            self.assertEqual(1, len(launched))
            self.assertIn('interrupted', results[0]['reason'])
            self.assertIn('not reaped', results[0]['reason'])


        def test_a_child_exit_first_observed_after_the_execution_deadline_interrupts_the_row(self):
            clock = FakeClock()
            rows, results, launched, _ = self.fake_rows(clock, FakeChild(clock, exits_at=1, stall=1801), FakeChild(clock, exits_at=1))
            self.assertEqual([('first', 'interrupted', 99)], rows)
            self.assertEqual(1, len(launched))
            self.assertEqual([], launched[0].signals)
            self.assertIn('deadline', results[0]['reason'])


        def test_a_child_exit_observed_before_the_execution_deadline_completes_the_row(self):
            clock = FakeClock()
            rows, _, launched, _ = self.fake_rows(clock, FakeChild(clock, exits_at=1799.9), FakeChild(clock, exits_at=1))
            self.assertEqual([('first', 'failed', 99), ('later', 'failed', 99)], rows)
            self.assertEqual([[], []], [child.signals for child in launched])


        def test_an_exit_racing_the_execution_deadline_resolves_to_the_deadline(self):
            # The facade observes the exit, then resumes past the deadline before it reads the clock:
            # it cannot show that the exit came first, so the row is deadline-exceeded.
            namespace = runpy.run_path(str(self.runner))['wait_for_child'].__globals__
            clock = FakeClock()
            child = FakeChild(clock, exits_at=0)
            observe = child.poll

            def late_poll():
                code = observe()
                clock.now = max(clock.now, 1800)
                return code
            child.poll = late_poll
            with mock.patch.dict(namespace, {'time': clock}):
                self.assertFalse(namespace['wait_for_child'](child, 'test', 1830))
            self.assertEqual(99, child.returncode)
            self.assertEqual([], child.signals)

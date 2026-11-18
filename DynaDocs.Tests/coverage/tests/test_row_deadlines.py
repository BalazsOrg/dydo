"""Measured Windows Python coverage keeps a finite, row-specific execution budget."""
import contextlib
import io
import runpy
import signal
import subprocess
import sys
import types
import unittest
from unittest import mock

from test_testing_facade_support import ROOT, RUNNER, PORTABLE, FakeClock, FakeChild, stack


class RowDeadlineTests(unittest.TestCase):
    def run_rows(self, runner, platform, name, capability, exits_at):
        namespace = runpy.run_path(str(runner))['execute_rows'].__globals__
        clock = FakeClock()
        child = FakeChild(clock, exits_at=exits_at)
        child.exit_code = 0
        later = FakeChild(clock, exits_at=0)
        later.exit_code = 0
        launch = mock.Mock(side_effect=[child, later])
        process = types.SimpleNamespace(Popen=launch, TimeoutExpired=subprocess.TimeoutExpired,
                                        CREATE_NEW_PROCESS_GROUP=0)
        with (mock.patch.dict(namespace, {'time': clock, 'subprocess': process}),
              mock.patch.object(sys, 'platform', platform),
              mock.patch.object(signal, 'CTRL_BREAK_EVENT', 1, create=True),
              mock.patch.dict(namespace, {
                  'prepare_row': mock.Mock(return_value=([sys.executable], [], 'configured', None)),
                  'artifact_error': mock.Mock(return_value=None),
              }), contextlib.redirect_stdout(io.StringIO())):
            rows = namespace['execute_rows']([stack(name), stack('later')], (capability,), ROOT, None, [])
        return rows, child, clock, launch

    def test_windows_python_coverage_can_finish_after_the_old_deadline(self):
        for runner in (RUNNER, PORTABLE):
            with self.subTest(runner=runner):
                rows, child, _, launch = self.run_rows(runner, 'win32', 'python', 'coverage', 2000)
                self.assertEqual(['passed', 'passed'], [row['state'] for row in rows])
                self.assertEqual([], child.signals)
                self.assertEqual(2, launch.call_count)
                self.assertEqual('2730.000000000', rows[0]['environment']['DYDO_ROW_DEADLINE'])

    def test_windows_python_coverage_hang_still_stops_later_rows(self):
        for runner in (RUNNER, PORTABLE):
            with self.subTest(runner=runner):
                rows, child, clock, launch = self.run_rows(runner, 'win32', 'python', 'coverage', None)
                self.assertEqual(['interrupted'], [row['state'] for row in rows])
                self.assertIn('row deadline exceeded after adapter cleanup', rows[0]['reason'])
                self.assertEqual([1, 'kill'], child.signals)
                self.assertGreaterEqual(clock.now, 2730)
                self.assertLessEqual(clock.now, 2731)
                self.assertEqual(1, launch.call_count)

    def test_other_rows_keep_the_original_deadline(self):
        for runner in (RUNNER, PORTABLE):
            for platform, name, capability in (
                    ('linux', 'python', 'coverage'), ('darwin', 'python', 'coverage'),
                    ('win32', 'dotnet', 'coverage'), ('win32', 'python', 'test'),
                    ('win32', 'python', 'static'), ('win32', 'python', 'mutation')):
                with self.subTest(runner=runner, platform=platform, stack=name, capability=capability):
                    # Exit before cancellation is needed on every platform; inspect the actual exported deadline.
                    rows, _, _, _ = self.run_rows(runner, platform, name, capability, 1)
                    self.assertEqual('1830.000000000', rows[0]['environment']['DYDO_ROW_DEADLINE'])

import contextlib
import io
import json
import os
import subprocess
import sys
import tempfile
import types
import unittest
from unittest import mock
import runpy
from pathlib import Path
from test_testing_facade_support import (configured, stack, manifest, fake_killpg)


class FacadeFixture(unittest.TestCase):
    def assert_caller_reporting(self, moment, handler):
        data = manifest(stack('first'), stack('later'))
        for item in data['stacks']:
            item['capabilities']['test'] = configured(['-c',
                "from pathlib import Path; p=Path('calls.txt'); "
                "p.write_text((p.read_text() if p.exists() else '') + " + repr(item['name'] + '\n') + ")"])
        root = self.fixture(data)
        (root / 'caller_interrupt.py').write_text(
            "import builtins,runpy,signal,sys\n"
            f"moment,handler={moment!r},{handler!r}\n"
            "report=builtins.print\n"
            "def caller_handler(signum, frame):\n"
            "    report('CALLER_HANDLER', flush=True)\n"
            "    if handler == 'raise': signal.default_int_handler(signum, frame)\n"
            "sig=signal.SIGBREAK if sys.platform == 'win32' else signal.SIGINT\n"
            "signal.signal(sig, signal.SIG_IGN if handler == 'ignore' else caller_handler)\n"
            "fired=False\n"
            "def reporting(*args, **kwargs):\n"
            "    global fired\n"
            "    if not fired and args and str(args[0]).startswith(moment):\n"
            "        fired=True\n"
            "        signal.raise_signal(sig)\n"
            "    return report(*args, **kwargs)\n"
            "builtins.print=reporting\n"
            "runpy.run_path('run_gap_check.py', run_name='__main__')\n", encoding='utf-8')
        process = subprocess.run([sys.executable, '-u', 'caller_interrupt.py', 'all'], cwd=root,
                                 capture_output=True, text=True, encoding='utf-8', timeout=60)
        expected_exit = 130 if handler == 'raise' else 0
        self.assert_exit(process, expected_exit)
        self.assertNotIn('Traceback', process.stderr)
        self.assertEqual(int(handler != 'ignore'), process.stdout.count('CALLER_HANDLER'))
        stopped_early = handler == 'raise' and moment == 'first test:'
        self.assertEqual(['first'] if stopped_early else ['first', 'later'],
                         (root / 'calls.txt').read_text().splitlines())
        payload = json.loads(next((root / 'results').glob('*/result.json')).read_text(encoding='utf-8'))
        self.assertEqual(expected_exit, payload['aggregateExit'])
        self.assert_rows(payload, [('first', 'test', 'passed'),
                                   ('later', 'test', 'interrupted' if stopped_early else 'passed')])


    def fake_rows(self, clock, *children):
        """The runner's own execute_rows over the test rows of `first` and `later` with the
        production deadlines on `clock`, launching `children` in turn instead of processes."""
        namespace = runpy.run_path(str(self.runner))['execute_rows'].__globals__
        launched = []

        def launch(*_args, **_kwargs):
            child = children[len(launched)]
            launched.append(child)
            clock.now += child.stall
            return child
        launcher = types.SimpleNamespace(Popen=launch, TimeoutExpired=subprocess.TimeoutExpired,
                                         CREATE_NEW_PROCESS_GROUP=getattr(subprocess, 'CREATE_NEW_PROCESS_GROUP', 0))
        temporary = tempfile.TemporaryDirectory(prefix='dydo-facade-')
        self.addCleanup(temporary.cleanup)
        with (mock.patch.dict(namespace, {'time': clock, 'subprocess': launcher}),
              mock.patch.object(os, 'killpg', fake_killpg(launched), create=True),
              contextlib.redirect_stdout(io.StringIO())):
            rows = namespace['execute_rows']([stack('first'), stack('later')], ('test',),
                                             Path(temporary.name).resolve(), None, [])
        return [(row['stack'], row['state'], row['childExit']) for row in rows], rows, launched, namespace

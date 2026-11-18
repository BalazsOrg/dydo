import json
import subprocess
import sys
import unittest
from test_testing_facade_support import (unavailable, configured, stack, manifest, coverage_report, verdict_stack, ROOT)


class FacadeFixture(unittest.TestCase):
    def suite_verdict_defects(self):
        bad = verdict_stack('bad', report=coverage_report('bad-coverage', 0), coverage_exit=0)
        bad['capabilities']['test']['suiteVerdict'] = {'exit': ['a'], 'failure': ['b', {}]}
        p, _, payload = self.invoke(['all'], manifest(bad, stack('peer')))
        self.assert_exit(p, 2)
        self.assertEqual('invalid', payload['results'][0]['state'])
        self.assertIn('invalid suite verdict declaration', payload['results'][0]['reason'])
        declaration = lambda item: item['capabilities']['coverage']['suiteVerdict']
        defects = [
            ('extra declaration key', lambda item: declaration(item).update(extra=True)),
            ('nonstring exit item', lambda item: declaration(item).update(exit=['collectors', 1])),
            ('empty exit path', lambda item: declaration(item).update(exit=[])),
            ('short failure path', lambda item: declaration(item).update(failure=['only'])),
            ('nonstring failure container', lambda item: declaration(item).update(failure=[1, {'gate': 'functional'}])),
            ('non-object matcher', lambda item: declaration(item).update(failure=['collectors', 'bad-coverage', 'findings', ['nope']])),
            ('empty matcher', lambda item: declaration(item).update(failure=['collectors', 'bad-coverage', 'findings', {}])),
            ('complex matcher value', lambda item: declaration(item).update(failure=['collectors', 'bad-coverage', 'findings', {'gate': []}])),
            ('two required artifacts', lambda item: item['capabilities']['coverage'].update(artifacts=[{'path': 'a', 'required': True}, {'path': 'b', 'required': True}])),
        ]
        for name, change in defects:
            with self.subTest(defect=name):
                item = verdict_stack('bad', report=coverage_report('bad-coverage', 0), coverage_exit=0)
                change(item)
                p, _, payload = self.invoke(['gate', 'coverage', '--stack', 'bad'], manifest(item, stack('peer')))
                self.assert_exit(p, 2)
                self.assertEqual('invalid', payload['results'][0]['state'])
                self.assertIn('invalid suite verdict declaration', payload['results'][0]['reason'])


    def row_defect(self, problem):
        bad = stack('bad')
        if problem == 'capability': del bad['capabilities']['coverage']
        elif problem == 'unavailable': bad['capabilities']['test'] = unavailable('missing tests')
        elif problem == 'empty': bad['capabilities']['test']['command']['argv'] = []
        elif problem == 'placeholder': bad['capabilities']['test']['command']['argv'].append('<project-path>')
        elif problem == 'executable': bad['capabilities']['test'] = configured(['dydo-certainly-nonexistent-executable-113'], kind='argv')
        elif problem == 'evidence': bad['isolation']['evidence'] = {'state': 'verified', 'kind': 'not-direct'}
        elif problem == 'escape': bad['cwd'] = '../escape'
        p, root, payload = self.invoke(['all'], manifest(bad, stack('peer')))
        self.assert_exit(p, 2)
        self.assert_rows(payload, [('bad', 'test', 'unavailable' if problem == 'unavailable' else 'invalid'), ('peer', 'test', 'passed')])
        self.assertEqual(['peer-test.txt'], [x.name for x in root.glob('*.txt')])
        if problem == 'escape':
            bad = stack('bad'); bad['capabilities']['test']['artifacts'][0]['path'] = '../escape'
            p, _, payload = self.invoke(['all'], manifest(bad, stack('peer')))
            self.assert_exit(p, 2); self.assertEqual('invalid', payload['results'][0]['state'])
            self.assertEqual('passed', payload['results'][1]['state'])


    def portable_data(self):
        return json.loads((ROOT / 'dydo/reference/gap-check.example.json').read_text())


    def partial(self, targeted):
        data = self.portable_data()
        data['stacks'][1] = stack('frontend')
        data['stacks'][1]['isolation'] = {'requirement': 'per-run-artifacts', 'evidence': {'state': 'verified', 'kind': 'adapter', 'path': 'gap_check.py'}}
        p, root, payload = self.invoke(['test', '--stack', 'frontend'] if targeted else ['all'], data)
        self.assert_exit(p, 0 if targeted else 2)
        self.assertEqual(['frontend-test.txt'], [x.name for x in root.glob('*.txt')])
        self.assertEqual(['passed'] if targeted else ['unavailable', 'passed', 'invalid'], [r['state'] for r in payload['results']])


    def assert_launch_interrupt(self, custom):
        data = manifest(stack('first'), stack('later'))
        data['stacks'][0]['capabilities']['test'] = configured(['-u', 'adapter.py'])
        root = self.fixture(data)
        (root / 'adapter.py').write_text(
            "import signal,sys,time\nfrom pathlib import Path\n"
            "def interrupted(signum, frame):\n    raise KeyboardInterrupt\n"
            "signal.signal(signal.SIGBREAK if sys.platform == 'win32' else signal.SIGINT, interrupted)\n"
            "mask = signal.pthread_sigmask(signal.SIG_BLOCK, ()) if hasattr(signal, 'pthread_sigmask') else ()\n"
            "try:\n    deadline=time.monotonic()+30\n"
            "    print('blocked' if signal.SIGINT in mask else 'ready', flush=True)\n"
            "    while time.monotonic()<deadline:\n        time.sleep(.05)\n"
            "except KeyboardInterrupt:\n    Path('cleanup.txt').write_text('complete')\n    sys.exit(130)\n"
            "sys.exit(99)\n",
            encoding='utf-8')
        (root / 'launch_interrupt.py').write_text(
            "import runpy,signal,subprocess,sys\nfrom pathlib import Path\n"
            f"custom={custom!r}\n"
            "def caller_handler(signum, frame):\n"
            "    with open('handler-calls.txt', 'a') as calls: calls.write('called\\n')\n"
            "    signal.default_int_handler(signum, frame)\n"
            "if custom:\n"
            "    signal.signal(signal.SIGINT, caller_handler)\n"
            "    if hasattr(signal, 'SIGBREAK'): signal.signal(signal.SIGBREAK, caller_handler)\n"
            "children=[]\n"
            "class InterruptedPopen(subprocess.Popen):\n"
            "    def __init__(self, args, *rest, **options):\n"
            "        adapter = 'adapter.py' in args\n"
            "        if adapter:\n            options.update(stdout=subprocess.PIPE, text=True)\n"
            "        super().__init__(args, *rest, **options)\n"
            "        if adapter:\n"
            "            children.append(self)\n"
            "            print('CHILD_MASK:', self.stdout.readline().strip(), flush=True)\n"
            "            signal.raise_signal(signal.SIGBREAK if sys.platform == 'win32' else signal.SIGINT)\n"
            "            signal.raise_signal(signal.SIGBREAK if sys.platform == 'win32' else signal.SIGINT)\n"
            "subprocess.Popen=InterruptedPopen\n"
            "try:\n    runpy.run_path('run_gap_check.py', run_name='__main__')\n"
            "finally:\n"
            "    for child in children:\n"
            "        if child.poll() is None:\n"
            "            Path('orphan.txt').write_text('adapter escaped facade cleanup')\n"
            "            child.kill()\n"
            "        child.wait()\n",
            encoding='utf-8')
        process = subprocess.run([sys.executable, '-u', 'launch_interrupt.py', 'all'], cwd=root,
                                 capture_output=True, text=True, encoding='utf-8', timeout=60)
        self.assert_exit(process, 130)
        self.assertIn('CHILD_MASK: ready', process.stdout)
        self.assertFalse((root / 'orphan.txt').exists(), 'launched adapter escaped facade cleanup')
        self.assertEqual('complete', (root / 'cleanup.txt').read_text(encoding='utf-8'))
        payload = json.loads(next((root / 'results').glob('*/result.json')).read_text(encoding='utf-8'))
        self.assertEqual(130, payload['results'][0]['childExit'])
        self.assertEqual(130, payload['aggregateExit'])
        self.assert_rows(payload, [('first', 'test', 'interrupted'), ('later', 'test', 'interrupted')])
        self.assertFalse((root / 'later-test.txt').exists())
        if custom:
            self.assertEqual(['called'], (root / 'handler-calls.txt').read_text().splitlines())


    def assert_final_return_interrupt(self, target, event, custom):
        root = self.fixture(manifest(stack('first'), stack('later')))
        (root / 'return_interrupt.py').write_text(
            "import dis,runpy,signal,sys\n"
            f"target,event_kind,custom={target!r},{event!r},{custom!r}\n"
            "def caller_handler(signum, frame):\n"
            "    print('CALLER_HANDLER', flush=True)\n"
            "    signal.default_int_handler(signum, frame)\n"
            "if custom:\n"
            "    signal.signal(signal.SIGINT, caller_handler)\n"
            "    if hasattr(signal, 'SIGBREAK'): signal.signal(signal.SIGBREAK, caller_handler)\n"
            "fired=False\n"
            "def trace(frame, event, arg):\n"
            "    global fired\n"
            f"    matching=frame.f_code.co_name == target and frame.f_code.co_filename == {str(self.runner)!r}\n"
            "    if matching and event_kind == 'opcode': frame.f_trace_opcodes=True\n"
            "    boundary=event == 'return' if event_kind == 'return' else event == 'opcode' and dis.opname[frame.f_code.co_code[frame.f_lasti]] in ('RETURN_VALUE', 'RETURN_CONST')\n"
            "    if not fired and matching and boundary:\n"
            "        fired=True\n"
            "        print('FIRST_INTERRUPT_AT_RETURN', flush=True)\n"
            "        signal.raise_signal(signal.SIGBREAK if sys.platform == 'win32' else signal.SIGINT)\n"
            "    return trace\n"
            "sys.settrace(trace)\n"
            "runpy.run_path('run_gap_check.py', run_name='__main__')\n", encoding='utf-8')
        process = subprocess.run([sys.executable, '-u', 'return_interrupt.py', 'all'], cwd=root,
                                 capture_output=True, text=True, encoding='utf-8', timeout=60)
        self.assert_exit(process, 130)
        self.assertIn('FIRST_INTERRUPT_AT_RETURN', process.stdout)
        self.assertEqual(int(custom), process.stdout.count('CALLER_HANDLER'))
        self.assertNotIn('Traceback', process.stderr)
        payload = json.loads(next((root / 'results').glob('*/result.json')).read_text(encoding='utf-8'))
        self.assertEqual(130, payload['aggregateExit'])
        self.assert_rows(payload, [('first', 'test', 'passed'), ('later', 'test', 'passed')])

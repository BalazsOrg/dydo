"""Behavioral probes shared by unittest and exact Reqnroll scenario bindings."""
import json
import queue
import shutil
import signal
import subprocess
import sys
import tempfile
import time
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
RUNNER = ROOT / 'DynaDocs.Tests/coverage/gap_check.py'
PORTABLE = ROOT / 'dydo/reference/gap-check.example.py'
CAPABILITIES = ('test', 'static', 'coverage', 'mutation')
# A harness that drives run_tests.main() puts the adapter's directory first on sys.path, as running
# run_tests.py as a script does, so the adapter's sibling imports resolve.
SCRIPT_PATH = f"import sys; sys.path.insert(0, {str(ROOT / 'DynaDocs.Tests/coverage')!r})\n"


def has_worktree_identity(listing, path):
    prefix = 'worktree '
    identity = path.resolve()
    return any(Path(line[len(prefix):]).resolve() == identity
               for line in listing.splitlines() if line.startswith(prefix))


def first_adapter_line(lines):
    """The first queued line, past the facade's one memory-cap warning: a host without systemd
    (Windows, macOS, a runner with no user manager) prints that warning before any adapter output."""
    line = lines.get(timeout=15)
    return lines.get(timeout=15) if line.startswith('memory cap unavailable: ') else line


def next_line(lines, matches, seconds):
    """The first queued line that matches, or None once `seconds` pass without one."""
    deadline = time.monotonic() + seconds
    while time.monotonic() < deadline:
        try:
            line = lines.get(timeout=1)
        except queue.Empty:
            continue
        if matches(line):
            return line
    return None


def unavailable(reason='not adopted'):
    return {'state': 'unavailable', 'reason': reason}


def configured(argv, artifacts=None, kind='current-python'):
    return {'state': 'configured', 'command': {'kind': kind, 'argv': argv}, 'artifacts': artifacts or []}


def stack(name):
    caps = {}
    for capability in CAPABILITIES:
        artifact = f'{name}-{capability}.txt'
        code = "import json,sys; from pathlib import Path; " + f"Path('{artifact}').write_text('produced'); " + f"print('{name}:{capability}'); print(json.dumps(sys.argv[1:],ensure_ascii=False))"
        argv = ['-c', code] + (['{base}'] if capability == 'mutation' else [])
        caps[capability] = configured(argv, [{'path': artifact, 'required': True}])
    return {'name': name, 'kind': 'fixture', 'cwd': '.', 'isolation': {'requirement': 'in-place', 'evidence': {'state': 'verified', 'kind': 'direct'}}, 'capabilities': caps}


def manifest(*stacks):
    return {'schema': 1, 'artifactRoot': 'results', 'stacks': list(stacks or [stack('dotnet')])}


def coverage_report(collector, child_exit=None, findings=None, errors=None):
    row = {'facts': {}, 'findings': list(findings or [])}
    if child_exit is not None:
        row['facts']['child_exit'] = child_exit
    if errors is not None:
        row['errors'] = errors
    return {'collectors': {collector: row}}


def functional(child_exit):
    return {'gate': 'functional', 'child_exit': child_exit}


def verdict_stack(name='first', collector=None, report=None, coverage_exit=0, declared=True):
    collector = collector or f'{name}-coverage'
    item = stack(name)
    artifact = f'{name}-coverage.json'
    payload = json.dumps(report if report is not None else coverage_report(collector, 0))
    code = ("from pathlib import Path; "
            f"Path({artifact!r}).write_text({payload!r}); "
            f"print({name + ':coverage'!r}); raise SystemExit({coverage_exit})")
    item['capabilities']['coverage']['command'] = {'kind': 'current-python', 'argv': ['-c', code]}
    item['capabilities']['coverage']['artifacts'] = [{'path': artifact, 'required': True}]
    if declared:
        item['capabilities']['coverage']['suiteVerdict'] = {
            'exit': ['collectors', collector, 'facts', 'child_exit'],
            'failure': ['collectors', collector, 'findings', {'gate': 'functional'}]}
    return item


class UnboundedWait(AssertionError):
    """An untimed wait on a child that is never reaped: on a real host the facade would hang forever."""


class FakeClock:
    def __init__(self):
        self.now = 0.0

    def monotonic(self):
        return self.now


class FakeChild:
    """A Popen stand-in on a FakeClock. It ignores the interrupt, exits by itself with 99 at
    `exits_at` (never when None) and, once killed, is reaped only when `reapable`. The facade
    resumes `stall` seconds late after launching it; `interrupt` raises one KeyboardInterrupt
    from its first wait."""
    pid = 4242

    def __init__(self, clock, exits_at=None, reapable=True, stall=0, interrupt=False):
        self.clock, self.exits_at, self.reapable, self.stall, self.interrupt = clock, exits_at, reapable, stall, interrupt
        self.exit_code, self.returncode, self.signals = 99, None, []

    def poll(self):
        if self.returncode is None and self.exits_at is not None and self.clock.now >= self.exits_at:
            self.returncode = self.exit_code
        return self.returncode

    def wait(self, timeout=None):
        if self.interrupt:
            self.interrupt = False
            raise KeyboardInterrupt
        if self.poll() is None:
            if self.exits_at is not None and (timeout is None or self.clock.now + timeout >= self.exits_at):
                self.clock.now = self.exits_at
            elif timeout is None:
                raise UnboundedWait('untimed wait on a killed child that is never reaped')
            else:
                self.clock.now += timeout
                raise subprocess.TimeoutExpired('fake-child', timeout)
        return self.poll()

    def send_signal(self, sig):
        self.signals.append(sig)

    def kill(self):
        self.signals.append('kill')
        self.exit_code, self.exits_at = -9, self.clock.now if self.reapable else None


def fake_killpg(children):
    """os.killpg for FakeChild stand-ins: SIGKILL kills the latest child, any other signal is recorded."""
    def killpg(_pid, sig):
        if sig == signal.SIGKILL:
            children[-1].kill()
        else:
            children[-1].send_signal(sig)
    return killpg




class FacadeFixture(unittest.TestCase):
    runner = RUNNER

    def policy_document(self, path):
        hub = ROOT / path
        pages = [hub]
        if path == 'dydo/reference/coverage-tools.md':
            suffixes = ['coverage', 'measurement', 'platforms', 'provisioning', 'results', 'running', 'statics']
            bodies = [hub.with_name(f'coverage-tools-{suffix}.md') for suffix in suffixes]
            self.assertEqual(bodies, sorted(hub.parent.glob('coverage-tools-*.md')))
            self.assertTrue(bodies, path)
            pages.extend(bodies)
        contents = []
        for page in pages:
            text = page.read_text(encoding='utf-8')
            self.assertTrue(text.strip(), str(page))
            contents.append(text)
        return '\n'.join(contents)

    def fixture(self, data=None, execution_seconds=None, cleanup_seconds=None, noncanonical_root=False):
        temporary = tempfile.TemporaryDirectory(prefix='dydo-facade-')
        self.addCleanup(temporary.cleanup)
        directory = Path(temporary.name)
        # Pin the facade root here: neither a stray marker nor a repository above may claim it.
        subprocess.run(['git', 'init', '--quiet', str(directory)], check=True)
        shutil.copyfile(self.runner, directory / 'gap_check.py')
        # Both legs run through the same launcher: the derived copy is byte-identical, so its
        # deadline policy is the project's and every case must hold for it too.
        module_file = directory / 'gap_check.py'
        if noncanonical_root:
            (directory / 'alias').mkdir()
            module_file = directory / 'alias/../gap_check.py'
        launcher = [
            'import importlib.util, sys',
            # The derived runner lives outside any ignored tree: caching its bytecode there would
            # make the candidate dirty and put an untracked artifact in the inventory.
            'sys.dont_write_bytecode = True',
            f'spec=importlib.util.spec_from_file_location("gap_check", {str(self.runner)!r})',
            'module=importlib.util.module_from_spec(spec); spec.loader.exec_module(module)',
            f'module.__file__={str(module_file)!r}',
        ]
        if execution_seconds is not None:
            launcher.append(f'module.EXECUTION_SECONDS_MAXIMUM={execution_seconds!r}')
        if cleanup_seconds is not None:
            launcher.append(f'module.CLEANUP_SECONDS={cleanup_seconds!r}')
        launcher.append('raise SystemExit(module.main())')
        (directory / 'run_gap_check.py').write_text('\n'.join(launcher) + '\n', encoding='utf-8')
        (directory / 'gap_check.json').write_text(json.dumps(data or manifest()), encoding='utf-8')
        return directory


    def invoke(self, args, data=None, directory=None):
        directory = directory or self.fixture(data)
        runner = directory / 'run_gap_check.py'
        process = subprocess.run([sys.executable, '-u', str(runner), *args], cwd=directory,
                                 capture_output=True, text=True, encoding='utf-8', timeout=60)
        result_paths = [line[8:] for line in process.stdout.splitlines() if line.startswith('Result: ')]
        payload = json.loads(Path(result_paths[0]).read_text(encoding='utf-8')) if result_paths else None
        return process, directory, payload


    def assert_exit(self, process, expected):
        self.assertEqual(expected, process.returncode, process.stdout + process.stderr)


    def assert_rows(self, payload, expected):
        self.assertEqual(expected, [(row['stack'], row['capability'], row['state']) for row in payload['results']])


    def assert_interrupted_selection(self, payload, operation):
        capabilities = ('test',) if operation == 'all' else ('test', 'static', 'coverage')
        self.assert_rows(payload, [(name, capability, 'interrupted') for name in ('first', 'later')
                                   for capability in capabilities])


    def gate(self, capability):
        args = ['gate', capability, '--stack', 'dotnet'] + (['--since', 'BASE'] if capability == 'mutation' else [])
        p, root, payload = self.invoke(args)
        self.assert_exit(p, 0)
        self.assert_rows(payload, [('dotnet', capability, 'passed')])
        self.assertEqual([f'dotnet-{capability}.txt'], [x.name for x in root.glob('*.txt')])
        self.assert_payload(payload, root)


    def mutation_invalid(self, tokens):
        bad, peer = stack('dotnet'), stack('peer')
        bad['capabilities']['mutation']['command']['argv'][-1:] = tokens
        p, root, payload = self.invoke(['gate', 'mutation', '--since', 'BASE'], manifest(bad, peer))
        self.assert_exit(p, 2)
        self.assert_rows(payload, [('dotnet', 'mutation', 'invalid'), ('peer', 'mutation', 'passed')])
        self.assertFalse((root / 'dotnet-mutation.txt').exists())
        self.assertIn('exactly one argv element equal to {base} and no substring occurrence', payload['results'][0]['reason'])


    def derived_case(self, report, coverage_exit):
        item = verdict_stack(report=report, coverage_exit=coverage_exit)
        return self.invoke(['--force-run'], manifest(item))


    def assert_derived(self, payload, test, coverage, aggregate):
        row = payload['results'][0]
        self.assertEqual(test, tuple(row[key] for key in ('state', 'childExit', 'resultExit')))
        self.assertEqual(coverage, tuple(payload['results'][2][key] for key in ('state', 'resultExit')))
        self.assertEqual(aggregate, payload['aggregateExit'])


    def globally_invalid(self, problem):
        data, args, diagnostic = manifest(), ['all'], ''
        if problem == 'json': diagnostic = 'invalid JSON'
        elif problem == 'schema': data['schema'] = 2; diagnostic = 'schema'
        elif problem == 'field': del data['artifactRoot']; diagnostic = 'artifactRoot'
        elif problem == 'stacks': data['stacks'] = {}; diagnostic = 'stacks'
        elif problem == 'duplicate': data['stacks'].append(stack('dotnet')); diagnostic = 'duplicate'
        elif problem == 'unknown': args = ['all', '--stack', 'missing']; diagnostic = 'unknown selected stack'
        elif problem == 'syntax': args = ['all', '--bad']; diagnostic = 'syntax'
        elif problem == 'base': args = ['gate', 'mutation', '--stack', 'dotnet']; diagnostic = '--since BASE'
        root = self.fixture(data)
        if problem == 'json': (root / 'gap_check.json').write_text('{broken')
        p, root, payload = self.invoke(args, directory=root)
        self.assert_exit(p, 2)
        self.assertEqual([], list(root.glob('*.txt')))
        self.assertIsNone(payload)
        self.assertIn(diagnostic, p.stderr)


    def exit_case(self, code):
        data = manifest()
        child_exit = {0: 0, 1: 17}[code] if code != 2 else None
        data['stacks'][0]['capabilities']['test'] = (unavailable() if code == 2
                                                      else configured(['-c', f'raise SystemExit({child_exit})']))
        p, _, payload = self.invoke(['all'], data)
        self.assert_exit(p, code)
        self.assertEqual(code, payload['aggregateExit'])
        self.assertEqual(child_exit, payload['results'][0]['childExit'])


    def assert_payload(self, payload, root):
        self.assertEqual({'schema', 'candidate', 'memoryCap', 'operation', 'selectedStacks', 'results', 'aggregateExit'}, set(payload))
        self.assertIs(type(payload['schema']), int)
        self.assertEqual(1, payload['schema'])
        self.assertEqual({'commit', 'dirty'}, set(payload['candidate']))
        self.assertIsInstance(payload['candidate']['dirty'], bool)
        self.assertIn('name', payload['operation'])
        for row in payload['results']:
            fields = {'stack', 'capability', 'state', 'argv', 'cwd', 'isolation',
                      'environment', 'childExit', 'resultExit', 'artifacts'}
            if sys.platform in ('linux', 'win32'):
                fields.add('memoryCapHitReporting')
                self.assertEqual('best-effort' if sys.platform == 'win32' else 'exact', row['memoryCapHitReporting'])
            if sys.platform == 'win32':
                fields.add('peakJobMemory')
                if payload['memoryCap']['state'] == 'enforced':
                    self.assertIsInstance(row['peakJobMemory'], int)
            self.assertEqual(fields, set(row) - {'reason'})
            self.assertIn(row['state'], ['passed', 'failed', 'unavailable', 'invalid', 'interrupted'])
            self.assertIn(row['resultExit'], [0, 1, 2, 130])
            self.assertTrue(row['childExit'] is None or type(row['childExit']) is int)
            self.assertEqual('.', row['cwd'])
            self.assertEqual({'requirement': 'in-place', 'evidence': {'state': 'verified', 'kind': 'direct'}}, row['isolation'])
            for item in row['artifacts']:
                self.assertEqual({'path', 'required'}, set(item))
                self.assertIsInstance(item['required'], bool)
                self.assertTrue((root / item['path']).exists())


    def schema_rows(self):
        defects = [
            ('extra stack field', lambda s: s.update(extra=True)),
            ('extra capability', lambda s: s['capabilities'].update(extra=unavailable())),
            ('configured reason', lambda s: s['capabilities']['test'].update(reason='forbidden')),
            ('unknown command kind', lambda s: s['capabilities']['test']['command'].update(kind='shell')),
            ('nonstring argv', lambda s: s['capabilities']['test']['command'].update(argv=[12])),
            ('nonboolean artifact', lambda s: s['capabilities']['test'].update(artifacts=[{'path': 'x', 'required': 'true'}])),
            ('absolute adapter', lambda s: s.update(isolation={'requirement': 'per-run-artifacts', 'evidence': {'state': 'verified', 'kind': 'adapter', 'path': str(RUNNER)}})),
            ('wrong direct kind', lambda s: s['isolation']['evidence'].update(kind='adapter')),
            ('unknown isolation', lambda s: s['isolation'].update(requirement='magic')),
            ('nonstring isolation', lambda s: s['isolation'].update(requirement=[])),
            ('nonstring command kind', lambda s: s['capabilities']['test']['command'].update(kind=[])),
            ('nonstring capability state', lambda s: s['capabilities']['test'].update(state=[])),
            ('unavailable with command', lambda s: s['capabilities'].update(test={'state': 'unavailable', 'reason': 'missing', 'command': {}})),
        ]
        for name, change in defects:
            with self.subTest(defect=name):
                bad = stack('bad'); change(bad)
                p, root, payload = self.invoke(['all'], manifest(bad, stack('peer')))
                self.assert_exit(p, 2)
                self.assertEqual('invalid', payload['results'][0]['state'])
                self.assertEqual('passed', payload['results'][1]['state'])
                self.assertFalse((root / 'bad-test.txt').exists())
        data = manifest(); data['stacks'][0]['isolation']['evidence'] = {'state': 'unavailable', 'reason': 'adopt isolation'}
        p, root, payload = self.invoke(['all'], data)
        self.assert_exit(p, 2); self.assertEqual('unavailable', payload['results'][0]['state'])
        self.assertEqual('adopt isolation', payload['results'][0]['reason'])
        self.assertEqual([], list(root.glob('*.txt')))
        data = manifest(); data['stacks'][0]['capabilities']['static']['artifacts'] = []
        p, _, payload = self.invoke(['gate', 'static'], data)
        self.assert_exit(p, 2); self.assertIn('required artifact', payload['results'][0]['reason'])
        self.suite_verdict_defects()
        self.test_required_gate_artifact_is_required_after_successful_child_exit()

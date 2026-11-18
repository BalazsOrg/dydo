"""Inherited facade cases; collected through the two public facade classes."""
import json
import os
import queue
import signal
import subprocess
import sys
import threading
import unittest
from unittest import mock
import runpy
from pathlib import Path
from test_testing_facade_support import (first_adapter_line, unavailable, configured, stack, manifest, coverage_report, verdict_stack, ROOT, RUNNER, PORTABLE)


class Cases:
    """Only the public facade classes collect these inherited unittest cases."""

    class FacadeCases03(unittest.TestCase):
        def test_row_suite_verdict(self):
            bad = verdict_stack('bad', report=coverage_report('bad-coverage', 0), coverage_exit=0)
            bad['capabilities']['coverage']['suiteVerdict'] = {'exit': ['collectors', 'bad-coverage', 'facts', 'child_exit']}
            p, root, payload = self.invoke(['--force-run'], manifest(bad, stack('peer')))
            self.assert_exit(p, 2)
            self.assert_rows(payload, [('bad', 'test', 'passed'), ('bad', 'static', 'passed'), ('bad', 'coverage', 'invalid'), ('peer', 'test', 'passed'), ('peer', 'static', 'passed'), ('peer', 'coverage', 'passed')])
            self.assertIn('invalid suite verdict declaration', payload['results'][2]['reason'])
            self.assertEqual(['bad-static.txt', 'bad-test.txt', 'peer-coverage.txt', 'peer-static.txt', 'peer-test.txt'], sorted(x.name for x in root.glob('*.txt')))


        def test_deferred_test_row_is_interrupted_when_coverage_never_runs(self):
            item = verdict_stack(report=coverage_report('first-coverage', 0), coverage_exit=0)
            data = manifest(item)
            root = self.fixture(data)
            (root / 'wait.py').write_text(
                "import signal,sys,time\nfrom pathlib import Path\n"
                "def interrupted(signum, frame):\n    raise KeyboardInterrupt\n"
                "signal.signal(signal.SIGBREAK if sys.platform == 'win32' else signal.SIGINT, interrupted)\n"
                "try:\n    print('WAITING_FOR_INTERRUPT', flush=True)\n    deadline=time.monotonic()+60\n"
                "    while time.monotonic()<deadline:\n        time.sleep(0.05)\n"
                "except KeyboardInterrupt:\n    print('ADAPTER_INTERRUPTED', flush=True)\n    sys.exit(130)\n"
                "finally:\n    Path('cleanup.txt').write_text('complete')\n", encoding='utf-8')
            data['stacks'][0]['capabilities']['static'] = configured(['-u', 'wait.py'], [{'path': 'first-static.txt', 'required': True}])
            (root / 'gap_check.json').write_text(json.dumps(data), encoding='utf-8')
            facade = subprocess.Popen([sys.executable, '-u', str(root / 'gap_check.py'), '--force-run'],
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
                facade.send_signal(signal.CTRL_BREAK_EVENT if os.name == 'nt' else signal.SIGINT)
                facade.wait(timeout=35); reader.join(timeout=5)
                self.assertEqual(130, facade.returncode, ''.join(output))
                self.assertEqual('complete', (root / 'cleanup.txt').read_text())
                self.assertFalse((root / 'first-coverage.json').exists())
                result_path = next(line.strip()[8:] for line in output if line.startswith('Result: '))
                payload = json.loads(Path(result_path).read_text(encoding='utf-8'))
                self.assert_rows(payload, [('first', 'test', 'interrupted'), ('first', 'static', 'interrupted'),
                                           ('first', 'coverage', 'interrupted')])
                self.assertEqual(130, payload['aggregateExit'])
                row = payload['results'][0]
                self.assertEqual(('interrupted', None, 130), tuple(row[key] for key in ('state', 'childExit', 'resultExit')))
                self.assertIn('interrupted before the coverage row', row['reason'])
            finally:
                if facade.poll() is None:
                    facade.send_signal(signal.CTRL_BREAK_EVENT if os.name == 'nt' else signal.SIGINT)
                    facade.wait(timeout=35)
                reader.join(timeout=5)
                facade.stdout.close()


        def test_aggregation(self):
            good, failed, bad, missing = (stack(n) for n in ['good', 'failed', 'bad', 'missing'])
            failed['capabilities']['test'] = configured(['-c', "print('FAIL_RAN'); raise SystemExit(17)"])
            bad['capabilities']['test']['command']['argv'] = []
            missing['capabilities']['test'] = unavailable()
            p, _, payload = self.invoke(['all'], manifest(good, failed, bad, missing))
            self.assert_exit(p, 2)
            self.assert_rows(payload, [('good', 'test', 'passed'), ('failed', 'test', 'failed'), ('bad', 'test', 'invalid'), ('missing', 'test', 'unavailable')])
            self.assertEqual([0, 17, None, None], [r['childExit'] for r in payload['results']])
            self.assertEqual([0, 1, 2, 2], [r['resultExit'] for r in payload['results']])
            self.assertEqual(2, payload['aggregateExit'])
            self.assertIn('FAIL_RAN', p.stdout)
            self.assertLess(p.stdout.index('missing test:'), p.stdout.index('Aggregate:'))
            self.assertLess(p.stdout.index('Aggregate:'), p.stdout.index('Result:'))


        def test_force_run_unavailability_outranks_a_raw_test_exit(self):
            data = manifest()
            caps = data['stacks'][0]['capabilities']
            caps['test'] = configured(['-c', 'raise SystemExit(17)'])
            caps['static'] = unavailable('Pending DYD-96')
            caps['coverage'] = unavailable('Pending DYD-96')
            p, _, payload = self.invoke(['--force-run'], data)
            self.assert_exit(p, 2)
            self.assertEqual(2, payload['aggregateExit'])
            self.assertEqual(('failed', 17, 1), tuple(payload['results'][0][k] for k in ('state', 'childExit', 'resultExit')))
            self.assertEqual(('unavailable', 2), tuple(payload['results'][1][k] for k in ('state', 'resultExit')))


        def test_global_json(self): self.globally_invalid('json')


        def test_global_schema(self): self.globally_invalid('schema')


        def test_global_field(self): self.globally_invalid('field')


        def test_global_stacks(self): self.globally_invalid('stacks')


        def test_global_duplicate(self): self.globally_invalid('duplicate')


        def test_global_unknown(self): self.globally_invalid('unknown')


        def test_global_syntax(self): self.globally_invalid('syntax')


        def test_global_base(self): self.globally_invalid('base')


        def test_exit_pass(self): self.exit_case(0)


        def test_exit_failure(self): self.exit_case(1)


        def test_exit_unavailable(self): self.exit_case(2)


        def test_result_artifact(self):
            root = self.fixture()
            subprocess.run(['git', '-c', 'user.name=Facade Test', '-c', 'user.email=facade@example.invalid', 'commit', '--allow-empty', '-qm', 'fixture'], cwd=root, check=True, capture_output=True)
            expected = subprocess.run(['git', 'rev-parse', 'HEAD'], cwd=root, check=True, capture_output=True, text=True).stdout.strip()
            for expected_exit in [0, 1, 2]:
                data = manifest()
                child_exit = {0: 0, 1: 17}.get(expected_exit)
                data['stacks'][0]['capabilities']['test'] = (unavailable() if expected_exit == 2
                                                              else configured(['-c', f'raise SystemExit({child_exit})']))
                (root / 'gap_check.json').write_text(json.dumps(data), encoding='utf-8')
                p, _, payload = self.invoke(['all'], directory=root)
                self.assert_exit(p, expected_exit)
                self.assertEqual(expected_exit, payload['aggregateExit'])
                self.assert_payload(payload, root)
                self.assertEqual(expected, payload['candidate']['commit'])
                self.assertTrue(payload['candidate']['dirty'])
                self.assertEqual({'name': 'all'}, payload['operation'])
                self.assertIn('Result: ', p.stdout)
            self.assertEqual(3, len(list(root.glob('results/*/result.json'))))
            self.assertEqual([], list(root.glob('results/**/*.tmp')))
            self.test_interrupting_the_real_dotnet_adapter_cleans_its_worktree()


        def test_candidate_identity_uses_only_exact_repository_scoped_git_trust(self):
            candidate = runpy.run_path(str(RUNNER))['candidate_identity']
            root = Path('C:/fixture/repository')
            calls = []

            def invoke(command, **kwargs):
                calls.append((command, kwargs))
                stdout = 'a' * 40 + '\n' if command[-1] == 'HEAD' else ''
                return subprocess.CompletedProcess(command, 0, stdout=stdout)

            with mock.patch.object(subprocess, 'run', side_effect=invoke):
                self.assertEqual({'commit': 'a' * 40, 'dirty': False}, candidate(root))
            expected = ['git', '-c', f'safe.directory={root.as_posix()}']
            self.assertEqual(expected + ['rev-parse', 'HEAD'], calls[0][0])
            self.assertEqual(expected + ['status', '--porcelain'], calls[1][0])
            self.assertEqual(root, calls[0][1]['cwd'])
            self.assertEqual(root, calls[1][1]['cwd'])


        def test_schema(self):
            p, root, payload = self.invoke(['all'])
            self.assert_exit(p, 0)
            self.assert_payload(payload, root)
            self.assertEqual(sys.executable, payload['results'][0]['argv'][0])
            data = manifest()
            data['stacks'][0]['capabilities']['test'] = configured([sys.executable, '-c', 'print("DIRECT_ARGV")'], kind='argv')
            p, _, payload = self.invoke(['all'], data)
            self.assert_exit(p, 0)
            self.assertEqual([sys.executable, '-c', 'print("DIRECT_ARGV")'], payload['results'][0]['argv'])
            for schema in [True, 1.0, '1']:
                with self.subTest(schema=schema):
                    data['schema'] = schema
                    p, _, payload = self.invoke(['all'], data)
                    self.assert_exit(p, 2)
                    self.assertIsNone(payload)
            for field, value in [('artifactRoot', '../escape'), ('artifactRoot', str(ROOT)), ('stacks', []), ('extra', True)]:
                with self.subTest(field=field, value=value):
                    data = manifest(); data[field] = value
                    p, _, payload = self.invoke(['all'], data)
                    self.assert_exit(p, 2)
                    self.assertIsNone(payload)
            self.schema_rows()


        def test_required_gate_artifact_is_required_after_successful_child_exit(self):
            data = manifest()
            data['stacks'][0]['capabilities']['static'] = configured(['-c', 'pass'], [{'path': 'missing.txt', 'required': True}])
            p, _, payload = self.invoke(['gate', 'static'], data)
            self.assert_exit(p, 2)
            self.assertEqual(('invalid', 0, 2), tuple(payload['results'][0][k] for k in ('state', 'childExit', 'resultExit')))


        def test_row_capability(self): self.row_defect('capability')


        def test_row_unavailable(self): self.row_defect('unavailable')


        def test_row_empty(self): self.row_defect('empty')


        def test_row_placeholder(self): self.row_defect('placeholder')


        def test_row_executable(self): self.row_defect('executable')


        def test_row_evidence(self): self.row_defect('evidence')


        def test_row_escape(self): self.row_defect('escape')


        def test_portable(self):
            data = self.portable_data(); dotnet, frontend, python = data['stacks']
            self.assertEqual(['aspnet', 'react-vite', 'python-uv'], [s['kind'] for s in data['stacks']])
            self.assertEqual(['dotnet', 'test', '<backend-solution-or-project>', '--no-restore'], dotnet['capabilities']['test']['exampleArgv'])
            self.assertEqual(['node', 'node_modules/vitest/vitest.mjs', 'run'], frontend['capabilities']['test']['command']['argv'])
            self.assertEqual(['uv', 'run', '--locked', '--extra', 'dev', '-m', 'pytest'], python['capabilities']['test']['command']['argv'])
            self.assertEqual('git-worktree-copy-working-changes', dotnet['isolation']['requirement'])
            self.assertEqual('unavailable', dotnet['isolation']['evidence']['state'])
            self.assertEqual('per-run-artifacts', frontend['isolation']['requirement'])
            self.assertEqual('in-place', python['isolation']['requirement'])
            self.assertIn('<', frontend['cwd']); self.assertIn('<', python['cwd'])
            self.assertIn('<', frontend['capabilities']['test']['artifacts'][0]['path'])
            for item in data['stacks']:
                for capability in ['static', 'coverage', 'mutation']:
                    self.assertEqual('unavailable', item['capabilities'][capability]['state'])
                    self.assertTrue(item['capabilities'][capability]['reason'])
            self.assertEqual('results', data['artifactRoot'])
            p, root, payload = self.invoke(['all'], data)
            self.assert_exit(p, 2)
            self.assertEqual(['unavailable', 'invalid', 'invalid'], [r['state'] for r in payload['results']])
            self.assertEqual([None, None, None], [r['childExit'] for r in payload['results']])


        def test_partial_all(self): self.partial(False)


        def test_partial_targeted(self): self.partial(True)


        def test_active_manifest(self):
            data = json.loads((ROOT / 'DynaDocs.Tests/coverage/gap_check.json').read_text())
            self.assertEqual('DynaDocs.Tests/coverage/results', data['artifactRoot'])
            dotnet, python, node, viewer, statechart = data['stacks']
            self.assertEqual({'kind': 'current-python', 'argv': ['-u', 'DynaDocs.Tests/coverage/run_tests.py', '--']}, dotnet['capabilities']['test']['command'])
            self.assertEqual({'kind': 'argv', 'argv': ['pnpm', '-C', 'skills/engineering/statechart/scripts', 'run', 'test']},
                             statechart['capabilities']['test']['command'])
            self.assertEqual(viewer['isolation'], statechart['isolation'])
            self.assertEqual(viewer['capabilities']['mutation'], statechart['capabilities']['mutation'])
            self.assertEqual({'requirement': 'git-worktree-copy-working-changes', 'evidence': {'state': 'verified', 'kind': 'adapter', 'path': 'DynaDocs.Tests/coverage/run_tests.py'}}, dotnet['isolation'])
            self.assertEqual({'kind': 'current-python', 'argv': ['-m', 'unittest', 'discover', '-s', 'DynaDocs.Tests/coverage/tests', '-p', 'test_*.py']}, python['capabilities']['test']['command'])
            self.assertEqual(['node', 'DynaDocs.Tests/coverage/node_tests.cjs'], node['capabilities']['test']['command']['argv'])
            self.assertEqual({'kind': 'argv', 'argv': ['pnpm', '-C', 'viewer', 'run', 'test']}, viewer['capabilities']['test']['command'])
            self.assertEqual({'requirement': 'in-place', 'evidence': {'state': 'verified', 'kind': 'direct'}}, viewer['isolation'])
            self.assertEqual(unavailable('No mutation mechanism is adopted for TypeScript; the DYD-103 lane the other stacks cite is canceled'),
                             viewer['capabilities']['mutation'])
            for item in data['stacks']:
                if item['name'] not in ('viewer', 'statechart'):
                    self.assertEqual(unavailable('Pending DYD-103'), item['capabilities']['mutation'])
                for capability in ['static', 'coverage']:
                    row = item['capabilities'][capability]
                    self.assertEqual('configured', row['state'])
                    self.assertEqual('current-python', row['command']['kind'])
                    self.assertEqual(['DynaDocs.Tests/coverage/gate_adapter.py', '--stack', item['name'], '--gate', capability], row['command']['argv'])
                    self.assertEqual([{'path': f'DynaDocs.Tests/coverage/results/adapters/{item["name"]}-{capability}.json', 'required': True}], row['artifacts'])
                    if capability == 'coverage':
                        collector = {'dotnet': 'csharp-coverage', 'python': 'python-coverage', 'node': 'javascript-coverage',
                                     'viewer': 'typescript-coverage', 'statechart': 'typescript-coverage'}[item['name']]
                        verdict = 'suite_exit' if item['name'] in ('viewer', 'statechart') else 'child_exit'
                        self.assertEqual({'exit': ['collectors', collector, 'facts', verdict],
                                          'failure': ['collectors', collector, 'findings', {'gate': 'functional'}]}, row['suiteVerdict'])


        def test_identity(self):
            self.assertEqual(RUNNER.read_bytes(), PORTABLE.read_bytes())
            self.test_targeted(); self.test_global_json(); self.test_selected_all(); self.test_schema(); self.test_aggregation()
            self.test_node()


        def test_node(self):
            p = subprocess.run(['node', '--test', '--test-reporter=tap', 'DynaDocs.Tests/coverage/tests/testing_facade.test.mjs'], cwd=ROOT, env={**os.environ, 'PYTHON': sys.executable, 'FACADE_RUNNER': str(self.runner)}, capture_output=True, text=True, encoding='utf-8', timeout=60)
            self.assert_exit(p, 0)
            self.assertIn('# pass 1', p.stdout)

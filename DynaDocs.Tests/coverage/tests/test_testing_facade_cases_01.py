"""Inherited facade cases; collected through the two public facade classes."""
import json
import os
import shutil
import subprocess
import sys
import tempfile
import unittest
from unittest import mock
import runpy
from pathlib import Path
from test_testing_facade_support import (unavailable, configured, stack, manifest, CAPABILITIES)


class Cases:
    """Only the public facade classes collect these inherited unittest cases."""

    class FacadeCases01(unittest.TestCase):
        def test_help(self):
            p, root, payload = self.invoke(['--help'])
            self.assert_exit(p, 0)
            for phrase in ['test', 'all', 'gate static', 'coverage', 'gate mutation', 'capabilities', '--force-run', '--stack', 'default', 'native', 'artifact', '0', '1', '2', '130', 'cleanup']:
                self.assertIn(phrase, p.stdout)
            self.assertEqual([], list(root.glob('*.txt')))
            self.assertIsNone(payload)


        def test_bare(self):
            p, root, payload = self.invoke([])
            self.assert_exit(p, 2)
            self.assertIn('Usage:', p.stdout)
            self.assertEqual([], list(root.glob('*.txt')))
            self.assertIsNone(payload)


        def test_targeted(self):
            p, root, payload = self.invoke(['test', '--stack', 'dotnet', '--', '--filter', 'FullyQualifiedName~ParserTests'])
            self.assert_exit(p, 0)
            self.assert_rows(payload, [('dotnet', 'test', 'passed')])
            self.assertEqual(['--filter', 'FullyQualifiedName~ParserTests'], payload['results'][0]['argv'][-2:])
            self.assertIn('["--filter", "FullyQualifiedName~ParserTests"]', p.stdout)
            self.assertEqual(['dotnet-test.txt'], [x.name for x in root.glob('*.txt')])
            self.test_targeted_grammar_and_literal_force_flag()


        def test_targeted_grammar_and_literal_force_flag(self):
            for args in [['test'], ['test', '--stack', 'dotnet,peer'], ['all', '--', '--filter'], ['all', '--since', 'BASE'], ['all', '--since', '']]:
                with self.subTest(args=args):
                    p, root, payload = self.invoke(args, manifest(stack('dotnet'), stack('peer')))
                    self.assert_exit(p, 2)
                    self.assertEqual([], list(root.glob('*.txt')))
                    self.assertIsNone(payload)
            p, _, payload = self.invoke(['test', '--stack', 'dotnet', '--', '--force-run', '--help', '--', '$literal'])
            self.assert_exit(p, 0)
            self.assertEqual(['--force-run', '--help', '--', '$literal'], payload['results'][0]['argv'][-4:])


        def test_selected_all(self):
            p, root, payload = self.invoke(['all', '--stack', 'python,frontend'], manifest(stack('dotnet'), stack('frontend'), stack('python')))
            self.assert_exit(p, 0)
            self.assert_rows(payload, [('frontend', 'test', 'passed'), ('python', 'test', 'passed')])
            self.assertEqual(['frontend', 'python'], payload['selectedStacks'])
            self.assertEqual({'frontend-test.txt', 'python-test.txt'}, {x.name for x in root.glob('*.txt')})
            self.assertEqual(1, p.stdout.count('frontend:test'))
            self.assertEqual(1, p.stdout.count('python:test'))


        def test_default_all(self):
            good, missing, bad = stack('good'), stack('missing'), stack('bad')
            missing['capabilities']['test'] = unavailable('adopt test')
            bad['capabilities']['test']['command']['argv'] = []
            p, root, payload = self.invoke(['all'], manifest(good, missing, bad))
            self.assert_exit(p, 2)
            self.assert_rows(payload, [('good', 'test', 'passed'), ('missing', 'test', 'unavailable'), ('bad', 'test', 'invalid')])
            self.assertEqual(['good-test.txt'], [x.name for x in root.glob('*.txt')])
            self.assertEqual(1, p.stdout.count('good:test'))


        def test_gate_static(self): self.gate('static')


        def test_gate_coverage(self): self.gate('coverage')


        def test_gate_mutation(self): self.gate('mutation')


        def test_mutation_base(self):
            base = 'BASE with spaces; $literal & untouched'
            p, _, payload = self.invoke(['gate', 'mutation', '--since', base, '--stack', 'dotnet'])
            self.assert_exit(p, 0)
            self.assertEqual(base, payload['results'][0]['argv'][-1])
            self.assertNotIn('--since', payload['results'][0]['argv'])
            self.assertIn(json.dumps([base]), p.stdout)


        def test_mutation_zero(self): self.mutation_invalid([])


        def test_mutation_two(self): self.mutation_invalid(['{base}', '{base}'])


        def test_mutation_substring(self): self.mutation_invalid(['--baseline={base}'])


        def test_capabilities(self):
            data = manifest(stack('dotnet'), stack('python'))
            data['stacks'][1]['capabilities']['coverage'] = unavailable('Pending measurement')
            p, root, payload = self.invoke(['capabilities'], data)
            self.assert_exit(p, 0)
            for name in ['dotnet:', 'python:', 'test: configured', 'static: configured', 'mutation: configured', 'coverage: unavailable: Pending measurement']:
                self.assertIn(name, p.stdout)
            self.assertEqual([], list(root.glob('*.txt')))
            self.assertIsNone(payload)
            self.assertFalse((root / 'results').exists())


        def test_capabilities_reports_malformed_rows_without_executing(self):
            for malformed in [None, [], 4, 'configured']:
                for whole_container in [False, True]:
                    with self.subTest(value=malformed, whole_container=whole_container):
                        bad = stack('bad')
                        if whole_container:
                            bad['capabilities'] = malformed
                        else:
                            bad['capabilities']['test'] = malformed
                        p, root, payload = self.invoke(['capabilities'], manifest(bad, stack('peer')))
                        self.assert_exit(p, 2)
                        self.assertIn('invalid', p.stdout)
                        self.assertIn('peer:', p.stdout)
                        self.assertEqual([], list(root.glob('*.txt')))
                        self.assertIsNone(payload)


        def test_force_run_keeps_malformed_capabilities_container_row_local(self):
            for malformed in [None, [], 4, 'configured']:
                with self.subTest(value=malformed):
                    bad = stack('bad')
                    bad['capabilities'] = malformed
                    p, root, payload = self.invoke(['--force-run'], manifest(bad, stack('peer')))
                    self.assert_exit(p, 2)
                    self.assertNotIn('Traceback', p.stderr, p.stderr)
                    self.assertIsNotNone(payload, p.stdout + p.stderr)
                    self.assert_rows(payload, [
                        ('bad', 'test', 'invalid'), ('bad', 'static', 'invalid'), ('bad', 'coverage', 'invalid'),
                        ('peer', 'test', 'passed'), ('peer', 'static', 'passed'), ('peer', 'coverage', 'passed')])
                    self.assertFalse((root / 'bad-test.txt').exists())
                    produced = {x.name for x in root.glob('*.txt')}
                    for capability in ('test', 'static', 'coverage'):
                        self.assertIn(f'peer-{capability}.txt', produced)


        def test_capabilities_validates_configuration_without_execution(self):
            cases = [
                (('kind',), ''),
                (('cwd',), '../outside'),
                (('isolation',), {}),
                (('isolation',), {'requirement': 'per-run-artifacts', 'evidence': {'state': 'verified', 'kind': 'adapter', 'path': '../adapter.py'}}),
                (('isolation', 'evidence'), {'state': 'unavailable', 'reason': ''}),
                (('capabilities', 'test', 'reason'), 'forbidden'),
                (('capabilities', 'test', 'command', 'kind'), 'shell'),
                (('capabilities', 'test', 'command', 'argv'), []),
                (('capabilities', 'test', 'command'), {'kind': 'argv', 'argv': ['dydo-no-such-executable-5f71']}),
                (('capabilities', 'test'), {'state': 'unavailable', 'reason': ''}),
                (('capabilities', 'test'), {'state': 'unavailable', 'reason': 'pending', 'command': {'kind': 'current-python', 'argv': ['-c', 'pass']}}),
                (('capabilities', 'test'), {'state': 'unavailable', 'reason': 'pending', 'artifacts': []}),
                (('capabilities', 'coverage', 'artifacts'), [{'path': '../old-report', 'required': True}]),
                (('capabilities', 'mutation', 'command', 'argv'), ['-c', 'pass']),
                (('capabilities', 'mutation', 'command', 'argv'), ['-c', 'pass', '{base}', '{base}']),
                (('capabilities', 'mutation', 'command', 'argv'), ['-c', 'pass', '--base={base}']),
            ]
            for keys, value in cases:
                with self.subTest(keys=keys, value=value):
                    bad = stack('bad')
                    target = bad
                    for key in keys[:-1]:
                        target = target[key]
                    target[keys[-1]] = value
                    p, root, payload = self.invoke(['capabilities'], manifest(bad, stack('peer')))
                    self.assert_exit(p, 2)
                    bad_output, peer_output = p.stdout.split('peer:')
                    self.assertIn('invalid', bad_output)
                    for capability in CAPABILITIES:
                        self.assertIn(f'{capability}: configured', peer_output)
                    self.assertIsNone(payload)
                    self.assertFalse((root / 'results').exists())
                    self.assertEqual([], list(root.glob('*.txt')))


        def test_capabilities_rejects_escaping_result_root_without_execution(self):
            data = manifest()
            data['artifactRoot'] = '../escaped-results'
            p, root, payload = self.invoke(['capabilities'], data)
            self.assert_exit(p, 2)
            self.assertIn('artifactRoot', p.stderr)
            self.assertIsNone(payload)
            self.assertFalse((root / 'results').exists())
            self.assertEqual([], list(root.glob('*.txt')))


        def test_capabilities_rejects_missing_absolute_executable_and_unavailable_masking(self):
            for unavailable_isolation in [False, True]:
                with self.subTest(unavailable_isolation=unavailable_isolation):
                    root = self.fixture()
                    bad = stack('bad')
                    bad['capabilities']['test']['command'] = {'kind': 'argv', 'argv': [str(root / 'missing.exe')]}
                    if unavailable_isolation:
                        bad['isolation']['evidence'] = {'state': 'unavailable', 'reason': 'adapter pending'}
                    (root / 'gap_check.json').write_text(json.dumps(manifest(bad, stack('peer'))), encoding='utf-8')
                    p, _, payload = self.invoke(['capabilities'], directory=root)
                    self.assert_exit(p, 2)
                    self.assertIn('test: invalid', p.stdout)
                    self.assertIn('missing executable', p.stdout)
                    self.assertIn('peer:', p.stdout)
                    self.assertIsNone(payload)
                    self.assertFalse((root / 'results').exists())
                    self.assertEqual([], list(root.glob('*.txt')))


        def test_relative_argv_executable_resolves_from_stack_cwd(self):
            root = self.fixture()
            working = root / 'working'
            working.mkdir()
            tools = root / 'tools'
            tools.mkdir()
            shell = Path(os.environ['COMSPEC']) if os.name == 'nt' else Path('/bin/sh')
            executable = tools / ('facade-shell.exe' if os.name == 'nt' else 'facade-shell')
            shutil.copy2(shell, executable)
            relative_executable = os.path.relpath(executable, working)
            data = manifest()
            data['stacks'][0]['cwd'] = 'working'
            data['stacks'][0]['capabilities']['test'] = configured(
                [relative_executable, '/c' if os.name == 'nt' else '-c', 'echo RELATIVE_EXECUTABLE_RAN'],
                kind='argv')
            (root / 'gap_check.json').write_text(json.dumps(data), encoding='utf-8')

            inspected, _, payload = self.invoke(['capabilities'], directory=root)
            self.assert_exit(inspected, 0)
            self.assertIn('test: configured', inspected.stdout)
            self.assertIsNone(payload)
            self.assertFalse((root / 'results').exists())

            executed, _, payload = self.invoke(['all'], directory=root)
            self.assert_exit(executed, 0)
            self.assertIn('RELATIVE_EXECUTABLE_RAN', executed.stdout)
            self.assert_rows(payload, [('dotnet', 'test', 'passed')])
            self.assertEqual(relative_executable, payload['results'][0]['argv'][0])


        def test_dot_relative_executable_and_bare_path_lookup_are_distinct(self):
            root = self.fixture()
            working = root / 'working'
            working.mkdir()
            shell = Path(os.environ['COMSPEC']) if os.name == 'nt' else Path('/bin/sh')
            tool = working / ('facade-tool.exe' if os.name == 'nt' else 'facade-tool')
            shutil.copy2(shell, tool)
            argv = ['./' + tool.name, '/c' if os.name == 'nt' else '-c', 'echo DOT_RELATIVE_RAN']
            data = manifest()
            data['stacks'][0]['cwd'] = 'working'
            data['stacks'][0]['capabilities']['test'] = configured(argv, kind='argv')
            (root / 'gap_check.json').write_text(json.dumps(data), encoding='utf-8')
            executed, _, payload = self.invoke(['all'], directory=root)
            self.assert_exit(executed, 0)
            self.assertIn('DOT_RELATIVE_RAN', executed.stdout)
            self.assertEqual(argv, payload['results'][0]['argv'])

            resolver = runpy.run_path(str(self.runner))['resolve_executable']
            tool_identity = tool.resolve()
            # On Windows this is POSIX control-flow evidence, not a native Linux run.
            with mock.patch.object(sys, 'platform', 'linux'), mock.patch('shutil.which') as which:
                which.side_effect = (lambda value, path=None: value
                                     if value in {str(tool), str(tool_identity)} else None)
                self.assertEqual(str(tool_identity), resolver('./' + tool.name, working))
                self.assertIsNone(resolver(tool.name, working))
                self.assertEqual(str(tool), resolver(str(tool), working))
                self.assertEqual([mock.call(str(tool_identity), path=None), mock.call(tool.name, path=None),
                                  mock.call(str(tool), path=None)], which.call_args_list)


        def test_unusable_result_destination_starts_no_child(self):
            for destination in ['results', 'results/nested']:
                with self.subTest(destination=destination):
                    data = manifest()
                    data['artifactRoot'] = destination
                    root = self.fixture(data)
                    (root / 'results').write_text('unrelated file', encoding='utf-8')
                    p, _, payload = self.invoke(['all'], directory=root)
                    self.assert_exit(p, 2)
                    self.assertIn('artifactRoot', p.stderr)
                    self.assertNotIn('Traceback', p.stderr)
                    self.assertIsNone(payload)
                    self.assertFalse((root / 'dotnet-test.txt').exists())
                    self.assertEqual('unrelated file', (root / 'results').read_text())


        def test_result_destination_is_prepared_before_dispatch(self):
            data = manifest()
            data['artifactRoot'] = 'nested/results'
            data['stacks'][0]['capabilities']['test'] = configured(['-c',
                "from pathlib import Path; assert len(list(Path('nested/results').glob('run-*/result.tmp'))) == 1"])
            p, _, payload = self.invoke(['all'], data)
            self.assert_exit(p, 0)
            self.assertEqual('passed', payload['results'][0]['state'])


        def test_artifact_destination_identity_accepts_equivalent_root_spelling(self):
            root = self.fixture(noncanonical_root=True)

            process, _, payload = self.invoke(['all'], directory=root)

            self.assert_exit(process, 0)
            self.assertTrue((root / 'dotnet-test.txt').is_file())
            self.assert_rows(payload, [('dotnet', 'test', 'passed')])
            results = list((root / 'results').glob('run-*/result.json'))
            self.assertEqual(1, len(results))
            self.assertEqual((root / 'results').resolve(), results[0].resolve().parent.parent)


        def test_artifact_destination_identity_rejects_foreign_resolved_identity(self):
            data = manifest()
            data['artifactRoot'] = 'foreign-results'
            root = self.fixture(data)
            foreign_temporary = tempfile.TemporaryDirectory(prefix='dydo-facade-foreign-')
            self.addCleanup(foreign_temporary.cleanup)
            foreign = Path(foreign_temporary.name)
            link = root / data['artifactRoot']
            if os.name == 'nt':
                created = subprocess.run(
                    [os.environ['COMSPEC'], '/d', '/c', 'mklink', '/J', str(link), str(foreign)],
                    capture_output=True, text=True, encoding='utf-8')
                self.assertEqual(0, created.returncode, created.stdout + created.stderr)
            else:
                link.symlink_to(foreign, target_is_directory=True)

            process, _, payload = self.invoke(['all'], directory=root)

            self.assert_exit(process, 2)
            self.assertIn('artifactRoot', process.stderr)
            self.assertIsNone(payload)
            self.assertFalse((root / 'dotnet-test.txt').exists())
            self.assertEqual([], list(foreign.iterdir()))

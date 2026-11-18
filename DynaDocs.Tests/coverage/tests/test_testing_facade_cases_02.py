"""Inherited facade cases; collected through the two public facade classes."""
import unittest
from test_testing_facade_support import (unavailable, configured, stack, manifest, coverage_report, functional, verdict_stack)


class Cases:
    """Only the public facade classes collect these inherited unittest cases."""

    class FacadeCases02(unittest.TestCase):
        def test_artifact_destination_identity_rejects_angle_placeholder(self):
            data = manifest()
            data['artifactRoot'] = '<artifact-root>'
            root = self.fixture(data)

            process, _, payload = self.invoke(['all'], directory=root)

            self.assert_exit(process, 2)
            self.assertIn('artifactRoot', process.stderr)
            self.assertIsNone(payload)
            self.assertFalse((root / 'dotnet-test.txt').exists())
            self.assertFalse((root / 'results').exists())


        def test_result_write_failure_is_a_controlled_exit(self):
            data = manifest()
            data['stacks'][0]['capabilities']['test'] = configured(['-c',
                "from pathlib import Path; root=Path('results'); root.mkdir(exist_ok=True); "
                "paths=list(root.glob('run-*/result.tmp')); "
                "[(p.unlink(), p.mkdir()) for p in paths]; "
                "root.rmdir() if not paths else None; "
                "root.write_text('blocked') if not paths else None; print('CHILD_RAN')"])
            p, _, payload = self.invoke(['all'], data)
            self.assert_exit(p, 2)
            self.assertIn('CHILD_RAN', p.stdout)
            self.assertIn('artifact', p.stderr)
            self.assertNotIn('Traceback', p.stderr)
            self.assertIsNone(payload)


        def test_stale_required_gate_artifact_cannot_pass(self):
            for capability in ['static', 'coverage', 'mutation']:
                for directory_artifact in [False, True]:
                    with self.subTest(capability=capability, directory_artifact=directory_artifact):
                        data = manifest(stack('stale'), stack('peer'))
                        argv = ['-c', 'pass'] + (['{base}'] if capability == 'mutation' else [])
                        data['stacks'][0]['capabilities'][capability] = configured(argv, [{'path': 'old-report', 'required': True}])
                        root = self.fixture(data)
                        report = root / 'old-report'
                        if directory_artifact:
                            report.mkdir()
                            report = report / 'evidence.txt'
                        report.write_bytes(b'old candidate evidence')
                        before = report.stat()
                        p, _, payload = self.invoke(['gate', capability] + (['--since', 'BASE'] if capability == 'mutation' else []), directory=root)
                        self.assert_exit(p, 2)
                        self.assert_rows(payload, [('stale', capability, 'invalid'), ('peer', capability, 'passed')])
                        self.assertEqual(0, payload['results'][0]['childExit'])
                        self.assertEqual(2, payload['results'][0]['resultExit'])
                        self.assertEqual(2, payload['aggregateExit'])
                        self.assertEqual(b'old candidate evidence', report.read_bytes())
                        self.assertEqual((before.st_mtime_ns, before.st_ino), (report.stat().st_mtime_ns, report.stat().st_ino))


        def test_new_or_refreshed_required_artifact_passes(self):
            for directory_artifact in [False, True]:
                for change in ['new', 'content', 'timestamp']:
                    with self.subTest(directory_artifact=directory_artifact, change=change):
                        target = 'report/nested/evidence.txt' if directory_artifact else 'report'
                        code = f"import os; from pathlib import Path; p=Path({target!r}); p.parent.mkdir(parents=True, exist_ok=True); "
                        if change == 'timestamp':
                            code += 's=p.stat(); os.utime(p, ns=(s.st_atime_ns, s.st_mtime_ns+1000000000))'
                        elif change == 'content':
                            code += "s=p.stat(); p.write_bytes(b'new evidence'); os.utime(p, ns=(s.st_atime_ns, s.st_mtime_ns))"
                        else:
                            code += "p.write_bytes(b'new evidence')"
                        data = manifest()
                        data['stacks'][0]['capabilities']['static'] = configured(['-c', code], [{'path': 'report', 'required': True}])
                        root = self.fixture(data)
                        report = root / target
                        if change != 'new':
                            report.parent.mkdir(parents=True, exist_ok=True)
                            report.write_bytes(b'old evidence')
                        p, _, payload = self.invoke(['gate', 'static'], directory=root)
                        self.assert_exit(p, 0)
                        self.assert_rows(payload, [('dotnet', 'static', 'passed')])
                        self.assertEqual(0, payload['aggregateExit'])
                        self.assertEqual(b'old evidence' if change == 'timestamp' else b'new evidence', report.read_bytes())


        def test_every_required_artifact_must_be_refreshed(self):
            data = manifest()
            data['stacks'][0]['capabilities']['coverage'] = configured(
                ['-c', "from pathlib import Path; Path('new-report').write_text('fresh')"],
                [{'path': 'new-report', 'required': True}, {'path': 'old-report', 'required': True}])
            root = self.fixture(data)
            (root / 'old-report').write_bytes(b'old evidence')
            p, _, payload = self.invoke(['gate', 'coverage'], directory=root)
            self.assert_exit(p, 2)
            self.assertEqual('invalid', payload['results'][0]['state'])
            self.assertEqual(0, payload['results'][0]['childExit'])
            self.assertEqual(b'old evidence', (root / 'old-report').read_bytes())
            self.assertEqual('fresh', (root / 'new-report').read_text())


        def test_non_utf8_manifest_is_globally_invalid(self):
            root = self.fixture()
            (root / 'gap_check.json').write_bytes(b'\xff')
            p, root, payload = self.invoke(['all'], directory=root)
            self.assert_exit(p, 2)
            self.assertIn('invalid JSON manifest', p.stderr)
            self.assertNotIn('Traceback', p.stderr)
            self.assertEqual([], list(root.glob('*.txt')))
            self.assertIsNone(payload)


        def test_nul_command_is_row_invalid_and_valid_peer_runs(self):
            bad = stack('bad')
            bad['capabilities']['test']['command']['argv'].append('\x00')
            p, root, payload = self.invoke(['all'], manifest(bad, stack('peer')))
            self.assert_exit(p, 2)
            self.assert_rows(payload, [('bad', 'test', 'invalid'), ('peer', 'test', 'passed')])
            self.assertIsNone(payload['results'][0]['childExit'])
            self.assertEqual(['peer-test.txt'], [x.name for x in root.glob('*.txt')])


        def test_full_g(self):
            first, second = stack('first'), stack('second')
            first['capabilities']['static'] = unavailable('Pending DYD-96')
            second['capabilities']['coverage']['command']['argv'] = []
            third = verdict_stack('third', report=coverage_report('third-coverage', 0), coverage_exit=0)
            p, root, payload = self.invoke(['--force-run'], manifest(first, second, third))
            self.assert_exit(p, 2)
            self.assert_rows(payload, [('first', 'test', 'passed'), ('first', 'static', 'unavailable'), ('first', 'coverage', 'passed'), ('second', 'test', 'passed'), ('second', 'static', 'passed'), ('second', 'coverage', 'invalid'), ('third', 'test', 'passed'), ('third', 'static', 'passed'), ('third', 'coverage', 'passed')])
            self.assertEqual({'first-test.txt', 'first-coverage.txt', 'second-test.txt', 'second-static.txt', 'third-static.txt', 'third-coverage.json'}, {x.name for x in root.glob('*.txt')} | {x.name for x in root.glob('*.json') if x.name != 'gap_check.json'})
            self.assertNotIn('third-test.txt', [x.name for x in root.glob('*.txt')])


        def test_derived_test_row_passes_from_the_instrumented_run(self):
            item = verdict_stack(report=coverage_report('first-coverage', 0), coverage_exit=0)
            p, root, payload = self.invoke(['--force-run'], manifest(item))
            self.assert_exit(p, 0)
            self.assert_rows(payload, [('first', 'test', 'passed'), ('first', 'static', 'passed'), ('first', 'coverage', 'passed')])
            row = payload['results'][0]
            self.assertEqual([], row['argv'])
            self.assertEqual(0, row['childExit'])
            self.assertTrue(row['reason'].startswith('test verdict derived from the coverage row'), row['reason'])
            self.assertNotIn('first-test.txt', [x.name for x in root.glob('*.txt')])
            self.assertIn('first-coverage.json', [x.name for x in root.glob('*.json')])


        def test_derived_suite_passes_policy_passes(self):
            p, _, payload = self.derived_case(coverage_report('first-coverage', 0), 0)
            self.assert_exit(p, 0)
            self.assert_derived(payload, ('passed', 0, 0), ('passed', 0), 0)


        def test_derived_suite_passes_policy_fails(self):
            p, _, payload = self.derived_case(coverage_report('first-coverage', 0), 1)
            self.assert_exit(p, 1)
            self.assert_derived(payload, ('passed', 0, 0), ('failed', 1), 1)


        def test_derived_suite_fails(self):
            p, _, payload = self.derived_case(coverage_report('first-coverage', 5, [functional(5)]), 1)
            self.assert_exit(p, 1)
            self.assert_derived(payload, ('failed', 5, 1), ('failed', 1), 1)
            self.assertTrue(payload['results'][0]['reason'].startswith('test verdict derived from the coverage row'))


        def test_derived_suite_failure_names_the_failing_tests_in_the_row(self):
            finding = {**functional(1), 'failingTests': ['A.B', 'C.D'],
                       'failingTestsSummary': '2 failing tests'}
            p, _, payload = self.derived_case(coverage_report('first-coverage', 1, [finding]), 1)
            self.assert_exit(p, 1)
            self.assert_derived(payload, ('failed', 1, 1), ('failed', 1), 1)
            reason = payload['results'][0]['reason']
            self.assertTrue(reason.endswith('; 2 failing tests: A.B, C.D'), reason)
            self.assertTrue(any(reason in line for line in p.stdout.splitlines()), p.stdout)


        def test_a_failure_finding_without_named_tests_keeps_the_plain_row(self):
            for finding in (functional(1),
                            {**functional(1), 'failingTests': [],
                             'failingTestsSummary':
                                 'no failing test named in altcover-runner.stdout or '
                                 'altcover-runner.stderr'}):
                with self.subTest(finding=finding):
                    p, _, payload = self.derived_case(coverage_report('first-coverage', 1, [finding]), 1)
                    self.assert_exit(p, 1)
                    reason = payload['results'][0]['reason']
                    if 'failingTestsSummary' in finding:
                        self.assertTrue(reason.endswith(
                            '; no failing test named in altcover-runner.stdout or '
                            'altcover-runner.stderr'), reason)
                        self.assertFalse(reason.endswith(': '), reason)
                    else:
                        self.assertTrue(reason.startswith('test verdict derived from the coverage row'),
                                        reason)
                        self.assertNotIn(';', reason)


        def test_derived_campaign_could_not_measure(self):
            p, _, payload = self.derived_case(coverage_report('first-coverage'), 2)
            self.assert_exit(p, 2)
            self.assert_derived(payload, ('invalid', None, 2), ('invalid', 2), 2)


        def test_derived_invalid_campaign_with_usable_report(self):
            p, _, payload = self.derived_case(coverage_report('first-coverage', 5, [functional(5)]), 2)
            self.assert_exit(p, 2)
            self.assert_derived(payload, ('invalid', None, 2), ('invalid', 2), 2)


        def test_derived_report_records_no_suite_exit(self):
            p, _, payload = self.derived_case(coverage_report('first-coverage'), 0)
            self.assert_exit(p, 2)
            self.assert_derived(payload, ('invalid', None, 2), ('passed', 0), 2)


        def test_derived_unattributed_child_exit(self):
            p, _, payload = self.derived_case(coverage_report('first-coverage', 5), 1)
            self.assert_exit(p, 2)
            self.assert_derived(payload, ('invalid', None, 2), ('failed', 1), 2)


        def test_derived_exit_2_diagnosis_is_rendered_on_the_unattributed_row(self):
            errors = [{'gate': 'csharp-coverage', 'message': 'native campaign incomplete',
                      'failureCategory': 'timeout', 'identitiesPath': '/tmp/run/identities.json'}]
            p, _, payload = self.derived_case(coverage_report('first-coverage', 2, errors=errors), 1)
            self.assert_exit(p, 2)
            self.assert_derived(payload, ('invalid', None, 2), ('failed', 1), 2)
            reason = payload['results'][0]['reason']
            self.assertTrue(reason.endswith('; timeout at /tmp/run/identities.json'), reason)


        def test_derived_exit_2_diagnosis_joins_category_and_message_when_both_are_named(self):
            errors = [{'gate': 'csharp-coverage', 'message': 'native campaign incomplete',
                      'failureCategory': 'preflight',
                      'errorMessage': 'Owner requires Windows CPython 3.14',
                      'identitiesPath': '/tmp/run/identities.json'}]
            p, _, payload = self.derived_case(coverage_report('first-coverage', 2, errors=errors), 1)
            self.assert_exit(p, 2)
            self.assert_derived(payload, ('invalid', None, 2), ('failed', 1), 2)
            reason = payload['results'][0]['reason']
            self.assertTrue(reason.endswith(
                '; preflight: Owner requires Windows CPython 3.14 at /tmp/run/identities.json'), reason)


        def test_derived_exit_2_diagnosis_unavailable_is_rendered_on_the_unattributed_row(self):
            errors = [{'gate': 'csharp-coverage', 'message': 'native campaign incomplete',
                      'diagnosisUnavailable': 'identities.json is missing or unreadable',
                      'identitiesPath': '/tmp/run/identities.json'}]
            p, _, payload = self.derived_case(coverage_report('first-coverage', 2, errors=errors), 1)
            self.assert_exit(p, 2)
            reason = payload['results'][0]['reason']
            self.assertTrue(reason.endswith(
                '; identities.json is missing or unreadable at /tmp/run/identities.json'), reason)


        def test_derived_unattributed_child_exit_without_diagnosis_keeps_the_plain_row(self):
            p, _, payload = self.derived_case(coverage_report('first-coverage', 2, errors=[]), 1)
            self.assert_exit(p, 2)
            reason = payload['results'][0]['reason']
            self.assertTrue(reason.endswith('to the suite'), reason)
            self.assertNotIn(';', reason)


        def test_a_non_dotnet_stacks_unattributed_row_is_unaffected_by_the_diagnosis_renderer(self):
            errors = [{'gate': 'functional', 'unrelated': 'value'}]
            p, _, payload = self.derived_case(coverage_report('first-coverage', 2, errors=errors), 1)
            self.assert_exit(p, 2)
            reason = payload['results'][0]['reason']
            self.assertTrue(reason.endswith('to the suite'), reason)
            self.assertNotIn(';', reason)


        def test_derivation_only_when_both_rows_are_selected(self):
            for args, test_runs, coverage_runs in [
                    (['all'], True, False),
                    (['test', '--stack', 'dotnet'], True, False),
                    (['gate', 'coverage', '--stack', 'dotnet'], False, True),
                    (['--force-run'], False, True)]:
                with self.subTest(args=args):
                    item = verdict_stack('dotnet', report=coverage_report('dotnet-coverage', 0), coverage_exit=0)
                    p, root, _ = self.invoke(args, manifest(item))
                    self.assert_exit(p, 0)
                    self.assertEqual(test_runs, (root / 'dotnet-test.txt').exists(), args)
                    self.assertEqual(coverage_runs, (root / 'dotnet-coverage.json').exists(), args)


        def test_undeclared_or_unavailable_coverage_keeps_the_plain_test_row(self):
            undeclared = verdict_stack('undeclared', report=coverage_report('undeclared-coverage', 0), coverage_exit=0, declared=False)
            absent = stack('absent')
            absent['capabilities']['coverage'] = unavailable('not adopted')
            p, root, payload = self.invoke(['--force-run'], manifest(undeclared, absent))
            self.assert_exit(p, 2)
            self.assert_rows(payload, [('undeclared', 'test', 'passed'), ('undeclared', 'static', 'passed'), ('undeclared', 'coverage', 'passed'), ('absent', 'test', 'passed'), ('absent', 'static', 'passed'), ('absent', 'coverage', 'unavailable')])
            for row in payload['results']:
                if row['capability'] == 'test':
                    self.assertNotEqual([], row['argv'])
                    self.assertEqual(0, row['childExit'])
            self.assertIn('undeclared-test.txt', [x.name for x in root.glob('*.txt')])
            self.assertIn('absent-test.txt', [x.name for x in root.glob('*.txt')])

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
from test_testing_facade_support import (has_worktree_identity, configured, stack, manifest, ROOT, RUNNER, SCRIPT_PATH)


class Cases:
    """Only the public facade classes collect these inherited unittest cases."""

    class FacadeCases04(unittest.TestCase):
        def test_copied_single_file_preserves_child_streams_exit_deadline_and_cleanup(self):
            with tempfile.TemporaryDirectory(prefix='dydo-standalone-facade-') as temporary:
                root = Path(temporary)
                subprocess.run(['git', 'init', '--quiet', str(root)], check=True)
                shutil.copyfile(RUNNER, root / 'gap_check.py')
                (root / 'child.py').write_text(
                    "import math,os,sys,time\nfrom pathlib import Path\n"
                    "try:\n"
                    "    deadline=float(os.environ['DYDO_ROW_DEADLINE'])\n"
                    "    assert math.isfinite(deadline) and deadline > time.monotonic()\n"
                    "    Path('deadline.txt').write_text(str(deadline), encoding='utf-8')\n"
                    "    print('STANDALONE_STDOUT', flush=True)\n"
                    "    print('STANDALONE_STDERR', file=sys.stderr, flush=True)\n"
                    "    raise SystemExit(17)\n"
                    "finally:\n"
                    "    Path('cleanup.txt').write_text('complete', encoding='utf-8')\n",
                    encoding='utf-8')
                data = manifest(stack('standalone'))
                data['stacks'][0]['capabilities']['test'] = configured(['-u', 'child.py'])
                (root / 'gap_check.json').write_text(json.dumps(data), encoding='utf-8')

                process = subprocess.run([sys.executable, '-u', str(root / 'gap_check.py'),
                                          'test', '--stack', 'standalone'], cwd=root,
                                         capture_output=True, text=True, encoding='utf-8', timeout=30)

                self.assertEqual(1, process.returncode, process.stdout + process.stderr)
                self.assertIn('STANDALONE_STDOUT', process.stdout)
                self.assertIn('STANDALONE_STDERR', process.stderr)
                self.assertEqual('complete', (root / 'cleanup.txt').read_text(encoding='utf-8'))
                deadline = float((root / 'deadline.txt').read_text(encoding='utf-8'))
                self.assertLess(deadline, float('inf'))
                result_path = next(root.glob('results/*/result.json'))
                row = json.loads(result_path.read_text(encoding='utf-8'))['results'][0]
                self.assertEqual(('failed', 17, 1),
                                 (row['state'], row['childExit'], row['resultExit']))
                self.assertEqual(deadline, float(row['environment']['DYDO_ROW_DEADLINE']))


        def test_root_is_the_nearest_ancestor_with_a_repository_marker(self):
            # A sandbox can leave an empty `.git` mount point in a shared temporary ancestor; only a
            # gitfile or a directory holding HEAD names a repository.
            plants = {
                'an empty .git directory': (lambda git: git.mkdir(), False),
                'a .git file': (lambda git: git.write_text('gitdir: elsewhere\n', encoding='utf-8'), True),
                'a .git directory holding HEAD': (lambda git: (git.mkdir(), (git / 'HEAD').write_text('ref: refs/heads/main\n', encoding='utf-8')), True),
            }
            for marker, (plant, adopted) in plants.items():
                with self.subTest(marker=marker), tempfile.TemporaryDirectory(prefix='dydo-facade-root-') as temporary:
                    repository = Path(temporary).resolve()
                    subprocess.run(['git', 'init', '--quiet', str(repository)], check=True)
                    ancestor = repository / 'ancestor'
                    project = ancestor / 'project'
                    project.mkdir(parents=True)
                    plant(ancestor / '.git')
                    shutil.copyfile(self.runner, project / 'gap_check.py')
                    (project / 'gap_check.json').write_text(json.dumps(manifest()), encoding='utf-8')
                    p = subprocess.run([sys.executable, '-u', str(project / 'gap_check.py'), 'all'], cwd=project,
                                       capture_output=True, text=True, encoding='utf-8', timeout=60)
                    self.assert_exit(p, 0)
                    result = next(Path(line[8:]) for line in p.stdout.splitlines() if line.startswith('Result: '))
                    expected = ancestor if adopted else repository
                    self.assertEqual(expected / 'results', result.parents[1])
                    self.assertTrue((expected / 'dotnet-test.txt').is_file())


        def test_owned_policy_docs_replace_tiers_with_the_dr048_gate_set(self):
            for path in ['dydo/guides/testing-strategy.md', 'dydo/reference/coverage-tools.md', 'Scaffold/dydo/guides/coding-standards.md']:
                text = self.policy_document(path)
                for expected in ['HCRAP', '80%', '60%', '20', 'seven', '15 lines', '100 tokens', 'dependency cycles']:
                    self.assertIn(expected, text, path)
                for retired in ['T1 | T2 | T3', '@test-tier', '≤ 30']:
                    self.assertNotIn(retired, text, path)
                normalized = ' '.join(text.split())
                self.assertTrue('no surviving or uncovered changed-code mutants' in normalized
                                or 'No changed-code mutant may survive or remain uncovered' in normalized, path)
                if path == 'Scaffold/dydo/guides/coding-standards.md':
                    prohibition = ('There are no tiers, tier annotations, tier registries, classic CRAP '
                                   'thresholds, per-file suppressions, or nesting-depth gate.')
                    self.assertNotRegex(normalized.replace(prohibition, ''), r'(?i)\btiers?\b', path)


        def test_worktree_collision_preserves_the_foreign_directory(self):
            adapter = ROOT / 'DynaDocs.Tests/coverage/run_tests.py'
            with tempfile.TemporaryDirectory(prefix='dydo-allocation-race-') as temporary:
                candidate = Path(temporary) / 'dydo-test-c0111de0'
                harness = (
                    "import importlib.util\nfrom pathlib import Path\nfrom types import SimpleNamespace\n"
                    + SCRIPT_PATH + f"spec=importlib.util.spec_from_file_location('adapter', {str(adapter)!r})\n"
                    "module=importlib.util.module_from_spec(spec); spec.loader.exec_module(module)\n"
                    f"module.tempfile.gettempdir=lambda: {temporary!r}\n"
                    "module.uuid.uuid4=lambda: SimpleNamespace(hex='c0111de0')\n"
                    "original=module.is_registered_worktree\n"
                    "def collide_after_preflight(path):\n"
                    "    registered=original(path)\n"
                    "    assert not registered and not path.exists()\n"
                    "    path.mkdir()\n"
                    "    (path/'foreign-marker.txt').write_text('foreign owner', encoding='utf-8')\n"
                    "    print('FOREIGN_DIRECTORY_CREATED', flush=True)\n"
                    "    return registered\n"
                    "module.is_registered_worktree=collide_after_preflight\n"
                    "module.main()\n"
                )
                process = subprocess.run([sys.executable, '-u', '-c', harness], cwd=ROOT,
                                         capture_output=True, text=True, encoding='utf-8', timeout=30)
                output = process.stdout + process.stderr
                self.assertEqual(1, process.returncode, output)
                self.assertIn('FOREIGN_DIRECTORY_CREATED', output)
                self.assertNotIn('Traceback', output)
                self.assertTrue(candidate.is_dir(), output)
                self.assertEqual('foreign owner', (candidate / 'foreign-marker.txt').read_text(encoding='utf-8'))
                listing = subprocess.run(['git', 'worktree', 'list', '--porcelain'], cwd=ROOT,
                                         text=True, capture_output=True, check=True).stdout
                self.assertFalse(has_worktree_identity(listing, candidate))


        def test_dotnet_adapter_closes_inherited_stdin(self):
            adapter = ROOT / 'DynaDocs.Tests/coverage/run_tests.py'
            with tempfile.TemporaryDirectory(prefix='dydo-runner-stdin-') as temporary:
                namespace = runpy.run_path(adapter, run_name='run_tests_probe')
                globals_ = namespace['run_tests'].__globals__
                # create_worktree is mocked, so the snapshot is an empty directory, not a checkout: skill
                # link materialization is stubbed like copy_dirty_files, because mirroring real host links
                # into a snapshot with no skills/ tree must stay loud (test_runner.py pins that seam).
                events = []
                materialize = mock.Mock(side_effect=lambda worktree: events.append('materialize'))
                with (mock.patch.dict(globals_, {
                          'is_registered_worktree': mock.Mock(return_value=False),
                          'create_worktree': mock.Mock(return_value=True),
                          'copy_dirty_files': mock.Mock(),
                          'materialize_skill_links': materialize,
                          'remove_worktree': mock.Mock(),
                      }),
                      mock.patch.object(namespace['tempfile'], 'gettempdir', return_value=temporary),
                      mock.patch.object(namespace['uuid'], 'uuid4',
                                        return_value=type('Uuid', (), {'hex': 'stdin001'})()),
                      mock.patch.object(namespace['subprocess'], 'run') as run):
                    run.return_value.returncode = 0
                    run.side_effect = lambda cmd, **kwargs: events.append(cmd[0]) or run.return_value

                    self.assertEqual(0, namespace['run_tests']())

                    dotnet = next(call for call in run.call_args_list if call.args[0][0] == 'dotnet')
                    self.assertIs(subprocess.DEVNULL, dotnet.kwargs['stdin'])
                    # The DR 049 guard is not vacuous in the snapshot only if the run still
                    # materializes its skill links before dotnet starts.
                    materialize.assert_called_once_with(Path(temporary) / 'dydo-test-stdin001')
                    self.assertLess(events.index('materialize'), events.index('dotnet'))


        def test_interrupt_at_atomic_directory_acquisition_preserves_ownership(self):
            adapter = ROOT / 'DynaDocs.Tests/coverage/run_tests.py'
            for collision in [False, True]:
                for interrupt in ['SIGINT', *(['SIGBREAK'] if os.name == 'nt' else [])]:
                    with self.subTest(collision=collision, interrupt=interrupt), tempfile.TemporaryDirectory(prefix='dydo-acquire-signal-') as temporary:
                        candidate = Path(temporary) / 'dydo-test-ac0011ed'
                        harness = (
                            "import importlib.util,signal\nfrom pathlib import Path\nfrom types import SimpleNamespace\n"
                            + SCRIPT_PATH + f"spec=importlib.util.spec_from_file_location('adapter', {str(adapter)!r})\n"
                            "module=importlib.util.module_from_spec(spec); spec.loader.exec_module(module)\n"
                            f"module.tempfile.gettempdir=lambda: {temporary!r}\n"
                            "module.uuid.uuid4=lambda: SimpleNamespace(hex='ac0011ed')\n"
                            "original=Path.mkdir\n"
                            "class Collision(FileExistsError):\n"
                            "    def __str__(self):\n"
                            f"        signal.raise_signal(signal.{interrupt})\n"
                            "        return 'foreign collision'\n"
                            "def interrupted_mkdir(path, *args, **kwargs):\n"
                            "    original(path, *args, **kwargs)\n"
                            "    print('DIRECTORY_ACQUIRED', flush=True)\n"
                            f"    if {collision!r}:\n"
                            "        (path/'foreign-marker.txt').write_text('foreign owner', encoding='utf-8')\n"
                            "        raise Collision\n"
                            f"    signal.raise_signal(signal.{interrupt})\n"
                            "Path.mkdir=interrupted_mkdir\n"
                            "try: module.main()\n"
                            "except SystemExit:\n"
                            "    assert signal.getsignal(signal.SIGINT) == signal.default_int_handler\n"
                            "    if hasattr(signal, 'SIGBREAK'): assert signal.getsignal(signal.SIGBREAK) == signal.default_int_handler\n"
                            "    print('HANDLERS_RESTORED', flush=True)\n"
                            "    raise\n"
                        )
                        process = subprocess.run([sys.executable, '-u', '-c', harness], cwd=ROOT,
                                                 capture_output=True, text=True, encoding='utf-8', errors='replace', timeout=30)
                        output = process.stdout + process.stderr
                        self.assertEqual(130, process.returncode, output)
                        self.assertIn('DIRECTORY_ACQUIRED', output)
                        self.assertIn('HANDLERS_RESTORED', output)
                        self.assertNotIn('Worktree:', output)
                        self.assertNotIn('Traceback', output)
                        self.assertEqual(collision, candidate.exists(), output)
                        if collision:
                            self.assertEqual('foreign owner', (candidate / 'foreign-marker.txt').read_text(encoding='utf-8'))

"""Reproduce the pinned docs hop's native .NET reader failures in disposable input."""
from pathlib import Path
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[7]
sys.path.insert(0, str(ROOT / 'DynaDocs.Tests/coverage'))
import run_tests

copy = run_tests.copy_dirty_files


def old_readers(snapshot):
    copy(snapshot)
    for pattern in ('DynaDocs.Tests/Commands/CommandDocConsistencyTests*.cs',
                    'DynaDocs.Tests/Steps/CanonicalSkillSteps*.cs',
                    'DynaDocs.Tests/Steps/CanonicalSkillAssertionTests*.cs'):
        for path in snapshot.glob(pattern):
            relative = path.relative_to(snapshot).as_posix()
            old = subprocess.run(['git', 'show', 'c3f914d:' + relative], cwd=ROOT, capture_output=True)
            if old.returncode == 0:
                path.write_bytes(old.stdout)
            else:
                path.unlink()


run_tests.copy_dirty_files = old_readers
selection = ('FullyQualifiedName~CommandDocConsistencyTests|FullyQualifiedName~HelpCommandTests|'
             'FullyQualifiedName~ScaffoldTreeTests|FullyQualifiedName~OneCanonicalSkillTreeReachesEverySupportedHostFeature')
sys.exit(run_tests.run_tests(['-c', 'Release', '--filter', selection], job=run_tests.contain_descendants()))

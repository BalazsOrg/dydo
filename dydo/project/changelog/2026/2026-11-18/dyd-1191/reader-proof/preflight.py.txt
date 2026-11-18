"""Verify the selected native routes, versions and locked dependencies before Light."""
from pathlib import Path
import json
import shutil
import subprocess
import sys

root = Path(__file__).resolve().parents[7]
sys.path.insert(0, str(root / 'DynaDocs.Tests/coverage'))
from gate_versions import python_versions, javascript_versions
from run_tests import test_command

print(json.dumps(python_versions(root / 'DynaDocs.Tests/coverage'), indent=2))
coverage = root / 'DynaDocs.Tests/coverage'
node = javascript_versions(coverage)
print('Native static-linter locked packages:', len(node['packages']), 'lock SHA256:', node['lock_sha256'])
for command in (['dotnet', '--version'], ['dotnet', '--list-runtimes'], ['node', '--version']):
    assert shutil.which(command[0])
    subprocess.run(command, check=True)
print('Supported .NET argv:', test_command(['-c', 'Release', '--filter', 'FullyQualifiedName~CommandDocConsistencyTests']))
print('Python selection: unittest discover -s DynaDocs.Tests/coverage/tests -p test_testing_facade*.py')
print('Discovery IDs retained separately: 232 native Python, .NET command/help/Scaffold/canonical + facade scenarios')

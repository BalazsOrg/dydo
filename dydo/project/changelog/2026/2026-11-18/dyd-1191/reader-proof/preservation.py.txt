"""Check moved test bodies, native discovery, byte gates and owned-path boundaries."""
import ast
from collections import Counter
import hashlib
import gzip
import json
from pathlib import Path
import re
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[7]
HERE = Path(__file__).resolve().parent
HEAD = 'c3f914dff9a36426b722f05058eb3cbc7b967312'
sys.path.insert(0, str(ROOT / 'DynaDocs.Tests/coverage'))
from gate_discovery import ordinary_discovery


def git(*args):
    return subprocess.check_output(['git', *args], cwd=ROOT, text=True)


def methods(tree):
    return [method for cls in ast.walk(tree) if isinstance(cls, ast.ClassDef)
            for method in cls.body if isinstance(method, ast.FunctionDef)]


def native_lines(label, prefix):
    text = ''.join(p.read_text() for p in sorted(HERE.glob(label + '-*.log')))
    return [line.strip() for line in text.splitlines() if line.startswith(prefix)]


old = methods(ast.parse(git('show', HEAD + ':DynaDocs.Tests/coverage/tests/test_testing_facade.py')))
paths = sorted(ROOT.glob('DynaDocs.Tests/coverage/tests/test_testing_facade*.py'))
new = [method for path in paths for method in methods(ast.parse(path.read_text()))]
old_names = Counter(method.name for method in old)
new_names = Counter(method.name for method in new)
assert new_names == old_names + Counter({'policy_document': 1}), (old_names - new_names, new_names - old_names)
unchanged = lambda method: method.name not in ('policy_document', 'test_owned_policy_docs_replace_tiers_with_the_dr048_gate_set')
assert Counter(ast.dump(method) for method in old if unchanged(method)) == Counter(
    ast.dump(method) for method in new if unchanged(method))
print('PASS all', len(old), 'original Python methods preserved once; only the policy reader changes')

before = native_lines('discovery-python-before-host', 'test_testing_facade.')
after = native_lines('discovery-python-final', 'test_testing_facade.')
assert before == after and len(before) == 232 and len(set(after)) == 232
print('PASS native Python IDs identical: 232, base 116 + inherited portable 116; no duplicate discovery')
before_dotnet = native_lines('discovery-dotnet-before-host', '    ')
after_dotnet = native_lines('discovery-dotnet-after', '    ')
# Command JSON wraps at the start of each receipt; only native output after its header counts.
def dotnet_ids(label):
    text = ''.join(p.read_text() for p in sorted(HERE.glob(label + '-*.log')))
    native = text.split('The following Tests are available:\n', 1)[1].split('\n--- Running tests', 1)[0]
    return [line.strip() for line in native.splitlines() if line.startswith('    ')]
old_ids, new_ids = dotnet_ids('discovery-dotnet-before-host'), dotnet_ids('discovery-dotnet-after')
assert len(old_ids) == len(set(old_ids)) and set(old_ids) <= set(new_ids)
assert len(new_ids) == len(set(new_ids)) == 116
print('PASS native .NET moved selection:', len(old_ids), 'IDs retained; widened reached Light:', len(new_ids))

relative = [str(path.relative_to(ROOT)) for path in paths]
roles = ordinary_discovery(ROOT, relative)
assert {row['file'] for row in roles} == set(relative)
print('PASS all', len(roles), 'Python test/helper files have native unittest test role')

def unpack(folder, manifest):
    data = gzip.decompress(b''.join((folder / part).read_bytes() for part in manifest['gzip_parts']))
    assert hashlib.sha256(data).hexdigest() == manifest['sha256']
    assert len(data) == manifest['bytes']
    return data

archive = HERE / 'static-v4'
report = json.loads(unpack(archive, json.loads((archive / 'static-manifest.json').read_text())))
assert not report['findings'] and not report['errors'] and not report['csharp_namespace_cycles_report']
for path, digest in report['source_sha256'].items():
    assert hashlib.sha256((ROOT / path).read_bytes()).hexdigest() == digest, path
for manifest in report['raw'].values():
    unpack(archive, manifest)
print('PASS final static source binding and lossless integrity of', len(report['raw']), 'raw artifacts')
owned_worktrees = {path for log in HERE.glob('*.log')
                  for path in re.findall(r'Worktree: (/tmp/dydo-test-[0-9a-f]{8})', log.read_text())}
registrations = git('worktree', 'list', '--porcelain')
for path in owned_worktrees:
    assert not Path(path).exists() and 'worktree ' + path + '\n' not in registrations, path
print('PASS all', len(owned_worktrees), 'recorded owned test snapshots and registrations removed')

changed = set(git('diff', '--name-only', HEAD).splitlines())
changed.update(git('ls-files', '--others', '--exclude-standard').splitlines())
for path in changed:
    assert (path.startswith('dydo/project/changelog/2026/2026-11-18/dyd-1191/reader-proof/')
            or (path.startswith('DynaDocs.Tests/') and Path(path).name.startswith(
                ('CommandDocConsistencyTests', 'CanonicalSkillSteps', 'CanonicalSkillAssertionTests', 'test_testing_facade')))), path
print('PASS reader-hop changed paths all owned; original docs-hop raw evidence untouched')

all_changed = set(git('diff', '--name-only', '56b9fd8dee8c7af7d4174572454e0f9670a9d27f').splitlines())
all_changed.update(git('ls-files', '--others', '--exclude-standard').splitlines())
sizes = {p: (ROOT / p).stat().st_size for p in sorted(all_changed) if (ROOT / p).is_file()}
assert all(size < 20000 for size in sizes.values()), {p: size for p, size in sizes.items() if size >= 20000}
print('PASS whole-ticket changed-file byte gate:', len(sizes), 'files; largest', max(sizes.values()))
print('SOURCE DIGEST:', hashlib.sha256(json.dumps({str(p.relative_to(ROOT)): hashlib.sha256(p.read_bytes()).hexdigest()
                                              for p in paths}, sort_keys=True).encode()).hexdigest())

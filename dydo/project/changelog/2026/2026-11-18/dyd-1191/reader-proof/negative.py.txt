"""Run an unchanged native assertion after mutating only a disposable runner snapshot."""
import hashlib
from pathlib import Path
import re
import sys

ROOT = Path(__file__).resolve().parents[7]
sys.path.insert(0, str(ROOT / 'DynaDocs.Tests/coverage'))
import run_tests

mode = sys.argv[1]
hub = ROOT / 'dydo/reference/dydo-commands.md'
originals = {path: path.read_bytes() for path in [hub, *hub.parent.glob('dydo-commands-*.md')]}
copy = run_tests.copy_dirty_files


def mutated_snapshot(snapshot):
    copy(snapshot)
    edits = 0
    for original in originals:
        path = snapshot / original.relative_to(ROOT)
        text = path.read_text()
        if mode in ('option', 'parity'):
            altered = text.replace('--join', 'REMOVED-JOIN')
        elif mode == 'example':
            # Only executable example lines change; option tables and requiredness stay intact.
            altered = re.sub(r'(?m)^(dydo statechart install[^\n]*)--root', r'\1REMOVED-ROOT', text)
        elif mode == 'negative':
            altered = text + '\nRetired command probe: dydo task\n' if path.name.endswith('-utilities.md') else text
        elif mode == 'canonical':
            altered = text + '\nedit both\n' if path.name.endswith('-utilities.md') else text
        else:
            raise ValueError(mode)
        if altered != text:
            edits += 1
            path.write_text(altered)
    assert edits > 0, 'Probe did not change a real split page'
    print('Disposable mutated pages:', edits, 'mode:', mode, flush=True)


selections = {'option': 'CommandDocConsistencyTests.AllOptions_AppearInReferenceDoc',
              'parity': 'CommandDocConsistencyTests.ReferenceDocAndTemplate_HaveSameOptions',
              'example': 'CommandDocConsistencyTests.Examples_IncludeAllRequiredFlags',
              'negative': 'CommandDocConsistencyTests.ActiveProductDocs_ExcludeRetiredCommandsAndRepositoryWorkPaths',
              'canonical': 'OneCanonicalSkillTreeReachesEverySupportedHostFeature'}
run_tests.copy_dirty_files = mutated_snapshot
try:
    # The normal adapter owns, contains and cleans its scratch worktree and all children.
    code = run_tests.run_tests(['-c', 'Release', '--filter', 'FullyQualifiedName~' + selections[mode]],
                               job=run_tests.contain_descendants())
finally:
    for path, expected in originals.items():
        assert path.read_bytes() == expected, f'Source document changed: {path}'
    print('Original local family byte identity:', hashlib.sha256(b''.join(originals.values())).hexdigest())
sys.exit(code)

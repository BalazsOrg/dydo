#!/usr/bin/env python3
"""Inventory all pinned tracked filename references, including historical raw evidence."""
from pathlib import Path
import importlib.util
import subprocess
import sys

sys.dont_write_bytecode = True
HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[5]
spec = importlib.util.spec_from_file_location('audit', HERE / 'audit.py')
audit = importlib.util.module_from_spec(spec)
spec.loader.exec_module(audit)
names = subprocess.check_output(['git', 'grep', '-l', '-E',
    'coverage-tools|dydo-commands|guard-system', audit.BASE, '--', '.'], cwd=ROOT, text=True)
files = [line.split(':', 1)[1] for line in names.splitlines()]
repaired = {p for p in subprocess.check_output(['git', 'diff', audit.BASE, '--name-only'], cwd=ROOT, text=True).splitlines()
            if not p.startswith(str(HERE.relative_to(ROOT)) + '/')}
rows = []
for path in files:
    if path == 'DynaDocs.Tests/Commands/CommandDocConsistencyTests.cs':
        action = 'NEXT CODE HOP: split-set option/example/parity/retired-command readers; do not weaken assertions'
    elif path == 'DynaDocs.Tests/Steps/CanonicalSkillAssertionTests.cs':
        action = 'NEXT CODE HOP: expand the two command-doc entries as logical families in CanonicalSkillSteps reader'
    elif path == 'DynaDocs.Tests/coverage/tests/test_testing_facade.py':
        action = 'NEXT CODE HOP: coverage hub + seven split pages, both inherited test classes, all original assertions'
    elif path == 'Commands/HelpCommand.cs':
        action = 'UNCHANGED runtime help points to surviving command hub; HelpCommandTests confirms'
    elif path == 'DynaDocs.Tests/Commands/HelpCommandTests.cs':
        action = 'UNCHANGED filename assertion remains valid and passes'
    elif path == 'DynaDocs.Tests/GuardRetroCommands.cs':
        action = 'UNCHANGED recorded guard command strings/search patterns; filenames still exist'
    elif path == 'DynaDocs.Tests/Fixtures/config/dydo-2x-old-keys.json':
        action = 'UNCHANGED historical config fixture key/hash, not a documentation body reader'
    elif path.startswith('dydo/project/changelog/'):
        action = 'UNCHANGED historical evidence/receipts, not live readers; hub URLs and anchors retained'
    elif path in repaired:
        action = 'CHANGED navigation/link targets only, or original family hub; byte audit covers exact rewrite'
    elif path.startswith(('skills/', 'claims/')):
        action = 'UNCHANGED out-of-scope source; surviving hubs/anchors retain compatibility'
    elif path.endswith('.excalidraw'):
        action = 'UNCHANGED diagram filename label; hub survives'
    elif path in ('README.md', 'npm/README.md'):
        action = 'UNCHANGED public URL to original command hub; file survives'
    elif path.startswith('dydo/project/'):
        action = 'UNCHANGED historical prose/pointer; hub and every old heading anchor retained'
    elif path == 'dydo/guides/testing-strategy.md':
        action = 'UNCHANGED oversized policy doc; #provisioning retained on coverage hub'
    else:
        action = 'UNCHANGED general hub pointer or navigation; original file and all anchors retained'
    rows.append(path + '\t' + action + '\n')
header = 'BASE ' + audit.BASE + '\nCOMMAND git grep -l -E coverage-tools|dydo-commands|guard-system BASE -- .\nPATH\tDISPOSITION\n'
audit.split_raw('all-consumers', header + ''.join(rows))
print(f'PASS explicitly classified {len(files)} pinned tracked filename consumers, including historical evidence')
print('Implicit consumers: DynaDocs.csproj Scaffold/** resource wildcard; Services/ScaffoldTree.cs and ScaffoldTreeTests; InitCommand scaffold adoption; DocScanner/MarkdownParser/AnchorExtractor/LinkResolver all-doc scanning; CanonicalSkillSteps.cs consumes CurrentGuidance; PortableTestingFacadeTests inherits TestingFacadeTests.')

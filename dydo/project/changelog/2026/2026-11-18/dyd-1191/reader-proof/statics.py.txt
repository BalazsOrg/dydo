"""Native changed-file statics, retaining lossless compressed raw commands in small chunks."""
import ast
import gzip
import hashlib
import json
from pathlib import Path
import subprocess
import sys
import tempfile

ROOT = Path(__file__).resolve().parents[7]
HERE = Path(__file__).resolve().parent
if len(sys.argv) > 1:
    HERE = HERE / sys.argv[1]
    HERE.mkdir()
sys.path.insert(0, str(ROOT / 'DynaDocs.Tests/coverage'))
from gate_collect import Collectors, metric_findings
from gate_discovery import ordinary_discovery
from gate_inventory import dependency_cycles
from gate_run import CommandLog
from python_metrics import import_edges, source_metrics, module_scores, nested_ternaries
from gate_clones import collect_clones


def save(name, data):
    encoded = gzip.compress(data, mtime=0)
    parts = []
    for offset in range(0, len(encoded), 16000):
        part = f'{name}-{offset // 16000:03}.gzpart'
        (HERE / part).write_bytes(encoded[offset:offset + 16000])
        parts.append(part)
    return {'sha256': hashlib.sha256(data).hexdigest(), 'bytes': len(data), 'gzip_parts': parts}


changed = sorted({str(path.relative_to(ROOT)) for pattern in
                  ('DynaDocs.Tests/Commands/CommandDocConsistencyTests*.cs',
                   'DynaDocs.Tests/Steps/CanonicalSkill*.cs',
                   'DynaDocs.Tests/coverage/tests/test_testing_facade*.py') for path in ROOT.glob(pattern)})
python = [path for path in changed if path.endswith('.py')]
assert len(ordinary_discovery(ROOT, python)) == len(python)
with tempfile.TemporaryDirectory(prefix='dyd1191-statics-') as scratch:
    output = Path(scratch)
    collector = Collectors(ROOT, output)
    restore = collector.log.run('locked-restore', ['dotnet', 'restore', 'DynaDocs.Tests/DynaDocs.Tests.csproj', '--locked-mode', '-p:NuGetAudit=false'])
    assert restore['exit_code'] == 0, restore
    evaluated = collector.projects()
    assert not evaluated['errors'], evaluated
    collector.project_rows = [p for p in collector.project_rows if p['path'] == 'DynaDocs.Tests/DynaDocs.Tests.csproj']
    assert len(collector.project_rows) == 1 and collector.project_rows[0]['test']
    collector.inventory = {'sources': [{'path': path, 'language': 'cs' if path.endswith('.cs') else 'python', 'role': 'test'} for path in changed]}
    csharp = collector.csharp_source()
    assert not csharp['errors'], csharp['errors']
    findings = [f for f in csharp['findings'] if f.get('path') in changed]
    native = collector.csharp_analyzers()
    findings.extend(native['findings'])
    errors = native['errors']
    py_metrics = []
    for path in python:
        source = (ROOT / path).read_text()
        metrics = source_metrics(source)
        py_metrics.append({'path': path, **metrics, 'module': module_scores(source)})
        findings.extend(metric_findings(path, metrics['methods']))
        findings.extend({'path': path, 'gate': 'nested-ternary', 'line': line} for line in nested_ternaries(ast.parse(source)))
        if py_metrics[-1]['module']['cognitive'] > 20:
            findings.append({'path': path, 'gate': 'module-cognitive', **py_metrics[-1]['module']})
    # Dead-code names need the real program's readers; filter the verdict after analysis.
    selected_inventory = collector.inventory
    all_python = [str(p.relative_to(ROOT)) for p in ROOT.glob('DynaDocs.Tests/coverage/**/*.py')
                  if 'node_modules' not in p.parts and '__pycache__' not in p.parts]
    collector.inventory = {'sources': [{'path': p, 'language': 'python'} for p in all_python]}
    dead = collector.python_dead_code()
    collector.inventory = selected_inventory
    findings.extend(f for f in dead['findings'] if f.get('path', f.get('diagnostic', {}).get('filename', '')) in changed
                    or f.get('diagnostic', {}).get('filename', '').removeprefix(str(ROOT) + '/') in changed)
    errors.extend(dead['errors'])
    modules = {str(p.relative_to(ROOT)) for p in ROOT.glob('DynaDocs.Tests/coverage/**/*.py') if 'node_modules' not in p.parts and '__pycache__' not in p.parts}
    edges = [edge for path in sorted(modules) for edge in import_edges(path, ast.parse((ROOT / path).read_text()), modules,
             ['DynaDocs.Tests/coverage', 'DynaDocs.Tests/coverage/tests'])]
    cycles = [cycle for cycle in dependency_cycles(edges) if set(cycle) & set(python)]
    findings.extend({'gate': 'module-cycle', 'members': cycle} for cycle in cycles)
    clones = collect_clones(collector)
    findings.extend(clones['findings'])
    errors.extend(clones['errors'])
    receipts = {}
    for index, path in enumerate(sorted(output.rglob('*'))):
        if path.is_file():
            receipts[str(path.relative_to(output))] = save(f'static-raw-{index:03}', path.read_bytes())
    report = {'changed': changed, 'source_sha256': {p: hashlib.sha256((ROOT / p).read_bytes()).hexdigest() for p in changed},
              'roles': collector.inventory, 'findings': findings, 'errors': errors,
              'native_analyzers': native, 'python_metrics': py_metrics,
              'csharp_changed': [f for project in csharp['facts']['projects'] for f in project['files'] if f['path'] in changed],
              'csharp_namespace_cycles_report': [f for f in csharp['findings'] if f.get('gate') == 'namespace-cycle'],
              'python_changed_edges': [e for e in edges if set(e) & set(python)],
              'clones': clones, 'raw': receipts}
    saved = save('static-report', json.dumps(report, indent=2).encode())
    (HERE / 'static-manifest.json').write_text(json.dumps(saved, indent=2) + '\n')
    print(json.dumps({'findings': findings, 'errors': errors, 'report': saved, 'raw_files': len(receipts)}, indent=2))
    assert not findings and not errors

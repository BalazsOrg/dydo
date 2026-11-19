#!/usr/bin/env python3
"""Run hand-written modules with dydo's managed statechart capability.

Invoke through the host's existing gate lock. All checker outputs stay in the
new scratch directory; this runner never writes generated models or properties.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import shutil
import subprocess
import sys


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--out', required=True, type=Path)
    parser.add_argument('--skill', required=True, type=Path)
    parser.add_argument('--baseline', action='store_true')
    parser.add_argument('--controls', action='store_true', help='run deliberate bad observer traces')
    parser.add_argument('--machine', action='append')
    args = parser.parse_args()
    if args.baseline and args.controls:
        parser.error('--baseline and --controls are exclusive')
    root = Path(__file__).resolve().parents[2]
    scratch = args.out.resolve()
    scratch.mkdir(parents=True, exist_ok=False)
    environment = dict(os.environ)
    environment['JAVA_TOOL_OPTIONS'] = '-Xmx512m -XX:+UseParallelGC'
    source_head = subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=root, text=True).strip()
    controls = json.loads((Path(__file__).parent / 'controls.json').read_text()) if args.controls else {}
    records = []
    machines = sorted((root / 'claims/statechart').glob('*/*.tla'))
    if args.machine:
        machines = [p for p in machines if p.parent.name in args.machine]
        if {p.parent.name for p in machines} != set(args.machine):
            parser.error('unknown machine')
    for model in machines:
        name = model.parent.name
        work = scratch / name
        inputs = work / 'inputs'
        inputs.mkdir(parents=True)
        copied = []
        for source in sorted(model.parent.glob('*.tla')):
            shutil.copy2(source, inputs / source.name)
            copied.append({'path': str(source.relative_to(root)), 'sha256': digest(source)})
        config = inputs / model.with_suffix('.cfg').name
        shutil.copy2(model.with_suffix('.cfg'), config)
        copied.append({'path': str(model.with_suffix('.cfg').relative_to(root)),
                       'sha256': digest(config)})
        entry = inputs / model.name
        expected = ['NoDeadlock']
        if not args.baseline:
            prop = root / 'verification/statechart' / name / (model.stem + 'Properties.tla')
            cfg = prop.with_suffix('.cfg')
            shutil.copy2(prop, inputs / prop.name)
            entry = inputs / prop.name
            # Keep the model's original constraints; add the external property list.
            config = inputs / cfg.name
            original = model.with_suffix('.cfg').read_text()
            original = original.replace('INIT Init', 'INIT PropertyInit')
            original = original.replace('NEXT Next', 'NEXT PropertyNext')
            config.write_text(original + '\n' + cfg.read_text())
            expected.extend(re.findall(r'^(?:INVARIANT|PROPERTY) (\S+)$', cfg.read_text(), re.MULTILINE))
            copied.extend({'path': str(p.relative_to(root)), 'sha256': digest(p)}
                          for p in (prop, cfg))
            if args.controls:
                control = controls[name]
                declarations = re.search(r'^VARIABLES (.+)$', model.read_text(), re.MULTILINE)
                auxiliaries = [v.strip() for v in declarations[1].split(',') if v.strip() != 'state']
                values = control.get('variables', {})
                if set(values) != set(auxiliaries):
                    raise ValueError('control must explicitly initialize every auxiliary variable: ' + name)
                assignments = ['state = ' + json.dumps(control['from']), 'seen = {state}']
                assignments.extend(k + ' = ' + v for k, v in values.items())
                init = 'PropertyInit == ' + ' /\\ '.join(assignments)
                next_parts = ['state\' = ' + json.dumps(control['to']), "seen' = seen \\cup {state'}"]
                if auxiliaries:
                    next_parts.append('UNCHANGED <<' + ', '.join(auxiliaries) + '>>')
                next_action = 'PropertyNext == ' + ' /\\ '.join(next_parts)
                text = entry.read_text()
                text = re.sub(r'^PropertyInit ==.*$', lambda _: init, text, flags=re.MULTILINE)
                text = re.sub(r'^PropertyNext ==.*$', lambda _: next_action, text, flags=re.MULTILINE)
                entry.write_text(text)
        view = inputs / 'view.mjs'
        view.write_text('import * as original from ' + json.dumps((model.parent / 'view.mjs').as_uri()) + ';\n'
                        'export * from ' + json.dumps((model.parent / 'view.mjs').as_uri()) + ';\n'
                        'export const step = (action, from, to) => action === "PropertyNext"\n'
                        '  ? (from.state === to.state ? "update evidence" : "proceed to " + to.state)\n'
                        '  : original.step(action, from, to);\n')
        command = ['dydo', 'statechart', str(entry), '--skill', str(args.skill.resolve()),
                   '--config', str(config), '--view', str(view),
                   '--out', str(work / 'chart')]
        record = {'machine': name, 'command': command, 'inputs': copied,
                  'java_tool_options': environment['JAVA_TOOL_OPTIONS'],
                  'source_head': source_head, 'entry_sha256': digest(entry),
                  'config_sha256': digest(config), 'expected': expected}
        try:
            result = subprocess.run(command, cwd=inputs, env=environment, text=True,
                                    stdout=subprocess.PIPE, stderr=subprocess.STDOUT,
                                    timeout=240)
            (work / 'driver.log').write_text(result.stdout)
            record['exit'] = result.returncode
        except subprocess.TimeoutExpired as error:
            output = error.stdout or b''
            if isinstance(output, bytes):
                output = output.decode(errors='replace')
            (work / 'driver.log').write_text(output)
            record['exit'] = 'timeout'
        records.append(record)
        report = work / 'chart/report.md'
        if report.exists():
            report_text = report.read_text()
            record['broken'] = re.findall(r'^## (\S+) breaks$', report_text, re.MULTILINE)
            held = re.search(r'^## Rules that hold\n([^\n]+)', report_text, re.MULTILINE)
            record['holds'] = held[1].split(', ') if held else []
            record['missing'] = sorted(set(expected) - set(record['holds']) - set(record['broken']))
            record['not_checked'] = 'Rules not checked' in report_text or bool(record['missing'])
            if args.controls:
                record['expected_violation'] = controls[name]['property']
                record['control_caught'] = (record['exit'] == 1 and not record['not_checked']
                                             and controls[name]['property'] in record['broken'])
            record['result'] = ('blocked' if record['not_checked'] else
                                'property violated' if record['broken'] else
                                'pass' if record['exit'] == 0 else 'blocked')
        else:
            record['result'] = 'blocked'
        (scratch / 'runs.json').write_text(json.dumps(records, indent=2) + '\n')
        print(name + ': ' + record['result'] + ' (exit ' + str(record['exit']) + ')', flush=True)
    if args.controls:
        return int(not all(r.get('control_caught') for r in records))
    return int(any(r['result'] != 'pass' for r in records))


if __name__ == '__main__':
    sys.exit(main())

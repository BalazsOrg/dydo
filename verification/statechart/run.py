#!/usr/bin/env python3
"""Run independent properties through dydo's managed statechart capability.

Invoke through the existing gate lock. Checker outputs remain in new scratch
directories; this runner never writes model or property sources.
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


def arguments():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--out', required=True, type=Path)
    parser.add_argument('--skill', required=True, type=Path)
    parser.add_argument('--baseline', action='store_true')
    parser.add_argument('--controls', action='store_true', help='run deliberate bad observer traces')
    parser.add_argument('--machine', action='append')
    args = parser.parse_args()
    if args.baseline and args.controls:
        parser.error('--baseline and --controls are exclusive')
    return parser, args


def checkout_head(root):
    return subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=root, text=True).strip()


def checkout_status(root):
    return subprocess.check_output(['git', 'status', '--porcelain'], cwd=root, text=True).strip()


def selected_models(root, names, parser):
    machines = sorted((root / 'claims/statechart').glob('*/*.tla'))
    if names:
        machines = [p for p in machines if p.parent.name in names]
        if {p.parent.name for p in machines} != set(names):
            parser.error('unknown machine')
    return machines


def inject_control(model, entry, control):
    declarations = re.search(r'^VARIABLES (.+)$', model.read_text(), re.MULTILINE)
    auxiliaries = [v.strip() for v in declarations[1].split(',') if v.strip() != 'state']
    values = control.get('variables', {})
    if set(values) != set(auxiliaries):
        raise ValueError('control must explicitly initialize every auxiliary variable: ' + model.stem)
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


def copy_source(source, destination, root, copied):
    shutil.copy2(source, destination)
    copied.append({'path': str(source.relative_to(root)), 'sha256': digest(destination)})


def prepare_inputs(model, root, inputs, baseline, control):
    copied = []
    for source in sorted(model.parent.glob('*.tla')):
        copy_source(source, inputs / source.name, root, copied)
    config = inputs / model.with_suffix('.cfg').name
    copy_source(model.with_suffix('.cfg'), config, root, copied)
    entry = inputs / model.name
    expected = ['NoDeadlock']
    if not baseline:
        prop = root / 'verification/statechart' / model.parent.name / (model.stem + 'Properties.tla')
        cfg = prop.with_suffix('.cfg')
        entry = inputs / prop.name
        copy_source(prop, entry, root, copied)
        copied.append({'path': str(cfg.relative_to(root)), 'sha256': digest(cfg)})
        original = model.with_suffix('.cfg').read_text()
        if 'SPECIFICATION Spec' in original and not control:
            entry = fair_wrapper(inputs, prop.stem)
            original = original.replace('SPECIFICATION Spec', 'SPECIFICATION PropertySpec')
        else:
            original = original.replace('SPECIFICATION Spec', 'INIT Init\nNEXT Next')
            original = original.replace('INIT Init', 'INIT PropertyInit').replace('NEXT Next', 'NEXT PropertyNext')
        config = inputs / cfg.name
        config.write_text(original + '\n' + cfg.read_text())
        expected.extend(re.findall(r'^(?:INVARIANT|PROPERTY) (\S+)$', cfg.read_text(), re.MULTILINE))
        if control:
            inject_control(model, entry, control)
    return entry, config, expected, copied


def fair_wrapper(inputs, properties):
    """The observer under the model's own Fairness (a generated SPECIFICATION Spec); adds no assumption."""
    wrapper = inputs / (properties + 'Fair.tla')
    wrapper.write_text('---- MODULE ' + wrapper.stem + ' ----\nEXTENDS ' + properties + '\n'
                       'PropertySpec == PropertyInit /\\ [][PropertyNext]_<<vars, seen>> /\\ Fairness\n====\n')
    return wrapper


def prepare_view(model, inputs):
    view = inputs / 'view.mjs'
    original = json.dumps((model.parent / 'view.mjs').as_uri())
    view.write_text('import * as original from ' + original + ';\n'
                    'export * from ' + original + ';\n'
                    'export const step = (action, from, to) => action === "PropertyNext"\n'
                    '  ? (from.state === to.state ? "update evidence" : "proceed to " + to.state)\n'
                    '  : original.step(action, from, to);\n')
    return view


def invoke(command, inputs, environment, logfile):
    try:
        result = subprocess.run(command, cwd=inputs, env=environment, text=True,
                                stdout=subprocess.PIPE, stderr=subprocess.STDOUT, timeout=240)
        logfile.write_text(result.stdout)
        return result.returncode
    except subprocess.TimeoutExpired as error:
        output = error.stdout or b''
        if isinstance(output, bytes):
            output = output.decode(errors='replace')
        logfile.write_text(output)
        return 'timeout'


def classify(record, report, expected):
    record['result'] = 'blocked'
    if not report.exists():
        return
    text = report.read_text()
    record['broken'] = re.findall(r'^## (\S+) breaks$', text, re.MULTILINE)
    held = re.search(r'^## Rules that hold\n([^\n]+)', text, re.MULTILINE)
    record['holds'] = []
    if held:
        record['holds'] = held[1].split(', ')
    record['missing'] = sorted(set(expected) - set(record['holds']) - set(record['broken']))
    record['not_checked'] = 'Rules not checked' in text or bool(record['missing'])
    if record['not_checked'] or record['exit'] not in (0, 1):
        return
    if record['broken']:
        record['result'] = 'property violated'
    elif record['exit'] == 0:
        record['result'] = 'pass'


def check_machine(model, root, args, environment, source_head, control):
    work = args.out.resolve() / model.parent.name
    inputs = work / 'inputs'
    inputs.mkdir(parents=True)
    entry, config, expected, copied = prepare_inputs(model, root, inputs, args.baseline, control)
    view = prepare_view(model, inputs)
    command = ['dydo', 'statechart', str(entry), '--skill', str(args.skill.resolve()),
               '--config', str(config), '--view', str(view), '--out', str(work / 'chart')]
    record = {'machine': model.parent.name, 'command': command, 'inputs': copied,
              'java_tool_options': environment['JAVA_TOOL_OPTIONS'], 'source_head': source_head,
              'entry_sha256': digest(entry), 'config_sha256': digest(config),
              'runner_sha256': digest(Path(__file__)), 'expected': expected}
    record['exit'] = invoke(command, inputs, environment, work / 'driver.log')
    classify(record, work / 'chart/report.md', expected)
    if control:
        record['expected_violation'] = control['property']
        record['control_caught'] = (record['exit'] == 1 and not record.get('not_checked', True)
                                     and control['property'] in record.get('broken', []))
    return record


def save_records(scratch, records):
    (scratch / 'runs.json').write_text(json.dumps(records, indent=2) + '\n')


def finish(root, source_head, records, args):
    if checkout_status(root) or checkout_head(root) != source_head:
        for record in records:
            record['result'] = 'blocked'
            record['reason'] = 'checkout changed during the run'
        save_records(args.out.resolve(), records)
        print('blocked: checkout changed during the run', file=sys.stderr)
        return 2
    if args.controls:
        return int(not all(r.get('control_caught') for r in records))
    return int(any(r['result'] != 'pass' for r in records))


def main():
    parser, args = arguments()
    root = Path(__file__).resolve().parents[2]
    if checkout_status(root):
        parser.error('checkout is dirty; commit the intended verification inputs before running')
    source_head = checkout_head(root)
    machines = selected_models(root, args.machine, parser)
    args.out.resolve().mkdir(parents=True, exist_ok=False)
    environment = dict(os.environ)
    environment['JAVA_TOOL_OPTIONS'] = '-Xmx512m -XX:+UseParallelGC'
    controls = {}
    if args.controls:
        controls = json.loads((Path(__file__).parent / 'controls.json').read_text())
    records = []
    for model in machines:
        record = check_machine(model, root, args, environment, source_head, controls.get(model.parent.name))
        records.append(record)
        save_records(args.out.resolve(), records)
        print(record['machine'] + ': ' + record['result'] + ' (exit ' + str(record['exit']) + ')', flush=True)
    return finish(root, source_head, records, args)


if __name__ == '__main__':
    sys.exit(main())

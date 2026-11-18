#!/usr/bin/env python3
"""DYD-1191 retained documentation audit. No product/runtime imports or writes by default.

Rebuild the expected split directly from the pinned Git blobs, then compare every byte.
--write is the original deterministic authoring operation; do not use it after reader edits.
"""
from pathlib import Path
import argparse
import hashlib
import json
import os
import re
import subprocess

ROOT = Path(__file__).resolve().parents[6]
EVIDENCE = Path(__file__).resolve().parent
BASE = '56b9fd8dee8c7af7d4174572454e0f9670a9d27f'


def git(*args):
    return subprocess.check_output(['git', *args], cwd=ROOT)


def headings(text):
    """ATX headings outside fenced code, including their original byte/text positions."""
    result, fence, offset = [], None, 0
    for line in text.splitlines(keepends=True):
        match = re.match(r'^\s{0,3}(`{3,}|~{3,})', line)
        if match:
            marker = match[1]
            if fence is None:
                fence = marker
            elif marker[0] == fence[0] and len(marker) >= len(fence):
                fence = None
        elif fence is None:
            match = re.match(r'^(#{1,6}) (.+?)\n?$', line)
            if match:
                result.append((offset, len(match[1]), match[2]))
        offset += len(line)
    return result


def anchor(title):
    # Matches Services/AnchorExtractor.cs for these literal, non-inline-code headings.
    return re.sub(r'[^\w-]', '', title.lower().replace(' ', '-'))


def chunks(path, title, groups, intro_owner=None):
    source = git('show', f'{BASE}:{path}').decode()
    hs = headings(source)
    by_title = {text: start for start, _, text in hs}
    first = next(start for start, level, _ in hs if level == 2)
    related = by_title['Related']
    prefix = source[:first]
    definitions = []
    if intro_owner:
        start = source.index('\n\n', source.index('# ')) + 2
        definitions.append((intro_owner, 'Coverage Tools: Platform Measurement', start, first))
        prefix = source[:start]
    for suffix, page_title, start_title, end_title in groups:
        definitions.append((suffix, page_title, by_title[start_title],
                            by_title[end_title] if end_title else len(source)))
    related_end = next((s for s, level, _ in hs if s > related and level == 2), len(source))
    intervals = [(0, len(prefix)), (related, related_end)] + [(b, e) for _, _, b, e in definitions]
    cursor = 0
    for begin, end in sorted(intervals):
        assert begin == cursor, (path, 'gap/overlap in source byte partition', cursor, begin)
        cursor = end
    assert cursor == len(source), (path, 'unassigned source tail')
    return dict(path=path, title=title, source=source, headings=hs,
                prefix=prefix, related=source[related:related_end], definitions=definitions)


def families():
    command_groups = [
        ('core', 'CLI Commands: Setup, Documentation and Guard', 'Setup Commands', 'Linear Commands'),
        ('linear', 'CLI Commands: Linear and Mode', 'Linear Commands', 'Worktree Commands'),
        ('worktrees', 'CLI Commands: Archive and Clean', 'Worktree Commands', 'dydo pr-ready'),
        ('delivery', 'CLI Commands: PR Readiness and Merge', 'dydo pr-ready', 'Validation Command'),
        ('utilities', 'CLI Commands: Validation, Testing and Utilities', 'Validation Command', 'Related'),
    ]
    return [
        chunks('dydo/reference/coverage-tools.md', 'Coverage and Assurance Tools', [
            ('provisioning', 'Coverage Tools: Provisioning', 'Provisioning', 'Operations'),
            ('running', 'Coverage Tools: Operations and Declared Rows', 'Operations', 'Row states, exits and evidence'),
            ('results', 'Coverage Tools: Results and Adapter Summary', 'Row states, exits and evidence', 'Static collectors'),
            ('statics', 'Coverage Tools: Static and Statechart Collectors', 'Static collectors', 'Coverage collectors'),
            ('coverage', 'Coverage Tools: Coverage Collectors', 'Coverage collectors', 'The C# campaign'),
            ('measurement', 'Coverage Tools: C# Measurement and Provenance', 'The C# campaign', 'Related'),
        ], intro_owner='platforms'),
        chunks('dydo/reference/dydo-commands.md', 'CLI Commands Reference', command_groups),
        chunks('Scaffold/dydo/reference/dydo-commands.md', 'CLI Commands Reference', command_groups),
        chunks('dydo/understand/guard-system.md', 'Guard System', [
            ('hook', 'Guard System: Hook and Integration', 'How the Hook Intercepts Tool Calls', 'Layer 1: Path Tiers'),
            ('paths', 'Guard System: Path Tiers', 'Layer 1: Path Tiers', 'Layer 2: Bash Command Analysis'),
            ('bash', 'Guard System: Bash Command Analysis', 'Layer 2: Bash Command Analysis', 'David Ondrej denylist audit'),
            ('denylist', 'Guard System: Denylist Audit', 'David Ondrej denylist audit', 'Layer 3: Nudges'),
            ('nudges', 'Guard System: Nudges', 'Layer 3: Nudges', 'Also Enforced'),
            ('enforcement', 'Guard System: Additional Enforcement', 'Also Enforced', 'Housekeeping Rides Along'),
            ('hook', 'Guard System: Hook and Integration', 'Housekeeping Rides Along', 'Related'),
            ('enforcement', 'Guard System: Additional Enforcement', 'Archive-before-delete guard', None),
        ]),
    ]


def page_path(family, suffix):
    path = Path(family['path'])
    return str(path.with_name(f'{path.stem}-{suffix}.md'))


def owner_map(fs):
    owners = {}
    for family in fs:
        for start, _, title in family['headings']:
            owner = family['path']
            for suffix, _, begin, end in family['definitions']:
                if begin <= start < end:
                    owner = page_path(family, suffix)
                    break
            owners[(family['path'], anchor(title))] = owner
    return owners


LINK = re.compile(r'(?<=\]\()([^\s()]+)(?=\))')


def rewrite_links(text, source_path, dest_path, owners):
    def replace(match):
        target = match[1]
        if '#' not in target or re.match(r'\w+://', target):
            return target
        file, fragment = target.split('#', 1)
        resolved = str(Path(os.path.normpath(str(Path(source_path).parent / file)))) if file else source_path
        owner = owners.get((resolved, fragment))
        if owner is None:
            # Split pages retain the old directory: nonfamily links need no relocation.
            return target
        relative = os.path.relpath(owner, Path(dest_path).parent)
        return ('#' + fragment) if owner == dest_path else './' + relative + '#' + fragment
    return LINK.sub(replace, text)


def expected(fs, owners):
    files, mappings = {}, []
    for family in fs:
        path = family['path']
        source = family['source']
        area = 'understand' if '/understand/' in path else 'reference'
        nav = f'[{family["title"]}](./{Path(path).name})'
        page_parts = {}
        for suffix, title, begin, end in family['definitions']:
            page = page_path(family, suffix)
            if page not in page_parts:
                page_parts[page] = f'---\narea: {area}\ntype: {"concept" if area == "understand" else "reference"}\n---\n\n# {title}\n\n{nav}\n\n'
            original = source[begin:end]
            body = rewrite_links(original, path, page, owners)
            page_parts[page] += body
            mappings.append(dict(source=path, destination=page,
                                 start_byte=len(source[:begin].encode()), end_byte=len(source[:end].encode()),
                                 original_sha256=hashlib.sha256(original.encode()).hexdigest()))
        for page, body in page_parts.items():
            files[page] = body.rstrip('\n') + f'\n\n## Related\n\n- {nav}\n'
        hub = family['prefix'].replace('type: reference', 'type: hub').replace('type: concept', 'type: hub')
        if path.endswith('coverage-tools.md'):
            hub += '## Platform Measurement\n\n[Platform Measurement](./coverage-tools-platforms.md)\n\n'
        for start, level, title in family['headings']:
            if level == 1 or title == 'Related':
                continue
            owner = owners[(path, anchor(title))]
            if owner == path:
                continue
            hub += f'{"#" * level} {title}\n\n[{title}](./{Path(owner).name}#{anchor(title)})\n\n'
        hub += family['related']
        files[path] = rewrite_links(hub, path, path, owners).rstrip('\n') + '\n'
    return files, mappings


def split_raw(name, text):
    # Raw logs are data, but also stay strictly below the authored-file limit.
    parts, current = [], ''
    for line in text.splitlines(keepends=True):
        assert len(line.encode()) < 18000, (name, 'oversize line')
        if len((current + line).encode()) >= 18000:
            parts.append(current)
            current = ''
        current += line
    parts.append(current)
    for i, part in enumerate(parts, 1):
        (EVIDENCE / f'{name}-{i:02}.log').write_text(part)


def run(write=False, docs_only=False):
    fs = families()
    owners = owner_map(fs)
    files, mappings = expected(fs, owners)
    tracked_docs = git('ls-files', '*.md').decode().splitlines()
    repairs = []
    for path in tracked_docs:
        if path in files or path.startswith(('skills/', 'claims/', 'dydo/project/changelog/')):
            continue
        if not path.startswith(('dydo/', 'Scaffold/dydo/')):
            continue
        before = git('show', f'{BASE}:{path}').decode()
        after = rewrite_links(before, path, path, owners)
        if before != after:
            if len(after.encode()) >= 20000:
                # Existing inbound hub anchors are preserved. Do not make an oversized
                # unrelated page a changed file merely to shorten a navigation hop.
                if write and (ROOT / path).read_bytes() == after.encode():
                    (ROOT / path).write_bytes(before.encode())
                print(f'UNCHANGED oversized inbound document uses preserved hub anchors: {path}')
                continue
            files[path] = after
            repairs.append(path)
    # Folder navigation is authored, so explicitly enumerate the new family pages.
    for path in ['dydo/reference/_reference.md', 'Scaffold/dydo/reference/_reference.md', 'dydo/understand/_understand.md']:
        text = files.get(path, git('show', f'{BASE}:{path}').decode())
        additions = [f'- [{headings(files[p])[0][2]}](./{Path(p).name})\n' for p in files
                     if Path(p).parent == Path(path).parent and p not in [f['path'] for f in fs]
                     and any(p == page_path(f, d[0]) for f in fs for d in f['definitions'])]
        insert = text.index('\n## ', text.index('## Contents') + len('## Contents'))
        files[path] = text[:insert] + ''.join(additions) + '\n' + text[insert:]
    if write:
        for path, text in files.items():
            (ROOT / path).write_bytes(text.encode())
        before_log = EVIDENCE / 'consumers-before.log'
        if before_log.exists():
            split_raw('consumers-before', before_log.read_text())
            before_log.unlink()
        for family in fs:
            subset = [m for m in mappings if m['source'] == family['path']]
            label = family['path'].replace('/', '-').removesuffix('.md')
            (EVIDENCE / f'mapping-{label}.json').write_text(json.dumps(subset, indent=2) + '\n')
            anchors = {fragment: owner for (original, fragment), owner in owners.items() if original == family['path']}
            (EVIDENCE / f'anchors-{label}.json').write_text(json.dumps(anchors, indent=2) + '\n')
    for path, wanted in files.items():
        actual = (ROOT / path).read_bytes()
        assert actual == wanted.encode(), f'Byte-preservation/navigation mismatch: {path}'
        assert len(actual) < 20000, (path, len(actual))
        print(f'PASS exact pinned-body reconstruction + size {len(actual):5d} bytes: {path}')
    family_paths = {f['path'] for f in fs}
    anchor_count = 0
    for family in fs:
        old_anchors = {anchor(t) for _, _, t in family['headings']}
        hub_anchors = {anchor(t) for _, _, t in headings((ROOT / family['path']).read_text())}
        assert old_anchors <= hub_anchors, (family['path'], old_anchors - hub_anchors)
        for key, owner in owners.items():
            if key[0] == family['path']:
                assert key[1] in {anchor(t) for _, _, t in headings((ROOT / owner).read_text())}, key
        anchor_count += len(old_anchors)
        print(f'PASS all {len(old_anchors)} original heading anchors retained: {family["path"]}')
    inbound = 0
    for path in tracked_docs + list(set(files) - set(tracked_docs)):
        text = (ROOT / path).read_text()
        for match in LINK.finditer(text):
            target = match[1]
            if '#' not in target or re.match(r'\w+://', target):
                continue
            file, fragment = target.split('#', 1)
            resolved = str(Path(os.path.normpath(str(Path(path).parent / file)))) if file else path
            if resolved not in files or not any(resolved.startswith(str(Path(f).with_suffix(''))) for f in family_paths):
                continue
            assert fragment in {anchor(t) for _, _, t in headings((ROOT / resolved).read_text())}, (path, target)
            inbound += 1
    # Enforce scope, including staged edits and untracked evidence.
    changed = set(git('diff', BASE, '--name-only').decode().splitlines())
    changed.update(git('ls-files', '--others', '--exclude-standard').decode().splitlines())
    for path in changed:
        if docs_only and path.startswith('DynaDocs.Tests/'):
            continue
        assert path in files or path.startswith(str(EVIDENCE.relative_to(ROOT)) + '/'), f'Out of scope: {path}'
        if (ROOT / path).is_file():
            assert (ROOT / path).stat().st_size < 20000, ('changed-file size', path)
    assert not git('diff', BASE, '--', 'skills/', 'claims/')
    if not docs_only:
        assert not git('diff', BASE, '--', 'DynaDocs.Tests/')
    identity = '\n'.join(p + '\t' + hashlib.sha256((ROOT / p).read_bytes()).hexdigest() for p in sorted(files))
    print('DOC SNAPSHOT SHA256:', hashlib.sha256(identity.encode()).hexdigest())
    print(f'PASS {len(mappings)} byte-range moves, {anchor_count} old hub anchors, {inbound} inbound family fragment links')
    print(f'PASS {len(repairs)} inbound link-only repairs; no skills/claims changes; all docs/evidence changed files <20000 bytes')
    print('TEST SCOPE:', 'excluded: next code hop must audit all reader/helper/test sizes and scope separately' if docs_only else 'no test changes')
    print('PASS Scaffold command pages reconstructed independently from Scaffold base; intentional differences retained')
    print('BASE INVENTORY: no Scaffold/dydo/reference/coverage-tools.md or Scaffold/dydo/understand/guard-system.md; none invented')
    return files


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--write', action='store_true')
    parser.add_argument('--docs-only', action='store_true')
    args = parser.parse_args()
    run(args.write, args.docs_only)

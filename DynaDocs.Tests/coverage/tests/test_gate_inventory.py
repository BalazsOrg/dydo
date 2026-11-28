"""Vendored upstream scripts in the source inventory: excluded while byte-identical, measured once changed.

Each case writes a throwaway root holding a skill's scripts beside its PROVENANCE record and reads
the inventory assemble_inventory builds from it, the inventory every static and coverage row uses."""
import hashlib
import sys
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from gate_inventory import assemble_inventory, upstream_blobs

_REPOSITORY = Path(__file__).resolve().parents[3]
_ANIMATIC = 'skills/productivity/animatic/scripts'
_ANIMATIC_RECORD = 'skills/productivity/animatic/PROVENANCE.md'
_ANIMATIC_PIN = '71009050da79506823464512ee76d71bbc014ce1'
_STATECHART = 'skills/engineering/statechart/scripts'
_STATECHART_RECORD = 'skills/engineering/statechart/PROVENANCE.md'
_STATECHART_PIN = '1ba91fc25c0977741dd0e267849e6234b037ed0a'


def _blob(text):
    data = text.encode('utf-8')
    return hashlib.sha1(b'blob %d\0' % len(data) + data).hexdigest()


def _record(pin, upstream):
    lines = ''.join(f'{_blob(text)}  scripts/{path}\n' for path, text in sorted(upstream.items()))
    return f'# Provenance\n\nSource: commit `{pin}`.\n\n```upstream-blobs\n{lines}```\n'


def _inventory(folder, files, record_path, record):
    """`files` under `folder` beside `record` at `record_path`, as the gates' inventory sees them."""
    with tempfile.TemporaryDirectory() as scratch:
        root = Path(scratch)
        for path, text in files.items():
            (root / folder / path).parent.mkdir(parents=True, exist_ok=True)
            (root / folder / path).write_text(text, encoding='utf-8', newline='\n')
        (root / record_path).write_text(record, encoding='utf-8')
        return assemble_inventory(root, sorted(f'{folder}/{path}' for path in files), [], [])


class VendoredUpstreamInventoryTests(unittest.TestCase):
    UPSTREAM = {'animatic.mjs': 'import { frameHtml } from "./frames.mjs";\nframeHtml();\n',
                'frames.mjs': 'export const frameHtml = () => 1;\n'}

    def test_a_byte_identical_animatic_script_is_excluded_with_its_upstream_origin(self):
        """Job: contract: testing-strategy.md "What this repository measures", a vendored upstream
        animatic script is recorded in `excluded` with the commit, path and blob it matches"""
        report = _inventory(_ANIMATIC, self.UPSTREAM, _ANIMATIC_RECORD, _record(_ANIMATIC_PIN, self.UPSTREAM))

        self.assertEqual([], report['errors'])
        self.assertEqual([], report['sources'])
        self.assertEqual([{'path': f'{_ANIMATIC}/animatic.mjs', 'reason': 'vendored-upstream',
                           'origin': {'root': f'{_ANIMATIC}/', 'upstream': {
                               'record': _ANIMATIC_RECORD, 'commit': _ANIMATIC_PIN,
                               'path': 'skills/in-progress/animatic/scripts/animatic.mjs',
                               'blob': _blob(self.UPSTREAM['animatic.mjs'])}}},
                          {'path': f'{_ANIMATIC}/frames.mjs', 'reason': 'vendored-upstream',
                           'origin': {'root': f'{_ANIMATIC}/', 'upstream': {
                               'record': _ANIMATIC_RECORD, 'commit': _ANIMATIC_PIN,
                               'path': 'skills/in-progress/animatic/scripts/frames.mjs',
                               'blob': _blob(self.UPSTREAM['frames.mjs'])}}}],
                         sorted(report['excluded'], key=lambda row: row['path']))

    def test_changed_bytes_or_an_unlisted_script_make_an_animatic_script_measured(self):
        """Job: trap: gate_inventory exclusion, an edited upstream script or a new local one leaves
        the vendored exclusion and becomes a measured JavaScript target with no list to update"""
        local = {**self.UPSTREAM, 'frames.mjs': self.UPSTREAM['frames.mjs'] + '// local\n',
                 'extra.mjs': 'export const extra = 1;\n'}

        report = _inventory(_ANIMATIC, local, _ANIMATIC_RECORD, _record(_ANIMATIC_PIN, self.UPSTREAM))

        self.assertEqual([], report['errors'])
        self.assertEqual([(f'{_ANIMATIC}/extra.mjs', 'javascript', 'target'),
                          (f'{_ANIMATIC}/frames.mjs', 'javascript', 'target')],
                         [(row['path'], row['language'], row['role']) for row in report['sources']])
        self.assertEqual([f'{_ANIMATIC}/animatic.mjs'], [row['path'] for row in report['excluded']])

    def test_an_unreadable_animatic_record_measures_every_script_and_names_the_gap(self):
        """Job: edge: gate_inventory exclusion, a missing blob block excludes nothing and is an error"""
        report = _inventory(_ANIMATIC, self.UPSTREAM, _ANIMATIC_RECORD, f'commit `{_ANIMATIC_PIN}` only\n')

        self.assertEqual(sorted(f'{_ANIMATIC}/{path}' for path in self.UPSTREAM),
                         [row['path'] for row in report['sources']])
        self.assertEqual([(_ANIMATIC_RECORD, 'upstream-provenance-unreadable')],
                         [(row['path'], row['type']) for row in report['errors']])

    def test_the_statechart_typescript_rule_keeps_its_recorded_gap(self):
        """Job: trap: gate_inventory exclusion, generalising the rule leaves statechart TypeScript
        under DYD-590's gap reason and its JavaScript measured"""
        upstream = {'ir.ts': 'export const ir = 1;\n'}
        local = {**upstream, 'test/corpus/Login/view.mjs': 'export const view = 1;\n'}

        report = _inventory(_STATECHART, local, _STATECHART_RECORD, _record(_STATECHART_PIN, upstream))

        self.assertEqual([(f'{_STATECHART}/test/corpus/Login/view.mjs', 'javascript')],
                         [(row['path'], row['language']) for row in report['sources']])
        self.assertEqual([{'path': f'{_STATECHART}/ir.ts', 'reason': 'typescript-outside-viewer-src',
                           'origin': {'root': f'{_STATECHART}/', 'gap': 'dydo/guides/testing-strategy.md',
                                      'issue': 'DYD-590', 'upstream': {
                                          'record': _STATECHART_RECORD, 'commit': _STATECHART_PIN,
                                          'path': 'pstack/skills/statechart/scripts/ir.ts',
                                          'blob': _blob(upstream['ir.ts'])}}}],
                         report['excluded'])

    def test_the_scripts_the_committed_record_lists_are_excluded_while_byte_identical(self):
        """Job: contract: skills/productivity/animatic/PROVENANCE.md, every script the record lists
        is checked out byte-identical to its blob and excluded; any other script is not vendored"""
        listed = _listed(_REPOSITORY)
        on_disk = _scripts(_REPOSITORY)

        report = assemble_inventory(_REPOSITORY, on_disk, [], [])

        self.assertTrue(listed)
        self.assertEqual([], report['errors'])
        self.assertEqual(listed, sorted(row['path'] for row in report['excluded']
                                        if row['reason'] == 'vendored-upstream'))

    def test_a_changed_or_unlisted_script_beside_the_committed_record_is_measured(self):
        """Job: trap: skills/productivity/animatic/PROVENANCE.md, the committed record excludes no
        script it does not list and none whose bytes left its blob"""
        listed = _listed(_REPOSITORY)
        changed, unlisted = listed[0], f'{_ANIMATIC}/dydo-local.mjs'
        with tempfile.TemporaryDirectory() as scratch:
            root = Path(scratch)
            for relative in [*_scripts(_REPOSITORY), _ANIMATIC_RECORD]:
                (root / relative).parent.mkdir(parents=True, exist_ok=True)
                (root / relative).write_bytes((_REPOSITORY / relative).read_bytes())
            with (root / changed).open('ab') as script:
                script.write(b'// dydo edit\n')
            (root / unlisted).write_text('export const local = 1;\n', encoding='utf-8', newline='\n')

            report = assemble_inventory(root, _scripts(root), [], [])

        measured = [row['path'] for row in report['sources']]
        excluded = [row['path'] for row in report['excluded']]
        self.assertEqual([], report['errors'])
        self.assertIn(changed, measured)
        self.assertIn(unlisted, measured)
        self.assertEqual(sorted(set(listed) - {changed}), sorted(excluded))


def _listed(root):
    """The repository paths of the scripts the animatic PROVENANCE record lists."""
    record = upstream_blobs((root / _ANIMATIC_RECORD).read_text(encoding='utf-8'))
    return sorted(f'{_ANIMATIC}/{path.removeprefix("scripts/")}' for path in record['blobs'])


def _scripts(root):
    """Every file under the animatic scripts folder, as repository paths."""
    return sorted(path.relative_to(root).as_posix() for path in (root / _ANIMATIC).rglob('*') if path.is_file())


if __name__ == '__main__':
    unittest.main()

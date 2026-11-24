"""verify-dydo's driver at its command line: what a run's ownership, cleanup and candidate checks leave on disk.

Each test drives control_dydo.main with the driver's own verbs. None needs a built candidate: every
outcome here is decided before the driver would start the candidate CLI."""
import contextlib
import io
import json
import os
from pathlib import Path
import subprocess
import sys
import tempfile
import unittest
from unittest.mock import patch

ROOT = Path(__file__).resolve().parents[3]
sys.path.insert(0, str(ROOT / "skills/engineering/verify-dydo/scripts"))
import control_dydo as control


def link(source, target):
    """A directory link, as a junction on Windows so no symlink privilege is needed."""
    if os.name == "nt":
        subprocess.run(["cmd", "/c", "mklink", "/J", str(source), str(target)], check=True, capture_output=True)
    else:
        source.symlink_to(target, target_is_directory=True)


class ControlDydoTests(unittest.TestCase):
    def setUp(self):
        temporary = tempfile.TemporaryDirectory(prefix="dydo-control-test-")
        self.addCleanup(temporary.cleanup)
        self.parent = Path(temporary.name)
        runs = patch.object(control, "RUNS", self.parent / "runs")
        runs.start()
        self.addCleanup(runs.stop)
        self.run_path = self.fresh_run()

    @staticmethod
    def fresh_run():
        run = control.allocate()
        (run / "scratch/project").mkdir()
        (run / "scratch/project/file.txt").write_text("scratch")
        (run / "evidence/candidate.json").write_text("{}")
        return run

    def main(self, verb, run=None):
        with contextlib.redirect_stdout(io.StringIO()):
            return control.main([verb, "--run", str(run or self.run_path)])

    def test_cleanup_deletes_only_scratch_keeps_evidence_and_repeats(self):
        """Job: contract: verify-dydo SKILL.md Cleanup, deletes only RUN/scratch and is repeatable"""
        self.assertEqual(0, self.main("cleanup"))
        self.assertEqual(0, self.main("cleanup"))

        self.assertFalse((self.run_path / "scratch").exists())
        self.assertTrue((self.run_path / "evidence/candidate.json").exists())
        self.assertTrue(json.loads((self.run_path / "evidence/cleanup.json").read_text())["scratch_absent"])

    def test_cleanup_refuses_a_run_it_does_not_own_and_deletes_nothing(self):
        """Job: trap: verify-dydo SKILL.md Cleanup, validates the exact run and its tree before deleting"""
        foreign = self.parent / "foreign"
        foreign.mkdir()
        (foreign / "keep.txt").write_text("foreign")

        def linked_scratch():
            run = self.fresh_run()
            link(run / "scratch/project/escape", foreign)
            self.addCleanup(os.rmdir if os.name == "nt" else os.unlink, run / "scratch/project/escape")
            return run

        def foreign_owner():
            run = self.fresh_run()
            control.write_json(run / "owner.json", {"schema": 1, "run": str(run), "checkout": "elsewhere"})
            return run

        def outside_runs():
            run = self.parent / "0123456789abcdef0123456789abcdef"
            (run / "scratch/project").mkdir(parents=True)
            (run / "evidence").mkdir()
            (run / "scratch/project/file.txt").write_text("scratch")
            control.write_json(run / "owner.json", {"schema": 1, "run": str(run), "checkout": str(control.ROOT)})
            return run

        for name, arrange in [("scratch links outside the run", linked_scratch),
                              ("owner names another checkout", foreign_owner),
                              ("run lies outside the driver's runs", outside_runs)]:
            with self.subTest(name):
                run = arrange()

                with self.assertRaises(ValueError):
                    self.main("cleanup", run)

                self.assertEqual("scratch", (run / "scratch/project/file.txt").read_text())
                self.assertEqual("foreign", (foreign / "keep.txt").read_text())

    def test_a_busy_run_belongs_to_its_live_driver(self):
        """Job: contract: verify-dydo SKILL.md Cleanup, a busy run belongs to the live driver"""
        (self.run_path / "busy").write_text("live driver")

        with self.assertRaises(FileExistsError):
            self.main("cleanup")

        self.assertEqual("live driver", (self.run_path / "busy").read_text())
        self.assertTrue((self.run_path / "scratch/project/file.txt").exists())

    def test_doctor_on_a_changed_candidate_refuses_before_the_candidate_runs(self):
        """Job: contract: verify-dydo SKILL.md doctor, a run whose recorded candidate no longer matches the checkout is refused before the candidate runs"""
        project = self.run_path / "scratch/project"
        (project / ".git").mkdir()
        (project / ".claude").mkdir()
        control.write_json(project / ".claude/settings.json",
                           {"hooks": {"PreToolUse": [{"hooks": [{"command": "dydo guard --host claude"}]}]}})
        for name in control.FIXTURE_FILES:
            (project / name).parent.mkdir(parents=True, exist_ok=True)
            if not (project / name).exists():
                (project / name).write_text("fixture")
        (self.run_path / "scratch/bin").mkdir()
        stale = {"head": "0" * 40, "diff_sha256": "", "untracked": {}}
        control.write_json(self.run_path / "state.json", {"phase": "ready", "candidate": stale, "build": {},
                                                          "fixture": control.fixture_hashes(self.run_path)})

        with self.assertRaises(ValueError):
            self.main("doctor")

        receipts = [json.loads(path.read_text()) for path in (self.run_path / "evidence").glob("*.json")]
        self.assertFalse([row for row in receipts if row.get("argv", [""])[0] == "dotnet"],
                         "the driver ran the candidate for a stale run")
        self.assertTrue(list((self.run_path / "evidence").glob("failure-*.json")))
        self.assertFalse((self.run_path / "scratch").exists())


if __name__ == "__main__":
    unittest.main()

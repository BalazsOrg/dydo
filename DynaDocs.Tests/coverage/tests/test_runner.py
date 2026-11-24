"""The isolated test runner, driven at its command line in a throwaway Git repository.

run_tests.py is copied into the throwaway repository, so its ROOT is that repository, and runs in a
child interpreter. A stand-in `dotnet` on PATH plays the test run: it reports what it saw, writes a
shell profile the way ShellCompletionInstaller.DetectShell picks one, and exits with a chosen code."""
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile
import unittest

RUNNER = Path(__file__).resolve().parents[1] / "run_tests.py"

# ShellCompletionInstaller.DetectShell: $SHELL ending in zsh or bash picks ~/.zshrc, else
# ~/.bashrc when it exists, else ~/.bash_profile; no shell is detected for anything else.
DOTNET = """import json, os, sys
from pathlib import Path
shell, home = os.environ.get("SHELL", ""), Path(os.environ["HOME"])
profile = None
if shell.endswith("/zsh"):
    profile = home / ".zshrc"
elif shell.endswith("/bash"):
    profile = home / ".bashrc" if (home / ".bashrc").exists() else home / ".bash_profile"
if profile is not None:
    with open(profile, "a") as stream:
        stream.write("# dydo completions\\n")
Path(os.environ["STAND_IN_REPORT"]).write_text(json.dumps({
    "cwd": os.getcwd(), "argv": sys.argv[1:], "uncommitted": Path("uncommitted.txt").is_file()}))
sys.exit(int(os.environ["STAND_IN_EXIT"]))
"""


def git(repo, *arguments):
    subprocess.run(["git", "-C", str(repo), "-c", "user.name=t", "-c", "user.email=t@example.invalid",
                    "-c", "commit.gpgsign=false", *arguments], check=True, capture_output=True)


@unittest.skipUnless(sys.platform == "linux", "the stand-in dotnet is a POSIX script")
class IsolatedRunnerTests(unittest.TestCase):
    def setUp(self):
        scope = tempfile.TemporaryDirectory(prefix="dydo-runner-test-")
        self.addCleanup(scope.cleanup)
        self.root = Path(scope.name)
        self.repo, self.home, self.bin = self.root / "repo", self.root / "home", self.root / "bin"
        for folder in (self.repo / "DynaDocs.Tests/coverage", self.home, self.bin):
            folder.mkdir(parents=True)
        shutil.copy2(RUNNER, self.repo / "DynaDocs.Tests/coverage/run_tests.py")
        (self.repo / "DynaDocs.sln").write_text("committed\n")
        git(self.repo, "init", "--quiet")
        git(self.repo, "add", ".")
        git(self.repo, "commit", "--quiet", "-m", "fixture")
        (self.repo / "uncommitted.txt").write_text("working state\n")
        dotnet = self.bin / "dotnet"
        dotnet.write_text(f"#!{sys.executable}\n" + DOTNET)
        dotnet.chmod(0o755)
        self.report = self.root / "report.json"

    def run_tests(self, exit_code=0, shell="/bin/bash"):
        code = ("import sys; sys.path.insert(0, sys.argv.pop(1)); import run_tests; run_tests.main()")
        environment = {**os.environ, "PATH": f"{self.bin}{os.pathsep}{os.environ['PATH']}", "HOME": str(self.home),
                       "SHELL": shell, "STAND_IN_REPORT": str(self.report), "STAND_IN_EXIT": str(exit_code)}
        result = subprocess.run([sys.executable, "-c", code, str(self.repo / "DynaDocs.Tests/coverage")],
                                cwd=self.repo, env=environment, capture_output=True, text=True, timeout=120)
        return result, json.loads(self.report.read_text())

    def registered_worktrees(self):
        listed = subprocess.run(["git", "-C", str(self.repo), "worktree", "list", "--porcelain"],
                                check=True, capture_output=True, text=True).stdout
        return [Path(line[len("worktree "):]).resolve() for line in listed.splitlines() if line.startswith("worktree ")]

    def test_the_test_runs_exit_code_is_the_runners_exit_code(self):
        """Job: contract: run_tests.py docstring, the isolated run's exit code is the runner's exit code"""
        for code in (0, 1, 7):
            with self.subTest(exit_code=code):
                result, _seen = self.run_tests(exit_code=code)

                self.assertEqual(code, result.returncode, result.stdout + result.stderr)

    def test_the_run_sees_the_uncommitted_working_state(self):
        """Job: contract: run_tests.py docstring, an untracked file reaches the isolated run outside the caller's checkout"""
        result, seen = self.run_tests()

        self.assertEqual(0, result.returncode, result.stdout + result.stderr)
        self.assertNotEqual(self.repo.resolve(), Path(seen["cwd"]).resolve())
        self.assertTrue(seen["uncommitted"])

    def test_the_run_writes_no_shell_profile(self):
        """Job: trap: run_tests.isolated_environment, shell completion (DYD-101's installer) never reaches the caller's profile"""
        (self.home / ".bashrc").write_text("# the caller's own profile\n")
        for shell in ("/bin/bash", "/usr/bin/zsh"):
            with self.subTest(shell=shell):
                result, _seen = self.run_tests(shell=shell)

                self.assertEqual(0, result.returncode, result.stdout + result.stderr)
                self.assertEqual(["# the caller's own profile\n"],
                                 [path.read_text() for path in sorted(self.home.iterdir())])

    def test_the_worktree_is_gone_after_a_passing_or_failing_run(self):
        """Job: contract: run_tests.py docstring, the temporary worktree is removed and unregistered after a passing or failing run"""
        for code in (0, 1):
            with self.subTest(exit_code=code):
                _result, seen = self.run_tests(exit_code=code)

                worktree = Path(seen["cwd"]).resolve()
                self.assertFalse(worktree.exists())
                self.assertNotIn(worktree, self.registered_worktrees())


if __name__ == "__main__":
    unittest.main()

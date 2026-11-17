#!/usr/bin/env python3
"""Bounded real-CLI verification. Run inside the host's shared test FIFO."""
import argparse
from contextlib import contextmanager
import hashlib
import json
import os
from pathlib import Path
import re
import shutil
import stat
import tempfile
import uuid

from owned_process import run_owned

ROOT = Path(__file__).resolve().parents[4]
RUNS = Path(tempfile.gettempdir()) / "dydo-verification"
FIXTURE_FILES = ("dydo.json", "dydo/index.md", ".claude/settings.json", "AGENTS.md", "dydo/files-off-limits.md")


def require(condition, message):
    if not condition:
        raise ValueError(message)


def no_links(path):
    """Reject reparse points as well as symlinks, including every existing ancestor."""
    for part in (path, *path.parents):
        try:
            attributes = part.lstat()
        except FileNotFoundError:
            continue
        require(not stat.S_ISLNK(attributes.st_mode), f"Linked path: {part}")
        require(not getattr(attributes, "st_file_attributes", 0) & 0x400,
                f"Reparse path: {part}")


def inspect_tree(path):
    no_links(path)
    for directory, folders, files in os.walk(path, followlinks=False):
        for name in folders + files:
            no_links(Path(directory) / name)


def write_json(path, value):
    path.write_text(json.dumps(value, indent=2) + "\n", encoding="utf-8")


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def candidate(run):
    values = []
    for arguments in (("rev-parse", "HEAD"), ("diff", "--binary", "HEAD"),
                      ("ls-files", "--others", "--exclude-standard", "-z")):
        result = execute(run, "source", ["git", *arguments], ROOT, timeout=30, env=dict(os.environ))
        success(result)
        values.append(result.stdout)
    untracked = {name: digest(ROOT / name) for name in values[2].split("\0") if name}
    return {"head": values[0].strip(),
            "diff_sha256": hashlib.sha256(values[1].encode("utf-8", errors="surrogateescape")).hexdigest(), "untracked": untracked}


def allocate():
    no_links(RUNS)
    RUNS.mkdir(exist_ok=True)
    run = RUNS / uuid.uuid4().hex
    run.mkdir()
    (run / "evidence").mkdir()
    (run / "scratch").mkdir()
    write_json(run / "owner.json", {"schema": 1, "run": str(run), "checkout": str(ROOT)})
    write_json(run / "state.json", {"phase": "starting"})
    return run


def owned(value):
    run = Path(value).absolute()
    require(run.parent == RUNS.absolute() and re.fullmatch(r"[0-9a-f]{32}", run.name),
            "Foreign run path")
    inspect_tree(run)
    owner = json.loads((run / "owner.json").read_text(encoding="utf-8"))
    require(owner == {"schema": 1, "run": str(run), "checkout": str(ROOT)},
            "Foreign run owner")
    return run


@contextmanager
def exclusive(run):
    lock = run / "busy"
    with lock.open("x", encoding="utf-8") as handle:
        handle.write(str(os.getpid()))
    try:
        yield
    finally:
        lock.unlink()


def execute(run, label, argv, cwd, payload=None, timeout=120, env=None):
    env = env if env is not None else {**os.environ, "UseSharedCompilation": "false", "MSBUILDDISABLENODEREUSE": "1",
           "DOTNET_CLI_USE_MSBUILD_SERVER": "0",
           "CODEX_HOME": str(run / "scratch/codex-home")}
    receipt = run / "evidence" / f"{label}-{uuid.uuid4().hex}.json"
    output = receipt.with_suffix("")
    row = {"argv": argv, "cwd": str(cwd), "stdin": payload, "exit": None,
           "timeout_seconds": timeout, "lifecycle": str(output / "result.json")}
    write_json(receipt, row)
    try:
        result = run_owned(argv, cwd=cwd, env=env, input=payload or "", output=output, timeout=timeout)
    except (Exception, KeyboardInterrupt) as error:
        write_json(receipt, {**row, "error": str(error), "error_type": type(error).__name__,
                            "stdout_path": str(output / "stdout.log"),
                            "stderr_path": str(output / "stderr.log")})
        raise
    write_json(receipt, {**row, "exit": result.returncode,
                        "stdout": result.stdout, "stderr": result.stderr})
    return result


def cli(run, label, arguments, payload=None):
    return execute(run, label, ["dotnet", str(run / "scratch/bin/dydo.dll"), *arguments],
                   run / "scratch/project", payload)


def success(result):
    require(result.returncode == 0, f"Command failed ({result.returncode}); see retained evidence")


def installed_hook(run):
    settings = json.loads((run / "scratch/project/.claude/settings.json").read_text(encoding="utf-8"))
    hooks = [hook["command"] for entry in settings["hooks"]["PreToolUse"] for hook in entry["hooks"]]
    require("dydo guard --host claude" in hooks, "Init-installed Claude hook missing")
    # Only this exact installed command is accepted, never arbitrary settings-file commands.
    return ["guard", "--host", "claude"]


def build_hashes(run):
    folder = run / "scratch/bin"
    return {str(path.relative_to(folder)): digest(path) for path in folder.rglob("*") if path.is_file()}


def fixture_hashes(run):
    return {name: digest(run / "scratch/project" / name) for name in FIXTURE_FILES}


def launch(run):
    source = candidate(run)
    write_json(run / "evidence/candidate.json", source)
    project = run / "scratch/project"
    project.mkdir()
    success(execute(run, "build", ["dotnet", "build", str(ROOT / "DynaDocs.csproj"),
                                  "--configuration", "Release", "--output", str(run / "scratch/bin"),
                                  "-p:PublishAot=false", "-p:UseSharedCompilation=false", "-nodeReuse:false"], ROOT, timeout=600))
    success(execute(run, "git-init", ["git", "init", "--quiet"], project))
    success(cli(run, "init", ["init", "claude"]))
    installed_hook(run)
    require(candidate(run) == source, "Candidate changed during build")
    write_json(run / "state.json", {"phase": "ready", "candidate": source,
                                    "build": build_hashes(run), "fixture": fixture_hashes(run)})
    doctor(run)


def doctor(run):
    state = json.loads((run / "state.json").read_text(encoding="utf-8"))
    require(state["phase"] == "ready", "Run is not ready")
    require(state["candidate"] == candidate(run), "Candidate changed; launch a fresh run")
    require(state["build"] == build_hashes(run), "Built candidate changed")
    require((run / "scratch/project/.git").is_dir(), "Owned mock repository missing")
    installed_hook(run)
    require(state["fixture"] == fixture_hashes(run), "Owned fixture changed; launch a fresh run")
    success(cli(run, "doctor", ["--version"]))


def init_docs(run):
    success(cli(run, "check-before", ["check"]))
    broken = run / "scratch/project/dydo/verification-broken.md"
    require(not broken.exists(), "Fixture path already exists")
    broken.write_text("---\narea: general\ntype: reference\n---\n# Broken\n\n[Missing](missing-target.md)\n",
                      encoding="utf-8")
    result = cli(run, "check-broken", ["check"])
    require(result.returncode == 1 and "missing-target.md" in result.stdout,
            "Check did not report the introduced broken link")
    broken.unlink()
    success(cli(run, "check-restored", ["check"]))
    write_json(run / "evidence/init-side-effects.json", fixture_hashes(run))


def guard(run, command, arguments, expected, session, sub_agent=False):
    """Send one synthetic PreToolUse event; a sub-agent's call carries the hook's agent_id."""
    event = {"session_id": session, "hook_event_name": "PreToolUse",
             "permission_mode": "default", "tool_name": "Bash",
             "tool_input": {"command": command}}
    if sub_agent:
        event.update(agent_id=f"verify-{uuid.uuid4().hex[:8]}", agent_type="code-writer")
    result = cli(run, "guard", arguments, json.dumps(event))
    success(result)
    if expected == "allow":
        require(not result.stdout.strip(), "Expected empty stdout on guard allow")
        return ""
    decision = json.loads(result.stdout)["hookSpecificOutput"]
    require(decision["permissionDecision"] == expected, f"Expected guard {expected}")
    return decision["permissionDecisionReason"]


def guard_journey(run):
    sentinel = run / "scratch/project/keep.txt"
    sentinel.write_text("retain this file\n", encoding="utf-8")
    before = digest(sentinel)
    guard(run, "rm -rf ./", installed_hook(run), "deny", uuid.uuid4().hex)
    require(digest(sentinel) == before, "Guard sample was executed")
    write_json(run / "evidence/guard-side-effects.json", {"keep.txt": before, "payload_executed": False})


# The dydo commands the skills hand a sub-agent, as they write them (the check-in's surveyor, crew
# gates, a statechart chart, the Merge Sub-ticket's merge): the worker lane must let each one through.
# Hand-written, so WorkerLaneSkillLintTests fails when sub-agent prose names a worker-allowed command
# word none of these runs. The merge line is only a string the guard reads; nothing here executes it.
SKILL_COMMANDS = (
    "dydo check", "dydo fix dydo/guides", "dydo mode show", "dydo pr-ready --body pr-body.md",
    "dydo merge 123 --match-head-commit 0123abc --match-base-commit 4567def",
    "dydo survey P-DYD-46", "dydo statechart SPEC.tla --skill skills/engineering/statechart --out chart",
    "dydo statechart install-skill --root /work/project", "dydo statechart resume --request request.json",
    "dydo archive branch topic", "dydo archive worktree tree", "dydo archive list", "dydo archive restore ID",
    "dydo --help")


def refusal_lists(reason):
    """The worker-allowed and top-level-only command names a worker-lane refusal prints."""
    allowed = re.search(r"run only these dydo commands: (.+) \(plus ", reason)
    top_level = re.search(r"top-level session that spawned you: (.+)\.", reason)
    require(allowed and top_level, "Worker-lane refusal does not print its classification")
    return allowed.group(1).split(", "), top_level.group(1).split(", ")


def worker_lane(run):
    """Synthetic hook protocol: a native sub-agent's call (agent_id) to the real guard; nothing is executed.

    A T3 delegate_task child carries no agent_id and reads as top-level (DYD-1072): never proof here."""
    hook = installed_hook(run)
    session = uuid.uuid4().hex
    reason = guard(run, "dydo init claude", hook, "deny", session, sub_agent=True)
    allowed, top_level = refusal_lists(reason)
    require("Not allowed here: dydo init" in reason, "Refusal does not name the refused word")
    verdicts = {}
    for command in SKILL_COMMANDS:
        word = command.split()[1]
        require(word in allowed or word.startswith("-"), f"Refusal does not list '{word}' as worker-allowed")
        guard(run, command, hook, "allow", session, sub_agent=True)
        verdicts[command] = "allow"
    for word in top_level:
        refused = guard(run, f"dydo {word}", hook, "deny", session, sub_agent=True)
        require(f"Not allowed here: dydo {word}" in refused, f"Refusal for '{word}' does not name it")
        verdicts[f"dydo {word}"] = "deny"
    # The same refused text from a top-level session (no agent_id) is not the worker lane's to refuse.
    guard(run, "dydo init claude", hook, "allow", session)
    verdicts["dydo init claude (top-level session)"] = "allow"
    write_json(run / "evidence/worker-lane-verdicts.json",
               {"allowed": allowed, "top_level_only": top_level, "verdicts": verdicts, "payload_executed": False})


def activation(run):
    """Synthetic hook protocol only: no mode command is ever executed."""
    for value in ("on", "off"):
        command = f"dydo mode set overnight {value}"
        for hook in (["guard"], installed_hook(run)):
            session = uuid.uuid4().hex
            reason = guard(run, command, hook, "deny", session)
            require("chief of staff readying overnight mode or ending it; otherwise stop" in reason,
                    "Missing chief-of-staff reminder")
            guard(run, command, hook, "allow", session)
    write_json(run / "evidence/activation-limits.json",
               {"layer": "synthetic hook protocol", "mode_set_executed": False,
                "role_authenticated": False, "human_approval_proven": False,
                "state_readback": "requires injected seam tests"})


def archive_journey(run):
    """Real local Git and installed CLI; remote/T3 eligibility belongs to separate native gates."""
    project = run / "scratch/archive-project"
    tree = run / "scratch/archive-tree"
    require(not project.exists() and not tree.exists(), "Archive journey already exists; launch a fresh run")
    project.mkdir()

    def git(label, *arguments):
        result = execute(run, label, ["git", *arguments], project)
        success(result)
        return result.stdout.strip()

    def archive(label, *arguments):
        result = execute(run, label, ["dotnet", str(run / "scratch/bin/dydo.dll"), "archive", *arguments], project)
        success(result)
        return result.stdout

    git("archive-init", "init", "-b", "primary")
    git("archive-user", "config", "user.name", "Dydo Verification")
    git("archive-email", "config", "user.email", "verification@example.invalid")
    git("archive-signing", "config", "commit.gpgsign", "false")
    (project / "file.txt").write_bytes(b"base\n")
    (project / ".gitignore").write_bytes(b"ignored/\n")
    write_json(project / "dydo.json", {"archive": {"reserveBytes": 0, "reservePercent": 0}})
    git("archive-add", "add", "file.txt", ".gitignore")
    git("archive-commit", "commit", "-m", "archive fixture")
    original = git("archive-original", "rev-parse", "HEAD")
    git("archive-worktree", "worktree", "add", "-b", "archive-topic", str(tree))
    (tree / "file.txt").write_bytes(b"staged only\n")
    success(execute(run, "archive-stage", ["git", "add", "file.txt"], tree))
    (tree / "file.txt").write_bytes(b"working version\n")
    (tree / "ignored/empty").mkdir(parents=True)
    (tree / "ignored/binary").write_bytes(bytes([0, 255, 1, 0]))
    archive("archive-delete", "worktree", str(tree))
    require(not tree.exists(), "Archived worktree still exists")
    root = project / ".git/dydo/safe-delete-archive"
    entries = [path for path in root.iterdir() if path.is_dir() and not path.name.startswith(".")]
    require(len(entries) == 1, "Expected exactly one published archive")
    entry = entries[0]
    manifest = json.loads((entry / "manifest.json").read_text(encoding="utf-8"))
    require(manifest["complete"] and (entry / "commits.bundle").stat().st_size > 0, "Archive not complete")
    archive("archive-list", "list")
    git("archive-expire-reflogs", "reflog", "expire", "--expire=now", "--all")
    git("archive-source-gc", "gc", "--prune=now")
    archive("archive-restore", "restore", entry.name)
    require((tree / "file.txt").read_bytes() == b"working version\n", "Working version lost")
    require((tree / "ignored/binary").read_bytes() == bytes([0, 255, 1, 0]), "Ignored binary lost")
    require((tree / "ignored/empty").is_dir(), "Empty directory lost")
    staged = execute(run, "archive-restored-index", ["git", "show", ":file.txt"], tree)
    success(staged)
    require(staged.stdout == "staged only\n", "Staged-only version lost")
    require(git("archive-restored-tip", "rev-parse", "archive-topic") == original, "Primary tip changed")
    write_json(run / "evidence/archive-roundtrip.json", {"archive": entry.name, "original": original,
               "working_sha256": digest(tree / "file.txt"), "staged": staged.stdout,
               "source_gc": True, "archive_retained": entry.exists(), "native_platform": os.name})


def cleanup(run):
    # Recheck the entire tree immediately before removal; evidence stays outside scratch.
    owned(run)
    for receipt in (run / "evidence").glob("*/result.json"):
        result = json.loads(receipt.read_text(encoding="utf-8"))
        require(result["cleanup_confirmed"], "Process cleanup unconfirmed; retain scratch and evidence")
    scratch = run / "scratch"
    if scratch.exists():
        shutil.rmtree(scratch)
    write_json(run / "state.json", {"phase": "cleaned"})
    write_json(run / "evidence/cleanup.json", {"scratch_absent": not scratch.exists()})


def drive(run, journey):
    doctor(run)
    {"init-docs": init_docs, "guard": guard_journey, "activation": activation, "archive": archive_journey, "worker-lane": worker_lane}[journey](run)


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("verb", choices=("launch", "doctor", "drive", "cleanup"))
    parser.add_argument("--run")
    parser.add_argument("--journey", choices=("init-docs", "guard", "activation", "archive", "worker-lane"))
    args = parser.parse_args(argv)
    require((args.verb == "launch") == (args.run is None), "Only launch allocates a run")
    require((args.verb == "drive") == (args.journey is not None), "Only drive takes a journey")
    run = allocate() if args.verb == "launch" else owned(args.run)
    print(json.dumps({"run": str(run), "evidence": str(run / "evidence")}), flush=True)
    with exclusive(run):
        try:
            if args.verb == "drive":
                drive(run, args.journey)
            else:
                {"launch": launch, "doctor": doctor, "cleanup": cleanup}[args.verb](run)
        except (Exception, KeyboardInterrupt) as error:
            write_json(run / "evidence" / f"failure-{uuid.uuid4().hex}.json",
                       {"error": str(error), "type": type(error).__name__})
            cleanup(run)
            raise
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

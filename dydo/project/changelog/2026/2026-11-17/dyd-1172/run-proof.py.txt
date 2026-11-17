"""Run a capped DYD-1172 check and retain its inputs, streams and exit."""

import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys


ROOT = Path(__file__).resolve().parents[6]
EVIDENCE = Path(__file__).resolve().parent


def main():
    name, level, *command = sys.argv[1:]
    output = EVIDENCE / name
    output.mkdir(exist_ok=False)
    owned = sorted(ROOT.glob("DynaDocs.Tests/Integration/SafeDelete*.cs"))
    owned.append(ROOT / "DynaDocs.Tests/Steps/SafeDeleteSteps.cs")
    git = lambda *args: subprocess.check_output(["git", *args], cwd=ROOT, text=True).strip()
    env = os.environ.copy()
    env.update(MSBUILDDISABLENODEREUSE="1", DOTNET_CLI_USE_MSBUILD_SERVER="0", UseSharedCompilation="false")
    argv = ["/var/tmp/dydo-gate-lock.sh", "env", "MSBUILDDISABLENODEREUSE=1",
            "DOTNET_CLI_USE_MSBUILD_SERVER=0", "UseSharedCompilation=false",
            "systemd-run", "--user", "--scope", "--slice=app-t3code-agents-cbc6c2286251.slice",
            "-p", "MemoryMax=" + ("4G" if level == "probe" else "8G"),
            "-p", "MemorySwapMax=0", "-p", "OOMPolicy=continue", "timeout",
            "120" if level == "probe" else "3600", *command]
    receipt = {"head": git("rev-parse", "HEAD"), "branch": git("branch", "--show-current"),
               "status": git("status", "--porcelain=v1"), "cwd": str(ROOT), "argv": argv,
               "sourceSha256": {str(p.relative_to(ROOT)): hashlib.sha256(p.read_bytes()).hexdigest() for p in owned}}
    (output / "command.json").write_text(json.dumps(receipt, indent=2) + "\n")
    with (output / "stdout.txt").open("wb") as stdout, (output / "stderr.txt").open("wb") as stderr:
        result = subprocess.run(argv, cwd=ROOT, env=env, stdout=stdout, stderr=stderr)
    (output / "exit.json").write_text(json.dumps({"exit": result.returncode}) + "\n")
    print(f"{name}: exit {result.returncode}; {output}", flush=True)
    return result.returncode


if __name__ == "__main__":
    raise SystemExit(main())

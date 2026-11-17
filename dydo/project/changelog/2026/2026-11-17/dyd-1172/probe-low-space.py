"""Force the existing disk-space hook; leave config and assertions untouched."""

import hashlib
import json
from pathlib import Path
import subprocess
import sys


EVIDENCE = Path(__file__).resolve().parent
ROOT = EVIDENCE.parents[5]
SOURCE = ROOT / "DynaDocs.Tests/Integration/SafeDeleteTests.cs"


def main():
    name, selection = sys.argv[1:]
    original = SOURCE.read_bytes()
    anchor = b"        CheckGitHub = _ => null,"
    assert original.count(anchor) == 1
    # 1 GiB available is below both default reserve branches on a 100 GiB volume,
    # while ample for these tiny fixtures when their configured reserve is zero.
    changed = original.replace(anchor, anchor + b"\n        Space = _ => (1024L * 1024 * 1024, 100L * 1024 * 1024 * 1024),")
    try:
        SOURCE.write_bytes(changed)
        command = [sys.executable, str(EVIDENCE / "run-proof.py"), name, "probe", "dotnet", "test",
                   "DynaDocs.Tests/DynaDocs.Tests.csproj", "--no-restore", "--filter", selection,
                   "--logger", "trx", "--results-directory", str(EVIDENCE / name / "results")]
        result = subprocess.run(command, cwd=ROOT)
    finally:
        SOURCE.write_bytes(original)
        (EVIDENCE / (name + "-restoration.json")).write_text(json.dumps({
            "originalSha256": hashlib.sha256(original).hexdigest(),
            "restoredSha256": hashlib.sha256(SOURCE.read_bytes()).hexdigest(),
            "onlyProbeChange": changed.decode().splitlines()[20],
        }, indent=2) + "\n")
    return result.returncode


if __name__ == "__main__":
    raise SystemExit(main())

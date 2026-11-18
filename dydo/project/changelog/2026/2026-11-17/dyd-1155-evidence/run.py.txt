import datetime
import hashlib
import json
import pathlib
import subprocess
import sys
import time

ROOT = pathlib.Path('/home/balazs/.t3/worktrees/DynaDocs/t3-a829c0a8-36dac2b1bcc02af9')
OUT = pathlib.Path('/tmp/dyd-1155-evidence')
OUT.mkdir(exist_ok=True)
name, *command = sys.argv[1:]
sources = [ROOT / 'Scaffold/dydo/reference/linear-workspace-standard.md', ROOT / 'dydo/reference/linear-workspace-standard.md', ROOT / 'Services/Survey/SurveyRules.cs']
receipt = {
    'name': name,
    'cwd': str(ROOT),
    'command': command,
    'head': subprocess.check_output(['git', '-C', str(ROOT), 'rev-parse', 'HEAD'], text=True).strip(),
    'base': '07d32a3b085bd7bcb9ecadcc0d494b6f182f4c85',
    'source_sha256': {str(p.relative_to(ROOT)): hashlib.sha256(p.read_bytes()).hexdigest() for p in sources},
    'started_utc': datetime.datetime.now(datetime.timezone.utc).isoformat(),
    'environment': 'T3 Codex docs-writer; Ubuntu 26.04 x64; app-t3code-agents-cbc6c2286251.slice',
}
start = time.monotonic()
with (OUT / (name + '.log')).open('wb') as log:
    result = subprocess.run(command, cwd=ROOT, stdout=log, stderr=subprocess.STDOUT)
receipt.update(exit=result.returncode, elapsed_seconds=round(time.monotonic() - start, 3), finished_utc=datetime.datetime.now(datetime.timezone.utc).isoformat(), log=name + '.log')
(OUT / (name + '.json')).write_text(json.dumps(receipt, indent=2) + '\n')
print(json.dumps(receipt, indent=2))
print((OUT / (name + '.log')).read_text())
sys.exit(result.returncode)

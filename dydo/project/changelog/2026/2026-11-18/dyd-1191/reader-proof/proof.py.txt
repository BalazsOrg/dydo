"""Capped, immutable receipts for the DYD-1191 reader hop. Run from the repo root."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[6]
FILTER = ('FullyQualifiedName~CommandDocConsistencyTests|FullyQualifiedName~HelpCommandTests|'
          'FullyQualifiedName~ScaffoldTreeTests|FullyQualifiedName~CanonicalSkillAssertionTests|'
          'FullyQualifiedName~OneCanonicalSkillTreeReachesEverySupportedHostFeature|'
          'FullyQualifiedName~OneProjectLocalInterfaceRunsTestsAndAssuranceHonestlyFeature')


def retain(label, output):
    raw = output.encode()
    for offset in range(0, len(raw), 16000):
        path = HERE / f'{label}-{offset // 16000 + 1:03}.log'
        with path.open('xb') as stream:
            stream.write(raw[offset:offset + 16000])


def run(label, command, full=False):
    slices = [part for part in Path('/proc/self/cgroup').read_text().split('/')
              if part.endswith('.slice')]
    cap = ['systemd-run', '--user', '--scope', '--slice=' + slices[-1],
           '-p', 'MemoryMax=8G' if full else 'MemoryMax=4G',
           '-p', 'MemorySwapMax=0', '-p', 'OOMPolicy=continue',
           'timeout', '3600' if full else '120']
    environment = dict(os.environ, MSBUILDDISABLENODEREUSE='1',
                       DOTNET_CLI_USE_MSBUILD_SERVER='0', UseSharedCompilation='false',
                       PYTHONDONTWRITEBYTECODE='1', PYTHON=sys.executable)
    sources = {}
    for pattern in ('DynaDocs.Tests/Commands/CommandDocConsistencyTests*.cs',
                    'DynaDocs.Tests/Steps/CanonicalSkill*.cs',
                    'DynaDocs.Tests/coverage/tests/test_testing_facade*.py'):
        sources.update({str(p.relative_to(ROOT)): hashlib.sha256(p.read_bytes()).hexdigest()
                        for p in ROOT.glob(pattern)})
    result = subprocess.run(cap + list(map(str, command)), cwd=ROOT, env=environment,
                            stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
    receipt = {'command': cap + list(map(str, command)), 'cwd': str(ROOT),
               'head': subprocess.check_output(['git', 'rev-parse', 'HEAD'], text=True).strip(),
               'sources': sources, 'exit': result.returncode,
               'dotnet_env': {key: environment[key] for key in
                              ('MSBUILDDISABLENODEREUSE', 'DOTNET_CLI_USE_MSBUILD_SERVER', 'UseSharedCompilation')}}
    retain(label, json.dumps(receipt, indent=2) + '\n' + result.stdout + f'\nEXIT: {result.returncode}\n')
    print(label, 'exit', result.returncode, flush=True)
    print(result.stdout[-2200:], flush=True)
    return result.returncode


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('label')
    parser.add_argument('--full', action='store_true')
    parser.add_argument('command', nargs=argparse.REMAINDER)
    args = parser.parse_args()
    sys.exit(run(args.label, args.command, args.full))

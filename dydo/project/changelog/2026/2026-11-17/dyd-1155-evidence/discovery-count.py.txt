from pathlib import Path
import sys

lines = Path(sys.argv[1] if len(sys.argv) > 1 else '/tmp/dyd-1155-evidence/discovery.log').read_text().splitlines()
names = [line.strip() for line in lines if line.startswith('    DynaDocs.Tests.')]
assert names, 'empty discovery'
print('Discovered cases:', len(names))
print('DocumentationTests:', sum('.DocumentationTests.' in name for name in names))
print('ScaffoldTreeTests:', sum('.ScaffoldTreeTests.' in name for name in names))

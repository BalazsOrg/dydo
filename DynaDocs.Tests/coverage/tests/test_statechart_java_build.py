"""The statechart helper build, driven at its command line on a synthetic recipe and JDK.

The JDK's javac and jar are small stand-in programs, packed into the JDK archive like the real ones,
so every check runs on real files and no call inside build.py is replaced."""
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
import tarfile
import tempfile
import unittest
import zipfile

SOURCE = Path(__file__).resolve().parents[3]
RECIPE = SOURCE / 'skills/engineering/statechart/scripts/java'
SOURCES = ['src/dydo/statechart/LoopbackTlc.java', 'src/dydo/statechart/LoopbackSockets.java']
CLASSES = ['dydo/statechart/LoopbackTlc.class', 'dydo/statechart/LoopbackSockets.class']
RELEASE = b'JAVA_RUNTIME_VERSION="21.0.8+9-LTS"\nIMPLEMENTOR="Eclipse Adoptium"\n'

# javac writes each locked class as Java 11 bytecode into its -d folder and leaves a mark that it ran.
JAVAC = '''import sys
from pathlib import Path
args = sys.argv[1:]
with open(MARK, 'a') as mark: mark.write('javac\\n')
out = Path(args[args.index('-d') + 1])
for name in CLASSES:
    (out / name).parent.mkdir(parents=True, exist_ok=True)
    (out / name).write_bytes(bytes.fromhex('cafebabe00000037') + name.encode())
'''
# jar packs its -C members into a ZIP with a fixed date, so two builds are byte-identical.
JAR = '''import sys, zipfile
from pathlib import Path
args = sys.argv[1:]
members = args[args.index('-C'):]
with zipfile.ZipFile(args[args.index('--file') + 1], 'w') as bundle:
    for folder, name in zip(members[1::3], members[2::3]):
        bundle.writestr(zipfile.ZipInfo(name, (2026, 1, 1, 0, 0, 0)), (Path(folder) / name).read_bytes())
'''


def sha(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def program(body, mark):
    return (f'#!{sys.executable}\nMARK = {str(mark)!r}\nCLASSES = {CLASSES!r}\n' + body).encode()


@unittest.skipUnless(sys.platform == 'linux', 'build.py builds only on Linux')
class StatechartJavaBuildTests(unittest.TestCase):
    def setUp(self):
        scope = tempfile.TemporaryDirectory(prefix='dydo-java-build-')
        self.addCleanup(scope.cleanup)
        self.root = Path(scope.name)
        self.mark = self.root / 'javac-ran'

    def toolchain(self, **changes):
        files = {'release': RELEASE, 'bin/javac': program(JAVAC, self.mark), 'bin/jar': program(JAR, self.mark),
                 'bin/java': b'java', 'lib/modules': b'modules'}
        return dict(files, **changes)

    def reset(self):
        for name in ('jdk', 'recipe', 'work'):
            shutil.rmtree(self.root / name, ignore_errors=True)
        for name in ('jdk.tar.gz', 'tla2tools.jar'):
            (self.root / name).unlink(missing_ok=True)

    def jdk(self, files=None, extra=None):
        """An extracted JDK and its archive; `extra(jdk, bundle)` adds members after the tree."""
        jdk, archive = self.root / 'jdk', self.root / 'jdk.tar.gz'
        files = files or self.toolchain()
        for relative, raw in files.items():
            (jdk / relative).parent.mkdir(parents=True, exist_ok=True)
            (jdk / relative).write_bytes(raw)
        for tool in ('bin/javac', 'bin/jar'):
            (jdk / tool).chmod(0o755)
        with tarfile.open(archive, 'w:gz') as bundle:
            bundle.add(jdk, arcname='jdk')
            if extra:
                extra(jdk, bundle)
        return jdk, archive

    def compile_input(self):
        """A TLC compile-input jar and the lock's independent pin of its exploded members."""
        data = {'META-INF/MANIFEST.MF': b'Manifest-Version: 1.0\r\n\r\n', 'tlc2/TLC.class': b'not a real class'}
        jar = self.root / 'tla2tools.jar'
        with zipfile.ZipFile(jar, 'w') as bundle:
            for name, raw in data.items():
                bundle.writestr(name, raw)
        rows = sorted((dict(name=name, kind='file', bytes=len(raw), sha256=hashlib.sha256(raw).hexdigest())
                       for name, raw in data.items()), key=lambda row: row['name'])
        canonical = json.dumps(rows, sort_keys=True, separators=(',', ':')).encode('utf-8')
        pin = dict(representation='byte-exact-exploded-tlc-v1', memberCount=len(rows), classCount=1,
                   expandedBytes=sum(row['bytes'] for row in rows), membersSha256=hashlib.sha256(canonical).hexdigest())
        return jar, pin

    def build(self, jdk, archive, archive_pin=None):
        """Copy build.py into a recipe of its own, pin the inputs in its lock, and run it."""
        here = self.root / 'recipe'
        here.mkdir()
        shutil.copy2(RECIPE / 'build.py', here / 'build.py')
        sources = []
        for name in SOURCES:
            (here / name).parent.mkdir(parents=True, exist_ok=True)
            (here / name).write_bytes(b'// ' + name.encode())
            sources.append(dict(path=name, sha256=sha(here / name)))
        tlc, compiler_input = self.compile_input()
        lock = dict(schema=1, bytecodeMajor=55, classes=CLASSES, sources=sources, compilerInput=compiler_input,
                    tlc=dict(sha256=sha(tlc)), jdk=dict(archiveSha256=archive_pin or sha(archive)),
                    artifact=dict(state='unbuilt', sha256=None))
        (here / 'build-lock.json').write_text(json.dumps(lock), encoding='utf-8')
        self.artifact, self.receipt = self.root / 'artifact.jar', self.root / 'receipt.json'
        argv = [sys.executable, '-I', str(here / 'build.py'), '--jdk', str(jdk), '--jdk-archive', str(archive),
                '--tlc', str(tlc), '--work', str(self.root / 'work'), '--output', str(self.artifact),
                '--receipt', str(self.receipt)]
        environment = {key: value for key, value in os.environ.items() if not key.startswith('PYTHON')}
        return subprocess.run(argv, capture_output=True, text=True, timeout=120, env=environment)

    def assert_refused(self, result):
        self.assertNotEqual(0, result.returncode, result.stderr)
        self.assertFalse(self.mark.exists(), 'the unverified javac ran')
        self.assertFalse(self.artifact.exists(), 'a refused build published an artifact')
        self.assertFalse(self.receipt.exists(), 'a refused build published a receipt')

    def test_a_verified_build_publishes_the_artifact_and_its_receipt(self):
        """Job: contract: DYD-415, a verified JDK and TLC build a helper jar holding exactly the locked classes, with a receipt naming its hash"""
        result = self.build(*self.jdk())

        self.assertEqual(0, result.returncode, result.stderr)
        with zipfile.ZipFile(self.artifact) as bundle:
            self.assertEqual(sorted(CLASSES), sorted(bundle.namelist()))
        receipt = json.loads(self.receipt.read_text(encoding='utf-8'))
        self.assertEqual(sha(self.artifact), receipt['artifactSha256'])
        self.assertTrue(self.mark.exists())

    def test_a_tampered_archive_is_refused_before_its_javac_runs(self):
        """Job: trap: DYD-415, a JDK archive differing from its lock pin is never executed"""
        pin = sha(self.jdk()[1])
        self.reset()
        trojan = self.jdk(self.toolchain(**{'bin/javac': program(JAVAC + 'TROJAN = 1\n', self.mark)}))

        self.assert_refused(self.build(*trojan, archive_pin=pin))

    def test_an_extracted_jdk_unlike_its_archive_is_refused_before_its_javac_runs(self):
        """Job: trap: DYD-415, a traversal member, a link escaping the JDK root and a javac changed after packing are each refused before javac runs"""
        def traversal(jdk, bundle):
            member = tarfile.TarInfo('jdk/bin/../../escape')
            bundle.addfile(member)

        def link_escape(jdk, bundle):
            (jdk / 'bin/escape').symlink_to('../../outside')
            bundle.add(jdk / 'bin/escape', arcname='jdk/bin/escape')

        def changed_javac(jdk, bundle):
            (jdk / 'bin/javac').write_bytes(program(JAVAC.replace('javac', 'jAvac'), self.mark))

        for name, extra in [('traversal member', traversal), ('link escaping the JDK root', link_escape),
                            ('javac changed after packing', changed_javac)]:
            with self.subTest(name):
                self.reset()
                self.assert_refused(self.build(*self.jdk(extra=extra)))
                self.assertFalse((self.root / 'escape').exists())

    def test_a_foreign_release_is_refused_before_its_javac_runs(self):
        """Job: trap: DYD-415, a JDK whose release names a vendor other than Eclipse Adoptium is refused before its javac runs"""
        foreign = self.toolchain(release=RELEASE.replace(b'Eclipse Adoptium', b'Other Vendor'))

        self.assert_refused(self.build(*self.jdk(foreign)))


if __name__ == '__main__':
    unittest.main()

const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { test } = require('node:test');
const { spawnSync } = require('node:child_process');

const repoRoot = path.resolve(__dirname, '..', '..', '..');
const script = path.join(repoRoot, 'setup-skills.mjs');
const hostRoots = ['.claude', '.agents'];

function writeSkill(root, category, name) {
  const skillDir = path.join(root, 'skills', category, name);
  fs.mkdirSync(skillDir, { recursive: true });
  fs.writeFileSync(path.join(skillDir, 'SKILL.md'), `---\nname: ${name}\ndescription: fixture\n---\nbody\n`);
}

function runSetup(root) {
  return spawnSync(process.execPath, [script, '--root', root], { cwd: os.tmpdir(), encoding: 'utf8', timeout: 10000 });
}

function inRoot(callback) {
  const root = fs.mkdtempSync(path.join(os.tmpdir(), 'dydo-setup-skills-'));
  try {
    callback(root);
  } finally {
    fs.rmSync(root, { recursive: true, force: true });
  }
}

// Every entry under both host roots: its kind, its raw link text, its inode, and a file's bytes.
function snapshot(root) {
  const entries = {};
  for (const host of hostRoots) {
    const hostSkills = path.join(root, host, 'skills');
    if (!fs.existsSync(hostSkills)) continue;
    for (const name of fs.readdirSync(hostSkills).sort()) {
      const target = path.join(hostSkills, name);
      const stats = fs.lstatSync(target);
      entries[`${host}/${name}`] = stats.isSymbolicLink()
        ? { link: fs.readlinkSync(target), ino: stats.ino }
        : { directory: fs.readdirSync(target).map((file) => [file, fs.readFileSync(path.join(target, file), 'utf8')]) };
    }
  }
  return entries;
}

// Job: contract: README "Install the skills" step 3, every skill linked flat into both host roots (DYD-219)
test('both host roots link every skill flat, whatever category depth holds it', () => {
  inRoot((root) => {
    const skills = { alpha: 'engineering', beta: 'productivity', gamma: 'roles/crew' };
    for (const [name, category] of Object.entries(skills)) writeSkill(root, category, name);

    const result = runSetup(root);

    assert.equal(result.status, 0, result.stderr);
    for (const host of hostRoots) {
      const hostSkills = path.join(root, host, 'skills');
      assert.deepEqual(fs.readdirSync(hostSkills).sort(), Object.keys(skills));
      for (const [name, category] of Object.entries(skills))
        assert.equal(fs.realpathSync(path.join(hostSkills, name)), fs.realpathSync(path.join(root, 'skills', category, name)));
    }
  });
});

// Job: contract: README "Install the skills", the script is safe to rerun
test('a rerun exits zero and leaves every link exactly as it was', () => {
  inRoot((root) => {
    writeSkill(root, 'engineering', 'alpha');
    writeSkill(root, 'roles/crew', 'beta');
    assert.equal(runSetup(root).status, 0);
    const before = snapshot(root);

    const rerun = runSetup(root);

    assert.equal(rerun.status, 0, rerun.stderr);
    assert.deepEqual(snapshot(root), before);
  });
});

// Job: trap: README "Install the skills", a collision at a planned link is refused, leaving every entry
// under both host skill roots as it was (DYD-91, DYD-225)
for (const [kind, occupy] of [
  ['a human-owned directory', (root, target) => {
    fs.mkdirSync(target, { recursive: true });
    fs.writeFileSync(path.join(target, 'human-owned.txt'), 'do not touch');
  }],
  ['a link elsewhere in the project', (root, target) => {
    fs.mkdirSync(path.join(root, 'elsewhere'));
    fs.symlinkSync(path.join(root, 'elsewhere'), target, 'junction');
  }],
  ['a foreign link outside the project', (root, target) => {
    fs.symlinkSync(os.tmpdir(), target, 'junction');
  }],
]) {
  test(`a collision with ${kind} is refused and both host roots stay unchanged`, () => {
    inRoot((root) => {
      writeSkill(root, 'engineering', 'alpha');
      writeSkill(root, 'engineering', 'beta');
      fs.mkdirSync(path.join(root, '.agents', 'skills'), { recursive: true });
      occupy(root, path.join(root, '.agents', 'skills', 'beta'));
      const before = snapshot(root);

      const result = runSetup(root);

      assert.notEqual(result.status, 0);
      assert.deepEqual(snapshot(root), before);
    });
  });
}

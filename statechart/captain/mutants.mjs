// Written by hand. Shows that the checks catch a wrong model. Each mutant changes one line of a copy
// of the model, in a new scratch folder, and the check it names must fail on it:
// - a view mutant must make check-chart.mjs fail with a step under an arrow whose words are false of it;
// - a property mutant must make TLC report its property broken, on the properties as run.py checks
//   them (verification/statechart/captain/).
// First it checks the model as it stands: the view check passes and every property holds.
//
// Usage: node statechart/captain/mutants.mjs   (no arguments). Exit 0 when every mutant is caught.
import { spawnSync } from "node:child_process";
import { copyFileSync, mkdtempSync, readdirSync, readFileSync, rmSync, writeFileSync } from "node:fs";
import { tmpdir } from "node:os";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { runTlc } from "./tlc.mjs";

const folder = fileURLToPath(new URL(".", import.meta.url));
const properties = path.join(folder, "..", "..", "verification", "statechart", "captain");

// [what is wrong, the action it changes, the text in that action, its replacement, what must catch it]
const mutants = [
  ["the PASS arrow is taken on a FAIL", "Pass", '/\\ verdict = "pass"', '/\\ verdict \\in {"pass", "fail"}', "view"],
  ["the FAIL arrow is taken on a PASS", "Fail", '/\\ verdict = "fail"', '/\\ verdict \\in {"pass", "fail"}', "view"],
  ["a leaf closes before its result is in", "CloseLeaf", "    /\\ ResultIn\n", "", "view"],
  ["the sizing is recorded the other way", "WorkItself", 'Ev("fits one agent", "captain")', 'Ev("needs more than one agent", "captain")', "view"],
  ["an unproved change goes to review", "SendLeafToReview", "    /\\ proved\n", "", "view"],
  ["the merge closes without CI green", "GreenOnTheMergeSha", "/\\ ci' = \"green\"", "/\\ ci' = \"none\"", "view"],
  ["the next parts are cut before a part is back", "CutTheNextParts", '/\\ part = "back"\n    /\\ contract = "not met"', '/\\ part \\in {"out", "back"}', "view"],
  ["a Question's answer comes from the captain itself", "Answered", 'Ev("answer", "question")', 'Ev("answer", "captain")', "view"],
  ["a green merge goes to a repair", "Repair", '/\\ gates \\in {"red", "conflict"}', '/\\ gates # "none"', "view"],
  ["a held ticket is taken", "Take", '/\\ ticket = "free"', '/\\ ticket \\in {"free", "held"}', "view"],
  ["a Question opens with no call met", "AskAQuestion", '/\\ asked = "needed"', '/\\ asked \\in {"none", "needed"}', "view"],
  ["the captain opens its own Question", "AskAQuestion", 'Ev("open a Question", "officer")', 'Ev("open a Question", "captain")', "view"],
  ["the next parts are cut on a met contract", "CutTheNextParts", '/\\ contract = "not met"', '/\\ contract \\in {"not met", "met"}', "view"],
  ["the whole is handed back on a met contract", "HandBackTheWhole", '/\\ contract = "cannot be met"', '/\\ contract \\in {"met", "cannot be met"}', "view"],
  ["a proved Fix comes back undone", "HandBackTheFix", "    /\\ ~proved\n", "", "view"],
  ["a leaf closes before its result is in", "CloseLeaf", "    /\\ ResultIn\n", "", "FR24_DoneWhenTheContractIsMet"],
  ["a step follows a close", "HandBackTheWork", '/\\ state = "Leaf"', '/\\ state \\in {"Leaf", "Done"}', "FR25_EndsAreFinal"],
  ["the captain writes the verdict on its own change", "TheReviewerWritesTheVerdict", 'Ev("write the verdict", "reviewer")', 'Ev("write the verdict", "captain")', "FR26_EachDecidesItsJob"],
  ["the captain's verdict rides beside the reviewer's", "TheReviewerWritesTheVerdict", 'Ev("write the verdict", "reviewer")}', 'Ev("write the verdict", "reviewer"), Ev("write the verdict", "captain")}', "FR26_EachDecidesItsJob"],
  ["a captain closes beside another's close", "CloseLeaf", 'Ev("close", "captain")', 'Ev("close", "captain"), Ev("close", "officer")', "FR24_DoneWhenTheContractIsMet"],
  ["a verdict is written under another event's name", "WriteTheVerdict", 'Ev("write the verdict", "captain")', 'Ev("write the result", "captain")', "FR29_TheRecordIsFaithful"],
  ["a FAIL is written with no blocker", "TheReviewerWritesTheVerdict", "blockers' \\in (SUBSET Blockers) \\ {{}}", "blockers' \\in SUBSET Blockers", "FR27_FailHasABlocker"],
  ["a verdict is sent to review", "SendLeafToReview", "/\\ merges\n    /\\ proved\n", '/\\ merges \\/ kind = "verdict"\n', "FR28_AVerdictIsNotReviewedAgain"],
];

function copy() {
  const dir = mkdtempSync(path.join(tmpdir(), "captain-mutant-"));
  for (const name of readdirSync(folder)) if (/\.(tla|cfg|mjs|mmd)$/.test(name)) copyFileSync(path.join(folder, name), path.join(dir, name));
  return dir;
}

// Applies one mutant inside the named action of the copy's Captain.tla; the text must occur there once.
function mutate(dir, action, find, replace) {
  const file = path.join(dir, "Captain.tla");
  const text = readFileSync(file, "utf8");
  const start = text.indexOf("\n" + action + " ==\n");
  const end = text.indexOf("\n\n", start + 1);
  const body = text.slice(start, end);
  if (start < 0 || body.split(find).length !== 2) throw new Error("mutant text not found once in " + action + ": " + find);
  writeFileSync(file, text.slice(0, start) + body.replace(find, replace) + text.slice(end));
}

const viewCheck = (dir) => spawnSync(process.execPath, [path.join(dir, "check-chart.mjs")], { encoding: "utf8", maxBuffer: 64 * 1024 * 1024 });

// TLC on the properties, configured as run.py configures them: the model's own config with the
// observer's INIT and NEXT, then the property registration.
function propertyCheck(dir) {
  const cfg = path.join(dir, "CaptainProperties.cfg");
  const own = readFileSync(path.join(dir, "Captain.cfg"), "utf8").replace("INIT Init", "INIT PropertyInit").replace("NEXT Next", "NEXT PropertyNext");
  writeFileSync(cfg, own + "\n" + readFileSync(path.join(properties, "CaptainProperties.cfg"), "utf8"));
  return runTlc({ folder: dir, spec: "CaptainProperties", config: cfg, extra: [path.join(properties, "CaptainProperties.tla")] });
}

const failures = [];
let dir = copy();
try {
  const view = viewCheck(dir);
  if (view.status !== 0) failures.push("the view check fails on the model as it stands:\n" + view.stderr + view.stdout);
  const held = propertyCheck(dir);
  if (held.unavailable) {
    console.error(held.unavailable);
    process.exit(2);
  }
  if (!held.complete) failures.push("the properties do not all hold on the model as it stands:\n" + held.out);
  else console.log("the model as it stands: the view check passes, and every property holds (" + held.summary + ")");
} finally {
  rmSync(dir, { recursive: true, force: true });
}
for (const [what, action, find, replace, catcher] of mutants) {
  dir = copy();
  try {
    mutate(dir, action, find, replace);
    let caught;
    if (catcher === "view") {
      const run = viewCheck(dir);
      caught = run.status === 1 && /where its words are false of it/.test(run.stderr);
    } else {
      const run = propertyCheck(dir);
      caught = new RegExp("property " + catcher + " is violated").test(run.out);
    }
    console.log((caught ? "caught" : "NOT CAUGHT") + " by " + catcher + ": " + what + " (" + action + ")");
    if (!caught) failures.push(what);
  } finally {
    rmSync(dir, { recursive: true, force: true });
  }
}
if (failures.length) {
  for (const failure of failures) console.error("not caught: " + failure);
  process.exit(1);
}
console.log(mutants.length + " mutants, each caught by the check it names.");

// Written by hand. Checks that register.md stays the closed list the model is held to, and that the
// model and its properties rest on it.
//
// Usage: node statechart/captain/check-register.mjs   (no arguments). Exit 0 when it holds, 1 with
// every problem.
//
// It fails on:
// - a spec rule ID (K1-K34, R1-R34, S1-S26, H1-H24, L1-L23, T1-T23, D1-D12, Y1-Y9, N1-N15), or a
//   source key the register defines (C, W and G), that no row cites, in a part or under "Not an order";
// - a row whose ID is not its part's prefix with a number, an ID used twice, a mark other than action
//   or property, or a source the register does not define;
// - a name in a row's "Built as" that the model or the properties registration lacks;
// - a property row of part 1 with no registered property, and a registered property whose name does
//   not start with a property row's ID, or that its row's "Built as" does not name;
// - an action in the model's Next that no row of part 1 names, or whose comment cites no row;
// - a row ID cited in the model or the properties that the register lacks.
import { readFileSync } from "node:fs";
import { fileURLToPath } from "node:url";

const here = (name) => fileURLToPath(new URL(name, import.meta.url));
const register = readFileSync(here("./register.md"), "utf8");
const model = readFileSync(here("./Captain.tla"), "utf8");
const base = readFileSync(here("./CaptainBase.tla"), "utf8");
const properties = readFileSync(here("../../verification/statechart/captain/CaptainProperties.tla"), "utf8");
const registered = readFileSync(here("../../verification/statechart/captain/CaptainProperties.cfg"), "utf8");

const problems = [];
const say = (text) => problems.push(text);

// The register's sections, by their "## " headings.
const sections = new Map();
let heading = "";
for (const line of register.split("\n")) {
  const h = /^## (.+)$/.exec(line);
  if (h) heading = h[1];
  else sections.set(heading, (sections.get(heading) ?? "") + line + "\n");
}
const cells = (line) => line.split("|").slice(1, -1).map((cell) => cell.trim());
const tableRows = (text) => (text ?? "").split("\n").filter((line) => line.startsWith("| ") && !/^\| -+|^\|---/.test(line));

const parts = { FR: "Part 1: the frame", IQ: "Part 2: intake and Questions", SP: "Part 3: the split", RM: "Part 4: review and merge" };
for (const name of Object.values(parts)) if (!sections.has(name)) say("register.md has no section " + name);
if (!sections.has("Not an order")) say("register.md has no section Not an order");

const spec = { K: 34, R: 34, S: 26, H: 24, L: 23, T: 23, D: 12, Y: 9, N: 15 };
const specIds = Object.entries(spec).flatMap(([letter, n]) => Array.from({ length: n }, (_, i) => letter + (i + 1)));
const sourceKeys = tableRows(sections.get("Sources")).map((line) => cells(line)[0]).filter((key) => /^[CWG]\d+$/.test(key));
const known = new Set([...specIds, ...sourceKeys]);

const rows = new Map();
let cited = "";
for (const [prefix, name] of Object.entries(parts)) {
  for (const line of tableRows(sections.get(name))) {
    const [id, rule, sources, mark, built] = cells(line);
    if (id === "ID") continue;
    if (!new RegExp("^" + prefix + "\\d+$").test(id)) say(name + ": a row whose ID is not " + prefix + " and a number: " + id);
    if (rows.has(id)) say("a row ID used twice: " + id);
    if (!["action", "property"].includes(mark)) say(id + ": its mark must be action or property, not " + mark);
    for (const token of (sources ?? "").match(/\b[A-Z]\d+\b/g) ?? [])
      if (!known.has(token)) say(id + ": cites a source the register does not define: " + token);
    rows.set(id, { prefix, rule, sources, mark, built: built ?? "" });
    cited += sources + "\n";
  }
}
for (const line of tableRows(sections.get("Not an order"))) cited += cells(line)[0] + "\n";
const citedTokens = new Set(cited.match(/\b[A-Z]\d+\b/g) ?? []);
for (const id of [...specIds, ...sourceKeys]) if (!citedTokens.has(id)) say("a source no row cites: " + id);

const nextBody = /^Next ==\n((?:    \\\/ \w+\n)+)/m.exec(model)[1];
const modelActions = [...nextBody.matchAll(/\\\/ (\w+)/g)].map((m) => m[1]).filter((a) => a !== "Ended");
const registeredProperties = [...registered.matchAll(/^(?:PROPERTY|INVARIANT) (\w+)$/gm)].map((m) => m[1]);
const names = new Set([...modelActions, ...registeredProperties]);
const backticked = (text) => [...text.matchAll(/`(\w+)`/g)].map((m) => m[1]);

for (const [id, row] of rows)
  for (const name of backticked(row.built)) if (!names.has(name)) say(id + ": Built as names " + name + ", which the model and the properties lack");
const frame = sections.get(parts.FR) ?? "";
for (const action of modelActions) if (!backticked(frame).includes(action)) say("an action of the model that no part 1 row names: " + action);

for (const [id, row] of rows)
  if (row.prefix === "FR" && row.mark === "property" && !backticked(row.built).some((n) => registeredProperties.includes(n)))
    say(id + ": a property row of the frame with no registered property");
for (const name of registeredProperties) {
  const id = /^([A-Z]{2}\d+)_/.exec(name)?.[1];
  const row = rows.get(id);
  if (!row || row.mark !== "property") say("a registered property not named after a property row: " + name);
  else if (!backticked(row.built).includes(name)) say(id + ": its Built as does not name its property " + name);
}

// The comment block right above a definition.
function comment(text, name) {
  const lines = text.split("\n");
  const at = lines.findIndex((line) => line.startsWith(name + " =="));
  const block = [];
  for (let i = at - 1; i >= 0 && lines[i].startsWith("\\*"); i--) block.unshift(lines[i]);
  return block.join("\n");
}
const rowIds = (text) => text.match(/\b(?:FR|IQ|SP|RM)\d+\b/g) ?? [];
for (const action of modelActions) if (!rowIds(comment(model, action)).some((id) => rows.has(id))) say("an action whose comment cites no register row: " + action);
for (const name of registeredProperties) if (!rowIds(comment(properties, name)).some((id) => rows.has(id))) say("a property whose comment cites no register row: " + name);
for (const [file, text] of [["CaptainBase.tla", base], ["Captain.tla", model], ["CaptainProperties.tla", properties]])
  for (const id of rowIds(text)) if (!rows.has(id)) say(file + " cites a row the register lacks: " + id);

if (problems.length) {
  for (const problem of problems) console.error(problem);
  process.exit(1);
}
const count = (prefix, mark) => [...rows.values()].filter((r) => r.prefix === prefix && (!mark || r.mark === mark)).length;
const lines = tableRows(sections.get("Not an order")).length - 1;
console.log(
  "register.md is closed: " + rows.size + " rows that order the work (" +
    Object.keys(parts).map((p) => p + " " + count(p) + ": " + count(p, "action") + " actions, " + count(p, "property") + " properties").join("; ") +
    "), and " + lines + " lines of sources that order no step; every one of the " + specIds.length + " spec rules and " +
    sourceKeys.length + " other source keys is cited; " + modelActions.length + " model actions and " + registeredProperties.length +
    " properties each rest on a row.",
);

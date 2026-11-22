// Written by hand. Checks that the hand-written chart.mmd is a faithful view of the comprehensive
// model in Captain.tla, through the map in view-map.mjs, on every step the model can take.
//
// Usage: node statechart/captain/check-chart.mjs   (it takes no arguments)
// It runs TLC on Captain.tla and Captain.cfg as they stand beside it, in a new scratch folder, and
// reads the state graph TLC dumps with `-dump dot,actionlabels`: every reachable state and every step,
// each named by its action. It reads no graph from anywhere else. Java and the TLC jar come from JAVA
// and TLA2TOOLS, or else from the copies the dydo statechart capability installed. Exit 0 when the
// view is faithful; exit 1 with every problem; exit 2 when it cannot run.
//
// It checks, and so assumes nothing it does not check:
// - chart.mmd's visible words, every line Mermaid renders, are the words Balazs approved (DYD-1440):
//   their SHA-256 must be the one pinned below, so any change to a state's or an arrow's words fails;
// - chart.mmd, read strictly (chart-words.mjs): no line it cannot read, no arrow without words, no
//   two arrows between the same two states, and no arrow that joins two cases with "or";
// - Captain.cfg says only INIT Init, NEXT Next and INVARIANT TypeOK, so TLC explores the whole model
//   with nothing pruned and checks for deadlock;
// - the map against the model text: every state in States is shown as a chart state, every action
//   in Next has a place, and the map names nothing the model or the chart lacks;
// - TLC's run: it completed with no error and an empty queue, so TypeOK and NoDeadlock hold on every
//   reachable state;
// - the graph is this model's whole graph: one node for each distinct state TLC found, one edge for
//   each state it generated after the initial ones, an initial node for each initial state, and each
//   node's label giving every variable of Captain.tla once and no other;
// - every step named by the stand-still action leaves its state unchanged (an edge back to its own
//   node), at an end, so skipping it hides no step and masks no deadlock; and every other step
//   changes the model's state, so no action passes for a step while it records nothing;
// - every other reachable step: its action has a map entry, and the step's place, the chart state it
//   stays inside or the chart arrow between the chart states of its two ends, is one the map gives
//   that action. Steps inside one chart state are checked too;
// - every place the map gives an action is taken by at least one step, every chart arrow has at
//   least one step under it, every chart state shows at least one reachable model state, every
//   model state is reached and every action takes at least one step.
// It prints the cases view-map.mjs lists as mismatches: model cases no chart words are true of.
import { spawnSync } from "node:child_process";
import { createHash } from "node:crypto";
import { existsSync, mkdtempSync, readdirSync, readFileSync, copyFileSync, rmSync } from "node:fs";
import { homedir, tmpdir } from "node:os";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { read } from "./chart-words.mjs";
import { states as shownAs, actions, standStill, mismatches, place } from "./view-map.mjs";

// The SHA-256 of chart.mmd's visible lines, trimmed and joined with "\n": the chart Balazs approved
// at the DYD-1440 walkthrough ("Okay, I like this statechart. This looks good."), as committed at
// 8077c474. Only he changes these words; a new pin goes with his approval of the new words.
const approvedWords = "f5a0b99c11799be14e44542faa78c0b97201ba6162103c53861019ccb2055d64";

const here = (name) => fileURLToPath(new URL("./" + name, import.meta.url));

function managed(relative) {
  const root = path.join(process.env.XDG_DATA_HOME || path.join(homedir(), ".local", "share"), "dydo", "statechart", "deps");
  if (!existsSync(root)) return undefined;
  const platform = process.platform + "-" + process.arch;
  for (const lock of readdirSync(root).sort().reverse()) {
    const found = relative(path.join(root, lock, platform));
    if (found && existsSync(found)) return found;
  }
  return undefined;
}

function javaPath() {
  if (process.env.JAVA) return process.env.JAVA;
  return managed((dir) => {
    const content = path.join(dir, "java", "content");
    if (!existsSync(content)) return undefined;
    const jre = readdirSync(content).find((name) => existsSync(path.join(content, name, "bin", "java")));
    return jre && path.join(content, jre, "bin", "java");
  });
}

function jarPath() {
  return process.env.TLA2TOOLS || managed((dir) => path.join(dir, "tlc", "tla2tools.jar"));
}

const number = (text) => Number(text.replace(/,/g, ""));

// TLC's state graph of the model beside this file: each node's variables, the initial nodes, and each
// step as [action, from node, to node], with TLC's own counts.
function runTlc() {
  const java = javaPath();
  const jar = jarPath();
  if (!java || !jar) {
    console.error("no Java or TLC jar: set JAVA and TLA2TOOLS, or run dydo statechart once to install them");
    process.exit(2);
  }
  const dir = mkdtempSync(path.join(tmpdir(), "captain-view-"));
  try {
    copyFileSync(here("Captain.tla"), path.join(dir, "Captain.tla"));
    copyFileSync(here("Captain.cfg"), path.join(dir, "Captain.cfg"));
    const tlc = spawnSync(
      java,
      ["-Xmx2g", "-XX:+UseParallelGC", "-cp", jar, "tlc2.TLC", "-workers", "1", "-nowarning", "-metadir",
        path.join(dir, "meta"), "-dump", "dot,actionlabels", path.join(dir, "graph"), "-config", "Captain.cfg", "Captain.tla"],
      { cwd: dir, encoding: "utf8", maxBuffer: 64 * 1024 * 1024 },
    );
    const out = (tlc.stdout || "") + (tlc.stderr || "");
    const complete = tlc.status === 0 && /Model checking completed\. No error has been found\./.test(out) &&
      /\s0 states left on queue/.test(out);
    const counts = /([\d,]+) states generated, ([\d,]+) distinct states found/.exec(out);
    const initial = /Finished computing initial states: ([\d,]+) distinct states? generated/.exec(out);
    const depth = /The depth of the complete state graph search is (\d+)/.exec(out);
    const summary = counts ? counts[1] + " generated, " + counts[2] + " distinct, depth " + (depth ? depth[1] : "?") : "";
    if (!complete || !counts || !initial)
      return { problems: ["TLC did not complete without error (exit " + tlc.status + "):\n" + out.trim()], summary };
    const graph = readDot(readFileSync(path.join(dir, "graph.dot"), "utf8"));
    return { ...graph, summary, generated: number(counts[1]), distinct: number(counts[2]), initialCount: number(initial[1]), java, jar };
  } finally {
    rmSync(dir, { recursive: true, force: true });
  }
}

function readDot(text) {
  const nodes = new Map();
  const values = new Map();
  const initial = new Set();
  const steps = [];
  const problems = [];
  for (const line of text.split("\n")) {
    const node = /^(-?\d+) \[label="(.*)"(,style = filled)?\];?$/.exec(line);
    if (node) {
      const assigned = [...node[2].matchAll(/\/\\\\ (\w+) = /g)].map((m) => m[1]);
      const value = /\/\\\\ state = \\"(\w+)\\"/.exec(node[2]);
      if (!value) problems.push("a TLC state with no state value: " + node[1]);
      else nodes.set(node[1], value[1]);
      values.set(node[1], assigned);
      if (node[3]) initial.add(node[1]);
      continue;
    }
    const edge = /^(-?\d+) -> (-?\d+) \[label="([^"]*)"/.exec(line);
    if (edge) steps.push([edge[3], edge[1], edge[2]]);
  }
  return { nodes, values, initial, steps, problems };
}

if (process.argv.length > 2) {
  console.error("usage: node check-chart.mjs   (it takes no arguments: it always runs TLC on the model beside it)");
  process.exit(2);
}
const chart = read(here("chart.mmd"));
const model = readFileSync(here("Captain.tla"), "utf8");
const config = readFileSync(here("Captain.cfg"), "utf8");
const graph = runTlc();

const modelStates = new Set(/^States == \{([^}]*)\}/m.exec(model)[1].match(/"(\w+)"/g).map((s) => s.slice(1, -1)));
const modelEnds = new Set(/^Ends == \{([^}]*)\}/m.exec(model)[1].match(/"(\w+)"/g).map((s) => s.slice(1, -1)));
const modelVariables = /^VARIABLES (.+)$/m.exec(model)[1].split(",").map((v) => v.trim()).sort();
const nextBody = /^Next ==\n((?:    \\\/ \w+\n)+)/m.exec(model)[1];
const modelActions = new Set([...nextBody.matchAll(/\\\/ (\w+)/g)].map((m) => m[1]).filter((a) => a !== standStill));
const chartStates = new Set(Object.keys(chart.states));
const chartArrows = new Set(Object.keys(chart.arrows));

const problems = [...chart.problems, ...(graph.problems ?? [])];
const say = (text) => problems.push(text);

const words = createHash("sha256").update(chart.visible.join("\n")).digest("hex");
if (words !== approvedWords)
  say("chart.mmd's visible words are not the words Balazs approved (sha256 " + words + ", approved " + approvedWords +
    "): a change to a state's or an arrow's words, or a new line, is his to approve");
const settings = config.split("\n").map((line) => line.trim()).filter((line) => line !== "" && !line.startsWith("\\*"));
if (settings.join("\n") !== ["INIT Init", "NEXT Next", "INVARIANT TypeOK"].join("\n"))
  say("Captain.cfg must say only INIT Init, NEXT Next and INVARIANT TypeOK, so that nothing is pruned; it says: " + settings.join("; "));
for (const [ends, text] of Object.entries(chart.arrows)) if (/ or /.test(text)) say('an arrow joins two cases with "or": ' + ends + ": " + text);
for (const s of modelStates) if (!(s in shownAs)) say("model state the map does not show: " + s);
for (const [s, c] of Object.entries(shownAs)) {
  if (!modelStates.has(s)) say("map names a state the model lacks: " + s);
  if (!chartStates.has(c)) say("map shows " + s + " as " + c + ", which chart.mmd lacks");
}
for (const a of modelActions) if (!(a in actions)) say("model action the map gives no place: " + a);
for (const [a, places] of Object.entries(actions)) {
  if (!modelActions.has(a)) say("map names an action the model lacks: " + a);
  for (const p of places) if (!chartArrows.has(p) && !chartStates.has(p)) say("map places " + a + " at " + p + ", which chart.mmd lacks");
}
if (shownAs[/^\s*\/\\ state = "(\w+)"/m.exec(model.slice(model.indexOf("Init ==")))[1]] !== chart.start)
  say("the chart's start arrow does not go to the chart state of the model's first state");

if (graph.steps) {
  if (graph.nodes.size !== graph.distinct)
    say("the graph has " + graph.nodes.size + " states, but TLC found " + graph.distinct + " distinct states");
  if (graph.steps.length !== graph.generated - graph.initialCount)
    say("the graph has " + graph.steps.length + " steps, but TLC generated " + (graph.generated - graph.initialCount) +
      " states after the initial ones");
  if (graph.initial.size !== graph.initialCount)
    say("the graph has " + graph.initial.size + " initial states, but TLC found " + graph.initialCount);
  const wrongLabels = [...graph.values].filter(([, assigned]) => [...assigned].sort().join(",") !== modelVariables.join(","));
  if (wrongLabels.length)
    say(wrongLabels.length + " graph states do not give each variable of Captain.tla exactly once, such as " + wrongLabels[0][0] +
      ": " + wrongLabels[0][1].join(", "));
}

const taken = new Map(Object.keys(actions).map((a) => [a, new Set()]));
const stepped = new Set();
const reached = new Set(graph.nodes?.values() ?? []);
const wrong = new Map();
for (const [action, from, to] of graph.steps ?? []) {
  const [a, b] = [graph.nodes.get(from), graph.nodes.get(to)];
  if (a === undefined || b === undefined) {
    wrong.set("dangling " + action, "a step between states the graph does not list: " + action + ", " + from + " to " + to);
    continue;
  }
  if (action === standStill) {
    if (from !== to || !modelEnds.has(a))
      wrong.set(standStill + " " + a + " " + b, "a step named " + standStill + " that is not standing still at an end: " + a + " to " + b +
        (from === to ? "" : ", a different state"));
    continue;
  }
  if (from === to) {
    wrong.set(action + " still", "a step of " + action + " that changes nothing, at " + a +
      ": every step but the stand-still must change the model's state");
    continue;
  }
  stepped.add(action);
  const where = place(a, b);
  if (!(action in actions)) {
    wrong.set(action + " " + where, "a model step whose action the map does not name: " + action + ", " + a + " to " + b);
    continue;
  }
  if (!actions[action].includes(where))
    wrong.set(action + " " + where, "a model step the map does not place there: " + action + ", " + a + " to " + b + ", at " + where);
  else taken.get(action).add(where);
}
problems.push(...wrong.values());
if (graph.steps) {
  for (const s of modelStates) if (!reached.has(s)) say("model state no reachable step reaches: " + s);
  for (const [a, places] of Object.entries(actions)) {
    if (modelActions.has(a) && !stepped.has(a)) say("model action no reachable step takes: " + a);
    for (const p of places) if (taken.get(a).size && !taken.get(a).has(p)) say("map places " + a + " at " + p + ", but no step of it is there");
  }
  const under = new Set([...taken.values()].flatMap((set) => [...set]));
  for (const ends of chartArrows) if (!under.has(ends)) say("chart arrow with no model step under it: " + ends + ": " + chart.arrows[ends]);
  const shown = new Set([...reached].map((s) => shownAs[s]));
  for (const c of chartStates) if (!shown.has(c)) say("chart state no reachable model state is shown as: " + c);
}
for (const m of mismatches) {
  const named = m.model.match(/[A-Z]\w+/g) ?? [];
  for (const name of named) if (!modelStates.has(name) && !modelActions.has(name)) say("a listed mismatch names what the model lacks: " + name);
}

if (problems.length) {
  for (const problem of problems) console.error(problem);
  process.exit(1);
}
const steps = graph.steps.filter(([a]) => a !== standStill).length;
console.log(
  "chart.mmd is a faithful view of the model, on every reachable step: its words are the approved words; " + modelStates.size +
    " model states shown as " + chartStates.size + " chart states, " + modelActions.size + " actions under " + chartArrows.size +
    " arrows or inside a state; TLC on Captain.tla as it stands, " + graph.summary + ", " + steps + " steps checked (" +
    graph.java + ", " + graph.jar + ").",
);
for (const m of mismatches) console.log("For Balazs, words not true of the model: " + m.model + " sits under " + m.under + ": " + m.why + ".");

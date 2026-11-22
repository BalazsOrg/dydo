// Written by hand. Checks that the hand-written chart.mmd is a faithful view of the comprehensive
// model in Captain.tla, through the map in view-map.mjs, on every step the model can take.
//
// Usage: node statechart/captain/check-chart.mjs [--dot <graph.dot>]
// Without --dot it runs TLC on Captain.tla and Captain.cfg itself and reads the state graph TLC
// dumps with `-dump dot,actionlabels`: every reachable state and every step, each named by its
// action. Java and the TLC jar come from JAVA and TLA2TOOLS, or else from the copies the dydo
// statechart capability installed. Exit 0 when the view is faithful; exit 1 with every problem.
//
// It checks:
// - chart.mmd, read strictly (chart-words.mjs): no line it cannot read, no arrow without words, no
//   two arrows between the same two states, and no arrow that joins two cases with "or";
// - the map against the model text: every state in States is shown as a chart state, every action
//   in Next has a place, and the map names nothing the model or the chart lacks;
// - TLC's run: it completed with no error, so TypeOK and NoDeadlock hold on every reachable state;
// - every reachable step: its action has a map entry, and the step's place, the chart state it stays
//   inside or the chart arrow between the chart states of its two ends, is one the map gives that
//   action. Steps inside one chart state are checked too;
// - every place the map gives an action is taken by at least one step, every chart arrow has at
//   least one step under it, every chart state shows at least one reachable model state, every
//   model state is reached and every action takes at least one step.
// It prints the cases view-map.mjs lists as mismatches: model cases no chart words are true of.
import { spawnSync } from "node:child_process";
import { existsSync, mkdtempSync, readdirSync, readFileSync, copyFileSync, rmSync } from "node:fs";
import { homedir, tmpdir } from "node:os";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { read } from "./chart-words.mjs";
import { states as shownAs, actions, standStill, mismatches, place } from "./view-map.mjs";

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

// TLC's state graph: each node's `state` value, and each step as [action, from node, to node].
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
    const depth = /The depth of the complete state graph search is (\d+)/.exec(out);
    const summary = counts ? counts[1] + " generated, " + counts[2] + " distinct, depth " + (depth ? depth[1] : "?") : "";
    if (!complete) return { problems: ["TLC did not complete without error (exit " + tlc.status + "):\n" + out.trim()], summary };
    return { ...readDot(readFileSync(path.join(dir, "graph.dot"), "utf8")), summary };
  } finally {
    rmSync(dir, { recursive: true, force: true });
  }
}

function readDot(text) {
  const nodes = new Map();
  const steps = [];
  const problems = [];
  for (const line of text.split("\n")) {
    const node = /^(-?\d+) \[label="(.*)"(?:,style = filled)?\];?$/.exec(line);
    if (node) {
      const value = /\/\\\\ state = \\"(\w+)\\"/.exec(node[2]);
      if (!value) problems.push("a TLC state with no state value: " + node[1]);
      else nodes.set(node[1], value[1]);
      continue;
    }
    const edge = /^(-?\d+) -> (-?\d+) \[label="([^"]*)"/.exec(line);
    if (edge) steps.push([edge[3], edge[1], edge[2]]);
  }
  return { nodes, steps, problems };
}

const args = process.argv.slice(2);
const dotAt = args.indexOf("--dot");
if (args.length && (dotAt !== 0 || args.length !== 2)) {
  console.error("usage: node check-chart.mjs [--dot <graph.dot>]");
  process.exit(2);
}
const graph = dotAt === 0 ? { ...readDot(readFileSync(args[1], "utf8")), summary: "from " + args[1] } : runTlc();
const chart = read(here("chart.mmd"));
const model = readFileSync(here("Captain.tla"), "utf8");

const modelStates = new Set(/^States == \{([^}]*)\}/m.exec(model)[1].match(/"(\w+)"/g).map((s) => s.slice(1, -1)));
const nextBody = /^Next ==\n((?:    \\\/ \w+\n)+)/m.exec(model)[1];
const modelActions = new Set([...nextBody.matchAll(/\\\/ (\w+)/g)].map((m) => m[1]).filter((a) => a !== standStill));
const chartStates = new Set(Object.keys(chart.states));
const chartArrows = new Set(Object.keys(chart.arrows));

const problems = [...chart.problems, ...(graph.problems ?? [])];
const say = (text) => problems.push(text);

for (const [ends, words] of Object.entries(chart.arrows)) if (/ or /.test(words)) say('an arrow joins two cases with "or": ' + ends + ": " + words);
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

const taken = new Map(Object.keys(actions).map((a) => [a, new Set()]));
const stepped = new Set();
const reached = new Set(graph.nodes?.values() ?? []);
const wrong = new Map();
for (const [action, from, to] of graph.steps ?? []) {
  if (action === standStill) continue;
  stepped.add(action);
  const [a, b] = [graph.nodes.get(from), graph.nodes.get(to)];
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
const steps = (graph.steps ?? []).filter(([a]) => a !== standStill).length;
console.log(
  "chart.mmd is a faithful view of the model, on every reachable step: " + modelStates.size + " model states shown as " +
    chartStates.size + " chart states, " + modelActions.size + " actions under " + chartArrows.size +
    " arrows or inside a state; TLC " + graph.summary + ", " + steps + " steps checked.",
);
for (const m of mismatches) console.log("For Balazs, words not true of the model: " + m.model + " sits under " + m.under + ": " + m.why + ".");

// Written by hand. Checks that the hand-written chart.mmd is a faithful view of the model in
// Captain.tla (on CaptainBase.tla), through the map in view-map.mjs, on every step the model can take.
//
// Usage: node statechart/captain/check-chart.mjs   (it takes no arguments)
// It runs TLC on Captain.tla and Captain.cfg as they stand beside it, with every .tla file beside it,
// in a new scratch folder, and reads the state graph TLC dumps (tlc.mjs): every reachable state with
// the value of each variable, and every step, named by its action. It reads no graph from anywhere
// else. Exit 0 when the view is faithful; exit 1 with every problem; exit 2 when it cannot run.
//
// It checks, and so assumes nothing it does not check:
// - chart.mmd's visible words, every line Mermaid renders, are the words Balazs approved (DYD-1440):
//   their SHA-256 must be the one pinned below, so any change to a state's or an arrow's words fails;
// - chart.mmd, read strictly (chart-words.mjs): no line it cannot read, no arrow without words, no
//   two arrows between the same two states, and no arrow that joins two cases with "or";
// - Captain.cfg says only INIT Init, NEXT Next and INVARIANT TypeOK, so TLC explores the whole model
//   with nothing pruned and checks for deadlock;
// - the map against the model text: every state in States is shown as a chart state, every action
//   in Next has a place, every chart arrow has a condition, and the map names nothing the model or the
//   chart lacks;
// - TLC's run: it completed with no error and an empty queue, so TypeOK and NoDeadlock hold on every
//   reachable state;
// - the graph is this model's whole graph: one node for each distinct state TLC found, one edge for
//   each state it generated after the initial ones, an initial node for each initial state, and each
//   node giving every variable of CaptainBase.tla once and no other;
// - every step named by the stand-still action leaves its state unchanged (an edge back to its own
//   node), at an end, so skipping it hides no step and masks no deadlock; and every other step
//   changes the model's state, so no action passes for a step while it records nothing;
// - every other reachable step: its action has a map entry, and the step's place, the chart state it
//   stays inside or the chart arrow between the chart states of its two ends, is one the map gives
//   that action. Steps inside one chart state are checked too;
// - every step under a chart arrow: the arrow's condition holds of it, over the values of the step's
//   two states and its events, so an arrow is never taken where its words are false of the step;
// - every place the map gives an action is taken by at least one step, every chart arrow has at
//   least one step under it, every chart state shows at least one reachable model state, every
//   model state is reached and every action takes at least one step.
// It prints the cases view-map.mjs lists as mismatches: model cases no chart words are true of.
import { createHash } from "node:crypto";
import { readFileSync } from "node:fs";
import { fileURLToPath } from "node:url";
import { read } from "./chart-words.mjs";
import { runTlc } from "./tlc.mjs";
import { states as shownAs, actions, standStill, mismatches, place, conditions } from "./view-map.mjs";

// The SHA-256 of chart.mmd's visible lines, trimmed and joined with "\n": the chart Balazs approved
// at the DYD-1440 walkthrough ("Okay, I like this statechart. This looks good."), as committed at
// 8077c474. Only he changes these words; a new pin goes with his approval of the new words.
const approvedWords = "f5a0b99c11799be14e44542faa78c0b97201ba6162103c53861019ccb2055d64";

const folder = fileURLToPath(new URL(".", import.meta.url));
const here = (name) => fileURLToPath(new URL("./" + name, import.meta.url));

if (process.argv.length > 2) {
  console.error("usage: node check-chart.mjs   (it takes no arguments: it always runs TLC on the model beside it)");
  process.exit(2);
}
const chart = read(here("chart.mmd"));
const base = readFileSync(here("CaptainBase.tla"), "utf8");
const model = readFileSync(here("Captain.tla"), "utf8");
const config = readFileSync(here("Captain.cfg"), "utf8");
const graph = runTlc({ folder, spec: "Captain", config: here("Captain.cfg"), dump: true });
if (graph.unavailable) {
  console.error(graph.unavailable);
  process.exit(2);
}

const names = (text, definition) => new Set(new RegExp("^" + definition + " == \\{([^}]*)\\}", "m").exec(text)[1].match(/"(\w+)"/g).map((s) => s.slice(1, -1)));
const modelStates = names(base, "States");
const modelEnds = names(base, "Ends");
const modelVariables = /^VARIABLES (.+)$/m.exec(base)[1].split(",").map((v) => v.trim()).sort();
const nextBody = /^Next ==\n((?:    \\\/ \w+\n)+)/m.exec(model)[1];
const modelActions = new Set([...nextBody.matchAll(/\\\/ (\w+)/g)].map((m) => m[1]).filter((a) => a !== standStill));
const chartStates = new Set(Object.keys(chart.states));
const chartArrows = new Set(Object.keys(chart.arrows));

const problems = [...chart.problems];
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
for (const ends of chartArrows) if (!conditions[ends]) say("chart arrow with no condition in the map: " + ends);
for (const ends of Object.keys(conditions)) if (!chartArrows.has(ends)) say("map gives a condition to an arrow chart.mmd lacks: " + ends);
if (shownAs[/^\s*\/\\ state = "(\w+)"/m.exec(base.slice(base.indexOf("Init ==")))[1]] !== chart.start)
  say("the chart's start arrow does not go to the chart state of the model's first state");

if (!graph.steps) {
  say("TLC did not complete without error (exit " + graph.status + "):\n" + graph.out.trim());
} else {
  problems.push(...graph.problems);
  if (graph.nodes.size !== graph.distinct)
    say("the graph has " + graph.nodes.size + " states, but TLC found " + graph.distinct + " distinct states");
  if (graph.steps.length !== graph.generated - graph.initialCount)
    say("the graph has " + graph.steps.length + " steps, but TLC generated " + (graph.generated - graph.initialCount) +
      " states after the initial ones");
  if (graph.initial.size !== graph.initialCount)
    say("the graph has " + graph.initial.size + " initial states, but TLC found " + graph.initialCount);
  const wrongLabels = [...graph.nodes].filter(([, values]) => Object.keys(values).sort().join(",") !== modelVariables.join(","));
  if (wrongLabels.length)
    say(wrongLabels.length + " graph states do not give each variable of CaptainBase.tla exactly once, such as " + wrongLabels[0][0] +
      ": " + Object.keys(wrongLabels[0][1]).join(", "));
}

const taken = new Map(Object.keys(actions).map((a) => [a, new Set()]));
const stepped = new Set();
const reached = new Set([...(graph.nodes?.values() ?? [])].map((values) => values.state));
const wrong = new Map();
const show = (values) => Object.entries(values).map(([k, v]) => k + "=" + JSON.stringify(v)).join(" ");
for (const [action, from, to] of graph.steps ?? []) {
  const [x, y] = [graph.nodes.get(from), graph.nodes.get(to)];
  if (x === undefined || y === undefined) {
    wrong.set("dangling " + action, "a step between states the graph does not list: " + action + ", " + from + " to " + to);
    continue;
  }
  const [a, b] = [x.state, y.state];
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
  if (!actions[action].includes(where)) {
    wrong.set(action + " " + where, "a model step the map does not place there: " + action + ", " + a + " to " + b + ", at " + where);
    continue;
  }
  taken.get(action).add(where);
  const condition = conditions[where];
  if (condition && !condition.holds(x, y))
    wrong.set("condition " + action + " " + where, "a step of " + action + " under " + where + " where its words are false of it (" +
      condition.claim + "):\n  from " + show(x) + "\n  to   " + show(y));
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
    " arrows or inside a state, each arrow's condition holding on every step under it; TLC on Captain.tla as it stands, " +
    graph.summary + ", " + steps + " steps checked (" + graph.java + ", " + graph.jar + ").",
);
for (const m of mismatches) console.log("For Balazs, words not true of the model: " + m.model + " sits under " + m.under + ": " + m.why + ".");

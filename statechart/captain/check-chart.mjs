// Written by hand. Checks that the hand-written chart.mmd is a faithful view of the comprehensive
// model in Captain.tla, through the map in view-map.mjs.
// Usage: node statechart/captain/check-chart.mjs <the chart.mmd the statechart tool drew with view.mjs>
// Exit 0 when the view is faithful; exit 1 with every difference otherwise.
//
// The map, read from the files:
// - every model state in Captain.tla's States is shown as a chart state, and the map names no other;
// - every action in Captain.tla's Next has a place: a chart arrow or one chart state;
// - every chart arrow has at least one action under it, so no arrow claims what the model lacks.
// The drawing, from TLC's walk of every reachable step:
// - the drawn chart and chart.mmd have the same states and the same arrows, in the same words;
// - no step left the map or the chart (view.mjs draws those "NOT ON THE ...");
// - no arrow joins two cases with "or".
import { readFileSync } from "node:fs";
import { fileURLToPath } from "node:url";
import { read, inWords } from "./chart-words.mjs";
import { states as shownAs, actions, standStill } from "./view-map.mjs";

const drawnPath = process.argv[2];
if (!drawnPath) {
  console.error("usage: node check-chart.mjs <the drawn chart.mmd>");
  process.exit(2);
}
const here = (name) => fileURLToPath(new URL("./" + name, import.meta.url));
const written = read(here("chart.mmd"));
const drawn = read(drawnPath);
const model = readFileSync(here("Captain.tla"), "utf8");

const modelStates = new Set(/^States == \{([^}]*)\}/m.exec(model)[1].match(/"(\w+)"/g).map((s) => s.slice(1, -1)));
const nextBody = /^Next ==\n((?:    \\\/ \w+\n)+)/m.exec(model)[1];
const modelActions = new Set([...nextBody.matchAll(/\\\/ (\w+)/g)].map((m) => m[1]).filter((a) => a !== standStill));
const chartStates = new Set(Object.keys(written.states));
const chartArrows = new Set(Object.keys(written.arrows).filter((ends) => !ends.startsWith("[*]")));
const places = Object.values(actions).flat();

const writtenArrows = new Set(inWords(written));
const drawnArrows = new Set(inWords(drawn));
const writtenStates = new Set(Object.values(written.states));
const drawnStates = new Set(Object.values(drawn.states));

const problems = [
  ...[...modelStates].filter((s) => !(s in shownAs)).map((s) => "model state the map does not show: " + s),
  ...Object.keys(shownAs).filter((s) => !modelStates.has(s)).map((s) => "map names a state the model lacks: " + s),
  ...Object.entries(shownAs).filter(([, c]) => !chartStates.has(c)).map(([s, c]) => "map shows " + s + " as " + c + ", which chart.mmd lacks"),
  ...[...modelActions].filter((a) => !(a in actions)).map((a) => "model action the map gives no place: " + a),
  ...Object.keys(actions).filter((a) => !modelActions.has(a)).map((a) => "map names an action the model lacks: " + a),
  ...places.filter((p) => !chartArrows.has(p) && !chartStates.has(p)).map((p) => "map places an action at " + p + ", which chart.mmd lacks"),
  ...[...chartArrows].filter((ends) => !places.includes(ends)).map((ends) => "chart arrow with no model action under it: " + ends),
  ...[...writtenArrows].filter((a) => !drawnArrows.has(a)).map((a) => "on chart.mmd, no model step drawn under it: " + a),
  ...[...drawnArrows].filter((a) => !writtenArrows.has(a)).map((a) => "a model step, not on chart.mmd: " + a),
  ...[...writtenStates].filter((s) => !drawnStates.has(s)).map((s) => "state on chart.mmd the model never reaches: " + s),
  ...[...drawnStates].filter((s) => !writtenStates.has(s)).map((s) => "state in the model, not on chart.mmd: " + s),
  ...[...writtenArrows].filter((a) => / or /.test(a.split(": ").slice(1).join(": "))).map((a) => 'an arrow joins two cases with "or": ' + a),
];
if (problems.length) {
  for (const problem of problems) console.error(problem);
  process.exit(1);
}
console.log(
  "chart.mmd is a faithful view of the model: " + modelStates.size + " model states shown as " + writtenStates.size +
    " chart states, " + modelActions.size + " actions under " + writtenArrows.size + " arrows or inside a state",
);

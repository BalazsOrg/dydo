// Written by hand. Checks that the hand-written chart.mmd and the chart the statechart tool draws
// from Captain.tla are the same machine: the same states and the same arrows, in the same words.
// Usage: node statechart/captain/check-chart.mjs <the drawn chart.mmd>
// Exit 0 when they match; exit 1 with every difference otherwise.
import { fileURLToPath } from "node:url";
import { read, inWords } from "./chart-words.mjs";

const drawnPath = process.argv[2];
if (!drawnPath) {
  console.error("usage: node check-chart.mjs <the drawn chart.mmd>");
  process.exit(2);
}
const written = read(fileURLToPath(new URL("./chart.mmd", import.meta.url)));
const drawn = read(drawnPath);
const writtenArrows = new Set(inWords(written));
const drawnArrows = new Set(inWords(drawn));
const writtenStates = new Set(Object.values(written.states));
const drawnStates = new Set(Object.values(drawn.states));

const problems = [
  ...[...writtenArrows].filter((a) => !drawnArrows.has(a)).map((a) => "on chart.mmd, not in the model: " + a),
  ...[...drawnArrows].filter((a) => !writtenArrows.has(a)).map((a) => "in the model, not on chart.mmd: " + a),
  ...[...writtenStates].filter((s) => !drawnStates.has(s)).map((s) => "state on chart.mmd the model never reaches: " + s),
  ...[...drawnStates].filter((s) => !writtenStates.has(s)).map((s) => "state in the model, not on chart.mmd: " + s),
  ...[...writtenArrows].filter((a) => / or /.test(a.split(": ").slice(1).join(": "))).map((a) => 'an arrow joins two cases with "or": ' + a),
];
if (problems.length) {
  for (const problem of problems) console.error(problem);
  process.exit(1);
}
console.log("chart.mmd and the model agree: " + writtenStates.size + " states, " + writtenArrows.size + " arrows");

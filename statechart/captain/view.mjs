// Written by hand. The view the statechart tool draws Captain.tla with. It holds no words of its
// own: it reads them from chart.mmd, so the drawn chart and the hand-written one cannot drift apart
// in wording. A model step that chart.mmd has no arrow for is drawn "NOT ON THE CHART", and
// check-chart.mjs fails on it.
import { fileURLToPath } from "node:url";
import { read } from "./chart-words.mjs";

const { states, arrows } = read(fileURLToPath(new URL("./chart.mmd", import.meta.url)));

export const title = "A captain and its ticket";
export const state = (v) => states[v.state] ?? "NOT ON THE CHART: " + v.state;
// "Ended" is the model's stand-still at an end; it is no arrow.
export const step = (action, from, to) =>
  action === "Ended" ? null : arrows[from.state + " --> " + to.state] ?? "NOT ON THE CHART: " + action;

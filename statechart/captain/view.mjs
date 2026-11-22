// Written by hand. The view the statechart tool draws Captain.tla with: the simple chart. It holds
// no words of its own: it reads them from chart.mmd, and it shows each model state and step through
// view-map.mjs. A model state the map does not name, a step the map does not give its action, and a
// step between two chart states that chart.mmd has no arrow for are each drawn with "NOT ON THE",
// and check-chart.mjs fails on them.
import { fileURLToPath } from "node:url";
import { read } from "./chart-words.mjs";
import { states as shownAs, mapped, standStill } from "./view-map.mjs";

const { states, arrows } = read(fileURLToPath(new URL("./chart.mmd", import.meta.url)));

export const title = "A captain and its ticket";
export const state = (v) => states[shownAs[v.state]] ?? "NOT ON THE MAP: " + v.state;
export const step = (action, from, to) => {
  if (action === standStill) return null;
  if (!mapped(action, from.state, to.state))
    return "NOT ON THE MAP: " + action + " from " + from.state + " to " + to.state;
  const [a, b] = [shownAs[from.state], shownAs[to.state]];
  if (a === b) return null;
  return arrows[a + " --> " + b] ?? "NOT ON THE CHART: " + action;
};

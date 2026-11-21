// Written by hand. Reads the words of a Mermaid state chart: each state's label, and each arrow
// as "from label --> to label: arrow label". chart.mmd holds the only copy of the captain's words;
// view.mjs and check-chart.mjs both read them through this module.
import { readFileSync } from "node:fs";

// The statechart tool writes some characters as Mermaid entities, such as #183; for a middle dot.
const plain = (text) => text.replace(/#(\d+);/g, (_, code) => String.fromCodePoint(Number(code))).trim();

export function read(path) {
  const states = {};
  const arrows = {};
  for (const line of readFileSync(path, "utf8").split("\n").map((raw) => raw.trim())) {
    const arrow = /^(\S+) --> (\S+): (.+)$/.exec(line);
    const state = /^([A-Za-z]\w*): (.+)$/.exec(line);
    if (arrow) arrows[arrow[1] + " --> " + arrow[2]] = plain(arrow[3]);
    else if (state && state[1] !== "title") states[state[1]] = plain(state[2]);
  }
  return { states, arrows };
}

// A chart as the set of its arrows in words, so two charts with different state ids compare equal.
export function inWords({ states, arrows }) {
  return Object.entries(arrows)
    .map(([ends, label]) => {
      const [from, to] = ends.split(" --> ");
      return (states[from] ?? from) + " --> " + (states[to] ?? to) + ": " + label;
    })
    .sort();
}

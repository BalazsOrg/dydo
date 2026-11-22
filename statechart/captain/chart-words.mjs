// Written by hand. Reads the words of chart.mmd strictly: each state's label, and each arrow as
// "From --> To" with its words. chart.mmd holds the only copy of the captain's words; view.mjs and
// check-chart.mjs both read them through this module.
//
// Every line must be one this reader knows: the frontmatter, the diagram header, a comment, a blank
// line, a state with its words, the one start arrow, or an arrow between two states with its words.
// Anything else is a problem, so no Mermaid edge in the file goes unread. It also returns the visible
// text: every line Mermaid renders, trimmed, in file order; that is every line but blanks and "%%"
// comments.
import { readFileSync } from "node:fs";

const plain = (text) => text.replace(/#(\d+);/g, (_, code) => String.fromCodePoint(Number(code))).trim();

export function read(path) {
  const states = {};
  const arrows = {};
  const problems = [];
  const visible = [];
  let start;
  const lines = readFileSync(path, "utf8").split("\n");
  let i = 0;
  if (lines[0]?.trim() === "---") {
    for (i = 1; i < lines.length && lines[i].trim() !== "---"; i++);
    i++;
    visible.push(...lines.slice(0, Math.min(i, lines.length)).map((line) => line.trim()).filter((line) => line !== ""));
  }
  for (; i < lines.length; i++) {
    const line = lines[i].trim();
    const where = "chart.mmd:" + (i + 1) + ": ";
    if (line === "" || line.startsWith("%%")) continue;
    visible.push(line);
    if (line === "stateDiagram-v2") continue;
    if (line.includes("-->")) {
      const arrow = /^(\[\*\]|[A-Za-z]\w*)\s*-->\s*([A-Za-z]\w*|\[\*\])\s*(?::\s*(.*))?$/.exec(line);
      if (!arrow) {
        problems.push(where + "an arrow the check cannot read: " + line);
        continue;
      }
      const [, from, to, words] = arrow;
      if (from === "[*]") {
        if (start || words || to === "[*]") problems.push(where + "a start arrow the check cannot place: " + line);
        else start = to;
        continue;
      }
      if (to === "[*]") {
        problems.push(where + "an arrow to the end marker, which the model has no step for: " + line);
        continue;
      }
      const ends = from + " --> " + to;
      if (!words || !plain(words)) problems.push(where + "an arrow with no words: " + ends);
      else if (ends in arrows) problems.push(where + "a second arrow between the same two states: " + ends);
      else arrows[ends] = plain(words);
      continue;
    }
    const state = /^([A-Za-z]\w*)\s*:\s*(.+)$/.exec(line);
    if (state) {
      if (state[1] in states) problems.push(where + "a state declared twice: " + state[1]);
      else states[state[1]] = plain(state[2]);
      continue;
    }
    problems.push(where + "a line the check cannot read: " + line);
  }
  for (const ends of Object.keys(arrows))
    for (const end of ends.split(" --> "))
      if (!(end in states)) problems.push("chart.mmd: an arrow from or to a state it does not declare: " + ends);
  if (start && !(start in states)) problems.push("chart.mmd: the start arrow goes to a state it does not declare: " + start);
  if (!start) problems.push("chart.mmd: no start arrow");
  return { states, arrows, start, problems, visible };
}

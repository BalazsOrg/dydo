// Written by hand. Runs TLC on the captain model as it stands in one folder, and reads the state
// graph it dumps (`-dump dot,actionlabels`): every reachable state with the value of each variable,
// and every step named by its action. check-chart.mjs and mutants.mjs both use it.
//
// Java and the TLC jar come from JAVA and TLA2TOOLS, or else from the copies the dydo statechart
// capability installed.
import { spawnSync } from "node:child_process";
import { copyFileSync, existsSync, mkdtempSync, readdirSync, readFileSync, rmSync } from "node:fs";
import { homedir, tmpdir } from "node:os";
import path from "node:path";

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

export function javaPath() {
  if (process.env.JAVA) return process.env.JAVA;
  return managed((dir) => {
    const content = path.join(dir, "java", "content");
    if (!existsSync(content)) return undefined;
    const jre = readdirSync(content).find((name) => existsSync(path.join(content, name, "bin", "java")));
    return jre && path.join(content, jre, "bin", "java");
  });
}

export function jarPath() {
  return process.env.TLA2TOOLS || managed((dir) => path.join(dir, "tlc", "tla2tools.jar"));
}

const number = (text) => Number(text.replace(/,/g, ""));

// Runs TLC on <spec>.tla with <config> in a new scratch folder holding a copy of every .tla file in
// `folder` and of `extra` files. With `dump`, it also reads the state graph. Returns TLC's output and
// counts, the graph, and problems.
export function runTlc({ folder, spec, config, extra = [], dump = false }) {
  const java = javaPath();
  const jar = jarPath();
  if (!java || !jar) return { unavailable: "no Java or TLC jar: set JAVA and TLA2TOOLS, or run dydo statechart once to install them" };
  const dir = mkdtempSync(path.join(tmpdir(), "captain-tlc-"));
  try {
    for (const name of readdirSync(folder)) if (name.endsWith(".tla")) copyFileSync(path.join(folder, name), path.join(dir, name));
    for (const file of [config, ...extra]) copyFileSync(file, path.join(dir, path.basename(file)));
    const args = ["-Xmx2g", "-XX:+UseParallelGC", "-cp", jar, "tlc2.TLC", "-workers", "1", "-nowarning", "-metadir", path.join(dir, "meta")];
    if (dump) args.push("-dump", "dot,actionlabels", path.join(dir, "graph"));
    args.push("-config", path.basename(config), spec + ".tla");
    const tlc = spawnSync(java, args, { cwd: dir, encoding: "utf8", maxBuffer: 64 * 1024 * 1024 });
    const out = (tlc.stdout || "") + (tlc.stderr || "");
    const complete = tlc.status === 0 && /Model checking completed\. No error has been found\./.test(out) && /\s0 states left on queue/.test(out);
    const counts = /([\d,]+) states generated, ([\d,]+) distinct states found/.exec(out);
    const initial = /Finished computing initial states: ([\d,]+) distinct states? generated/.exec(out);
    const depth = /The depth of the complete state graph search is (\d+)/.exec(out);
    const summary = counts ? counts[1] + " generated, " + counts[2] + " distinct, depth " + (depth ? depth[1] : "?") : "";
    const result = { out, status: tlc.status, complete, summary, java, jar };
    if (!complete || !counts || !initial) return result;
    Object.assign(result, { generated: number(counts[1]), distinct: number(counts[2]), initialCount: number(initial[1]) });
    if (dump) Object.assign(result, readDot(readFileSync(path.join(dir, "graph.dot"), "utf8")));
    return result;
  } finally {
    rmSync(dir, { recursive: true, force: true });
  }
}

// The graph TLC dumps: for each node its variables, parsed into JavaScript values; the initial nodes;
// and each step as [action, from node, to node].
export function readDot(text) {
  const nodes = new Map();
  const initial = new Set();
  const steps = [];
  const problems = [];
  for (const line of text.split("\n")) {
    const node = /^(-?\d+) \[label="(.*)"(,style = filled)?\];?$/.exec(line);
    if (node) {
      try {
        nodes.set(node[1], readState(unescape(node[2])));
      } catch (error) {
        problems.push("a TLC state the check cannot read: " + node[1] + ": " + error.message);
      }
      if (node[3]) initial.add(node[1]);
      continue;
    }
    const edge = /^(-?\d+) -> (-?\d+) \[label="([^"]*)"/.exec(line);
    if (edge) steps.push([edge[3], edge[1], edge[2]]);
  }
  return { nodes, initial, steps, problems };
}

const unescape = (label) => label.replace(/\\(.)/g, (_, c) => (c === "n" ? "\n" : c));

// One state's label: "/\ name = value" for each variable, a value possibly over several lines.
export function readState(text) {
  const values = {};
  const parts = ("\n" + text).split(/\n\/\\ /).slice(1);
  for (const part of parts) {
    const eq = /^(\w+) = ([\s\S]*)$/.exec(part);
    if (!eq) throw new Error("no variable in: " + part.slice(0, 60));
    if (eq[1] in values) throw new Error("a variable given twice: " + eq[1]);
    values[eq[1]] = readValue(eq[2]);
  }
  return values;
}

// A TLA+ value as TLC prints it: a string, a Boolean, an integer, a set {..}, a tuple <<..>>, or a
// record [f |-> v, ..]. A set and a tuple become arrays, a record an object.
export function readValue(text) {
  let i = 0;
  const space = () => {
    while (i < text.length && /\s/.test(text[i])) i++;
  };
  const expect = (token) => {
    space();
    if (!text.startsWith(token, i)) throw new Error("expected " + token + " at " + i + " in " + text.slice(0, 80));
    i += token.length;
  };
  const list = (close, item) => {
    const items = [];
    space();
    if (text.startsWith(close, i)) {
      i += close.length;
      return items;
    }
    for (;;) {
      items.push(item());
      space();
      if (text.startsWith(close, i)) {
        i += close.length;
        return items;
      }
      expect(",");
    }
  };
  const value = () => {
    space();
    if (text[i] === '"') {
      const end = text.indexOf('"', i + 1);
      const s = text.slice(i + 1, end);
      i = end + 1;
      return s;
    }
    if (text.startsWith("TRUE", i)) return (i += 4), true;
    if (text.startsWith("FALSE", i)) return (i += 5), false;
    const digits = /^-?\d+/.exec(text.slice(i));
    if (digits) return (i += digits[0].length), Number(digits[0]);
    if (text.startsWith("<<", i)) return (i += 2), list(">>", value);
    if (text[i] === "{") return (i += 1), list("}", value);
    if (text[i] === "[") {
      i += 1;
      const record = {};
      for (const [k, v] of list("]", () => {
        space();
        const key = /^\w+/.exec(text.slice(i))[0];
        i += key.length;
        expect("|->");
        return [key, value()];
      }))
        record[k] = v;
      return record;
    }
    throw new Error("a value the check cannot read at " + i + ": " + text.slice(i, i + 40));
  };
  const v = value();
  space();
  if (i !== text.length) throw new Error("text after the value: " + text.slice(i, i + 40));
  return v;
}

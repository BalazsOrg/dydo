// built from the model; edit there: tools/SkillBuild
const states = {
  "Uninitialised": "A project before dydo init · N12 DR 056 §1",
  "Initialised": "Settings written: native memory off, the Claude guard committed, the allow list local · N12 DR 055 §1, N12 DR 056 §1–2",
  "Guarded": "An agent tool call meets the guard · N12 DR 056 §1, N12 DR 056 §3",
  "MemoryDenied": "A write to a host memory folder refused · N12 DR 055 §2",
  "CodexBackground": "Codex's background memory writer, unseen by the guard · N12 DR 055 §3"
};
const steps = {
  "Host_Background": "Codex's memory runs from a background process the guard never sees · N12 DR 055 §3",
  "Host_Call": "an agent's tool call runs through dydo guard, one policy for Claude and Codex · N12 DR 056 §1, N12 DR 056 §3",
  "Host_Deny": "a write to a host memory folder is blocked · N12 DR 055 §2",
  "Host_FlagOff": "with memories = false it writes nothing: the Codex half rests on that flag · N12 DR 055 §1, N12 DR 055 §3",
  "Host_Init": "dydo init or --join turns native memory off for both hosts, commits one Claude guard entry and keeps the allow list local · N12 DR 055 §1, N12 DR 056 §1–2",
  "Host_Pass": "not a memory write: the guard's other rules decide the call · N12 DR 056 §1",
  "Host_Refused": "the write is refused and the session goes on: the guard stops accidents, not rogue agents · N12 DR 055 §2–3"
};
export const title = "Host settings and the guard";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

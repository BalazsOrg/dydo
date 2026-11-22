// Written by hand. How the simple view (chart.mmd) relates to the model (Captain.tla on CaptainBase.tla).
// The view is a projection: each model state is shown as one chart state, and each model action either
// stands under one chart arrow or stays inside one chart state. Each chart arrow has the condition its
// words claim, over the step's two states and its events (the event record). check-chart.mjs tests
// every reachable step against this map and every step under an arrow against its condition.

// Each model state, and the chart state that shows it. Its words must be true of the model state.
export const states = {
  Dispatched: "Dispatched",
  Taken: "Taken",
  Leaf: "Leaf",
  Asking: "Asking",
  Cutting: "Cutting",
  Waiting: "Waiting",
  InReview: "InReview",
  Fixing: "Fixing",
  Merging: "Merging",
  Done: "Done",
  HandedBack: "HandedBack",
};

// Each model action: the chart arrows it stands under, as "From --> To", and the chart states it stays
// inside. Every step the model takes must have its place here, and every place listed must be taken by
// at least one step.
export const actions = {
  CannotTake: ["Dispatched --> HandedBack"],
  Take: ["Dispatched --> Taken"],
  WorkItself: ["Taken --> Leaf"],
  Split: ["Taken --> Cutting"],
  SplitTheRest: ["Leaf --> Cutting"],
  ProveTheWork: ["Leaf"],
  WriteTheVerdict: ["Leaf", "Waiting"],
  AnswerTheCall: ["Leaf", "Waiting"],
  WriteTheResult: ["Leaf", "Waiting"],
  MeetACall: ["Leaf"],
  AskAQuestion: ["Leaf --> Asking"],
  Answered: ["Asking --> Leaf"],
  CloseLeaf: ["Leaf --> Done"],
  HandBackTheWork: ["Leaf --> HandedBack"],
  Dispatch: ["Cutting --> Waiting"],
  HandBackTheCut: ["Cutting --> HandedBack"],
  PartBack: ["Waiting"],
  CheckTheContract: ["Waiting"],
  CutTheNextParts: ["Waiting --> Cutting"],
  HandBackTheWhole: ["Waiting --> HandedBack"],
  WireTheParts: ["Waiting"],
  SendTheWholeToReview: ["Waiting --> InReview"],
  CloseTheWhole: ["Waiting --> Done"],
  SendLeafToReview: ["Leaf --> InReview"],
  TheReviewerWritesTheVerdict: ["InReview"],
  Pass: ["InReview --> Merging"],
  Fail: ["InReview --> Fixing"],
  Replan: ["InReview --> HandedBack"],
  TheFixIsProved: ["Fixing"],
  ReviewAgain: ["Fixing --> InReview"],
  HandBackTheFix: ["Fixing --> HandedBack"],
  RunTheGates: ["Merging"],
  Land: ["Merging"],
  GreenOnTheMergeSha: ["Merging --> Done"],
  Repair: ["Merging --> Fixing"],
};

// "Ended" is the model's stand-still at an end; it is no step. check-chart.mjs checks that every step
// it names leaves its state unchanged, at an end.
export const standStill = "Ended";

// The step has the event named n, and every event of that name in it is taken by actor a. `to` is the
// state the step reaches; its `event` is the set of events of the step.
const has = (to, n, a) => to.event.some((e) => e.name === n) && to.event.every((e) => e.name !== n || e.by === a);
const handsBack = (to) => has(to, "hand back", "captain");
// The result of a ticket that does not merge is in (register FR24).
const resultIn = (s) =>
  s.kind === "change" ? s.proved : s.kind === "verdict" ? ["pass", "fail"].includes(s.verdict) : s.kind === "answer" ? s.call === "settled" : s.written;
const same = (a, b) => a.length === b.length && a.every((x) => b.includes(x));
// Nothing of the ticket changed in the step but where the captain is and the step's events.
const untouched = (a, b) => Object.keys(a).every((k) => k === "state" || k === "event" || JSON.stringify(a[k]) === JSON.stringify(b[k]));

// Each chart arrow's condition: what its words claim of every step under it, in plain words (claim),
// and as a test over the step's two states (holds). Written from the words, never from the model's
// actions. A part that changes the model keeps every condition true, or brings the arrow to him as a
// view change.
export const conditions = {
  "Dispatched --> HandedBack": {
    claim: "the ticket is held by another agent or closed, so the captain cannot take it and hands it back untouched, with the reason on record",
    holds: (a, b) =>
      ["held", "closed"].includes(a.ticket) && has(b, "cannot take", "captain") && handsBack(b) && b.kind === "unread" && untouched(a, b),
  },
  "Dispatched --> Taken": {
    claim: "the ticket is free, the captain sets it In Progress, and its result is read from it",
    holds: (a, b) =>
      a.ticket === "free" && b.ticket === "free" && has(b, "set In Progress", "captain") && a.kind === "unread" && b.kind !== "unread",
  },
  "Taken --> Leaf": {
    claim: "the captain sizes the ticket as fitting one agent",
    holds: (a, b) => has(b, "fits one agent", "captain"),
  },
  "Taken --> Cutting": {
    claim: "the captain sizes the ticket as needing more than one agent",
    holds: (a, b) => has(b, "needs more than one agent", "captain"),
  },
  "Leaf --> Cutting": {
    claim: "the captain finds the work too big before its result is in, and splits what is left",
    holds: (a, b) => !resultIn(a) && has(b, "proves too big", "captain"),
  },
  "Leaf --> Asking": {
    claim: "the work has met a call the captain may not decide; it passes the need up, its officer opens a Question for a fresh agent, and the work waits on it",
    holds: (a, b) =>
      a.asked === "needed" && b.asked === "open" && has(b, "pass the need up", "captain") && has(b, "open a Question", "officer"),
  },
  "Asking --> Leaf": {
    claim: "the answer on the Question it waits on is back",
    holds: (a, b) => a.asked === "open" && b.asked === "none" && has(b, "answer", "question"),
  },
  "Leaf --> InReview": {
    claim: "a change this captain merges is proved, nothing in it waits on an answer, and a reviewer is dispatched onto a new round",
    holds: (a, b) =>
      a.kind === "change" && a.merges && a.proved && a.asked === "none" && b.verdict === "due" && has(b, "dispatch a reviewer", "captain"),
  },
  "Leaf --> Done": {
    claim: "the captain merges nothing, its result is in, nothing waits on an answer, and it closes the ticket",
    holds: (a, b) => !a.merges && a.asked === "none" && resultIn(a) && has(b, "close", "captain"),
  },
  "Leaf --> HandedBack": {
    claim: "the work cannot be done inside this ticket, and the captain hands it back to its officer",
    holds: (a, b) => has(b, "cannot be done inside the ticket", "captain") && handsBack(b),
  },
  "Cutting --> Waiting": {
    claim: "the parts are dispatched as sub-tickets, and the captain waits on them",
    holds: (a, b) => b.part === "out" && has(b, "dispatch the parts", "captain"),
  },
  "Cutting --> HandedBack": {
    claim: "the work cannot be cut inside this ticket, and the captain hands it back to its officer",
    holds: (a, b) => has(b, "cannot be cut inside the ticket", "captain") && handsBack(b),
  },
  "Waiting --> Cutting": {
    claim: "a part is back, the captain has found its contract not met yet and its result is not in, and it cuts the next parts",
    holds: (a, b) => a.part === "back" && a.contract === "not met" && !resultIn(a) && has(b, "cut the next parts", "captain"),
  },
  "Waiting --> HandedBack": {
    claim: "a part is back and the captain has found its contract cannot be met inside this ticket; it hands it back",
    holds: (a, b) =>
      a.part === "back" && a.contract === "cannot be met" && has(b, "a part shows the contract cannot be met", "captain") && handsBack(b),
  },
  "Waiting --> InReview": {
    claim: "the parts are back with the contract met and wired into a change this captain merges, proved, nothing waits on an answer, and a reviewer is dispatched",
    holds: (a, b) =>
      a.part === "wired" && a.contract === "met" && a.merges && a.proved && a.asked === "none" && b.verdict === "due" &&
      has(b, "dispatch a reviewer", "captain"),
  },
  "Waiting --> Done": {
    claim: "the parts are back, the captain has found its contract met and merges nothing, its result is in, and it closes the ticket",
    holds: (a, b) =>
      a.part === "back" && a.contract === "met" && !a.merges && a.asked === "none" && (a.kind === "change" || resultIn(a)) &&
      has(b, "close", "captain"),
  },
  "InReview --> Fixing": {
    claim: "the verdict written is FAIL, no blocker is fundamental, and one Fix ticket takes every blocker",
    holds: (a, b) =>
      a.verdict === "fail" && a.blockers.length > 0 && !a.blockers.includes("fundamental") && same(a.blockers, b.blockers) &&
      has(b, "act on FAIL", "captain") && has(b, "open a Fix", "captain"),
  },
  "InReview --> Merging": {
    claim: "the verdict written is PASS, and the captain acts on it",
    holds: (a, b) => a.verdict === "pass" && a.blockers.length === 0 && has(b, "act on PASS", "captain"),
  },
  "InReview --> HandedBack": {
    claim: "the verdict written is FAIL with a fundamental blocker, which cannot be fixed inside this ticket; the captain hands it back",
    holds: (a, b) => a.verdict === "fail" && a.blockers.includes("fundamental") && has(b, "act on FAIL", "captain") && handsBack(b),
  },
  "Fixing --> InReview": {
    claim: "the Fix is done, its head proved and nothing waiting on an answer, and a reviewer is dispatched onto a new round",
    holds: (a, b) => a.proved && a.asked === "none" && b.verdict === "due" && has(b, "dispatch a reviewer", "captain"),
  },
  "Fixing --> HandedBack": {
    claim: "the Fix's holder hands the Fix back undone, its head not proved, and the captain hands the ticket back",
    holds: (a, b) => !a.proved && has(b, "the Fix comes back undone", "fixer") && handsBack(b),
  },
  "Merging --> Done": {
    claim: "the merge is in, its gates and CI are green on the merge SHA, and the captain closes the ticket",
    holds: (a, b) => a.landed === "in" && b.landed === "in" && b.gates === "green" && b.ci === "green" && has(b, "close", "captain"),
  },
  "Merging --> Fixing": {
    claim: "the merge is not clean and green: a CONFLICT or a real red is on record, and a Fix ticket repairs it",
    holds: (a, b) => ["red", "conflict"].includes(a.gates) && has(b, "open a Fix", "captain"),
  },
};

// The model cases no chart state or arrow has words true of. The chart is his approved view, so its
// words are not bent to fit: each case is placed where it least misleads and brought to him for a view
// change. check-chart.mjs prints them on every run. The frame has none; the parts list theirs here.
export const mismatches = [];

// The chart place of one model step: the chart state it stays inside, or the arrow it stands under.
export function place(from, to) {
  return states[from] === states[to] ? states[from] : states[from] + " --> " + states[to];
}

// Whether a model step from one model state to another is one the map gives its action.
export function mapped(action, from, to) {
  return (actions[action] ?? []).includes(place(from, to));
}

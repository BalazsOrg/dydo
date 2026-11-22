// Written by hand. How the simple view (chart.mmd) relates to the comprehensive model (Captain.tla).
// The view is a projection: each model state is shown as one chart state, and each model action
// either stands under one chart arrow or stays inside one chart state, where the chart draws no step.
// check-chart.mjs checks every reachable step of the model against this map.

// Each model state, and the chart state that shows it. Its words must be true of the model state.
export const states = {
  Dispatched: "Dispatched",
  Taken: "Taken",
  Leaf: "Leaf", // a his-call envoy with him, a reviewer writing its verdict: still the work itself
  Asking: "Asking", // at any stage of the Question: research, council or his-call
  Cutting: "Cutting", // with fog too: then the part in sight is one that brings the next into sight
  Waiting: "Waiting", // on its parts, their evidence checks, fixes and mends, a part's Question, their wiring; see mismatches
  InReview: "InReview", // the round's Review hop out with a fresh reviewer; see mismatches for once its verdict is in
  Mending: "InReview", // NOT TRUE OF IT: see mismatches below
  Fixing: "Fixing", // on the Fix; and on the Question one of its blockers waits on
  Repairing: "Fixing", // a conflict or a real red gate is repaired on the ticket branch by a Fix
  Merging: "Merging", // its evidence: the gates, CI, the PR
  AwaitingTurn: "Merging", // a two-way PR into master waits on the coordinator's turn
  Held: "Merging", // a PR re-synced a third time, held while the coordinator sequences
  AwaitingClick: "Merging", // a one-way PR waits on his click
  Landed: "Merging", // the merge is in; the Merge hop waits on green on its merge SHA
  Rerunning: "Merging", // DYTEST reruns a red gate after the merge once
  Done: "Done",
  HandedBack: "HandedBack",
};

// Each model action: the chart arrows it stands under, as "From --> To", and the chart states it
// stays inside. Every step the model takes must have its place here, and every place listed must be
// taken by at least one step.
export const actions = {
  CannotTake: ["Dispatched --> HandedBack"],
  FillerFailsItsCheck: ["Dispatched --> HandedBack"],
  Take: ["Dispatched --> Taken"],
  WorkItself: ["Taken --> Leaf"],
  Split: ["Taken --> Cutting"],
  FindTheWay: ["Taken --> Cutting"],
  SplitTheRest: ["Leaf --> Cutting"],
  PassTheNeedUp: ["Leaf", "Waiting"],
  QuestionOpened: ["Leaf", "Cutting", "Waiting"],
  WaitOnTheAnswer: ["Leaf --> Asking"],
  ResearchTheCall: ["Leaf", "Waiting"],
  AskACouncil: ["Leaf --> Asking", "Waiting"],
  AskHim: ["Leaf --> Asking", "Waiting"],
  Answered: ["Asking --> Leaf", "Leaf", "Cutting", "Waiting", "Fixing"],
  CouncilDecided: ["Asking --> Leaf", "Waiting"],
  CouncilPrepared: ["Asking --> Leaf", "Waiting"],
  HeDecides: ["Leaf", "Waiting"],
  ProveTheWork: ["Leaf"],
  ScoutTheSpec: ["Leaf"],
  WriteTheVerdict: ["Leaf", "Waiting"],
  SendLeafToReview: ["Leaf --> InReview"],
  CloseLeaf: ["Leaf --> Done"],
  HandBackTheWork: ["Leaf --> HandedBack"],
  Dispatch: ["Cutting --> Waiting"],
  HandBackTheCut: ["Cutting --> HandedBack"],
  SomeHopStops: ["Waiting", "InReview", "Fixing"],
  SetTheHopBackToTodo: ["Waiting", "InReview", "Fixing"],
  ResumeTheHop: ["Waiting", "InReview", "Fixing"],
  PartBack: ["Waiting"],
  PartHandedBack: ["Waiting"],
  DispatchAReadyPart: ["Waiting"],
  CheckThePart: ["Waiting"],
  TheCheckPasses: ["Waiting"],
  TheCheckFindsACheapShould: ["Waiting"],
  TheCheckFails: ["Waiting"],
  NeedFromAPart: ["Waiting"],
  ThePartIsAnswered: ["Waiting"],
  CutTheNextParts: ["Waiting --> Cutting"],
  FogRemains: ["Waiting --> Cutting"],
  TheWayIsFound: ["Waiting"],
  HandBackTheWhole: ["Waiting --> HandedBack"],
  WireTheParts: ["Waiting"],
  TheWiringIsIn: ["Waiting"],
  SendTheWholeToReview: ["Waiting --> InReview"],
  CloseTheWhole: ["Waiting --> Done"],
  TheExchange: ["Waiting"],
  TheJudgesAgree: ["Waiting"],
  TheJudgesSplit: ["Waiting"],
  TheCouncilDecides: ["Waiting --> Done"],
  TheCouncilPrepares: ["Waiting --> Done"],
  TheReviewPasses: ["InReview"],
  TheReviewFails: ["InReview"],
  NoMoreRound: ["InReview --> Merging"],
  OneMoreRound: ["InReview"],
  MendACheapShould: ["InReview"],
  TheMendIsProved: ["InReview"],
  ReviewTheMendedHead: ["InReview"],
  Fail: ["InReview --> Fixing"],
  Replan: ["InReview --> HandedBack"],
  CheckpointOneMoreRound: ["InReview --> Fixing"],
  CheckpointAsksHim: ["InReview --> Fixing"],
  CheckpointEscalates: ["InReview --> HandedBack"],
  TheFixIsProved: ["Fixing"],
  ReviewAgain: ["Fixing --> InReview"],
  HandBackTheFix: ["Fixing --> HandedBack"],
  RunTheGates: ["Merging"],
  WatchTheCi: ["Merging"],
  RedBeforeLanding: ["Merging --> Fixing"],
  MergeConflict: ["Merging --> Fixing"],
  MergeIntoFeature: ["Merging"],
  OfferForTheTurn: ["Merging"],
  OpenTheHitlMerge: ["Merging"],
  MasterMoves: ["Merging"],
  LandOnTheTurn: ["Merging"],
  HisClick: ["Merging"],
  ResyncOnTheRecheck: ["Merging"],
  ConflictOnTheRecheck: ["Merging"],
  Sequenced: ["Merging"],
  GreenOnTheMergeSha: ["Merging --> Done"],
  GateRed: ["Merging"],
  Flake: ["Merging --> Done"],
  RealRed: ["Merging --> Fixing"],
  TheRepairIsProved: ["Fixing"],
  RepairReviewed: ["Fixing --> InReview"],
  RepairFails: ["Fixing --> HandedBack"],
};

// "Ended" is the model's stand-still at an end; it is no step. check-chart.mjs checks that every
// step it names leaves its state unchanged, at an end.
export const standStill = "Ended";

// The model cases no chart state or arrow has words true of. The chart is his approved view, so its
// words are not bent to fit: each case is placed where it least misleads and brought to him for a
// view change. check-chart.mjs prints them on every run.
export const mismatches = [
  {
    model: "Mending, and the steps MendACheapShould, TheMendIsProved and ReviewTheMendedHead",
    under: 'InReview, "Waiting on a reviewer\'s verdict"',
    why:
      "after a PASS with a cheap should, the captain waits on the should's fix in place before the merge " +
      "(S17), not on a verdict; the chart has no state for that wait and no arrow out of the review for it",
  },
  {
    model: "Waiting, when all it waits on is its own need (PassTheNeedUp, QuestionOpened)",
    under: 'Waiting, "Waiting on its sub-tickets"',
    why:
      "the officer that received the need opened the Question under its own ticket (H16), so the captain waits on a " +
      "ticket that is not its sub-ticket; H10 has it wait on it, and after a cut it never goes back to Asking",
  },
  {
    model: "InReview once the round's verdict is written (TheReviewPasses, TheReviewFails), before the captain acts on it",
    under: 'InReview, "Waiting on a reviewer\'s verdict"',
    why:
      "the reviewer on the round's Review hop writes the verdict, and the captain then acts on it (R17, R33, R8); " +
      "between the two the verdict is in, so the captain no longer waits on it, and the chart has no state for that moment",
  },
];

// The chart place of one model step: the chart state it stays inside, or the arrow it stands under.
export function place(from, to) {
  return states[from] === states[to] ? states[from] : states[from] + " --> " + states[to];
}

// Whether a model step from one model state to another is one the map gives its action.
export function mapped(action, from, to) {
  return (actions[action] ?? []).includes(place(from, to));
}

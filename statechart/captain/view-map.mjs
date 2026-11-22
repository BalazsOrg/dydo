// Written by hand. How the simple view (chart.mmd) relates to the comprehensive model (Captain.tla).
// The view is a projection: each model state is shown as one chart state, and each model action
// either stands under one chart arrow or stays inside one chart state, where the chart draws no step.
// view.mjs draws the model through this map, and check-chart.mjs checks it both ways.

// Each model state, and the chart state that shows it.
export const states = {
  Dispatched: "Dispatched",
  Taken: "Taken",
  Leaf: "Leaf",
  Asking: "Asking", // at any stage of the Question: research, council or his-call
  Wayfinding: "Cutting", // the fog before the parts in sight are cut
  Cutting: "Cutting",
  Waiting: "Waiting",
  InReview: "InReview",
  Clear: "InReview", // a PASS is in; one more round, a mend, or the merge comes next
  Checkpoint: "InReview", // the third FAIL is in; one more round or the officer comes next
  Mending: "InReview", // a cheap should is mended, and its new head waits on a round
  Fixing: "Fixing",
  Repairing: "Fixing", // a conflict or a real red gate is repaired on the ticket branch
  Merging: "Merging",
  AwaitingTurn: "Merging", // a two-way PR into master waits on the coordinator's turn
  Rerunning: "Merging", // DYTEST reruns a red gate once
  AwaitingClick: "Merging", // a one-way PR waits on his click
  Done: "Done",
  HandedBack: "HandedBack",
};

// Each model action: the chart arrow it stands under, as "From --> To", or the one chart state it
// stays inside. An action that ends in a model state its own case decides (Flake) lists both.
export const actions = {
  CannotTake: "Dispatched --> HandedBack",
  FillerFailsItsCheck: "Dispatched --> HandedBack",
  Take: "Dispatched --> Taken",
  WorkItself: "Taken --> Leaf",
  Split: "Taken --> Cutting",
  FindTheWay: "Taken --> Cutting",
  SplitTheRest: "Leaf --> Cutting",
  Ask: "Leaf --> Asking",
  AskACouncil: "Leaf --> Asking",
  AskHim: "Leaf --> Asking",
  Answered: "Asking --> Leaf",
  SendLeafToReview: "Leaf --> InReview",
  CloseLeaf: "Leaf --> Done",
  HandBackTheWork: "Leaf --> HandedBack",
  PartsInSight: "Cutting",
  Dispatch: "Cutting --> Waiting",
  HandBackTheCut: "Cutting --> HandedBack",
  CutTheNextParts: "Waiting --> Cutting",
  FogRemains: "Waiting --> Cutting",
  HandBackTheWhole: "Waiting --> HandedBack",
  SendTheWholeToReview: "Waiting --> InReview",
  CloseTheWhole: "Waiting --> Done",
  Pass: "InReview",
  Fail: "InReview --> Fixing",
  FailOnASecurityHole: "InReview --> Fixing",
  Replan: "InReview --> HandedBack",
  ThirdFail: "InReview",
  CheckpointOneMoreRound: "InReview --> Fixing",
  CheckpointEscalates: "InReview --> HandedBack",
  ReviewAgain: "Fixing --> InReview",
  HandBackTheFix: "Fixing --> HandedBack",
  NoMoreRound: "InReview --> Merging",
  AwaitTheTurn: "InReview --> Merging",
  OneMoreRound: "InReview",
  MendACheapShould: "InReview",
  ReviewTheMendedHead: "InReview",
  TurnGiven: "Merging",
  Merged: "Merging --> Done",
  GreenForHisClick: "Merging",
  HisClick: "Merging --> Done",
  MergeConflict: "Merging --> Fixing",
  GateRed: "Merging",
  Flake: ["Merging --> Done", "Merging"],
  RealRed: "Merging --> Fixing",
  RepairReviewed: "Fixing --> InReview",
  RepairFails: "Fixing --> HandedBack",
};

// "Ended" is the model's stand-still at an end; it is no step.
export const standStill = "Ended";

// Whether a model step from one model state to another is one the map gives its action.
export function mapped(action, from, to) {
  const shown = states[from] === states[to] ? states[from] : states[from] + " --> " + states[to];
  return [actions[action]].flat().includes(shown);
}

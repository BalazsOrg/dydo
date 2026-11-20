// built from the model; edit there: tools/SkillBuild
const states = {
  "Edited": "A claim edited in the C# model · K1, K2, K4, K17, K19, K21",
  "Rejected": "The build or its tests refuse · K4, K7, K10, K24",
  "Built": "Built files written by the builder · K5, K6, K8, K9, K12, K13, K23, K26",
  "Nudged": "A built file edited by hand: the guard nudges · K6",
  "Checked": "Golden, shape and stray-claim checks pass · K7, K10, K24",
  "TlcPassed": "TLC passes the moves against the hand-written properties · K3, K20",
  "TlcFailed": "TLC fails: the push is not done · K3, K20",
  "InReview": "Model PR read through its built-skill diff · K16",
  "Approved": "Claims approved by Balazs · K16, K20",
  "Released": "Built skills shipped downstream · K18, K15, K25, K27, K28, K29"
};
const steps = {
  "Build_Approve": "he approves: in the 4.0.0 push once, at the final candidate; after it, each model PR · K16, K20, K22",
  "Build_EditThere": "the edit goes into the model instead · K6",
  "Build_Golden": "the golden test finds a byte difference, or the stray-claim lint finds pasted claim text · K7, K24",
  "Build_HandEdit": "an Edit or Write to a built path · K6",
  "Build_Mend": "the model is fixed · K6",
  "Build_Pass": "golden, shape and stray-claim checks pass, in the suite and in CI · K7, K10, K24",
  "Build_Read": "the model PR is read through its built-skill diff · K16",
  "Build_Refused": "the build refuses: a claim not on exactly one line per carrier, a misplaced move, a shape limit · K4, K10, K9",
  "Build_Rejected": "he rejects a change at his walkthrough: new captains on new tickets edit the model, and the candidate is rebuilt (DYD-1195 8661b8f4) · K16, K22",
  "Build_Remodel": "the push is not done: the model is fixed and built again · K20, K6",
  "Build_Ship": "downstream gets the built files in one ordinary update · K18",
  "Build_Tlc": "TLC passes the generated moves against the hand-written properties; the result is recorded with the candidate · K3, K20, N14",
  "Build_TlcFail": "TLC fails a property: the push is not done and nothing goes to his approval · K3, K20",
  "Build_Write": "dotnet run --project tools/SkillBuild writes every built file with its header · K6, K8, K5, K12, K13, K23"
};
export const title = "The claim build";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

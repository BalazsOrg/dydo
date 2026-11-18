// built from the model; edit there: tools/SkillBuild
const states = {
  "Cut": "A step is cut · S1, S2, S7",
  "OneTicket": "One ticket, no hops · S4, R12",
  "EvidenceCheck": "Evidence check beside the leaf · S4, R6",
  "CheckFix": "Fix rN beside the leaf · S9, R8",
  "Chain": "Hop sub-tickets in step order · S3",
  "StepDone": "Step Done · S6",
  "AfkMerge": "AFK Merge hop · S12",
  "HitlMerge": "HITL Merge hop · S12, R27"
};
const steps = {
  "Hops_AddCheck": "the officer adds an evidence-check seat · S4, R6",
  "Hops_AfkType": "the step merges into the feature branch, or two-way into main · S12",
  "Hops_Chain": "the step has its own review loop · S3, S4",
  "Hops_CheckAgain": "Fix rN Done; the next check runs · R8, S9",
  "Hops_CheckDone": "the evidence check's Review hop is Done · S4, S6",
  "Hops_CheckFail": "the evidence check finds a blocker · S9, R11",
  "Hops_HitlType": "a one-way main merge · S12, R27",
  "Hops_LeafDone": "its holder closes it Done · S6",
  "Hops_Single": "the step has no review loop of its own · S4"
};
export const title = "A step's hops, by Type label";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

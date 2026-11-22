const states = {
  "Cut": "A step is cut · S1, S2, S7",
  "OneTicket": "One ticket, no hops · S4, R12",
  "EvidenceCheck": "Evidence check beside the leaf · S4, R6",
  "CheckFix": "Fix rN beside the leaf · S9, R8",
  "ImplementHop": "Implement hop · S3",
  "ReviewHop": "Review hop, one per round · S3, S8",
  "FixHop": "Fix hop, Fix rN · S3, S9",
  "StepDone": "Step Done · S6",
  "AfkMerge": "AFK Merge hop · S12",
  "HitlMerge": "HITL Merge hop · S12, R27"
};
const steps = {
  "Hops_AddCheck": "the officer adds an evidence-check seat · S4, R6",
  "Hops_AfkType": "PASS: the Merge hop, AFK into the feature branch or two-way into main · S12",
  "Hops_Chain": "the step has its own review loop: its Implement hop starts · S3, S4",
  "Hops_CheckAgain": "Fix rN Done; the next check runs · R8, S9",
  "Hops_CheckDone": "the evidence check's Review hop is Done · S4, S6",
  "Hops_CheckFail": "the evidence check finds a blocker · S9, R11",
  "Hops_FixedHop": "Fix rN Done; the next round's Review hop · S3, S9, R8",
  "Hops_HitlType": "PASS: the Merge hop, HITL for a one-way main merge · S12, R27",
  "Hops_ImplementDone": "Implement hop Done; Review r1 starts · S3, R2",
  "Hops_LeafDone": "its holder closes it Done · S6",
  "Hops_ReviewFails": "FAIL: a Fix rN hop · S3, S9, R11",
  "Hops_Single": "the step has no review loop of its own · S4"
};
export const title = "A step's hops, by Type label";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

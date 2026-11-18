// built from the model; edit there: tools/SkillBuild
const states = {
  "Offered": "PR into master offered · R26, R27",
  "Paced": "Waiting for its turn · R29",
  "Resync": "Re-checking against current master · R29",
  "Held": "Held after a third forced re-sync · R29",
  "AfkMerge": "AFK merge by its captain · R26, S12",
  "CiWatch": "One-way: watching its CI · R31",
  "HitlMerge": "HITL Merge waiting on his click · R27, R31, S12",
  "Merged": "Merged into master · R26, R31",
  "Reverted": "Reverted after a real red · R22, Y9",
  "Closed": "Master Merge hop Done · R20, R31"
};
const steps = {
  "MasterRoute_AfkDone": "dydo merge merges into master · R26",
  "MasterRoute_Again": "master moved: re-sync again · R29",
  "MasterRoute_CiGreen": "its CI is green · R31, S12",
  "MasterRoute_Click": "Balazs clicks the merge · R27, R31",
  "MasterRoute_Hold": "forced to re-sync a third time · R29",
  "MasterRoute_Merge": "re-check green: PASS, gates green with current master, CI green · R26, R29",
  "MasterRoute_OneWay": "one-way: a flagged path, or declared by its captain · R27",
  "MasterRoute_RealRed": "a real red after the merge · R22, Y9",
  "MasterRoute_Reland": "fixed, fix-scoped round passed, offered again · R22",
  "MasterRoute_Sequenced": "the colliding work is sequenced · R29",
  "MasterRoute_Turn": "the admiral gives it its turn · R29",
  "MasterRoute_TwoWay": "declared two-way in the PR body · R26, R27",
  "MasterRoute_Woken": "receipt and CI green on the merge · R20, R31",
  "Gap1": "GAP: R29 says the admiral lands PRs one at a time, not how a turn is signalled"
};
export const title = "A PR into master";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

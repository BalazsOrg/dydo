// built from the model; edit there: tools/SkillBuild
const states = {
  "MergeTodo": "Merge hop in Todo · S3, S12",
  "PreGate": "Running dydo test gate on the combined tree · R21, Y4",
  "PreFix": "A code-writer fixing a red gate · R21",
  "Rereview": "A fresh round on the new head · R8, R17, R23, R24",
  "Running": "Running dydo merge · R21, R23",
  "Resolving": "Resolving a CONFLICT on the ticket branch · R19, R21",
  "HunkReview": "Judging a resolution's hunks · R19",
  "Gating": "Gates on the merge · R18, R20",
  "Rerun": "Red gate rerun once · R22",
  "Reverted": "Merge reverted · R22, S12, Y9",
  "FixRound": "Fix and fix-scoped round · R22",
  "MergeDone": "Merge hop Done · R20"
};
const steps = {
  "FeatureMerge_Conflict": "dydo merge reports CONFLICT · R21, R19",
  "FeatureMerge_FixAfterRevert": "a Fix follows on the ticket branch · R22, R21",
  "FeatureMerge_Flake": "green on the rerun: a flake · R22, Y9",
  "FeatureMerge_Green": "DYTEST green on the merge tree, CI green on the merge SHA · R18, R20",
  "FeatureMerge_HandShaped": "refused: a hand-shaped tree · R19",
  "FeatureMerge_HunksPass": "PASS on the hunks; the gate and the merge run again · R19, R17",
  "FeatureMerge_Merge": "dydo merge writes the merge commit · R23, R32",
  "FeatureMerge_NoPass": "refused: no PASS on the head or a clean-sync ancestor · R17, R23, R24",
  "FeatureMerge_NoReceipt": "refused: no green receipt for the combined tree · R21, R23, Y4",
  "FeatureMerge_PreFixed": "the fix moves the head: a fresh round · R8, R23",
  "FeatureMerge_PreGreen": "a green receipt for the combined tree · Y4, R23",
  "FeatureMerge_PreRed": "a real red gate before the merge · R21",
  "FeatureMerge_RealRed": "red again: a real red · R22, Y9, S12",
  "FeatureMerge_Red": "a gate goes red after the merge · R22",
  "FeatureMerge_Reland": "the fix-scoped round passes; re-land · R22",
  "FeatureMerge_RereviewPass": "PASS on the new head · R17, R23",
  "FeatureMerge_Resolved": "the resolution is committed · R19",
  "FeatureMerge_Start": "the Merge hop's blocker clears · R21, S3, S6"
};
export const title = "The Merge hop into the feature branch";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

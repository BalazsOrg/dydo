// built from the model; edit there: tools/SkillBuild
const states = {
  "Questioned": "A role skill questioned: should it exist, or fold into another? · K22, K19",
  "Kept": "Kept as a survivor · K22",
  "Folded": "Folded: a survivor carries its responsibilities · K22",
  "Removed": "Removed · K22",
  "WrittenFresh": "Written fresh from the 4.0.0 decisions and claims · K22, K19, K20",
  "Approved": "Approved by him at the final candidate, folds and removals included · K16, K22",
  "GAP 1": "GAP: unknown state",
  "GAP 2": "GAP: unknown state",
  "GAP 3": "GAP: unknown state"
};
const steps = {
  "Evaluation_Approve": "he approves the rewrite with the other claims, at the final candidate · K16, K22",
  "Evaluation_ApproveFold": "he approves the fold with the other claims, at the final candidate · K16, K22",
  "Evaluation_ApproveRemoval": "he approves the removal with the other claims, at the final candidate · K16, K22",
  "Evaluation_Fold": "questioned, it folds into a survivor, which carries its responsibilities · K22",
  "Evaluation_Keep": "questioned, it should exist: a survivor · K22",
  "Evaluation_Remove": "questioned, it should not exist: removed · K22",
  "Evaluation_Rewrite": "only a survivor is written fresh from the 4.0.0 decisions and claims, simpler and streamlined; its current file is a checklist of what not to lose, never a source · K22, K19",
  "Gap1": "GAP: K16 and K22 give his approval of a rewrite at the final candidate, not the route when he rejects it",
  "Gap2": "GAP: K16 and K22 give his approval of a fold at the final candidate, not the route when he rejects it",
  "Gap3": "GAP: K16 and K22 give his approval of a removal at the final candidate, not the route when he rejects it"
};
export const title = "The nine role skills evaluated";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

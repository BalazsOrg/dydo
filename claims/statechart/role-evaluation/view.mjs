const states = {
  "Questioned": "A role skill questioned: should it exist, or fold into another? · K22, K19",
  "Kept": "Kept as a survivor · K22",
  "Folded": "Folded: a survivor carries its responsibilities · K22",
  "Removed": "Removed · K22",
  "WrittenFresh": "Written fresh from the 4.0.0 decisions and claims · K22, K19, K20",
  "Changed": "Rejected at his walkthrough: a change for new captains on new tickets · K22, K16",
  "Approved": "Approved by him at the final candidate, folds and removals included · K16, K22"
};
const steps = {
  "Evaluation_Approve": "he approves the rewrite with the other claims, at the final candidate · K16, K22",
  "Evaluation_ApproveFold": "he approves the fold with the other claims, at the final candidate · K16, K22",
  "Evaluation_ApproveRemoval": "he approves the removal with the other claims, at the final candidate · K16, K22",
  "Evaluation_Fold": "questioned, it folds into a survivor, which carries its responsibilities · K22",
  "Evaluation_Keep": "questioned, it should exist: a survivor · K22",
  "Evaluation_Rebuild": "new captains on new tickets question it again with his verdict, and rebuild · K22, K16",
  "Evaluation_RejectFold": "he rejects the fold: a change for new captains on new tickets · K22, K16",
  "Evaluation_RejectRemoval": "he rejects the removal: a change for new captains on new tickets · K22, K16",
  "Evaluation_RejectRewrite": "he rejects the rewrite: a change for new captains on new tickets · K22, K16",
  "Evaluation_Remove": "questioned, it should not exist: removed · K22",
  "Evaluation_Rewrite": "only a survivor is written fresh from the 4.0.0 decisions and claims, simpler and streamlined; its current file is a checklist of what not to lose, never a source · K22, K19"
};
export const title = "The nine role skills evaluated";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

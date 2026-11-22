const states = {
  "Invoked": "dydo merge run on a PR · R23",
  "Refused": "Refused with a stated reason · R19, R23, R26, R27",
  "Conflict": "CONFLICT: the merge with the tip is not clean · R21, R23",
  "Merged": "A merge commit on the target · R32, N12 DR 061 §3",
  "AlreadyIn": "Already integrated: exit 0 · N12 DR 061 §2"
};
const steps = {
  "MergeCommand_AlreadyIn": "a closed PR already in its base · N12 DR 061 §2",
  "MergeCommand_Conflict": "the merge with the tip is not clean · R21, R23",
  "MergeCommand_DraftOrFork": "refused: a draft or fork PR, or a closed PR not yet in its base · N12 DR 061 §2",
  "MergeCommand_HandShaped": "refused: a hand-shaped tree, not git merge-tree --write-tree p1 p2 · R19",
  "MergeCommand_Merge": "every check holds: a merge commit; a re-land reverts the revert · R23, R24, R29, R32, N12 DR 061 §3",
  "MergeCommand_NoPass": "refused: no PASS on the head, nor on an ancestor followed only by clean syncs · R17, R23, R24",
  "MergeCommand_NoReceipt": "refused: DYTEST holds no green receipt for the combined tree · R23, Y4",
  "MergeCommand_NotTwoWay": "refused: a PR into master whose body does not declare it two-way, or whose CI is not green · R26",
  "MergeCommand_OneWay": "refused: a PR into master that the dydo.json path list flags one-way · R27",
  "MergeCommand_ReviewOpen": "refused: the Review is not Done · R17",
  "MergeCommand_TipMoved": "refused: the target's tip moved from the pin · N12 DR 061 §2"
};
export const title = "dydo merge and its refusals";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

// built from the model; edit there: tools/SkillBuild
const states = {
  "Live": "Branch or worktree in use, a T3 worktree included · N10",
  "Archived": "Archived for 30 days · N10",
  "Purged": "Purged · N10"
};
const steps = {
  "SafeDelete_Archive": "after its merge, its captain safe-deletes it · N10, N12 DR 045 §8",
  "SafeDelete_ArchiveT3": "a T3 worktree is archived without breaking T3 · N10",
  "SafeDelete_Purge": "30 days pass · N10",
  "Gap1": "GAP: N10 names no restore from the archive; the built command has one"
};
export const title = "Safe delete";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

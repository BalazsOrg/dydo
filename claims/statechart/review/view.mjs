// built from the model; edit there: tools/SkillBuild
const states = {
  "Wired": "Parts wired, before the Merge hop · R1, R2, R6",
  "Round": "Review round · R3, R5, R6, R8, R12, R15, R33",
  "Verdict": "Verdict posted · R11, R17, S8",
  "Fixing": "Fix rN · S9, R8",
  "Checkpoint": "Round-3 checkpoint · R10, S10",
  "Replan": "Sent up to re-plan · R9, S10",
  "Escalated": "Escalated up the chain · R9, R10, S10",
  "Ended": "Review phase ended · R8, R6"
};
const steps = {
  "Review_Broken": "a blocker is broken or against the spec · R9, S9",
  "Review_CheckpointEscalate": "a fundamental break, a new requirement, or a need for Balazs · R10, S10",
  "Review_CheckpointGoOn": "checkpoint: one more round, the security lens first for a security hole · R10, S10, R9",
  "Review_Clean": "no new blocker this round · R8",
  "Review_FixDone": "Fix rN is Done: its blockers Done, proof run on the head · S9, R8",
  "Review_Fundamental": "a blocker is fundamental, or a new requirement · R9, H5",
  "Review_NeedsBalazs": "a blocker needs Balazs · R9",
  "Review_OneMore": "no new blocker, but the officer orders one more round · R8",
  "Review_Open": "the parts are wired; Review r1 opens · R2, R3, R6",
  "Review_Post": "the verdict and the PR line are posted · R11, R17, S8",
  "Review_Security": "a blocker is a security hole · R9",
  "Review_Skip": "the parent only groups its parts · R6",
  "Review_ToCheckpoint": "round 3 ends with a blocker · R10"
};
export const title = "The review loop";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

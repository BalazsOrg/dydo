// built from the model; edit there: tools/SkillBuild
const states = {
  "Integrated": "Delivery merged into the feature branch · N5",
  "Mapped": "A round from zero: a fresh feature map · N5",
  "ReadPhase": "Read lens on a slice · N5, R34",
  "BreakPhase": "Break lens on the slice · N5, R34",
  "DrivePhase": "Drive lens on the slice · N5, R34, R3",
  "Inquisition": "The round's verdict, held by its root inquisition captain · N5, S8",
  "RoundFixes": "Fixing the round's blockers and shoulds · N5, S9, S17",
  "Prerequisites": "Landing prerequisites · N7, S18, T12, T8, N14, N15, Y2, Y6, L6, L12, N8, N9",
  "MergeReview": "Landing merge review: a Merge-confidence council · N6, R20, L16",
  "Replanned": "A fundamental blocker or a new requirement: the officer above re-plans · R9",
  "Escalated": "A blocker that needs Balazs: up the chain at once · R9",
  "Walkthrough": "Balazs walks the landing candidate · N6, N4, N3",
  "NotAccepted": "Not accepted: his findings are tickets in the Project · N6",
  "Merging": "Landing PR into master · N6, R26, R27",
  "Landed": "Landed on master · N6, S14, D11",
  "Tagged": "Release tag pushed by Balazs · R28, N2, N15"
};
const steps = {
  "Landing_Accepted": "Balazs accepts the landing candidate · N6",
  "Landing_AllSlices": "every slice through its lenses: the round's verdict · N5, S8",
  "Landing_BreakDone": "Break done; Drive, a separate sub-agent · N5, R34",
  "Landing_CleanRound": "a round finds no blocker and no should · N5",
  "Landing_Confident": "the Merge-confidence council passes the landing candidate · N6, L16, R20, H22",
  "Landing_Hardened": "the Guard hardening Project lands · N2",
  "Landing_MapDrawn": "a slice's Read lens starts · N5, R34",
  "Landing_Merged": "the landing PR is in master · N6, R26",
  "Landing_NextMergeRound": "the fixes are merged; a full-lens merge-review round, the security lens first after a security hole · R8, R9",
  "Landing_NextRound": "the fixes are merged; the next round from zero, a fresh feature map · N5",
  "Landing_NextSlice": "Drive done; the next slice's Read lens · N5",
  "Landing_NotAccepted": "he does not accept: his findings become tickets in the same Project · N6",
  "Landing_PrerequisiteMet": "a prerequisite met: shoulds closed, mutation clean, purge done, evidence rewritten on his click, DYTEST wired in, principle descriptions written, backfill page passed, law shipped, Upgrade note and re-measure, statuses Enablement · N7, S18, T12, N15, T8, N14, Y2, Y6, L6, L12, N8, N9",
  "Landing_ReCut": "a fix captain re-cuts the branch; another landing follows · N6",
  "Landing_ReadDone": "Read done; Break, a separate sub-agent · N5, R34",
  "Landing_Ready": "every landing prerequisite met · N7, S18, T12, N15",
  "Landing_ReviewFail": "a blocker is broken or against the spec · R9, R11",
  "Landing_ReviewFundamental": "a blocker is fundamental, or a new requirement · R9",
  "Landing_ReviewNeedsHim": "a blocker needs Balazs · R9",
  "Landing_ReviewSecurity": "a blocker is a security hole · R9",
  "Landing_RoundFound": "the round finds a blocker or a should · N5, S8, S9, S11, S17",
  "Landing_Staff": "the Inquisition ticket opens; its root captain staffs a round and draws a fresh feature map · N5, H5, R13, R14",
  "Landing_Tag": "Balazs pushes the release tag · R28, N2, N15"
};
export const title = "The landing";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

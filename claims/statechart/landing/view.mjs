// built from the model; edit there: tools/SkillBuild
const states = {
  "Integrated": "Delivery merged into the feature branch · N5",
  "Mapped": "A round from zero: a fresh area map · N5",
  "ReadPhase": "Read phase on an area · N5",
  "BreakPhase": "Break phase on the area · N5",
  "DrivePhase": "Drive phase on the area · N5, R3",
  "Inquisition": "The round's verdict, held by the coordinator · N5, S8",
  "RoundFixes": "Fixing the round's blockers and shoulds · N5, S9, S17",
  "Prerequisites": "Landing prerequisites · N7, S18, T12, T8, N14, N15, Y2, Y6, L6, L12, N8, N9",
  "MergeReview": "Landing merge review: a Merge-confidence council · N6, R20, L16",
  "Replanned": "A fundamental blocker or a new requirement: the officer above re-plans · R9",
  "Escalated": "A blocker that needs Balazs: up the chain at once · R9",
  "Walkthrough": "Balazs walks the landing candidate · N6, N4, N3",
  "Merging": "Landing PR into master · N6, R26, R27",
  "Landed": "Landed on master · N6, S14, D11",
  "Tagged": "Release tag pushed by Balazs · R28, N2, N15",
  "GAP 1": "GAP: unknown state"
};
const steps = {
  "Landing_Accepted": "Balazs accepts the landing candidate · N6",
  "Landing_AllAreas": "every area through its phases: the round's verdict · N5, S8",
  "Landing_BreakDone": "Break done; Drive, the default next phase · N5",
  "Landing_CleanRound": "a round finds no blocker and no should · N5",
  "Landing_Confident": "the Merge-confidence council passes the landing candidate · N6, L16, R20, H22",
  "Landing_Hardened": "the Guard hardening Project lands · N2",
  "Landing_MapDrawn": "an area's Read phase starts · N5",
  "Landing_Merged": "the landing PR is in master · N6, R26",
  "Landing_NextArea": "Drive done; the next area's Read phase · N5",
  "Landing_NextMergeRound": "the fixes are merged; a full-lens merge-review round, the security lens first after a security hole · R8, R9",
  "Landing_NextRound": "the fixes are merged; the next round from zero, a fresh area map · N5",
  "Landing_PrerequisiteMet": "a prerequisite met: shoulds closed, mutation clean, purge done, evidence rewritten on his click, DYTEST wired in, principle descriptions written, backfill page passed, law shipped, Upgrade note and re-measure, statuses Enablement · N7, S18, T12, N15, T8, N14, Y2, Y6, L6, L12, N8, N9",
  "Landing_ReadDone": "Read done; Break, the default next phase · N5",
  "Landing_Ready": "every landing prerequisite met · N7, S18, T12, N15",
  "Landing_ReviewFail": "a blocker is broken or against the spec · R9, R11",
  "Landing_ReviewFundamental": "a blocker is fundamental, or a new requirement · R9",
  "Landing_ReviewNeedsHim": "a blocker needs Balazs · R9",
  "Landing_ReviewSecurity": "a blocker is a security hole · R9",
  "Landing_RoundFound": "the round finds a blocker or a should · N5, S8, S9, S17",
  "Landing_Staff": "the coordinator staffs a round; a fresh area map · N5, R13, R14",
  "Landing_Tag": "Balazs pushes the release tag · R28, N2, N15",
  "Gap1": "GAP: N6 gives only his acceptance; the route when he does not accept is unstated"
};
export const title = "The landing";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

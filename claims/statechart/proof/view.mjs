// built from the model; edit there: tools/SkillBuild
const states = {
  "Start": "A test enters the suite · T1",
  "Written": "A proof test in the ticket's commits · T5, T4",
  "Red": "Shown failing without the code · T3",
  "Gated": "dydo test gate and dydo code-check on changed files · T2, T13, T21, Y6, T11, T10, T14, Y7",
  "GateRed": "A gate red: a failing test, or a static gate · T23, T19",
  "Receipt": "A green receipt keyed by tree; coverage only reported · Y4, T9, T23",
  "Graduated": "Graduated with its job tag · T5, T1, T7",
  "Dropped": "Dropped · T5, T6, T8",
  "Finding": "A finding: an untraced pin, or a missing tag · T1, R3, T5",
  "Old": "An old test measured against the new requirements · T8, T20, T22",
  "Conformance": "A rewrite's conformance suite · T6"
};
const steps = {
  "Testing_Delete": "deleted, with one line in the commit when it blocks 4.0.0 work · T8, T22",
  "Testing_Drop": "the merge drops a test that does not graduate · T5",
  "Testing_GoRed": "a selected test or a static gate fails · T23, T19",
  "Testing_Graduate": "it graduates with its job tag · T5, T4",
  "Testing_Green": "all selected tests pass and the statics hold · T23, Y4, T9",
  "Testing_Keep": "it meets the new test requirements: kept · T8, T22",
  "Testing_Legacy": "an old test meets the purge · T8, T20",
  "Testing_MatchesOld": "a matches-old-implementation assertion goes at the rewrite's landing · T6",
  "Testing_Refix": "fixed; the gates run again · T23",
  "Testing_Rewrite": "a rewrite gets a conformance assertion · T6",
  "Testing_Rewritten": "rewritten to meet them · T8",
  "Testing_RunGates": "the code goes in; the gates run on changed files · T13, T21, Y6",
  "Testing_ShowRed": "shown failing without the code · T3",
  "Testing_Traced": "a traced assertion is shown failing without the code · T6, T3",
  "Testing_Untraced": "an untraced pin, or a missing job tag · T1, R3, T5",
  "Testing_Write": "a proof test is written for a hop · T5"
};
export const title = "Proof tests and gates";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

// built from the model; edit there: tools/SkillBuild
const states = {
  "Staffing": "Staffing a seat · R12, R13, R14, R15, K14",
  "Briefed": "Briefed · H3, K14",
  "Working": "Working the hop · H1, R5",
  "ReCut": "Too big: re-cut · H2",
  "Returned": "Returned · K14, R5",
  "CouncilCalled": "A council instead of one seat · L16, L17",
  "SelfAuthored": "Authored by the captain · R16"
};
const steps = {
  "Dispatch_Council": "stakes or value: a council · L16, L17",
  "Dispatch_ReCut": "the cutter cuts coherent children · H2, H14",
  "Dispatch_Return": "the crew returns to its dispatcher · K14, R5, S6, N12 DR 047 §7",
  "Dispatch_SelfAuthor": "cheaper to do than to brief, too small to need its own review · R16",
  "Dispatch_Send": "one seat, cross-family where possible, sent from its kind's template · H3, K14, R15, S1",
  "Dispatch_Start": "the holder starts its hop · S6, S13",
  "Dispatch_TooBig": "too big for one agent · H2"
};
export const title = "Crew dispatch and return";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

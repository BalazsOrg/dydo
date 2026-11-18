// built from the model; edit there: tools/SkillBuild
const states = {
  "HardCall": "A hard call · L18, H5",
  "DrSettled": "Settled by a DR · L18",
  "CaseFile": "Clerk holds the case file · L18",
  "Sealed": "Two judges, sealed · L18",
  "Exchange": "One exchange · L18",
  "DecidedOvernight": "Decided overnight · L18, L19, L20",
  "Prepared": "Prepared for him · L18, L20",
  "ProposedDr": "Proposed DR · L21",
  "Scored": "His answer scored · L20",
  "Investigated": "Ruling found wrong, investigated · L19"
};
const steps = {
  "Tribunal_DecideNight": "overnight, the judges agree on a two-way hard call · L18, L19, L20",
  "Tribunal_Exchange": "both positions in; one exchange · L18",
  "Tribunal_File": "hard: a fresh clerk takes the Question · L18",
  "Tribunal_Prepare": "by day, or they disagree, or it is one-way or DR-grade · L18, L20, L21",
  "Tribunal_ProposeDr": "overnight, both agree on a two-way, seriously blocking DR-grade call · L21",
  "Tribunal_Score": "he answers; scored against the prediction · L20",
  "Tribunal_Seat": "two judges from different families take sealed positions · L18, R13, R15",
  "Tribunal_Settled": "a DR clearly settles it · L18",
  "Tribunal_Wrong": "the ruling is found wrong · L19",
  "Gap1": "GAP: L20 scores his daytime answers against the prediction but names no scorer",
  "Gap2": "GAP: L19 has a ruling found wrong investigated but names no investigator"
};
export const title = "The tribunal";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

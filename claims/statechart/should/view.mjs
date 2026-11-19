// built from the model; edit there: tools/SkillBuild
const states = {
  "Found": "A round finds a should · R11, S17",
  "Known": "Already open · S17",
  "Open": "Open should · S17",
  "Done": "Should fixed · S17, S18",
  "Duplicate": "Should marked Duplicate · S17",
  "Canceled": "Should canceled with a reason · S18",
  "Moved": "Should moved out by his call · S18"
};
const steps = {
  "Should_AlreadyOpen": "a duplicate is already open · S17",
  "Should_Cancel": "at the landing: canceled with a reason · S18",
  "Should_Dup": "the batch captain finds it a duplicate · S17",
  "Should_Fixed": "fixed in place, when a lane is free, before a round, or at the landing · S17",
  "Should_Move": "at the landing: moved out by his call · S18",
  "Should_Register": "no duplicate open: registered under the ticket where found · S17"
};
export const title = "A should";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

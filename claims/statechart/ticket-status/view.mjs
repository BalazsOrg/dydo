// built from the model; edit there: tools/SkillBuild
const states = {
  "New": "Not yet on the board · S2",
  "FutureFeature": "FutureFeature · S5",
  "Backlog": "Backlog · S5",
  "Todo": "Todo · S3, S5",
  "InProgress": "In Progress · S5, S6",
  "Done": "Done · S5, S6",
  "Canceled": "Canceled, with its reason · S5, S10, S12",
  "Duplicate": "Duplicate · S5, S17",
  "GAP 2": "GAP: unknown state"
};
const steps = {
  "TicketStatus_CancelDead": "a re-plan cancels a dead hop · S10",
  "TicketStatus_CancelRunning": "a re-plan, or a reverted Merge · S10, S12",
  "TicketStatus_Close": "whoever started it closes it · S6",
  "TicketStatus_Contract": "the coordinator contracts an uncontracted ticket · N12 DR 063 §4",
  "TicketStatus_MarkDuplicate": "a duplicate should · S17",
  "TicketStatus_Open": "opened in Todo, ahead of its turn if a hop · S3",
  "TicketStatus_Start": "whoever starts the step sets it In Progress · S6, S13, S3",
  "TicketStatus_Stopped": "liveness finds it stopped · S24",
  "Gap1": "GAP: S5 lists FutureFeature, but no rule says what enters it",
  "Gap2": "GAP: S5 lists FutureFeature, but no rule says how a ticket leaves it",
  "Gap3": "GAP: S5 lists Backlog, but no rule says when a ticket is filed there"
};
export const title = "A ticket's status";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

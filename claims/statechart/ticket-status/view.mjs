const states = {
  "New": "Not yet on the board · S2",
  "FutureFeature": "FutureFeature · S5",
  "Backlog": "Backlog · S5",
  "Todo": "Todo · S3, S5",
  "InProgress": "In Progress · S5, S6",
  "Done": "Done · S5, S6",
  "Canceled": "Canceled, with its reason · S5, S10, S12",
  "Duplicate": "Duplicate · S5, S17"
};
const steps = {
  "TicketStatus_CancelDead": "a re-plan cancels a dead hop · S10",
  "TicketStatus_CancelRunning": "a re-plan, or a reverted Merge · S10, S12",
  "TicketStatus_Close": "whoever started it closes it · S6",
  "TicketStatus_Contract": "a Wayfinding captain contracts it · N12 DR 063 §4, S5, H5",
  "TicketStatus_Filed": "filed without a contract · S5",
  "TicketStatus_Future": "an unscheduled strategic possibility · S5",
  "TicketStatus_MarkDuplicate": "a duplicate should · S17",
  "TicketStatus_Open": "opened in Todo, ahead of its turn if a hop · S3",
  "TicketStatus_Promote": "he promotes it; it waits in Backlog until contracted · S5",
  "TicketStatus_Start": "whoever starts the step sets it In Progress · S6, S13, S3",
  "TicketStatus_Stopped": "liveness finds it stopped · S24"
};
export const title = "A ticket's status";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

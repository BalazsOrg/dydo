// built from the model; edit there: tools/SkillBuild
const states = {
  "Need": "A need, where it arises · H16, H22",
  "Decided": "Decided where it arises: obvious and reversible · H16, H5",
  "AskedInThread": "Asked in one line, in his thread · H22, H6",
  "Research": "A research Question, held by a fresh captain · H16, H23, L18, L21",
  "Answered": "Answered by its research · H16, L18",
  "Council": "A council child, AFK · H23, L18, L23",
  "Queued": "On his HITL queue, behind the active envoy · H16, H18, H23",
  "Active": "Envoy out · H15, H18, H19, H23",
  "Reported": "Envoy reported back · H15, H19",
  "Closed": "Envoy thread closed · H18, H19"
};
const steps = {
  "Council_Hold": "a hard call not worth his time: a council child, AFK · L23, H23, L18",
  "Envoy_Close": "he closes the envoy's thread · H19, H18",
  "Envoy_Decide": "the call is obvious and reversible: decided where it arises · H16, H5",
  "Envoy_InThread": "he is already in this officer's thread · H22, H6",
  "Envoy_Open": "worth his time: its his-call child joins his HITL queue · H23, H16, H15, H17, S10",
  "Envoy_PassUp": "passed up: its officer opens a research Question and waits on it · H16, H12, H23",
  "Envoy_Report": "the envoy records the result and reports back · H15, H19",
  "Envoy_Send": "the last envoy reported and he closed its thread: the next goes onto the head of the queue · H18, H19, H23",
  "JobTickets_Research": "its research resolves the call · H23, H16, L18, K30"
};
export const title = "A need for Balazs";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

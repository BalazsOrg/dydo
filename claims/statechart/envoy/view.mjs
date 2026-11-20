// built from the model; edit there: tools/SkillBuild
const states = {
  "Need": "A need for Balazs · H16",
  "Homework": "Homework first · H16",
  "PassedUp": "Passed up the chain · H16, H12",
  "Decided": "Decided by the officer · H16, L18",
  "AskedInThread": "Asked in one line, in his thread · H22, H6",
  "ToTribunal": "Sent to the tribunal · H5, L18",
  "Queued": "Queued behind the active envoy · H15, H18",
  "Active": "Envoy out · H15, H19, H23",
  "Reported": "Envoy reported back · H15, H19",
  "Closed": "Envoy thread closed · H18, H19",
  "MovedOn": "He has moved on, the thread left open · H18"
};
const steps = {
  "Envoy_Close": "he closes the envoy's thread · H19, H18",
  "Envoy_Decide": "the homework answers it, or the call is obvious and reversible · H16, L18",
  "Envoy_Hard": "a hard call: competing answers, each good through some lens · H5, L18",
  "Envoy_InThread": "he is already in this officer's thread · H22, H6",
  "Envoy_NextLevel": "the officer above takes it · H12",
  "Envoy_Open": "only he can settle it · H15, H17, S10",
  "Envoy_PassUp": "a captain passes what remains up the chain · H16, H12",
  "Envoy_Report": "the envoy records the result and reports back · H15, H19",
  "Envoy_Send": "the last envoy reported and he closed its thread · H18, H19, H23",
  "Envoy_Take": "the officer above takes the need · H16"
};
export const title = "A need for Balazs";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

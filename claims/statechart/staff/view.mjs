const states = {
  "Day": "By day · H7",
  "Investigating": "Investigating for him · H7",
  "Muster": "Muster · L21, K11",
  "NightWatch": "Overnight watch · H7, H12",
  "MorningBrief": "Morning brief · L19, L21"
};
const steps = {
  "Staff_Answered": "findings given to him · H7",
  "Staff_Ask": "he asks for cross-Project thinking, or about an unknown on his machine · H7",
  "Staff_Brief": "the morning brief closes the night · L19, L21",
  "Staff_Morning": "he returns: the night ends · H7, L19",
  "Staff_Muster": "he musters: the night starts · H7, K11",
  "Staff_Watch": "the DR-grade calls listed; the watch starts · L21, H7"
};
export const title = "The chief of staff";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

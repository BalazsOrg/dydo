// built from the model; edit there: tools/SkillBuild
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
  "Staff_Brief": "the brief is given · L19, L21",
  "Staff_Morning": "the night ends · H7, L19, K11",
  "Staff_Muster": "overnight mode starts · K11, L21",
  "Staff_Watch": "DR-grade calls surfaced; the watch starts · L21, H7",
  "Gap1": "GAP: H7 says the chief of staff runs the overnight watch, not what starts it",
  "Gap2": "GAP: H7 and L19 name the watch and the morning brief, not what ends the night"
};
export const title = "The chief of staff";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

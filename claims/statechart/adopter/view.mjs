const states = {
  "Released": "A dydo release ships · K18, N1",
  "Updating": "One ordinary update, with its Upgrade note · K18, N8, L2",
  "Tailored": "A shipped file the project keeps tailored · N11",
  "Consulted": "update-dydo consults the user · N11",
  "Major": "4.0.0 as a major update · N1, N9, Y5, D1",
  "Current": "Up to date; later builds line-stable · K18",
  "Harvested": "Friction and local patches harvested for the next release · H9"
};
const steps = {
  "Adopter_Consult": "update-dydo consults the user · N11",
  "Adopter_Decided": "the user decides what happens to it · N11",
  "Adopter_Done": "updated; later builds stay line-stable, one claim per bullet · K18",
  "Adopter_Harvest": "a release is planned: its map holder harvests their Agent friction queues and local patches · H9",
  "Adopter_Major": "4.0.0: statuses deleted, DYTEST a prerequisite, the rebuilt skills swapped in · N1, N9, Y5",
  "Adopter_Swapped": "dydo's law and glossary read-only, the two-law template, descriptive-doc rules on by default · N8, L2, D9, D1",
  "Adopter_Tailored": "a shipped file the project keeps tailored, docs included · N11",
  "Adopter_Update": "one ordinary update with an Upgrade note, through the update-dydo wizard · K18, N8"
};
export const title = "The adopter update";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

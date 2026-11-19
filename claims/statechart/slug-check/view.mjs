// built from the model; edit there: tools/SkillBuild
const states = {
  "Written": "A new DR: applies: (or new-ground), and one weighed: line per tension it resolved · L8",
  "Valid": "Its slugs valid: dydo check passes · L8",
  "Invalid": "An unknown or malformed slug: dydo check fails · L8"
};
const steps = {
  "Slugs_Corrected": "the slug is corrected; dydo check runs again · L8",
  "Slugs_Invalid": "an unknown or malformed slug: dydo check fails · L8",
  "Slugs_Valid": "every applies: and weighed: slug is valid: dydo check passes · L8"
};
export const title = "A new DR's slugs checked";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

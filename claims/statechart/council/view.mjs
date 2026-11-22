const states = {
  "Convened": "Council convened · L14, L15, L16",
  "Waiting": "Merge confidence waiting for both families · L22",
  "Answered": "Every member answered, sealed · L14",
  "Synthesised": "Synthesised by a non-member · L14"
};
const steps = {
  "Council_Answer": "every member answers alone, sealed · L14, L15, L16",
  "Council_FamilyOut": "Merge confidence with one family out · L22",
  "Council_Proceed": "something blocks: it runs on one family · L22",
  "Council_Synthesise": "a non-member synthesises · L14"
};
export const title = "A council";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

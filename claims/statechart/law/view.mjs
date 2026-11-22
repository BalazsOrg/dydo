const states = {
  "Evidence": "Evidence for the law · L11",
  "Candidate": "Amendment candidate · L7, L11",
  "Mechanical": "A test, lint or gate · L7, L13",
  "Proposed": "Principle proposed to him · L7, L13",
  "Enacted": "Principle enacted · L3, L6, L13",
  "Retired": "Principle retired · L11",
  "CaseStudy": "Case study prepared · L9",
  "CaseLaw": "Case study approved · L9"
};
const steps = {
  "Law_Approve": "he approves it · L9, L4",
  "Law_Departs": "his answer departs from his precedent or his law, or other evidence arrives · L7, L11",
  "Law_Enact": "he enacts it under a new slug · L13, L1, L2, L3, L5, L6, L10",
  "Law_Mechanical": "no reasonable person would choose otherwise · L7, L13",
  "Law_Precedential": "a call looks precedential · L9, L8",
  "Law_Propose": "a reasonable person could choose otherwise and still be right · L7, L13",
  "Law_Retire": "he retires or supersedes it · L11, L13"
};
export const title = "How the law grows";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

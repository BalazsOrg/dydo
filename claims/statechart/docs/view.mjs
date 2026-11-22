const states = {
  "Start": "A descriptive doc · D1, D2",
  "Proposed": "A new descriptive doc proposed · D4",
  "NotWritten": "Not written · D4, D3, D8",
  "Invited": "Invited · D4",
  "Admitted": "Admitted: solid, important, expensive to rebuild from code · D3",
  "Declared": "Its explained code declared in explains: · D5, D2",
  "Flagged": "A change to its paths flagged · D5",
  "Finding": "A review finding on it · D6, D2",
  "Grandfathered": "An existing doc, grandfathered · D7",
  "CutDoc": "Cut · D7"
};
const steps = {
  "Docs_Admit": "solid, important and expensive to rebuild from code · D3",
  "Docs_CutDrifted": "found drifted or with an unlinked reason: cut · D7",
  "Docs_Declare": "written, declaring the code it explains in explains: · D5, D2",
  "Docs_Existing": "an existing descriptive doc is grandfathered · D7",
  "Docs_FixDrifted": "found drifted or with an unlinked reason: fixed · D7",
  "Docs_FixFinding": "the finding is fixed · D6",
  "Docs_Flag": "a change to its paths: dydo check and pr-ready flag it · D5",
  "Docs_GivePaths": "one mechanical ticket gives it its declared paths · D7",
  "Docs_Hold": "the writer updates the doc or states it holds · D5",
  "Docs_Invite": "he invites it · D4",
  "Docs_New": "a new descriptive doc is proposed · D4, D1",
  "Docs_Refuse": "it fails one of the three tests · D3",
  "Docs_ReviewFinds": "review finds an unlinked why, or a declared path changed without the doc checked · D6, D2",
  "Docs_Uninvited": "no invitation, or an architecture plan: not written · D4, D8"
};
export const title = "Descriptive docs";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

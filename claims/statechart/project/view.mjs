// built from the model; edit there: tools/SkillBuild
const states = {
  "Unheld": "No live coordinator: not started, idle or closed · H4",
  "Charting": "Charting the spec and the map · S14, S15, S16, N12 DR 060 §1",
  "Watching": "Watching the survey, the board and liveness · H5, H10, H11, R25",
  "Replanning": "Re-planning after a fundamental blocker or a new requirement · R9, S10",
  "Landing": "All delivery merged: the landing runs · N5"
};
const steps = {
  "Project_Charted": "the spec exists and the first tickets are contracted · S14, S15",
  "Project_Replan": "a blocker is fundamental, or a new requirement · R9",
  "Project_Replanned": "dead hops Canceled with a reason · S10",
  "Project_StartLanding": "every delivery ticket is merged into the feature branch · N5",
  "Project_Take": "the Project opens; it has a live coordinator · H4",
  "Gap1": "GAP: H4 names idle Projects but never defines when a Project goes idle",
  "Gap2": "GAP: H4 says who holds a live Project, not who starts or wakes its coordinator"
};
export const title = "A Project and its coordinator";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

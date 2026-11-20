// built from the model; edit there: tools/SkillBuild
const states = {
  "Unheld": "No live coordinator: not started, idle or closed · H4",
  "Charting": "Charting the spec and the map · S14, S15, S16, N12 DR 060 §1, H5",
  "Watching": "Watching the survey, the board and liveness · H5, H10, H11, R25",
  "Replanning": "Re-planning after a fundamental blocker or a new requirement · R9, S10",
  "Landing": "All delivery merged: the landing runs · N5"
};
const steps = {
  "Coordination_Idle": "Paused in Linear, or no coordinator or agent running in it: idle · H4",
  "Coordination_Wake": "he or the chief of staff starts or wakes its coordinator · H4, H7",
  "Project_Charted": "the Project is up: the spec exists and the first tickets are contracted · S14, S15, H4",
  "Project_Replan": "a blocker is fundamental, or a new requirement · R9, H5",
  "Project_Replanned": "dead hops Canceled with a reason · S10",
  "Project_StartLanding": "every delivery ticket is merged into the feature branch · N5",
  "Project_Take": "he opens the Project and charts it, HITL · H5, S14"
};
export const title = "A Project and its coordinator";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

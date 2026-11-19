// built from the model; edit there: tools/SkillBuild
const states = {
  "Live": "Hop In Progress, recently changed · S24",
  "Quiet": "A liveness finding · S24, S23",
  "Restarted": "Hop back in Todo, restarting · S24"
};
const steps = {
  "Liveness_FoundAlive": "alive: the finding snoozes one window · S24, H12",
  "Liveness_FoundBehind": "the record is behind: brought up to date · S24, H12",
  "Liveness_FoundStopped": "stopped: the hop back to Todo · S24, H12",
  "Liveness_Quiet": "2 hours with no change and no liveness comment · S24, S23",
  "Liveness_Resumed": "the resumed or fresh agent attaches its thread link · S24, S13"
};
export const title = "Liveness";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

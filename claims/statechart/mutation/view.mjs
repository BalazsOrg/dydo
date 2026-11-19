// built from the model; edit there: tools/SkillBuild
const states = {
  "Background": "Background campaigns in spare capacity · T12",
  "Queued": "Landing work queued for the exact tree · T12",
  "Running": "Running: incomplete, the landing blocked · T12, N15",
  "Survivors": "A mutant survived · T12",
  "Ticketed": "A survivor to fix or claim · T12, H5, N6",
  "Claimed": "Claimed equivalent · T12",
  "Dispute": "Claim disputed: a council · T12, L14",
  "Clean": "No survivor without a fix or an approved equivalent · T12, N15"
};
const steps = {
  "Mutation_AllKilled": "no survivor without a fix or an approved equivalent · T12, N15",
  "Mutation_Approve": "a fresh independent reviewer approves it · T12",
  "Mutation_ClaimEquivalent": "claimed equivalent, with a reason · T12",
  "Mutation_Cloud": "a large queue spills to the cloud · T12",
  "Mutation_CutFix": "the admiral cuts the survivor's fix ticket · N6, H5, T12",
  "Mutation_Fix": "fixed; the changed tree's work is queued again · T12",
  "Mutation_LandingCheck": "the landing check judges the Project's diff · T12, T13",
  "Mutation_Lapse": "the mutant or its code changes: the approval lapses · T12",
  "Mutation_Reject": "rejected, and the author disputes it: a council · T12, L16",
  "Mutation_Rejected": "rejected, and the author accepts: back to fixing · T12",
  "Mutation_Ruled": "the council rules it equivalent · T12, L14",
  "Mutation_RuledNot": "the council rules it a real survivor · T12, L14",
  "Mutation_Schedule": "scheduled like a test run, at normal priority · T12",
  "Mutation_Survive": "a mutant survives · T12"
};
export const title = "The landing's mutation check";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

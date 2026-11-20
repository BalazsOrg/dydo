// built from the model; edit there: tools/SkillBuild
const states = {
  "Taken": "Taken · S6, S15",
  "Leaf": "Working it as a leaf · S2, S7",
  "Split": "Split into sub-tickets · S1, S15",
  "Waiting": "Waiting on children, a Question's captain among them · H3, H16",
  "InReview": "In review · R2, R8",
  "Fixing": "Fixing · S9, R8",
  "Checkpoint": "At the checkpoint after a third blocked round · R10, S10",
  "Repairing": "Repairing a conflict or a red gate · R19, R21, R22",
  "Question": "A leaf's own work waiting on a Question · H16",
  "Finishing": "Finishing · R21, S6",
  "Done": "Done · S6",
  "Returned": "Returned to its officer · H23"
};
const steps = {
  "Captain_Filler": "a filler ticket fails its check and goes back · H11",
  "CaptainDispatch_Send": "each sub-ticket is dispatched · H3",
  "CaptainFinish_Close": "the contract is met: the ticket closes Done · S6",
  "CaptainFinish_HandOver": "the leaf is whole and has no review of its own: its officer reviews it, or it is too small to need one, or it is a verdict or an answer · R32, R5",
  "CaptainFinish_Return": "one line goes back to the officer · H23, K14",
  "CaptainMergeTrouble_Hunks": "the repair is in: a fresh review of what it changed · R19",
  "CaptainMerging_Broken": "a conflict or a real red gate: an Implement ticket repairs it · R21, R19",
  "CaptainReview_Clean": "a round finds no new blocker · R8",
  "CaptainReview_Fix": "a round other than the third returns a blocker: a Fix ticket · R9, S9",
  "CaptainReview_Groups": "the children are back and the ticket only groups parts: no review of its own · R6",
  "CaptainReview_OneMore": "no new blocker, and one more round is ordered · R8",
  "CaptainReview_Other": "the leaf's change is whole: another agent reviews it · R33, R12",
  "CaptainReview_Replan": "in a round other than the third, a blocker is fundamental or a new requirement: back to the officer · R9",
  "CaptainReview_Whole": "the parts are wired: a Review of the whole is dispatched · R2, R1",
  "CaptainSplit_Cut": "the ticket is bigger than one agent: it is cut · H13, H14",
  "CaptainSplit_Leaf": "the ticket fits one agent · S2, S7",
  "CaptainTrouble_Answered": "the answer is on the Question ticket · H16",
  "CaptainTrouble_Ask": "a call that is not obvious: a Question sub-ticket · H16, S10",
  "CaptainTrouble_Checkpoint": "`checkpoint: one more round, because …`: the blockers go to a Fix · R10, S10",
  "CaptainTrouble_Escalate": "`escalated: …`: a fundamental break, a new requirement or a need for the user goes to the officer · R10",
  "CaptainTrouble_NextRound": "the Fix is Done: a fresh round · R8, S9",
  "CaptainTrouble_ReCut": "`too big, re-cut`: neither the work nor its split fits · H2",
  "CaptainTrouble_SplitRest": "the leaf proves too big: what is left is cut · H13, H14",
  "CaptainTrouble_Third": "a third round still has a blocker: the checkpoint · R10"
};
export const title = "A captain and its ticket";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

// built from the model; edit there: tools/SkillBuild
const states = {
  "Taken": "Taken · S6, S15",
  "Leaf": "Working it as a leaf · S2, S7",
  "Split": "Split into sub-tickets · S1, S15",
  "Waiting": "Waiting on children · H3",
  "InReview": "In review · R2, R8",
  "Fixing": "Fixing · S9, R8",
  "Question": "Waiting on a Question · H16",
  "Finishing": "Finishing · R21, S6",
  "Done": "Done · S6",
  "Returned": "Returned to its officer · H23"
};
const steps = {
  "Captain_Filler": "a filler ticket fails its check and goes back · H11",
  "CaptainDispatch_Send": "each sub-ticket is dispatched · H3",
  "CaptainFinish_Close": "the contract is met: the ticket closes Done · S6",
  "CaptainFinish_HandOver": "the work is whole, and its review and merge are the officer's · R32, R5",
  "CaptainFinish_Return": "one line goes back to the officer · H23, K14",
  "CaptainMerging_Broken": "a conflict or a real red gate: an Implement ticket · R21, R19",
  "CaptainReview_Clean": "a round finds no new blocker · R8",
  "CaptainReview_Fix": "a round returns a blocker: a Fix ticket · R9, S9",
  "CaptainReview_Groups": "the ticket only groups parts: no review · R6",
  "CaptainReview_Other": "the leaf's change is whole: another agent reviews it · R33, R12",
  "CaptainReview_Replan": "a blocker is fundamental or a new requirement: back to the officer · R9",
  "CaptainReview_Whole": "the parts are wired: a Review of the whole is dispatched · R2, R1",
  "CaptainSplit_Cut": "the ticket is bigger than one agent: it is cut · H13, H14",
  "CaptainSplit_Leaf": "the ticket fits one agent · S2, S7",
  "CaptainTrouble_Answered": "the answer is on the Question ticket · H16",
  "CaptainTrouble_Ask": "a call that is not obvious: a Question sub-ticket · H16, S10",
  "CaptainTrouble_NextRound": "the Fix is Done: a fresh round · R8, S9",
  "CaptainTrouble_ReCut": "`too big, re-cut`: neither the work nor its split fits · H2",
  "CaptainTrouble_SplitRest": "the leaf proves too big: what is left is cut · H13, H14"
};
export const title = "A captain and its ticket";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

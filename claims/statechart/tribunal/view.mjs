const states = {
  "HardCall": "A Question's call, after its research · L18, H16, H23",
  "DrSettled": "Settled by a DR · L18",
  "CaseFile": "A council child: its chair holds the case file · L18, L23, H23",
  "Sealed": "Two judges, sealed · L18",
  "Exchange": "One exchange · L18",
  "DecidedOvernight": "Decided, provisionally · L18, L19, L20",
  "Prepared": "Prepared for him: a his-call child · L18, L20, H23",
  "ProposedDr": "Proposed DR awaiting his ratification · L21",
  "Ratified": "Proposed DR ratified · L21",
  "Declined": "Proposed DR declined: deleted, his words on the raising ticket · L21",
  "Scored": "His answer scored · L20",
  "Investigated": "Ruling found wrong, investigated · L19"
};
const steps = {
  "Tribunal_DecideNight": "the judges agree on a two-way hard call · L18, L19, L20",
  "Tribunal_Decline": "he declines it: deleted, his words on the raising ticket · L21",
  "Tribunal_Exchange": "both positions in; one exchange · L18",
  "Tribunal_File": "a hard call not worth his time: a council child, by day or night · L18, L20, H23",
  "Tribunal_HisCall": "worth his time to understand: a his-call child · L18, H23, H16",
  "Tribunal_OneFamily": "one family out: the judges sit on the remaining family · L22, L18, L20",
  "Tribunal_Prepare": "they disagree, or it is one-way or DR-grade · L18, L20, L21, H23",
  "Tribunal_ProposeDr": "overnight, both agree on a two-way, seriously blocking DR-grade call · L21",
  "Tribunal_Ratify": "he ratifies it · L21",
  "Tribunal_Score": "he answers; any council prediction is scored, as evidence only · L20",
  "Tribunal_Seat": "two judges from different families take sealed positions · L18, R13, R15",
  "Tribunal_Settled": "a DR clearly settles it · L18",
  "Tribunal_Wrong": "the ruling is found wrong · L19"
};
export const title = "The tribunal";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

// built from the model; edit there: tools/SkillBuild
const states = {
  "ClaimArea": "The claim build and the role evaluation · K1, K4, K6, K7, K16, K22, K24",
  "HostArea": "Host settings, the guard and the statechart tool · N12 DR 055 §1, N12 DR 056 §1, N12 DR 062 §1, H20",
  "ProjectArea": "A Project and its admiral · H4, H5, H10, S14, S15, R25, N13",
  "TicketArea": "A ticket: dispatch, status and hops · H1, H2, H3, K14, S3, S5, S6, S12",
  "ProofArea": "Proof tests and gates · T1, T3, T5, T13, T21, T23, Y4",
  "DocsArea": "Descriptive docs · D1, D2, D3, D4, D5, D6, D7",
  "ReviewArea": "The review loop and its shoulds · R2, R3, R8, R9, R10, R11, S8, S17",
  "MergeArea": "dydo merge, the Merge hop and safe delete · R18, R19, R20, R21, R22, R23, N10",
  "WatchArea": "Survey, map, liveness and CI findings · S19, S22, S23, S24, S26, Y9, R30",
  "JudgementArea": "Envoys, councils, the tribunal and the law · H15, H16, L7, L14, L18, L22",
  "StaffArea": "Chief of staff, overnight, and DRs awaiting ratification · H7, L19, L21",
  "LandingArea": "The landing: inquisition, prerequisites and mutation · N5, N6, N7, T12, N15",
  "MasterArea": "The master route · R26, R27, R29, R31",
  "Tagged": "4.0.0 tagged · R28, N2, N15",
  "AdopterArea": "The adopter update · K18, N1, N8, N11"
};
const steps = {
  "Overview_Accept": "his acceptance: the landing PR goes to master · N6",
  "Overview_Build": "the model builds the skills; a project's init turns memory off and commits the guard · K6, N12 DR 055 §1, N12 DR 056 §1",
  "Overview_Cut": "the admiral cuts a ticket and dispatches its captain · H5, H3",
  "Overview_Decided": "a Question decided, by him by day or provisionally overnight, is acted on and never ratified · L19, L20",
  "Overview_Explain": "a change to declared paths flags its doc · D5",
  "Overview_Explained": "the writer updates the doc or states it holds · D5",
  "Overview_GiveOut": "he gives out a ticket from that list, or tells the chief of staff to · H7",
  "Overview_Hold": "an admiral holds a live Project · H4",
  "Overview_Land": "every delivery ticket merged: the landing runs · N5",
  "Overview_List": "the chief of staff reads the board across Projects into his short list · H7",
  "Overview_Merge": "the first round with no new blocker: the Merge hop runs dydo merge · R8, R17, R23",
  "Overview_Need": "a need only he can settle goes up the chain, or a hard call to the tribunal · H16, L18",
  "Overview_Next": "merged: the admiral cuts the next ticket · R20, H5",
  "Overview_Proposed": "overnight, both judges agree on a two-way, seriously blocking DR-grade call: a proposed DR · L21",
  "Overview_Prove": "the Implement hop proves its work with red tests and receipts · R5, T3",
  "Overview_Ratify": "he ratifies the proposed DR · L21",
  "Overview_Refix": "a landing round's blockers become Fix rN · N5, S9",
  "Overview_Review": "the parts are wired; the review opens · R2",
  "Overview_Route": "findings and CI reds go to their officer · Y9, H11, H5",
  "Overview_Ship": "adopters take one ordinary update · K18, N8",
  "Overview_Tag": "Balazs pushes the release tag · R28, N2",
  "Overview_Watch": "the board, the map and liveness are read · S19, S22, S24"
};
export const title = "dydo 4.0.0";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

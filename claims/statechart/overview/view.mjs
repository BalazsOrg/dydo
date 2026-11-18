// built from the model; edit there: tools/SkillBuild
const states = {
  "ClaimArea": "The claim build · K1, K4, K6, K7, K10, K16, K24",
  "ProjectArea": "Project and its admiral · H4, H5, H10, S14, S15, R25, N13",
  "DispatchArea": "Crew dispatch and return · H1, H2, H3, H20, K14, R5, R13, R14, R16",
  "StatusArea": "Ticket statuses · S5, S6, S10, S13",
  "HopArea": "Hops by Type label · S3, S4, S9, S12",
  "ProofArea": "Proof tests and gates · T1, T3, T5, T6, T13, T21, T23, Y4",
  "DocsArea": "Descriptive docs · D1, D2, D3, D4, D5, D6, D7",
  "ReviewArea": "The review loop · R2, R3, R8, R9, R10, R11, S8",
  "ShouldArea": "Shoulds · S17, S18",
  "MergeArea": "dydo merge and the feature-branch merge · R18, R19, R20, R21, R22, R23, R24",
  "FindingsArea": "Survey, board and CI findings · S19, S22, S23, Y9, H11, R30",
  "LivenessArea": "Liveness · S24, H12",
  "EnvoyArea": "Envoys · H15, H16, H18, H19, H22, H23",
  "LawArea": "Law, councils and tribunal · L7, L11, L14, L16, L18, L22, L23",
  "StaffArea": "Chief of staff and overnight · H7, L19, L20, L21",
  "MutationArea": "The landing and its mutation check · N5, N6, N7, T12, N15",
  "MasterArea": "The master route · R26, R27, R29, R31",
  "SafeDeleteArea": "Safe delete · N10",
  "Tagged": "4.0.0 tagged · R28, N2, N15",
  "AdopterArea": "The adopter update · K18, N1, N8, N11"
};
const steps = {
  "Overview_Accept": "his acceptance: the landing PR goes to master · N6",
  "Overview_Archive": "merged: branches and worktrees archived · N10",
  "Overview_Build": "the model builds the skills the Project runs on · K1, K6",
  "Overview_Cut": "the admiral cuts a ticket and dispatches its captain · H5, H3",
  "Overview_Explain": "a change to declared paths flags its doc · D5",
  "Overview_Feed": "findings feed the admiral's next cut · H5, S22",
  "Overview_Findings": "a real red after the merge becomes a CI finding · Y9",
  "Overview_Hard": "a hard call goes to the tribunal · H5, L18",
  "Overview_Hops": "a step with its own review loop gets its hops · S3, S4",
  "Overview_Land": "every delivery ticket merged: the landing runs · N5, T12",
  "Overview_Merge": "PASS on the head: the Merge hop runs dydo merge · R17, R23",
  "Overview_Need": "a need only Balazs can settle goes up the chain · H16",
  "Overview_Night": "overnight rulings wait for the morning brief · L19, L21",
  "Overview_Prove": "the Implement hop proves with receipts · R5, T3",
  "Overview_Quiet": "the survey finds a quiet hop · S23, S24",
  "Overview_Review": "the parts are wired; the review opens · R2",
  "Overview_Ship": "adopters take one ordinary update · K18, N8",
  "Overview_Shoulds": "a round finds a should · S17",
  "Overview_Start": "the holder sets its step In Progress · S6, S13",
  "Overview_Tag": "Balazs pushes the release tag · R28, N2"
};
export const title = "dydo 4.0.0";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

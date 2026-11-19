// built from the model; edit there: tools/SkillBuild
const states = {
  "Watch": "Findings arrive · S22, S26, Y9, Y8",
  "Board": "dydo survey board read · S22, S25, S26, H7",
  "SharedLink": "One thread link on two non-nested open tickets · S23, S13",
  "LandedSpec": "A landed Project that still has its spec · S23, S14",
  "StaleShoulds": "Open shoulds under Done parents past the threshold · S23, T14",
  "CiRed": "A real CI red: a CI-findings ticket · Y9, H8",
  "Routed": "Routed to its officer · H11, H8",
  "Flake": "A flake or runner noise · Y9, R22",
  "Counted": "A DYTEST result with a count · Y9",
  "Repeated": "A flake that repeats · Y9",
  "Weed": "A mistake found after landing · R30",
  "DytestNeed": "Something dydo needs from DYTEST · Y3, Y1",
  "Filed": "Filed in a DYTEST Project · Y3",
  "Acted": "Its owner acts · H5"
};
const steps = {
  "Findings_BatchShoulds": "the admiral cuts a shoulds batch · S17",
  "Findings_Count": "it stays a DYTEST result with a count · Y9",
  "Findings_DeleteSpec": "the map holder moves its lasting parts and deletes it · S14, D11",
  "Findings_FileIt": "filed and built in a DYTEST Project, never patched from dydo · Y3, Y1",
  "Findings_FixLink": "its officer fixes the record: one agent holds one ticket · S7, S13, H5",
  "Findings_LandedSpec": "a landed Project still has a spec document · S23",
  "Findings_Need": "dydo needs something from DYTEST · Y3",
  "Findings_Noise": "a flake or runner noise: a timeout, red then green on rerun · Y9",
  "Findings_Pull": "its officer takes it, or an officer with an open lane pulls it · H8, H10, H11",
  "Findings_ReadBoard": "dydo survey board, read-only and on demand · S22, S26",
  "Findings_RealRed": "a real CI red becomes a CI-findings ticket · Y9, R22",
  "Findings_Repeat": "it repeats · Y9",
  "Findings_Route": "to the open ticket's officer, or the admiral for a long-closed ticket · H11",
  "Findings_Rule": "it gets a lint, test or rule as well as its fix · R30",
  "Findings_SharedLink": "one thread link on two non-nested open tickets · S23",
  "Findings_StaleShoulds": "open shoulds under Done parents past the threshold, 7 days by default · S23, T14",
  "Findings_Ticket": "a repeating flake becomes a CI-findings ticket · Y9",
  "Findings_Weed": "a mistake is found after landing · R30"
};
export const title = "Survey, board and CI findings";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

// built from the model; edit there: tools/SkillBuild
const states = {
  "Invoked": "dydo statechart invoked · N12 DR 062 §1",
  "InvalidOverride": "An explicit invalid override: an error · N12 DR 062 §1",
  "Ready": "Ready: verified compatible dependencies, offline, no consent asked again · N12 DR 062 §1",
  "ApprovalRequired": "Approval required: a locked plan, the saved request, exit 3 · N12 DR 062 §2",
  "Continuing": "Authorized continuation: identities verified, locked assets installed · N12 DR 062 §3, N12 DR 062 §4",
  "NonSuccess": "Non-success: denied, unavailable, failed or canceled, the request kept · N12 DR 062 §5",
  "Checking": "TLC checking under native statechart ownership · N12 DR 062 §6",
  "Charted": "Charted; drain authoritative, registered scratch cleaned · N12 DR 062 §6",
  "ScratchKept": "Drain uncertain: scratch kept with a durable reason · N12 DR 062 §6"
};
const steps = {
  "ChartTool_Authorize": "the installed skill submits that exact continuation through the host's normal authorization, and the host grants it · N12 DR 062 §3",
  "ChartTool_Check": "the check starts under native statechart ownership, with registered scratch and startup gates · N12 DR 062 §6",
  "ChartTool_Deny": "denied, authorization unavailable or canceled: non-success, the request kept; a local flag, token or receipt is not host approval · N12 DR 062 §3, N12 DR 062 §5",
  "ChartTool_Drained": "an authoritative drain: registered-owned scratch is cleaned · N12 DR 062 §6",
  "ChartTool_InstallFails": "the installation fails: non-success, the request kept, and no checker starts on an unverified plan · N12 DR 062 §5",
  "ChartTool_Installed": "identities verified, the locked assets installed into owned roots, readiness proved and the receipt published atomically; the saved inputs continue · N12 DR 062 §4",
  "ChartTool_Invalid": "an explicit invalid override is an error · N12 DR 062 §1",
  "ChartTool_Missing": "a dependency is missing: a precise locked plan, a structured event, the saved request and exit 3, proposing dydo statechart resume --request · N12 DR 062 §2",
  "ChartTool_Ready": "its dependencies verified compatible, it runs offline with no repeated installation or consent · N12 DR 062 §1",
  "ChartTool_Uncertain": "the drain is uncertain, or only notified: scratch kept with a durable reason · N12 DR 062 §6"
};
export const title = "A dydo statechart run";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

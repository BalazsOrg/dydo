const states = {
  "OverLimit": "A changed file over the token limit · T15, T16",
  "Split": "Split under the limit · T15",
  "PathA": "Ignore path (a): a file nobody writes by hand · T17",
  "OwnTicket": "Ignore path (b): its own ticket · T18",
  "CouncilB": "Path (b) before a council · T18",
  "Ignored": "In the ignore file · T17, T18",
  "Stale": "Stale entry: the gate fails · T17"
};
const steps = {
  "IgnoreEntry_Authored": "authored code that must stay whole · T18, T19",
  "IgnoreEntry_Bless": "he blesses it · T18",
  "IgnoreEntry_CheckA": "the reviewer checks the entry · T17",
  "IgnoreEntry_Convene": "the ticket goes to a council · T18",
  "IgnoreEntry_Generated": "nobody writes it by hand · T17, T14",
  "IgnoreEntry_GoStale": "the entry goes stale · T17",
  "IgnoreEntry_Split": "the writer splits the file · T15, T16"
};
export const title = "A file over the token limit";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

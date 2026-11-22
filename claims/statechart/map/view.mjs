const states = {
  "Closed": "Map not open · S19, H6",
  "Snapshot": "A revisit: the last visit's disposable, machine-local snapshot shown first · S26",
  "Fresh": "A fresh read: top-level tickets, open plus closed in the last 3 days faded; the view in the URL · S19, S20, S21, S26",
  "Shown": "The fresh read stands, its snapshot saved or not; Linear stays authoritative · S26",
  "Expanded": "A card expanded: the same layout at any depth · S19"
};
const steps = {
  "MapView_Animate": "the fresh read arrives and its changes animate · S26",
  "MapView_Expand": "he expands a card: the same layout at any depth · S19",
  "MapView_Open": "he opens a Project's map: a fresh read from Linear · S19, S21, S26, H6",
  "MapView_Revisit": "he opens a map he has opened before: last visit's snapshot first · S26",
  "MapView_Save": "the snapshot is written outside any repository · S26",
  "MapView_SaveFails": "the cache write fails; the fresh read still stands · S26"
};
export const title = "dydo map";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

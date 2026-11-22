// Written by hand. A second view of Captain.tla, for reading the comprehensive model whole: every
// model state is its own chart state, and every arrow is named by its action, the name Captain.tla
// gives the rule it rests on. Nothing is checked against it; check-chart.mjs checks the simple view.
export const title = "A captain and its ticket: the comprehensive model";
export const state = (v) => v.state;
export const step = (action) => (action === "Ended" ? null : action);

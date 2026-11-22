const states = {
  "Untagged": "Accepted DR, untagged · L12",
  "Drafted": "Tags drafted · L12",
  "Checked": "Citations checked · L12",
  "OnPage": "On his review page · L12",
  "Merged": "Row approved and merged · L12",
  "Rejected": "Row rejected · L12"
};
const steps = {
  "Backfill_Approve": "he marks the row approved · L12",
  "Backfill_Check": "a reviewer checks every citation · L12",
  "Backfill_Draft": "cheap-tier crew drafts the tags · L12",
  "Backfill_Page": "batched onto one review page · L12",
  "Backfill_Reject": "he rejects the row · L12",
  "Backfill_Retag": "he marks the row retag · L12"
};
export const title = "The DR backfill";
export const state = (v) => states[v.state];
export const step = (action) => (action === "Done" ? null : steps[action]);

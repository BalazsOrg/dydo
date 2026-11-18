---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Backfill

## Draft

`claim:Backfill.Draft`: After he approves the sorted principles, cheap-tier crew drafts applies: and quoted weighed: tags.

Spec: L12

Transition in The DR backfill (`claims/statechart/backfill/Backfill.tla`): Untagged → Drafted, "cheap-tier crew drafts the tags". A system transition: no skill carries it.

## Check

`claim:Backfill.Check`: A reviewer checks every citation as a doc claim.

Spec: L12

Transition in The DR backfill (`claims/statechart/backfill/Backfill.tla`): Drafted → Checked, "a reviewer checks every citation". A system transition: no skill carries it.

## Page

`claim:Backfill.Page`: He reviews one page per batch, doubted and new-ground rows first.

Spec: L12

Transition in The DR backfill (`claims/statechart/backfill/Backfill.tla`): Checked → OnPage, "batched onto one review page". A system transition: no skill carries it.

## Approve

`claim:Backfill.Approve`: Only rows he approves merge.

Spec: L12

Transition in The DR backfill (`claims/statechart/backfill/Backfill.tla`): OnPage → Merged, "he marks the row approved". A system transition: no skill carries it.

## Retag

`claim:Backfill.Retag`: A row he marks retag is drafted again.

Spec: L12

Transition in The DR backfill (`claims/statechart/backfill/Backfill.tla`): OnPage → Drafted, "he marks the row retag". A system transition: no skill carries it.

## Reject

`claim:Backfill.Reject`: A row he rejects does not merge.

Spec: L12

Transition in The DR backfill (`claims/statechart/backfill/Backfill.tla`): OnPage → Rejected, "he rejects the row". A system transition: no skill carries it.

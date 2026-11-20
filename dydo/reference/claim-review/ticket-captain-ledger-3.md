---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Ledger: ticket-captain, 2. Cut the hops

What became of each line of the legacy `ticket-captain` spine in this section, and of each resource it links.

`claim:Project.OneTicket`, kept in `claim:CaptainSplit.OneTicket`

> One agent holds one ticket; wrap several steps for one agent in one ticket.

`claim:Project.Contract`, kept in `claim:CaptainSplit.Contract`

> Write each sub-ticket's contract in its description: its expected outcome, quoting and linking the spec sections it derives from.

`claim:Linear.CutRule`, kept in `claim:CaptainSplit.Cut`, `claim:CaptainSplit.OffRecord`

> Cut a ticket only when someone other than its asker uses what the work leaves; a native helper, a scout, a lookup, one seat among several, a command or a file stays off the record.

`claim:Hops.Single`, kept in `claim:CaptainSplit.Steps`

> A leaf lane or a batched should stays one ticket, with no hops.

`claim:Hops.Chain`, kept in `claim:CaptainSplit.Steps`

> A step with its own review loop gets hop sub-tickets typed Implement, Review (one per round), Fix and Merge, blocked in step order; you may open them ahead in Todo.

`claim:Hops.AddCheck`, kept in `claim:CaptainBriefs.LeafCheck`

> For a leaf's optional evidence check, add a fresh seat as a Review hop beside it, never a full review.

`claim:Hops.AfkType`, kept in `claim:CaptainSplit.Mode`

> Type the Merge hop AFK Merge for a feature-branch merge or a two-way main merge.

`claim:Hops.HitlType`, deferred: `dydo merge` refuses the default branch as the user's click, so the route into it is not rebuilt in DYD-1372. The Mode rule itself is claim CaptainSplit.Mode.

> Type the Merge hop HITL Merge only for a one-way main merge.

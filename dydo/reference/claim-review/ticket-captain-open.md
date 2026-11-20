---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Open claims: ticket-captain

Claims of `ticket-captain` that nobody has verified yet. The skill does not state them: each line below is withheld until its claim has evidence.

## Captain.Thread

`claim:Captain.Thread`: Whoever sets a ticket In Progress attaches the holder's T3 thread link.

Open: Spec S13 leaves it to verify whether Linear takes a T3 thread link as an attachment, and nobody has: DYD-1372, DYD-1281 and DYD-1339, read 2026-11-20, carry none. T3's `link_issue` already shows a ticket beside its thread.

Withheld from Take:

> Attach your thread's link to the ticket, titled `T3 · ticket-captain · <kind> · <model>`.

## CaptainDispatch.Roster

`claim:CaptainDispatch.Roster`: Each seat's model comes from the user's roster.

Open: The roster is the user's to write (R13), and it is not written: there is no text to state yet.

Withheld from Seats:

> Pick each seat's model from the roster below.

## CaptainDispatch.Usage

`claim:CaptainDispatch.Usage`: A dispatcher balances vendors by their remaining usage for every seat it fills.

Open: `dydo usage` is no command of the CLI: `Commands/DydoRootCommand.cs` registers none, and R14 calls the name provisional.

Withheld from Seats:

> Read `dydo usage` and balance vendors across every seat you fill.

## CaptainMerging.Mutation

`claim:CaptainMerging.Mutation`: No ticket or merge waits on mutation testing.

Open: `dydo/guides/testing-strategy.md` still lists `gate mutation --since BASE` among the project's gates, and DYTEST Mutation, which T12 moves it to, is not built.

Withheld from MergeRules:

> Nothing waits on mutation testing.

---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Open claims: ticket-captain

Claims of `ticket-captain` that nobody has verified yet. The skill does not state them: each line below is withheld until its claim has evidence, but for the one placeholder, whose line stands in the skill with its token unfilled. The skill is hand-written, in `skills/roles/officers/ticket-captain/`: the lines below are the model's own, and the build writes none of them.

## Captain.Thread

`claim:Captain.Thread`: Whoever sets a ticket In Progress attaches the holder's T3 thread link.

Open: Spec S13 leaves it to verify whether Linear takes a T3 thread link as an attachment, and nobody has: DYD-1372, DYD-1281 and DYD-1339, read 2026-11-20, carry none. T3's `link_issue` already shows a ticket beside its thread.

Withheld from Take:

> Attach your thread's link to the ticket, titled `T3 · ticket-captain · <kind> · <model>`.

## CaptainDispatch.Roster

`claim:CaptainDispatch.Roster`: Each seat's model comes from the user's roster.

Placeholder: The roster is the user's to write (R13), and it is not written: there is no text to state yet. He asked for the token where it will stand (DYD-1372 07c86aad): "at least put a {roster_placeholder} there", and "I will write it. (not now). Just use a placeholder for now."

Stated in Seats, with `{roster_placeholder}` where its text will stand:

> Once the user's roster stands below, pick each seat's model from it.
>
> {roster_placeholder}

## CaptainDispatch.Usage

`claim:CaptainDispatch.Usage`: A dispatcher balances vendors by their remaining usage for every seat it fills.

Open: `dydo usage` is no command of the CLI: `Commands/DydoRootCommand.cs` registers none, and R14 calls the name provisional.

Withheld from Seats:

> Read `dydo usage` and balance vendors across every seat you fill.

## CaptainMerging.Receipt

`claim:CaptainMerging.Receipt`: A merge is done only once DYTEST also holds a green receipt on the merge tree.

Open: DYTEST is not built: no command of the CLI is DYTEST (`Commands/DydoRootCommand.cs` registers none), so nothing issues a receipt to wait for.

Withheld from MergeSteps:

> Before you close it, wait for DYTEST's green receipt on the merge tree as well.

## CaptainMerging.Archive

`claim:CaptainMerging.Archive`: After a merge the captain archives the branches and worktrees its dispatches created.

Open: Dry runs with the built CLI, 2026-11-20, on this ticket's own T3-made worktree and its branch: `dydo archive worktree` refused, "T3 ownership/activity unavailable … target retained", and `dydo archive branch` refused, "Branch is checked out elsewhere". Every worktree a dispatch creates is T3's (`Services/SafeDeleteT3Policy.cs`; spec CA19, DYD-1216).

Withheld from MergeSteps:

> Archive each branch and worktree your dispatches created, with `dydo archive branch <target>` and `dydo archive worktree <target>`; the archive keeps them 30 days.

## CaptainMerging.Mutation

`claim:CaptainMerging.Mutation`: No ticket or merge waits on mutation testing.

Open: `dydo/guides/testing-strategy.md` still lists `gate mutation --since BASE` among the project's gates, and DYTEST Mutation, which T12 moves it to, is not built.

Withheld from MergeRules:

> Nothing waits on mutation testing.

## CaptainImplement.LimitNumber

`claim:CaptainImplement.LimitNumber`: The file size limit is 5k tokens, counted as bytes ÷ 4, unless the project's configuration sets another.

Open: The number is this repository's alone as the system runs: only `tools/SkillBuild/changed-file-sizes.sh` and this suite check 20,000 bytes. T14 makes the number configuration, and no key of `dydo.json`, no command of the CLI and no file of the scaffold holds it, so nothing tells another project's holder its limit.

Withheld from ImplementRules:

> Unless your project's configuration says otherwise, the limit is 5k tokens, counted as bytes ÷ 4.

## CaptainImplementCode.Graduation

`claim:CaptainImplementCode.Graduation`: Proof tests live in the ticket's commits with their SHA in the receipt, and nothing depends on old evidence.

Open: T5 has the merge drop the proof tests that do not graduate, and no merge in this repository has been seen to: `dydo merge` (`Services/Merge`) reads no test's tag. Until a run shows which tests a merge keeps, what a writer does with a proof test is not known.

Withheld from ImplementTests:

> Keep proof tests in the ticket's commits, record their SHA in your receipt, and make nothing depend on old evidence.

## CaptainImplementDocs.DrFields

`claim:CaptainImplementDocs.DrFields`: A new decision record carries applies or new-ground, and one weighed line for each tension it resolved.

Open: No check reads either field: `Services`, `Commands` and `Rules` hold no `weighed` or `new-ground`, and neither copy of `dydo/reference/writing-docs.md` names them. Whether a decision record must carry them as the system runs is not known.

Withheld from ImplementDocs:

> A new decision record you write carries `applies:` (or `new-ground`) and one `weighed: <slug> over <slug>, because <reason>` for each tension it resolved.

## CaptainReviewRead.IgnoreEntry

`claim:CaptainReviewRead.IgnoreEntry`: A reader checks each new ignore entry: an exact path or a generated-directory glob, for a file nobody writes by hand, with a reason.

Open: No ignore file for the file size limit exists in the running system: this repository's only size check, `tools/SkillBuild/changed-file-sizes.sh`, reads none, and no command of the CLI checks a file's size. Until the gate and its ignore file ship (T16), a reviewer has no entry to check.

Withheld from ReviewRead:

> Check each new ignore entry: an exact path or a generated-directory glob, for a file nobody writes by hand, with a reason.

## CaptainReviewBreak.Equivalent

`claim:CaptainReviewBreak.Equivalent`: A reviewer that wrote neither the code nor its tests approves an equivalent mutant one at a time, with a reason; a claim its author disputes goes to a council.

Open: No mutation mechanism runs on any stack of this repository (`dydo/guides/testing-strategy.md`: "Mutation is unavailable on every stack"), so no surviving mutant and no equivalence claim exists to approve, and the council that would take a dispute has no resource in this skill.

Withheld from ReviewBreak:

> Asked to judge an equivalent mutant: approve one at a time, with a reason, and only if you wrote neither the code nor its tests; a rejection the author disputes goes to a council.

## CaptainMergeTrouble.Dytest

`claim:CaptainMergeTrouble.Dytest`: A gate red after the merge is rerun through DYTEST, which counts a flake as a result.

Open: DYTEST is not built: no command of the CLI is DYTEST (`Commands/DydoRootCommand.cs` registers none), so the rerun is hosted CI's, started by hand.

Withheld from RedLead:

> Run the rerun through DYTEST, so that a flake is counted.

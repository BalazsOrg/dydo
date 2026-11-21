---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainReviewRead

## Local

`claim:CaptainReviewRead.Local`: A local read takes each changed file, and each function in it, on its own: does it work, and is anything plainly wrong.

Spec: R34, R6

Evidence (code): `skills/roles/crew/reviewer/resources/toolset.md`

Layer: A compiler and a test run say that code builds and passes, and nothing about whether a careful reader would let it stand.

Reason: Most faults sit inside one function and show themselves to anyone who reads it slowly.

**ticket-captain** (ReviewRead; read by TicketCaptain):

> Local: read each changed file, and each function in it, on its own. Does it work, and is anything plainly wrong, such as a security hole, a bad practice or waste?

## VsSpec

`claim:CaptainReviewRead.VsSpec`: A read against the spec holds the change to the governing spec and to the code around it.

Spec: R34, R3

Evidence (code): `skills/roles/crew/reviewer/resources/toolset.md`

Layer: A diff shows what changed and never the sentence of the spec it was meant to serve.

Reason: Drift starts as one reasonable guess, and later parts are built on top of it.

**ticket-captain** (ReviewRead; read by TicketCaptain):

> Against the spec: read the change beside the governing spec and the code around it, for anything built on an assumption the spec does not make.

## Whole

`claim:CaptainReviewRead.Whole`: A read of the whole takes the folders and the structure together.

Spec: R34

Evidence (code): `skills/roles/crew/reviewer/resources/toolset.md`

Layer: Every tool that reads code reads it a file at a time.

Reason: Two sound parts can still duplicate each other, or pull a design in opposite directions.

**ticket-captain** (ReviewRead; read by TicketCaptain):

> The whole: read the folders and the structure together, for what no single file shows.

## Tags

`claim:CaptainReviewRead.Tags`: A reader checks each suite test's job tag against what the test proves.

Spec: T5

Evidence (run): This repository's suite, read 2026-11-21: each test carries its job, as `[Trait("Job", "contract: …")]` or `trap: …`. `dydo/guides/coding-standards.md`, here and in the scaffold, says every suite test carries a trap, traced contract or edge job tag.

Layer: A lint can see that a tag is there and cannot see whether the test does the job the tag claims.

Reason: A tag is a claim about why the test exists, and an unearned one keeps a useless test alive.

**ticket-captain** (ReviewRead; read by TicketCaptain, on claim:CaptainReviewRead.Tests):

> Read the tests as part of the change. One that pins a rule no spec or decision record traces, changes whenever the code does, checks that a deleted feature is gone, or can never fail is a finding; so is a job tag its test does not earn.

## Tests

`claim:CaptainReviewRead.Tests`: A test that pins an untraced rule, changes whenever the code does, checks that a deleted feature is gone, or can never fail is a finding.

Spec: T1, T22, R3

Evidence (code): `skills/roles/crew/reviewer/resources/toolset.md`

Layer: A green run counts a test as proof whatever it asserts.

Reason: A test that asserts the wrong reality cements it, and every later change is made to agree with the mistake.

**ticket-captain** (ReviewRead; read by TicketCaptain):

> Read the tests as part of the change. One that pins a rule no spec or decision record traces, changes whenever the code does, checks that a deleted feature is gone, or can never fail is a finding; so is a job tag its test does not earn.

## Docs

`claim:CaptainReviewRead.Docs`: On docs an unlinked reason is a finding, and on code a change to paths a doc's explains declares means reading that doc.

Spec: D6, D2, D5

Evidence (code): `Services/ExplainsCheck.cs`

Layer: `dydo check` warns that a declared path changed, and cannot tell whether the doc still says what the code does.

Reason: A doc left behind by its code misleads the next agent with more authority than a stale comment.

**ticket-captain** (ReviewRead; read by TicketCaptain):

> On docs, a reason that links no decision record or principle is a finding; on code, a change to paths that a doc's `explains:` declares means reading that doc too.

## Quote

`claim:CaptainReviewRead.Quote`: A reading finding quotes both passages by file and line; an unused member is its definition with a reference search that finds no caller outside the tests.

Spec: R5, R11

Evidence (code): `skills/roles/crew/reviewer/resources/toolset.md`

Layer: A reader's finding has no failing test or output behind it, only the words it points at.

Reason: The Fix's writer checks each severity at the source, and cannot check a paraphrase.

**ticket-captain** (ReviewRead; read by TicketCaptain):

> Write a finding so it can be checked without you: both passages quoted at `file:line`, or, for an unused member, its definition with a reference search that finds no caller outside the tests.

## IgnoreEntry

`claim:CaptainReviewRead.IgnoreEntry`: A reader checks each new ignore entry: an exact path or a generated-directory glob, for a file nobody writes by hand, with a reason.

Spec: T17

Evidence: open, so no skill states this claim yet. No ignore file for the file size limit exists in the running system: this repository's only size check, `tools/SkillBuild/changed-file-sizes.sh`, reads none, and no command of the CLI checks a file's size. Until the gate and its ignore file ship (T16), a reviewer has no entry to check.

Layer: An entry in an ignore file silences a gate for good, and the gate cannot ask whether the file is really generated.

Reason: A hand-written file listed as generated escapes the limit without anyone having decided that it may.

**ticket-captain** (withheld; ReviewRead; read by TicketCaptain):

> Check each new ignore entry: an exact path or a generated-directory glob, for a file nobody writes by hand, with a reason.

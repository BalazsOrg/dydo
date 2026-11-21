---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainImplementDocs

A line marked unwritten is the model's own, of a skill written by hand in `skills/roles/officers/ticket-captain/`: the build writes it nowhere, and the hand-written skill need not hold it.

## Witness

`claim:CaptainImplementDocs.Witness`: Every sentence of a doc is a claim that needs a witness in the repository: code, configuration or governing intent.

Spec: D2, K32

Evidence (run): This skill is written so: each of its lines carries what its writer checked, and a line nothing could check is withheld and listed (`dydo/reference/claim-review/ticket-captain-open.md`).

Layer: `dydo check` reads a doc's frontmatter, names and links, and never whether a sentence in it is true.

Reason: A plausible sentence with nothing behind it is believed by every agent that reads it afterwards.

**ticket-captain** (unwritten; ImplementDocs; read by TicketCaptain):

> Every sentence you write is a claim that needs a witness in the repository: code, configuration, or governing intent such as the spec, a decision record or a ticket. Where there is none, return the gap instead of the sentence.

## BuiltFile

`claim:CaptainImplementDocs.BuiltFile`: A built file changes only in its source.

Spec: K29

Evidence (code): `tools/SkillBuild/BuiltHeader.cs`

Layer: A built file opens in an editor like any other, and its next build silently undoes the edit.

Reason: The header names the source, so a change made there is still standing after the next run.

**ticket-captain** (unwritten; ImplementDocs; read by TicketCaptain, on claim:CaptainImplementDocs.OneHome):

> Write each meaning in one home and link to it from everywhere else; change a built file only in the source its header names.

## OneHome

`claim:CaptainImplementDocs.OneHome`: Each meaning has one home, and everything else links to it.

Spec: K29

Evidence (code): `skills/roles/crew/implementer/resources/docs.md`

Layer: Nothing compares two docs for saying the same thing in different words.

Reason: Two copies drift apart, and a reader cannot tell which of them still holds.

**ticket-captain** (unwritten; ImplementDocs; read by TicketCaptain):

> Write each meaning in one home and link to it from everywhere else; change a built file only in the source its header names.

## Agents

`claim:CaptainImplementDocs.Agents`: A skill, its resources or an entry file is written with writing-for-agents.

Spec: K14, K31

Evidence (code): `skills/productivity/writing-for-agents/SKILL.md`

Layer: A brief names the skills its dispatcher foresaw, and a writer may meet a skill file only in the middle of its change.

Reason: A text an agent acts on fails in ways ordinary prose never does, and that skill names them.

**ticket-captain** (unwritten; ImplementDocs; read by TicketCaptain):

> Write a skill, its resources or an entry file with /writing-for-agents.

## Invited

`claim:CaptainImplementDocs.Invited`: A new descriptive doc is written only when invited, and only if its subject is solid, important and expensive to rebuild from code.

Spec: D3, D4

Evidence (code): `skills/roles/crew/implementer/resources/docs.md`

Layer: Nothing stops an agent from adding a doc beside the code it has just written.

Reason: Each uninvited doc is one more text to keep true, about code that will move.

**ticket-captain** (unwritten; ImplementDocs; read by TicketCaptain, on claim:CaptainImplementDocs.Descriptive):

> A descriptive doc explains and never authorises: on what the code does the code wins, and a reason counts only when it links a decision record or a principle. Write a new one only when invited, and only if its subject is solid, important and expensive to rebuild from code.

## Descriptive

`claim:CaptainImplementDocs.Descriptive`: A descriptive doc explains and never authorises: on what the code does the code wins, and a reason counts only when it links a decision record or a principle.

Spec: D2

Evidence (code): `skills/roles/crew/implementer/resources/docs.md`

Layer: A doc and the code it describes can disagree, and nothing in either says which one to trust.

Reason: A doc treated as authority outlives the code it described and steers work toward what is gone.

**ticket-captain** (unwritten; ImplementDocs; read by TicketCaptain):

> A descriptive doc explains and never authorises: on what the code does the code wins, and a reason counts only when it links a decision record or a principle. Write a new one only when invited, and only if its subject is solid, important and expensive to rebuild from code.

## Explains

`claim:CaptainImplementDocs.Explains`: When a change touches the paths a doc's explains declares, its writer updates the doc or states that it holds.

Spec: D5

Evidence (code): `Services/ExplainsCheck.cs`

Layer: The check names the doc and the paths that changed; whether the doc is still right is beyond it.

Reason: The moment the code moves is the one moment somebody knows what the doc should now say.

**ticket-captain** (unwritten; ImplementDocs; read by TicketCaptain):

> When `dydo check` flags a doc whose `explains:` paths your change touches, update the doc or state in your return that it still holds.

## Format

`claim:CaptainImplementDocs.Format`: A project's doc conventions, frontmatter and naming among them, are in its writing-docs reference.

Spec: K29, K31

Evidence (run): `dydo/reference/writing-docs.md` exists here and under `Scaffold/`, the scaffold dydo ships; both open on Frontmatter and hold Naming Conventions.

Layer: `dydo check` rejects a doc that breaks a convention and teaches none of them beforehand.

Reason: A writer that learns the shape from the reference passes the check on its first run.

**ticket-captain** (unwritten; ImplementDocs; read by TicketCaptain):

> Your project's doc conventions, frontmatter and naming among them, are in `dydo/reference/writing-docs.md`.

## Prove

`claim:CaptainImplementDocs.Prove`: A docs change is proved with dydo check, and dydo fix is run after a document is added, moved or renamed.

Spec: K31, D5

Evidence (command): `dydo check`

Layer: The CLI runs both on request and neither by itself after an edit.

Reason: A moved doc leaves dead links behind in files its writer never opened.

**ticket-captain** (unwritten; ImplementDocs; read by TicketCaptain):

> Prove a docs change with `dydo check` and the checks your brief names, and run `dydo fix` after you add, move or rename a document.

## DrFields

`claim:CaptainImplementDocs.DrFields`: A new decision record carries applies or new-ground, and one weighed line for each tension it resolved.

Spec: L1

Evidence: open, so no skill states this claim yet. No check reads either field: `Services`, `Commands` and `Rules` hold no `weighed` or `new-ground`, and neither copy of `dydo/reference/writing-docs.md` names them. Whether a decision record must carry them as the system runs is not known.

Layer: `dydo check` validates a decision record's status and date, and nothing about how it ties to the law.

Reason: A decision that names what it weighed can be found again when the same tension returns.

**ticket-captain** (unwritten; withheld; ImplementDocs; read by TicketCaptain):

> A new decision record you write carries `applies:` (or `new-ground`) and one `weighed: <slug> over <slug>, because <reason>` for each tension it resolved.

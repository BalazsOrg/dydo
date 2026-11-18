---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Findings

## ReadBoard

`claim:Findings.ReadBoard`: A finding: dydo survey board, read-only and on demand.

Spec: S22, S26

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): Watch → Board, "dydo survey board, read-only and on demand". A system transition: no skill carries it.

## OpenMap

`claim:Findings.OpenMap`: A finding: he opens dydo map on the top-level tickets.

Spec: S19, S20, S21, H6

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): Watch → Map, "he opens dydo map on the top-level tickets". A system transition: no skill carries it.

## SharedLink

`claim:Findings.SharedLink`: A finding: one thread link on two non-nested open tickets.

Spec: S23

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): Board → SharedLink, "one thread link on two non-nested open tickets". A system transition: no skill carries it.

## LandedSpec

`claim:Findings.LandedSpec`: A finding: a landed Project still has a spec document.

Spec: S23

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): Board → LandedSpec, "a landed Project still has a spec document". A system transition: no skill carries it.

## StaleShoulds

`claim:Findings.StaleShoulds`: A finding: open shoulds under Done parents past the threshold, 7 days by default.

Spec: S23, T14

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): Board → StaleShoulds, "open shoulds under Done parents past the threshold, 7 days by default". A system transition: no skill carries it.

## FixLink

`claim:Findings.FixLink`: A finding: its officer fixes the record: one agent holds one ticket.

Spec: S7, S13, H5

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): SharedLink → Acted, "its officer fixes the record: one agent holds one ticket". A system transition: no skill carries it.

## DeleteSpec

`claim:Findings.DeleteSpec`: A finding: the map holder moves its lasting parts and deletes it.

Spec: S14, D11

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): LandedSpec → Acted, "the map holder moves its lasting parts and deletes it". A system transition: no skill carries it.

## BatchShoulds

`claim:Findings.BatchShoulds`: A finding: the admiral cuts a shoulds batch.

Spec: S17

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): StaleShoulds → Acted, "the admiral cuts a shoulds batch". A system transition: no skill carries it.

## RealRed

`claim:Findings.RealRed`: A finding: a real CI red becomes a CI-findings ticket.

Spec: Y9, R22

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): Watch → CiRed, "a real CI red becomes a CI-findings ticket". A system transition: no skill carries it.

## Route

`claim:Findings.Route`: A finding: to the open ticket's officer, or the admiral for a long-closed ticket.

Spec: H11

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): CiRed → Routed, "to the open ticket's officer, or the admiral for a long-closed ticket". A system transition: no skill carries it.

## Pull

`claim:Findings.Pull`: A finding: its officer takes it, or an officer with an open lane pulls it.

Spec: H8, H10, H11

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): Routed → Acted, "its officer takes it, or an officer with an open lane pulls it". A system transition: no skill carries it.

## Noise

`claim:Findings.Noise`: A finding: a flake or runner noise: a timeout, red then green on rerun.

Spec: Y9

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): Watch → Flake, "a flake or runner noise: a timeout, red then green on rerun". A system transition: no skill carries it.

## Count

`claim:Findings.Count`: A finding: it stays a DYTEST result with a count.

Spec: Y9

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): Flake → Counted, "it stays a DYTEST result with a count". A system transition: no skill carries it.

## Repeat

`claim:Findings.Repeat`: A finding: it repeats.

Spec: Y9

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): Flake → Repeated, "it repeats". A system transition: no skill carries it.

## Ticket

`claim:Findings.Ticket`: A finding: a repeating flake becomes a CI-findings ticket.

Spec: Y9

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): Repeated → CiRed, "a repeating flake becomes a CI-findings ticket". A system transition: no skill carries it.

## Weed

`claim:Findings.Weed`: A finding: a mistake is found after landing.

Spec: R30

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): Watch → Weed, "a mistake is found after landing". A system transition: no skill carries it.

## Rule

`claim:Findings.Rule`: A finding: it gets a lint, test or rule as well as its fix.

Spec: R30

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): Weed → Acted, "it gets a lint, test or rule as well as its fix". A system transition: no skill carries it.

## Need

`claim:Findings.Need`: A finding: dydo needs something from DYTEST.

Spec: Y3

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): Watch → DytestNeed, "dydo needs something from DYTEST". A system transition: no skill carries it.

## FileIt

`claim:Findings.FileIt`: A finding: filed and built in a DYTEST Project, never patched from dydo.

Spec: Y3, Y1

Transition in [Survey, board and CI findings](../../../claims/statechart/findings/Findings.tla): DytestNeed → Filed, "filed and built in a DYTEST Project, never patched from dydo". A system transition: no skill carries it.

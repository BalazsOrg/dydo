---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Roster

## Models

`claim:Roster.Models`: One model roster, which he writes himself and not as a table, semi-hard-coded in every dispatching officer's spine and updated by him whenever a model ships or changes (DYD-1082 71e30c21).

Spec: R13

**coordinator** (Dispatching):

> The model roster is his to write; his text goes here.

**ticket-captain** (legacy, not built; Dispatching):

> The model roster is his to write; his text goes here.

## Usage

`claim:Roster.Usage`: Every dispatching officer reads dydo usage and balances vendors for every seat.

Spec: R14

**coordinator** (Dispatching, on claim:Coordination.Seats):

> Staff every seat by one fixed rule, never by judgement: from the roster's models in its tier whose vendor still has quota in `dydo usage`, skipping a vendor below 20% unless every vendor is, take the vendor that resets soonest, then more remaining quota, then roster order.

**ticket-captain** (legacy, not built; Dispatching):

> Read `dydo usage` and balance vendors for every seat you fill, sub-captains and crew as well as reviewers.

## Seats

`claim:Roster.Seats`: Seats scale with the breadth and depth of the work.

Spec: R12

**ticket-captain** (legacy, not built; Dispatching):

> Scale each round's seats with the breadth and depth of the work; there is no constant pair.

**working-tree-standard** (TreeDelivery):

> Each review round's seats scale with the breadth and depth of the work, with no constant pair; a small leaf may need no review.

---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Roster

## Models

`claim:Roster.Models`: One model roster, his, as one easy-to-edit table in every dispatching officer's spine.

Spec: R13

**admiral** (Dispatching):

> Pick each seat's model from this roster; it is his to edit, and he fills or approves the strengths, fits and not-for columns.
>
> | Model | Vendor | Tier (B7) | Strengths | Fits | Not for |
> |---|---|---|---|---|---|
> | Sonnet | Anthropic | check: evidence and behaviour checks | | | |
> | Luna | OpenAI | check: evidence and behaviour checks | | | |
> | Opus 5.5 | Anthropic | normal: ordinary reviews at lower and middle levels | | | |
> | GPT 6.2 Sol | OpenAI | normal: ordinary reviews at lower and middle levels | | | |
> | Fable | Anthropic | strong: non-obvious discovery, and the late whole-system "simplify" review | | | |
> | Astra | OpenAI | strong: non-obvious discovery, and the late whole-system "simplify" review | | | |

**ticket-captain** (Dispatching):

> Pick each seat's model from this roster; it is his to edit, and he fills or approves the strengths, fits and not-for columns.
>
> | Model | Vendor | Tier (B7) | Strengths | Fits | Not for |
> |---|---|---|---|---|---|
> | Sonnet | Anthropic | check: evidence and behaviour checks | | | |
> | Luna | OpenAI | check: evidence and behaviour checks | | | |
> | Opus 5.5 | Anthropic | normal: ordinary reviews at lower and middle levels | | | |
> | GPT 6.2 Sol | OpenAI | normal: ordinary reviews at lower and middle levels | | | |
> | Fable | Anthropic | strong: non-obvious discovery, and the late whole-system "simplify" review | | | |
> | Astra | OpenAI | strong: non-obvious discovery, and the late whole-system "simplify" review | | | |

## Usage

`claim:Roster.Usage`: Every dispatching officer reads dydo usage and balances vendors for every seat.

Spec: R14

**admiral** (Dispatching):

> Read `dydo usage` and balance vendors for every seat you fill, captains and crew as well as reviewers.

**ticket-captain** (Dispatching):

> Read `dydo usage` and balance vendors for every seat you fill, sub-captains and crew as well as reviewers.

## Seats

`claim:Roster.Seats`: Seats scale with the breadth and depth of the work.

Spec: R12

**admiral** (Dispatching):

> Scale each round's seats with the breadth and depth of the work; there is no constant pair.

**ticket-captain** (Dispatching):

> Scale each round's seats with the breadth and depth of the work; there is no constant pair.

**working-tree-standard** (TreeDelivery):

> Each review round's seats scale with the breadth and depth of the work, with no constant pair; a small leaf may need no review.

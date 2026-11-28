---
area: general
type: reference
---

# Glossary

Universal terms for this repository. Read this glossary, then only the area glossaries below that
your topic touches. The `domain-modeling` skill holds the format and placement rules.

## HCRAP

Hybrid CRAP, the per-method change-risk score of
[Decision 048](./project/decisions/048-one-level-static-gates-certainly-wrong-no-escape-hatch.md):
`CC² × (1 − line coverage)³ + cognitive complexity`. The coverage term uses cyclomatic complexity,
the floor uses cognitive complexity, so at full coverage the score is the cognitive complexity. The
one threshold is 20.

## Hub File

An authored navigation page, often named `_<folder>.md`, that makes a documentation folder navigable where
the documentation model requires one.

## JITI

Just-In-Time Information: agents navigate to the context needed for the current ticket instead of reading
the entire repository upfront.

## Template Addition

Retired with the compiler by Decision 049. Customization is a direct edit to the skill folder; a project document linked under ## Must-Reads carries shared context.

## Area glossaries

- [dydo Glossary](./reference/dydo-glossary.md) - how work is named: the Linear, knowledge and execution vocabulary; its terms are locked
- [Guard Glossary](./glossary-guard.md) - the guard: which paths agents may read or write, and the rules that warn them

## Related

- [dydo Glossary](./reference/dydo-glossary.md)

<!-- Adapted from mattpocock/skills domain-modeling at 6f7f7831a625f040a8e06a0e7954826b87ce775a (MIT). -->

# Glossary Format

## Structure

An area glossary, `dydo/glossary-billing.md`:

```md
---
area: general
type: reference
---

# Billing Glossary

Read this glossary when the topic touches invoices or payments.

## Customer

A person or organization that places an [Order](./glossary-ordering.md#order).
_Avoid_: Client, buyer, account

## Invoice

A request for payment for an [Order](./glossary-ordering.md#order), sent to its **Customer** after
delivery.
_Avoid_: Bill, payment request

## Related

- [Glossary](./glossary.md) - the root glossary
```

Each term is an alphabetized `##` heading whose anchor is its kebab-case name, with its definition
below and an `_Avoid_:` line for rejected synonyms. Link to a term as
`[Term](./glossary-<area>.md#term-name)`.

## Rules

- **Be opinionated.** When multiple words exist for the same concept, pick the best one and list the others under `_Avoid_`. When one word means two things, sharpen it into two terms.
- **Keep definitions tight.** One or two sentences max. Define what it IS, not what it does.
- **Bold other glossary terms inside a definition.** When a definition mentions another term in the same glossary, write it as `**Customer**`, and let the sentence around it say how the two relate. When the term lives in another glossary, link it instead.
- **Only include terms specific to this project's context.** General programming concepts (timeouts, error types, utility patterns) don't belong even if the project uses them extensively. Before adding a term, ask: is this a concept unique to this context, or a general programming concept? Only the former belongs.
- **Split off an area** when a glossary passes its ceiling of about 15-20 terms, rather than growing it or grouping under subheadings: move a natural cluster into its own area glossary. Terms stay `##` headings so their anchors hold.

## Root and area glossaries

The root `dydo/glossary.md` holds the universal terms. Under `## Area glossaries` it lists every
other glossary, one line each saying what it covers:

```md
## Area glossaries

- [dydo Glossary](./reference/dydo-glossary.md) - how work is named; its terms are locked
- [Billing Glossary](./glossary-billing.md) - invoices and payments
- [Ordering Glossary](./glossary-ordering.md) - receiving and tracking customer orders
```

The root's header says in one sentence to read it, then only the area glossaries the topic touches.

Area glossaries are flat kebab-case peers of the root, `dydo/glossary-<area>.md`: never nested,
never inside code folders.

- A term lives in exactly one glossary.
- A new term goes to its area's glossary. It moves to the root when a second area uses it the same way.
- An area glossary is created with its first resolved term, and its root line is added in the same edit. The root itself is scaffolded, never created lazily.

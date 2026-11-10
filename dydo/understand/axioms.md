---
area: understand
type: context
---

# Axioms

dydo rests on seven axioms. Every rule in the skills and docs follows from at least one of them, and
every change is judged against them.

## The seven

1. **The user can't scale; agents can.** His attention is the scarce resource.
2. **Align intent.** The system is an extension of the user's will: his values, what he thinks is good
   code, and what he thinks is good aesthetically. His intent must be elicited, then reflected everywhere.
3. **An agent's context is finite.** A full context gets dumber or gets compacted, and either way it
   works worse.
4. **Everything evolves.** Agents, tools and platforms keep getting better. The system stays modular and
   light so that something new never strands it.
5. **Efficiency.** The best result for the time and tokens spent.
6. **Agents are set up for success.** The easy path is the right path.
7. **Agents can verify their own work.** Consequence: by the time a PR reaches the user, he has very high
   confidence it is correct, because the problems were caught along the way.

## Rules that follow

| Rule | From |
|---|---|
| Ask the user only what only he can decide, and make it clear whether he is needed. | 1, 2 |
| Bureaucracy is right-sized: too much and things get slow, too little and agents aren't autonomous enough. | 4, 5 |
| A ticket is a unit of work, sized to what one agent can handle within its context. One agent keeps related work when the shared context is worth more than the split. | 3, 5 |
| A role skill carries its own level in full and a summary of the levels below it. Deeper detail is linked from the failure path, and its sources are named so agents never have to guess where to look. | 3, 6 |
| Constrain outcomes, not methods. | 4, 6 |
| Build on what the hosts provide rather than rebuilding it. | 4, 5 |
| Use the right agent for the right task. | 5 |
| Review where work merges, not at every hop. When a round finds no blocker, the officer decides whether another round is needed. | 5, 7 |
| Every claim has one home; everything else points to it. | 3, 5, 6 |
| Each role skill states exactly the tools and surfaces that role touches, and nothing it doesn't need. | 3, 6 |
| Verify in several independent ways: tests and lint (fast and deterministic), driving the running product hands-on, statecharts and formal verification, and review. | 7 |
| No self-review: an agent's own context biases it, and a fresh context catches what it can't. | 7 |

## Facts of the implementation

Linear (live work), Git (durable knowledge), T3 Code (threads and dispatch) and `dydo map` are how
dydo implements the axioms today. They aren't axioms, and they can be replaced (axiom 4).

## Related

- [About This Project](./about.md) - what dydo is and does
- [Understand](./_understand.md) - the folder's hub

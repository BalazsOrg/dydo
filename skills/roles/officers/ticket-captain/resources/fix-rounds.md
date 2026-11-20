<!-- built from the model; edit there: tools/SkillBuild -->

# Fix rounds

## Fix rN

Nominal path:

- Give the Fix to the implementer's thread while it is alive and has room in its context, and to a fresh agent when its context is heavy or the blocker says the approach was wrong; the next round is a fresh look either way.

Exits:

- Start the next round, a fresh full-lens look, once Fix rN's blockers are Done and its proof ran on the head.

## Round-3 checkpoint

Exits:

- By default, write `checkpoint: one more round, because …` on the Review and go on.
- Escalate only for a fundamental break, a new requirement or a need for Balazs, writing `escalated: …` on the Review.

## Save, resume and end

- Save the verdict's head SHA on the Review hop and resume Fix rN from it; end resolved when a round finds no new blocker, or escalated in any round as soon as a blocker is fundamental, a new requirement or needs Balazs, and at round 3 by the checkpoint's `escalated: …` on the Review.

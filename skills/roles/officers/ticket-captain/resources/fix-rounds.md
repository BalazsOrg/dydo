<!-- built from the model; edit there: tools/SkillBuild -->

# Fix rounds

- Give a Fix to its writer's thread, with `t3_thread_send`, while that thread is alive and has room in its context; dispatch a fresh agent for it when the context is heavy or a blocker says the approach was wrong.
- Split a Fix only between writers who work in parallel.
- Dispatch the next round once every blocker of the Fix is Done and its proof ran on the head: a fresh reviewer, every lens, and after a security hole the security lens first.
- When your officer re-plans, cancel each sub-ticket the new plan drops, with a one-line reason.

**The checkpoint.**
After a third round that still has a blocker, write `checkpoint: one more round, because …` on the Review ticket; its blockers then go to a Fix, as in any round.
Write `escalated: …` instead, and take it to your officer, only for a fundamental break, a new requirement or a need for the user.

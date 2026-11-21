<!-- built from the model; edit there: tools/SkillBuild -->

# Fix rounds

- Open one Fix when its round's verdict is posted, unless you opened it ahead, titled after the round, Fix rN, and list on it every blocker it is to close.
- A blocker too big to fix with the rest gets a sub-ticket of its own inside the Fix; no other blocker gets one.
- Give a Fix to the thread of the last child that wrote on the change it repairs, its implementer or an earlier Fix's writer, with `t3_thread_send`, while that thread is alive and has room in its context. Dispatch a fresh writer for it, with `delegate_task`, when that context is heavy, when a blocker says the approach was wrong, or when no child has written on the change because you wrote it: you never fix a change of your own. Either way the Fix is a new ticket to its holder: brief it by [Briefs](briefs.md), under the Fix's own key.
- Split a Fix only between writers who work in parallel, as Fix rNa and Fix rNb.
- If the Fix's writer changed a finding's severity, record the change and its reason on the Review ticket.
- Dispatch the next round once the Fix is Done, each blocker it lists closed and its proof run on the head: a fresh reviewer, every lens, and after a security hole the security lens first.
- When your officer re-plans, cancel each sub-ticket the new plan drops, with a one-line reason.

**The checkpoint.**
After a third round that still has a blocker, write `checkpoint: one more round, because …` on the Review ticket; its blockers then go to a Fix, as in any round.
Write `escalated: …` instead, and take it to your officer, only for a fundamental break, a new requirement or a need for the user.

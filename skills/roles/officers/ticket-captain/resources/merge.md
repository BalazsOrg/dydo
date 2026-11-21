# Review and merge

Your brief says that you merge your change, and into which branch. A merge needs a reviewer's PASS on the exact commit you merge. The verdict is another agent's: you never judge your own work.

**Get it reviewed**

1. Push your branch.
2. Open a Review sub-ticket under your ticket, and dispatch a fresh agent onto it by [Brief](brief.md).
3. Wait for the verdict. It is on the Review ticket.

**The verdict is FAIL**

1. Open one Fix sub-ticket under your ticket, and list every blocker on it.
2. Give the Fix to the agent that wrote the change. When that is you, do the Fix by "A fix after review" in [Implement](implement.md). When it is an agent you dispatched, send it to that agent's thread with `t3_thread_send`, briefed by [Brief](brief.md). Dispatch a fresh agent instead when the writer's context is full, or when a blocker says the approach was wrong.
3. When the Fix is done, go back to "Get it reviewed": the next reviewer is a fresh agent. Put a blocker that the Fix's writer disputes in the new brief, with its evidence.
4. After a third FAIL, write one line on the Review ticket before you go on: `checkpoint: one more round, because …` or `escalated: …`.

A blocker may be one that cannot be fixed inside your ticket: the approach is wrong at the root, it asks for a new requirement, or it needs the user. Then write why on your ticket, and end with `handed back <KEY>: <reason>`.

**The verdict is PASS**

- Each should is already a ticket that waits: the reviewer opened it. Leave it open.
- Rule on each nit in one line on your ticket: it gets a fix later, and you open a ticket for it, or it gets none, and you say why.

**Merge**

1. Run the gates your brief names on your branch merged with the tip of the target branch.
2. Open the pull request into the target branch, with your ticket's key in its title. Write its body with `/pr`: it holds the reviewer's PASS line, word for word.
3. Merge with `dydo merge <pr-number> --match-head-commit <the commit the PASS names> --match-base-commit <the target's tip>`.
4. When hosted CI is green on the merge, set your ticket Done. End with `done <KEY>: merged <the merge commit>`.

If the merge is not clean and green, because of a conflict, a red gate or red CI: open a Fix sub-ticket for it, have it repaired on your branch as in "The verdict is FAIL", and get the repair reviewed.

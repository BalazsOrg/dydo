# Review and merge

Your brief says that you merge your change, and into which branch. A merge needs a reviewer's PASS on the exact commit you merge. The verdict is another agent's: you never judge your own work.

**Get it reviewed**

1. Run the gates your brief names on your branch, then push it.
2. Open a Review sub-ticket under your ticket, and dispatch a fresh agent onto it by [Brief](brief.md).
3. Wait for the verdict. It is on the Review ticket.

**The verdict is FAIL**

1. Open one Fix sub-ticket under your ticket, and list every blocker on it.
2. Give the Fix to the agent that wrote the change:
   - You wrote it: set the Fix In Progress, do it by "A fix after review" in [Implement](implement.md), and set it Done.
   - An agent you dispatched wrote it: send the Fix to that agent's thread with `t3_thread_send`, briefed by [Brief](brief.md).
   - The writer's context is full, or a blocker says the approach was wrong: dispatch a fresh agent instead.
3. When the Fix is done, go back to "Get it reviewed". The next reviewer is a fresh agent.
4. After a third FAIL, write one line on the Review ticket before you go on: `checkpoint: one more round, because …` or `escalated: …`.

**Hand it to your officer**

Write why on your ticket, and end with `handed back <KEY>: <reason>`, in three cases:

- The branch your brief names as the target is the default branch: `dydo merge` refuses it.
- A blocker cannot be fixed inside your ticket: the approach is wrong at the root, it asks for a new requirement, or it needs the user.
- The Fix is handed back to you undone.

**The verdict is PASS**

Each should is already a ticket that waits, and so is each nit the reviewer ruled needs a fix: the reviewer opened them. Leave them open, and merge.

**Merge**

1. Run the gates your brief names on your branch merged with the tip of the target branch.
2. Open the pull request into the target branch, with your ticket's key in its title, and attach its link to your ticket. Write its body with `/pr`: it holds the reviewer's PASS line, word for word.
3. Merge with `dydo merge <pr-number> --match-head-commit <SHA> --match-base-commit <SHA>`. The first is the full 40-character SHA of the commit the PASS names. The second is that of the target's tip.
4. When hosted CI is green on the merge, set your ticket Done. End with `done <KEY>: merged <the merge commit>`.

**The merge is not clean and green**

- A conflict, or a red gate before the merge: open a Fix sub-ticket for it, have it repaired on your branch as in "The verdict is FAIL", and get the repair reviewed.
- Red CI on a merge that is already in: rerun it once. If it is red again, revert the merge on the target branch, then repair it the same way.

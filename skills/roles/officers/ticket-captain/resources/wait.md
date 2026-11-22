# Wait

Your parts are out, each on its sub-ticket.

**When a part comes back**

| It ended with | Do this |
| --- | --- |
| `done`, and its result is a commit | bring that commit into your branch, if it is not there yet |
| `done`, and its result is the findings of a seat of your review | when every seat is back, go to "Give the verdict" in [Review](review.md) |
| `done`, and its result is a verdict on a part | on a FAIL, cut a part that fixes its blockers, and after it a fresh review of the part with its fix, at the same level. For a finding it leaves without a ruling, cut a review part that rules on it |
| `done`, and its result is an answer | if the answer carries a FAIL, do as for a FAIL verdict on a part, and keep what waited on it waiting until the fresh review clears it; otherwise go on with what waited on it |
| `done`, and its result is rulings on findings | if it ended with FAIL, do as for a FAIL verdict on a part, and keep what waited on them waiting until the fresh review clears it; otherwise go on with what waited on them |
| `handed back`, and it is a Question you asked | go by "The Question comes back without its answer" in [Ask](ask.md) |
| `handed back`, and it is any other part | cut it another way, dispatch a fresh agent onto it, or hand your own ticket back |

A conflict between two parts' commits is a part of its own: cut a sub-ticket for it.

**When an agent you dispatched tells you about a problem**

- The problem is inside your ticket: cut a part for it, or send it to the part it belongs to.
- The problem is outside your ticket: pass it to your officer's thread with `t3_thread_send`.
- It is the key of a Question that waits on the user: pass the key to your officer the same way.

**After each part**

Dispatch each part that waited on the one that is back and clear. Then check your contract:

- It is not met, and nothing new can be cut while parts are out: wait.
- It is not met otherwise: cut the next parts by [Cut](cut.md).
- It cannot be met inside your ticket, because your contract or another ticket's must change: write why on your ticket, and end with `handed back <KEY>: <reason>`.
- It is met: finish by "Finish" in the `ticket-captain` skill.

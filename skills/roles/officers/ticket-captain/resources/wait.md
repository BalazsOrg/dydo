# Wait

Your parts are out, each on its sub-ticket. You orchestrate: you write none of the work yourself.

**When a part comes back**

Read its ticket, not only its last line.

| It ended with | Do this |
| --- | --- |
| `done`, and its result is a commit | bring that commit into your branch, if it is not there yet |
| `done`, and its result is a verdict, findings or an answer | use it: it shows what to cut next |
| `handed back`, and it is a Question you asked | go by "The Question comes back without its answer" in [Ask](ask.md) |
| `handed back`, and it is any other part | its work is yours to place again: cut it another way, or hand your own ticket back |

A conflict between two parts' commits is a part of its own: cut a sub-ticket for it.

**When an agent you dispatched tells you about a problem**

- The problem is inside your ticket: cut a part for it, or send it to the part it belongs to.
- The problem is outside your ticket: pass it to your officer's thread with `t3_thread_send`.
- It is the key of a Question that waits on the user: if the spec or the user's recorded words already answer it, send that answer to the agent's thread, with where it is recorded. If they do not, pass the key to your officer. When the answer comes to you, send it to the agent that passed the key up.

**After each part, check your contract**

- It is not met, and you can see the next parts: cut them by [Cut](cut.md).
- It is not met, and parts are still out with nothing new to cut: wait.
- It cannot be met inside your ticket, because your contract or another ticket's must change: write why on your ticket, and end with `handed back <KEY>: <reason>`.
- It is met: finish by "Finish" in the `ticket-captain` skill.

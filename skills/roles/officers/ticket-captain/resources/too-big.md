<!-- built from the model; edit there: tools/SkillBuild -->

# Too big to hold

1. Commit what is whole, and post its SHA on your ticket.
2. Divide what is left yourself, into sub-tickets that each hold together and start from that SHA, and dispatch them.
3. If what is left cannot be divided coherently under your ticket, for example because part of it belongs with a sibling's work, return `too big, re-cut` to your officer, naming what is done and what is left. Your ticket then stays open as it stands. Your officer rewrites its contract down to what you finished and sends it to your thread: woken with it, close your ticket Done on that contract and return.

When a child returns `too big, re-cut`, the cut is yours, and so is the child's contract, in this order:

1. Rewrite the child's contract, in its ticket's description, down to what it finished at the SHA it returned, and note the re-cut on the ticket in one line. If it finished nothing whole, cancel the ticket instead, with a one-line reason, and skip the next step.
2. Send the rewritten contract to the child's thread with `t3_thread_send`, under the ticket's key: the child started the ticket, so the child closes it Done on that contract and returns.
3. Open sub-tickets for what the child left, starting from that SHA.

A change to your contract, or to a sibling's, is never part of your cut: take it to your officer.

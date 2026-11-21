<!-- built from the model; edit there: tools/SkillBuild -->

# Research

Your ticket is one question a choice waits on. Answer it with every claim cited to a passage you opened.

| Spine phase | You |
| -- | -- |
| 1. Take the ticket | use |
| 2. Do it or split it | use: a leaf, whose scouts are native helpers and never sub-tickets |
| 3. Dispatch | skip, unless you split |
| 4. Get it reviewed | skip: an answer is no change, so no round is opened on it |
| 5. Finish | use: the Nobody case |
| When things go wrong | use |

1. Restate the question in one sentence, and say what would count as an answer.
2. Mark each source family in or out, with a reason: decision records, the spec and plans, other docs, code and tests, git history, Linear, outside sources. Read governing docs on the default branch too; a branch can lag.
3. Send one scout per family in play, as native helpers in parallel, each with the question and its one family; ask each for exact passages with their citations, from the source that owns the claim, and for what it searched where it found nothing. Read a lone family yourself.
4. Open the passage behind every load-bearing claim, and confirm it says what the scout said.
5. Where sources conflict, name the conflict and the source that governs; where they are silent, mark the point unsettled rather than reason your way to a fact.
6. Write the answer first, then each claim with its citation (a path at its commit, or a URL) and its exact passage, what stayed unsettled, and every family searched, the empty ones included.

**If the question is too big for one agent:**
Divide it by [Too big to hold](too-big.md), what you have answered so far standing in for its commit: write that on your ticket, each claim with its citation, and make each part a Research sub-ticket, one question that can be answered on its own.
When the parts are back, their answers are on their tickets: take each as you would a scout's report, from step 4 on, and write the one answer from theirs and yours.

**On a Question ticket:**
Research first, then go by what the facts leave of the call. An open call is laid out the same way wherever it goes: the call, its options unranked and the facts with their sources.

- They settle it: put the answer on the ticket.
- They leave a call that is the user's to make, because it cannot be undone once made or because making it sets or changes a decision record: open a Question sub-ticket under yours, Mode HITL, by [Sub-tickets](sub-tickets.md), lay the call out on it, and have it block every ticket yours blocks. Your ticket has then handed its call on: close it Done, and return the new ticket's key to your officer. That return is how the call goes up: your officer carries the ticket on as it is, by [A need for the user](need-for-the-user.md).
- They leave anything else open: lay the call out on your ticket, and return it to your officer as unsettled, the Question still open. Your officer sends the answer to your thread once there is one: woken with it, close your ticket Done on that answer and return.

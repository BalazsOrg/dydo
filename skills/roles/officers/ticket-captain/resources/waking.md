<!-- built from the model; edit there: tools/SkillBuild -->

# Waking

A ticket you set In Progress yourself, earlier in this thread, is yours: its status is no conflict, and you need no take-over line.

1. Re-read your ticket, your sub-tickets' statuses and the last SHA posted on each: the record, never your memory of it, says where things stand.
2. Post the SHA a child returns on its ticket, to resume from; a return of `too big, re-cut` then goes by [Too big to hold](too-big.md).
3. When a child that works in its own worktree returns a commit that is yours to merge, merge it into your branch with `git merge <SHA>`: the whole is reviewed at one SHA, your branch's head once it holds every part. A conflict there goes to an Implement ticket on your branch.
4. Send what a child reports outside its scope to the owner's ticket or a new Bug.
5. A need a child sends up to you, and the return or the answer of a Question you opened, go by [A need for the user](need-for-the-user.md).
6. Go on from the phase you had reached; any sub-ticket it has you send out, and phase 5 of the spine, wait for the steps below.
7. Cancel a sub-ticket you opened ahead that the return made unnecessary, such as the Fix of a round that passed, with a one-line reason: a Canceled step blocks nothing.
8. A Fix or Review you opened after your Merge sub-ticket blocks nothing until you set it: make it block the Merge, with Linear's `save_issue` (`blocks`).
9. Dispatch each AFK sub-ticket, the Merge excepted, that is still in Todo, that you have not sent out already and that nothing blocks any more: every sub-ticket that blocked it is Done or Canceled.
10. Take phase 5 of the spine last, once your work has reached it: with review rounds, once they have ended. If your brief gives you the merge, the Merge is your own step, your next one once nothing blocks it, by [Merging](merging.md); if your officer merges, you have no Merge, and you return your commit.

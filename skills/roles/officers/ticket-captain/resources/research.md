# Research

Your ticket is a Question: a call that another captain may not decide, and that waits on your answer. Deciding the answer is your job, unless the call is the user's.

**Research first**

1. Look where the answer may already be: the user's recorded words, the truth documents your brief names, the statechart, the spec, decision records, other docs, the code and its tests, git history, the tickets. For a wide search, send one native helper to each place, and ask each for exact passages.
2. Open the passage behind every claim your answer rests on, and confirm that it says what you think.
3. Where sources disagree, the truth wins in the order "While you work" in the `ticket-captain` skill gives.

**Then answer**

Take the first case that fits. A call goes to the user only when neither of the first two fits:

1. The truth, as in "While you work" in the `ticket-captain` skill, settles the call: that is the answer, even when the call looks like the user's.
2. The call is another agent's job, such as a reviewer's ruling on a nit or a Wayfinding captain's cut: ask the agent that holds that job, on its thread with `t3_thread_send`. When none holds it, open a sub-ticket of your ticket for it (for a nit, a Review ticket that asks only for its ruling) and dispatch a fresh agent onto it by [Brief](brief.md); if it comes back without its result, dispatch a fresh agent again. Wait for the result, and answer with it, a FAIL and its blockers included.
3. The call cannot be undone, or a record keeps it for the user: decide nothing, and go by "A call for the user".
4. Otherwise decide it yourself, and give your reason.

**Write the answer**

Write on your ticket the answer first. Then give each claim with its source, a path at its commit or a link, and the exact passage. Then say what stayed unsettled.

**A call for the user**

1. Make the user's question ready on your ticket: every option, each option's trade-off, and your recommendation. Give the facts you found with their sources, as in "Write the answer".
2. State each item in plain words: what it does, or what it changes. A decision record's number, a rule ID, a state name, a finding number or a ticket key may follow that, never stand in its place.
3. Leave the ticket open, and end with `handed back <KEY>: a call for the user`.

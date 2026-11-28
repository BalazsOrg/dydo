# Research

Your ticket's result is an answer: a fact, or a call that another captain may not decide. Deciding the answer is your job, unless the call is the user's.

**What you owe when the ticket closes**

On your ticket, in this order:

1. The answer, first.
2. Each claim the answer rests on, with its source, a path at its commit or a link, and the exact passage. You opened every passage yourself.
3. Where sources disagreed: what each says, which one governs, and why.
4. What stayed unsettled, and why.
5. Where you looked: every source you searched, the ones that held nothing included, and each one you left out, with the reason.
6. For a call, the reason you decided it as you did, or the user's question made ready by "A call for the user".

**Take the question**

Restate the question in one sentence, and say what would count as its answer.

**Research first**

1. Look where the answer may already be: the user's recorded words, the truth documents your brief names, the statechart, the spec, decision records, other docs, the code and its tests, git history, the tickets, and outside sources such as official docs, published release notes, standards and library source.
2. When the answer serves work on another branch, read a governing doc there as well as on yours: a branch can lag.
3. A wide search can go to native helpers side by side, one source each, with the helper prompt below.
4. Open the passage behind every claim your answer rests on, and confirm that it says what you think.
5. Take each claim from the source that owns it. A doc or report that describes code, data or another system is a lead to that thing, not its proof.
6. Where sources disagree, the truth wins in the order "While you work" in the `ticket-captain` skill gives. Where they are silent, mark the point unsettled. Never reason your way to a fact.

**Native helpers and captains**

A lookup you hand off goes to a native helper: it holds no ticket, reads only the prompt you write, shares your checkout, and returns its passages to you. Work that needs its own ticket, such as another agent's ruling, goes to a fresh captain, as in "Then answer".

Paste this into each helper's prompt, with its brackets filled:

> Find what one source says about one question. Question: [the question]. Source: [the one source to search].
> - Search that source to its end, and follow its references inside it.
> - Copy each finding as its exact passage, with its citation: a path at its commit, or a link. Take it from the source that owns the claim; a doc's account of the code is a lead to the code.
> - When the source holds nothing, say so, and list what you searched.
> - Return passages and citations only. Draw no conclusion.

**Then answer**

Take the first case that fits. A call goes to the user only when neither of the first two fits:

1. The truth, as in "While you work" in the `ticket-captain` skill, settles the call: that is the answer, even when the call looks like the user's.
2. The call is another agent's job, such as a reviewer's ruling on a nit or a Wayfinding captain's cut: ask the agent that holds that job, on its thread with `t3_thread_send`. When none holds it, open a sub-ticket of your ticket for it (for a nit, a Review ticket that asks only for its ruling) and dispatch a fresh agent onto it by [Brief](brief.md); if it comes back without its result, go on as [Ask](ask.md) says for a Question that comes back without its answer. Wait for the result, and answer with it, a FAIL and its blockers included.
3. The call cannot be undone, or a record keeps it for the user: decide nothing, and go by "A call for the user".
4. Otherwise decide it yourself, and give your reason.

A fact needs only the first case.

Write what you owe on your ticket. When the answer meets the contract, set the ticket Done and end with `done <KEY>: <the answer in a few words>`. When what the contract needs stays unsettled, leave the ticket open and end with `handed back <KEY>: <what stayed unsettled>`.

**A call for the user**

1. Make the user's question ready on your ticket: every option, each option's trade-off, and your recommendation. Give the facts you found with their sources, as in "What you owe when the ticket closes".
2. State each item in plain words: what it does, or what it changes. A decision record's number, a rule ID, a state name, a finding number or a ticket key may follow that, never stand in its place.
3. Leave the ticket open, and end with `handed back <KEY>: a call for the user`.

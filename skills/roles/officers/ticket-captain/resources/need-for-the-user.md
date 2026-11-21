<!-- built from the model; edit there: tools/SkillBuild -->

# A need for the user

You never take a need to the user yourself: it goes up through your officer, and only the coordinator sends an envoy to the user.
Your own need, and one a child sends up to you, take these steps in order.

1. Decide a call yourself when, from where you stand, it is obvious and reversible; it gets no ticket.
2. Only these needs keep the ticket they have, with its blocking relations: a Grilling, a Walkthrough or another HITL sub-ticket you cut, and the sub-ticket a child opened for its Enablement or for a call only the user can make. Open a sub-ticket for any other need, by [Sub-tickets](sub-tickets.md), blocking only what waits on it, and go on with the rest: an Enablement for what only the user can give, a token or an authorisation; a Question for a call.
3. When the need is an AFK Question a child sent up unsettled, your sub-ticket is a Question of your own: put the child's Question's key and what was found on it, for its captain to start from, and have it block the child's. The next step dispatches onto yours, never onto the child's, which its own captain started and left open.
4. Dispatch a fresh captain onto a Question you opened, with `delegate_task`, to research it first; skip this when you already know that only the user can meet the need. A call that only wants the user's approval is never such a need.
5. When the answer is on the Question ticket, resume what waited; an answer to a child's need you put on the child's sub-ticket and send to the child's thread with `t3_thread_send`.
6. A Question of yours whose captain returned it unsettled is still open when its answer comes: send the answer to that captain's thread, under the Question's key, with `t3_thread_send`. That captain started the ticket, so it closes it Done on the answer and returns. If the send does not get through, for another reason than a limit, note the failed delivery on the Question in one line and take its closure to your officer, by the step below.
7. Send what stays open, by its key and with what was found, to your officer's thread, which `t3_thread_list` shows as your own thread's parent, with `t3_thread_send`: at once for a need only the user can meet, and for a Question when it comes back unsettled. Leave the sub-ticket open; what waited still waits on its answer.
8. If that send is refused, because your thread's mode is narrower than your officer's, end your turn on the message instead; your work then stops until your officer sends to your thread again.

- When the user is already in your thread, you may ask one line there instead, with the host's question tool: never what the sources can answer, and never a call that only wants the user's approval.
- Say what a decision record decides whenever you cite one to the user, never only its number.

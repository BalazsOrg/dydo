<!-- built from the model; edit there: tools/SkillBuild -->

# A need for the user

You never take a need to the user yourself: it goes up through your officer, and only the coordinator sends an envoy to the user.
Your own need, and one a child sends up to you, take these steps in order.

1. Decide a call yourself when, from where you stand, it is obvious and reversible; it gets no ticket.
2. A need that already has its ticket keeps it, with its blocking relations: a Grilling, a Walkthrough or another HITL sub-ticket you cut, and the sub-ticket a child opened for its Enablement or for a call only the user can make. Open a sub-ticket for any other need, by [Sub-tickets](sub-tickets.md), blocking only what waits on it, and go on with the rest: an Enablement for what only the user can give, a token or an authorisation; a Question for a call.
3. Dispatch a fresh captain onto a Question with `delegate_task`, to research it first; skip this when you already know that only the user can meet the need. A call that only wants the user's approval is never such a need.
4. When the answer is on the Question ticket, resume what waited; an answer to a child's need you put on the child's sub-ticket and send to the child's thread with `t3_thread_send`.
5. Send what stays open to your officer's thread with `t3_thread_send`, by its key and with what was found: at once for a need only the user can meet, and for a Question when it comes back unsettled. Leave the sub-ticket open; what waited still waits on its answer.
6. If that send is refused, because your thread's mode is narrower than your officer's, end your turn on the message instead; your work then stops until your officer sends to your thread again.

- When the user is already in your thread, you may ask one line there instead, with the host's question tool: never what the sources can answer, and never a call that only wants the user's approval.
- Say what a decision record decides whenever you cite one to the user, never only its number.

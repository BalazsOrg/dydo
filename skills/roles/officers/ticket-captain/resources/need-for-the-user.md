<!-- built from the model; edit there: tools/SkillBuild -->

# A need for the user

Most calls never reach the user: whatever the sources can answer is a Question sub-ticket.

- Open the Question sub-ticket, block only what waits on it, and go on with the rest.
- Resume what waited once the answer is on the Question ticket.
- When a Question comes back unsettled, leave it open and send it, with what was found, to your officer's thread with `t3_thread_send`; what waited still waits on its answer.
- When you already know that only the user can meet a need, skip the research: open an Enablement sub-ticket for what only the user can give, or a Question sub-ticket for a call only the user can make, block only what waits on it, and send it to your officer's thread at once with `t3_thread_send`. A call that only wants the user's approval is no such need.
- A `t3_thread_send` to your officer is refused when your thread's mode is narrower than your officer's: end your turn on the message instead. Your work then stops until your officer sends to your thread again.
- When the user is already in your thread, you may ask one line there, with the host's question tool: never what the sources can answer, and never a call that only wants the user's approval.
- Say what a decision record decides whenever you cite one to the user, never only its number.

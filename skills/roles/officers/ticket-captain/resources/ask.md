# Ask

1. Open the Question as a sub-ticket of your ticket. Write on it the question in one sentence, the options you see, and what waits on the answer.
2. Dispatch a fresh agent onto it by [Brief](brief.md).
3. Wait for the answer. Work that does not depend on it goes on.
4. When the answer is on the Question, go on with the work that waited.

**The Question comes back without its answer**

Its captain handed it back as a call for the user:

1. Leave the Question open. The user's question is ready on it.
2. Send its key to your officer's thread with `t3_thread_send`.
3. Keep waiting. The wait ends when the answer is on the Question.

It came back for any other reason, such as an agent that could not take the ticket: dispatch a fresh agent onto it, and keep waiting.

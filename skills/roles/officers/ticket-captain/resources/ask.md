# Ask

You have a call that is not yours to decide. It becomes a Question ticket, and a fresh agent answers it.

1. Open the Question as a sub-ticket of your ticket. Write on it the question in one sentence, the options you see, and what waits on the answer.
2. Dispatch a fresh agent onto it by [Brief](brief.md).
3. Wait for the answer. Work that does not depend on it goes on.
4. When the answer is on the Question, go on with the work that waited.

**The Question comes back without its answer**

- Its captain handed it back as a call for the user. The Question stays open, with the user's question made ready on it. Send its key to your officer's thread with `t3_thread_send`, and keep waiting. When the answer comes to your thread, write it on the Question and set the Question Done.
- It came back for any other reason, such as an agent that could not take the ticket. Dispatch a fresh agent onto it, and keep waiting.

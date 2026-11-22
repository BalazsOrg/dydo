# Brief

You dispatch another agent onto a ticket. It starts with your brief and the repository's `AGENTS.md`: it has none of your conversation and none of what you read.

**Write the brief**

Open with these three sentences: `You are the captain of <KEY>. Its contract is your outcome. Your officer is the captain of <your ticket's key>.`

Then name:

- `/ticket-captain`, and every other skill the work needs, each with a slash. An agent that merges needs `/pr`.
- What the ticket's result is: a change, a verdict, findings as one seat of a review, an answer, a cut, or a session with the user.
- How the ticket fits your ticket and the spec.
- The truth documents the project keeps beside the spec, or that there are none.
- For a change: the branch and the base it works on, the paths it owns, the gates it must pass, and how the work is proved.
- For a change: the skill that holds the project's rules for code, comments and tests, with a slash, or that the project has none.
- For a change: whether the agent merges it, and into which `feature/*` branch. A part that you will wire merges nothing.

| The ticket | Its brief adds |
| --- | --- |
| a review | the candidate, its head commit and its base commit; the model that wrote it; the ticket judged; the governing spec, or that there is none; the level and its lenses; what the author already proved |
| one seat of a review | the same, and the one kind of review it does; for Driving or Modelling, the skill for it, with a slash |
| a Question | the question, and the ticket that waits on it |
| a Fix | the Review it comes from, and every blocker to fix |
| a Wayfinding ticket | `/wayfinder`, the ticket whose route is not in sight, and its spec |

A review's level is the row of the reviewer's table that fits the candidate:

- `Leaf evidence check`, for one agent's change. Its lens: it works, with a security baseline.
- `Ticket review`, for a whole wired from parts. Add the lens: it integrates.
- `Feature / Milestone`, when a subsystem is done. Add the lenses: it is optimal, secure, performant and in harmony.

**Dispatch**

- Send the brief with `delegate_task`. Give the agent its own worktree (`isolation: "worktree"`) when it writes while others write, or when it opens its own pull request.
- A reviewer is never an agent that wrote any part of the candidate. Prefer a model family other than the author's.
- Pick the model and its effort from the roster:

{roster_placeholder}

- If the model you picked refuses with a limit, wait and retry. If it still refuses, use another model and say so on the ticket.

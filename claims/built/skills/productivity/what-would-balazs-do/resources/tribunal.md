<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->

# Tribunal

## decision

- List every option still standing, each as one exact action quoted from the final positions.
- Each member names one of them, verbatim.
- Tally by the verdict table in the skill.
- Research the case law, sending `research` sparingly where a source is out of reach.
- Write the case file below.
- Done when it holds every option, unranked.
- The clerk is mechanical: it forwards text verbatim, tracks turns and claims, and tallies ballots; its view enters only through its own turns.
- A concession quotes the evidence or restates the argument it yields to.
- Answer as Balazs would, in the spirit of his words, citing why he decided.
- Done when both return sealed positions and their results show the model and effort asked for.
- A factual claim names its check; the other two members run it independently and report confirmed, refuted or inconclusive.
- After that the tribunal is incomplete and ends without a decision; never swap a vendor silently.
- Later rounds arrive as queued messages; answer each with one turn.
- Send each the judge brief with the case file.
- Judges get this and nothing else from your session: the call, in one sentence; the objective; the constraints the options, unranked the primary evidence, quoted with its source and why he decided: stated by him, or inferred and naming the DRs or docs it rests on each premise, stated as a claim with its source your class for the call, and whether he is present or away
- You are a judge on a `what-would-balazs-do` tribunal.
- You may send `research` subagents after evidence.
- Never dispatch judges.
- Return your sealed position in the skill's form by <deadline>.
- As a judge, use the Sealed position form and phases 4-7 as a member; phases 1-3, the clerk duties and writing the ballot list are the clerk's.
- Write and log your own sealed position before any judge starts.
- Write your own turn before reading theirs.
- Set every phase's deadline when you open the case file.
- An experiment goes to a proof-only `code-writer` in an isolated worktree, its effects contained; its brief names the commissioning captain, the pinned source SHA and the test path.
- Members otherwise only read.
- The raising officer is a full member and also the clerk.
- A reframe states exactly what it changes, and the judges may object.
- Send every member the same bundle: the sealed positions and the last round's turns, always queued, never steered.
- Stop when a round changes nothing and no claim is pending, and after 4 rounds at most, evidence rounds and reframes included.
- the member's own class for the call the call, in its own words an option the file did not list, or why none exists the premises it checked, and what it found its answer: the action he would take and why, naming any recorded decision it overrides what would change its mind
- A missing or stalled member gets one retry. After that the tribunal is incomplete and ends without a decision; never swap a vendor silently.
- No class is lowered over an unresolved objection: the strictest class stands.
- Append each finished phase to the log before the next.
- A Linear Document `Tribunal log - <KEY>`, attached to the Question.
- Checkpoint `Dispatch` on every dispatch and retry, and append each round once complete, so a fresh clerk resumes without dispatching a judge twice or resetting a retry.

```markdown
## Case file
## Dispatch
<member>: task <id>; deadline <time>; retries <n>; pending: <turn or none>
## Sealed positions
<member> - <role/model and effort, as the result shows them>: <position>
## Round <n>
<member>: <turn, verbatim>
## Claims
<claim> | <check> | <member>: confirmed, refuted or inconclusive
## Ballot
<option, exact text>
<member>: <option, exact text>
## Verdict
result: <decision, prepared Question, or no decision>; stop: converged, cap or incomplete; undo: <steps>
```

- Three members, Popper-style: each position stands only while it survives the strongest challenge.
- Load that skill and follow its `resources/tribunal.md`.
- Each turn raises the strongest material objection to another member's position, or says why none exists, and answers the strongest outstanding challenge to its own.

## dispatch

- Start two fresh judges as T3 `delegate_task` children, one Claude and one Codex, each with the strongest model and effort set explicitly.

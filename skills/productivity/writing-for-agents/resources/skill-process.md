# Writing a skill by process

You hold a ticket to write or rewrite a skill: its `SKILL.md` and its resources. You run the steps below in order. One agent writes each file, and a second agent checks every line of it.

## The truth

The truth for the skill is what the user approved and said. Your brief says where it is. Where two sources disagree, the first in this order wins:

1. The user's latest recorded words.
2. The truth documents your brief names: the ideas every skill must agree with.
3. The statechart: the process the skill describes, one arrow for each case, with its page that says which position gets which text.
4. The Project's spec.
5. The decision records.

If your brief names no truth document and no statechart, ask your officer for them before you start. Where the user's words were recorded after a truth document or the statechart and disagree with it, tell your officer, because that document is then stale.

## Steps

1. **Sort the files.** List every file the skill has now. For each one, write one sentence: who reads it, in which situation, to do which job. Then change the file set until every file has its own sentence:
   - The sentence needs "or" between two readers or between two jobs: split the file.
   - Two files have the same sentence: merge them.
   - The statechart gives that reader no such text: delete the file.

   Settle the file set before anyone rewrites a line.
2. **Write.** Write each file by [Writing a skill file](skill-write.md), or brief a writer with it. Give the writer the file's sentence and the truth.
3. **Check.** Dispatch a second agent, never the writer, with [Checking a skill file](skill-check.md). Prefer a model family other than the writer's. Give it the files, each file's sentence and the truth, and leave out your reasons for any line.
4. **Fix.** Fix or cut every line that fails. If you think a failed line is right, send it back with its source: the checker rules, not you. Send the changed lines to the checker again, and repeat until every line holds.
5. **Keep the record.** Put the check record, with every line's last verdict, where your brief says: in the pull request or on the ticket, never in the skill.

## When the user comments on a line

A comment on one line names a fault that other lines have too.

1. Say the fault in one sentence that does not name the line. Examples: "a rule that is true only in this project", "a definition copied from a record that a later one replaced", "a line the reader must decode".
2. Have the checker look for that fault in every line of every file of the skill.
3. Fix every line it finds in the same piece of work. Return the fault's sentence with the lines it was found in.

## Return

For each file, its line count before and after. Then what you cut that someone might miss, the lines you are least sure of, and where the check record is.

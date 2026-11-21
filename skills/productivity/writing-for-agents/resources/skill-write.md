# Writing a skill file

You write or rewrite one skill file so that the agent who loads it can act on every line.

Your reader is an agent holding a ticket in a project you have never seen. It has this file, the skill that links to it, and its brief. It has none of your context.

Your brief gives you the file's one sentence, which says who reads it and for which job, and the truth: the core page and the statechart.

## Method

1. Read the file, and the skill that links to it. Write down, for yourself, what the reader must do differently because this file exists.
2. Delete first. A line stays only if all four hold:
   - A competent agent would get this wrong without being told.
   - The linking skill does not already say it.
   - It holds in a project other than the one it was written in.
   - The core page, the statechart or the user's recorded words say it. A rule in the old file is not a source.
3. Rewrite each line that stays as an action the reader can take.
4. Read the result as the reader. For each line ask "what exactly do I do now?" If you have to guess, rewrite the line or delete it.
5. When you find one fault, look for the same fault in every other line.

## Rules for a line

- One instruction, in plain words (ASD-STE100), short enough to read once.
- Say what to do. Say what never to do only where the wrong move is tempting.
- Where a rule is abstract, give two or three concrete examples in the same line.
- Name a thing by the name the reader sees: a section by its heading, a file by its path, a command as typed.
- Use a word the reader has met, or explain it where it first appears. No slogans, no terms from the history of how the file was made.
- Write to "you". Leave out "your project's": the reader has one project.
- Give a reason only when the reader needs it to apply the rule to a case the rule does not name.

## Rules for the file

- It serves one reader doing one job. If you find a second reader or a second job while you write, stop and return the file to be split.
- It fits on one screen, about forty lines. If it does not, you have not deleted enough.
- A lookup table counts as one line when the reader takes one row from it. A table the reader must read whole counts line by line.
- Group lines under the moment they apply: before you start, while you work, a named special case, finishing.
- A case that is rare, or that belongs to one project's tooling, goes out.
- For a rule that holds only in the project it was written in, take out the project's names. If a rule is left, write that rule: "run `make lint`" becomes "run the gates your brief names". If nothing is left, delete it.
- Keep any exact format another agent parses, such as a return line or a commit name.
- Before you rename a file, a heading or a format, search for everything that uses the name. Keep the name unless it has become false. If you change it, change every use in the same piece of work, and list each use you may not edit.
- If you are unsure a rule is true or where it came from, cut it and list it in your return.

## Return

The rewritten file, then a list of what you cut that someone might miss, and the lines you are least sure of.

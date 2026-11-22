# Writing docs

- Every sentence you write is a claim that needs a witness in the repository: code, configuration or governing intent (the spec, a DR, a ticket); where none exists, return the gap instead of the sentence.
- Write each meaning in one home and link to it from everywhere else.
- Write a skill, its resources or an entry file with /writing-for-agents.
- A descriptive doc explains and never authorizes: on what the code does, the code wins; a reason counts only when it links a DR or a principle.
- Write a new descriptive doc only when invited, and only if its subject is solid, important and expensive to rebuild from code.
- When your change touches the paths a doc's `explains:` declares, update the doc or state that it holds.
- A new DR you write carries `applies:` (or `new-ground`) and one `weighed: <slug> over <slug>, because <reason>` for each tension it resolved.
- Prove a docs change with `dydo check` and the checks the ticket names; run `dydo fix` after you add, move or rename a document.

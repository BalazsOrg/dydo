# A merge conflict

## Resolving a CONFLICT on the ticket branch

Arrivals:

- Resolve a merge conflict on the ticket branch only, then commit it as `<KEY> merge: <what>`.

Nominal path:

- Read both sides, and what changed since the source was reviewed, before you resolve a hunk.
- State each hunk's intended behaviour; one that needs a new product choice goes back to your captain.
- A resolution that refactors leaves the code no worse than either side.
- Return both parents and the merge commit's SHA.

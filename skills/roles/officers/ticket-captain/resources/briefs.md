<!-- built from the model; edit there: tools/SkillBuild -->

# Briefs

Brief without loading the child's kind resource: the child loads its own.

**Every brief names:**

- The ticket, by its key, and its kind.
- `/ticket-captain`, then every other skill the ticket needs, each with a slash.
- Whether you cut the ticket as a leaf: its captain then does the work itself, and splits it only if it proves too big.
- Who merges the work: the child, you, or nobody when it merges nothing.
- For a change: the paths it owns and the project's gates it must pass. For a Fix, also the findings it closes.
- For a question: the question, the choice that waits on it, and that you send the child to research it.
- Whether the child takes over a ticket another agent started, and then the commit it resumes from.

**A review brief also names:**

- The candidate, by its SHA, and the model family that wrote it.
- The PR the candidate is in, by its number, and the base's SHA; or that it has no PR, as at a leaf's evidence check.
- The governing spec, or that there is none.
- The level: a leaf's evidence check, a ticket review, or a feature or Milestone review. A leaf gets at most the check, never a full review: open the check as a Review ticket of its own beside the leaf's, for a fresh seat.
- The lenses, which rise up the tree: it works, with a security baseline; then it integrates; then it is optimal, secure, performant and in harmony. A first version never gets a final one's lenses.

Write no review brief for a plan, spec or contract: it gets no review rounds, and a risky spec at most one pass by a native helper, which returns questions.

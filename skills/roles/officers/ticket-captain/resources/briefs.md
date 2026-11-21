<!-- built from the model; edit there: tools/SkillBuild -->

# Briefs

Brief without loading the child's kind resource: the child loads its own.

**Every brief names:**

- The ticket, by its key, and its kind.
- `/ticket-captain`, then every other skill the ticket needs, each with a slash.
- Whether you cut the ticket as a leaf: its captain then does the work itself, and splits it only if it proves too big.
- Who merges the work: the child, you, or nobody when it merges nothing. When it is the child, also the branch its PR targets, a `feature/` branch: `dydo merge` takes no other.
- For a change: the paths it owns and the project's gates it must pass. For a Fix, also the findings it closes, each should by its ticket's key, and the SHA it starts from: your branch's head, which holds the candidate they were found at.
- For a Question you send a child to research, the only Question you brief: the question, the choice that waits on it, and that the child is to research it. A HITL Question goes up unbriefed, by [A need for the user](need-for-the-user.md).
- Whether the child takes over a ticket another agent started, and then the commit it resumes from.

**A review brief also names:**

- The candidate, by its SHA, and the model family that wrote it.
- The ticket whose work is judged, by its key, apart from the Review ticket's own: the round's holder files each should under it.
- The PR the candidate is in, by its number, and the base's SHA: a candidate you will merge is in its PR, a draft you opened by [Merging](merging.md), before you brief its Review, since `dydo merge` reads a PASS only from that PR's body. Only a candidate somebody else will have reviewed in a PR has none, a leaf's evidence check or the round of a part its officer merges, and its brief says so.
- The governing spec, or that there is none.
- The level, which follows the candidate and never the size of the ticket you brief: the evidence check of a leaf's change, a ticket review, or a feature or Milestone review. A leaf's change gets at most the check, never a full review: open the check as a Review ticket of its own beside the leaf's, for a fresh seat.
- The lenses, which rise up the tree: it works, with a security baseline; then it integrates; then it is optimal, secure, performant and in harmony. A first version never gets a final one's lenses.

Write no review brief for a plan, spec or contract: it gets no review rounds, and a risky spec at most one pass by a native helper, which returns questions.

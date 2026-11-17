<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->

# Break

## evidence

- A test that needs a sleep or a timeout is wrong; force races with fake timers or interleaving.

## inquisition

- Discard the test; note the observation.
- Discard the test; note the deciding observation.
- **List the hypotheses.** Yours, plus every register entry for the area tagged `testable`. Done when every tagged entry and every hypothesis of yours is on the list.
- Each names the suspected failure, the seam, the input or state, the expected observation, and the test that would refute it.
- Break forms hypotheses about how the area fails and decides each with a test it writes itself, on its proof branch.
- **Write one test per hypothesis** in the project's own test framework, at a test path, through the area's public seam where one exists, and run only that test. Done when each test has run.
- **Read the area for how it fails.** Its inputs, states, error paths, boundaries, concurrency and the seams it calls through. Done when you can name where the area's behaviour can go wrong.
- **Red, failing for the stated reason:** confirmed.
- **Green:** not reproduced.
- **Red for another reason, or undecidable:** inconclusive.
- Done when every hypothesis has one verdict and every confirmed one has its commit.

## return

- **Write the area's entries** in the return shape. A confirmed finding's `where` is the defect's `file:line`, not the test's; its proof is `red test <SHA> <test path>::<test name>`.
- A confirmed register entry is a sighting with that proof.
- Every not-reproduced and inconclusive hypothesis goes under `Not reproduced` with its observation, marked `inconclusive` where it is.
- Done when every hypothesis on the list appears exactly once in the return.

## worktree

- The worktree is clean.
- Commit the test alone as `<KEY> proof: <hypothesis>` and note the SHA.
- **Leave the branch for the captain.** The captain pushes it; you leave it committed and clean. Done when `git status` is clean and `git log` on the branch shows only your proof commits.
- `HEAD` is on the proof branch your brief names, `proof/<KEY>-<area>-<n>`, cut from the audit SHA.
- Every path you will touch is a test path: a test project, a test folder or a test file.
- Prove three things and return a failure to the captain instead of working around it:

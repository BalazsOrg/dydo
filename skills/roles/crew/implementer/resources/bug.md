# A Bug

- Name the expected and the observed behaviour at the highest stable seam, and what the fix must leave unchanged.
- Reproduce it red: a test at the product's boundary when the defect shows there, else at that seam; an existing red test is the reproduction.
- With no red reproduction yet, load /diagnosing-bugs.
- An unused member needs no test: its definition at `file:line` and a reference search with no caller outside the tests reproduce it, and the fix deletes it with its test-only callers.
- Fix it, then revert the fix, watch the reproduction fail and restore it; return that red line, with the behaviour you kept still green.
- Force a race with fake timers or a set interleaving; a test that needs a sleep or a timeout is wrong.

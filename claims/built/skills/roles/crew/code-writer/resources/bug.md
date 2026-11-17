<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->

# Bug

## evidence

- Name the expected and observed behaviour at the highest stable seam, and the behaviour the fix must not change.
- Reproduce it red: a scenario when the defect shows at the product's boundary, else a test at the seam.
- Fix it, then revert the fix, watch the reproduction fail, and restore.
- An existing red test, an inquisition's among them, is the reproduction; adopt it.
- Done when one observation could refute the reproduction.
- An unused member, export or field needs no test: its definition at `file:line` and a reference search showing no caller outside the tests are its reproduction.
- Its fix deletes the member with its test-only callers, and step 3's revert does not apply.

## return

- The fix claims success only with that red line in the return, or for an unused member its definition and reference search, and with the unchanged behaviour still green.

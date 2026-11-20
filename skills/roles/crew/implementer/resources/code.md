<!-- built from the model; edit there: tools/SkillBuild -->

# Writing code

- Load /codebase-design when you design a seam or an interface.
- Test at the deepest module boundary, or the product boundary, that can observe the behaviour; a unit test graduates only for an edge or trap that test can't reach cheaply.
- Mock only at the system's edge, such as external APIs, time and randomness; your own modules stay real.
- Write a suite test only to guard a trap, pin a traced contract (spec or DR), or reach an edge driving can't.
- Show every graduating test failing without the code: stash the code, run the test, restore; a Bug still starts from a red reproduction.
- Tag each graduating test with its job: trap (naming the bug or pitfall), contract (citing the spec or DR) or edge.
- Keep proof tests in the ticket's commits, and record their SHA in the receipt and in `proof:`.
- Make nothing depend on old evidence: a squash or a fresh start must break nothing.
- Write no test that changes whenever the code does, checks that a deleted feature is gone, or can never fail.
- Property tests for edges and differential tests for rewrites are guidance, never a gate.
- A rewrite or wide refactor gets a conformance suite, apart from its graduating tests and never instead of them: its traced assertions graduate like any graduating test, red first, and its 'matches old implementation' assertions go at the rewrite's landing.
- Run tests through `dydo test`, and the static gates through `dydo code-check`, which checks changed files only.

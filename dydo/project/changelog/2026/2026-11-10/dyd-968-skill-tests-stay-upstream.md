---
area: general
type: changelog
date: 2026-11-10
---

# dydo 3.7.0 - Skill test trees stay in the dydo repository (unreleased)

## Summary

The 3.6.0 skill copy instruction took all of `skills/`, so adopting projects took in the statechart
skill's own test suite, 387 files under `skills/engineering/statechart/scripts/test/`. A project
that requires every test file to belong to a route then failed its own checks
([DYD-968](https://linear.app/bodnar-balazs/issue/DYD-968)).

A skill's test tree is any `test` or `tests` folder inside a `skills/<category>/<name>/` folder.
dydo keeps the trees of its own skills in its repository, and CI and the release workflow run them
there. No install or update copies one into a project. A skill only the project has keeps its own
tests.

## Changed

- Every copy instruction now carries one exact recipe: clone the dydo repository outside the
  project, then `git archive` the release tag's `skills`, `setup-skills.mjs` and
  `THIRD-PARTY-NOTICES.md` with the two test-tree exclude pathspecs and extract the archive with
  `tar -xf` in the project root.
- `update-dydo` leaves upstream test trees out of its merge set and its Skills comparison. Step 6
  removes a tracked copy, step 8 proves no upstream test tree is tracked, and step 9 reports the
  removal.
- The statechart row of the third-party notices names the test corpus as staying in the dydo
  repository, so the notices stay true in a project that holds none.

`dydo statechart install-skill` is unchanged: its embedded payload already lists runtime files only.

No change in this entry renames a term mechanically, so it has no merge normalisations.

## Upgrade steps for the 3.7.0 release entry

The 3.7.0 release entry carries this step:

```markdown
1. Remove the copies of dydo's skill test trees. A test tree is any `test` or `tests` folder
   inside a `skills/<category>/<name>/` folder that the dydo repository holds; dydo keeps and runs
   them upstream. A project that took `skills/` whole at 3.6.0 tracks the statechart skill's suite,
   387 files: run `git rm -r skills/engineering/statechart/scripts/test`, list the removal, and
   revert any test-route exception the project added for those files. `update-dydo` at 3.7.0 does
   this in its step 6 and proves it in step 8. A skill only the project has keeps its own tests.
```

## Files Changed

`README.md`, `npm/README.md`, `dydo/reference/about-dynadocs.md`,
`Scaffold/dydo/reference/about-dynadocs.md`, `dydo/reference/dydo-commands.md`,
`Scaffold/dydo/reference/dydo-commands.md`, `skills/productivity/update-dydo/SKILL.md`,
`THIRD-PARTY-NOTICES.md`, its byte-identical mirror `npm/THIRD-PARTY-NOTICES.md`, and this entry.

## Related

- [DynaDocs](../../../../reference/about-dynadocs.md) - Skills install
- [Changelog](../../_changelog.md) - Releases and the Upgrade steps rule

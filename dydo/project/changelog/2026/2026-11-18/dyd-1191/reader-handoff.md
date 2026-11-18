---
area: project
type: reference
---

# DYD-1191 reader handoff

Next sequential code-writer hop, after the committed docs-writer hop. No tests were edited here.
The active ticket requires equal assertion strength, missing-option red then green, nonempty
discovery and reached-feature Light. The split bodies are the contract; do not copy option text
or canonical-source policy back into the hubs to make the old readers pass.

## Family loading contract

The local and Scaffold command families have the same suffixes: `core`, `linear`, `worktrees`,
`delivery`, `utilities`. The coverage family has `platforms`, `provisioning`, `running`, `results`,
`statics`, `coverage`, `measurement`. The guard family has `hook`, `paths`, `bash`, `denylist`,
`nudges`, `enforcement`. All names begin with the original filename stem plus a hyphen. The original
`.md` filename is the family hub. Use deterministic discovery or an explicit complete family set,
with nonempty assertions. Do not widen to unrelated reference docs and accidentally satisfy an
option or policy assertion using someone else's text.

For command-section parsing, read bodies only, excluding hub stubs: the hubs deliberately retain
the original `### dydo ...` headings for compatibility, but contain navigation, not option text.
Parsing hubs and bodies together with a last-write-wins dictionary can silently overwrite real
sections with empty stubs. Parse each body and merge command sections with duplicate detection, or
join a deterministic set of body pages that excludes hubs. Preserve all flag/required-example
assertions and existing reflection-based registry selection. Assert the split-page set and parsed
sections are nonempty. Generic policy/negative-text checks should read the logical family, including
the hub, and preserve the original positive and negative assertions over that family.

## Readers requiring changes

| Consumer | Current reader and observed effect | Required next-hop correction |
|---|---|---|
| `CommandDocConsistencyTests.AllOptions_AppearInReferenceDoc` | Line 127 reads only local hub; fails on 33 listed command-option pairs | Read all five local bodies; keep reflection, option extraction and failure reporting |
| `ReferenceDocAndTemplate_HaveSameOptions` | Lines 157-158 read only hubs; passes with empty stub flag sets | Read both complete body families independently; retain union of section names and symmetric missing-section/flag assertions; forbid empty parses |
| `Examples_IncludeAllRequiredFlags` | Line 202 reads only hub; passes because it sees no examples | Read all local command bodies; keep required-option and example assertions; ensure actual example discovery is nonempty |
| `ActiveProductDocs_ExcludeRetiredCommandsAndRepositoryWorkPaths` | Line 307 names local hub in fixed file list; moved text escapes the negative scan | Expand that one logical file to the local hub plus all command bodies; preserve the exact forbidden regex and other listed docs |
| `CanonicalSkillAssertionTests.CurrentGuidance` + `CanonicalSkillSteps.CurrentGuidanceUsesCanonicalSource` | Two entries name local and Scaffold hubs; Steps line 215 reads each file alone, line 217 fails | Read each entry as a logical family where it is a split hub; preserve canonical literal, stale-path and all forbidden-word assertions over each complete family |
| `test_testing_facade.TestingFacadeTests.test_owned_policy_docs_replace_tiers_with_the_dr048_gate_set` | Lines 1271-1287 read coverage hub alone; ordinary and inherited portable tests both fail at HCRAP | Replace only the coverage entry with hub plus seven bodies; keep every positive token, retired-token exclusion, normalized mutation requirement and special Scaffold prohibition |

Do not simply append every split command page to `CurrentGuidance`: its positive assertion would
then demand skill-authoring guidance on unrelated map/merge/statechart pages. Each original named
document remains one logical assertion unit. Negative assertions must cover all its moved text.

The retained .NET run discovers 29 tests: 13 command-consistency, 5 help, 3 ScaffoldTree and 8
canonical-skills scenarios. It passes 27 and fails exactly option completeness and “Remove stale
two-source guidance”. The Python selector discovers both the base and inherited portable test and
both fail. Keep that discovery coverage after refactoring test files.

## Missing-option red and green

After updating readers, first establish green for all unchanged assertions. Then remove a real
option's documentation from its owning section in a disposable snapshot: for example every
`--join` mention in the local `dydo init` section of `dydo-commands-core.md`, while leaving the
runtime registry, Scaffold and assertions unchanged. Run the exact option-completeness test and
retain a failure naming `--join (command: init)`. Restore the original doc bytes and retain green.
This proves the new reader reaches a split body's options. A renamed filter with zero discovered
tests, a missing hub, an empty parse or a weaker assertion does not meet this gate.

Run option parity and example checks too. Mutation of only a required flag in one actual example
can demonstrate required-example reach if needed; keep the existing assertion's semantics. The
ticket does not require a new Gherkin journey for this documentation restructuring.

## DR069 consequence for the code hop

All four likely existing edit targets already exceed the file limit at this pinned base:

| Existing test source | Bytes |
|---|---:|
| `DynaDocs.Tests/Commands/CommandDocConsistencyTests.cs` | 20,441 |
| `DynaDocs.Tests/Steps/CanonicalSkillSteps.cs` | 25,816 |
| `DynaDocs.Tests/Steps/CanonicalSkillAssertionTests.cs` | 22,144 |
| `DynaDocs.Tests/coverage/tests/test_testing_facade.py` | 151,496 |

Their next edit triggers DR069. Captain must include necessary test-source/helper splitting in the
code-writer's owned paths; do not waive the gate, add an ignore for authored tests or raise the limit.
Preserve .NET/Reqnroll and Python unittest discovery, including inherited portable tests and existing
assertions, when splitting. The docs audit's optional `--docs-only` excludes test scope deliberately;
the code hop must separately prove every changed test/helper file is below 20,000 bytes.

## Consumers that keep their current behavior

- Runtime `Commands/HelpCommand.cs:71` prints `./dydo/reference/dydo-commands.md`, which remains a
  hub. `HelpCommandTests.Help_ContainsReferenceToDocumentation` and all four other help tests pass.
- `CommandDocConsistencyTests.ScaffoldExamples_UseRequiredFlags` recursively enumerates Scaffold
  Markdown already. It reaches the new pages and passes without a reader change. Its other help,
  license, about-reference and registry checks also retain their existing inputs.
- `DynaDocs.csproj` embeds `Scaffold/**` with repository-relative logical names. New pages are
  automatically included; `ScaffoldTreeTests` proves the entire on-disk/resource set and bytes,
  and all three tests pass after a fresh isolated build. Keep the option-parity test separate:
  byte identity between local and Scaffold text is intentionally not the product contract.
- `Services/ScaffoldTree.cs` and init consume those resources rather than hardcoded original
  filenames. No resource registry or generated manifest edit is necessary.
- `DocScanner`, `MarkdownParser`, `AnchorExtractor` and `LinkResolver` scan the docs tree and links.
  New pages are included automatically; native local check passes with zero errors/warnings.
- The historical config fixture `DynaDocs.Tests/Fixtures/config/dydo-2x-old-keys.json` retains its
  old template-hash key. It is historical config data, not a current body reader.
- `DynaDocs.Tests/GuardRetroCommands.cs` retains recorded search/guard command strings; they remain
  meaningful with the original filenames present. No retro transcript is rewritten.
- README/npm public URLs, diagrams and general documentation pointers continue to land on the
  surviving hubs. Historical changelog, migration, inquisition and decision evidence remains
  accessible. All 175 explicit filename consumers have individual dispositions in the inventory.
- Seven bounded inbound documents point straight to split owners. The unchanged oversized
  testing-strategy provisioning link and all other inbound fragment links retain working hub anchors.
- There are no explicit matching references under skills/ or claims/ in the bounded `rg` search;
  both source trees remain untouched. Canonical-skills checks consume documentation outside them.

## Proof selection and handback

The exact .NET selection retained here is:

```text
FullyQualifiedName~CommandDocConsistencyTests|FullyQualifiedName~HelpCommandTests|FullyQualifiedName~ScaffoldTreeTests|FullyQualifiedName~OneCanonicalSkillTreeReachesEverySupportedHostFeature
```

The Python selector is unittest discovery of `test_testing_facade.py` with
`-k test_owned_policy_docs_replace_tiers_with_the_dr048_gate_set`. After splitting that module,
adapt discovery to include the new owners and retain both inherited cases. Reached-feature Light
must include any test/helper ownership reached by the refactor; select from actual new reach.
The original run's .NET snapshots restore locked dependencies, build fresh embedded resources and
clean themselves up. No full suite, coverage campaign or mutation pass is claimed in this docs hop.

Use the [implementation recipe](./implementation.md#exact-rerun-recipe-and-remaining-gates), retain
the original red receipts, then record next-hop green and changed-code/test statics separately.
Default `audit.py` checks this docs-only scope; `audit.py --docs-only` after the code hop still checks
every resulting documentation byte and anchor without claiming to validate the test edits. No
`--write` is needed for proof. New helper paths must be owned and reviewed by the captain.

Fresh independent Claude Opus high review after fixes, DR058 grading, ticket-wide gates and PR work
remain captain-owned. Statechart, mutation and native Windows are unmet, never pass. This handoff is
an IMPLEMENTED documentation return, not independent PASS and not final ticket acceptance.

## Related

- [Split design and proof](./implementation.md)

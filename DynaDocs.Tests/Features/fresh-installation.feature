# Job: contract: dydo-commands-core.md dydo init then dydo check; DYD-99 (a fresh installation is usable): for each integration selection, init exits 0 and records the Claude and Codex integrations; check then exits 0 with warnings and no errors, and once the two foundation documents are customized it exits 0 with neither
@DYD-99
Feature: A fresh installation remains usable
  A project can adopt dydo without this repository's private working context.
  Checking and the authored documentation tree keep the installation usable.

  Scenario Outline: Maintain a fresh installation for each integration selection
    Given an empty project directory
    When I initialize dydo with "<integration>"
    Then the command succeeds
    And the recorded Claude integration is <claude>
    And the recorded Codex integration is <codex>
    When I check the documentation
    Then the documentation has no validation errors but has warnings
    When I customize the foundation documents for a tiny task-list project
    And I check the documentation
    Then the documentation has no validation errors or warnings

    Examples:
      | integration | claude | codex |
      | none        | false  | false |
      | claude      | true   | false |
      | codex       | false  | true  |
      | all         | true   | true  |

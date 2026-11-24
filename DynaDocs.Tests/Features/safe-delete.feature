# Job: contract: DYD-1029, DYD-1133 - each scenario row gets the outcome these tickets set for its input
@SafeDelete
Feature: Safe delete archives unique work before removal
  Scenario: Recover files governed by repository-local exclude and attributes
    Given an owned safe-delete repository
    When I archive a worktree governed by repository-local exclude and attributes
    Then the archive restores ordinary ignored and attribute behavior after source garbage collection

  Scenario Outline: Archive an unpushed worktree with proved remote absence
    Given an owned safe-delete repository
    When I archive the unpushed worktree in a "<visibility>" closed-world repository
    Then the absent remote ref required no push and the archived worktree restores independently
    Examples:
      | visibility |
      | public |
      | private |

  Scenario Outline: Recover history and every worktree version independently
    Given an owned safe-delete repository
    When I archive and restore the "<journey>" journey
    Then the safe-delete journey preserves all accepted state
    Examples:
      | journey |
      | unfetched remote |
      | detached HEAD |
      | local-only restore |
      | explicit remote restore |
      | proof-enabled settled T3 eligibility |

  Scenario Outline: Refuse without losing the original target
    Given an owned safe-delete repository
    When I exercise the "<refusal>" safe-delete refusal
    Then the safe-delete target refs and files remain intact
    Examples:
      | refusal |
      | local default |
      | remote default |
      | unknown remote default |
      | main worktree |
      | current worktree |
      | locked worktree |
      | busy worktree |
      | checked out branch |
      | missing worktree |
      | prunable worktree |
      | feature remote |
      | PR head |
      | fetch failure |
      | copy failure |
      | unreadable source |
      | nested repository |
      | unsupported file |
      | in-progress Git |
      | local ref race |
      | remote ref race |
      | source file race |
      | live T3 owner |
      | custom T3 root |
      | previous T3 root |

  Scenario Outline: Validate archive policy through the command
    Given an owned safe-delete repository
    When I exercise the "<policy>" safe-delete policy
    Then the safe-delete policy outcome is verified
    Examples:
      | policy |
      | empty bundle |
      | corrupt payload |
      | incomplete archive |
      | restore collision |
      | recovery ref collision |
      | remote restore collision |
      | restore failure |
      | partial deletion failure |
      | expiry boundary |
      | read-only commands never purge |
      | default byte reserve |
      | default percentage reserve |
      | configured reserve |
      | invalid reserve |
      | dry-run estimates |
      | dry-run remote lower bound |
      | archive symlink |
      | archive traversal |
      | link round trip or refusal |
      | differently named upstream kept |

  Scenario Outline: Raw destructive Git requires the archive command
    Given an owned safe-delete repository
    When the safe-delete guard examines "<command>"
    Then its safe-delete verdict is "<verdict>"
    Examples:
      | command | verdict |
      | git branch -D topic | refuse |
      | git branch -d -f topic | refuse |
      | git branch --delete --force topic | refuse |
      | git update-ref -d refs/heads/topic | refuse |
      | git push origin --delete topic | refuse |
      | git -C repo push origin --delete topic | refuse |
      | git -c core.bare=false push origin :refs/heads/topic | refuse |
      | /usr/bin/git --git-dir=repo push origin +:refs/heads/topic | refuse |
      | git push --prune origin | refuse |
      | git push --mirror origin | refuse |
      | git worktree remove tree | refuse |
      | git worktree remove --force tree | refuse |
      | git branch -d merged-topic | allow |
      | dydo archive branch topic | allow |
      | dydo archive worktree tree | allow |
      | dydo archive list | allow |
      | dydo archive restore ID | allow |
      | dydo archive list && git branch -D topic | refuse |

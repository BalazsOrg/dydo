# Job: contract: spec N10; DYD-1029 (through dydo archive and the guard; each case shows one clause: an archived branch
# or worktree restores its unique state after deletion; a refusal exits 2 and keeps the target; a failure after the
# remote deletion exits 2 and keeps the complete archive and the local targets; restore refuses a damaged or colliding
# archive and keeps it; a dry run prints its estimates and neither archives nor deletes; archives expire only when a
# later archive purges them; a differently named upstream is reported and kept; links round-trip (Windows: refused);
# the guard refuses raw destructive Git and admits dydo archive)
@SafeDelete
Feature: Safe delete archives unique work before removal
  Scenario Outline: Recover history and every worktree version independently
    Given an owned safe-delete repository
    When I archive and restore the "<journey>" journey
    Then the safe-delete journey preserves all accepted state
    Examples:
      | journey |
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

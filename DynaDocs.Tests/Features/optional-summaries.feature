# Job: contract: DYD-98 (a document needs no opening summary; check and fix never demand or insert one); writing-docs.md Rules Enforced (Title, Links, Frontmatter: the findings check still reports)
@DYD-98
Feature: Optional document summaries
  A document may begin with useful content after its title.
  Checking and fixing documents must not demand an opening summary.

  Background:
    Given a documentation tree whose frontmatter, navigation and links are valid

  Scenario Outline: Checking needs a title, not an opening summary
    Given a document whose body after its frontmatter is "<body>"
    When I check the documentation
    Then checking reports <finding>

    Examples:
      | body                                                | finding                     |
      | # Document\n\n## Details\n\nUseful content.         | nothing                     |
      | # Document\n\n- First item\n- Second item           | nothing                     |
      | # Document                                          | nothing                     |
      | # Document\n\n(One-line summary)                    | nothing                     |
      | ## Details\n\nUseful content.                       | "Missing title (# heading)" |
      | # Document\n\n## Details\n\n[Missing](./missing.md) | "Broken link: ./missing.md" |

  Scenario Outline: Fixing leaves a document's bytes alone
    Given a document whose body after its frontmatter is "<body>"
    When I fix the documentation
    Then that document's bytes are unchanged

    Examples:
      | body                                        |
      | # Document\n\n## Details\n\nUseful content. |
      | # Document\n\nA useful navigation description. |

  Scenario: A document without frontmatter is left for a manual repair
    Given a document with an H1 title and no frontmatter or opening summary
    When I fix the documentation
    Then that document's bytes are unchanged
    When I check the documentation
    Then checking reports "Missing frontmatter"

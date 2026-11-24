# Job: contract: DR 063 §1 §4 §5 (each scenario pins one of their rules through the command: by scenario, §4's findings one line each sorted by kind then key; §5's argument shapes with their exits, the unknown Project, and a Linear failure exiting 2 and named; §1's shared key help with no request, and a read whose every request is a query)
@survey
Feature: Survey a Project's lanes
  dydo survey reads one Linear Project on demand, with the user's key, and prints a short digest:
  one line per finding, sorted by kind and then key. It writes nothing and keeps nothing.

  Scenario: Each kind of finding is one line, sorted by kind and then key
    Given the Project's body is:
      """
      ## Goal
      - DYD-10 ships the release
      """
    And the Project has these issues:
      | key    | state       | assignee | blocked by | label    | priority | project |
      | DYD-1  | In Progress | Ada      |            |          | 0        | Map     |
      | DYD-2  | Todo        |          | DYD-1      | Feature  | 0        | Map     |
      | DYD-3  | Done        |          |            |          | 0        | Map     |
      | DYD-4  | Done        |          |            |          | 0        | Map     |
      | DYD-5  | Todo        |          | DYD-4      | Feature  | 0        | Map     |
      | DYD-7  | Backlog     | Ada      |            | Question | 0        | Map     |
      | DYD-8  | Todo        |          |            | Feature  | 0        | Map     |
      | DYD-9  | Backlog     |          | DYD-3      |          | 0        | Other   |
      | DYD-10 | Todo        | Ada      |            | Feature  | 0        | Map     |
      | DYD-11 | Backlog     | Ada      |            | Question | 2        | Map     |
      | DYD-12 | Backlog     |          |            | Feature  | 0        | Map     |
      | DYD-13 | Backlog     |          | DYD-1      | Feature  | 0        | Map     |
    When I survey "25acbbb9a90b"
    Then the survey exits 0
    And the survey lists these findings, in order:
      | kind                 | key    |
      | pickable             | DYD-8  |
      | question-no-priority | DYD-7  |
      | unblocked            | DYD-5  |
      | unblocked            | DYD-9  |
      | uncontracted         | DYD-12 |

  Scenario Outline: The argument is a Project UUID, slugId or Project URL
    Given a quiet Project
    When I survey "<argument>"
    Then the survey exits <exit>
    And Linear was asked for Project "<asked>"
    Examples:
      | argument                                               | exit | asked                                |
      | 4e6aa9da-f325-4b53-a318-deba4b1b4d26                   | 0    | 4e6aa9da-f325-4b53-a318-deba4b1b4d26 |
      | 25acbbb9a90b                                           | 0    | 25acbbb9a90b                         |
      | https://linear.app/x/project/map-25acbbb9a90b          | 0    | 25acbbb9a90b                         |
      | https://linear.app/x/project/map-25acbbb9a90b/overview | 0    | 25acbbb9a90b                         |
      | https://linear.app/x/project/25acbbb9a90b/issues       | 0    | 25acbbb9a90b                         |
      |                                                        | 2    | nothing                              |
      | P-DYD-46                                               | 2    | nothing                              |
      | dydo 3.6.0                                             | 2    | nothing                              |
      | 25ACBBB9A90B                                           | 2    | nothing                              |
      | 25acbbb9a90                                            | 2    | nothing                              |
      | {4e6aa9da-f325-4b53-a318-deba4b1b4d26}                 | 2    | nothing                              |
      | https://example.com/x/project/dydo-25acbbb9a90b        | 2    | nothing                              |
      | https://linear.app/x/issue/DYD-530/dydo-survey         | 2    | nothing                              |
      | https://linear.app/x/project                           | 2    | nothing                              |
      | https://linear.app/x/project/dydo-current-release      | 2    | nothing                              |

  Scenario: An unknown Project is a usage error
    Given Linear has no such Project
    When I survey "25acbbb9a90b"
    Then the survey exits 2
    And the error says "Linear has no project 25acbbb9a90b"

  Scenario: Without LINEAR_API_KEY the survey prints dydo map's key help
    Given a quiet Project
    And LINEAR_API_KEY is not set
    When I survey "25acbbb9a90b"
    Then the survey exits 2
    And the error is dydo map's missing-key help
    And Linear was asked for Project "nothing"

  Scenario Outline: A Linear failure exits with the tool-error code and names the failure
    Given Linear fails with "<failure>"
    When I survey "25acbbb9a90b"
    Then the survey exits 2
    And the error says "<message>"
    Examples:
      | failure        | message                        |
      | HTTP 500       | Linear answered HTTP 500       |
      | a rejected key | Linear rejected LINEAR_API_KEY |
      | no answer      | Could not reach Linear         |

  Scenario: The survey only reads
    Given a quiet Project
    When I survey "25acbbb9a90b"
    Then every Linear request was a query

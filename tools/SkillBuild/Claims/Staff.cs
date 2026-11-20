namespace DynaDocs.SkillBuild;

/// <summary>The chief of staff (M11): his short list by day, an unknown on his machine, and the night from his muster to the morning brief.</summary>
public static class Staff
{
    public static readonly Claim Ask = new("He may ask the chief of staff for cross-Project thinking or about an unknown on his machine.", [Actor.Balazs, Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "He asks for big cross-Project thinking, or about an unknown on his machine: find it out.", new(StaffState.Investigating, 1001, MoveKind.Arrival))])
    { Spec = ["H7"], Transition = new(StaffState.Day, StaffState.Investigating, "he asks for cross-Project thinking, or about an unknown on his machine") };

    public static readonly Claim Answered = new("The chief of staff gives him what it found.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Give him what you found, then go back to the short list.", new(StaffState.Investigating, 1002, MoveKind.Exit))])
    { Spec = ["H7"], Transition = new(StaffState.Investigating, StaffState.Day, "findings given to him") };

    public static readonly Claim Muster = new("The night starts with his muster; overnight mode is the chief of staff's alone (DYD-1082 555a8300 gap 6, DYD-1195 33520d6c).", [Actor.Balazs, Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "His muster starts the night: load `overnight-mode` and hold the muster with him; overnight mode is yours alone, and the coordinators run as they do by day.", new(StaffState.Muster, 1003, MoveKind.Arrival))])
    { Spec = ["H7", "K11"], Transition = new(StaffState.Day, StaffState.Muster, "he musters: the night starts") };

    public static readonly Claim Watch = new("At muster the chief of staff lists every open dr-grade call as a thread link; then the overnight watch runs.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "At muster, list for him every open ticket labelled `dr-grade` as a thread link, then start the overnight watch.", new(StaffState.Muster, 1004, MoveKind.Exit))])
    { Spec = ["L21", "H7"], Transition = new(StaffState.Muster, StaffState.NightWatch, "the DR-grade calls listed; the watch starts") };

    public static readonly Claim Morning = new("The night ends on his return, and the morning brief closes it (DYD-1082 555a8300 gap 7).", [Actor.Balazs, Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "When he returns, the night ends: give him the morning brief.", new(StaffState.NightWatch, 1005, MoveKind.Exit))])
    { Spec = ["H7", "L19"], Transition = new(StaffState.NightWatch, StaffState.MorningBrief, "he returns: the night ends") };

    public static readonly Claim Brief = new("The morning brief lists overnight proposed DRs for ratification, never decided Questions.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "List each overnight `proposed` DR for his ratification; a decided Question is never listed for approval.", new(StaffState.MorningBrief, 1006, MoveKind.Exit))])
    { Spec = ["L19", "L21"], Transition = new(StaffState.MorningBrief, StaffState.Day, "the morning brief closes the night") };

    public static readonly Claim ShortList = new("The chief of staff keeps one short list for him from dydo survey board across Projects.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Keep his one short list from `dydo survey board` across Projects: HITL threads waiting on him, HITL Merges waiting on his click, and anything waiting too long, each as a thread link.", new(StaffState.Day, 1007, MoveKind.Nominal))])
    { Spec = ["H7"] };

    public static readonly Claim Continuous = new("It lists continuous tickets with their age and gives one out when he says so.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "List the continuous tickets with their age; when he tells you to, give one to an officer with an open lane, and leave anything outside the pull rule on the list for him.", new(StaffState.Day, 1008, MoveKind.Nominal))])
    { Spec = ["H7", "H8"] };

    public static readonly Claim NoEnvoy = new("It catches an open HITL ticket with no envoy.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Catch each open HITL ticket that has no envoy.", new(StaffState.Day, 1009, MoveKind.Nominal))])
    { Spec = ["H7"] };

    public static readonly Claim LinksOnly = new("It never relays content and sends no envoys.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Pass on thread links, never their content, and send no envoys: an escalation that needs him goes through its coordinator's own envoy.",
            new(StaffState.Day, 1010, MoveKind.Nominal), () => [Envoy.OwnEnvoy])])
    { Spec = ["H7"] };

    // The muster, night survey and morning brief resources: the pre-candidate chief of staff's checklist and templates
    // (K22), less the night orders and the admiral's standing orders, which DYD-1195 33520d6c (Q12) superseded.
    private const string Request = """
        ```text
        His muster request for <date>, <Project>. Return:
        Intent: his goals and purpose for the Project in your own words, by /in-your-own-words, apart from the work.
        Work: your next tickets in fill order, the first runnable one, and the plan's stop line, or none.
        Blockers: each operational obstacle, and the evidence that would show it fixed.
        Permissions: each action the night's work needs, with a safe exercise for it, or the gap.
        ```
        """;

    private const string Present = """
        ```text
        <Project> — <coordinator thread link>
        Intent, as the coordinator reads it: <verbatim read-back>
        Work: <next tickets, the first runnable one>
        Stop line: <the plan's, or none>
        Unblock now: <obstacle; fix; evidence it holds>
        Permissions: <action; safe exercise; result, or the gap>
        ```
        """;

    private const string MorningForm = """
        ```text
        ## The night
        <one real visual of the work touched>
        - <result; at most three result lines>

        ## PRs into master awaiting your click
        - <title> (<PR link>): <result>. Review <verdict>; CI <state on the current head>.

        ## Proposed DRs to ratify
        - <title> (<thread link>): <what it decides>. Ratify or decline.

        ## Waiting on you
        - <each HITL thread waiting on him, as a thread link>

        ## Next
        1. <next action and its owner>
        ```
        """;

    public static readonly Claim MusterReady = new("The muster is a checklist held with him present, after overnight-mode's transition preflight; the night starts once it is done.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Hold this checklist with him present, after `overnight-mode`'s transition preflight; the night starts only once every line is done.", new(ChiefOfStaffResource.Muster, 2001))])
    { Spec = ["K11", "H7", "K22"] };

    public static readonly Claim MusterAsk = new("Each live coordinator returns a muster report to a fixed request.", [Actor.ChiefOfStaff, Actor.Coordinator],
        [new(Skill.ChiefOfStaff, "Ask each live coordinator for its muster report, with the request below.", new(ChiefOfStaffResource.Muster, 2002))])
    { Spec = ["K11", "H7", "K22"] };

    public static readonly Claim MusterShow = new("He sees each report, its intent read-back apart from the work, and his corrections go back to its coordinator.", [Actor.ChiefOfStaff, Actor.Balazs],
        [new(Skill.ChiefOfStaff, "Show him each report verbatim in the form below, the intent read-back apart from the work, and send each coordinator his corrections to its read-back.", new(ChiefOfStaffResource.Muster, 2003))])
    { Spec = ["K11", "H7", "K22"] };

    public static readonly Claim MusterUnblock = new("Operational blockers are resolved while he is present, each fix verified with evidence.", [Actor.ChiefOfStaff, Actor.Balazs],
        [new(Skill.ChiefOfStaff, "Resolve each operational blocker while he is present, verifying each fix with evidence.", new(ChiefOfStaffResource.Muster, 2004))])
    { Spec = ["K11", "K22"] };

    public static readonly Claim MusterPermissions = new("Each permission the night needs is exercised safely and its real result recorded; no dummy merge.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Exercise each permission the night's work needs through a safe, authorised operation where one exists, such as contacting the coordinator, each session's Linear writes and feature-branch merges, and record the real result; an unprotected branch proves no merge permission, and a dummy merge is never run.", new(ChiefOfStaffResource.Muster, 2005))])
    { Spec = ["K11", "K22"] };

    public static readonly Claim MusterGaps = new("An exercise that can't run or an item left unresolved is a gap with his explicit disposition.", [Actor.ChiefOfStaff, Actor.Balazs],
        [new(Skill.ChiefOfStaff, "Keep an exercise you can't run, or an item left unresolved, as a gap, and get his explicit disposition for the work that depends on it.", new(ChiefOfStaffResource.Muster, 2006))])
    { Spec = ["K11", "K22"] };

    public static readonly Claim MusterStopLine = new("A stop line he wants goes in the Project plan the coordinator reads; there are no night orders (DYD-1195 33520d6c).", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "A stop line he wants goes in the Project plan, which its coordinator reads; there are no night orders.", new(ChiefOfStaffResource.Muster, 2007))])
    { Spec = ["K11"] };

    public static readonly Claim MusterRunnable = new("Each coordinator confirms its next work is runnable before the night starts.", [Actor.ChiefOfStaff, Actor.Coordinator],
        [new(Skill.ChiefOfStaff, "Get each coordinator's confirmation that its next work is runnable; then the muster is ready.", new(ChiefOfStaffResource.Muster, 2008))])
    { Spec = ["K11", "K22"] };

    public static readonly Claim MusterRequest = new("The muster request a coordinator answers.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, Request, new(ChiefOfStaffSlot.MusterRequest, 2010))])
    { Spec = ["K11", "K22", "H5"] };

    public static readonly Claim MusterForm = new("The form each Project's report is shown to him in.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, Present, new(ChiefOfStaffSlot.MusterPresent, 2011))])
    { Spec = ["K11", "K22"] };

    public static readonly Claim SurveyRead = new("Each watch tick reads the board across the Projects against the last observation, keeping no shadow board.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Each watch tick, read `dydo survey board` across the Projects and compare it with your last observation; keep no shadow board.", new(ChiefOfStaffResource.NightSurvey, 2030))])
    { Spec = ["H7", "K22", "S26"] };

    public static readonly Claim SurveyThreads = new("Where progress is unclear, the coordinator's thread is read, a captain's only as needed.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Where progress is unclear, read the coordinator's thread, and a captain's only as needed, and check every nudge still unanswered.", new(ChiefOfStaffResource.NightSurvey, 2031))])
    { Spec = ["H7", "K22"] };

    public static readonly Claim SurveyFinds = new("The survey looks for idle officers with runnable work, unanswered nudges, blockers and colliding work.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Look for an idle officer with runnable work, an unanswered nudge, a blocker that needs intervention and colliding work; an unchanged board or a quiet running task alone is no stall.", new(ChiefOfStaffResource.NightSurvey, 2032))])
    { Spec = ["H7", "H12", "K22"] };

    public static readonly Claim SurveyComplete = new("A tick is quiet only once every Project and nudge is checked; an unreadable source leaves it incomplete.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "A tick ends quiet only once every Project and every outstanding nudge is checked; a source you could not read leaves it incomplete, to resolve, never quiet.", new(ChiefOfStaffResource.NightSurvey, 2033))])
    { Spec = ["H7", "K22"] };

    public static readonly Claim MorningFill = new("The morning brief is filled in the conversation after the verified shutdown, writing nothing to the repo.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Fill this in the conversation once `overnight-mode` has verified the shutdown, writing nothing to the repo.", new(ChiefOfStaffResource.MorningBrief, 2020))])
    { Spec = ["H7", "K11", "K22"] };

    public static readonly Claim MorningTemplate = new("The morning brief's form: the night, PRs awaiting his click, proposed DRs to ratify, what waits on him, and next.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, MorningForm, new(ChiefOfStaffResource.MorningBrief, 2021))])
    { Spec = ["H7", "L19", "L21", "K22"] };

    public static readonly Claim DoubleCheck = new("Overnight, its 12-hour check on a quiet Project is a cheap double check only.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Overnight, your 12-hour check on a quiet Project is only a cheap double check behind the coordinators' own scheduled surveys.", new(StaffState.NightWatch, 1011, MoveKind.Nominal))])
    { Spec = ["H7", "H12"] };
}

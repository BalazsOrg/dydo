namespace DynaDocs.SkillBuild;

/// <summary>The chief of staff (M11): his short list by day, an unknown on his machine, and the overnight muster, watch and morning brief.</summary>
public static class Staff
{
    public static readonly Claim Ask = new("He may ask the chief of staff for cross-Project thinking or about an unknown on his machine.", [Actor.Balazs, Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "He asks for big cross-Project thinking, or about an unknown on his machine: find it out.", new(StaffState.Investigating, 1001, MoveKind.Arrival))])
    { Spec = ["H7"], Transition = new(StaffState.Day, StaffState.Investigating, "he asks for cross-Project thinking, or about an unknown on his machine") };

    public static readonly Claim Answered = new("The chief of staff gives him what it found.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Give him what you found, then go back to the short list.", new(StaffState.Investigating, 1002, MoveKind.Exit))])
    { Spec = ["H7"], Transition = new(StaffState.Investigating, StaffState.Day, "findings given to him") };

    public static readonly Claim Muster = new("Overnight mode, the chief of staff's and the admiral's, starts at muster.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Overnight mode starts at muster, which you hold.", new(StaffState.Muster, 1003, MoveKind.Arrival))])
    { Spec = ["K11", "L21"], Transition = new(StaffState.Day, StaffState.Muster, "overnight mode starts") };

    public static readonly Claim Watch = new("DR-grade calls surface at muster; then the overnight watch runs.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Surface every pending DR-grade call to him at muster, then start the overnight watch.", new(StaffState.Muster, 1004, MoveKind.Exit))])
    { Spec = ["L21", "H7"], Transition = new(StaffState.Muster, StaffState.NightWatch, "DR-grade calls surfaced; the watch starts") };

    public static readonly Claim Morning = new("The night ends in the morning brief.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "When the night ends, write the morning brief.", new(StaffState.NightWatch, 1005, MoveKind.Exit))])
    { Spec = ["H7", "L19", "K11"], Transition = new(StaffState.NightWatch, StaffState.MorningBrief, "the night ends") };

    public static readonly Claim Brief = new("The morning brief lists overnight proposed DRs for ratification, never decided Questions.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "List each overnight `proposed` DR for his ratification; a decided Question is never listed for approval.", new(StaffState.MorningBrief, 1006, MoveKind.Exit))])
    { Spec = ["L19", "L21"], Transition = new(StaffState.MorningBrief, StaffState.Day, "the brief is given") };

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
        [new(Skill.ChiefOfStaff, "Pass on thread links, never their content, and send no envoys: an escalation that needs him goes through its admiral's own envoy.",
            new(StaffState.Day, 1010, MoveKind.Nominal), () => [Envoy.OwnEnvoy])])
    { Spec = ["H7"] };

    public static readonly Claim DoubleCheck = new("Overnight, its 12-hour check on a quiet Project is a cheap double check only.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Overnight, your 12-hour check on a quiet Project is only a cheap double check behind the admirals' own scheduled surveys.", new(StaffState.NightWatch, 1011, MoveKind.Nominal))])
    { Spec = ["H7", "H12"] };
}

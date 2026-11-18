namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the workflows the inventory's carriers.json declared.</summary>
public static class FixtureWorkflows
{
    public static readonly SkillSpec OvernightMode = SkillSpec.Workflow(S.OvernightMode, "Overnight Mode", "Overnight mode, the user away while admirals run on standing and night orders. Load only on the chief of staff's muster or morning-brief announcement, or when the user calls overnight mode.") with
    {
        Sections = [new(F.OvernightModeMuster, "Muster"), new(F.OvernightModeStandingOrders, "Standing orders"), new(F.OvernightModeNightWatch, "Night watch"), new(F.OvernightModeMorningBrief, "Morning brief")],
        Resources =
        [
            new(OvernightModeResource.TransitionPreflight, "Transition preflight", [new(F.OvernightModeTransitionPreflightLead), new(F.OvernightModeTransitionPreflightAtEitherTransition, "At either transition")])
        ]
    };

    public static readonly SkillSpec Walkthrough = SkillSpec.Workflow(S.Walkthrough, "Walkthrough", "Show me the product as it stands - what changed since the last walkthrough, where to look first, and how to try it. Load only when the user asks, or at the map holder's Milestone or landing step.") with
    {
        Sections = []
    };

    public static readonly SkillSpec Wayfinder = SkillSpec.Workflow(S.Wayfinder, "Wayfinder", "Fog in a Project map or inside one ticket. Chart the visible route as Wayfinding Tickets and resolve them one at a time until the destination is reached.") with
    {
        Sections = [new(F.WayfinderChartAsYouGo, "Chart as you go"), new(F.WayfinderReferByName, "Refer by name"), new(F.WayfinderTheMap, "The map"), new(F.WayfinderTickets, "Tickets"), new(F.WayfinderTicketTypes, "Ticket Types"), new(F.WayfinderFogOfWar, "Fog of war"), new(F.WayfinderDiscoveryBeforeQuestion, "Discovery before Question"), new(F.WayfinderOutOfScope, "Out of scope"), new(F.WayfinderInvocation, "Invocation"), new(F.WayfinderChartTheMap, "Chart the map"), new(F.WayfinderWorkThroughTheMap, "Work through the map")]
    };

    public static readonly SkillSpec WhatWouldBalazsDo = SkillSpec.Workflow(S.WhatWouldBalazsDo, "What Would Balazs Do", "A call Balazs would otherwise be asked. Use when an officer must decide one while he is away, prepare a DR-grade or irreversible one for him, or when he asks for it on any call.") with
    {
        Sections = [new(F.WhatWouldBalazsDoMustReads, "Must-Reads"), new(F.WhatWouldBalazsDoAuthority, "Authority"), new(F.WhatWouldBalazsDoWhen, "When"), new(F.WhatWouldBalazsDoCaseLaw, "Case law"), new(F.WhatWouldBalazsDoTribunal, "Tribunal"), new(F.WhatWouldBalazsDoVerdict, "Verdict"), new(F.WhatWouldBalazsDoRecord, "Record")],
        Resources =
        [
            new(WhatWouldBalazsDoResource.Tribunal, "Tribunal", [new(F.WhatWouldBalazsDoTribunalLead), new(F.WhatWouldBalazsDoTribunalPhases, "Phases"), new(F.WhatWouldBalazsDoTribunalSealedPosition, "Sealed position"), new(F.WhatWouldBalazsDoTribunalJudgeBrief, "Judge brief")])
        ]
    };
}

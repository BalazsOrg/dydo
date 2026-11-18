namespace DynaDocs.Tests.SkillBuild.Fixture.Kinds;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>capacity</c> claims, sub-topics budget to use.</summary>
public static class Capacity
{
    public static readonly Claim BudgetNativeCapacity = new(
        "Budget open native capacity before commissioning or resuming a captain (the captain does the same for its crew).",
        [A.Admiral, A.TicketCaptain],
        [
            new(S.Admiral, "**Native capacity:** Budget open native capacity before commissioning or resuming the captain from its record.", new(F.AdmiralBoundary, 5)),
            new(S.TicketCaptain, "Budget open native capacity and run necessary stages serially, preserving the fresh reviewer obligation.", new(F.TicketCaptainBoundary, 4), () => [Capacity.RunNecessaryStagesSerially!, DispatchOneToWithout.PreserveFreshReviewerObligation]),
            new(S.WorkingTreeStandard, "The Admiral budgets open native capacity before commissioning or resuming a captain.", new(F.WorkingTreeStandardDelegation, 1))
        ]);

    public static readonly Claim CapHitIsFinding = new(
        "A cap hit is a finding, never a reason to raise the cap.",
        [A.AnyAgent, A.Crew, A.Reviewer],
        [
            new(S.CodeWriter, "A cap hit is a finding; never raise the cap.", new(F.CodeWriterProve, 7)),
            new(S.Inquisitor, "A cap hit is a finding; never raise the cap.", new(F.InquisitorMethod, 7)),
            new(S.Reviewer, "Every probe and rerun runs capped: `[env MSBUILDDISABLENODEREUSE=1 DOTNET_CLI_USE_MSBUILD_SERVER=0 UseSharedCompilation=false] systemd-run --user --scope --slice=<agent slice> -p MemoryMax=4G -p MemorySwapMax=0 -p OOMPolicy=continue timeout 120 <cmd>`, a gate at 8G and `timeout 3600`; a cap hit is a finding, never a reason to raise the cap.", new(F.ReviewerMethod, 6), () => [Capacity.RunCommandsMemoryCapped!]),
            new(S.TicketCaptain, "A cap hit is a finding, never a reason to raise the cap.", new(F.TicketCaptainBoundary, 17))
        ]);

    public static readonly Claim Capacity429RetryNotBlocker = new(
        "A capacity 429 means retry in a few minutes, never a blocker.",
        [A.TicketCaptain, A.AnyAgent, A.Officer],
        [
            new(S.Admiral, "Every brief that sends out Codex agents names a cap: around 8 at once, toward 16 only while clean, fewer on `429 turn_rate_limit_exceeded` (\"Codex is at capacity\"); a capacity 429 means retry in a few minutes, never a blocker; never \"maximise parallelism\".", new(F.AdmiralBoundary, 12), () => [Capacity.CodexConcurrencyCap!, Capacity.NeverMaximiseParallelism!]),
            new(S.TicketCaptain, "Every brief that sends out Codex agents names a cap: around 8 at once, toward 16 only while clean, fewer on `429 turn_rate_limit_exceeded` (\"Codex is at capacity\"); a capacity 429 means retry in a few minutes, never a blocker; never \"maximise parallelism\".", new(F.TicketCaptainBoundary, 12), () => [Capacity.CodexConcurrencyCap!, Capacity.NeverMaximiseParallelism!]),
            new(S.WorkingTreeStandard, "A capacity 429 means retry after a few minutes, never a blocker.", new(F.WorkingTreeStandardDelegation, 18))
        ]);

    public static readonly Claim CodexConcurrencyCap = new(
        "Every brief that sends out Codex agents names a concurrency cap: about 8 at once, toward 16 only while runs stay clean, fewer or back off on 429 turn_rate_limit_exceeded.",
        [A.Admiral, A.TicketCaptain, A.Officer],
        [
            new(S.WorkingTreeStandard, "Every brief that sends out Codex agents names their concurrency cap: start around 8 at once, rise toward 16 only while runs stay clean, and back off on `429 turn_rate_limit_exceeded` (\"Codex is at capacity\").", new(F.WorkingTreeStandardDelegation, 17))
        ]);

    public static readonly Claim CommissionProviderModelSwarmWidth = new(
        "The Inquisition commission sets a provider and model for each phase and the swarm width, the most crew agents running at once.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "a provider and model for each phase, and the swarm width: the most crew agents running at once;", new(F.TicketCaptainInquisitionN1CommissionInputs, 2))
        ]);

    public static readonly Claim ContextCheckUnavailableOrError = new(
        "Where the context check is unavailable (no T3, not its own session), unknown host or an error, say so in the return context field and keep working under the other rules.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Without T3, or wherever you are not your own session, the check is unavailable: write `unavailable` in the return's context field and keep to the rest of these rules.", new(F.InquisitorContextSizeOwnSessionOnly, 2))
        ]);

    public static readonly Claim ContextSizeForInquisitionCrewOnly = new(
        "The context-size rule is for inquisition crew only.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "The rule is for inquisition crew only: you run long and autonomous, and nothing compacts your context for you.", new(F.InquisitorContextSizeLead, 2))
        ]);

    public static readonly Claim ContextSizeScriptInTempFile = new(
        "Save the context-size script to a temporary file outside the repository so it never enters a commit.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Save this script to a temporary file outside the repository, for example `<temp dir>/context-size.mjs`, so it never enters a commit: ```js\nimport fs from 'node:fs'; import path from 'node:path'; import os from 'node:os';\nconst e = process.env, last = (f, pick) => fs.readFileSync(f, 'utf8').trim().split('\\n').reverse().map(l => { try { return pick(JSON.parse(l)); } catch { return undefined; } }).find(v => v !== undefined);\nconst find = (dir, id) => { for (const d of fs.readdirSync(dir, { withFileTypes: true })) { const p = path.join(dir, d.name); if (d.isDirectory()) { const r = find(p, id); if (r) return r; } else if (d.name.includes(id) && d.name.endsWith('.jsonl')) return p; } };\nif (e.CLAUDE_CODE_SESSION_ID) {\n  const root = path.join(e.CLAUDE_CONFIG_DIR || path.join(os.homedir(), '.claude'), 'projects');\n  const u = last(find(root, e.CLAUDE_CODE_SESSION_ID), j => j.message?.usage);\n  console.log(u.input_tokens + u.cache_creation_input_tokens + u.cache_read_input_tokens);\n} else if (e.CODEX_THREAD_ID) {\n  const root = path.join(e.CODEX_HOME || path.join(os.homedir(), '.codex'), 'sessions');\n  console.log(last(find(root, e.CODEX_THREAD_ID), j => j.payload?.type === 'token_count' ? j.payload.info.last_token_usage.input_tokens : undefined));\n} else console.log('unknown host');\n```", new(F.InquisitorContextSizeInstallTheCommandOnce, 1))
        ]);

    public static readonly Claim ContextSizeTempDirQuotes = new(
        "Replace <temp dir> with the folder the script is saved in and keep the quotes, since a Windows temp path can hold spaces.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Replace `<temp dir>` with the folder you saved the script in, and keep the quotes, since a Windows temp path can hold spaces: ```bash\nnode \"<temp dir>/context-size.mjs\"\n```", new(F.InquisitorContextSizeRunItAfterEachWorkItem, 1))
        ]);

    public static readonly Claim CrewBriefNamesMemoryCap = new(
        "Every crew brief that runs code names the memory cap command: [env MSBUILDDISABLENODEREUSE=1 DOTNET_CLI_USE_MSBUILD_SERVER=0 UseSharedCompilation=false] systemd-run --user --scope --slice=<agent slice> -p MemoryMax=8G -p MemorySwapMax=0 -p OOMPolicy=continue timeout 3600 <cmd>, a probe at 4G with a short timeout, a gate lock outside the cap.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Every crew brief that runs code names the memory cap: `[env MSBUILDDISABLENODEREUSE=1 DOTNET_CLI_USE_MSBUILD_SERVER=0 UseSharedCompilation=false] systemd-run --user --scope --slice=<agent slice> -p MemoryMax=8G -p MemorySwapMax=0 -p OOMPolicy=continue timeout 3600 <cmd>`, a probe at 4G with a short timeout, a gate lock outside the cap; run your own the same way.", new(F.TicketCaptainBoundary, 13), () => [Capacity.RunCommandsMemoryCapped!])
        ]);

    public static readonly Claim CrewContextLimit110k = new(
        "The crew context limit is 110k tokens unless the commission set another; the brief states it.",
        [A.TicketCaptain, A.Inquisitor, A.Admiral],
        [
            new(S.Admiral, "Once the feature is integrated, file an Inquisition in `Backlog` with its feature SHA and cost, and put the commission question to the user: a provider and model for each phase (Read, Break, Drive), the swarm width, the Codex cap (start about 8 at once, toward 16 while runs stay clean, back off on a 429), and the crew context limit, 110k tokens by default.", new(F.AdmiralMethod, 56), () => [InquisitionAreaToFollow.CommissionInputs, InquisitionAreaToFollow.FileAfterIntegration]),
            new(S.Inquisitor, "The limit is 110k tokens unless the commission set another, and your brief states it.", new(F.InquisitorContextSizeLead, 1)),
            new(S.TicketCaptain, "the crew context limit, 110k tokens unless the commission set another;", new(F.TicketCaptainInquisitionN1CommissionInputs, 3))
        ]);

    public static readonly Claim DotnetEnvPrefix = new(
        "Use the bracketed env prefix for .NET commands, without the brackets.",
        [A.Reviewer, A.Inquisitor, A.Crew, A.AnyAgent],
        [
            new(S.CodeWriter, "Use the bracketed env prefix for .NET commands, without the brackets.", new(F.CodeWriterProve, 10)),
            new(S.Inquisitor, "Use the bracketed env prefix for .NET commands, without the brackets.", new(F.InquisitorMethod, 10)),
            new(S.Reviewer, "Use the bracketed env prefix for .NET commands, without the brackets.", new(F.ReviewerMethod, 9)),
            new(S.TicketCaptain, "Use the bracketed env prefix for .NET commands, without the brackets.", new(F.TicketCaptainBoundary, 16))
        ]);

    public static readonly Claim FinishedGroupShareReturnsToOfficer = new(
        "A finished group's share returns to the captain's officer, who may give it to a running group.",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.Admiral, "Above about 10 areas its captain releases with group Sub-tickets: commission one captain per group with the share of the width and Codex its Sub-ticket carries, give a finished group's share to a running group if you choose, and resume the parent when its groups close for the cross-group triage.", new(F.AdmiralMethod, 59), () => [InquisitionGroupToRead.GroupSplitThreshold, InquisitionGroupToRead.ParentMergesGroupRegisters]),
            new(S.TicketCaptain, "Split the commissioned swarm width and its Codex share between the groups, write each group's share on its Sub-ticket, and keep the totals inside the commission; a finished group's share returns to your officer, who may give it to a running group.", new(F.TicketCaptainInquisitionN4SplitAboveAbout10Areas, 3), () => [Capacity.SplitSwarmWidthBetweenGroups!])
        ]);

    public static readonly Claim KeepSwarmWidthRunning = new(
        "Keep up to the swarm width running, with Codex inside its cap.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Keep up to the swarm width running, with Codex inside its cap.", new(F.TicketCaptainInquisitionN6ScheduleThePipeline, 2))
        ]);

    public static readonly Claim NeverMaximiseParallelism = new(
        "A brief asks for a capped count, never for \"maximise parallelism\".",
        [A.TicketCaptain, A.Officer],
        [
            new(S.WorkingTreeStandard, "A brief asks for a capped count, never for \"maximise parallelism\".", new(F.WorkingTreeStandardDelegation, 19))
        ]);

    public static readonly Claim OmitSliceWhereNone = new(
        "Omit --slice where no agent slice exists.",
        [A.Reviewer, A.Inquisitor, A.Crew, A.AnyAgent],
        [
            new(S.CodeWriter, "Omit `--slice` where no agent slice exists.", new(F.CodeWriterProve, 9)),
            new(S.Inquisitor, "Omit `--slice` where no agent slice exists.", new(F.InquisitorMethod, 9)),
            new(S.Reviewer, "Omit `--slice` where no agent slice exists.", new(F.ReviewerMethod, 8)),
            new(S.TicketCaptain, "Omit `--slice` where no agent slice exists.", new(F.TicketCaptainBoundary, 15))
        ]);

    public static readonly Claim PastLimitStop = new(
        "Past the context limit, stop; the item just documented is the last.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Past your limit, stop: the item you just documented is your last. Done when you have either started the next item under the limit or stopped over it.", new(F.InquisitorMethod, 4))
        ]);

    public static readonly Claim ReadContextSizeAfterEachItem = new(
        "An inquisition crew agent reads its context size with the context-size resource after each work item, never once at the start.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "[context-size](resources/context-size.md), the command you run after each work item.", new(F.InquisitorMustReads, 3))
        ]);

    public static readonly Claim RunCommandsMemoryCapped = new(
        "Run every test, gate, probe and drive capped with systemd-run: MemoryMax=8G, MemorySwapMax=0, OOMPolicy=continue, timeout 3600, a probe at 4G with a short timeout.",
        [A.TicketCaptain, A.Reviewer, A.Inquisitor, A.Crew],
        [
            new(S.CodeWriter, "Run every test, gate and probe capped: `[env MSBUILDDISABLENODEREUSE=1 DOTNET_CLI_USE_MSBUILD_SERVER=0 UseSharedCompilation=false] systemd-run --user --scope --slice=<agent slice> -p MemoryMax=8G -p MemorySwapMax=0 -p OOMPolicy=continue timeout 3600 <cmd>`, a probe at 4G with a short timeout.", new(F.CodeWriterProve, 6)),
            new(S.Inquisitor, "Run every probe, test and drive capped: `[env MSBUILDDISABLENODEREUSE=1 DOTNET_CLI_USE_MSBUILD_SERVER=0 UseSharedCompilation=false] systemd-run --user --scope --slice=<agent slice> -p MemoryMax=8G -p MemorySwapMax=0 -p OOMPolicy=continue timeout 3600 <cmd>`, a probe at 4G with a short timeout.", new(F.InquisitorMethod, 6))
        ]);

    public static readonly Claim RunNecessaryStagesSerially = new(
        "Run necessary stages serially.",
        [A.TicketCaptain], []);

    public static readonly Claim SplitSwarmWidthBetweenGroups = new(
        "Split the commissioned swarm width and its Codex share between the groups, write each group's share on its Sub-ticket, and keep the totals inside the commission.",
        [A.TicketCaptain], []);

    public static readonly Claim SurveyorBulkReadsInOwnContext = new(
        "The bulk board and thread reads stay in the surveyor child's context.",
        [A.Admiral, A.Crew],
        [
            new(S.Admiral, "The bulk reads stay in the surveyor's context.", new(F.AdmiralCheckInLead, 2)),
            new(S.ChiefOfStaff, "Keep bulk board and thread reads in this child's context.", new(F.ChiefOfStaffNightSurveySurvey, 7))
        ]);

    public static readonly Claim UnderLimitStartNextItem = new(
        "Under the context limit, start the next item.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**Under the limit:** start the next item.", new(F.InquisitorContextSizeRunItAfterEachWorkItem, 2))
        ]);

    public static readonly Claim UseSessionAgentSlice = new(
        "Use your session's agent slice: the nearest .slice in /proc/self/cgroup within the user manager; a nearer session slice takes precedence.",
        [A.Reviewer, A.Inquisitor, A.Crew, A.AnyAgent],
        [
            new(S.CodeWriter, "Use your session's agent slice: the nearest `.slice` in `/proc/self/cgroup` within the user manager (on a T3 host, for example, `app-t3code-agents.slice`; a nearer session slice takes precedence).", new(F.CodeWriterProve, 8)),
            new(S.Inquisitor, "Use your session's agent slice: the nearest `.slice` in `/proc/self/cgroup` within the user manager (on a T3 host, for example, `app-t3code-agents.slice`; a nearer session slice takes precedence).", new(F.InquisitorMethod, 8)),
            new(S.Reviewer, "Use your session's agent slice: the nearest `.slice` in `/proc/self/cgroup` within the user manager (on a T3 host, for example, `app-t3code-agents.slice`; a nearer session slice takes precedence).", new(F.ReviewerMethod, 7)),
            new(S.TicketCaptain, "Use your session's agent slice: the nearest `.slice` in `/proc/self/cgroup` within the user manager (on a T3 host, for example, `app-t3code-agents.slice`; a nearer session slice takes precedence).", new(F.TicketCaptainBoundary, 14))
        ]);
}

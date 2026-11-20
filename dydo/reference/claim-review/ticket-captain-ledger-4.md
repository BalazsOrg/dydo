---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Ledger: ticket-captain, 3. Staff and dispatch

What became of each line of the legacy `ticket-captain` spine in this section, and of each resource it links.

`claim:Dispatch.Template`, kept in `claim:CaptainDispatch.Brief`, `claim:CaptainBriefs.Skills`, `claim:CaptainBriefs.NoPeek`

> Brief from the kind's dispatch template and name every skill the role requires with a slash: the template carries the default set, and you add the ticket's own skills; never read a crew skill to brief it.

`claim:Roster.Models`, kept in `claim:CaptainDispatch.Roster` (open, not rendered)

> The model roster is his to write; his text goes here.

`claim:Roster.Usage`, kept in `claim:CaptainDispatch.Usage` (open, not rendered)

> Read `dydo usage` and balance vendors for every seat you fill, sub-captains and crew as well as reviewers.

`claim:Roster.Seats`, kept in `claim:CaptainDispatch.Scale`

> Scale each round's seats with the breadth and depth of the work; there is no constant pair.

`claim:Brief.Lenses`, kept in `claim:CaptainBriefs.Lenses`

> Name the lenses in each brief, shifting up the tree: it works (with a security baseline), then it integrates, then optimal, secure, performant and in harmony; never review a first version as if it were final.

`claim:Brief.SpecInBrief`, kept in `claim:CaptainBriefs.Governing`

> Name the governing spec in each reviewer's brief, or say there is none.

`claim:Brief.ScoutPass`, kept in `claim:CaptainReview.NoLoop`

> A plan, spec or contract gets no review loop; give a spec at most one scout pass, chosen by risk, which returns Questions and is never repeated.

`claim:Dispatch.Channel`, kept in `claim:CaptainDispatch.Send`, `claim:CaptainSplit.Helpers`

> A Linear unit goes out by `delegate_task`; a native sub-agent never gets one, shares your checkout, and you commit its work.

`claim:Dispatch.Worktrees`, kept in `claim:CaptainDispatch.OwnWorktree`

> Give parallel lanes, and sub-tickets with children, their own worktree; serial hops share their ticket's.

`claim:Dispatch.Foreground`, dropped, it fails Handled: T3's own instructions to every agent already say it: a native child is awaited in the turn, and a background agent goes through `delegate_task`.

> Use a native foreground sub-agent only for a result you wait on within your turn, and stay in your turn until it arrives; start no native background sub-agent.

`claim:Dispatch.ParentNotActive`, dropped, it fails Need: Its reader is a native child, which gets no ticket and never loads this skill.

> A native child can't `delegate_task`: when it hits `parent_not_active`, ask your officer to make that dispatch.

`claim:Dispatch.CodexCap`, dropped, it fails Reason: Nobody knows what the cap counts or where 8 and 16 came from: a one-off turned into a rule (1ffbc4c8). CLIProxy carries the limits, and claim CaptainFinish.Limits covers the error.

> A brief that sends out Codex agents names a cap: about 8 at once, toward 16 only while runs stay clean, fewer on `429 turn_rate_limit_exceeded`, which means retry in a few minutes, never a blocker.

`claim:Project.Usage`, kept in `claim:CaptainFinish.Limits`

> Keep the work going through any usage limit, and never bring one to him.

`claim:Project.LanesFull`, kept in `claim:CaptainDispatch.Lanes`, `claim:CaptainDispatch.Order`

> Keep your level's lanes full, toward a soft target of four; when the mainline can't fill one, pull your own pickable side tickets first, then continuous tickets.

`claim:Project.PullRule`, kept in `claim:CaptainDispatch.Pull`

> Pull without asking only a ticket that is Mode AFK; a Bug or small Feature at Severity should or lower, or an unrated CI finding; two-way; no design call; oldest first.

`claim:Project.Pulled`, kept in `claim:CaptainDispatch.Pulled`

> A continuous ticket you pull follows your chain from then on.

`claim:Dispatch.Send`, kept in `claim:CaptainDispatch.Family`

> Seat any reviewer from a family other than the author's when possible, single seats included.

`claim:Dispatch.Council`, deferred: It leads to a council ticket, and the council-chair resource that ticket's captain needs is not rebuilt in DYD-1372.

> Convene a council only for a heavy PR, one that meets the stakes or value test: stakes is the last check before main or a mistake expensive to undo, and value is a call between good options where what he would choose is the question.

`claim:Dispatch.HighStakes`, deferred: It leads to a council ticket, and the council-chair resource that ticket's captain needs is not rebuilt in DYD-1372.

> A high-stakes Review is guard or security work, a base other tickets build on, or the late whole-system simplify review.

`claim:Dispatch.SelfAuthor`, kept in `claim:CaptainSplit.Small`, `claim:CaptainReview.Risk`

> Author a change yourself only when it is cheaper to do than to brief and too small to need its own review; persistence, migrations, permissions, guard rules and uncertain native interfaces always go to crew, and a sub-captain authors nothing.

`claim:Dispatch.Return`, kept in `claim:CaptainDispatch.Resume`

> Every crew return comes back to you: post its hop's SHA on the hop as the resume point.

`claim:Report.Route`, kept in `claim:CaptainDispatch.Route`

> Route an out-of-scope report from your crew to the owner's ticket or a new Bug.

Resource `resources/implementer-brief.md` (Implementer brief), kept as `resources/briefs.md`

Resource `resources/reviewer-brief.md` (Reviewer brief), kept as `resources/briefs.md`

Resource `resources/research-brief.md` (Research brief), kept as `resources/briefs.md`

Resource `resources/scout-brief.md` (Scout brief), kept as `resources/research.md`

Resource `resources/ticket-captain-brief.md` (Ticket-captain brief), deferred: What every brief names is `resources/briefs.md`; its table of a job ticket's skill belongs with job tickets, not rebuilt in DYD-1372.

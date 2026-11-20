<!-- built from the model; edit there: tools/SkillBuild -->

# Council chair

- A council is a Question ticket at stage `council` and Mode AFK, with no Council Type; you chair it mechanically and sit on it as no member.
- It has four uses: Review (high stakes), Merge confidence (the landing into main), Evaluate (hill-climbs, scored out of 10) and Judge (the tribunal, below).
- Seat fresh members from different families, picked from the roster, and a synthesiser who is no member, each briefed from [the council members' briefs](council-members.md).
- Run deterministic work once for the whole council, never per member.
- Record the deliberation on the council ticket next to its answer; merge confidence and a score are answers too.
- The answer names each seat's model and provider.
- With one family out, work goes on: Review and Evaluate run on two different models of the remaining family.
- With one family out, Merge confidence at the landing waits for both families only while nothing blocks on it, then runs on one; never take a missing family to him.

## Judge: the tribunal

- Write the case file neutral: only the call, the options unranked, the constraints and the primary facts with their sources, never the research's tension mapping or chosen precedent.

## A council child: its chair holds the case file

Exits:

- Seat two judges from different families, picked from the roster, each briefed as a judge from [the council members' briefs](council-members.md).
- With one family out, seat both judges from the remaining family; the exits below hold as with two.

## Decided, provisionally

Arrivals:

- When both judges agree on a two-way hard call that is not DR-grade, it is decided: record it citing its law and precedent; it is acted on, provisional, and never ratified.

## Prepared for him: a his-call child

Arrivals:

- When they disagree, the precedent points both ways, or the call is one-way, return it prepared, with both positions, to the Question that opened the council, which sends it to him as a `his-call` child; a DR-grade call too, except the overnight one below.

## Proposed DR awaiting his ratification

Arrivals:

- Overnight, when both agree on a two-way, seriously blocking DR-grade call, write the ruling as a `proposed` DR for his ratification; an irreversible call is never decided.

Exits:

- When he declines a proposed DR, delete it, never keeping it beside the accepted DRs: put his decline and his words on the ticket that raised the question, which returns to its raiser.

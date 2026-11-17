<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->

# Morning brief

## overnight

- If blocked, report shutdown incomplete with the observed mode and exact human-terminal action; a stopped watch does not establish mode off.
- Fill this template in the conversation after verified shutdown.
- Complete `overnight-mode` shutdown first: wait for `dydo mode set overnight off` to return its result, then run `dydo mode show` in a separate later tool call, never in parallel, and require `overnight: off`.

## return

- <concrete action or decision; context and why it needs you>
- **Decided:** <title> (<ticket>): <context>; decided <what>, because <why>.
- Ratify or reverse.
- <title> (<PR link>): <result>.
- <next action and owner>
- **Open:** <title> (<ticket>): <context, options and consequences>.
- <result; at most three result lines total>
- Review <current verdict>; CI <state on current head>.
- <one real visual of the work touched>
- Recommend <answer>.

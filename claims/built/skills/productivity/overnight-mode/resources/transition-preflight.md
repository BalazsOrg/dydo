<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->

# Transition preflight

## overnight

- Confirm the agreed work and stop line before activation.
- If the value already matched before execution, report it as already on/off rather than claiming a new transition.
- Capture the action, result and fresh readback separately.
- Preserve the existing state and evidence.
- On any unexpected refusal, execution failure, missing readback or mismatched value, report the transition incomplete with the actual result and current observed state; if the readback failed, say the state is unknown.
- Return the blocker to the responsible captain or user with the exact action needed.
- Resolve the blocker before proceeding; every recovery still needs fresh readback.
- If the reminder applies to you, retry the same command once.
- Use `wizard` for terminal help when needed.
- A warning, retry, successful setter exit, schedule or message establishes no transition by itself.
- Wait for `dydo mode set` to return its result. Only then run `dydo mode show` in a separate later tool call, never in parallel with the setter.
- Run the applicable command only as the chief of staff readying overnight mode or ending it; otherwise stop.

```text
Start: dydo mode set overnight on
End:   dydo mode set overnight off
```

- The already-invoked chief of staff prepares startup and shutdown while the user is present.
- Read `dydo mode show` and record the observed value.
- Require the requested value before announcing activation or shutdown.

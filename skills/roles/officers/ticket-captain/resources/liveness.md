<!-- built from the model; edit there: tools/SkillBuild -->

# Liveness

## A liveness finding

Exits:

- Alive: comment `liveness <UTC time>: alive`, and the finding snoozes one window.
- Record behind: comment `liveness <UTC time>: record behind` and bring the record up to date (Done, Fix rN, …).
- Stopped: comment `liveness <UTC time>: stopped`, set the hop back to Todo, then resume the thread or dispatch a fresh agent; the old thread link stays as history.

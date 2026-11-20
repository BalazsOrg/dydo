<!-- built from the model; edit there: tools/SkillBuild -->

# Liveness

## A liveness finding

Exits:

- A liveness finding on a hop that is alive: comment `liveness <UTC time>: alive`, and the finding snoozes one window.
- A liveness finding on a hop whose record is behind: comment `liveness <UTC time>: record behind` and bring the record up to date (Done, Fix rN, …).
- A liveness finding on a stopped hop: comment `liveness <UTC time>: stopped`, set the hop back to Todo, then resume the thread or dispatch a fresh agent; the old thread link stays as history.

## Save, resume and end

- Save each finding as its `liveness <UTC time>: <outcome>` comment and resume the hop from its last posted SHA; end resolved once the hop is alive, up to date or restarted, or escalated to your officer with a hop you can't restart.

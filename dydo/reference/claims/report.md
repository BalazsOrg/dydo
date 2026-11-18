---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Report

## Up

`claim:Report.Up`: Any agent reports a problem outside its scope to its officer.

Spec: R5

**code-writer** (Working, nominal):

> Report a problem outside your scope to your officer.

**reviewer** (Round, nominal):

> Report a problem outside your scope to your officer.

**scout** (Working, nominal):

> Report a problem outside your question to your officer.

**ticket-captain** (Briefed, nominal):

> Report a problem outside your ticket to your officer.

## Route

`claim:Report.Route`: The officer routes an out-of-scope report to the owner's ticket or a new Bug.

Spec: R5

**admiral** (Watching, nominal):

> Route an out-of-scope report to the owner's ticket or a new Bug.

**ticket-captain** (Returned, nominal):

> Route an out-of-scope report from your crew to the owner's ticket or a new Bug.

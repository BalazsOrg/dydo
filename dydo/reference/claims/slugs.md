---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Slugs

## Valid

`claim:Slugs.Valid`: dydo check passes a DR whose applies: and weighed: slugs are valid.

Spec: L8

Transition in A new DR's slugs checked (`claims/statechart/slug-check/SlugCheck.tla`): Written → Valid, "every applies: and weighed: slug is valid: dydo check passes". A system transition: no skill carries it.

## Invalid

`claim:Slugs.Invalid`: An unknown or malformed slug fails dydo check.

Spec: L8

Transition in A new DR's slugs checked (`claims/statechart/slug-check/SlugCheck.tla`): Written → Invalid, "an unknown or malformed slug: dydo check fails". A system transition: no skill carries it.

## Corrected

`claim:Slugs.Corrected`: A DR whose slug failed is corrected and checked again.

Spec: L8

Transition in A new DR's slugs checked (`claims/statechart/slug-check/SlugCheck.tla`): Invalid → Written, "the slug is corrected; dydo check runs again". A system transition: no skill carries it.

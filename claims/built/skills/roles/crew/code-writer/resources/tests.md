<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->

# Good and Bad Tests

## evidence

- Asserting on call counts/order
- Mocking internal collaborators
- Your own modules and internal collaborators stay real.
- Testing private methods
- **Tautological tests**: Expected value restates the implementation, so the test passes by construction.
- At a boundary, inject the dependency, and give each external operation its own function (`getUser`, `createOrder`) rather than one generic fetcher, so each mock returns one shape and the setup holds no logic.
- At system boundaries only: external APIs, time and randomness, and sometimes the database or file system, where a test instance is better still.
- One logical assertion per test
- **Integration-style**: Test through real interfaces, not mocks of internal parts.
- Uses public API only
- Verifying through external means instead of interface
- Tests behavior users/callers care about
- Describes WHAT, not HOW
- Survives internal refactors
- **Implementation-detail tests**: Coupled to internal structure.
- Test breaks when refactoring without behavior change

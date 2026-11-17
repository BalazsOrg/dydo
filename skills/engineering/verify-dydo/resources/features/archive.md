# Archive before deletion and restore

Launch and doctor a fresh candidate with `verify-dydo`, then drive `--journey archive`.
The Windows driver owns a disposable repository and linked worktree, stages one file version,
changes its working bytes, adds ignored binary data and an empty directory, and runs the actual
archive command. It checks a nonempty complete entry, lists it, expires source reflogs, runs
`git gc --prune=now`, restores and checks both versions, files, directories and primary tip.
The raw argv, environment, exits and `archive-roundtrip.json` stay in RUN/evidence after cleanup.

This route proves local Windows recovery. The executable `safe-delete.feature` and named
`SafeDelete*Tests` own eligibility, reserves, corruption, races, retention, guards and remote
create-only policy. Local bare-remote tests inject GitHub eligibility. T3 activity injection tests
prove the policy seam; only the captain's disposable T3-host gate can prove thread health and
enable a supported release route. The production T3 policy currently retains recognized targets.

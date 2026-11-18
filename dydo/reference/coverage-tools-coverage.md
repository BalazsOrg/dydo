---
area: reference
type: reference
---

# Coverage Tools: Coverage Collectors

[Coverage and Assurance Tools](./coverage-tools.md)

## Coverage collectors

| Stack | Collector | Mechanism |
|---|---|---|
| `python` | `python-coverage` | coverage.py 7.16.0 with branch coverage, plus a flat per-process callable witness. `python_coverage.collect` writes `config.json`, the callable inventory of every target computed once (`inventory.pickle`, sealed with the SHA-256 of its pickled body and written only when it reads back through a process's own path as exactly the inventory computed, which a process reuses only for a source whose SHA-256 and compile state - interpreter build, `-O` level and debug ranges - match and whose entry is internally consistent, recomputing the rest; a missing, unreadable, damaged or malformed cache is a miss, never a startup failure; a cache deliberately rewritten and resealed is a forgery and out of scope, since whoever can write it can rewrite `sitecustomize.py`) and puts a generated `startup/sitecustomize.py` onto `PYTHONPATH`. `config.json` also names the collecting interpreter's own `purelib` and `platlib` as `packages`. A measured process searches them after its own path, and only while it imports coverage.py, so a process of the same build started outside its venv (`sys._base_executable`) imports the collector's coverage.py and is measured, then restores search-path precedence so unrelated packages resolve as they would unmeasured; instrumentation dependencies remain in `sys.modules`; a process that still cannot measure fails the campaign. It runs the ordinary unittest suite, then combines the `.coverage` data with every `counter-*.json` receipt. Each measured process opens one reporting connection at startup to the loopback port `collect` listens on, found in `DYDO_PYTHON_COVERAGE_FAILURES` or, without that variable, in the `port` of `config.json`, names itself on it and holds it until exit. An `Exception` during measurement startup, or any `BaseException` during its exit `finish`, goes on that connection, naming the pid, phase and command line, so it takes no new descriptor and does not depend on the output or on any parent passing the exit on; a clean exit closes the connection with nothing more, and a killed process's close counts as such. `collect` fails instead of combining the rest on any failure, on an unreadable or incomplete record, or on a connection still open 5 s after its command returned. Opening the connection is part of startup: a connect refused, reset, timed out after 0.1 s or short of an ephemeral port is retried three times within 0.47 s, and a process still without a connection, including one that cannot create its socket, find its port or import `python_coverage`, exits 70 at startup before its program runs, so no measured program runs without one; a gone reporting peer (for example `EPIPE`, `ECONNRESET`, `ECONNABORTED`, `EBADF` or `ENOTCONN`) also ends the report with exit 70. Recoverable zero-byte send errors (`ENOBUFS`, `EAGAIN`/`EWOULDBLOCK`, `ENOMEM` and `EINTR`, including their Windows socket equivalents) retry the full framed record for 0.25 s with 0.01 s backoff: these mean temporary buffer/memory pressure, would-block or interruption, not a gone peer. After that budget, or on an unclassified socket error, the sender tries a compact, named failure record. If even that cannot be sent, it stops sending and keeps the connection open until the collector closes it, so a command that returns fails after the settle; a parent still waiting for that child also waits for the collector to close. It never silently closes a healthy connection on retry exhaustion. Every other `BaseException` interrupting a failure send retries the full framed record for as long as interruptions recur, keeping the connection open: a partial record followed by a full one is unreadable and also fails the campaign; a process still reporting at command end fails the campaign after the settle. Once a full frame is sent, the connection closes and the first reporting interruption is re-raised, or the caller re-raises the original measurement failure if sending was uninterrupted. An interruption while opening or naming the reporting connection exits 70 before the program runs. A `BaseException` outside `Exception` (such as `KeyboardInterrupt` or `SystemExit`) during measurement startup propagates without a failure record: it aborts interpreter startup before the program runs, so it does not fail the campaign under a parent that ignores that exit. Three cases are the honest limit of what `collect` can observe: a killed child; a process that never ran its program whose parent ignores both its exit code and its missing output; and an asynchronous signal delivered while a failure report is in flight, which is in the same class as a killed child. A process without `DYDO_PYTHON_COVERAGE_CONFIG` stays unmeasured and silent: the generated `sitecustomize.py` then imports nothing |
| `node` | `javascript-coverage` | `node javascript_coverage.cjs` driving c8 12.0.0 with `--all --exclude-after-remap=false` over `node_tests.cjs`; Istanbul function and branch counters are joined to the ESLint callable inventory |
| `dotnet` | `csharp-coverage` | `run_tests.py --assurance-output <dir>`: one isolated Git worktree copy of the working candidate, one AltCover campaign inside a Windows Job object, evidence published back outside the snapshot |
| `viewer` | `typescript-coverage` | `pnpm -C viewer run coverage` with `--coverage.reporter=lcov`, `--coverage.reportsDirectory=<raw>/coverage` and a vitest JSON report added; `typescript_join.py` joins the Istanbul LCOV to the `typescript-source` walker's callables |
| `statechart` | `typescript-coverage` | `javascript_coverage.cjs --typescript-run` runs the package test script with source-stable Istanbul counters after Node strips TypeScript types without moving positions. A preload hook instruments only inventory targets and follows spawned Node processes through `NODE_OPTIONS`. Per-process JSON reports live in `<raw>/istanbul`; TAP records the suite verdict. `typescript_join.py` joins statements, branches and callable bodies to the existing walker, then `statechart_platform.py` partitions them. Native Node LCOV is retained as diagnostic evidence, not the policy denominator |

A maintained JavaScript file with no filename extension is recorded as a gap naming DYD-105, so the
row fails closed rather than quietly measuring less than the inventory.

The Python, JavaScript and TypeScript rows take their targets from the source inventory through
`gate_adapter._target_paths`: the stack's non-test rows, minus any row carrying `coverageExemption`.
`gate_inventory._coverage_exemption` writes that field, and the only rows that carry it are the two
external-host drivers of DR 048's Amendment 2026-09-17,
`DynaDocs.Tests/HostCanaries/run-host-canaries.mjs` and `openai-sse-provider.mjs`. The exemption is
coverage-scoped: both files are still measured by every static collector above. It also fails
closed - an unextracted or unassociated driver produces the gap `host-driver-logic-untested` or
`host-driver-unassociated` instead of the exemption, and the file returns to the coverage targets.

The viewer TypeScript join reads Istanbul LCOV per module: `DA` lines for the line floor, `BRDA` branches
for the branch floor, and `FN`/`FNDA` function counters. Node's reporter can give two functions the
same name; their `FNDA` counters follow their `FN` lines in order, so each pairs with its own line
rather than the last counter of that name. Each `DA` line belongs to the innermost
callable spanning it, so a component is charged neither the complexity nor the coverage of its
callbacks. A callable whose own Istanbul counter is zero covers none of its lines, even on a line
other code hit, and a callable with no line of its own is measured by that counter alone; with no
single counter on its line it is a gap. Top-level statements form a `<module>` callable scored by
the walker's module metrics, as Python's module entry is. A target absent from the LCOV is
declarative only when the walker finds no runtime statement in it. The viewer's `vite.config.ts`
keeps `src/api/types.ts` out of the report, so that file passes this way. Any other absent target
is a gap. A record for a source outside the repository, such as the managed Pele a statechart
test loads, is no target and is not read. The suite verdict is `suite_exit`,
taken from vitest's JSON report rather than the process exit, because the viewer's own
per-file coverage thresholds also fail that process. The statechart row takes it from the TAP
summary: a failed or cancelled test, or no test at all, fails the suite, and the row records the
skipped count beside it. A nonzero process exit that neither the suite nor a measured finding
explains is a gap.

Each row above is the stack's single suite execution under `--force-run`: the declaring stack's test
row is derived from it and launches nothing of its own. The C# campaign is Windows-only, so a Linux
host derives no verdict for `dotnet` and keeps that stack's separate test execution. The Python and
JavaScript collectors need `DynaDocs.Tests/coverage/requirements.lock` and
`DynaDocs.Tests/coverage`'s pnpm lock installed, or they cannot launch a suite at all. The TypeScript
collectors also need `viewer/node_modules` from `pnpm -C viewer install --frozen-lockfile` and the
statechart package's from `pnpm -C skills/engineering/statechart/scripts install --frozen-lockfile`;
the statechart static row needs both, since its typecheck compiles with the viewer's TypeScript.

`gate_policy.evaluate_policy` judges the joined modules of every stack the same way: line coverage
at least 80% and branch coverage at least 60% per module, HCRAP
(`cc² × (1 − covered/total)³ + cognitive`) at most 20 and cognitive complexity at most 20 per
callable, and at most seven parameters outside constructors. Rates are exact fractions, never
rounded percentages, and a module that claims to be non-executable may carry no methods, lines or
branches.

---

## Related

- [Coverage and Assurance Tools](./coverage-tools.md)

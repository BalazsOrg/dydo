<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->

# Context size

## capacity

- Without T3, or wherever you are not your own session, the check is unavailable: write `unavailable` in the return's context field and keep to the rest of these rules.
- **`unknown host`, or an error:** say so in the return's context field and keep working the area.
- The rule is for inquisition crew only: you run long and autonomous, and nothing compacts your context for you.

Save this script to a temporary file outside the repository, for example `<temp dir>/context-size.mjs`, so it never enters a commit: ```js
import fs from 'node:fs'; import path from 'node:path'; import os from 'node:os';
const e = process.env, last = (f, pick) => fs.readFileSync(f, 'utf8').trim().split('\n').reverse().map(l => { try { return pick(JSON.parse(l)); } catch { return undefined; } }).find(v => v !== undefined);
const find = (dir, id) => { for (const d of fs.readdirSync(dir, { withFileTypes: true })) { const p = path.join(dir, d.name); if (d.isDirectory()) { const r = find(p, id); if (r) return r; } else if (d.name.includes(id) && d.name.endsWith('.jsonl')) return p; } };
if (e.CLAUDE_CODE_SESSION_ID) {
  const root = path.join(e.CLAUDE_CONFIG_DIR || path.join(os.homedir(), '.claude'), 'projects');
  const u = last(find(root, e.CLAUDE_CODE_SESSION_ID), j => j.message?.usage);
  console.log(u.input_tokens + u.cache_creation_input_tokens + u.cache_read_input_tokens);
} else if (e.CODEX_THREAD_ID) {
  const root = path.join(e.CODEX_HOME || path.join(os.homedir(), '.codex'), 'sessions');
  console.log(last(find(root, e.CODEX_THREAD_ID), j => j.payload?.type === 'token_count' ? j.payload.info.last_token_usage.input_tokens : undefined));
} else console.log('unknown host');
```
```

Replace `<temp dir>` with the folder you saved the script in, and keep the quotes, since a Windows temp path can hold spaces: ```bash
node "<temp dir>/context-size.mjs"
```
```

- The limit is 110k tokens unless the commission set another, and your brief states it.
- **Over the limit:** stop. The item you just documented is your last; return with every item not yet done listed as handed back.
- An inquisition crew agent reads its own context size after each work item, and hands the rest of its area back once it is past its limit.
- The number is the model's input at its last call, so it trails your newest work by one call: read it after each work item, never once at the start.
- **Under the limit:** start the next item.

## dispatch

- The captain sends a fresh agent for the rest of the area.
- Inquisition crew run as their own sessions, dispatched through T3 `delegate_task` (DR 057), because they outlive the captain's turn.

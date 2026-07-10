# Harness 进度记录

> **仅追加会话日志** — 跨会话 pipeline 状态的单一事实来源。
> 每个 agent 在完成其阶段**之前**追加一条记录（使用下面匹配的模板）。
> 只有 orchestrator 在所有门禁 + CI 通过后，才会将任务标记为 `complete`。

## 会话日志

_(尚无条目 — orchestrator 和 subagents 会在运行期间追加到这里)_

---

## 模板

追加会话记录时，请使用与你的角色匹配的格式：

### Developer / Evaluator / Feature-Demonstrator（任务范围）

```
### [DATE] — [AGENT]: [Task ID] — [Task Title]
**Status:** completed | partial | blocked
**Changes:**
- [Description of what was produced or verified]
**Decisions:**
- [Any decisions made during this phase]
**Next:**
- [What the next agent/phase should focus on]
```

### Orchestrator（阶段转换）

```
### [DATE] — orchestrator: [Phase] — [Slug or summary]
**Status:** completed | blocked
**Outcome:**
- [What was produced — e.g. "requirements doc at .harness/requirements/foo.md"]
**Gate:**
- [Gate result if applicable — e.g. "combined review: APPROVED" or "user demo: approved"]
**Next:**
- [Next phase or agent to spawn]
```

### Analyst / Architect / Product-Owner（规划阶段）

```
### [DATE] — [AGENT]: [Slug]
**Status:** completed | partial | blocked
**Artifacts:**
- [Files created or updated]
**Open questions:**
- [Unresolved items, if any]
```

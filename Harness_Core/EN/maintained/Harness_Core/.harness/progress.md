# Harness Progress Notes

> **Append-only session log** — the single source of truth for pipeline state across sessions.
> Each agent appends one entry (using the matching template below) **before** completing its phase.
> Only the orchestrator marks a task `complete`, after all gates + CI pass.

## Session Log

_(no entries yet — the orchestrator and subagents append here during a run)_

---

## Templates

When appending session notes, use the format that matches your role:

### Developer / Evaluator / Feature-Demonstrator (task-scoped)

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

### Orchestrator (phase transitions)

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

### Analyst / Architect / Product-Owner (planning phases)

```
### [DATE] — [AGENT]: [Slug]
**Status:** completed | partial | blocked
**Artifacts:**
- [Files created or updated]
**Open questions:**
- [Unresolved items, if any]
```

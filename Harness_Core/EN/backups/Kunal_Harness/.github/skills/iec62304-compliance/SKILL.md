---
name: iec62304-compliance
description: IEC 62304 medical device software lifecycle compliance: safety class assignment (A/B/C), code review requirements for all classes, architecture documentation obligations, traceability from FR-XX requirements to tests. Use when working on safety-classified components or preparing regulatory documentation.
---

# IEC 62304 Compliance Skill

## Software Safety Classification

| Class | Risk of Harm | CT Context Examples |
|-------|-------------|---------------------|
| A | No injury possible | Log viewers, status-only displays, export utilities |
| B | Non-serious injury | Position reporting UI, telemetry logging |
| C | Serious injury or death | CT device control commands, safety interlocks |

**Most CT device control software is Class B or C.** When in doubt, classify higher.

## Required Lifecycle Activities by Class

| Activity | Class A | Class B | Class C |
|----------|---------|---------|---------|
| Software requirements (SRS) | ✓ | ✓ | ✓ |
| Software architecture (SAD) | ✓ | ✓ | ✓ |
| Detailed design (SDS) | ✓ | ✓ | ✓ |
| Unit tests | ✓ | ✓ | ✓ |
| Integration tests | ✓ | ✓ | ✓ |
| System tests | ✓ | ✓ | ✓ |
| Code review | ✓ | ✓ | ✓ |
| Full traceability | ✓ | ✓ | ✓ |

## Traceability Chain

Every requirement must trace the full chain:

```
FR-XX (SRS) → SAD component → ADR decision → Gherkin scenario → eval_feedback verdict
```

In this harness, the mapping is:
- **SRS** → `.harness/requirements/{slug}-requirements.md` (FR-XX, NFR-XX IDs)
- **SAD** → `.harness/architecture/adr/` (architectural decisions)
- **SDS** → `design_note` field in backlog task
- **Test Case** → Gherkin scenario name in `acceptance_criteria`
- **Verification Result** → `eval_feedback/{slug}_{task-id}.json` verdict

## Code Review Requirements (all classes)

- No dead code or unreachable branches
- No hardcoded values that should be configuration (file paths, magic numbers)
- Public API has XML documentation comments
- No compiler warnings suppressed without documented justification
- All public API parameters validated; invalid input causes a documented exception, not undefined behaviour
- Error paths handled and logged with full context — no silent failures
- No unhandled exceptions propagate as uncontrolled application crashes
- Safety-critical state changes have pre-condition and post-condition checks
- Concurrent access to safety-critical state protected by synchronisation primitives
- Anomaly severity: `critical` (Class C path) | `major` (Class B path) | `minor` (Class A, quality only)

## Architecture Documentation Requirements

- Every new architectural component states its safety class in the relevant ADR
- Components of different safety classes communicate through defined `ExtInf/` interfaces — no direct coupling
- Any decision affecting Class C behaviour references the SAD section it populates
- New ADRs for Class B/C decisions include: the risk being addressed, the mitigation, and the test approach

## Anomaly Management

Any defect found during `@dev-evaluator` evaluation must be recorded in the verdict JSON with:
- `defect_id` (e.g. `{slug}_{task_id}_D01`)
- The requirement ID it violates (`FR-XX` or `NFR-XX`)
- Severity: `critical` (Class C path) | `major` (Class B path) | `minor` (Class A, quality only)

Critical and major defects block task completion and must be resolved before moving to the next task.

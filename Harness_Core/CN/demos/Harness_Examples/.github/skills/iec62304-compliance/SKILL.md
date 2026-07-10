---
name: iec62304-compliance
description: IEC 62304 医疗器械软件生命周期合规：safety class assignment（A/B/C）、所有类别的 code review requirements、architecture documentation obligations、从 FR-XX requirements 到 tests 的 traceability。处理安全分级组件或准备 regulatory documentation 时使用。
---

# IEC 62304 Compliance Skill

## 软件安全分类

| Class | 伤害风险 | CT 上下文示例 |
|-------|-------------|---------------------|
| A | 不可能造成伤害 | Log viewers、status-only displays、export utilities |
| B | 非严重伤害 | Position reporting UI、telemetry logging |
| C | 严重伤害或死亡 | CT device control commands、safety interlocks |

**大多数 CT device control software 是 Class B 或 C。** 有疑问时，按更高等级分类。

## 各 Class 所需生命周期活动

| 活动 | Class A | Class B | Class C |
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

每条 requirement 都必须追踪完整链路：

```
FR-XX (SRS) → SAD component → ADR decision → Gherkin scenario → eval_feedback verdict
```

在此 harness 中，映射为：
- **SRS** → `.harness/requirements/{slug}-requirements.md`（FR-XX、NFR-XX IDs）
- **SAD** → `.harness/architecture/adr/`（architectural decisions）
- **SDS** → backlog task 中的 `design_note` 字段
- **Test Case** → `acceptance_criteria` 中的 Gherkin scenario name
- **Verification Result** → `eval_feedback/{slug}_{task-id}.json` verdict

## Code Review Requirements（所有 classes）

- 没有 dead code 或 unreachable branches
- 没有本应配置化的 hardcoded values（file paths、magic numbers）
- Public API 有 XML documentation comments
- 没有无 documented justification 的 compiler warnings suppression
- 所有 public API parameters 都已验证；invalid input 会导致 documented exception，而不是 undefined behaviour
- Error paths 被处理并带完整上下文记录日志 — 没有 silent failures
- 没有 unhandled exceptions 作为 uncontrolled application crashes 传播
- Safety-critical state changes 有 pre-condition 和 post-condition checks
- 对 safety-critical state 的并发访问由 synchronisation primitives 保护
- Anomaly severity：`critical`（Class C path）| `major`（Class B path）| `minor`（Class A，仅质量）

## Architecture Documentation Requirements

- 每个新 architectural component 都在相关 ADR 中说明其 safety class
- 不同 safety classes 的 components 通过定义明确的 `ExtInf/` interfaces 通信 — 无 direct coupling
- 任何影响 Class C behaviour 的决策都引用它所填充的 SAD section
- Class B/C decisions 的新 ADRs 包含：正在处理的 risk、mitigation 和 test approach

## Anomaly Management

`@dev-evaluator` 评估期间发现的任何 defect 都必须记录在 verdict JSON 中，并包含：
- `defect_id`（例如 `{slug}_{task_id}_D01`）
- 它违反的 requirement ID（`FR-XX` 或 `NFR-XX`）
- Severity：`critical`（Class C path）| `major`（Class B path）| `minor`（Class A，仅质量）

Critical 和 major defects 会阻塞任务完成，必须在进入下一个任务前解决。

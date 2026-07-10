# Code Review Checklist

八个 review dimensions 的详细 checklist。

---

## 1. Change Scope

**Goal**：理解变更内容并映射 blast radius。

| 检查项 | 详细信息 |
|-------|---------|
| List all changed files | 按 source、test、config、build、resource 分类 |
| Identify file roles | Model / View / ViewModel / Service / Utility / Test / Config |
| Map dependencies | 哪些 other modules consume 或 are consumed by changed code？ |
| Detect cross-project impact | 变更是否影响 shared libraries（例如 `CT_SW_Common`、`DirectResultPipeline`、`MIPPP`）？ |
| Detect mirrored files | 是否有 mirrored copies（per TICS rules）也需要更新？ |
| Note new files | 是否新增 files？它们是否位于正确 project/namespace/folder？ |
| Note deleted files | 是否删除 files？所有 references 是否已清理？ |

---

## 2. Fix Completeness

**Goal**：对于 bug-fix commits，验证 root cause 确实已解决。

| 检查项 | 详细信息 |
|-------|---------|
| Root cause identified | 是否清楚解释了 bug 的原因？ |
| Root cause addressed | code change 是否直接修复 root cause，而不是只修 symptom？ |
| Edge cases covered | fix 是否处理 boundary conditions、null inputs、concurrent access？ |
| Error paths handled | 修复后 exception/error paths 是否被正确处理？ |
| No partial state | 所有 scenarios 中系统是否保持 consistent state？ |
| Regression test | 是否有 test（new 或 existing）能捕获 recurrence？ |
| Related areas checked | codebase 其他地方是否存在相似 patterns 也受影响？ |

---

## 3. Feature Completeness

**Goal**：对于 feature commits，验证所有 requirements 已实现。

| 检查项 | 详细信息 |
|-------|---------|
| Acceptance criteria met | implementation 是否满足所有 stated requirements？ |
| UI/UX complete | 所有 UI elements、bindings 和 interactions 是否已接线？ |
| Data flow complete | 完整 data pipeline（input → processing → output）是否已实现？ |
| Integration wired | 所有 integration points（events、services、DI registrations）是否已连接？ |
| Error handling | user-facing error messages 和 fallback behaviors 是否已实现？ |
| 配置 | 新 configuration items 是否已 documented and defaulted？ |
| Backward compatibility | feature 是否在需要时保持 backward compatibility？ |

---

## 4. Regression Risk

**Goal**：识别变更引入的新风险。

| 检查项 | 严重度 | 详细信息 |
|-------|----------|---------|
| 空值安全 | 关键 | 是否新增 nullable dereferences 且没有 null checks？ |
| Thread safety | 关键 | 是否有 shared mutable state 未同步访问？ |
| Resource leaks | 关键 | 是否有 new IDisposable objects 未正确 dispose？ |
| Event subscription leaks | 高 | 是否有 new event subscriptions 没有对应 unsubscriptions？ |
| API contract breakage | 高 | 是否有 public/internal signature changes 会破坏 callers？ |
| Behavioral side effects | 高 | 变更是否改变无关 code paths 的 behavior？ |
| Performance regression | 中 | 是否有新的 O(n²) patterns、excessive allocations 或 UI thread blocking calls？ |
| Configuration sensitivity | 中 | 是否有本应 configurable 的 new hard-coded values？ |
| 吞掉异常 | 中 | 是否有 new catch blocks 静默吞掉 exceptions 且不 logging？ |
| Magic numbers/strings | 低 | 是否有新的 unexplained literal values？ |

---

## 5. Test Impact

**Goal**：将变更映射到 affected features，并推荐 test focus。

| 检查项 | 详细信息 |
|-------|---------|
| Directly affected features | 列出 code 被直接修改的 features |
| Indirectly affected features | 列出 consume 或 depend on modified code 的 features |
| Regression test scope | 应重新运行哪些 existing test suites？ |
| Manual test scenarios | 需要哪些 manual test scenarios（尤其是 UI changes）？ |
| Edge case test scenarios | 应特别测试哪些 boundary/edge-case scenarios？ |
| Integration test needs | 是否有 cross-module integration scenarios 需要验证？ |
| Performance test needs | 此变更是否需要 performance/load testing？ |

**Output format**：按优先级排列的 test plan：
- P0（commit 前必须测试）：Core scenarios directly affected
- P1（merge 前应测试）：Adjacent features and regression suite
- P2（推荐）：Broader integration and edge cases

---

## 6. Coding Standards

**Goal**：验证 TICS 和 CodeScene standards 合规性。

### TICS Checks (Philips C# Coding Standard 5.33)

| 规则领域 | 检查项 |
|-----------|-------|
| File header | Copyright header present (4@101) |
| XML docs | Public/internal types and members documented |
| 空值安全 | No unsafe nullable dereferences |
| Exception handling | No swallowed exceptions; sufficient logging context |
| 命名 | Follows project naming conventions |
| Type structure | One top-level type per file; file named after type |
| Namespace | Follows existing project namespace pattern |
| Accessibility | Fields private by default; types internal by default |
| Disposal | IDisposable implemented when owning disposables |
| Events | Null-check before raising; paired subscribe/unsubscribe |

### CodeScene Checks

| 指标 | 阈值 | 检查项 |
|--------|-----------|-------|
| Cyclomatic complexity | ≤ 9 per function | 是否有 function 超限？ |
| Function length | ≤ 20 statements preferred | 是否有 oversized functions？ |
| Nesting depth | ≤ 2 levels | 是否有 deeply nested blocks？ |
| 基本类型偏执 | ≤ 30% primitive params | 是否有 functions with too many primitives？ |
| 字符串参数过多 | ≤ 2 raw strings | 是否有 functions with string overload？ |
| ?????Bumpy Road? | ≤ 1 nested block per function | 是否有 functions with multiple nested blocks？ |
| Module mean complexity | ≤ 4.0 | File-level average complexity 是否可接受？ |

---

## 7. Architecture Conformance

**Goal**：验证变更遵守 project architectural rules。

完整 rule set 见 [architecture-rules.md](./architecture-rules.md)。

| 检查项 | 详细信息 |
|-------|---------|
| Layer violations | 变更是否引入 upward 或 cross-layer dependencies？ |
| Dependency direction | Dependencies 是否按正确方向流动（abstraction → implementation）？ |
| Circular dependencies | 变更是否创建新的 circular references？ |
| Separation of concerns | Business logic 是否保持在 UI/View layers 外？ |
| DI/IoC compliance | Dependencies 是否通过 injection，而不是 Service Locator resolve？ |
| Interface segregation | New interfaces 是否聚焦且不过宽？ |
| Shared library rules | 对 shared libraries 的变更是否保持 backward compatibility？ |
| Project boundary | Project-specific types 是否留在 project boundary 内？ |

---

## 8. Commit Log

**Goal**：生成清晰、conventional commit message。

Template 和 examples 见 [commit-log-template.md](./commit-log-template.md)。

| 检查项 | 详细信息 |
|-------|---------|
| Type prefix | 正确 type（fix/feat/refactor/chore/docs/test/style） |
| 范围 | 标识 affected module/component |
| Subject line | ≤ 72 characters, imperative mood, no period |
| Body | 解释 what and why（not how） |
| Footer | 适用时引用 ticket/issue ID |
| Breaking changes | API changed 时使用 BREAKING CHANGE footer |

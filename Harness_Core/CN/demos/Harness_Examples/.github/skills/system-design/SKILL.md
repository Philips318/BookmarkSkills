---
description: Architecture Decision Records (ADRs)、C4 component/container diagrams、interface contracts，以及 CT 项目的 design pattern selection。制定或记录 architectural decisions、创建 ADRs 或设计新 system components 时使用。
---

# System Design Skill

CT 项目的 Architecture Decision Records、C4 diagrams 和 design pattern guidance。

## Design Document Content Requirements

每个新 component design 必须包含（如果 < 10 个 classes，可跳过标为 optional 的项）：

| 工件 | 必需 |
|----------|----------|
| 用例图 — actor 和 use case | 始终 |
| Component diagram with UML relationships | If ≥ 10 classes |
| Sequence diagram at component level | If ≥ 10 classes |
| 类图（每张图最多 15 个类） | 始终 |
| 包含对象生命周期的类级 sequence diagram | 始终 |
| 状态图 | 使用状态机时 |
| Threading model — all synchronised blocks identified | When multi-threaded |
| Timing diagram | When timing NFRs are specified |
| Packaging/deployment diagram | When deploying new binaries |
| 测试策略 — unit、module、integration、NFR 层级 | 始终 |

## Interface Design Rules

- Interfaces 小且单一职责；名称使用名词或形容词短语
- 在 interface XML comments 中记录 NFR constraints（timing、thread-safety）
- 在 interface definition 中记录 method 可能抛出的所有 exceptions
- 明确说明 interface scope：within-module、within-subsystem 或 public (ExtInf)
- 绝不使用来自其他 components 的 private 或 internal interfaces
- Classes 只暴露其 interface 上声明的方法 — 没有额外 public methods

## Class Design Rules

- 一个 class 只有一个职责 — non-functional roles（facade、proxy、factory）也算职责
- Class 必须可测试：所有 dependencies 都可注入且可 mock，没有 static dependencies
- 优先组合而非继承；仅对 IS-A relationships 或 Template Method 使用继承
- 将 creation logic 与 business logic 分离 — 使用 factories 或 DI
- Constructors 只做构造 — 不放 business logic
- 遵循 Principle of Least Knowledge：不假设另一个 class 的 internals 或 config files

## Third-Party / 2nd-Party Integration

- 将交互限制在尽可能少的 classes 中
- 在以下情况使用 Bridge pattern：可预见 replacement、存在多个相似 components，或 component 未暴露 interface
- Domain 或 application layers 不直接引用 2nd-party internal types

## ADR Format

```markdown
# ADR-{NNN}: {Title}

## Status
Proposed | Accepted | Superseded by ADR-{NNN}

## Context
[1â€“3 paragraphs: the problem, constraints, and forces at play.
What architectural pressure requires a decision? What happens if we defer it?]

## Decision
[One clear statement: "We will use X for Y because Z."]

## Options Considered
| Option         | Pros                        | Cons                       |
|----------------|-----------------------------|----------------------------|
| Option A (chosen) | ...                      | ...                        |
| Option B       | ...                         | ...                        |

## Consequences
**Positive:**
- [What becomes easier or better constrained by this decision]

**Negative / Trade-offs:**
- [What becomes harder, more expensive, or more constrained]

## IEC 62304 Traceability
SAD-XXXX Â§{section} â€” [reference to the Software Architecture Document section this decision populates]
```

## C4 Model Diagrams (Mermaid)

### System Context (Level 1)
```
C4Context
  Person(operator, "CT Operator", "Controls CT device via UI")
  System(ctApp, "CT Control Application", "Manages CT device and measurement reporting")
  System_Ext(hw, "CT Hardware", "Physical device â€” RS-485 serial")
  Rel(operator, ctApp, "Uses")
  Rel(ctApp, hw, "Commands / reads via", "RS-485")
```

### Container (Level 2)
```
C4Container
  Container(ui, "WPF UI Layer", "C# WPF", "Operator-facing panels and controls")
  Container(app, "Application Layer", "C#", "Orchestrates device commands and data flow")
  Container(infra, "Infrastructure Layer", "C#", "RS-485 protocol, file I/O, logging")
  Rel(ui, app, "Calls via ICtDeviceService")
  Rel(app, infra, "Uses via IDeviceChannel")
```

## Layered Architecture Rules（项目特定）

```
UI Layer          â†’  Application Layer  â†’  Domain Layer  â†’  Infrastructure Layer
(WPF ViewModels)     (Service classes)     (Entities)        (IDeviceChannel impl)
```

## NuGet Package Separation

每个 repository 从其 build outputs 产出 4 个 packages：

| 包 | 来源 | 用法 |
|---------|--------|-------|
| `{repo}Inf.pkg` | `Output/OutInf/` | **Cross-repo dependency** — 其他 repos 唯一可引用的 package |
| `{repo}Impl.pkg` | `Output/OutImpl/` + `OutCfg/` + `OutRes/` | 仅 deployment — 绝不作为 dependency 引用 |
| `{repo}PostActions.pkg` | `Build/Actions/Install/` + `Uninstall/` | 仅 deployment |
| `{repo}Tests.pkg` | `Output/OutTests/` | 仅 test execution — 绝不作为 dependency 引用 |

**规则：**向另一个 CT repository 添加 dependency 时，只引用其 `{repo}Inf.pkg`。从另一个 repo 引用 `Impl.pkg` 是严重 architectural violation。

## Design Patterns Reference

| 模式 | 使用时机 |
|---------|---------|
| Repository | 将 data storage 抽象到 domain-facing interface 后面 |
| Command | 将 device operations 封装为 objects — 支持 undo、retry、queuing |
| Observer / Event | 将 position updates 流式发送给多个 UI consumers，避免耦合 |
| 策略 | 可替换 protocols — real RS-485 与 simulator 使用相同 `IDeviceChannel` |
| Factory | 从 configuration 创建 device channel instances，避免耦合到具体类型 |

## Safety Classification（IEC 62304）

每个新 architectural component 都必须声明其 safety class：

| Class | 伤害风险 | 对架构的影响 |
|-------|-------------|----------------------|
| A | 不可能造成伤害 | Standard review；unit tests sufficient |
| B | 非严重伤害 | Code review required；unit + integration tests |
| C | 严重伤害或死亡 | Formal code review；all test levels；traceability mandatory |

不同 safety classes 的 components 必须通过定义明确的 contracts 交互 — Class A 和 Class C components 之间不能 direct coupling。

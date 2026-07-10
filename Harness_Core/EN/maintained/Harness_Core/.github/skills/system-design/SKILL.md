---
description: Architecture Decision Records (ADRs), C4 component/container diagrams, interface contracts, and design pattern selection for the CT project. Use when making or documenting architectural decisions, creating ADRs, or designing new system components.
---

# System Design Skill

Architecture Decision Records, C4 diagrams, and design pattern guidance for the CT project.

## Design Document Content Requirements

Every new component design must include (skip items marked optional if < 10 classes):

| Artefact | Required |
|----------|----------|
| Use-case diagram — actors and use cases | Always |
| Component diagram with UML relationships | If ≥ 10 classes |
| Sequence diagram at component level | If ≥ 10 classes |
| Class diagram (max 15 classes per diagram) | Always |
| Sequence diagram at class level with object lifetimes | Always |
| State diagram | When state machines are used |
| Threading model — all synchronised blocks identified | When multi-threaded |
| Timing diagram | When timing NFRs are specified |
| Packaging/deployment diagram | When deploying new binaries |
| Test strategy — unit, module, integration, NFR level | Always |

## Interface Design Rules

- Interfaces are small and single-responsibility; name as noun or adjective phrase
- Document NFR constraints (timing, thread-safety) in interface XML comments
- Document all exceptions a method may throw in the interface definition
- State the interface scope explicitly: within-module, within-subsystem, or public (ExtInf)
- Never use private or internal interfaces from other components
- Classes expose only the methods declared on their interface — no extra public methods

## Class Design Rules

- A class has a single responsibility — non-functional roles (facade, proxy, factory) count
- A class must be testable: all dependencies injectable and mockable, no static dependencies
- Favour composition over inheritance; use inheritance only for IS-A relationships or Template Method
- Keep creation logic separate from business logic — use factories or DI
- Constructors do construction only — no business logic
- Follow the Principle of Least Knowledge: no assumptions about another class's internals or config files

## Third-Party / 2nd-Party Integration

- Restrict interaction to the minimum number of classes possible
- Use the Bridge pattern when: replacement is foreseeable, multiple similar components exist, or the component exposes no interface
- Do not reference 2nd-party internal types directly from domain or application layers

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

## Layered Architecture Rules (Project-Specific)

```
UI Layer          â†’  Application Layer  â†’  Domain Layer  â†’  Infrastructure Layer
(WPF ViewModels)     (Service classes)     (Entities)        (IDeviceChannel impl)
```

## NuGet Package Separation

Each repository produces 4 packages from its build outputs:

| Package | Source | Usage |
|---------|--------|-------|
| `{repo}Inf.pkg` | `Output/OutInf/` | **Cross-repo dependency** — the only package other repos may reference |
| `{repo}Impl.pkg` | `Output/OutImpl/` + `OutCfg/` + `OutRes/` | Deployment only — never referenced as a dependency |
| `{repo}PostActions.pkg` | `Build/Actions/Install/` + `Uninstall/` | Deployment only |
| `{repo}Tests.pkg` | `Output/OutTests/` | Test execution only — never referenced as a dependency |

**Rule:** When adding a dependency on another CT repository, reference its `{repo}Inf.pkg` only. Referencing `Impl.pkg` from another repo is a hard architectural violation.

## Design Patterns Reference

| Pattern | Use When |
|---------|---------|
| Repository | Abstracting data storage behind a domain-facing interface |
| Command | Encapsulating device operations as objects â€” enables undo, retry, queuing |
| Observer / Event | Streaming position updates to multiple UI consumers without coupling |
| Strategy | Swappable protocols â€” real RS-485 vs. simulator uses the same `IDeviceChannel` |
| Factory | Creating device channel instances from configuration without coupling to concrete types |

## Safety Classification (IEC 62304)

Every new architectural component must declare its safety class:

| Class | Risk of Harm | Impact on Architecture |
|-------|-------------|----------------------|
| A | No injury possible | Standard review; unit tests sufficient |
| B | Non-serious injury | Code review required; unit + integration tests |
| C | Serious injury or death | Formal code review; all test levels; traceability mandatory |

Components of different safety classes must interface through defined contracts â€” no direct coupling between Class A and Class C components.

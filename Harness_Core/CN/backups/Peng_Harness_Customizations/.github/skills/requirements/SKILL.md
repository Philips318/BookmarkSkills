---
name: requirements
description: '将模糊的一行需求结构化为完整 requirements package：User Story、Acceptance Criteria（Given/When/Then）、Non-Functional Requirements、constraints、risks 和 BDD Gherkin scenarios。面向 IEC 62304 medical device software 设计。'
argument-hint: 'Provide a requirement description, module name, and optionally the IEC 62304 safety class (A/B/C)'
user-invocable: true
---

# Requirements Structuring

使用此 skill 将模糊的一行需求描述转化为结构化、可测试的 requirements package，供 SE、developers 和 QA 直接使用。

## When to Use

- SE 收到简短需求，需要扩展为完整 specification。
- Three Amigos session 前 — 准备结构化 draft 供讨论。
- 检查 existing requirement 的 completeness 和 consistency。
- 根据 acceptance criteria 生成 BDD Gherkin scenarios。

## Preferred Inputs

提供以下一项或多项：

- **Requirement description** — 即使是一句话也足够。
- **Module / component name** — 例如 "MIA Tissue Management"、"Volume Rendering"。
- **IEC 62304 safety class** — A / B / C。如果未指定，从 `domain-knowledge/safety-rules.md` triggers 推导；不确定时上调一级。
- **Existing AC or constraints** — 如果已有 partial specification。

起草前，**始终**查阅 `domain-knowledge`（ct-glossary、dicom-patterns、spectral-knowledge、clinical-workflow、safety-rules），以便：
- 验证 terminology（HU/keV/formal names，避免 marketing）
- 为 AC examples 提取真实 boundary values
- 识别可能转化为 ACs 或 Out-of-Scope items 的 safety red lines

## Output Structure

此 skill 产出与 RequirementsAnalyst agent template 匹配的 requirements package。Sections **必须按此顺序出现**，以便 downstream agents 解析：

1. Stakeholders（table: role / name / concern）
2. Glossary（feature-local terms，补充 global `ct-glossary.md`）
3. User Story（`As a / I want / so that`）
4. Acceptance Criteria（AC-1、AC-2，…，Given/When/Then）
5. Non-Functional Requirements（所有 5 个 sub-categories — 见下方）
6. Verification Methods（每个 AC 一行：Precondition / Steps / Expected Result）
7. Constraints（Technical / Regulatory / Business）
8. Assumptions
9. Out of Scope
10. Open Questions（table: # / Question / Owner / Blocking?）
11. Risks（table: # / Risk / Likelihood / Impact / Mitigation）
12. Safety Classification（A / B / C），并带**引用 `safety-rules.md` 的 justification**
13. Skipped Artifacts（如有，说明原因）

### 3. User Story

Format: `As a [role], I want [goal], so that [benefit]`。使用具体 role（radiologist、technologist、service engineer、system），不要使用泛泛的 "user"。

### 4. Acceptance Criteria (AC)

每个 AC 使用 Given/When/Then format：

```gherkin
AC-1: [Short title]
  Given [precondition]
  When [action]
  Then [expected outcome]
```

覆盖：
- Normal flow（happy path）
- Alternative flows
- Error / exception flows
- Boundary conditions（使用来自 `domain-knowledge` 的 anchors，例如 keV 40–200、iodine accuracy at 5 mg/ml、EFOV 500 mm）

见 [requirements-checklist.md § Acceptance Criteria](./references/requirements-checklist.md#acceptance-criteria)。

### 5. Non-Functional Requirements (NFR)

所有五个 sub-categories 必须存在。不适用时写 `N/A — <one-line rationale>`；不要留空。

- **Performance**: response time, throughput, memory limits
- **Security**: authentication, authorization, data protection, PHI handling
- **Usability**: accessibility, localization, clinical workflow fit
- **Reliability**: error recovery, data integrity, failover, degraded mode（例如 Citrix）
- **Regulatory / Compliance**: IEC 62304 class, DICOM conformance statements, IFU obligations

### 6. Verification Methods

每个 AC 一行。只做 requirement-level（Tester agent 会在 Stage 4 中扩展为 detailed test cases）：

| AC | 前置条件 | Steps | 预期结果 |
|----|--------------|-------|-----------------|
| AC-1 | [setup] | 1. step<br>2. step | [observable outcome] |

### 7–9. Constraints / Assumptions / Out of Scope

标准 sections。Out of Scope 应明确指出该 feature **不**覆盖的 safety red lines（例如 "mammography workflow is out of scope — see safety-rules.md"）。

### 10–11. Open Questions and Risks

使用 tables。Open Questions 必须有 Owner 和 Blocking? flag。Risks 必须有 Likelihood 和 Impact rating。

### 12. Safety Classification

Format:
```
Safety Classification: [A | B | C]
Justification: [Quote the matching trigger from domain-knowledge/safety-rules.md]
```
不确定时上调一级。justification 必须引用具体 safety-rules.md 行，不要 paraphrase。

## Medical Device Compliance

- **Class A**: Standard output；impact analysis optional。如果跳过，在 §13 Skipped Artifacts 中记录原因。
- **Class B**: Companion `01-impact-analysis.md` 是 **mandatory**（见 `impact-analysis` skill）。
- **Class C**: Impact analysis + §11 Risks 中专用 patient-safety risk scenarios + 所有适用 `safety-rules.md` red lines 反映在 ACs 或 Out of Scope 中。

## Integration with bdd-generator and Verification

`bdd-generator` skill 消费此输出并要求：
- 每个 AC 至少有一个 BDD scenario（`.feature` file 末尾有 Coverage Matrix）
- Feature 携带与 §12 匹配的 `@class-A` / `@class-B` / `@class-C` tag
- Scenario Outline `Examples` tables 中的 boundary values 来自 `domain-knowledge`

`requirements-review` skill 检查上述 structure 和 integration contract。

## Quality Rules

- 每个 AC 必须可独立测试。
- 避免 vague terms："fast"、"user-friendly"、"reliable" — 对它们量化（见 `requirements-review` skill § Clarity table）。
- 使用 `domain-knowledge/ct-glossary.md` 中的 formal CT/DICOM/Spectral terminology，不用 marketing names 或 ad-hoc synonyms。
- 每个 NFR 必须有 measurable acceptance threshold（或带 rationale 的 `N/A`）。
- 如果输入过于模糊，无法产出高质量结果，填写 Open Questions table，并在声明 package ready 前询问 stakeholder。

## Portability Note

此 skill 具备 **team-portable** 性。13-section template、Given/When/Then format、NFR sub-categories 和 Safety Classification process 都是通用的，适用于任何 IEC 62304 medical-device project。Domain anchors 和 safety triggers 来自单独的 `domain-knowledge` skill，各团队可按 product line 自定义 — 无需修改此文件。Team-specific stakeholder lists、recurring acronyms 或 product-line-specific AC patterns 应放入 `references/requirements-checklist.md`（versioned）或 `/memories/repo/`（per-workspace）。

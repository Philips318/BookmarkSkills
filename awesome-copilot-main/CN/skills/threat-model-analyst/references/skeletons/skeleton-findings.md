#骨架：3-findings.md> **⛔复制下面的模板内容（不包括外部代码栏）。替换`[FILL]`占位符。所有10个属性行都是强制性的。按级别组织，而不是按严重性
> **⛔不要缩写属性名。使用EXACT名称：`SDL Bugbar Severity`（不是`Severity`），`Exploitation Prerequisites`（不是`Prerequisites`），`Exploitability Tier`（不是`Tier`），`Remediation Effort`（不是`Effort`），`CVSS 4.0`（不是`CVSS Score`）
> **⛔不要使用粗体内联标头（`**Description:**`）。使用`#### Description`标记h4标题
b> **⛔分级节标题必须是：`## Tier 1 — Direct Exposure (No Prerequisites)`，而不是`## Tier 1 Findings`.**

---```markdown
# Security Findings

---

## Tier 1 — Direct Exposure (No Prerequisites)

[REPEAT: one finding block per Tier 1 finding, sorted by severity (Critical→Important→Moderate→Low) then CVSS descending]

### FIND-[FILL: NN]: [FILL: title]

| Attribute | Value |
|-----------|-------|
| SDL Bugbar Severity | [FILL: Critical / Important / Moderate / Low] |
| CVSS 4.0 | [FILL: N.N] (CVSS:4.0/[FILL: full vector starting with AV:]) |
| CWE | [CWE-[FILL: NNN]](https://cwe.mitre.org/data/definitions/[FILL: NNN].html): [FILL: weakness name] |
| OWASP | A[FILL: NN]:2025 – [FILL: category name] |
| Exploitation Prerequisites | [FILL: text or "None"] |
| Exploitability Tier | Tier [FILL: 1/2/3] — [FILL: tier description] |
| Remediation Effort | [FILL: Low / Medium / High] |
| Mitigation Type | [FILL: Redesign / Standard Mitigation / Custom Mitigation / Existing Control / Accept Risk / Transfer Risk] |
| Component | [FILL: component name] |
| Related Threats | [T[FILL: NN].[FILL: X]](2-stride-analysis.md#[FILL: component-anchor]), [T[FILL: NN].[FILL: X]](2-stride-analysis.md#[FILL: component-anchor]) |

<!-- ⛔ POST-FINDING CHECK: Verify this finding IMMEDIATELY:
  1. ALL 10 attribute rows present (SDL Bugbar Severity through Related Threats)
  2. Row names are EXACT: 'SDL Bugbar Severity' (not 'SDL Bugbar'), 'Exploitation Prerequisites' (not 'Prerequisites'), 'Exploitability Tier' (not 'Risk Tier'), 'Remediation Effort' (not 'Effort')
  3. Related Threats are HYPERLINKS with `](2-stride-analysis.md#` — NOT plain text like 'T01.S, T02.T'
  4. CVSS starts with `CVSS:4.0/` — NOT bare vector
  5. CWE is a hyperlink to cwe.mitre.org — NOT plain text
  6. OWASP uses `:2025` suffix — NOT `:2021`
  If ANY check fails → FIX THIS FINDING NOW before writing the next one. -->

#### Description

[FILL-PROSE: technical description of the vulnerability]

#### Evidence

**Prerequisite basis:** [FILL: cite the specific code/config that determines this finding's prerequisite — e.g., "binds to 127.0.0.1 only (src/Server.cs:42)", "no auth middleware on /api routes (Startup.cs:18)", "console app with no network listener (Program.cs)". This MUST match the Component Exposure Table in 0.1-architecture.md.]

[FILL: specific file paths, line numbers, config keys, code snippets]

#### Remediation

[FILL: actionable remediation steps]

#### Verification

[FILL: how to verify the fix was applied]

<!-- ⛔ POST-SECTION CHECK: Verify this finding's sub-sections:
  1. Exactly 4 sub-headings present: `#### Description`, `#### Evidence`, `#### Remediation`, `#### Verification`
  2. Sub-headings use `####` level (NOT bold `**Description:**` inline text)
  3. No extra sub-headings like `#### Impact`, `#### Recommendation`, `#### Mitigation`
  4. Description has at least 2 sentences of technical detail
  5. Evidence cites specific file paths or line numbers (not generic)
  If ANY check fails → FIX NOW before moving to next finding. -->

[END-REPEAT]
[CONDITIONAL-EMPTY: If no Tier 1 findings, include this line instead of the REPEAT block]
*No Tier 1 findings identified for this repository.*
[END-CONDITIONAL-EMPTY]

---

## Tier 2 — Conditional Risk (Authenticated / Single Prerequisite)

[REPEAT: same finding block structure as Tier 1, sorted same way]

### FIND-[FILL: NN]: [FILL: title]

| Attribute | Value |
|-----------|-------|
| SDL Bugbar Severity | [FILL] |
| CVSS 4.0 | [FILL] (CVSS:4.0/[FILL]) |
| CWE | [CWE-[FILL]](https://cwe.mitre.org/data/definitions/[FILL].html): [FILL] |
| OWASP | A[FILL]:2025 – [FILL] |
| Exploitation Prerequisites | [FILL] |
| Exploitability Tier | Tier [FILL] — [FILL] |
| Remediation Effort | [FILL] |
| Mitigation Type | [FILL] |
| Component | [FILL] |
| Related Threats | [FILL] |

#### Description

[FILL-PROSE]

#### Evidence

**Prerequisite basis:** [FILL: cite the specific code/config that determines this finding's prerequisite — must match the Component Exposure Table in 0.1-architecture.md]

[FILL]

#### Remediation

[FILL]

#### Verification

[FILL]

[END-REPEAT]
[CONDITIONAL-EMPTY: If no Tier 2 findings, include this line instead of the REPEAT block]
*No Tier 2 findings identified for this repository.*
[END-CONDITIONAL-EMPTY]

---

## Tier 3 — Defense-in-Depth (Prior Compromise / Host Access)

[REPEAT: same finding block structure]

### FIND-[FILL: NN]: [FILL: title]

| Attribute | Value |
|-----------|-------|
| SDL Bugbar Severity | [FILL] |
| CVSS 4.0 | [FILL] (CVSS:4.0/[FILL]) |
| CWE | [CWE-[FILL]](https://cwe.mitre.org/data/definitions/[FILL].html): [FILL] |
| OWASP | A[FILL]:2025 – [FILL] |
| Exploitation Prerequisites | [FILL] |
| Exploitability Tier | Tier [FILL] — [FILL] |
| Remediation Effort | [FILL] |
| Mitigation Type | [FILL] |
| Component | [FILL] |
| Related Threats | [FILL] |

#### Description

[FILL-PROSE]

#### Evidence

**Prerequisite basis:** [FILL: cite the specific code/config that determines this finding's prerequisite — must match the Component Exposure Table in 0.1-architecture.md]

[FILL]

#### Remediation

[FILL]

#### Verification

[FILL]

[END-REPEAT]
[CONDITIONAL-EMPTY: If no Tier 3 findings, include this line instead of the REPEAT block]
*No Tier 3 findings identified for this repository.*
[END-CONDITIONAL-EMPTY]
```
在`3-findings.md`的末尾，附加威胁覆盖率验证表：```markdown
---

## Threat Coverage Verification

| Threat ID | Finding ID | Status |
|-----------|------------|--------|
[REPEAT: one row per threat from ALL components in 2-stride-analysis.md]
| [FILL: T##.X] | [FILL: FIND-## or —] | [FILL: ✅ Covered (FIND-XX) / ✅ Mitigated (FIND-XX) / 🔄 Mitigated by Platform] |
[END-REPEAT]

<!-- ⛔ POST-TABLE CHECK: Verify Threat Coverage Verification:
  1. Status column uses ONLY these 3 values with emoji prefixes:
     - `✅ Covered (FIND-XX)` — vulnerability needs remediation
     - `✅ Mitigated (FIND-XX)` — team built a control (documented in finding)
     - `🔄 Mitigated by Platform` — external platform handles it
  2. Do NOT use plain text like "Finding", "Mitigated", "Covered" without the emoji
  3. Do NOT use "Needs Review", "Accepted Risk", or "N/A"
  4. Column headers are EXACTLY: `Threat ID | Finding ID | Status` (NOT `Threat | Finding | Status`)
  5. Every threat from 2-stride-analysis.md appears in this table (no missing threats)
  If ANY check fails → FIX NOW. -->
```
**固定规则烘烤到这个骨架：**
-查找ID:`FIND-`前缀（从不查找`F-`，`F01`,`Finding`）
-属性名称：`SDL Bugbar Severity`，`Exploitation Prerequisites`,`Exploitability Tier`,`Remediation Effort`（精确-非缩写）
- CVSS：以`CVSS:4.0/`开始（从来没有裸矢量）
- CWE：超链接（非纯文本）
- OWASP:`:2025`后缀（从来不是`:2021`）
-相关威胁：单个超链接（非纯文本）
-子章节：`#### Description`，`#### Evidence`,`#### Remediation`,`#### Verification`-按层组织-没有`## Critical Findings`或`## Mitigated`区域
-完全3层的部分（都是强制性的，即使是空的“*没有发现N层的发现*”）
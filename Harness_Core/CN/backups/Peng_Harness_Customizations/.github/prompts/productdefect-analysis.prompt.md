---
name: Product Defect Analysis
description: '根据 Code Review Report 和 SSRS/SRS requirements，填写 Product Defect record（例如 CTD-style）的 Investigated section。产出覆盖全部 11 个 Investigated fields 的 self-contained HTML report。'
argument-hint: 'Provide the Code Review Report (path or text) and the SSRS/SRS requirements (path or text); optionally the Product Defect ID, title, and related defect IDs'
agent: 'agent'
---

完成 **Product Defect record** 的 **Investigated** section，并生成一个 **self-contained HTML report**。

使用以下 repository assets 作为 method、field guidance 和 template 的来源：

- [Skill definition](../skills/productdefect-analysis/SKILL.md)
- [Field Guide](../skills/productdefect-analysis/references/field-guide.md)
- [Report Template](../skills/productdefect-analysis/references/report-template.html)

## Context

这是 IEC 62304 / ISO 13485 下的 **Philips CT medical device** Product Defect investigation。分析必须将 defect 追溯到真实 failing SSRS/SRS requirement，并保持 technical answers 与 Code Review Report 一致。**绝不编造** requirement ID、release number 或 root cause — 无法推导的值标为 "to confirm"。

## Inputs

- **Code Review Report**：path 或 pasted content（root cause、fix、changed files、regression risk、test impact、residual risks、commit log）。通过 `file://` URL 使用 `fetch_webpage` 读取 HTML report，或使用 `read_file`。
- **SSRS / SRS requirements**：document path（`.doc`/`.docx`）或 pasted text。使用 PowerShell Word COM one-liner（见 skill）提取 failing `Requirement:` text、`Requirement ID:`、`Product:` 和 `Target Release:`。
- **Product Defect ID + title**（optional）以及共享 root cause 的 **related defect IDs**。

如果缺少任何 input，先询问再生成。

## Workflow

1. **Parse the Code Review Report**：提取 symptom、root cause、fix、changed files、regression risk、test impact、residual risks。
2. **Parse the SSRS/SRS**：定位 failing requirement（ID + text）、affected products、target release。
3. **Map shared root causes**：如果给出 related defect IDs，确认它们共享 root cause，并说明一个 fix 解决全部；建议链接 duplicates。
4. 按 [Field Guide](../skills/productdefect-analysis/references/field-guide.md) 填写 11 个 fields：
   1. Customer impact + work-around
   2. How to recover from failure mode
   3. Root cause-analysis
   4. Installed-base yes/no + oldest release + affected configurations
   5. Frequency of occurrence（Occurs every time / May occur / Not expected to occur）
   6. Proposed solution + technical risk + reliability impact（Change Point Analysis）
   7. DHF/DMR documents to create or modify
   8. Is testing required（reference the regression test + manual re-execution）
   9. IFU / SMI update required
   10. Investigator's advice
   11. Identify/Confirm failing product requirement（SSRS/SRS ID + text）
5. 从 [report template](../skills/productdefect-analysis/references/report-template.html) 构建 HTML report：替换每个 `{{PLACEHOLDER}}`；将 frequency 渲染为 highlighted selection；对 installed base 和 IFU 使用 yes/no pills；为任何 non-derivable value 添加 "to confirm" note。
6. **Save the HTML file**：写入 workspace root 下的 `PD Analysis/productdefect-analysis-{DEFECT_ID}.html`（如需要则创建 `PD Analysis` 目录），除非用户明确指定其他 output path。
7. **Summarize**：报告 file path，并列出 investigator 必须验证的 "to confirm" items。

## Output

一个 self-contained HTML file（light theme、inline CSS、no external assets），每个 Investigated field 对应一个 titled section，另有 footnote 引用 Code Review Report 和 SSRS references。
Default save location：workspace root `PD Analysis/` directory，filename 为 `productdefect-analysis-{DEFECT_ID}.html`，除非用户明确要求其他路径。

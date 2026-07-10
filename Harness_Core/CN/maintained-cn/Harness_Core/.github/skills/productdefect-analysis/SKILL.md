---
name: productdefect-analysis
description: '为 CT medical device software 生成 Product Defect Analysis（Product Defect record 的 "Investigated" section，例如 CTD-style defects）。输入 Code Review Report 和 SSRS/SRS requirements，产出结构化、自包含 HTML report，覆盖 customer impact、recovery、root-cause analysis、installed-base exposure、frequency、proposed solution、DHF/DMR impact、testing、IFU/SMI impact、investigator advice，以及 failing product requirement。'
argument-hint: 'Provide the Code Review Report (path or text) and the SSRS/SRS requirements (path or text); optionally the Product Defect ID and title'
user-invocable: true
---

# Product Defect Analysis

使用此 skill 填写 CT medical device software 的 Product Defect record 中的 **Investigated** section。给定 **Code Review Report**（technical root cause + fix）和 **SSRS/SRS requirements**（被违反的 requirement），它会生成完整、audit-ready 的 Product Defect Analysis，并以 **self-contained HTML report** 交付。

## When to Use

- code review 已识别 defect 的 root cause 和 fix，而你必须为 quality/regulatory sign-off 完成 Product Defect "Investigated" form。
- 多个 defect tickets 共享同一 root cause，每个都需要各自填写 analysis。
- 需要将 defect trace 到具体 failing SSRS/SRS requirement ID。
- 想要 consistent、reviewer-friendly HTML deliverable，而不是 free-text answers。

## Preferred Inputs

提供以下一项或多项：

- **Code Review Report**: HTML/Markdown/text report（path 或 pasted content）。root cause、fix、changed files、regression risk、test impact 和 commit log 的来源。
- **SSRS / SRS requirements**: requirements document path（例如 `.doc`/`.docx` SSRS）或 pasted requirement text。**failing product requirement**（ID + text）、affected products 和 target release 的来源。
- **Product Defect ID + title**（optional）: 例如 `CTD00025333 — 2 polyps were marked each time`。
- **Related defect IDs**（optional）: 共享相同 root cause 的其他 tickets。

如果缺少 input，请先询问再生成。不要 invent root cause 或 requirement ID。

## Reading the Inputs

- **Code Review Report (HTML)**: 使用 `fetch_webpage` 和 `file://` URL，或使用 `read_file`。提取：problem background / symptom、root cause、changed files、fix description/code、regression risk、test impact（existing/new tests）、residual risks 和 commit log。
- **SSRS/SRS (.doc/.docx)**: 通过 PowerShell Word COM one-liner 提取，然后搜索 feature keyword 以定位 `Requirement:` text、`Requirement ID:`、`Product:` 和 `Target Release:` lines。Example:

  ```powershell
  $word = New-Object -ComObject Word.Application; $word.Visible=$false
  $doc = $word.Documents.Open("<SSRS path>",$false,$true)
  $text = $doc.Content.Text; $doc.Close($false); $word.Quit()
  $lines = $text -split "`r"
  for($i=0;$i -lt $lines.Count;$i++){ if($lines[$i] -match "<keyword>"){
    $s=[Math]::Max(0,$i-4); $e=[Math]::Min($lines.Count-1,$i+4)
    for($j=$s;$j -le $e;$j++){ Write-Output $lines[$j].Trim() } } }
  ```

## Output Template (the 11 fields)

Report 必须按以下顺序回答每个 field。如何 source 和 word 每个 field，见 [field-guide.md](./references/field-guide.md)。

1. **Describe the impact including possible work-around for the Customer**
2. **How to recover the system back from failure mode?**
3. **Root cause-analysis of this problem**
4. **Is the issue present in the installed base (yes/no)** — if yes:
   - What is the oldest release that has this issue
   - List affected product configurations
5. **What is the frequency of occurrence?** — `Occurs every time` / `May occur` / `Not expected to occur`
6. **Proposed solution** (describe technical risk + reliability impact; consider Change Point Analysis)
7. **DHF/DMR documents to create or modify** — Requirements, IFU, Design-, Test- and/or Purchase Specs
8. **Is testing required?** — if not, give rationale; reference a test to re-execute
9. **Is an update of the IFU / SMI required** (add or remove content)
10. **My (Investigator's) advice**
11. **Identify/Confirm failing product requirement** (SSRS/SRS ID + text)

## Field Sourcing Rules

| 字段 | 主要来源 | 备注 |
|-------|----------------|-------|
| 客户影响 + 规避措施 | Code Review（症状） | 描述用户可见影响；给出手动规避措施，或说明不存在规避措施 |
| 从 failure mode 恢复 | Code Review（严重度） | 如果 app 保持稳定，说明无需恢复；否则给出恢复步骤 |
| 根因 | Code Review（root cause section） | 复用确切机制；引用已变更的方法/类 |
| installed base 是否受影响 + 最早版本 | SSRS（Target Release / Rationale）+ code history | 如果 bug 位于共享/既有代码，回答 **是**；如无法推导，最早版本标记为“待确认” |
| 受影响配置 | SSRS `Product:` line | 列出交付受影响组件的产品/配置 |
| 发生频率 | Code Review（reproducibility） | 如果是确定性问题，填写“每次都会发生”；说明选择理由 |
| 建议解决方案 + 风险 | Code Review（fix + regression risk） | 说明修复；风险通常为低；引用 Change Point Analysis 的影响范围 |
| DHF/DMR 文档 | Code Review（test impact） | 通常只更新 Test Spec；如果 requirement 仍有效，则 requirement/IFU 不变 |
| 是否需要测试 | Code Review（Test Plan / new tests） | 引用具体 regression test + manual re-execution |
| IFU / SMI 更新 | 缺陷性质 | 对于无用户说明变更的内部行为修复，填写“否” |
| 调查员建议 | 综合判断 | 接受/拒绝修复；pre-commit actions；链接重复 tickets |
| 失败需求 | SSRS（Requirement ID + text） | 引用 ID 和文本；如适用，添加相关 ID |

## Workflow

1. **Collect inputs**: confirm Code Review Report 和 SSRS 可用；询问任何 missing item。
2. **Parse the Code Review Report**: extract symptom、root cause、fix、changed files、regression risk、test impact、residual risks。
3. **Parse the SSRS**: locate failing requirement（ID + text）、affected products、target release。
4. **Map shared root causes**: 如果给出 related defect IDs，确认它们共享 root cause，并说明 one fix resolves all；建议 link duplicates。
5. **Fill the 11 fields**: 遵循 Field Sourcing Rules；把 inputs 无法推导的信息（例如 exact oldest release）标为 "to confirm"，不要猜测。
6. **Build the HTML report**: 使用 [report-template.html](./references/report-template.html)，替换每个 `{{PLACEHOLDER}}`。每个 field 成为 titled section；frequency choice 渲染为三个 options 中的 highlighted selection。
7. **Save the HTML file**: 写入 workspace root 下的 `PD Analysis/productdefect-analysis-{DEFECT_ID}.html`（如需要创建 `PD Analysis` directory），除非 user 明确指定其他 output path（例如 `PD Analysis/productdefect-analysis-CTD00025333.html`）。
8. **Summarize**: 报告 file path，并指出 investigator 必须 verify 的任何 "to confirm" items。

## Quality Rules

- **No fabrication**: 不要 invent requirement ID、release number 或 root cause。如果 inputs 不支持某值，写 "to confirm" 并说明需要什么。
- **Traceability**: failing requirement field 必须 quote 来自 input 的真实 SSRS/SRS ID + text。
- **Consistency with the fix**: proposed solution、frequency 和 testing answers 必须与 Code Review Report 实际内容一致（例如 deterministic symptom → "Occurs every time"）。
- **Self-contained HTML**: inline all CSS；不使用 external assets 或 network calls。
- **Language**: field answers 使用 clear English（template labels are English）。保持 investigator wording precise 且 review-ready。

## Related Skills

- `code-review` — 产出供此 skill 使用的 Code Review Report。
- `domain-knowledge` — 验证 impact/root cause 中使用的 CT/DICOM/Spectral terminology 和 safety wording。
- `impact-analysis` — 如果 defect 跨 modules，则进行 deeper upstream/downstream impact。
- `requirements-traceability` — 确认 testing field 中引用的 requirement-to-test trace。

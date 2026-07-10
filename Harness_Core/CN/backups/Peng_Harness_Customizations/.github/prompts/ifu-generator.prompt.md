---
description: '为 CT feature 生成 IFU (Instructions for Use) content。提供 requirements、test cases、clinical context，并可选提供 screenshot paths 或 workflow video。'
mode: 'agent'
tools: ['read_file', 'create_file', 'replace_string_in_file', 'list_dir', 'grep_search', 'semantic_search', 'run_in_terminal']
---

为提供的 CT feature 生成 Instructions for Use (IFU) content，并产出一个带 left navigation + right content 的 **self-contained HTML document**。

使用以下 repository assets 作为 conventions、safety templates 和 HTML structure 的来源：

- [IFU Style Guide](../skills/ifu-generator/references/ifu-style-guide.md) — Writing conventions、Word style mapping（fonts/sizes）
- [Warning/Caution/Note Classification](../skills/ifu-generator/references/warning-caution-note.md) — Signal word hierarchy、真实发布示例
- [CT Domain Knowledge](../skills/ifu-generator/references/ct-domain-knowledge.md) — CT glossary、真实 WARNING/CAUTION/NOTE examples、procedure step patterns、clinical context templates
- [HTML Template](../skills/ifu-generator/references/ifu-template.html) — Word-compatible HTML template（Trebuchet MS + Microsoft Sans Serif）
- IFU reference documents（现有 IFUs，用于 style reference）：`C:\Work\Code\Git_Code\IFU\`

## Context

这是 **Philips CT medical device** IFU。目标读者是：
- **CT Technologists**（放射科技师）— 执行扫描和后处理操作
- **Radiologists**（影像科医生）— 查看图像和诊断结果

IFU 必须符合 IEC 82079-1、IEC 62366 和 Philips documentation standards。

## Required Inputs（from user）

1. **Requirement**（SRS/SSRS）：包含 ID、text、classification（CTS/CTQ）、product、release 的 feature specification
2. **Test Cases**：包含 preconditions、steps 和 expected results 的 test procedures
3. **Clinical Use Description**：临床上为什么以及何时使用该 feature
4. **UI Screenshots**（optional）：用于嵌入的 screenshot 文件路径
5. **Workflow Video**（optional）：真实 workflow 的 screen-recording（`.mp4`、`.mov`、`.avi`、`.webm`、`.gif`）。分析前先预处理为 frames — 可补充/部分替代 UI-flow test cases 并提供 screenshots，但**不能**替代 requirement ID/text、classification、product/release、clinical purpose 或 safety judgment。先去除任何 patient data。

## Workflow

1. **Parse requirement**：提取 ID、text、classification、product line、target release。
2. **Pre-process workflow video**（如提供）：分析步骤前提取 frames 并推导 workflow：
   - 运行 [video pre-processing script](../skills/ifu-generator/references/Extract-VideoFrames.ps1)，将 key frames 提取为 PNG 到 `{feature_name}_frames/`。
   - 按时间顺序排列 frames，并重构 user workflow（actions、menu paths、UI state changes、result states）。
   - 将每个确认过的 frame 作为对应 step 的 screenshot；任何未确认的 UI element name 标为 `[TBD]`。
   - 嵌入前验证 frames 已 de-identified。
3. **Analyze test cases**：将 test steps 转换为 user-facing operation steps：
   - 移除 test-internal setup/teardown steps
   - 将 verification steps 转换为 "Result" statements
   - 将技术测试语言转换为面向用户的祈使句 instructions
4. **Generate safety signals**：
   - CTS requirement → 必须包含关于 safety implications 的 WARNING
   - CTQ requirement → 包含关于正确使用的 CAUTION
   - 使用 [Warning/Caution/Note Classification](../skills/ifu-generator/references/warning-caution-note.md) 中的 templates
5. **Write IFU content**：
   - **Overview**：结合 requirement purpose + clinical context
   - **Prerequisites**：来自 test preconditions（过滤为 user-relevance）
   - **Procedure**：来自 test steps（按 [Style Guide](../skills/ifu-generator/references/ifu-style-guide.md) 转换为 IFU style）
   - **Result**：来自 test expected outcomes
   - **Troubleshooting**：从 requirement scope 派生 common failure scenarios
6. **Handle screenshots**：
   - 如果提供 paths → 嵌入 `<img src="path" alt="description" />` 并加 figure captions
   - 如果未提供 paths → 插入 `<div class="ifu-figure-placeholder">[INSERT SCREENSHOT: description]</div>`
7. **Build HTML**：使用 [HTML template](../skills/ifu-generator/references/ifu-template.html)：
   - 填充所有 `{{PLACEHOLDER}}` values
   - 生成与 content headings 匹配的 navigation links
   - 确保所有 CSS/JS 均为 inline（self-contained）
8. **Save**：写入 workspace root 下的 `IFU/{feature_name}_ifu.html`（如需要则创建 `IFU` 目录），除非用户明确指定其他输出路径。

## Output

一个 self-contained HTML file（light theme），包含：
- Default save location：workspace root `IFU/` directory，filename 为 `{feature_name}_ifu.html`，除非用户明确要求其他路径。
- Left sidebar navigation（sticky、scroll-tracked）
- Right content area，包含：
  - Feature title + metadata（product、requirement ID、version、date）
  - Overview（purpose + clinical context）
  - Before you begin（prerequisites）
  - Feature-level warnings/cautions/notes
  - Numbered procedure steps（with screenshots or placeholders）
  - Result description
  - Troubleshooting table
  - AI-generated draft disclaimer

## Rules

- **This is a draft** — 始终包含 content requires Technical Documentation and Regulatory review 的 disclaimer。
- 所有 instructions 使用**祈使语气**（"Select"、"Click"、"Verify"）。
- system responses 使用**现在时**（"The system displays..."）。
- 将 system response 嵌入同一个 step sentence："Click X. The X interface opens."（不要另起一行）
- **One action per step** — 不要合并多个 actions。
- 所有 UI element names 都使用 **bold**。
- warnings 放在 hazardous step **之前**，绝不放在之后。
- **No emoji** in WARNING/CAUTION/NOTE — 只使用 signal word text（匹配 Word IFU style）。
- 使用 [CT Domain Knowledge](../skills/ifu-generator/references/ct-domain-knowledge.md) 中的正确 CT terminology — 绝不发明 abbreviations。
- 适用时包含 dosimetric units：CTDIvol (mGy)、DLP (mGy·cm)、keV。
- 不要发明 inputs 中找不到的 UI element names。
- 不要编造 clinical information — unknowns 标为 `[TBD]`。
- HTML output 必须使用 Word-compatible fonts（headings 用 Trebuchet MS，body 用 Microsoft Sans Serif），支持复制粘贴到 official IFU Word document。
- 不确定 hazard level 时，默认使用 WARNING（conservative approach）。
- HTML 必须完全 self-contained（inline CSS、inline JS、no external dependencies）。

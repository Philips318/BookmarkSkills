---
name: ifu-generator
description: '为 Philips CT medical device features 生成 Instructions for Use (IFU) content。输入 requirements、test cases、clinical context，以及可选 UI screenshots 或 workflow video — 产出 self-contained HTML document（with left navigation）和 Word (.docx) document。遵循 IEC 82079-1 和 Philips IFU conventions。'
argument-hint: 'Provide: 1) Requirement text, 2) Test cases/procedures, 3) Clinical use description, and optionally 4) UI screenshot paths or 5) a workflow video'
user-invocable: true
---

# IFU Generator

使用此 skill 为 Philips CT medical device features 生成 Instructions for Use（IFU）content。给定 product requirements、test procedures 和 clinical context 后，它会产出 professionally structured IFU documentation，并以 **two formats** 交付：interactive HTML page（用于 browser review）和 Word document（.docx，用于 direct editing and delivery）。

## When to Use

- 有 new feature requirement，需要为 technical writers 起草 IFU content。
- 需要为 radiologists/technologists 创建带 step-by-step instructions 的 operation procedures。
- 希望生成遵循 existing CT IFU documents style 和 structure 的 initial IFU content。
- 需要基于 requirement safety classification 确保 proper Warning/Caution/Note placement。

## Business Context

此 skill 为 **Philips CT medical devices** 生成 IFU content，面向：

- **CT Technologists**（放射科技师）— primary operator audience，执行 scans 和 post-processing
- **Radiologists / Imaging Physicians**（影像科医生）— reviews images、interprets results、uses advanced visualization

IFU 必须符合：
- **IEC 82079-1** — Preparation of information for use of products（safety information、structure）
- **IEC 62366** — Usability engineering for medical devices
- **FDA 21 CFR 801** — Labeling requirements for medical devices
- **Philips IFU Style Guide** — Internal documentation conventions

## Related Skill: domain-knowledge

编写 IFU content 前，查阅 `domain-knowledge` skill 以确保 terminology 和 clinical accuracy：

- **ct-glossary.md** — 使用 **formal feature names**（例如 "Precise Image" not "AI Recon"；"OnPlan" not "iStation"）、correct units（HU、mg/ml、keV、%EDW）和 correct abbreviations（HU/WW/WC/FOV/IPP/IOP/CTDIvol/DLP/SSDE）
- **clinical-workflow.md** — 与 official 11-step scanner workflow 和 ISP module structure 对齐 procedure steps
- **safety-rules.md** — 使用 listed warnings 作为 Warning/Caution placement 的 canonical text（例如 "VNC recommended for body scans only"、"Pediatric patients must use pediatric exam cards"、"Lossy compression must not be used for diagnosis"）
- **spectral-knowledge.md** — documenting spectral features 时使用 canonical result type table（units、accuracy thresholds、limitations）

这确保 IFU 匹配 existing Philips IFU corpus，并只使用 approved terminology。

### Product Scope

| Subsystem | IFU Coverage |
|-----------|-------------|
| **Console Software** | Post-processing applications（MPR、3D、DE、Rib、Spine、Endo、Spectral 等） |
| **Scan Workflow** | Protocol selection、scan planning、patient positioning |
| **Image Reconstruction** | Reconstruction parameters、spectral results |
| **Control Platform** | System management、configuration、patient administration |
| **CT Couch** | Patient table positioning、movement controls |

## Required Inputs

IFU generation 需要以下 inputs：

### 1. Requirement (SRS/SSRS)

定义 feature behavior 的 product requirement：

```
Requirement ID: CT-NMP.SSRS.Console.XXXX
Requirement text: The [application] shall [behavior] when [condition]
Classification: CTS / CTQ / Standard
Product: CT5200RT, CT7900, etc.
Target Release: VX.0
```

### 2. Test Cases / Test Procedures

Test cases 提供 operation steps 和 expected behavior：

```
Test Case ID: TC-XXXX
Precondition: [Required state before test]
Steps:
  1. [Action]
  2. [Action]
  ...
Expected Result: [What should happen]
```

### 3. Clinical Use Description

解释 feature 在 clinical practice 中 **why** 和 **when** 使用的 context：

```
Clinical context: [Description of clinical workflow and purpose]
Example: "Rib labeling is used during pre-surgical planning to identify and label
individual ribs for accurate surgical site identification in thoracic procedures."
```

### 4. UI Screenshots (Optional)

用于 procedure illustration 的 screenshots 或 image file paths：

- **Mode A**（paths provided）: 提供 PNG/SVG/JPG screenshots 的 file paths。skill 会用 `<img>` tags embed，并生成合适 captions。
  ```
  Screenshots:
    - Step 3: C:\Work\Screenshots\rib_toolbar.png (Rib labeling button in toolbar)
    - Step 5: C:\Work\Screenshots\rib_result.png (Labeled ribs in 3D view)
  ```
  
- **Mode B**（no paths）: skill 生成 text placeholders `[INSERT SCREENSHOT: description]`，供后续 manual replacement。

### 5. Workflow Video (Optional)

真实 CT Console workflow 的 screen-recording（`.mp4`、`.mov`、`.avi`、`.webm`、`.gif`）。视频在 IFU generation 前会 **pre-processed into frames and steps** — skill 不直接 consume raw video。

**How it is used:**

- **Frame extraction**: 使用 [video pre-processing script](./references/Extract-VideoFrames.ps1) 在 scene changes（或 fixed intervals）提取 key frames。Frames 保存为 PNG files。
- **Workflow derivation**: skill 分析 ordered frames，以推导 user workflow — user actions、menu paths、UI state changes 和 result states。
- **Screenshots**: Extracted frames 作为 IFU screenshots（Mode A）复用，并为每个 step 自动生成 captions。
- **Uncertainty handling**: 无法从 video 确认 UI element name 时，标记为 `[TBD]`，不要 invent。

**Important constraints:**

- video 可 **supplement or partially replace** UI-flow features 的 test cases（menu paths、enable/disable settings、scan planning、app launch、result review）。它**不能**替代：requirement ID/text、CTS/CTQ/Normal classification、product line/release、clinical purpose 或 safety-hazard judgment。
- **De-identify first**: 如果 recording 包含 patient information（name、ID、DOB、images），必须在 frames extracted 或 embedded 前 anonymize。
- 最可靠 input combination 是 `Requirement + Clinical Context + Workflow Video`，或更完整的 `Requirement + Test Case + Clinical Context + Workflow Video`。

## Output

每个 IFU 生成 **two files**：

### 1. HTML file (`{feature_name}_ifu.html`)

用于 **browser-based review** 的 self-contained HTML file。包含：

- **Left navigation panel**: Hierarchical TOC with scroll-tracking active state（screen-only；printing 时 hidden）
- **Right content pane**: Full IFU content（content 中 no metadata bar — metadata 只在 left nav 中）
  1. Feature title
  2. Overview（purpose + clinical context）
  3. Before you begin（prerequisites）
  4. Feature-level warnings/cautions
  5. Procedure（numbered steps with screenshots or placeholders）
  6. Result description
  7. Troubleshooting table
  8. Draft disclaimer

### 2. Word file (`{feature_name}_ifu.docx`)

用于 **direct editing and delivery** 给 Tech Doc team 的 Word document。使用 proper Word styles：

- 不需要从 HTML copy-paste — Word file ready to edit
- Proper heading styles（Heading 1–5）with Trebuchet MS font
- Body text in Microsoft Sans Serif
- WARNING/CAUTION blocks as styled paragraphs
- NOTE as bold-prefix body text
- Numbered procedure steps
- Troubleshooting as a Word table
- Draft disclaimer at the end

Word file 使用 Word COM automation（`Build-IFU-Word.ps1`）生成，并与 HTML file 保存在一起。

### Word Style Compatibility

HTML 使用能 1:1 map 到 Word styles 的 font/size combinations：

| HTML ?? | Word Style Target |
|---|---|
| `<h1>` Trebuchet MS 16pt Bold | Heading 1 |
| `<h2>` Trebuchet MS 12pt Bold | Heading 2 |
| `<h3>` Microsoft Sans Serif 11pt | Heading 3 |
| Signal label Trebuchet MS 10pt Bold | Heading 4 (WARNING / CAUTION) |
| Signal body Trebuchet MS 9.5pt Bold | Heading 7 |
| Body text Microsoft Sans Serif 9.5pt | 正文文本 |
| Procedure steps Microsoft Sans Serif 10pt | 列表段落 |
| NOTE: inline bold prefix | Body Text with bold prefix |

Review 后，tech doc team 直接编辑 Word file — 无需从 HTML copy-paste。

Files 直接保存到 workspace root 的 `IFU/` directory 下（如需要则创建），除非 user 明确指定其他 output path：
- `IFU/{feature_name}_ifu.html` — browser review
- `IFU/{feature_name}_ifu.docx` — Word editing & delivery

> **File Output Rule**: skill MUST create BOTH files on disk。HTML 使用 `create_file`。Word file 使用 PowerShell script with Word COM automation（via terminal run）。Do NOT output content in chat — files must be ready to open immediately。

> **No metadata bar in content area**: Product、Requirement、Release、Category、Date info 只放在 left nav panel（HTML）或 document properties（Word）。不要在 main content area 显示 metadata bar。

---

## Workflow

1. **Parse inputs**: Extract requirement ID、text、classification、product 和 release version。
2. **Pre-process workflow video**（if provided）: analyzing steps 前，extract frames 并 derive workflow：
   - Run [video pre-processing script](./references/Extract-VideoFrames.ps1)，将 key frames 提取为 PNG files 到 `{feature_name}_frames/` folder。
   - 按 chronological order 排列 frames，并分析以重建 user workflow（actions、menu paths、UI state changes、result states）。
   - 将每个 confirmed UI transition 当作 candidate procedure step；复用对应 frame 作为该 step 的 screenshot（Mode A）。
   - 无法从 frames 确认的 UI element name 标记为 `[TBD]`。
   - embedding 前确认 frames 已 de-identified（no patient name/ID/DOB/images）。
3. **Classify requirement by user interaction type**（critical step）:
   - **Interactive**: User performs actions → 生成带 numbered steps 的 **Procedure** sections
   - **Automatic/System behavior**: System acts automatically → 生成 **Reference/Description** sections（NO procedure steps）
   - **Mixed**: Some user actions trigger automatic behavior → 为 user actions 生成 procedure，inline 描述 automatic behavior
   
   **How to classify:**
   - 如果 requirement 说 "shall display/show/position/calculate/determine automatically" → **Automatic**
   - 如果 requirement 说 "shall allow/enable the user to..." 或 test cases 有 "Click/Select/Enter" → **Interactive**
   - 如果 test cases 主要是 "Verify/Observe/Check" 且无 user actions → **Automatic**（test verification ≠ user operation）
   
4. **Analyze test cases**: 将 test steps 映射为 user-facing operation steps：
   - Filter out internal/technical setup steps（test environment specific）
   - Convert test language to IFU language（imperative mood、user-facing）
   - Identify expected results for each step
   - **For automatic behavior requirements**: 从 test expected results 中提取 behavior description，但不要把 verification steps 转成 user procedures
4. **Determine safety signals**: 基于 requirement classification：
   - CTS → generate WARNING(s) based on the safety hazard
   - CTQ → generate CAUTION or NOTE as appropriate
   - Standard → generate NOTE if needed for correct operation
5. **Structure the content**:
   - 用 clinical context + requirement purpose 写 Overview
   - 从 test preconditions 写 Prerequisites
   - **Interactive**: 从 test steps 写 Procedure（converted to user language）
   - **Automatic**: 写 system behavior description + conditions table + NOTEs
   - 从 common failure scenarios 写 Troubleshooting
6. **Handle screenshots**:
   - Mode A: embed `<img>` with provided paths and auto-generate captions
   - Mode B: insert `[INSERT SCREENSHOT: ...]` placeholders at appropriate steps
7. **Build HTML**: 用 generated content 填充 [IFU template](./references/ifu-template.html)（content area 无 metadata bar）
8. **Save HTML**: 使用 `create_file` 写入 workspace root 下的 `IFU/{feature_name}_ifu.html`，除非 user 明确指定其他 output path。
9. **Generate Word**: 创建使用 Word COM automation 的 PowerShell script，在 workspace root 下用 proper Word styles（Trebuchet MS headings、Microsoft Sans Serif body）构建 `IFU/{feature_name}_ifu.docx`，然后 via terminal 运行。Word file 与 HTML 内容相同，但使用 native Word formatting。Reference script: `Build-IFU-Word.ps1`。

---

## IFU Chapter Structure

Chapter structure 取决于 feature 是 **interactive** 还是 **automatic**。

### Pattern A: Interactive Feature (user performs actions)

```
# [Feature Name]

## Overview
  - What it does (from requirement)
  - Why it's used (from clinical context)
  - When to use it (workflow position)

## Before you begin
  - Prerequisites (from test preconditions)
  - Required data/patient state
  - Required system state

## Warnings and precautions
  - Feature-level WARNINGs (for CTS features)
  - CAUTIONs (for CTQ features or equipment protection)
  - NOTEs (for correct operation guidance)

## [Procedure Title]
  1. Step 1 (action + inline system response)
  2. Step 2 (action + inline system response + screenshot)
  ...
  → Final result

## Troubleshooting
  | Symptom | Cause | Action |
```

### Pattern B: Automatic/System Behavior (no user action required)

```
# [Feature Name]

## Overview
  - What the system does automatically
  - Why this behavior exists (clinical rationale)
  - When it applies (which workflows / exam types)

## System Behavior
  - Description of automatic behavior
  - Conditions table: [Condition] → [System Behavior]
  - NOTEs explaining key details

## Related Workflow
  - Where in the workflow this behavior is visible
  - What the user will see (without asking them to "verify" it)

## Troubleshooting
  | Symptom | Cause | Action |
```

> **Key distinction**: Pattern A 有带 numbered steps（user actions）的 **Procedure** sections。Pattern B 有 description 和 conditions tables 组成的 **System Behavior** sections（no numbered action steps — user 不需要做任何事）。

---

## Writing Rules

### Step Conversion (Test Case → IFU)

| Test Case Language | IFU Language |
|-------------------|-------------|
| "Open the application under test" | "**Open** the [Application Name] from the **Applications** menu" |
| "Verify that X is displayed" | （变为 result statement: "→ The system displays X"） |
| "Set parameter A to value B" | "In the **[Field Name]** field, **enter** [value]" |
| "Wait for processing to complete" | "**Wait** for the processing to complete" + result |
| "Check that no error occurs" | （Omit — not user-relevant；或转换为 troubleshooting） |
| "Cleanup: close application" | （Omit — test teardown, not user instruction） |

> **Published IFU step pattern**: System response embedded in the same sentence, not a separate line:
> `Click Short Tube Conditioning. The Short Tube Conditioning interface opens.`
> NOT: `Click Short Tube Conditioning.` → `The interface opens.` (separate line)

### Warning Placement

- Feature-level warnings: **before** the procedure section
- Step-specific warnings: **immediately before** the hazardous step
- Never place warnings after the hazard

### Language Style

- **Imperative mood**: "Select the series"（not "You should select"）
- **Present tense for results**: "The system displays..."（not "will display"）
- **One action per step**: 每个 numbered step = one user action
- **Bold for UI elements**: **OK**、**Cancel**、**Tools > Rib Labeling**
- **Consistent terminology**: 同一个 UI element 全文使用同一个 name

完整 style rules 见 [IFU Style Guide](./references/ifu-style-guide.md)。

### Warning/Caution/Note Rules

见 [Warning/Caution/Note Classification](./references/warning-caution-note.md)：
- Signal word hierarchy（WARNING > CAUTION > NOTE）
- Three-part structure（Hazard + Consequence + Avoidance）
- CT-specific warning templates
- CTS/CTQ mapping to signal words

### CT Domain Knowledge

见 [CT Domain Knowledge](./references/ct-domain-knowledge.md)：
- **Glossary**: Standard CT terminology（SBI、MonoE、iDose4、O-MAR、EFOV、DRI、CTDIvol、DLP 等）
- **Real WARNING/CAUTION/NOTE examples** extracted from published IFU documents
- **Prerequisite patterns**: System state、patient setup、data/configuration、feature-specific
- **Procedure step patterns**: Real IFU step language（imperative、atomic、with inline system response）
- **Clinical use descriptions**: Templates for post-processing、scan workflow 和 dose optimization features
- **Product lines**: CT Verida Family、Rembra、Areta、system component names（Console、Gantry、CTBox、PIM 等）

生成 IFU content 时，**always consult this reference** 以：
1. 使用正确 CT terminology（never invent abbreviations）
2. 遵循 real IFU step patterns（"Click X. The Y interface opens."）
3. 使用 real WARNING patterns 作为 new warnings 的 templates
4. 包含正确 dosimetric units（CTDIvol in mGy、DLP in mGy·cm、keV）
5. 使用正确 system component names（Console、Gantry、CTBox 等）

---

## Important Rules

- **This is a draft generator** — 始终 include disclaimer，说明 content 需要 Technical Documentation 和 Regulatory Affairs review。
- **Do not fabricate clinical information** — clinical context 不足时，将 sections 标记为 `[TBD — requires clinical input]`。
- **Conservative safety approach** — hazard severity 不确定时，使用 WARNING（not CAUTION）。
- **Do not invent UI element names** — 使用 requirements/test cases 中的 names。未知时使用 `[Button Name]` 这类 descriptive placeholders。
- **Screenshot handling**: Mode A（paths provided）= embed images；Mode B（no paths）= generate placeholders。
- **One procedure per task** — 如果 requirement 覆盖多个 tasks，创建 separate procedure sections。
- **Keep steps atomic** — 每步最多 one action。
- **Do NOT turn test verification into user procedures** — 如果 test case 写 "Verify that X is displayed" 或 "Observe the preview image"，这是 test validation，不是 user action。对 automatic system behavior，描述 what happens（reference/description），不要指示 user 去 "verify" 或 "observe"。
- **Classify before writing** — 写 content 前必须判断 requirement 是 interactive 还是 automatic behavior。Interactive features 使用 Pattern A（procedure），automatic features 使用 Pattern B（system behavior description）。
- HTML 必须 fully self-contained（inline CSS、inline JS）。
- **Always save BOTH HTML and Word files to disk**。Never output content in chat — create both files directly。
- **No metadata bar in content area** — Product/Requirement/Release/Date info 只放 left nav（HTML）或 document properties（Word）。
- Word file 使用 Word COM automation（PowerShell）生成。Reference implementation 见 `Build-IFU-Word.ps1`。

---

## Related Skills

- [DFMEA Analysis](../dfmea-analysis/SKILL.md) — for risk analysis of the same requirements
- [Requirements](../requirements/SKILL.md) — for structuring requirements into acceptance criteria
- [Doc Generator](../doc-generator/SKILL.md) — for SDS and other technical documentation

## References

- [IFU Style Guide](./references/ifu-style-guide.md) — Writing conventions、Word style mapping、font/size specifications
- [Warning/Caution/Note Classification](./references/warning-caution-note.md) — Signal word hierarchy、HTML patterns、CTS/CTQ mapping
- [CT Domain Knowledge](./references/ct-domain-knowledge.md) — CT glossary、real WARNING/CAUTION/NOTE examples、prerequisite patterns、procedure step patterns、clinical context templates、product line reference
- [HTML Template](./references/ifu-template.html) — Word-compatible HTML template with Trebuchet MS / Microsoft Sans Serif fonts
- **Existing IFU reference documents** — 按以下顺序 auto-discover：
  1. workspace root 的 `IFU/` folder（多数 teams 将 IFU corpus 放在那里）
  2. workspace 内任何匹配 `**/IFU/` 的 folder（使用 `file_search`）
  3. `/memories/repo/ifu-corpus-path.md` 中记录的 path（per-workspace override）
  4. 如果都未找到，则只使用 `domain-knowledge` + inputs 从头生成 IFU，并在 draft disclaimer 中说明

## Portability Note

此 skill 在 Philips CT environments 内是 **team-portable**。IEC 82079-1 / IEC 62366 structure、Pattern A（Interactive）vs Pattern B（Automatic）classification、WARNING/CAUTION/NOTE rules 和 Word-style mapping 基于 Philips IFU conventions，同时遵循 international standards。Domain terminology（formal feature names、units、safety-warning canonical text）来自单独的 `domain-knowledge` skill — 另一个 team 的 domain skill 会自动供应不同内容。Team-specific items（actual IFU corpus location、internal review roles、product-line-specific templates）应放入 `references/`（versioned）或 `/memories/repo/`（per-workspace），不要 hardcode 在此处。

# IFU Style Guide — Philips CT Medical Device

Instructions for Use（IFU）writing conventions for Philips CT systems，符合 IEC 82079-1 和 Philips documentation standards。

> **Output Goal:** Generated HTML 必须在视觉上匹配 Philips IFU Word document styling，使 content 可从 HTML preview 直接 copy-paste 到 official IFU Word document，并只需 minimal reformatting。

---

## Word Style Mapping (Critical for Copy-Paste Compatibility)

HTML output 必须使用与 official Philips CT IFU Word template（例如 `300013585292_C_spectral_ct_Verida_Family_ifu_en-us.docx`）中 styles 对应的 fonts、sizes 和 structural patterns。

| HTML ?? | Word Style | Font | Size | Weight | 备注 |
|---|---|---|---|---|---|
| `<h1>` | Heading 1 | Trebuchet MS | 16pt | 加粗 | Chapter title |
| `<h2>` | Heading 2 | Trebuchet MS | 12pt | 加粗 | Section heading |
| `<h3>` | Heading 3 | Microsoft Sans Serif | 11pt | 常规 | Sub-section |
| `.signal-label` | Heading 4 | Trebuchet MS | 10pt | 加粗 | "WARNING" / "CAUTION" signal word |
| `<h5>` | Heading 5 | Trebuchet MS | 10pt | 加粗 | Procedure group title |
| `<h6>`, step text | Heading 6 | Microsoft Sans Serif | 10pt | 常规 | Sub-heading, numbered step |
| `.signal-body` | Heading 7 | Trebuchet MS | 9.5pt | 加粗 | WARNING / CAUTION body text |
| `<p>` body text | 正文文本 | Microsoft Sans Serif | 9.5pt | 常规 | General prose |
| `<li>` procedure | 列表段落 | Microsoft Sans Serif | 10pt | 常规 | Numbered procedure steps |
| `<td>` table cell | Table Paragraph | Microsoft Sans Serif | 9.5pt | 常规 | Table content |
| NOTE: inline | 正文文本 | Microsoft Sans Serif + bold prefix | 9.5pt | Mixed | `<strong>NOTE:</strong>` inline |

### Font Usage Rules

- **Trebuchet MS** — all headings（H1、H2）、signal word labels、signal word body text、steps 中 bold action verbs、table headers
- **Microsoft Sans Serif** — body text、procedure step descriptions、table cells、list items、figure captions
- Fallback stack: `"Trebuchet MS", "Segoe UI", sans-serif` and `"Microsoft Sans Serif", "Segoe UI", sans-serif`

### WARNING / CAUTION / NOTE HTML Patterns

**WARNING**（block — Word: Heading 4 label + Heading 7 body）:
```html
<div class="ifu-signal ifu-warning">
  <div class="signal-label">WARNING</div>
  <div class="signal-body">
    <p>[Hazard description]. [Consequence]. [Avoidance action].</p>
  </div>
</div>
```

**CAUTION**（block — same structure）:
```html
<div class="ifu-signal ifu-caution">
  <div class="signal-label">CAUTION</div>
  <div class="signal-body">
    <p>[Hazard description]. [Consequence]. [Avoidance action].</p>
  </div>
</div>
```

**NOTE**（inline — Word: Body Text with bold prefix）:
```html
<p class="ifu-note-inline"><strong>NOTE:</strong> [Informational text].</p>
```

> **IMPORTANT:** 不要使用 `<div class="signal-icon">⚠️</div>` 或 emoji in signal word blocks。Word IFU 不使用 emoji icons — bold signal word（"WARNING"、"CAUTION"）本身就是 visual indicator。

---

## Document Structure (Standard Chapter Sequence)

完整 CT IFU 通常遵循以下 chapter structure。并非每个 feature 都需要所有 chapters — skill 生成的是适配此整体结构的 **feature-level content**。

| # | Chapter | 用途 | Typical Content |
|---|---------|---------|-----------------|
| 1 | **About this document** | Scope and conventions | Intended audience, document conventions（icons, formatting）, related documents |
| 2 | **Safety** | Safety information overview | General safety warnings, contraindications, electromagnetic compatibility notes |
| 3 | **System overview** | Hardware/software orientation | System components, user interface overview, display layout |
| 4 | **Getting started** | Initial orientation | Login, startup, workspace selection, patient management |
| 5 | **[Feature chapters]** | Feature-specific instructions | One chapter per major feature or workflow（例如 "Spectral results", "Rib labeling", "MPR viewing"） |
| 6 | **Troubleshooting** | Problem resolution | Error messages, corrective actions, contact information |
| 7 | **Specifications** | Technical parameters | Performance specs, DICOM conformance, supported configurations |
| 8 | **Index** | Cross-reference | Alphabetical index of key terms |

---

## Feature Chapter Structure

每个 feature chapter（此 skill 的 main deliverable）遵循以下 internal structure：

```
## [Feature Name]
### Overview
  - Purpose / clinical context (1-2 paragraphs)
  - Intended use scenario
  - Prerequisites

### [Workflow / Task Name]
#### Before you begin
  - Prerequisites list
  - Required data / patient position
  
#### Procedure
  1. Step-by-step instructions (numbered)
  2. Each step = one user action
  3. Include expected system response after each action

#### Result
  - What the user sees after completing the procedure
  - How to verify correctness

### Warnings and precautions
  - Feature-specific warnings, cautions, notes

### Troubleshooting
  - Common issues specific to this feature
  - Error messages and corrective actions
```

---

## Writing Style Rules

### Language

| 规则 | 示例 |
|------|---------|
| Use **imperative mood** for instructions | "Select the rib label" (not "You should select...") |
| Use **present tense** for system responses | "The system displays the result" (not "will display") |
| **One action per step** | Split "Click OK and wait for processing" into 2 steps |
| Use **active voice** | "The system generates the report" (not "The report is generated") |
| Address user as **"you"** when needed | "Ensure you have selected the correct series" |
| Keep sentences **short** (≤ 25 words) | Break complex instructions into multiple sentences |
| Use **consistent terminology** | Always use the same term for the same UI element |

### Numbered Steps Format

```
1. **[Action verb]** [object] [location/qualifier].
   
   → [System response / expected result]

2. **[Next action verb]** [object].
   
   [INSERT SCREENSHOT: description of what is shown]
   
   → [System response]
```

**Action verbs**（standardized）:
- UI interactions: `Select`, `Click`, `Tap`, `Press`, `Drag`, `Scroll`
- Data actions: `Enter`, `Type`, `Specify`, `Set`
- Navigation: `Open`, `Close`, `Navigate to`, `Go to`
- Viewing: `Review`, `Verify`, `Check`, `Inspect`
- File operations: `Save`, `Export`, `Load`, `Import`

### Result Statement

Procedure 或 significant step 后，描述 user 应看到什么：

```
→ The system displays [description of result].
→ The [element] appears in [location].
→ A confirmation message appears.
```

---

## Terminology Conventions

### UI Element Naming

| 元素类型 | Format | 示例 |
|-------------|--------|---------|
| Button | 加粗 | **OK**, **Cancel**, **Apply** |
| Menu item | Bold with path | **Tools > Rib Labeling** |
| Tab / Panel | 加粗 | **Control Panel**, **Series List** |
| Field / Input | 加粗 | **Patient Name** field |
| Checkbox | Bold with state | Select the **Auto-detect** checkbox |
| Dropdown | 加粗 | From the **Orientation** list, select... |
| Keyboard key | Monospace/caps | Press `Enter`, Press `Ctrl+Z` |
| Icon | Describe + name | Click the **Edit** icon (pencil) |

### Clinical Terms

使用 standard radiology terminology。首次引入 term 时，提供简短解释：

```
The MPR (Multi-Planar Reconstruction) view shows...
```

---

## Figure / Screenshot Conventions

### Placement Rules
- 将 screenshot 放在它所说明 step 的**紧后方**
- 使用 callouts（numbered circles 或 arrows）突出 relevant UI elements
- 在下方加入 figure caption：`Figure X: [Description]`

### Figure Format (in HTML output)
```html
<figure class="ifu-figure">
  <img src="[path]" alt="[description]" />
  <figcaption>Figure 1: [Caption describing what is shown]</figcaption>
</figure>
```

### Placeholder Format (when no screenshot provided)
```
[INSERT SCREENSHOT: Brief description of the expected screenshot content]
```

---

## Cross-Reference Format

在 IFU 内引用其他 sections 时：

```
For more information, see "[Section title]" on page [X].
```

HTML output（with navigation）:
```html
For more information, see <a href="#section-id">Section title</a>.
```

---

## Table Format

表格用于：
- Parameter descriptions
- Option comparisons
- Keyboard shortcuts
- Error code listings

```html
<table class="ifu-table">
  <thead><tr><th>Parameter</th><th>Description</th><th>Range</th></tr></thead>
  <tbody>
    <tr><td>Window Width</td><td>Controls contrast range</td><td>1–4096</td></tr>
  </tbody>
</table>
```

---

## Conditional Content

当 procedure 因 product configuration 或 option 而不同：

```
> **Note:** This feature is available only with the [Option Name] license.

If [condition]:
1. Step for condition A...

If [alternative condition]:
1. Step for condition B...
```

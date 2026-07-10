# IFU Style Guide — Philips CT Medical Device

Instructions for Use (IFU) writing conventions for Philips CT systems, conforming to IEC 82079-1 and Philips documentation standards.

> **Output Goal:** The generated HTML must visually match the Philips IFU Word document styling so that content can be copy-pasted from the HTML preview directly into the official IFU Word document with minimal reformatting.

---

## Word Style Mapping (Critical for Copy-Paste Compatibility)

The HTML output must use fonts, sizes, and structural patterns that correspond to the styles defined in the official Philips CT IFU Word template (e.g., `300013585292_C_spectral_ct_Verida_Family_ifu_en-us.docx`).

| HTML Element | Word Style | Font | Size | Weight | Notes |
|---|---|---|---|---|---|
| `<h1>` | Heading 1 | Trebuchet MS | 16pt | Bold | Chapter title |
| `<h2>` | Heading 2 | Trebuchet MS | 12pt | Bold | Section heading |
| `<h3>` | Heading 3 | Microsoft Sans Serif | 11pt | Regular | Sub-section |
| `.signal-label` | Heading 4 | Trebuchet MS | 10pt | Bold | "WARNING" / "CAUTION" signal word |
| `<h5>` | Heading 5 | Trebuchet MS | 10pt | Bold | Procedure group title |
| `<h6>`, step text | Heading 6 | Microsoft Sans Serif | 10pt | Regular | Sub-heading, numbered step |
| `.signal-body` | Heading 7 | Trebuchet MS | 9.5pt | Bold | WARNING / CAUTION body text |
| `<p>` body text | Body Text | Microsoft Sans Serif | 9.5pt | Regular | General prose |
| `<li>` procedure | List Paragraph | Microsoft Sans Serif | 10pt | Regular | Numbered procedure steps |
| `<td>` table cell | Table Paragraph | Microsoft Sans Serif | 9.5pt | Regular | Table content |
| NOTE: inline | Body Text | Microsoft Sans Serif + bold prefix | 9.5pt | Mixed | `<strong>NOTE:</strong>` inline |

### Font Usage Rules

- **Trebuchet MS** — all headings (H1, H2), signal word labels, signal word body text, bold action verbs in steps, table headers
- **Microsoft Sans Serif** — body text, procedure step descriptions, table cells, list items, figure captions
- Fallback stack: `"Trebuchet MS", "Segoe UI", sans-serif` and `"Microsoft Sans Serif", "Segoe UI", sans-serif`

### WARNING / CAUTION / NOTE HTML Patterns

**WARNING** (block — Word: Heading 4 label + Heading 7 body):
```html
<div class="ifu-signal ifu-warning">
  <div class="signal-label">WARNING</div>
  <div class="signal-body">
    <p>[Hazard description]. [Consequence]. [Avoidance action].</p>
  </div>
</div>
```

**CAUTION** (block — same structure):
```html
<div class="ifu-signal ifu-caution">
  <div class="signal-label">CAUTION</div>
  <div class="signal-body">
    <p>[Hazard description]. [Consequence]. [Avoidance action].</p>
  </div>
</div>
```

**NOTE** (inline — Word: Body Text with bold prefix):
```html
<p class="ifu-note-inline"><strong>NOTE:</strong> [Informational text].</p>
```

> **IMPORTANT:** Do NOT use `<div class="signal-icon">⚠️</div>` or emoji in signal word blocks. The Word IFU does not use emoji icons — the signal word ("WARNING", "CAUTION") alone in bold serves as the visual indicator.

---

## Document Structure (Standard Chapter Sequence)

A complete CT IFU typically follows this chapter structure. Not all chapters are required for every feature — the skill generates **feature-level content** that fits into this overall structure.

| # | Chapter | Purpose | Typical Content |
|---|---------|---------|-----------------|
| 1 | **About this document** | Scope and conventions | Intended audience, document conventions (icons, formatting), related documents |
| 2 | **Safety** | Safety information overview | General safety warnings, contraindications, electromagnetic compatibility notes |
| 3 | **System overview** | Hardware/software orientation | System components, user interface overview, display layout |
| 4 | **Getting started** | Initial orientation | Login, startup, workspace selection, patient management |
| 5 | **[Feature chapters]** | Feature-specific instructions | One chapter per major feature or workflow (e.g., "Spectral results", "Rib labeling", "MPR viewing") |
| 6 | **Troubleshooting** | Problem resolution | Error messages, corrective actions, contact information |
| 7 | **Specifications** | Technical parameters | Performance specs, DICOM conformance, supported configurations |
| 8 | **Index** | Cross-reference | Alphabetical index of key terms |

---

## Feature Chapter Structure

Each feature chapter (the main deliverable of this skill) follows this internal structure:

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

| Rule | Example |
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

**Action verbs** (standardized):
- UI interactions: `Select`, `Click`, `Tap`, `Press`, `Drag`, `Scroll`
- Data actions: `Enter`, `Type`, `Specify`, `Set`
- Navigation: `Open`, `Close`, `Navigate to`, `Go to`
- Viewing: `Review`, `Verify`, `Check`, `Inspect`
- File operations: `Save`, `Export`, `Load`, `Import`

### Result Statement

After a procedure or significant step, describe what the user should see:

```
→ The system displays [description of result].
→ The [element] appears in [location].
→ A confirmation message appears.
```

---

## Terminology Conventions

### UI Element Naming

| Element Type | Format | Example |
|-------------|--------|---------|
| Button | Bold | **OK**, **Cancel**, **Apply** |
| Menu item | Bold with path | **Tools > Rib Labeling** |
| Tab / Panel | Bold | **Control Panel**, **Series List** |
| Field / Input | Bold | **Patient Name** field |
| Checkbox | Bold with state | Select the **Auto-detect** checkbox |
| Dropdown | Bold | From the **Orientation** list, select... |
| Keyboard key | Monospace/caps | Press `Enter`, Press `Ctrl+Z` |
| Icon | Describe + name | Click the **Edit** icon (pencil) |

### Clinical Terms

Use standard radiology terminology. When a term is first introduced, provide a brief explanation:

```
The MPR (Multi-Planar Reconstruction) view shows...
```

---

## Figure / Screenshot Conventions

### Placement Rules
- Place the screenshot **immediately after** the step it illustrates
- Use callouts (numbered circles or arrows) to highlight relevant UI elements
- Include a figure caption below: `Figure X: [Description]`

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

When referencing other sections within the IFU:

```
For more information, see "[Section title]" on page [X].
```

In HTML output (with navigation):
```html
For more information, see <a href="#section-id">Section title</a>.
```

---

## Table Format

Use tables for:
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

When a procedure differs based on product configuration or option:

```
> **Note:** This feature is available only with the [Option Name] license.

If [condition]:
1. Step for condition A...

If [alternative condition]:
1. Step for condition B...
```

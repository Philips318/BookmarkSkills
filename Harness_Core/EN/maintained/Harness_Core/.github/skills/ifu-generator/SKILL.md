---
name: ifu-generator
description: 'Generate Instructions for Use (IFU) content for Philips CT medical device features. Takes requirements, test cases, clinical context, and optionally UI screenshots or a workflow video as input — produces both a self-contained HTML document (with left navigation) and a Word (.docx) document. Follows IEC 82079-1 and Philips IFU conventions.'
argument-hint: 'Provide: 1) Requirement text, 2) Test cases/procedures, 3) Clinical use description, and optionally 4) UI screenshot paths or 5) a workflow video'
user-invocable: true
---

# IFU Generator

Use this skill to generate Instructions for Use (IFU) content for Philips CT medical device features. Given product requirements, test procedures, and clinical context, it produces professionally structured IFU documentation delivered in **two formats**: an interactive HTML page (for browser review) and a Word document (.docx, for direct editing and delivery).

## When to Use

- You have a new feature requirement and need to draft IFU content for technical writers.
- You need to create operation procedures with step-by-step instructions for radiologists/technologists.
- You want to generate initial IFU content that follows the established style and structure of existing CT IFU documents.
- You need to ensure proper Warning/Caution/Note placement based on requirement safety classification.

## Business Context

This skill generates IFU content for **Philips CT medical devices** used by:

- **CT Technologists** (放射科技师) — primary operator audience, performs scans and post-processing
- **Radiologists / Imaging Physicians** (影像科医生) — reviews images, interprets results, uses advanced visualization

The IFU must conform to:
- **IEC 82079-1** — Preparation of information for use of products (safety information, structure)
- **IEC 62366** — Usability engineering for medical devices
- **FDA 21 CFR 801** — Labeling requirements for medical devices
- **Philips IFU Style Guide** — Internal documentation conventions

## Related Skill: domain-knowledge

Before writing IFU content, consult the `domain-knowledge` skill to ensure terminological and clinical accuracy:

- **ct-glossary.md** — use **formal feature names** (e.g., "Precise Image" not "AI Recon"; "OnPlan" not "iStation"), correct units (HU, mg/ml, keV, %EDW), and correct abbreviations (HU/WW/WC/FOV/IPP/IOP/CTDIvol/DLP/SSDE)
- **clinical-workflow.md** — align procedure steps with the official 11-step scanner workflow and ISP module structure
- **safety-rules.md** — use the listed warnings as canonical text for Warning/Caution placement (e.g., "VNC recommended for body scans only", "Pediatric patients must use pediatric exam cards", "Lossy compression must not be used for diagnosis")
- **spectral-knowledge.md** — use the canonical result type table (units, accuracy thresholds, limitations) when documenting spectral features

This ensures the IFU matches the existing Philips IFU corpus and uses only approved terminology.

### Product Scope

| Subsystem | IFU Coverage |
|-----------|-------------|
| **Console Software** | Post-processing applications (MPR, 3D, DE, Rib, Spine, Endo, Spectral, etc.) |
| **Scan Workflow** | Protocol selection, scan planning, patient positioning |
| **Image Reconstruction** | Reconstruction parameters, spectral results |
| **Control Platform** | System management, configuration, patient administration |
| **CT Couch** | Patient table positioning, movement controls |

## Required Inputs

The following inputs are **required** for IFU generation:

### 1. Requirement (SRS/SSRS)

The product requirement defining the feature behavior:

```
Requirement ID: CT-NMP.SSRS.Console.XXXX
Requirement text: The [application] shall [behavior] when [condition]
Classification: CTS / CTQ / Standard
Product: CT5200RT, CT7900, etc.
Target Release: VX.0
```

### 2. Test Cases / Test Procedures

Test cases provide the operation steps and expected behavior:

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

Context explaining **why** and **when** the feature is used in clinical practice:

```
Clinical context: [Description of clinical workflow and purpose]
Example: "Rib labeling is used during pre-surgical planning to identify and label
individual ribs for accurate surgical site identification in thoracic procedures."
```

### 4. UI Screenshots (Optional)

Screenshots or image file paths for procedure illustration:

- **Mode A** (paths provided): Provide file paths to PNG/SVG/JPG screenshots. The skill will embed them with `<img>` tags and generate appropriate captions.
  ```
  Screenshots:
    - Step 3: C:\Work\Screenshots\rib_toolbar.png (Rib labeling button in toolbar)
    - Step 5: C:\Work\Screenshots\rib_result.png (Labeled ribs in 3D view)
  ```
  
- **Mode B** (no paths): The skill generates text placeholders `[INSERT SCREENSHOT: description]` for later manual replacement.

### 5. Workflow Video (Optional)

A screen-recording of the real workflow on the CT Console (`.mp4`, `.mov`, `.avi`, `.webm`, `.gif`). The video is **pre-processed into frames and steps** before IFU generation — the skill does NOT consume raw video directly.

**How it is used:**

- **Frame extraction**: Key frames are extracted at scene changes (or fixed intervals) using the [video pre-processing script](./references/Extract-VideoFrames.ps1). Frames are saved as PNG files.
- **Workflow derivation**: The skill analyzes the ordered frames to derive the user workflow — user actions, menu paths, UI state changes, and result states.
- **Screenshots**: Extracted frames are reused as IFU screenshots (Mode A), with auto-generated captions per step.
- **Uncertainty handling**: When a UI element name cannot be confirmed from the video, mark it `[TBD]` rather than inventing a name.

**Important constraints:**

- A video can **supplement or partially replace** test cases for UI-flow features (menu paths, enable/disable settings, scan planning, app launch, result review). It **cannot** replace: requirement ID/text, CTS/CTQ/Normal classification, product line/release, clinical purpose, or safety-hazard judgment.
- **De-identify first**: If the recording contains patient information (name, ID, DOB, images), it must be anonymized before frames are extracted or embedded.
- The most reliable input combination is `Requirement + Clinical Context + Workflow Video`, or more completely `Requirement + Test Case + Clinical Context + Workflow Video`.

## Output

The skill generates **two files** for every IFU:

### 1. HTML file (`{feature_name}_ifu.html`)

A self-contained HTML file for **browser-based review**. Includes:

- **Left navigation panel**: Hierarchical TOC with scroll-tracking active state (screen-only; hidden when printing)
- **Right content pane**: Full IFU content (no metadata bar in content — metadata is in the left nav only)
  1. Feature title
  2. Overview (purpose + clinical context)
  3. Before you begin (prerequisites)
  4. Feature-level warnings/cautions
  5. Procedure (numbered steps with screenshots or placeholders)
  6. Result description
  7. Troubleshooting table
  8. Draft disclaimer

### 2. Word file (`{feature_name}_ifu.docx`)

A Word document for **direct editing and delivery** to the Tech Doc team. Uses proper Word styles:

- No copy-paste from HTML needed — the Word file is ready to edit
- Proper heading styles (Heading 1–5) with Trebuchet MS font
- Body text in Microsoft Sans Serif
- WARNING/CAUTION blocks as styled paragraphs
- NOTE as bold-prefix body text
- Numbered procedure steps
- Troubleshooting as a Word table
- Draft disclaimer at the end

The Word file is generated using Word COM automation (`Build-IFU-Word.ps1`) and saved alongside the HTML file.

### Word Style Compatibility

The HTML uses font/size combinations that map 1:1 to Word styles:

| HTML Element | Word Style Target |
|---|---|
| `<h1>` Trebuchet MS 16pt Bold | Heading 1 |
| `<h2>` Trebuchet MS 12pt Bold | Heading 2 |
| `<h3>` Microsoft Sans Serif 11pt | Heading 3 |
| Signal label Trebuchet MS 10pt Bold | Heading 4 (WARNING / CAUTION) |
| Signal body Trebuchet MS 9.5pt Bold | Heading 7 |
| Body text Microsoft Sans Serif 9.5pt | Body Text |
| Procedure steps Microsoft Sans Serif 10pt | List Paragraph |
| NOTE: inline bold prefix | Body Text with bold prefix |

After review, the tech doc team edits the Word file directly — no copy-paste from HTML needed.

Files are saved directly to disk under the workspace root `IFU/` directory (create it if needed), unless the user explicitly specifies another output path:
- `IFU/{feature_name}_ifu.html` — browser review
- `IFU/{feature_name}_ifu.docx` — Word editing & delivery

> **File Output Rule**: The skill MUST create BOTH files on disk. Use `create_file` for the HTML. Use a PowerShell script with Word COM automation (run via terminal) for the Word file. Do NOT output content in chat — files must be ready to open immediately.

> **No metadata bar in content area**: Product, Requirement, Release, Category, Date info goes in the left nav panel (HTML) or document properties (Word) only. Do NOT show a metadata bar in the main content area.

---

## Workflow

1. **Parse inputs**: Extract requirement ID, text, classification, product, and release version.
2. **Pre-process workflow video** (if provided): Before analyzing steps, extract frames and derive the workflow:
   - Run the [video pre-processing script](./references/Extract-VideoFrames.ps1) to extract key frames as PNG files into a `{feature_name}_frames/` folder.
   - Order the frames chronologically and analyze them to reconstruct the user workflow (actions, menu paths, UI state changes, result states).
   - Treat each confirmed UI transition as a candidate procedure step; reuse the corresponding frame as that step's screenshot (Mode A).
   - Mark any UI element name that cannot be confirmed from the frames as `[TBD]`.
   - Verify the frames are de-identified before embedding (no patient name/ID/DOB/images).
3. **Classify requirement by user interaction type** (critical step):
   - **Interactive**: User performs actions → generate **Procedure** sections with numbered steps
   - **Automatic/System behavior**: System acts automatically → generate **Reference/Description** sections (NO procedure steps)
   - **Mixed**: Some user actions trigger automatic behavior → generate procedure for user actions, describe automatic behavior inline
   
   **How to classify:**
   - If the requirement says "shall display/show/position/calculate/determine automatically" → **Automatic**
   - If the requirement says "shall allow/enable the user to..." or test cases have "Click/Select/Enter" → **Interactive**
   - If test cases are mostly "Verify/Observe/Check" with no user actions → **Automatic** (test verification ≠ user operation)
   
4. **Analyze test cases**: Map test steps to user-facing operation steps:
   - Filter out internal/technical setup steps (test environment specific)
   - Convert test language to IFU language (imperative mood, user-facing)
   - Identify expected results for each step
   - **For automatic behavior requirements**: Extract the behavior description from test expected results, but do NOT convert verification steps into user procedures
4. **Determine safety signals**: Based on requirement classification:
   - CTS → generate WARNING(s) based on the safety hazard
   - CTQ → generate CAUTION or NOTE as appropriate
   - Standard → generate NOTE if needed for correct operation
5. **Structure the content**:
   - Write Overview using clinical context + requirement purpose
   - Write Prerequisites from test preconditions
   - **Interactive**: Write Procedure from test steps (converted to user language)
   - **Automatic**: Write description of system behavior + conditions table + NOTEs
   - Write Troubleshooting from common failure scenarios
6. **Handle screenshots**:
   - Mode A: embed `<img>` with provided paths and auto-generate captions
   - Mode B: insert `[INSERT SCREENSHOT: ...]` placeholders at appropriate steps
7. **Build HTML**: Fill the [IFU template](./references/ifu-template.html) with generated content (no metadata bar in content area)
8. **Save HTML**: Use `create_file` to write `IFU/{feature_name}_ifu.html` under the workspace root, unless the user explicitly specifies another output path.
9. **Generate Word**: Create a PowerShell script that uses Word COM automation to build `IFU/{feature_name}_ifu.docx` under the workspace root with proper Word styles (Trebuchet MS headings, Microsoft Sans Serif body), then run it via terminal. The Word file contains the same content as the HTML but with native Word formatting. Reference script: `Build-IFU-Word.ps1`.

---

## IFU Chapter Structure

The chapter structure depends on whether the feature is **interactive** or **automatic**.

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

> **Key distinction**: Pattern A has **Procedure** sections with numbered steps (user actions). Pattern B has **System Behavior** sections with description and conditions tables (no numbered action steps — the user does not need to do anything).

---

## Writing Rules

### Step Conversion (Test Case → IFU)

| Test Case Language | IFU Language |
|-------------------|-------------|
| "Open the application under test" | "**Open** the [Application Name] from the **Applications** menu" |
| "Verify that X is displayed" | (Becomes a result statement: "→ The system displays X") |
| "Set parameter A to value B" | "In the **[Field Name]** field, **enter** [value]" |
| "Wait for processing to complete" | "**Wait** for the processing to complete" + result |
| "Check that no error occurs" | (Omit — not user-relevant; or convert to troubleshooting) |
| "Cleanup: close application" | (Omit — test teardown, not user instruction) |

> **Published IFU step pattern**: System response is embedded in the same sentence, not a separate line:
> `Click Short Tube Conditioning. The Short Tube Conditioning interface opens.`
> NOT: `Click Short Tube Conditioning.` → `The interface opens.` (separate line)

### Warning Placement

- Feature-level warnings: **before** the procedure section
- Step-specific warnings: **immediately before** the hazardous step
- Never place warnings after the hazard

### Language Style

- **Imperative mood**: "Select the series" (not "You should select")
- **Present tense for results**: "The system displays..." (not "will display")
- **One action per step**: Each numbered step = one user action
- **Bold for UI elements**: **OK**, **Cancel**, **Tools > Rib Labeling**
- **Consistent terminology**: Same UI element = same name throughout

See [IFU Style Guide](./references/ifu-style-guide.md) for complete style rules.

### Warning/Caution/Note Rules

See [Warning/Caution/Note Classification](./references/warning-caution-note.md) for:
- Signal word hierarchy (WARNING > CAUTION > NOTE)
- Three-part structure (Hazard + Consequence + Avoidance)
- CT-specific warning templates
- CTS/CTQ mapping to signal words

### CT Domain Knowledge

See [CT Domain Knowledge](./references/ct-domain-knowledge.md) for:
- **Glossary**: Standard CT terminology (SBI, MonoE, iDose4, O-MAR, EFOV, DRI, CTDIvol, DLP, etc.)
- **Real WARNING/CAUTION/NOTE examples** extracted from published IFU documents
- **Prerequisite patterns**: System state, patient setup, data/configuration, feature-specific
- **Procedure step patterns**: Real IFU step language (imperative, atomic, with inline system response)
- **Clinical use descriptions**: Templates for post-processing, scan workflow, and dose optimization features
- **Product lines**: CT Verida Family, Rembra, Areta, system component names (Console, Gantry, CTBox, PIM, etc.)

When generating IFU content, **always consult this reference** to:
1. Use correct CT terminology (never invent abbreviations)
2. Follow real IFU step patterns ("Click X. The Y interface opens.")
3. Use real WARNING patterns as templates for new warnings
4. Include correct dosimetric units (CTDIvol in mGy, DLP in mGy·cm, keV)
5. Use correct system component names (Console, Gantry, CTBox, etc.)

---

## Important Rules

- **This is a draft generator** — always include a disclaimer stating that content requires review by Technical Documentation and Regulatory Affairs.
- **Do not fabricate clinical information** — if clinical context is insufficient, mark sections as `[TBD — requires clinical input]`.
- **Conservative safety approach** — when uncertain about hazard severity, use WARNING (not CAUTION).
- **Do not invent UI element names** — use names from requirements/test cases. If unknown, use descriptive placeholders like `[Button Name]`.
- **Screenshot handling**: Mode A (paths provided) = embed images; Mode B (no paths) = generate placeholders.
- **One procedure per task** — if the requirement covers multiple tasks, create separate procedure sections.
- **Keep steps atomic** — one action per step, maximum.
- **Do NOT turn test verification into user procedures** — if the test case says "Verify that X is displayed" or "Observe the preview image", this is test validation, NOT a user action. For automatic system behavior, describe what happens (reference/description), do not instruct the user to "verify" or "observe" it.
- **Classify before writing** — always determine if the requirement describes interactive or automatic behavior BEFORE writing content. Use Pattern A (procedure) for interactive features, Pattern B (system behavior description) for automatic features.
- The HTML must be fully self-contained (inline CSS, inline JS).
- **Always save BOTH HTML and Word files to disk**. Never output content in chat — create both files directly.
- **No metadata bar in content area** — Product/Requirement/Release/Date info belongs in the left nav (HTML) or document properties (Word) only.
- The Word file is generated using Word COM automation (PowerShell). See `Build-IFU-Word.ps1` for the reference implementation.

---

## Related Skills

- [DFMEA Analysis](../dfmea-analysis/SKILL.md) — for risk analysis of the same requirements
- [Requirements](../requirements/SKILL.md) — for structuring requirements into acceptance criteria
- [Doc Generator](../doc-generator/SKILL.md) — for SDS and other technical documentation

## References

- [IFU Style Guide](./references/ifu-style-guide.md) — Writing conventions, Word style mapping, font/size specifications
- [Warning/Caution/Note Classification](./references/warning-caution-note.md) — Signal word hierarchy, HTML patterns, CTS/CTQ mapping
- [CT Domain Knowledge](./references/ct-domain-knowledge.md) — CT glossary, real WARNING/CAUTION/NOTE examples, prerequisite patterns, procedure step patterns, clinical context templates, product line reference
- [HTML Template](./references/ifu-template.html) — Word-compatible HTML template with Trebuchet MS / Microsoft Sans Serif fonts
- **Existing IFU reference documents** — auto-discover in this order:
  1. `IFU/` folder at the **workspace root** (most teams keep the IFU corpus there)
  2. Any folder matching `**/IFU/` within the workspace (use `file_search`)
  3. Path recorded in `/memories/repo/ifu-corpus-path.md` (per-workspace override)
  4. If none found, generate IFU from scratch using only `domain-knowledge` + the inputs, and note this in the draft disclaimer

## Portability Note

This skill is **team-portable** within Philips CT environments. The IEC 82079-1 / IEC 62366 structure, Pattern A (Interactive) vs Pattern B (Automatic) classification, WARNING/CAUTION/NOTE rules, and Word-style mapping are based on Philips IFU conventions but follow international standards. Domain terminology (formal feature names, units, safety-warning canonical text) comes from the separate `domain-knowledge` skill — another team's domain skill supplies different content automatically. Team-specific items (actual IFU corpus location, internal review roles, product-line-specific templates) belong in `references/` (versioned) or `/memories/repo/` (per-workspace), not hardcoded here.

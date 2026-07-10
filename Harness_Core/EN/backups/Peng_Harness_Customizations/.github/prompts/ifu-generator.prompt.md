---
description: 'Generate IFU (Instructions for Use) content for a CT feature. Provide requirements, test cases, clinical context, and optionally screenshot paths or a workflow video.'
mode: 'agent'
tools: ['read_file', 'create_file', 'replace_string_in_file', 'list_dir', 'grep_search', 'semantic_search', 'run_in_terminal']
---

Generate Instructions for Use (IFU) content for the provided CT feature and produce a **self-contained HTML document** with left navigation + right content.

Use these repository assets as source of conventions, safety templates, and HTML structure:

- [IFU Style Guide](../skills/ifu-generator/references/ifu-style-guide.md) — Writing conventions, Word style mapping (fonts/sizes)
- [Warning/Caution/Note Classification](../skills/ifu-generator/references/warning-caution-note.md) — Signal word hierarchy, real published examples
- [CT Domain Knowledge](../skills/ifu-generator/references/ct-domain-knowledge.md) — CT glossary, real WARNING/CAUTION/NOTE examples, procedure step patterns, clinical context templates
- [HTML Template](../skills/ifu-generator/references/ifu-template.html) — Word-compatible HTML template (Trebuchet MS + Microsoft Sans Serif)
- IFU reference documents (existing IFUs for style reference): `C:\Work\Code\Git_Code\IFU\`

## Context

This is a **Philips CT medical device** IFU. The audience is:
- **CT Technologists** (放射科技师) — perform scans and post-processing operations
- **Radiologists** (影像科医生) — review images and diagnostic results

The IFU must conform to IEC 82079-1, IEC 62366, and Philips documentation standards.

## Required Inputs (from user)

1. **Requirement** (SRS/SSRS): Feature specification with ID, text, classification (CTS/CTQ), product, release
2. **Test Cases**: Test procedures with preconditions, steps, and expected results
3. **Clinical Use Description**: Why and when this feature is used clinically
4. **UI Screenshots** (optional): File paths to screenshots for embedding
5. **Workflow Video** (optional): A screen-recording (`.mp4`, `.mov`, `.avi`, `.webm`, `.gif`) of the real workflow. Pre-processed into frames before use — it can supplement/partially replace UI-flow test cases and supply screenshots, but **cannot** replace requirement ID/text, classification, product/release, clinical purpose, or safety judgment. De-identify any patient data first.

## Workflow

1. **Parse requirement**: Extract ID, text, classification, product line, target release.
2. **Pre-process workflow video** (if provided): Extract frames and derive the workflow before analyzing steps:
   - Run the [video pre-processing script](../skills/ifu-generator/references/Extract-VideoFrames.ps1) to extract key frames as PNG into `{feature_name}_frames/`.
   - Order frames chronologically and reconstruct the user workflow (actions, menu paths, UI state changes, result states).
   - Reuse each confirmed frame as that step's screenshot; mark any unconfirmed UI element name as `[TBD]`.
   - Verify frames are de-identified before embedding.
3. **Analyze test cases**: Convert test steps into user-facing operation steps:
   - Remove test-internal setup/teardown steps
   - Convert verification steps to "Result" statements
   - Translate technical test language to user-facing imperative instructions
4. **Generate safety signals**:
   - CTS requirement → must include WARNING about safety implications
   - CTQ requirement → include CAUTION about correct usage
   - Use templates from the [Warning/Caution/Note Classification](../skills/ifu-generator/references/warning-caution-note.md)
5. **Write IFU content**:
   - **Overview**: Combine requirement purpose + clinical context
   - **Prerequisites**: From test preconditions (filtered for user-relevance)
   - **Procedure**: From test steps (converted to IFU style per [Style Guide](../skills/ifu-generator/references/ifu-style-guide.md))
   - **Result**: From test expected outcomes
   - **Troubleshooting**: Common failure scenarios derived from requirement scope
6. **Handle screenshots**:
   - If paths provided → embed `<img src="path" alt="description" />` with figure captions
   - If NO paths → insert `<div class="ifu-figure-placeholder">[INSERT SCREENSHOT: description]</div>`
7. **Build HTML**: Use the [HTML template](../skills/ifu-generator/references/ifu-template.html):
   - Fill all `{{PLACEHOLDER}}` values
   - Generate navigation links matching content headings
   - Ensure all CSS/JS is inline (self-contained)
8. **Save**: Write to `IFU/{feature_name}_ifu.html` under the workspace root (create the `IFU` directory if needed), unless the user explicitly specifies another output path.

## Output

A single self-contained HTML file (light theme) with:
- Default save location: workspace root `IFU/` directory, with filename `{feature_name}_ifu.html` unless the user explicitly requests another path.
- Left sidebar navigation (sticky, scroll-tracked)
- Right content area with:
  - Feature title + metadata (product, requirement ID, version, date)
  - Overview (purpose + clinical context)
  - Before you begin (prerequisites)
  - Feature-level warnings/cautions/notes
  - Numbered procedure steps (with screenshots or placeholders)
  - Result description
  - Troubleshooting table
  - AI-generated draft disclaimer

## Rules

- **This is a draft** — always include disclaimer that content requires Technical Documentation and Regulatory review.
- Use **imperative mood** for all instructions ("Select", "Click", "Verify").
- Use **present tense** for system responses ("The system displays...").
- Embed system response in the same step sentence: "Click X. The X interface opens." (not on a separate line)
- **One action per step** — never combine multiple actions.
- **Bold** all UI element names.
- Place warnings **before** the hazardous step, never after.
- **No emoji** in WARNING/CAUTION/NOTE — use signal word text only (matches Word IFU style).
- Use correct CT terminology from [CT Domain Knowledge](../skills/ifu-generator/references/ct-domain-knowledge.md) — never invent abbreviations.
- Include dosimetric units where applicable: CTDIvol (mGy), DLP (mGy·cm), keV.
- Do not invent UI element names not found in the inputs.
- Do not fabricate clinical information — mark unknowns as `[TBD]`.
- The HTML output must use Word-compatible fonts (Trebuchet MS for headings, Microsoft Sans Serif for body) to support copy-paste into the official IFU Word document.
- When uncertain about hazard level, default to WARNING (conservative approach).
- The HTML must be fully self-contained (inline CSS, inline JS, no external dependencies).

---
description: "Convert living QMS markdown documents to Word (.docx) via the pre-tested Export-Qms.py script (pandoc + PDLM templates). Run when documents are ready for formal review."
mode: "agent"
---

> **Manual-only prompt.** This is NOT part of the runtime harness loop. Runtime agents maintain markdown QMS files directly; Word export is a separate human-invoked step for formal review submissions. Do not invoke this prompt during automated orchestration.

# Convert QMS Markdown to Word

All conversion mechanics live in the pre-tested script `.github/scripts/Export-Qms.py` (filename→template map, strict Mermaid pre-rendering, pandoc invocation, cover-page merge, failure handling). The batch wrapper `.github/scripts/Export-Qms.bat` calls it automatically using the repo venv Python. Do not hand-write pandoc commands — always call the script via the batch wrapper.

## Step 1 — Determine which documents to convert

If the user has NOT explicitly named the documents to convert, **ask before doing anything** using the ask-questions tool. Do not assume "all" or guess from recent edits.

Offer the six QMS documents as multi-select options:
- `docs/qms/SwRS.md`
- `docs/qms/SSDS.md`
- `docs/qms/SDD.md`
- `docs/qms/MVP.md`
- `docs/qms/MVProcedure.md`
- `docs/qms/MVReport.md`

Only proceed once the user has selected at least one.

## Step 2 — Prerequisites

`Export-Qms.py` fails fast if any tool is missing:
- `pandoc` on PATH — markdown→docx engine
- `mmdc` (mermaid-cli) on PATH — Mermaid pre-rendering
- `python-docx` in the repo venv (`venv\`) — cover-page merging

To install/verify **all** of these from scratch in one shot, run the bootstrap script (idempotent — safe to re-run). On a bare machine it also installs Python and Node.js/npm via winget:

```bat
.github\scripts\Setup-QmsEnv.bat
```

If the export later fails fast with a missing-tool error, run the setup script above, then **reopen the terminal** so PATH refreshes, and retry the export. The setup script cannot bootstrap winget itself — if winget is absent it prints guidance and exits. Do not hand-install with ad-hoc `pip`/`winget` commands; always use the setup script so python-docx lands in the repo venv the exporter looks for.

Mermaid handling is **strict by design**: every diagram is pre-rendered to PNG, and if any single diagram fails or times out the whole run fails — a QMS document must be complete, never partial.

## Step 3 — Run the export

Pass the user-selected files to the batch wrapper (run from the repo root):

```bat
.github\scripts\Export-Qms.bat docs/qms/SDD.md docs/qms/MVReport.md
```

Optional arguments:
- `--output-dir DIR` (default: same folder as each source `.md` file)
- `--templates-dir DIR` (default `docs/qms-templates`)
- `--mermaid-timeout SEC` (default `600`; raise for very large/complex diagrams)

## Step 4 — Report

Report which documents were converted and their output paths (the script prints them). If the script throws, surface the exact error to the user — do not retry blindly or fall back to a manual pandoc command.

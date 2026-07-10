#!/usr/bin/env python3
"""
Extract-QmsSkeleton.py
======================
Extract the canonical section skeleton from each PDLM QMS Word template and
write it as a machine-readable markdown reference under
``docs/qms-templates/skeletons/``.

The .docx templates are the source of truth for document structure, but they
are binary and are NOT available to the runtime agents.  This script distills
each template into a markdown skeleton that IS committed to the repo and read
by the agents, capturing:

  * every heading verbatim (exact text, in document order),
  * every table with its exact column headers,
  * the template's embedded author guidance (instruction paragraphs and
    ``<placeholder>`` example sections) preserved as ``<!-- GUIDANCE: ... -->``.

Heading levels follow the convention used by Export-Qms.py
(``pandoc --shift-heading-level-by -1``), so a template *Heading 1* becomes a
markdown ``##`` and the document title is a single ``#``.

Repeatable example headings such as ``<Issue One>`` or
``[Add additional sub-systems, as needed.]`` are NOT emitted as literal
headings (they are placeholders, not fixed sections); they are captured as
guidance under their parent section instead.

Usage:
    python .github/scripts/Extract-QmsSkeleton.py [--reseed] [--templates-dir DIR] [--out-dir DIR]

Options:
    --reseed             Also (over)write docs/qms/{Stem}.md from each skeleton.
    --templates-dir DIR  PDLM templates directory   (default: docs/qms-templates)
    --out-dir DIR        Skeleton output directory   (default: docs/qms-templates/skeletons)
    --living-dir DIR     Living QMS docs directory   (default: docs/qms)
"""

import argparse
import re
import sys
from pathlib import Path

try:
    from docx import Document
    from docx.oxml.ns import qn
    from docx.table import Table
    from docx.text.paragraph import Paragraph
except ImportError:
    print("ERROR: python-docx is not installed.  Run:  pip install python-docx",
          file=sys.stderr)
    sys.exit(1)


# Output stem -> (template filename, document title)
DOC_MAP: dict[str, tuple[str, str]] = {
    "SwRS": (
        "2003000201 HA030-054-05_PDLM SwRS Template.docx",
        "Software Requirements Specification",
    ),
    "SSDS": (
        "2003000198 HA030-054-02_PDLM SDS_SSDS Template.docx",
        "Sub-System Design Specification",
    ),
    "SDD": (
        "2003000204 HA030-055-02_PDLM Software Design Document Template _.docx",
        "Software Design Document",
    ),
    "MVP": (
        "2003000256 HA030-064-01_PDLM Module Verification or Module Integration Plan Template.docx",
        "Module Verification Plan",
    ),
    "MVProcedure": (
        "2003000258 HA030-064-03_PDLM Module Verification or Module Integration Procedure Template.docx",
        "Module Verification Procedure",
    ),
    "MVReport": (
        "2003000257 HA030-064-02_PDLM Module Verification or Module Integration Report Template.docx",
        "Module Verification Report",
    ),
    # --- System-level (black-box) verification, owned by @test-designer. ---
    # PDLM .docx templates are not in the repo yet; the filenames below are the
    # expected drop-in names.  Until the .docx are added these stems are skipped
    # gracefully (see PENDING_STEMS).  Replace the filenames if the official
    # template names differ, then re-run this script to generate the skeletons.
    "VerificationPlan": (
        "PDLM Software Verification Plan Template.docx",
        "Software Verification Plan",
    ),
    "VerificationProcedure": (
        "PDLM Software Verification Procedure Template.docx",
        "Software Verification Procedure",
    ),
}

# Stems whose PDLM .docx template has not been added yet: a missing template is
# a graceful SKIP, not a hard error.  Remove a stem once its .docx is in place.
PENDING_STEMS = {"VerificationPlan", "VerificationProcedure"}

# A heading that contains a <...> or [...] placeholder fragment is a template
# example / repeatable section, not a fixed section name (e.g. "<Issue One>",
# "APPENDIX <x> - <Title>", "[Add additional sub-systems, as needed.]").
_RE_PLACEHOLDER = re.compile(r"<[^>]*>|\[[^\]]*\]")


def _clean(text: str) -> str:
    """Collapse internal whitespace/newlines to single spaces."""
    return re.sub(r"\s+", " ", text).strip()


def _strip_placeholder_markers(text: str) -> str:
    """Remove surrounding <> or [] so a placeholder column becomes a clean name."""
    t = _clean(text)
    if t.startswith("<") and t.endswith(">"):
        t = t[1:-1].strip()
    elif t.startswith("[") and t.endswith("]"):
        t = t[1:-1].strip()
    return t


def _heading_level(style_name: str) -> int | None:
    """Return the heading level N for a 'Heading N' style, else None."""
    if not style_name or not style_name.startswith("Heading"):
        return None
    m = re.search(r"(\d+)", style_name)
    return int(m.group(1)) if m else None


def _iter_body(doc: Document):
    """Yield (kind, obj) for paragraphs and tables in document order."""
    body = doc.element.body
    for child in body:
        if child.tag == qn("w:p"):
            yield "p", Paragraph(child, doc)
        elif child.tag == qn("w:tbl"):
            yield "tbl", Table(child, doc)


def _table_columns(tbl: Table) -> list[str]:
    """Return cleaned column names from the table's header (first) row."""
    if not tbl.rows:
        return []
    cols = []
    for cell in tbl.rows[0].cells:
        name = _strip_placeholder_markers(cell.text)
        name = name.replace("|", r"\|")
        cols.append(name or "Column")
    # De-duplicate merged-cell repeats while preserving order.
    seen: list[str] = []
    for c in cols:
        if not seen or seen[-1] != c:
            seen.append(c)
    return seen


def extract_skeleton(template_path: Path, title: str) -> str:
    """Build the markdown skeleton string for one template."""
    doc = Document(str(template_path))
    lines: list[str] = [f"# {title}", ""]

    started = False           # become True at the first real Heading 1
    pending_guidance: list[str] = []

    def flush_guidance():
        for g in pending_guidance:
            lines.append(f"<!-- GUIDANCE: {g} -->")
        if pending_guidance:
            lines.append("")
        pending_guidance.clear()

    for kind, obj in _iter_body(doc):
        if kind == "p":
            text = _clean(obj.text)
            level = _heading_level(obj.style.name or "")

            if level is not None and text:
                if _RE_PLACEHOLDER.search(text):
                    # Repeatable example heading -> guidance, not a fixed section.
                    pending_guidance.append(f"repeatable example section: {text}")
                    continue
                if level == 1 and not started:
                    started = True
                if not started:
                    continue
                flush_guidance()
                md_level = "#" * (level + 1)   # template H1 -> md '##'
                lines.append(f"{md_level} {text}")
                lines.append("")
            elif text and started:
                # Body paragraph after a heading = template author guidance.
                pending_guidance.append(text)

        elif kind == "tbl" and started:
            cols = _table_columns(obj)
            if not cols:
                continue
            flush_guidance()
            lines.append("| " + " | ".join(cols) + " |")
            lines.append("|" + "|".join([" --- "] * len(cols)) + "|")
            lines.append("")

    flush_guidance()

    # Normalise trailing blank lines to exactly one.
    while len(lines) >= 2 and lines[-1] == "" and lines[-2] == "":
        lines.pop()
    if lines and lines[-1] != "":
        lines.append("")
    return "\n".join(lines)


def main() -> None:
    parser = argparse.ArgumentParser(
        description="Extract canonical QMS section skeletons from PDLM .docx templates."
    )
    parser.add_argument("--reseed", action="store_true",
                        help="Also overwrite docs/qms/{Stem}.md from each skeleton.")
    parser.add_argument("--templates-dir", default="docs/qms-templates")
    parser.add_argument("--out-dir", default="docs/qms-templates/skeletons")
    parser.add_argument("--living-dir", default="docs/qms")
    args = parser.parse_args()

    tmpl_dir = Path(args.templates_dir)
    out_dir = Path(args.out_dir)
    living_dir = Path(args.living_dir)
    out_dir.mkdir(parents=True, exist_ok=True)
    if args.reseed:
        living_dir.mkdir(parents=True, exist_ok=True)

    for stem, (tmpl_name, title) in DOC_MAP.items():
        tmpl_path = tmpl_dir / tmpl_name
        if not tmpl_path.exists():
            if stem in PENDING_STEMS:
                print(f"  SKIP: {stem} (template pending: {tmpl_name})")
                continue
            print(f"ERROR: template not found: {tmpl_path}", file=sys.stderr)
            sys.exit(1)

        skeleton = extract_skeleton(tmpl_path, title)
        skel_path = out_dir / f"{stem}.skeleton.md"
        skel_path.write_text(skeleton, encoding="utf-8")
        n_head = skeleton.count("\n#")
        print(f"  {stem}: skeleton -> {skel_path}  ({n_head} headings)")

        if args.reseed:
            living_path = living_dir / f"{stem}.md"
            living_path.write_text(skeleton, encoding="utf-8")
            print(f"           reseed   -> {living_path}")

    print("\nDone.")


if __name__ == "__main__":
    main()

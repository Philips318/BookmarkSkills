#!/usr/bin/env python3
"""
Verify-QmsStructure.py
======================
Verify that each living QMS markdown document in ``docs/qms/`` still conforms
to the canonical skeleton extracted from its PDLM Word template
(``docs/qms-templates/skeletons/{Stem}.skeleton.md``).

Conformance rules (always enforced):
  * STRUCTURE   - every skeleton heading is present in the living document with
                  the exact text and level, in the same relative order
                  (additional per-module / per-issue subsections are allowed).
  * TABLES      - every skeleton table's column header row is present in the
                  living document with identical columns (extra tables allowed).
  * SELF-CONTAINED - the living document contains NO reference into ``.harness/``
                  (those are planning artifacts, not shipped with the product).

Optional rule (``--check-content``, used before export / completion):
  * COMPLETENESS - every leaf section has either real content or an explicit
                  ``Not Applicable`` marker; no unresolved ``{placeholder}`` or
                  ``<template instruction>`` text remains outside comments.

Exit code 0 = all documents conform; 1 = at least one violation.

Usage:
    python .github/scripts/Verify-QmsStructure.py [--check-content]
                                                  [--living-dir DIR]
                                                  [--skeleton-dir DIR]
                                                  [files ...]
"""

import argparse
import re
import sys
from pathlib import Path

DOC_STEMS = ["SwRS", "SSDS", "SDD", "MVP", "MVProcedure", "MVReport",
             "VerificationPlan", "VerificationProcedure"]

# Stems whose PDLM .docx template has not been added yet.  Until their skeleton
# is extracted, a missing skeleton is a graceful SKIP (informational), not a
# conformance failure -- so the workflow can reference these documents before
# the formal template lands.  Remove a stem from this set once its skeleton
# exists under docs/qms-templates/skeletons/.
PENDING_STEMS = {"VerificationPlan", "VerificationProcedure"}

_RE_FENCE   = re.compile(r"^\s*```")
_RE_HEADING = re.compile(r"^(#{1,6})\s+(.*\S)\s*$")
_RE_COMMENT = re.compile(r"<!--.*?-->", re.DOTALL)
_RE_HARNESS = re.compile(r"\.harness[\\/]")
_RE_HOLDER  = re.compile(r"\{[A-Za-z0-9_ -]+\}|<[^>]*template[^>]*>", re.IGNORECASE)
_RE_NA      = re.compile(r"not\s+applicable", re.IGNORECASE)


def _norm(text: str) -> str:
    return re.sub(r"\s+", " ", text).strip()


def parse_headings(text: str) -> list[tuple[int, str]]:
    """Return [(level, text)] for ATX headings outside fenced code blocks."""
    headings: list[tuple[int, str]] = []
    in_fence = False
    for line in text.splitlines():
        if _RE_FENCE.match(line):
            in_fence = not in_fence
            continue
        if in_fence:
            continue
        m = _RE_HEADING.match(line)
        if m:
            headings.append((len(m.group(1)), _norm(m.group(2))))
    return headings


def parse_table_columns(text: str) -> list[list[str]]:
    """Return the column lists of every markdown table (by its header row)."""
    tables: list[list[str]] = []
    lines = text.splitlines()
    in_fence = False
    for i, line in enumerate(lines):
        if _RE_FENCE.match(line):
            in_fence = not in_fence
            continue
        if in_fence:
            continue
        if line.strip().startswith("|") and i + 1 < len(lines):
            sep = lines[i + 1].strip()
            if re.match(r"^\|?[\s:|-]+\|?$", sep) and "-" in sep:
                cells = [_norm(c) for c in line.strip().strip("|").split("|")]
                tables.append(cells)
    return tables


def _is_subsequence(required: list, actual: list) -> int | None:
    """Return index of first missing required item, or None if all present in order."""
    it = iter(actual)
    for idx, item in enumerate(required):
        for cand in it:
            if cand == item:
                break
        else:
            return idx
    return None


def _strip_comments(text: str) -> str:
    return _RE_COMMENT.sub("", text)


def check_structure(stem: str, living: str, skeleton: str) -> list[str]:
    errors: list[str] = []

    req_headings = parse_headings(skeleton)
    act_headings = parse_headings(living)
    miss = _is_subsequence(req_headings, act_headings)
    if miss is not None:
        lvl, txt = req_headings[miss]
        errors.append(
            f"[{stem}] missing/renamed/out-of-order heading (level {lvl}): '{txt}'"
        )

    req_tables = parse_table_columns(skeleton)
    act_tables = parse_table_columns(living)
    act_pool = list(act_tables)
    for cols in req_tables:
        if cols in act_pool:
            act_pool.remove(cols)
        else:
            errors.append(f"[{stem}] missing table with columns: {cols}")

    for m in _RE_HARNESS.finditer(living):
        snippet = living[max(0, m.start() - 30): m.start() + 20].replace("\n", " ")
        errors.append(f"[{stem}] forbidden .harness reference near: ...{snippet}...")
        break

    return errors


def check_content(stem: str, living: str) -> list[str]:
    """Flag empty leaf sections (no content and no N/A) and stray placeholders."""
    errors: list[str] = []
    prose = _strip_comments(living)

    for m in _RE_HOLDER.finditer(prose):
        errors.append(f"[{stem}] unresolved placeholder: '{m.group(0)}'")
        break

    # Split into sections by heading; a leaf section (no deeper heading right
    # after it) must contain non-blank body text or a Not Applicable marker.
    lines = prose.splitlines()
    heads = [(i, _RE_HEADING.match(ln)) for i, ln in enumerate(lines)]
    head_idx = [(i, len(m.group(1)), _norm(m.group(2))) for i, m in heads if m]

    for n, (i, lvl, txt) in enumerate(head_idx):
        end = head_idx[n + 1][0] if n + 1 < len(head_idx) else len(lines)
        # Leaf = next heading (if any) is not deeper than this one.
        is_leaf = not (n + 1 < len(head_idx) and head_idx[n + 1][1] > lvl)
        if not is_leaf:
            continue
        body = "\n".join(lines[i + 1:end]).strip()
        has_table = "|" in body and "---" in body
        if not body and not has_table:
            errors.append(f"[{stem}] empty section needs content or 'Not Applicable': '{txt}'")
        elif body and not has_table and not body.replace("#", "").strip():
            errors.append(f"[{stem}] empty section needs content or 'Not Applicable': '{txt}'")
    return errors


def main() -> None:
    parser = argparse.ArgumentParser(description="Verify QMS docs conform to template skeletons.")
    parser.add_argument("files", nargs="*", help="Specific living .md files (default: all six).")
    parser.add_argument("--check-content", action="store_true",
                        help="Also flag empty sections and unresolved placeholders.")
    parser.add_argument("--living-dir", default="docs/qms")
    parser.add_argument("--skeleton-dir", default="docs/qms-templates/skeletons")
    args = parser.parse_args()

    living_dir = Path(args.living_dir)
    skel_dir = Path(args.skeleton_dir)

    if args.files:
        stems = [Path(f).stem for f in args.files]
    else:
        stems = DOC_STEMS

    all_errors: list[str] = []
    for stem in stems:
        living_path = living_dir / f"{stem}.md"
        skel_path = skel_dir / f"{stem}.skeleton.md"
        if not skel_path.exists():
            if stem in PENDING_STEMS:
                print(f"  SKIP: {stem} (skeleton pending PDLM template)")
                continue
            all_errors.append(f"[{stem}] skeleton not found: {skel_path} (run Extract-QmsSkeleton.py)")
            continue
        if not living_path.exists():
            all_errors.append(f"[{stem}] living document not found: {living_path}")
            continue

        living = living_path.read_text(encoding="utf-8")
        skeleton = skel_path.read_text(encoding="utf-8")

        errs = check_structure(stem, living, skeleton)
        if args.check_content:
            errs += check_content(stem, living)
        if errs:
            all_errors.extend(errs)
        else:
            print(f"  OK: {stem}")

    if all_errors:
        print("\nQMS STRUCTURE CONFORMANCE FAILED:\n", file=sys.stderr)
        for e in all_errors:
            print(f"  - {e}", file=sys.stderr)
        sys.exit(1)

    print("\nAll QMS documents conform to their template skeletons.")


if __name__ == "__main__":
    main()

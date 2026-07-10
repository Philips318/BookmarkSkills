# Field Guide — Product Defect Analysis

How to source and word each of the 11 fields in the "Investigated" section. Keep answers precise,
review-ready, and traceable to the Code Review Report and SSRS/SRS inputs.

---

## 1. Describe the impact including possible work-around for the Customer

- **Source**: Code Review Report → problem background / symptom.
- **Write**: the user-visible effect in the clinical workflow (which application, which action,
  what wrong outcome). Then a manual work-around the customer can apply, or state explicitly that
  no configuration-level work-around prevents the issue.
- **Example**: "In CTC Mark Polyp, a single click creates 2 polyps... Work-around: manually delete
  the duplicate marker. No configuration-level work-around exists."

## 2. How to recover the system back from failure mode?

- **Source**: Code Review Report → severity / stability.
- **Write**: if the application stays stable, say no system recovery is needed and describe the simple
  user correction (delete duplicate, re-enter, save again). If it requires restart/service action,
  list the concrete steps. State whether there is data loss.

## 3. Root cause-analysis of this problem

- **Source**: Code Review Report → root cause section (reuse the exact mechanism).
- **Write**: name the responsible method/class and the precise mechanism (e.g. fallback compared a
  changed `m_CurrentCommand`, causing double dispatch). Then one sentence on how the fix breaks the
  chain. Be specific — this is the most scrutinized field.

## 4. Is the issue present in the installed base (yes/no)?

- **Source**: SSRS `Target Release` / `Rationale` + code history.
- **Rule**: if the defect is in shared or pre-existing code (the review usually says so), answer **Yes**.
  - **Oldest release**: derive from SSRS target release / "existed prior to PEPF x.y" rationale. If the
    exact earliest shipping release is not derivable from inputs, write the best-supported answer and
    mark "to confirm against version history of <file>."
  - **Affected configurations**: copy the SSRS `Product:` line (e.g. "Incisive CT, CT 5300, CT7900,
    CT6500, Astra") — i.e. every configuration shipping the affected component.

## 5. What is the frequency of occurrence?

- **Source**: Code Review Report → reproducibility of the symptom.
- **Choose one** and justify:
  - **Occurs every time** — deterministic symptom (the logs/test reproduce it on every trigger).
  - **May occur** — depends on a specific condition/timing.
  - **Not expected to occur** — theoretical / not observed.

## 6. Proposed solution (technical risk + reliability impact)

- **Source**: Code Review Report → fix description + regression-risk dimension.
- **Write**: the concrete code change (what variable/comparison/logic changed and why it fixes it).
  Then the risk assessment: typically **Low** — no new public/internal API, no thread/shared-state,
  no resource allocation, no new event subscriptions. Reference **Change Point Analysis**: state the
  blast radius is confined to the changed decision point, and reliability impact is positive.

## 7. DHF/DMR documents to create or modify

- **Source**: Code Review Report → test impact.
- **Write**: usually only the **Test Specification** is updated (new regression test). State that the
  requirement, IFU, and purchase specs are unchanged **if** the requirement remains valid and the
  product was simply failing to meet it.

## 8. Is testing required?

- **Source**: Code Review Report → Test Plan / new tests.
- **Write**: "Yes." Reference the **specific** regression test (file/class) and the **legacy NUnit /
  test chain** to run it through, plus the **manual re-execution** steps (the P0 verification path
  across the affected views). If you argue testing is not required, give an explicit rationale.

## 9. Is an update of the IFU / SMI required?

- **Source**: nature of the defect.
- **Write**: "No" for internal behavior fixes that do not change user instructions, intended use, or
  safety information. Otherwise specify what content is added/removed.

## 10. My (Investigator's) advice

- **Source**: synthesis.
- **Write**: accept or reject the fix; the pre-commit actions (run the regression test through the
  correct pipeline, runtime smoke test, keep unrelated changes out for a ticket-atomic commit); and,
  if applicable, recommend linking duplicate tickets that share the root cause.

## 11. Identify/Confirm failing product requirement

- **Source**: SSRS/SRS → `Requirement ID:` + `Requirement:` text.
- **Write**: quote the exact requirement ID and text that the defect violates. Add related IDs when
  more than one requirement is implicated (e.g. a "mark polyps" requirement and an "enter polyp
  information" requirement).
- **Never** invent an ID — it must come from the SSRS input.

---

## "To confirm" policy

When an input does not support a definitive value (commonly the exact oldest affected release), do not
guess. Provide the best-supported statement and append a short "(to confirm against ...)" note so the
investigator knows what to verify before sign-off.

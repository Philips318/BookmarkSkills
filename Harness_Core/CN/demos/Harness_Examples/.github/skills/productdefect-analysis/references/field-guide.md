# Field Guide — Product Defect Analysis

如何为 "Investigated" section 的 11 个 fields 提供来源和措辞。答案应 precise、review-ready，并可追溯到 Code Review Report 和 SSRS/SRS inputs。

---

## 1. Describe the impact including possible work-around for the Customer

- **Source**: Code Review Report → problem background / symptom。
- **Write**: 写出 clinical workflow 中 user-visible effect（哪个 application、哪个 action、什么 wrong outcome）。然后给出 customer 可采用的 manual work-around，或明确说明没有 configuration-level work-around 能防止该 issue。
- **Example**: "In CTC Mark Polyp, a single click creates 2 polyps... Work-around: manually delete the duplicate marker. No configuration-level work-around exists."

## 2. How to recover the system back from failure mode?

- **Source**: Code Review Report → severity / stability。
- **Write**: 如果 application 保持 stable，说明不需要 system recovery，并描述简单 user correction（delete duplicate、re-enter、save again）。如果需要 restart/service action，列出具体 steps。说明是否存在 data loss。

## 3. Root cause-analysis of this problem

- **Source**: Code Review Report → root cause section（复用 exact mechanism）。
- **Write**: 写出 responsible method/class 和 precise mechanism（例如 fallback compared a changed `m_CurrentCommand`, causing double dispatch）。然后用一句话说明 fix 如何打断该 chain。要具体 — 这是最受审查的 field。

## 4. Is the issue present in the installed base (yes/no)?

- **Source**: SSRS `Target Release` / `Rationale` + code history。
- **Rule**: 如果 defect 在 shared 或 pre-existing code 中（review 通常会说明），回答 **Yes**。
  - **Oldest release**: 从 SSRS target release / "existed prior to PEPF x.y" rationale 推导。如果 exact earliest shipping release 无法从 inputs 推导，写出 best-supported answer，并标记 "to confirm against version history of <file>."
  - **Affected configurations**: 复制 SSRS `Product:` line（例如 "Incisive CT, CT 5300, CT7900, CT6500, Astra"）— 即所有 shipping affected component 的 configurations。

## 5. What is the frequency of occurrence?

- **Source**: Code Review Report → symptom 的 reproducibility。
- **Choose one** 并 justify：
  - **Occurs every time** — deterministic symptom（logs/test 每次 trigger 都 reproduce）。
  - **May occur** — 取决于特定 condition/timing。
  - **Not expected to occur** — theoretical / not observed。

## 6. Proposed solution (technical risk + reliability impact)

- **Source**: Code Review Report → fix description + regression-risk dimension。
- **Write**: 具体 code change（哪个 variable/comparison/logic 变了，以及为什么能修复）。然后写 risk assessment：通常为 **Low** — no new public/internal API、no thread/shared-state、no resource allocation、no new event subscriptions。引用 **Change Point Analysis**：说明 blast radius confined to the changed decision point，且 reliability impact 为 positive。

## 7. DHF/DMR documents to create or modify

- **Source**: Code Review Report → test impact。
- **Write**: 通常只更新 **Test Specification**（new regression test）。如果 requirement 仍然有效且 product 只是未满足它，则说明 requirement、IFU 和 purchase specs unchanged。

## 8. Is testing required?

- **Source**: Code Review Report → Test Plan / new tests。
- **Write**: "Yes." 引用**具体** regression test（file/class）以及运行它的 **legacy NUnit / test chain**，再给出 **manual re-execution** steps（affected views 的 P0 verification path）。如果主张不需要 testing，必须给出 explicit rationale。

## 9. Is an update of the IFU / SMI required?

- **Source**: defect nature。
- **Write**: 对不改变 user instructions、intended use 或 safety information 的 internal behavior fixes，写 "No"。否则说明 added/removed content。

## 10. My (Investigator's) advice

- **Source**: synthesis。
- **Write**: accept 或 reject fix；pre-commit actions（通过 correct pipeline 运行 regression test、runtime smoke test、保持 ticket-atomic commit，不包含 unrelated changes）；如适用，建议 link duplicate tickets that share the root cause。

## 11. Identify/Confirm failing product requirement

- **Source**: SSRS/SRS → `Requirement ID:` + `Requirement:` text。
- **Write**: quote defect 违反的 exact requirement ID 和 text。如涉及多个 requirement，则添加 related IDs（例如 "mark polyps" requirement 和 "enter polyp information" requirement）。
- **Never** invent an ID — 它必须来自 SSRS input。

---

## "To confirm" policy

当 input 不能支持 definitive value（常见的是 exact oldest affected release）时，不要猜测。提供 best-supported statement，并追加简短的 "(to confirm against ...)" note，让 investigator 知道 sign-off 前要验证什么。

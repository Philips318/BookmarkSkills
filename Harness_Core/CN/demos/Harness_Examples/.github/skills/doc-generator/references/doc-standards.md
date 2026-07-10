## IEC 62304 Documentation Requirements by Safety Class

### Class A — Standard
- Software Development Plan
- Software Requirements Specification (SRS)
- Software Architecture (basic)
- Verification: Unit tests pass

### Class B — + Detailed Design
- Everything in Class A, plus:
- Software Design Specification (SDS) — detailed module design
- Integration test plan
- Risk analysis / Impact Analysis for changes
- Traceability: Requirement → Design → Test

### Class C — + Comprehensive
- Everything in Class B, plus:
- DFMEA with RPN scores
- Detailed risk mitigation for each identified hazard
- Dual review for safety-critical code
- Complete traceability matrix
- Validation test plan

## Document Quality Checklist

提交任何 generated document 供 review 前：

- [ ] 所有 `[TBD]` placeholders 已解决或明确标记
- [ ] Mermaid diagrams 能正确渲染（在 VS Code 中 preview）
- [ ] Code examples 可编译且语法正确
- [ ] Version number 和 date 是当前值
- [ ] Traceability links 有效（requirement IDs 存在）
- [ ] Examples 中没有 confidential information 或 patient data
- [ ] 整个 document 中 terminology 一致
- [ ] Safety class 已清楚说明（if applicable）

## Mermaid Diagram Conventions

- 静态结构使用 `classDiagram`
- Runtime interactions 使用 `sequenceDiagram`
- Component/dependency diagrams 使用 `graph TD`（top-down）
- State machines 使用 `stateDiagram-v2`
- Process flows 使用 `flowchart LR`（left-right）
- 使用 `:::critical` 或 red styling 标出 critical/safety paths
- Diagrams 保持聚焦 — 每个 diagram 最多 10-12 entities

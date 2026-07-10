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

Before submitting any generated document for review:

- [ ] All `[TBD]` placeholders resolved or explicitly flagged
- [ ] Mermaid diagrams render correctly (preview in VS Code)
- [ ] Code examples compile and are syntactically correct
- [ ] Version number and date are current
- [ ] Traceability links are valid (requirement IDs exist)
- [ ] No confidential information or patient data in examples
- [ ] Consistent terminology throughout the document
- [ ] Safety class is clearly stated (if applicable)

## Mermaid Diagram Conventions

- Use `classDiagram` for static structure
- Use `sequenceDiagram` for runtime interactions
- Use `graph TD` (top-down) for component/dependency diagrams
- Use `stateDiagram-v2` for state machines
- Use `flowchart LR` (left-right) for process flows
- Color critical/safety paths with `:::critical` or red styling
- Keep diagrams focused — max 10-12 entities per diagram

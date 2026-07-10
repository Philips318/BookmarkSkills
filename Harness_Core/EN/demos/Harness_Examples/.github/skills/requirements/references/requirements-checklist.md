- Every AC must map to at least one Gherkin scenario.
- Gherkin scenarios must be independently executable.
- No scenario should depend on another scenario's side effects.
- Use concrete values in examples, not placeholders.

## Acceptance Criteria

Each AC must satisfy:

1. **Atomic** — tests exactly one behavior.
2. **Independent** — no dependency on other ACs.
3. **Unambiguous** — only one interpretation possible.
4. **Measurable** — pass/fail can be determined objectively.
5. **Traceable** — links back to the parent User Story.

### Coverage Matrix

For each requirement, verify AC coverage across:

| Category | Covered? | Example |
|----------|----------|---------|
| Happy path | ☐ | Normal user flow |
| Alternative flow | ☐ | Different valid input |
| Boundary (min) | ☐ | Minimum allowed value |
| Boundary (max) | ☐ | Maximum allowed value |
| Invalid input | ☐ | Out-of-range, null, empty |
| Error recovery | ☐ | Network failure, timeout |
| Concurrency | ☐ | Simultaneous access |
| Security | ☐ | Unauthorized access attempt |

## NFR Checklist

| Category | Question | Required for |
|----------|----------|-------------|
| Performance | Response time < ? ms | All |
| Performance | Memory usage < ? MB | Image processing |
| Security | Authentication required? | Patient data |
| Security | Audit logging needed? | Class B/C |
| Compatibility | DICOM version? | Imaging modules |
| Compatibility | Min OS version? | All |
| Usability | Keyboard navigation? | Clinical workflow |
| Reliability | Auto-save / recovery? | Data entry |

## Completeness Score Rubric

- **90-100**: Ready for Three Amigos, minimal discussion needed.
- **70-89**: Good draft, needs targeted discussion on flagged areas.
- **50-69**: Significant gaps, stakeholder clarification needed before proceeding.
- **Below 50**: Requirement too vague, return to stakeholder with specific questions.

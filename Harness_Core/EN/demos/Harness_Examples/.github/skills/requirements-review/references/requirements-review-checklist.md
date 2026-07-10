## Requirements Review Checklist

### AC Coverage Matrix

For each requirement, verify AC coverage across:

| Category | Covered? | Question to Ask |
|----------|----------|----------------|
| Happy path | ☐ | Is the normal/expected flow fully described? |
| Alternative flow | ☐ | Are there other valid ways to achieve the goal? |
| Boundary (min) | ☐ | What happens at the minimum allowed value? |
| Boundary (max) | ☐ | What happens at the maximum allowed value? |
| Boundary (zero/empty) | ☐ | What happens with zero, null, or empty input? |
| Invalid input | ☐ | What happens with out-of-range or malformed input? |
| Error recovery | ☐ | What happens on network failure, timeout, or crash? |
| Concurrency | ☐ | What happens with simultaneous access? |
| Security | ☐ | Is unauthorized access handled? |
| State transitions | ☐ | Are all valid state changes covered? |
| Undo / Cancel | ☐ | Can the action be reversed? What happens on cancel? |
| Performance | ☐ | Is there a response time or throughput requirement? |

### Ambiguity Detector — Flagged Terms

These words/phrases trigger an automatic finding if found in AC or NFR:

| Pattern | Why It's a Problem |
|---------|-------------------|
| "fast", "slow", "quickly" | No measurable threshold |
| "user-friendly", "intuitive" | Subjective |
| "reliable", "robust" | No failure/recovery spec |
| "appropriate", "suitable", "reasonable" | Undefined criteria |
| "etc.", "and so on", "and more" | Incomplete enumeration |
| "should" (without "shall") | Ambiguous obligation level |
| "if possible", "ideally" | Optional or mandatory? |
| "some", "many", "few" | No quantity specified |
| "normally", "usually", "typically" | What about abnormal cases? |
| "similar to", "like" | Define exactly |

### NFR Completeness Check

| NFR Category | Required Fields |
|-------------|----------------|
| Performance | Metric + threshold + measurement method + load condition |
| Security | Authentication method + authorization scope + audit requirement |
| Compatibility | Protocol version + OS versions + hardware constraints |
| Usability | Interaction count + accessibility standard + localization scope |
| Reliability | MTBF + recovery time + data integrity guarantee |

### IEC 62304 Class-Specific Checks

| Check | Class A | Class B | Class C |
|-------|---------|---------|---------|
| User Story | ✓ | ✓ | ✓ |
| AC (Given/When/Then) | ✓ | ✓ | ✓ |
| NFR with thresholds | ✓ | ✓ | ✓ |
| Verification Methods | ✓ | ✓ | ✓ |
| Impact Analysis | — | ✓ | ✓ |
| Risk Assessment | — | — | ✓ |
| Patient Safety Scenarios | — | — | ✓ |
| DFMEA Input | — | — | ✓ |

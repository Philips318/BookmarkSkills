# Software Requirements Specification

## PURPOSE

<!-- GUIDANCE: [This paragraph provides a brief statement of to what the software requirements apply. -->
<!-- GUIDANCE: Example: The purpose of this document is to provide requirements specifications for DMS Service SW. This SW provides application(s) can monitor, control and analyze the DMS HW feedbacks.] -->

## SCOPE

<!-- GUIDANCE: This record applies to the Philips CT/AMI. -->

## INTRODUCTION

<!-- GUIDANCE: <The Introduction shall include a definition of how the requirements specification was generated. Definition must include: -->
<!-- GUIDANCE: Tool Used (e.g. DOORS) -->
<!-- GUIDANCE: Repository Version (e.g. DOORS baseline) -->
<!-- GUIDANCE: Export Method (e.g. RPE, CSV) -->
<!-- GUIDANCE: Query/Filter Parameters (e.g. RPE template name/rev, Product and Target Release, etc.) -->
<!-- GUIDANCE: Post Processing Steps required (e.g. headings alignment, etc) -->
<!-- GUIDANCE: > -->

## OVERVIEW

<!-- GUIDANCE: <Provide a brief description of the SW modules and its use without describing its design.> -->

## Modules Requirements

<!-- GUIDANCE: <Specify the functional and non-functional requirements (for features and functions) per SW module. Include interface requirements, User Interface requirements and quality aspects requirements> -->
<!-- GUIDANCE: repeatable example section: <Module 1> -->
<!-- GUIDANCE: < The following are examples for topics to be considered, as applicable, when specifying requirements (this is not an exhaustive list of possibilities): -->
<!-- GUIDANCE: Functional requirements -->
<!-- GUIDANCE: Performance requirements -->
<!-- GUIDANCE: Ranges -->
<!-- GUIDANCE: Limits -->
<!-- GUIDANCE: Defaults -->
<!-- GUIDANCE: Data characteristics -->
<!-- GUIDANCE: Interfaces -->
<!-- GUIDANCE: Errors, warnings and operator messages -->
<!-- GUIDANCE: Security -->
<!-- GUIDANCE: User Interface -->
<!-- GUIDANCE: Error handling -->
<!-- GUIDANCE: Data base -->
<!-- GUIDANCE: Network -->
<!-- GUIDANCE: Maintenance -->
<!-- GUIDANCE: Regulatory -->
<!-- GUIDANCE: User documentation -->
<!-- GUIDANCE: Safety related > -->
<!-- GUIDANCE: repeatable example section: <Module n> -->
<!-- GUIDANCE: <Module n - requirements list> -->
<!-- GUIDANCE: repeatable example section: <SOUP AND LEGACY SOFTWARE REQUIREMENTS> -->
<!-- GUIDANCE: <for each SOUP or legacy SW item specify the following requirements necessary for its intended use as applicable: Functional and performance requirements, system hardware and software necessary to support the proper operation of the item, item acceptance criteria > -->
<!-- GUIDANCE: <Remove this subsection if no SOUP or Legacy items> -->
<!-- GUIDANCE: repeatable example section: <Item 1> -->
<!-- GUIDANCE: <Remove this subsection if no SOUP or Legacy items> -->

### Image Display — WW/WC Reset to Default (IEC 62304 Class B)

**IEC 62304 Safety Class:** B  
**Slug:** `ww-wc-reset`  
**Feature:** Reset Window/Level to Default button in the CT image display panel.

#### Functional Requirements

| ID | Requirement | Priority | DICOM Reference |
|----|-------------|----------|-----------------|
| FR-01 | The image display panel **shall** include a visible Reset Window/Level button present whenever a CT series is loaded in the active viewport. | Must | — |
| FR-02 | When activated, the Reset Window/Level button **shall** set the active viewport WW and WC to the values in DICOM tags (0028,1051) and (0028,1050) at index 0. The applied WW value **shall** be greater than 0. | Must | DICOM PS3.3 §C.7.6.3.1.5 |
| FR-03 | When DICOM tags (0028,1051)/(0028,1050) are multi-valued (VM > 1), the system **shall** reset to index-0 preset by default and **shall** include the preset name from (0028,1055)[0] in the confirmation message when that tag is present. | Must | DICOM PS3.3 §C.7.6.3.1.5 |
| FR-04 | When tags (0028,1051) or (0028,1050) are absent, the system **shall** apply a configurable software-defined fallback WW/WC (default WW = 400, WC = 40) sourced from application configuration. | Must | — |
| FR-05 | After a reset action, the system **shall** display a non-blocking confirmation message visible for ≥ 2 seconds that auto-dismisses without user interaction. | Must | — |
| FR-06 | The reset action **shall** be undoable via the standard application Undo function, restoring the WW/WC values active immediately before the reset. | Must | — |
| FR-07 | The system **shall** reject WW values ≤ 0 from any source (DICOM tag, fallback config, Undo stack), log a warning, apply the configurable fallback, and notify the user. | Must | — |

#### Non-Functional Requirements

| ID | Requirement | Metric | Category |
|----|-------------|--------|----------|
| NFR-01 | Reset action shall complete within 200 ms (95th percentile) on minimum-specification hardware. | ≤ 200 ms p95 | Performance |
| NFR-02 | Reset button shall be keyboard-operable via a documented shortcut (default: Ctrl+Shift+W) and shall have a descriptive tooltip; reachable via Tab navigation. | Keyboard + tooltip | Usability / Accessibility |
| NFR-03 | The reset function shall not log, transmit, or persist PHI as a result of the reset action. | Code review pass | Security |
| NFR-04 | The reset function shall not throw an unhandled exception under any supported DICOM dataset variant; failures shall be logged at WARNING level and the current WW/WC shall be preserved. | Zero unhandled exceptions | Reliability |
| NFR-05 | All code accessing DICOM tags (0028,1050), (0028,1051), (0028,1055) shall reference the tag numbers and DICOM PS3.3 §C.7.6.3.1.5 in XML doc comments; tag literals shall reside in a dedicated DICOM constants file. | Code review pass | Maintainability |

## TESTABILITY REQUIREMENTS

<!-- GUIDANCE: <Describe the testability requirements of the SW module> -->
<!-- GUIDANCE: [This section can be divided into SW modules or apply to all modules] -->
<!-- GUIDANCE: repeatable example section: <Module 1> -->
<!-- GUIDANCE: <When the requirements in this section apply to all SW modules, remove the sub-section header> -->

### Image Display — WW/WC Reset: Requirement-to-Verification Traceability

All functional and non-functional requirements for the WW/WC Reset feature are traced to one or more verification scenarios (BDD Gherkin), unit tests, or review activities. The scenario names below are the immutable contract — they correspond verbatim to the Gherkin `Scenario:` lines in the feature specifications.

| Requirement | Verification Method | Verification Scenario(s) |
|-------------|--------------------|--------------------------|
| FR-01 | BDD | Reset restores DICOM default Window Width and Window Center; Reset button is disabled when no series is loaded |
| FR-02 | Unit Test + BDD | Reset restores DICOM default Window Width and Window Center |
| FR-03 | Unit Test + BDD | Confirmation message includes the DICOM preset name |
| FR-04 | Unit Test + BDD | Fallback values applied when DICOM windowing tags are absent; Warning is logged when fallback values are used |
| FR-05 | BDD | Reset restores DICOM default Window Width and Window Center; Confirmation message auto-dismisses |
| FR-06 | BDD | Undo restores the previous Window Width and Window Center; Redo reapplies the reset after an undo; Undo is unavailable when no reset has been performed |
| FR-07 | Unit Test + BDD | Zero Window Width is rejected; Negative Window Width is rejected; Fallback applied when DICOM Window Width is zero; Fallback applied when DICOM Window Width is negative |
| NFR-01 | Performance Test | Measured during integration testing (95th percentile ≤ 200 ms on minimum-spec hardware) |
| NFR-02 | BDD / FlaUI | Keyboard shortcut triggers reset |
| NFR-03 | Code Review | Inspection of all code paths triggered by reset — confirm no PHI fields accessed or logged |
| NFR-04 | Unit Test + BDD | Fallback applied when DICOM Window Width is zero; Fallback applied when DICOM Window Width is negative |
| NFR-05 | Unit Test + Code Review | DICOM display tag constants map to correct tag numbers; code review verifies XML doc comments reference PS3.3 §C.7.6.3.1.5 |

## INSTALLATIONS AND DEPLOYMENT REQUIREMENTS

<!-- GUIDANCE: < Specify SW installation, upgrade, uninstall, backup and restore requirements. > -->
<!-- GUIDANCE: repeatable example section: <Module 1> -->
<!-- GUIDANCE: <Describe the installation requirements of the SW module> -->

## CROSS MODULES REQUIREMENTS

<!-- GUIDANCE: <specify the cross modules requirements that relevant for all SW modules. > -->
<!-- GUIDANCE: When there are no Cross Modules Requirements, remove this section. -->
<!-- GUIDANCE: When the requirements in this section apply to all SW modules, remove sub-section headers -->
<!-- GUIDANCE: The following are examples for topics to be considered, as applicable, when specifying requirements (this is not an exhaustive list of possibilities): -->
<!-- GUIDANCE: Non Functional requirements -->
<!-- GUIDANCE: Performance requirements -->
<!-- GUIDANCE: Ranges -->
<!-- GUIDANCE: Limits -->
<!-- GUIDANCE: Defaults -->
<!-- GUIDANCE: Data characteristics -->
<!-- GUIDANCE: Interfaces -->
<!-- GUIDANCE: Errors, warnings and operator messages -->
<!-- GUIDANCE: Security -->
<!-- GUIDANCE: User Interface -->
<!-- GUIDANCE: Error handling -->
<!-- GUIDANCE: Data base -->
<!-- GUIDANCE: Network -->
<!-- GUIDANCE: Maintenance -->
<!-- GUIDANCE: Regulatory -->
<!-- GUIDANCE: User documentation -->
<!-- GUIDANCE: Safety related -->
<!-- GUIDANCE: repeatable example section: <Topic 1> -->
<!-- GUIDANCE: < When the requirements in this section apply to all SW modules, remove the sub-section header> -->
<!-- GUIDANCE: repeatable example section: <Topic n> -->
<!-- GUIDANCE: < When the requirements in this section apply to all SW modules, remove the sub-section header> -->

## TERMS AND ABBREVIATIONS

| Term / Abbreviation | Description |
| --- | --- |
| WW | Window Width — DICOM tag (0028,1051); controls the contrast range of the displayed CT image. Must be > 0. |
| WC | Window Center (Window Level) — DICOM tag (0028,1050); controls the brightness midpoint of the displayed CT image. |
| WL | Window Level — synonym for Window Center (WC). |
| DICOM | Digital Imaging and Communications in Medicine — international standard for medical imaging data. |
| IEC 62304 | International standard for medical device software lifecycle processes. |
| VM | Value Multiplicity — the number of values a DICOM attribute may hold. |
| DS | Decimal String — DICOM Value Representation used for WW and WC. |
| PHI | Protected Health Information. |
| MVVM | Model-View-ViewModel — WPF architectural pattern used in this application. |

## APPENDICES

| Appendix | Title |
| --- | --- |

## REFERENCES

### External References

| Document ID | Document Title |
| --- | --- |

### Internal References

| Document ID | Document Title |
| --- | --- |

## RECORD CHANGE SUMMARY

| Revision | Document Change No. | Document Editor | Description of Change |
| --- | --- | --- | --- |
| 1.0 | TBD | @analyst (AI) | Initial entry: added Image Display — WW/WC Reset to Default requirements (FR-01 to FR-07, NFR-01 to NFR-05); IEC 62304 Class B. Date: 2026-07-02. |
| 1.1 | TBD | @product-owner (AI) | Added TESTABILITY REQUIREMENTS: Requirement-to-Verification Traceability table mapping FR-01–FR-07 and NFR-01–NFR-05 to 17 Gherkin scenarios across 4 tasks. Date: 2026-07-02. |

## RECORD APPROVALS

| Signature Reason | Function | Name |
| --- | --- | --- |

<!-- GUIDANCE: Signatures and dates are captured in PLM tool as part of the document change order. -->
<!-- GUIDANCE: repeatable example section: APPENDIX <x> – <Title> -->

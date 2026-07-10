# Boilerplate Text — Standard DMR Wording

Companion reference for the `generate-3pp-dmr` skill. Use these strings **verbatim**. Do not paraphrase — they are the standard wording across released 3PP DMRs.

## Purpose (Section 1)
```
This document describes the detailed technical design of an element of the product.
```

## Scope (Section 2)
```
This document applies to Philips CT/AMI.
```

## Confidentiality statement (top of Section 6 Design Details)
```
All sheets of this document contain confidential and proprietary information of Philips healthcare ("Philips") and are intended for use by current Philips personnel. Copying, disclosure to others, or other use is prohibited without the express written authorization of the Philips' law department. Report violations of this requirement to the Philips law department.
```

## N/A statements (third-party responsibility)

Architecture Views (Section 5):
```
N/A-This is a 3rd party item and the architecture views are the responsibility of the manufacturer.
```

Allocation of Quality Aspects (6.1):
```
N/A-This is a 3rd party item and the quality aspects are the responsibility of the manufacturer.
```

Element detailed design (6.2):
```
N/A-This is a 3rd party item and the detailed design is the responsibility of the manufacturer.
```

Design Constraints (6.4.1):
```
N/A-This is a 3rd party item, and the manufacturer is responsible for the design.
```

Design robustness (6.5):
```
N/A-This is a 3rd party item and the design robustness is the responsibility of the manufacturer.
```

## Interfaces (6.3) — pattern
```
<Product> interfaces with CT gantry by an electrical cable designed by Philips. The 12 NC of this cable is <cable 12NC(s)>.
```

## Functionality (6.4.1) — patterns
Injector:
```
<Product> is used to inject contrast agent and normal saline into the human body in the Contrast Scan workflow to improve the contrast of CT images.
```
SAS interface feature (add when applicable):
```
Spiral Auto Scan or Start Automatic Scan (SAS) - Provides an interface that triggers both the CT scan process and the injector injection process from the injector panel at the same time.
```

## Compatibility Statement (6.4.1)
```
Compatibility Statement can be found in Appendix <A, B and C>.
```

## Installation (6.4.1)
```
<Product> is a 3rd party injector, the installation shall refer the service manual of <Manufacturer>.
```

## Appendix entry (Section 9) — pattern
```
The file "<Appendix file name>" is attached to this record in the PLM tool and represents Appendix <X> of this document. This is a file of external origin.
```

## Closing note (end of document)
```
Note: for template information, see custom properties of this document.
```

## Standard Terminology rows (Section 3)
| Term | Definition |
|---|---|
| CT | Computed Tomography |
| DMR | Device Master Record |
| P/N | Part Number |

Add product-specific terms (e.g., SAS = Start Automatic Scan, 12NC = 12 Numeric Code, CANOpen) when they appear in the inputs.

## Draft disclaimer (communicate to the user, not inserted into the DMR body)
```
This DMR is an AI-generated draft. It must be reviewed by Systems Engineering and go through the standard release/approval workflow in the PLM tool before use. Verify every 12NC, model number, product number, and document ID against the source CIP/TRR. Replace all [TBD] placeholders.
```

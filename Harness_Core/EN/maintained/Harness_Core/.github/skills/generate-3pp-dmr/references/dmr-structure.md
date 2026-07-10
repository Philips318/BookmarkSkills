# DMR Structure — Per-Section Specification

Companion reference for the `generate-3pp-dmr` skill. Defines every section of the standard Philips 3PP Device Master Record, what goes in it, and where the content comes from.

Legend: **(boilerplate)** = fixed wording · **(N/A)** = standard third-party N/A statement · **(data)** = filled from inputs · **(reference)** = points to an appendix/external doc.

---

## Front matter

- **Title** — `<Manufacturer> <Product> <variant>` (e.g., "Guerbet OptiVantage Injector").
- **Table of Contents** — auto-generated from the headings (the Word build script inserts a TOC field).
- **Document properties** — Document ID (e.g., `D001580671`), 12NC(s), revision. Stored in custom properties; the visible note at the end reads: "Note: for template information, see custom properties of this document."

## 1 Purpose *(boilerplate)*
> This document describes the detailed technical design of an element of the product.

## 2 Scope *(boilerplate)*
> This document applies to Philips CT/AMI.

## 3 Terminology & Abbreviations *(data table)*
Two-column table: **Terminology & Abbreviations | Description/Definition**. Always include:

| Terminology & Abbreviations | Description/Definition |
|---|---|
| CT | Computed Tomography |
| DMR | Device Master Record |
| P/N | Part Number |

Add product-specific terms found in the inputs (e.g., SAS, CANOpen, 12NC) as needed.

## 4 Overview *(data)*
- One sentence defining the product and the Philips systems it applies to: "This document defines the specifications of `<Product>` for `<list of Philips CT systems with product numbers>` as detailed in the PLM tool."
- Sentence: "The detail 12NC list of `<Product>` are listed below:"
- **12NC table** — three columns:

| 12NC | 12NC Name | Model Numbers and Description |
|---|---|---|
| `<12NC>` | `<short name>` | `<model + description>` |

Source: CIP product identification + spec sheets. Copy 12NCs verbatim.

## 5 Architecture Views *(N/A)*
> N/A-This is a 3rd party item and the architecture views are the responsibility of the manufacturer.

## 6 Design Details
Begin the section with the standard **confidentiality statement** (see boilerplate-text.md) immediately under the heading.

### 6.1 Allocation of Quality Aspects *(N/A)*
> N/A-This is a 3rd party item and the quality aspects are the responsibility of the manufacturer.

### 6.2 Element detailed design *(N/A)*
> N/A-This is a 3rd party item and the detailed design is the responsibility of the manufacturer.

### 6.3 Interfaces *(data)*
Describe how the accessory connects to the CT gantry. Typical pattern:
> `<Product>` interfaces with CT gantry by an electrical cable designed by Philips. The 12 NC of this cable is `<cable 12NC(s)>`.

Source: TRR interface confirmation (CANOpen / SAS) + CIP. Include cable 12NCs if present.

### 6.4 Parts
#### 6.4.1 `<Product>`
- **Functionality** *(data)* — What the accessory does in the clinical workflow. Source: supplier docs / IFU / TRR. Example: "`<Product>` is used to inject contrast agent and normal saline into the human body in the Contrast Scan workflow to improve the contrast of CT images." Include interface features such as SAS (Start Automatic Scan) if applicable.
- **Design Constraints** *(N/A)* — "N/A-This is a 3rd party item, and the manufacturer is responsible for the design."
- **Compatibility Statement** *(reference)* — "Compatibility Statement can be found in Appendix A, B and C." (Match the actual appendix letters used.)
- **Manufacturer** *(data table)*:

| Manufacturer | Manufacturer Model | Manufacturer P/N |
|---|---|---|
| `<mfr>` | `<model family>` | `<P/N list>` |

- **Installation** *(reference)* — "`<Product>` is a 3rd party injector, the installation shall refer the service manual of `<Manufacturer>`."
- **Affected Products** *(data table)* — Philips CT systems. Header row "Philips CT System" spanning, then:

| Product | Product Number |
|---|---|
| `<Philips CT system>` | `<product number(s)>` |

Source: CIP affected products + TRR scope. Copy product numbers verbatim.

### 6.5 Design robustness *(N/A)*
> N/A-This is a 3rd party item and the design robustness is the responsibility of the manufacturer.

## 7 References *(data table)*

| Reference Number | Document Title | Document ID |
|---|---|---|
| 1 | `<TRR title>` | `<Doc ID> Rev <X>` |

Source: TRR document IDs + revisions. List every TRR that established compatibility.

## 8 Document Revision History *(data table)*

| Revision | Release Date | Author | Description of changes | CR / Reason |
|---|---|---|---|---|
| A | Per PLM tool | `<author>` | Initial release | `<CN/CR>` |

For initial release use Revision A / "Initial release". For an update, add a new row describing the change and the CR/CN number.

## 9 Appendices *(data)*
One subsection per attachment. Standard wording:
> The file "`<Appendix file name>`" is attached to this record in the PLM tool and represents Appendix `<X>` of this document. This is a file of external origin.

Typical appendices: compatibility statements (per Philips system family) and the supplier operator/service manual.

End the document with: "Note: for template information, see custom properties of this document."

---

## Checklist

- [ ] Title, TOC, and document ID present.
- [ ] Purpose and Scope use exact boilerplate.
- [ ] Terminology table includes CT, DMR, P/N (+ product-specific terms).
- [ ] Overview names the product and all affected Philips systems.
- [ ] 12NC table populated (every 12NC verbatim) or `[TBD]`.
- [ ] All N/A sections present with exact standard wording.
- [ ] Interfaces section describes the gantry connection / cable 12NC.
- [ ] Functionality described from supplier/IFU input.
- [ ] Manufacturer table populated (Manufacturer | Model | P/N).
- [ ] Affected Products table lists Philips systems + product numbers.
- [ ] References table lists the TRR(s) with Document ID + revision.
- [ ] Revision history correct (A / initial release, or incremented).
- [ ] Appendices listed with the standard "attached in the PLM tool" wording.
- [ ] Confidentiality statement present in Design Details.
- [ ] Every unknown value is a visible `[TBD]`.
- [ ] Draft disclaimer communicated to the user.

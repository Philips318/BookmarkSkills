# DMR Structure — Per-Section Specification

`generate-3pp-dmr` skill 的 companion reference。定义 standard Philips 3PP Device Master Record 的每个 section、内容以及来源。

Legend: **(boilerplate)** = fixed wording · **(N/A)** = standard third-party N/A statement · **(data)** = filled from inputs · **(reference)** = points to an appendix/external doc。

---

## Front matter

- **Title** — `<Manufacturer> <Product> <variant>`（例如 "Guerbet OptiVantage Injector"）。
- **Table of Contents** — auto-generated from the headings（Word build script inserts a TOC field）。
- **Document properties** — Document ID（例如 `D001580671`）、12NC(s)、revision。存储于 custom properties；文末 visible note 为："Note: for template information, see custom properties of this document."

## 1 Purpose *(boilerplate)*
> This document describes the detailed technical design of an element of the product.

## 2 Scope *(boilerplate)*
> This document applies to Philips CT/AMI.

## 3 Terminology & Abbreviations *(data table)*
Two-column table: **Terminology & Abbreviations | Description/Definition**。始终包含：

| Terminology & Abbreviations | Description/Definition |
|---|---|
| CT | Computed Tomography |
| DMR | Device Master Record |
| P/N | Part Number |

根据需要添加 inputs 中出现的 product-specific terms（例如 SAS、CANOpen、12NC）。

## 4 Overview *(data)*
- 一句定义 product 及其适用的 Philips systems："This document defines the specifications of `<Product>` for `<list of Philips CT systems with product numbers>` as detailed in the PLM tool."
- Sentence: "The detail 12NC list of `<Product>` are listed below:"
- **12NC table** — three columns:

| 12NC | 12NC Name | Model Numbers and Description |
|---|---|---|
| `<12NC>` | `<short name>` | `<model + description>` |

Source: CIP product identification + spec sheets。12NCs verbatim copy。

## 5 Architecture Views *(N/A)*
> N/A-This is a 3rd party item and the architecture views are the responsibility of the manufacturer.

## 6 Design Details
Section heading 下立即放 standard **confidentiality statement**（见 boilerplate-text.md）。

### 6.1 Allocation of Quality Aspects *(N/A)*
> N/A-This is a 3rd party item and the quality aspects are the responsibility of the manufacturer.

### 6.2 Element detailed design *(N/A)*
> N/A-This is a 3rd party item and the detailed design is the responsibility of the manufacturer.

### 6.3 Interfaces *(data)*
描述 accessory 如何连接 CT gantry。Typical pattern:
> `<Product>` interfaces with CT gantry by an electrical cable designed by Philips. The 12 NC of this cable is `<cable 12NC(s)>`.

Source: TRR interface confirmation（CANOpen / SAS）+ CIP。如存在 cable 12NCs，则包含。

### 6.4 Parts
#### 6.4.1 `<Product>`
- **Functionality** *(data)* — accessory 在 clinical workflow 中做什么。Source: supplier docs / IFU / TRR。Example: "`<Product>` is used to inject contrast agent and normal saline into the human body in the Contrast Scan workflow to improve the contrast of CT images." 如果适用，包含 SAS（Start Automatic Scan）等 interface features。
- **Design Constraints** *(N/A)* — "N/A-This is a 3rd party item, and the manufacturer is responsible for the design."
- **Compatibility Statement** *(reference)* — "Compatibility Statement can be found in Appendix A, B and C."（匹配实际使用的 appendix letters。）
- **Manufacturer** *(data table)*:

| Manufacturer | Manufacturer Model | Manufacturer P/N |
|---|---|---|
| `<mfr>` | `<model family>` | `<P/N list>` |

- **Installation** *(reference)* — "`<Product>` is a 3rd party injector, the installation shall refer the service manual of `<Manufacturer>`."
- **Affected Products** *(data table)* — Philips CT systems。Header row "Philips CT System" spanning，然后：

| Product | Product Number |
|---|---|
| `<Philips CT system>` | `<product number(s)>` |

Source: CIP affected products + TRR scope。Product numbers verbatim copy。

### 6.5 Design robustness *(N/A)*
> N/A-This is a 3rd party item and the design robustness is the responsibility of the manufacturer.

## 7 References *(data table)*

| Reference Number | 文档标题 | ?? ID |
|---|---|---|
| 1 | `<TRR title>` | `<Doc ID> Rev <X>` |

Source: TRR document IDs + revisions。列出每个 established compatibility 的 TRR。

## 8 Document Revision History *(data table)*

| Revision | Release Date | Author | Description of changes | CR / Reason |
|---|---|---|---|---|
| A | Per PLM tool | `<author>` | Initial release | `<CN/CR>` |

Initial release 使用 Revision A / "Initial release"。Update 时添加新 row，描述 change 和 CR/CN number。

## 9 Appendices *(data)*
每个 attachment 一个 subsection。Standard wording:
> The file "`<Appendix file name>`" is attached to this record in the PLM tool and represents Appendix `<X>` of this document. This is a file of external origin.

Typical appendices: compatibility statements（per Philips system family）和 supplier operator/service manual。

文末写："Note: for template information, see custom properties of this document."

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

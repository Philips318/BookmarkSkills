# CT Domain Knowledge — IFU Generator Reference

Extracted from published Philips CT IFU documents:
- `300013585292_C_spectral_ct_Verida_Family_ifu_en-us.docx` (Spectral CT Verida Family, 594 pages)
- `300009380645_A_CT Rembra RT_CT Areta RT_CT Rembra_IFU-en-US.docx` (Rembra/Areta, 14230 paragraphs)

---

## 1. CT Glossary / Terminology

### Imaging & Reconstruction

| Term | Full Name | Description |
|------|-----------|-------------|
| **SBI** | Spectral Base Image | Base dataset for spectral analysis; acquired at two energy levels |
| **MonoE** | Mono-energetic | Simulated images at a specific keV (40–200 keV); reduces beam hardening |
| **iDose4** | Iterative Dose Reduction 4 | Iterative reconstruction technology for low-dose imaging |
| **O-MAR** | Orthopedic Metal Artifact Reduction | Reduces metal artifacts from implants; two results generated (with/without) |
| **EFOV** | Extended Field of View | Reconstruction beyond standard FOV (>500 mm); accuracy reduces at edges |
| **VNC** | Virtual Non-Contrast | Simulated non-contrast images from spectral data; body scans only |
| **MPR** | Multi-Planar Reconstruction | Viewing in sagittal, coronal, and oblique planes |
| **MIP** | Maximum Intensity Projection | Projects maximum voxel values along viewing ray |
| **MinIP** | Minimum Intensity Projection | Projects minimum voxel values; used for airway visualization |
| **SSD** | Surface Shaded Display | 3D surface rendering of anatomical structures |
| **VRT** | Volume Rendering Technique | 3D visualization with adjustable opacity transfer functions |
| **IMR** | Iterative Model Reconstruction | Advanced iterative reconstruction for ultra-low-dose imaging |
| **SPI** | Spectral Precise Image | Enhanced spectral imaging with improved spatial resolution |
| **CCT** | Continuous CT | Real-time CT fluoroscopy for interventional procedures |

### Dose Management

| Term | Full Name | Description |
|------|-----------|-------------|
| **CTDIvol** | Volume CT Dose Index | Standardized dose metric in mGy; measured against 16/32 cm phantoms |
| **DLP** | Dose Length Product | CTDIvol × scan length (mGy·cm); estimates total radiation per series |
| **SSDE** | Size-Specific Dose Estimates | Patient-size-adjusted dose estimate |
| **DRI** | DoseRight Index | Philips AEC reference level (replaces mAs); auto-adjusts tube current |
| **AEC** | Automatic Exposure Control | Auto-modulates tube current based on patient attenuation |
| **DoseRight** | DoseRight | Philips brand for AEC system; includes 3D modulation & Z-modulation |
| **kVp** | Kilovoltage Peak | X-ray tube voltage; lower kVp = higher contrast but more noise |
| **mAs** | Milliampere-seconds | Tube current × exposure time; controls radiation dose level |

### Scan Workflow

| Term | Full Name | Description |
|------|-----------|-------------|
| **Surview** | Surview (Scout) | Low-dose planar projection for scan planning (AP, Lateral, Dual) |
| **Exam Card** | Exam Card | Protocol template defining scan, recon, and result parameters |
| **Result** | Result (Series) | A single reconstruction output from a scan acquisition |
| **Acquisition Window** | Acquisition Window | Display showing real-time scan images during acquisition |
| **View Window** | View Window | Display for reviewing reconstructed images |
| **Plan Window** | Plan Window | Interface for defining scan geometry on surview |
| **Series List** | Series List | Panel listing all scan series and results for current exam |
| **Scan Ruler** | Scan Ruler | Visual timeline showing scan sequence and geometry |
| **FOV** | Field of View | Diameter of the reconstructed image area (mm) |

### Cardiac

| Term | Full Name | Description |
|------|-----------|-------------|
| **ECG** | Electrocardiogram | Heart electrical signal used for cardiac gating |
| **Prospective Gating** | Prospective ECG Gating | X-ray triggered at specific cardiac phase; lower dose |
| **Retrospective Tagging** | Retrospective ECG Tagging | Continuous scanning with post-acquisition phase selection |
| **Step & Shoot** | Step & Shoot | Axial prospective cardiac scan mode |
| **Precise Cardiac** | Precise Cardiac (Motion Compensated Recon) | Motion-compensated reconstruction for cardiac imaging |
| **PIM** | Patient Interface Module | ECG signal connection device on patient table |

### Patient & Safety

| Term | Full Name | Description |
|------|-----------|-------------|
| **Age Group** | Age Group | Patient age category: Infant, Child, Adolescent, Adult |
| **Isocenter** | Isocenter | Center of gantry rotation; patient should be within ±2 cm |
| **STC** | Short Tube Conditioning | Daily tube warm-up procedure before first scan |
| **IQ Check** | Image Quality Check | Monthly/weekly quality assurance test using phantom |
| **Air Calibration** | Air Calibration | Detector calibration without patient or objects in FOV |
| **Constancy Test** | Monthly Constancy Test | Scheduled QA test per regulatory requirements |

### Spectral-Specific

| Term | Full Name | Description |
|------|-----------|-------------|
| **Iodine Density** | Iodine Density [mg/ml] | Quantitative iodine concentration map |
| **Iodine no Water** | Iodine no Water [mg/ml*] | Iodine map with water content subtracted |
| **Z Effective** | Effective Atomic Number | Map of effective atomic number from spectral decomposition |
| **Electron Density** | Electron Density [%EDW] | Electron density relative to water |
| **Uric Acid** | Uric Acid Map | Spectral result for kidney stone characterization |
| **Non-HU** | Non-Hounsfield Unit | Spectral results where pixel values ≠ standard HU |
| **HA** / **HB** / **HC** etc. | Reconstruction Filters | Named filter kernels (e.g., HA = Brain soft, slightly enhancing) |

---

## 2. IFU Chapter Structure (from Published Documents)

The published IFU follows this chapter hierarchy:

### Spectral CT Verida Family IFU (H1 → H2 → H6)
```
H5  Introduction
H5    Understanding your Philips Scanner
H5    Preparing for an Exam
H5    Scanning a Patient
H5    Summary of Scanning Workflow

H1  Low Dose Lung Cancer Screening Option Parameters
H1  Spectral Results
  H2  What is a spectral base image (SBI)?
  H2  MonoE – Mono-energetic spectral results
  H2  Non-HU Based Spectral Results
  H2  Modified HU Spectral Results
H1  iDose4
  H2  Understanding iDose4
  H2  Setting up iDose4 Exams
H1  Pediatric and Small Patients
  H2  Clinical situations, Warnings, and Precautions
  H2  Radiation Dose Reduction Strategies
H1  Metal Artifact Reduction (O-MAR)
  H2  O-MAR Scan Workflow
  H2  O-MAR Image Samples
  H2  O-MAR Labeling
H1  Extended Field of View (EFOV)
H1  Reconstruction Filters
H1  Spectral Precise Image
```

### Rembra/Areta IFU (H2 → H3)
```
H2  Continuous CT (option)
  H3  Overview
  H3  Principles of operation
  H3  Safety instructions for CCT accessories
  H3  Preparations for CCT
  H3  CCT scan parameters
  H3  Starting the CCT procedure
  H3  Precise Intervention Viewer
  H3  Radiation information
H2  Cardiac
  H3  Helical Retrospective Tagging
  H3  Step & Shoot
  H3  Arrhythmia Compensation
  H3  Patient Qualifications
  H3  Preparing the equipment
H2  Dual energy
H2  Perfusion
H2  DoseRight (AEC)
H2  Pulmo Gating
```

---

## 3. Prerequisite / "Before You Begin" Patterns

Real prerequisites extracted from published IFU documents:

### System State Prerequisites
- Short tube conditioning is required at least once daily before the first scan, or after more than 8 hours without an exposure.
- Ensure the waterproof ring is clean before scanning the patient to assure proper image quality.
- Before using your system, confirm that the room meets the appropriate conditions to ensure the system runs normally.

### Patient Setup Prerequisites
- It is recommended that the patient is centered in the gantry opening within ±2 cm of isocenter for all scans.
- Make sure that there is enough clearance between the patient and the gantry to avoid injury.
- Table movement or gantry position may harm larger patients. Ensure proper patient clearance before scanning.

### Data / Configuration Prerequisites
- Verify that the patient information loaded into the Demographic fields is correct.
- The Patient ID must be selected before the bar code is scanned.
- Ensure the Precise Position viewer is shown before starting positioning workflow.

### Feature-Specific Prerequisites
- Two Results will be reconstructed: with and without O-MAR. Both Results must be reviewed by the reading physician.
- Thin slices will not be generated for axial results that do not contain a tilted axial result.
- Creating additional SBI data sets will increase reconstruction times.

---

## 4. Procedure Step Patterns (from Published IFU)

Real procedure step language from IFU documents:

### Typical Step Patterns
```
Click Start Exam.
Click Short Tube Conditioning. The Short Tube Conditioning interface opens.
Click Air calibration.
Click Confirm to begin the calibration.
Click Exit Console. Click Yes.
Click Service Workflow bar.
Click System Setting.
Click Edit.
Click Save As.
Click Save.
Plan the scan on the Surview, setting the parameters as needed.
From the Show All - Reconstruction, click O-MAR.
Ensure the Precise Position viewer is shown.
Click Settings.
Click Operation Manual.
```

### Key Observations for Step Generation
1. Steps use **imperative mood**: "Click", "Select", "Plan", "Ensure", "Verify"
2. **System response immediately follows** the action in the same sentence (period-separated): "Click X. The X interface opens."
3. Steps are **atomic** — one verb per step (exception: "Click X. Click Y." when two related quick clicks)
4. **UI element names** appear as-is (no bold in Word; bold recommended in HTML for clarity)
5. **Menu paths** use hyphen or arrow: "Show All - Reconstruction" or "From the Show All"
6. **Conditional steps** start with "If applicable" or "if needed"

---

## 5. WARNING / CAUTION / NOTE Real Examples

### WARNING Examples (CTS — Life/Health Risk)

#### Radiation & Dose
- Do not perform Short Tube Conditioning when there is a person in the scanning room.
- Setting wrong patient demographics, scan parameters or geometry, reconstruction or injection parameters may lead to re-scan and excessive radiation.
- When DoseRight is enabled, ensure that external devices and shielding are not located in the scan field of view during surview acquisition, as these devices may reduce automatic dose optimization effectiveness.
- Applying the Brain Area DoseRight Index will raise a patient's overall DoseRight Index through the detected area. Confirm and adjust the settings as needed before scanning.

#### Image Quality & Diagnosis Accuracy
- In cases where the conventional images have very high pixel values, such as in strongly attenuating metal, the CT number of spectral results of these pixels cannot be used for quantitative analysis.
- Darkening or brightening of artifacts can appear on low and high mono energy images. These artifacts are expressed more strongly as the keV value approaches one of the edges of the keV range (40 or 200).
- The accuracy of iodine quantification may be reduced when measuring iodine concentrations which are less than 5 mg/ml.
- VNC images are recommended to be used on body scans only.
- At typical clinical abdomen scan dose, uric acid stones smaller than 3 mm may not be detected.

#### Patient Safety
- To avoid risk of electric shock, do not connect accessory cables while touching patient.
- The table supports a maximum patient weight of 307 kg (677 lbs) in the supine or prone position.
- Do not use the patient support accessory if it is damaged or deteriorated.
- Make sure that the head and arm rest is positioned on the table to support the full weight of the patient's head and arms.
- Avoid pointing the bar code reader at the eyes. The laser light can cause eye damage.

#### Procedure Safety
- If Directions are not followed, then this could cause fatal or serious injury to an operator, patient or any other person, or could lead to a misdiagnosis or mistreatment.
- Only authorized users should access Preferences as changing the system configurations will change the system's behavior.
- When creating new Exam Cards for helical scans, use thin, overlapping slices to reduce the appearance of stair-step artifacts on non-axial images.

### CAUTION Examples (CTQ — Equipment/Minor Injury)

- Never interrupt the electric current to the computer when it is on. Doing so could cause damage to the computing system or to the software.
- You must back up images before deleting them from the scanner. Blocked images will be lost when deleted from the scanner.
- Before proceeding to Exam Card selection, verify that the patient information loaded into the Demographic fields is correct. Failure to do so could result in scanning a patient with the wrong information and may require another scan, resulting in additional radiation exposure.
- Route all cables between the injector, the patient, the table and the CT scanner so that they do not become damaged, or impede the free movement of personnel.
- When loading data into an application, ensure the orientation shown on the images is consistent with the image appearance. Data that contains wrong orientation information will be incorrectly presented within the application.
- After rebuilding, locked files become unlocked and may be inadvertently erased. Use the Lock Patients function to lock the files again.
- Patient health-related information recorded on removable media may become accessible to unauthorized individuals and thus create a privacy protection risk.

### NOTE Examples (Informational — Correct Operation)

- NOTE: You must have permission to access the Dose Management which is provided by the IT Administrator when defining your username and password.
- NOTE: The Dose Management area is relevant for using DoseRight.
- NOTE: The Pause button is not enabled during scan initialization.
- NOTE: Thin slices will not be generated for axial results that do not contain a tilted axial result.
- NOTE: Do not skip the Automatic Centering step. This vertically aligns the phantom and correctly positions the table for the scan.
- NOTE: Creating additional SBI data sets will increase reconstruction times.
- NOTE: Two Results will be reconstructed: with and without O-MAR. Both Results must be reviewed by the reading physician.
- NOTE: During an active scan, the CTDIvol, SSDE, and DLP values are displayed but cannot be edited.
- NOTE: Local phase editing is on by default.
- NOTE: Chest-Abdomen Results have a continuous link with an overlap of 30mm.

### NOTICE (Regulatory — Alternative Signal Word)

The IFU also uses **NOTICE** for regulatory or best-practice information (e.g., pediatric dose optimization):
- Use Dose Optimization Tools.
- Optimize Pediatric Exam Cards for your Facility.
- Lower kVp selections also increase HU ranges and improve image contrast.

---

## 6. Clinical Use Description Patterns

### Feature Clinical Context Templates

#### Post-Processing Application
```
[Feature] is used by radiologists during [clinical workflow] to [purpose].
It is typically launched after [prerequisite imaging] has been completed.
The feature assists in [clinical task] by providing [capability].
```

#### Scan Workflow Feature
```
[Feature] is used by CT technologists during [workflow phase] to [purpose].
It requires [prerequisites] and is configured through [interface].
```

#### Dose Optimization Feature
```
[Feature] helps optimize radiation dose during [scan type] by [mechanism].
It is particularly important for [patient population] to minimize [risk].
```

### Clinical Workflow Positions
1. **Pre-scan**: Patient registration → Exam Card selection → Scan planning on Surview
2. **During scan**: Acquisition monitoring → Dose tracking → Real-time viewing
3. **Post-scan**: Result viewing → Post-processing applications → Reporting
4. **Quality Assurance**: Short Tube Conditioning → IQ Check → Air Calibration → Constancy Test

---

## 7. Product Lines and System Names

| Product | Description |
|---------|-------------|
| **CT Verida Family** | Spectral CT platform (Verida, Verida RT) |
| **CT Rembra** | Premium CT scanner |
| **CT Rembra RT** | Radiation therapy planning variant |
| **CT Areta RT** | Mid-range RT planning scanner |
| **CT5200RT** | RT-specific CT scanner |
| **CT7900** | High-end clinical CT |

### System Components Referenced in IFU
- **Console** — Main operator workstation
- **Gantry** — X-ray tube and detector housing
- **Patient Table (Couch)** — Motorized patient support
- **CTBox** — Gantry control panel (touchscreen)
- **CIRS** — Console Integrated Rack System
- **Interventional Control Box** — CT fluoroscopy remote control
- **PIM** (Patient Interface Module) — ECG connection device
- **Phantom Holder** — QA phantom positioning accessory
- **Infant Cradle** — Pediatric positioning accessory

---

## 8. Key IFU Writing Conventions (from Published Documents)

1. **Signal words** appear as standalone `Heading 4` paragraphs: `WARNING`, `CAUTION`, `NOTICE`
2. **Signal body** immediately follows as `Heading 7` — always bold (Trebuchet MS 9.5pt)
3. **NOTE** is **inline** — prefix `NOTE:` in bold within Body Text (no separate heading)
4. **Procedure steps** use `List Paragraph` style with `Heading 6` for sub-steps
5. **System responses** embedded in same sentence as action: "Click X. The Y interface opens."
6. **Cross-references** use chapter name and page: `See chapter "X" on page Y.`
7. **Figure references** are inline: `(see Image X)` or `(see Figure X)`
8. **Option availability** noted with: "This feature is available only with the [Option] license."
9. **Two results pattern** (O-MAR, spectral): "Two Results will be reconstructed. Both must be reviewed."
10. **Dosimetric values** always include units: `CTDIvol (mGy)`, `DLP (mGy·cm)`, `keV`

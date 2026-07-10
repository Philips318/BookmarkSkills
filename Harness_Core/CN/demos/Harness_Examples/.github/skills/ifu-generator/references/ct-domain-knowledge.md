# CT Domain Knowledge — IFU Generator Reference

Extracted from published Philips CT IFU documents:
- `300013585292_C_spectral_ct_Verida_Family_ifu_en-us.docx`（Spectral CT Verida Family, 594 pages）
- `300009380645_A_CT Rembra RT_CT Areta RT_CT Rembra_IFU-en-US.docx`（Rembra/Areta, 14230 paragraphs）

---

## 1. CT Glossary / Terminology

### Imaging & Reconstruction

| 术语 | 全称 | 描述 |
|------|-----------|-------------|
| **SBI** | Spectral Base Image | Spectral analysis 的 base dataset；以 two energy levels acquired |
| **MonoE** | Mono-energetic | 在 specific keV（40–200 keV）下的 simulated images；reduce beam hardening |
| **iDose4** | Iterative Dose Reduction 4 | low-dose imaging 的 iterative reconstruction technology |
| **O-MAR** | Orthopedic Metal Artifact Reduction | 减少 implants 造成的 metal artifacts；生成两个 results（with/without） |
| **EFOV** | Extended Field of View | 超出 standard FOV（>500 mm）的 reconstruction；edges accuracy reduces |
| **VNC** | Virtual Non-Contrast | 从 spectral data 模拟 non-contrast images；body scans only |
| **MPR** | Multi-Planar Reconstruction | 在 sagittal、coronal 和 oblique planes 中 viewing |
| **MIP** | Maximum Intensity Projection | 沿 viewing ray 投影 maximum voxel values |
| **MinIP** | Minimum Intensity Projection | 投影 minimum voxel values；用于 airway visualization |
| **SSD** | Surface Shaded Display | anatomical structures 的 3D surface rendering |
| **VRT** | Volume Rendering Technique | 带 adjustable opacity transfer functions 的 3D visualization |
| **IMR** | Iterative Model Reconstruction | ultra-low-dose imaging 的 advanced iterative reconstruction |
| **SPI** | Spectral Precise Image | improved spatial resolution 的 enhanced spectral imaging |
| **CCT** | Continuous CT | interventional procedures 的 real-time CT fluoroscopy |

### Dose Management

| 术语 | 全称 | 描述 |
|------|-----------|-------------|
| **CTDIvol** | Volume CT Dose Index | mGy 中的 standardized dose metric；against 16/32 cm phantoms measured |
| **DLP** | Dose Length Product | CTDIvol × scan length（mGy·cm）；estimates total radiation per series |
| **SSDE** | Size-Specific Dose Estimates | Patient-size-adjusted dose estimate |
| **DRI** | DoseRight Index | Philips AEC reference level（replaces mAs）；auto-adjusts tube current |
| **AEC** | Automatic Exposure Control | 基于 patient attenuation auto-modulates tube current |
| **DoseRight** | DoseRight | Philips brand for AEC system；includes 3D modulation & Z-modulation |
| **kVp** | Kilovoltage Peak | X-ray tube voltage；lower kVp = higher contrast but more noise |
| **mAs** | Milliampere-seconds | Tube current × exposure time；controls radiation dose level |

### Scan Workflow

| 术语 | 全称 | 描述 |
|------|-----------|-------------|
| **Surview** | Surview (Scout) | 用于 scan planning 的 low-dose planar projection（AP、Lateral、Dual） |
| **Exam Card** | Exam Card | 定义 scan、recon 和 result parameters 的 protocol template |
| **Result** | Result (Series) | scan acquisition 产生的 single reconstruction output |
| **Acquisition Window** | Acquisition Window | acquisition 期间显示 real-time scan images 的 display |
| **View Window** | View Window | 用于 reviewing reconstructed images 的 display |
| **Plan Window** | Plan Window | 在 surview 上 defining scan geometry 的 interface |
| **Series List** | Series List | 列出 current exam 所有 scan series 和 results 的 panel |
| **Scan Ruler** | Scan Ruler | 显示 scan sequence 和 geometry 的 visual timeline |
| **FOV** | Field of View | reconstructed image area diameter（mm） |

### Cardiac

| 术语 | 全称 | 描述 |
|------|-----------|-------------|
| **ECG** | Electrocardiogram | 用于 cardiac gating 的 heart electrical signal |
| **Prospective Gating** | Prospective ECG Gating | X-ray 在 specific cardiac phase triggered；lower dose |
| **Retrospective Tagging** | Retrospective ECG Tagging | continuous scanning with post-acquisition phase selection |
| **Step & Shoot** | Step & Shoot | Axial prospective cardiac scan mode |
| **Precise Cardiac** | Precise Cardiac (Motion Compensated Recon) | cardiac imaging 的 motion-compensated reconstruction |
| **PIM** | Patient Interface Module | patient table 上的 ECG signal connection device |

### Patient & Safety

| 术语 | 全称 | 描述 |
|------|-----------|-------------|
| **Age Group** | Age Group | Patient age category: Infant, Child, Adolescent, Adult |
| **Isocenter** | Isocenter | gantry rotation center；patient should be within ±2 cm |
| **STC** | Short Tube Conditioning | first scan 前的 daily tube warm-up procedure |
| **IQ Check** | Image Quality Check | 使用 phantom 的 monthly/weekly quality assurance test |
| **Air Calibration** | Air Calibration | 无 patient 或 objects in FOV 时的 detector calibration |
| **Constancy Test** | Monthly Constancy Test | per regulatory requirements 的 scheduled QA test |

### Spectral-Specific

| 术语 | 全称 | 描述 |
|------|-----------|-------------|
| **Iodine Density** | Iodine Density [mg/ml] | Quantitative iodine concentration map |
| **Iodine no Water** | Iodine no Water [mg/ml*] | water content subtracted 的 iodine map |
| **Z Effective** | Effective Atomic Number | spectral decomposition 得出的 effective atomic number map |
| **Electron Density** | Electron Density [%EDW] | Electron density relative to water |
| **Uric Acid** | Uric Acid Map | kidney stone characterization 的 spectral result |
| **Non-HU** | Non-Hounsfield Unit | Pixel values ≠ standard HU 的 spectral results |
| **HA** / **HB** / **HC** etc. | Reconstruction Filters | Named filter kernels（例如 HA = Brain soft, slightly enhancing） |

---

## 2. IFU Chapter Structure (from Published Documents)

Published IFU 遵循以下 chapter hierarchy：

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

从 published IFU documents 提取的 real prerequisites：

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

IFU documents 中的 real procedure step language：

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
1. Steps 使用 **imperative mood**: "Click", "Select", "Plan", "Ensure", "Verify"
2. **System response immediately follows** 同一句中的 action（period-separated）: "Click X. The X interface opens."
3. Steps 是 **atomic** — 每步一个 verb（例外：两个相关 quick clicks 时 "Click X. Click Y."）
4. **UI element names** 原样出现（Word 中不 bold；HTML 中建议 bold 以提高清晰度）
5. **Menu paths** 使用 hyphen 或 arrow: "Show All - Reconstruction" 或 "From the Show All"
6. **Conditional steps** 以 "If applicable" 或 "if needed" 开头

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

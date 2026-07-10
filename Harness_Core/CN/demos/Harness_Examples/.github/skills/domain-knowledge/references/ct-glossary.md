# CT Domain Glossary

## CT Imaging Fundamentals

| 术语 | 全称 | 单位 | 描述 |
|------|-----------|------|-------------|
| **HU** | Hounsfield Unit | — | CT density scale。Water = 0 HU，Air = -1000 HU，Bone ≈ +1000 HU |
| **WW** | Window Width | HU | Display contrast range；must be > 0 |
| **WC** | Window Center | HU | Display brightness center point |
| **FOV** | Field of View | mm | Reconstructed image diameter。Head: 200-250，Abdomen: 350-400，Body: 450-500 |
| **PixelSpacing** | Pixel Spacing | mm | Physical size per pixel = FOV / Matrix size |
| **IPP** | Image Position Patient | mm | patient space 中 top-left pixel 的 3D coordinates (x,y,z) |
| **IOP** | Image Orientation Patient | — | 6 direction cosines（row + column vectors） |
| **CTDIvol** | CT Dose Index Volume | mGy | Standardized dose metric per scan |
| **DLP** | Dose Length Product | mGy·cm | CTDIvol × scan length |
| **SSDE** | Size-Specific Dose Estimate | mGy | Patient-size-adjusted dose |
| **kVp** | Peak Kilovoltage | kV | X-ray tube voltage（80/100/120/140 kVp） |
| **mAs** | Milliampere-seconds | mAs | Tube current × time = dose parameter |
| **keV** | Kilo-electron Volt | keV | Photon energy（Spectral: 40-200 keV） |

## CT Scan Modes

| 模式 | 描述 |
|------|-------------|
| **Axial** | Step-and-shoot；每次 rotation 期间 table stops |
| **Helical** | rotation 期间 continuous table movement；最常见 |
| **Cardiac** | ECG-gated；Step & Shoot 或 Helical with phase selection |
| **Composite** | 一个 exam 中包含多个 scan ranges |
| **Interventional** | procedure 期间 real-time guidance |
| **Surview/Scout** | 用于 scan planning 的 low-dose projection（not for diagnosis） |

## Patient Positions (8 standard)

| Code | 全称 | Axial IOP |
|------|-----------|-----------|
| HFS | Head First Supine | `1\0\0\0\1\0` |
| HFP | Head First Prone | `-1\0\0\0\-1\0` |
| FFS | Feet First Supine | `-1\0\0\0\1\0` |
| FFP | Feet First Prone | `1\0\0\0\-1\0` |
| HFDR | Head First Decubitus Right | `0\1\0\-1\0\0` |
| HFDL | Head First Decubitus Left | `0\-1\0\1\0\0` |
| FFDR | Feet First Decubitus Right | `0\-1\0\-1\0\0` |
| FFDL | Feet First Decubitus Left | `0\1\0\1\0\0` |

## Patient Coordinate System (LPS)

- **X** → Patient Left（Right → Left）
- **Y** → Patient Posterior（Anterior → Posterior）
- **Z** → Patient Superior（Feet → Head, in Supine）
- **Origin (0,0,0)** = CT Gantry Isocenter

## Reconstruction Algorithms

| 名称 | Marketing Name | 用途 |
|------|---------------|---------|
| iDose⁴ | — | Iterative dose reduction |
| O-MAR | — | Orthopedic Metal Artifact Reduction |
| Precise Image | (was AI Recon) | Deep learning reconstruction |
| Precise Cardiac | (was MCR) | Motion-compensated cardiac reconstruction |
| Precise MAR | — | Advanced metal artifact reduction |

## Dose Management

| Feature | 描述 |
|---------|-------------|
| **DoseRight** | Auto dose optimization: ACS（tube current selection）+ 3D modulation |
| **DRI** | DoseRight Index — adjustable per clinical indication |
| **Dose Check** | Notification（soft warning）+ Alert（hard stop, requires acknowledgement） |
| **Dose SR** | DICOM Structured Report for dose documentation |

## Typical Window Presets

| Anatomy | WW | WC |
|---------|----|----|
| Soft Tissue | 400 | 40 |
| Lung | 1500 | -600 |
| Bone | 2000 | 300 |
| Brain | 80 | 40 |
| Liver | 150 | 30 |
| Mediastinum | 350 | 50 |

## User Roles

| Role | 描述 | Access Level |
|------|-------------|-------------|
| **Operator** | Trained technologist；scans、reconstructs、reviews | Clinical functions |
| **Administrator** | Operator + user management | Extended |
| **Service Engineer** | Full access；IST key + password required | Configuration, calibration |
| **Emergency Login** | No password；limited to 5 exams；forces re-login | Restricted clinical |

## ISP / Post-Processing Terms

| 术语 | 全称 | 描述 |
|------|-----------|-------------|
| **ISP** | IntelliSpace Portal | Philips post-processing workstation（client-server） |
| **ISP10** | IntelliSpace Portal v10 | Current ISP release |
| **PACS** | Picture Archiving and Communication System | Image archive |
| **HIS** | Hospital Information System | Hospital-wide patient/admin data |
| **RIS** | Radiology Information System | Radiology orders, worklist source |
| **MWL** | Modality Worklist | DICOM service to fetch scheduled patients |
| **MPPS** | Modality Performed Procedure Step | DICOM service to report procedure status |
| **KIN** | Key Image Notes | Annotated key slices saved with study |
| **CBV** | Cerebral Blood Volume | Perfusion metric（ml/100g） |
| **CBF** | Cerebral Blood Flow | Perfusion metric（ml/100g/min） |
| **MTT** | Mean Transit Time | Perfusion metric（seconds） |
| **TTP** | Time To Peak | Perfusion metric（seconds） |
| **ROI** | Region of Interest | User-drawn analysis region |
| **CIRS** | Common Image Reconstruction System | Recon backend |
| **MIPPP** | Medical Image Post-Process Platform | Internal post-processing platform |
| **FRU** | Field Replaceable Unit | Service-replaceable hardware |
| **IST** | Philips Integrated Security mechanism | Service login token |
| **AIAT** | Assembly, Installation, Adjustment, Test | Service workflow |
| **DOM** | Dose Modulation | Dose-saving technique |

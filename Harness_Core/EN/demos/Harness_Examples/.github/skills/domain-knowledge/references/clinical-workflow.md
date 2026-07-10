# Clinical Workflow Knowledge

## Scanner Console Workflow

```
1. Startup → 2. STC → 3. Air Cal → 4. Register → 5. Exam Card → 6. Surview → 7. Scan → 8. Recon → 9. Review → 10. Archive → 11. Shutdown
```

### Step Details

| Step | Name | Description | Safety Checks |
|------|------|-------------|--------------|
| 1 | **Startup** | Power on gantry → boot computer → launch CT Console → login | Login required; Emergency mode available (max 5 exams) |
| 2 | **STC** | Short Tube Conditioning — warm up X-ray tube | Required after cold start |
| 3 | **Air Calibration** | Calibrate detector response | Required for accurate HU values |
| 4 | **Patient Registration** | New / Scheduled / Anonymous; enter demographics, weight, age | Patient ID verification; age/weight for dose calculations |
| 5 | **Exam Card Selection** | Choose protocol by anatomy + age group | Smart Protocoling: AI-suggested cards based on order/history |
| 6 | **Surview** | Scout scan for planning; draw scan boxes on surview image | Low-dose; NOT diagnostic quality |
| 7 | **Scan** | Execute series (axial/helical/cardiac/composite) | Dose Check: Notification + Alert thresholds |
| 8 | **Reconstruction** | Online recon with selected kernel, filter, iDose/Precise Image | Can add offline recon later; O-MAR as separate pass |
| 9 | **Review** | 2D, MPR, Volume, Endo viewing modes; measurements, annotations | Spectral viewer for spectral results |
| 10 | **Archive** | Film, send to PACS, create reports, Dose SR | Queue management; Storage Commitment |
| 11 | **Shutdown** | Orderly system shutdown | Verify all exams archived |

## Review / Viewing Workflow

| Mode | Purpose | Key Operations |
|------|---------|---------------|
| **2D** | Axial/Coronal/Sagittal slices | Scroll, zoom, pan, window, measure |
| **MPR** | Multi-Planar Reconstruction | Arbitrary plane orientation |
| **Volume** | 3D rendering (MIP/MinIP/AvgIP/VR) | Rotate, clip, transfer function |
| **Endo** | Virtual endoscopy | Fly-through, wall view |

### Projection Types (Volume Mode)

| Type | Full Name | Use |
|------|-----------|-----|
| **MIP** | Maximum Intensity Projection | Vessels, calcifications |
| **MinIP** | Minimum Intensity Projection | Airways, low-density structures |
| **AvgIP** | Average Intensity Projection | Lung parenchyma |
| **VR** | Volume Rendering | 3D anatomy visualization |

## Spectral CT Viewer Workflow

```
1. Open series (SBI auto-detected) → 2. Conventional view → 3. Select spectral result type → 4. Adjust keV (MonoE) → 5. Toggle MagicGlass / Fusion → 6. Spectral plot
```

### Layout Presets

| Category | Default Result Types |
|----------|---------------------|
| General | Conventional + MonoE |
| Oncology | Conventional + Iodine Map + VNC |
| Vascular | MonoE low keV + Contrast-Enhanced Structures |
| Neuro | MonoE high keV + VNC |
| Fusion | Conventional with Z Effective overlay |

## Clinical Applications (Post-Processing)

| Application | Purpose | Key Inputs |
|-------------|---------|-----------|
| **Lung Nodule Analysis** | Detect and measure lung nodules | Thin-slice chest CT |
| **CT Colonoscopy** | Virtual colonoscopy | Prone + supine with prep |
| **Brain Perfusion** | Cerebral blood flow mapping | Dynamic scan with contrast |
| **Vessel Analysis** | Vessel centerline, stenosis measurement | CTA with contrast |
| **Dental Planning** | Implant planning, panoramic views | High-res facial CT |
| **Cardiac Calcium Scoring** | Coronary artery calcium quantification | Non-contrast cardiac CT |
| **Cardiac Function Analysis** | Ejection fraction, wall motion | Multi-phase cardiac CT |
| **Cardiac Artery Analysis** | Coronary artery stenosis | Gated CTA |
| **Body Helical Perfusion** | Organ perfusion mapping | Dynamic helical scan |

## Patient Registration Patterns

| Registration Type | Use Case | Key Fields |
|------------------|----------|-----------|
| **New Patient** | Walk-in, manual entry | Name, ID, DOB, Sex, Weight |
| **Scheduled** | From Worklist (MWL query) | Auto-populated from RIS |
| **Anonymous** | Emergency / unknown patient | System-generated temporary ID |
| **Barcode** | Scan patient wristband | Auto-populate from barcode → lookup |

## Exam Card Management

- **Exam Card** = Protocol template (scan parameters, recon parameters, viewing presets)
- **Exam Card Manager** = CRUD interface for protocols
- **CT Protocol Manager** = Advanced protocol management with import/export
- **Smart Protocoling** (Precise Planning / iPlanning) = AI-suggested exam cards
- **Age Group Matching** = Auto-select pediatric card for children < 70 kg
- **Spectral Results** = Add via Exam Card Manager (Add Spectral Result button)

## DICOM Network Workflows

| Workflow | Services Used | Purpose |
|----------|--------------|---------|
| **Worklist** | C-FIND on MWL SCP | Pull scheduled patients from RIS |
| **Image Send** | C-STORE | Push images to PACS |
| **Query/Retrieve** | C-FIND + C-MOVE | Search and pull prior studies |
| **Storage Commitment** | N-ACTION + N-EVENT-REPORT | Confirm PACS has safely stored |
| **MPPS** | N-CREATE + N-SET | Report procedure status to RIS |
| **Dose Report** | C-STORE (SR) | Send DICOM Dose Structured Report |

## Feature Name Mapping (Marketing → Formal)

| Marketing Name | Formal Name |
|---------------|-------------|
| AI Recon | Precise Image |
| MCR | Precise Cardiac |
| Camera WF | Precise Position / Smart Positioning Camera |
| Brain iBatch | Precise Brain |
| iBatch | Precise Spine |
| iPlanning | Precise Planning / Smart Exam |
| iStation | OnPlan |
| Needle Tracking | Precise Intervention |
| Cloud Hook | HealthSuite connect |

**Requirement implication:** Always use the **formal name** in requirements; include marketing name in parentheses only if needed for cross-reference.

## IntelliSpace Portal (ISP) — Post-Processing Workstation

ISP is Philips' **client-server multi-user post-processing platform** for advanced visualization. Complementary to the CT scanner console: the **console acquires**, ISP **analyzes**.

### Architecture

- **Server** — runs computation, stores patient data centrally
- **Client** — thin client connects via LAN/WAN/VPN; Citrix XenDesktop supported (with image-quality caveat)
- **Multi-user** — up to 15 concurrent clients per dual-server config
- **Threshold** — ~25k slices (2-5 clients) / ~30k (6-10 clients) / ~60k (11-15 clients, dual server)
- **Client/server version must match** — cannot connect to mismatched server

### ISP Top-Level Modules

| Module | Function |
|--------|----------|
| Directory | Browse devices, patient list, worklist, series list, archive manager, queue manager |
| Quick Review | Lightweight viewer for fast triage (cine, combine, batch) |
| CT Analysis | Clinical applications (see below) |
| MR / NM / US Analysis | Modality-specific applications |
| Workflow Bar | Patient bar, priors, film, report, KnowledgeScape, help |

### CT Clinical Applications on ISP

| Application | Purpose |
|-------------|---------|
| Bone Mineral Density | Osteoporosis screening with population reference |
| Brain Perfusion | CBV/CBF/MTT/TTP maps; vessel definition; permeability |
| COPD Analysis | Lung segmentation, emphysema, air-trapping, airway measurement |
| Comprehensive Cardiac Analysis | Coronary extraction, segmentation, function, plaque |
| CT Viewer | General 2D/MPR/3D viewing |
| CT Colonography | Virtual colonoscopy with prone/supine |
| Dental Planning | Implant planning, panoramic |
| Lung Nodule Assessment | Detect, segment, follow-up of pulmonary nodules |
| Pulmo CT (4D) | Respiratory-gated lung analysis |
| Vessel Explorer / CTA | Vessel centerline, stenosis quantification |
| Tumor Tracking | Follow-up lesion measurement over time |
| Calcium Scoring | Coronary calcium quantification (Agatston) |
| Spectral Magic Glass / Spectral Viewer | Spectral CT post-processing on ISP |
| Multi-Modality Tumor Tracking | Cross-modality follow-up |

### Key ISP Workflow Patterns

| Pattern | Description |
|---------|-------------|
| **Prefetch** | PACS pre-loads prior studies for fast review |
| **Priors** | Auto-link prior studies of same patient |
| **Run Processing** | Background batch processing on server |
| **Quick Review** | Triage view before launching full application |
| **Send Link** | Share study reference with another user |
| **Direct Record** | Send to PACS without full review |
| **De-identify** | Create anonymized copy (original retained) |
| **Key Image Notes (KIN)** | Annotate and save important slices |
| **Bookmarks** | Save state of analysis for resumption |
| **Add to Running Application** | Append additional series to in-progress session |

### ISP Data Operations

| Operation | Notes |
|-----------|-------|
| Lock/Unlock study | Prevents accidental deletion |
| Auto Delete Studies | Site-configured cleanup policy |
| De-identification | Creates **new study** — original kept; deletion requires site admin |
| Change Patient Details | Creates **new study** — original kept |
| Clipboard copy | **WARNING** — may contain PHI; user must clear Windows clipboard |
| Multimedia Viewer | Non-DICOM file handling (movies, screenshots) |
| Patient Disk (CD/DVD) | Includes Philips DICOM Viewer for offline review |

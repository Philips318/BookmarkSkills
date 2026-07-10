# Warning / Caution / Note Classification — CT IFU

Philips CT Instructions for Use 的 signal words 和 safety information classification，符合 IEC 82079-1 和 ANSI Z535.6。

---

## Signal Word Hierarchy

| Signal Word | Risk Level | 含义 |
|-------------|-----------|---------|
| **WARNING** | **High** | Hazardous situation；如不避免，**could result in death or serious injury** |
| **CAUTION** | **Medium** | Hazardous situation；如不避免，**could result in minor or moderate injury**，或 **damage to equipment** |
| **NOTE** | **Low** | **not safety-related** 但帮助 user 正确或高效操作 system 的 important information |

> **No emoji icons.** Official Philips IFU Word document 不使用 emoji（⚠️/ℹ️）。仅 bold signal word text 作为 visual indicator。

---

## Formatting Rules (Word-Compatible HTML)

下面的 HTML patterns 匹配 Philips IFU Word document styles：
- Signal word label → Word **Heading 4**（Trebuchet MS, 10pt, Bold）
- Signal body text → Word **Heading 7**（Trebuchet MS, 9.5pt, Bold）
- NOTE → Word **Body Text** with bold prefix（Microsoft Sans Serif, 9.5pt）

### WARNING Format
```html
<div class="ifu-signal ifu-warning">
  <div class="signal-label">WARNING</div>
  <div class="signal-body">
    <p>[Hazard description]. [Consequence if not avoided]. [Action to avoid hazard].</p>
  </div>
</div>
```

### CAUTION Format
```html
<div class="ifu-signal ifu-caution">
  <div class="signal-label">CAUTION</div>
  <div class="signal-body">
    <p>[Hazard description]. [Consequence if not avoided]. [Action to avoid hazard].</p>
  </div>
</div>
```

### NOTE Format (inline — no box)
```html
<p class="ifu-note-inline"><strong>NOTE:</strong> [Important information or tip].</p>
```

---

## Three-Part Structure for Warnings and Cautions

每个 WARNING 或 CAUTION 必须包含：

1. **Hazard identification**: hazard 是什么
2. **Consequence**: 如果未避免 hazard 会发生什么
3. **Avoidance action**: 如何避免 hazard

**Template:**
```
[Nature of hazard]. [Consequence]. [How to avoid].
```

**Example:**
```
WARNING
Incorrect rib labeling may lead to misidentification of surgical sites.
Verify all rib labels against the patient's anatomy before clinical use.
Always confirm the automatic labeling results with the source images.
```

---

## Placement Rules

| Placement | 规则 |
|-----------|------|
| **Before the hazard** | WARNING 和 CAUTION 必须出现在使 user 暴露于 hazard 的 step **之前** |
| **Beginning of chapter** | 适用于整个 feature 的 general safety warnings 应出现在 feature chapter 开头 |
| **Inline with steps** | Step-specific warnings 立即出现在相关 step 之前 |
| **Not at end** | Never place safety information only at the end of a procedure |

---

## CT-Specific Warning Categories

### Patient Safety Warnings

| 类别 | 何时使用 | 模板 |
|----------|-------------|----------|
| **Dose** | Radiation dose implications | "WARNING: [Action] may result in additional radiation exposure to the patient. Verify [parameter] before proceeding." |
| **Image quality** | Results may affect diagnosis | "WARNING: [Condition] may affect image quality and diagnostic accuracy. Verify results against source images." |
| **Label accuracy** | Automatic labeling/annotation | "WARNING: Automatic [labeling/measurement] results are for reference only. Always verify correctness before clinical decision-making." |
| **Patient positioning** | Incorrect position risk | "WARNING: Incorrect patient positioning may lead to [consequence]. Verify patient position before scanning." |
| **Contrast agent** | Injection-related risks | "WARNING: Verify contrast agent parameters before injection. Incorrect settings may result in [consequence]." |

### Equipment/Data Cautions

| 类别 | 何时使用 | 模板 |
|----------|-------------|----------|
| **Data loss** | Unsaved work risk | "CAUTION: Unsaved changes will be lost if you [action]. Save your work before proceeding." |
| **Processing time** | Long operations | "CAUTION: [Operation] may take several minutes for large datasets. Do not interrupt the process." |
| **Disk space** | Storage implications | "CAUTION: [Operation] requires significant disk space. Verify available storage before proceeding." |
| **Network** | Connectivity dependent | "CAUTION: [Feature] requires network connectivity. Results may be incomplete if the connection is interrupted." |
| **Configuration** | Settings impact | "CAUTION: Changing [setting] affects all subsequent [operations]. Verify the setting is appropriate for your workflow." |

### Informational Notes

| 类别 | 何时使用 | 模板 |
|----------|-------------|----------|
| **Availability** | Feature/option gating | "NOTE: This feature is available only with the [Option Name] license / on [Product Model]." |
| **Limitation** | Known constraints | "NOTE: [Feature] supports a maximum of [N] [items]. For larger datasets, use [alternative]." |
| **Best practice** | Recommended approach | "NOTE: For optimal results, [recommendation]." |
| **Prerequisite** | Required conditions | "NOTE: [Feature] requires [prerequisite]. Ensure [condition] before proceeding." |
| **Default behavior** | Automatic actions | "NOTE: The system automatically [behavior] when [condition]." |
| **Workaround** | Alternative methods | "NOTE: If [condition], you can alternatively [workaround action]." |

---

## CTS/CTQ Mapping to Warning Level

从 requirements 生成 IFU content 时：

| Requirement Classification | IFU Signal Word | 理由 |
|---------------------------|-----------------|-----------|
| **CTS** (Critical to Safety) | **WARNING** | Safety-critical requirement → must have explicit warning |
| **CTQ** (Critical to Quality) | **CAUTION** or **NOTE** | Quality-critical → may need caution about correct usage |
| **Standard requirement** | **NOTE** (if needed) | May need a note for correct operation |
| **Non-functional** | Usually none | Performance/technical specs don't typically need signal words in procedures |

---

## Language Rules for Safety Text

| 规则 | Correct | Incorrect |
|------|---------|-----------|
| Use imperative | "Verify the labels" | "You should verify the labels" |
| Be specific | "Incorrect rib labels may lead to wrong surgical site identification" | "This may be dangerous" |
| State consequence | "...may result in misdiagnosis" | "...may cause problems" |
| One hazard per signal word | Separate WARNING for each distinct hazard | Combining multiple unrelated hazards |
| No minimizing language | "may result in injury" | "could possibly result in minor issues" |
| Active voice | "Verify the result before clinical use" | "The result should be verified" |

---

## Real Examples from Published Philips CT IFU

以下是 released IFU documents 中的 actual WARNING、CAUTION 和 NOTE texts。生成 new signal word content 时，把它们作为 templates 和 style references。

### Published WARNING Examples

**Radiation & Dose:**
```
WARNING
Do not perform Short Tube Conditioning when there is a person in the scanning room.
```
```
WARNING
Setting wrong patient demographics, scan parameters or geometry, reconstruction or
injection parameters may lead to re-scan and excessive radiation.
```
```
WARNING
When DoseRight is enabled, ensure that external devices and shielding (e.g., bismuth
shields, radiation therapy planning hardware, life support devices) are not located in the
scan field of view during surview acquisition, as these devices may reduce automatic dose
optimization effectiveness.
```

**Image Quality & Diagnosis:**
```
WARNING
In cases where the conventional images have very high pixel values, such as in strongly
attenuating metal, the CT number of spectral results of these pixels cannot be used for
quantitative analysis.
```
```
WARNING
The accuracy of iodine quantification may be reduced when measuring iodine
concentrations which are less than 5 mg/ml.
```
```
WARNING
VNC images are recommended to be used on body scans only.
```
```
WARNING
At typical clinical abdomen scan dose, uric acid stones smaller than 3 mm may not be detected.
```

**Patient Safety:**
```
WARNING
To avoid risk of electric shock, do not connect accessory cables while touching patient.
```
```
WARNING
The table supports a maximum patient weight of 307 kg (677 lbs) in the supine or prone position.
```

**Procedure Safety:**
```
WARNING
Only authorized users should access Preferences as changing the system configurations
will change the system's behavior.
```
```
WARNING
When creating new Exam Cards for helical scans, use thin, overlapping slices to reduce
the appearance of stair-step artifacts on non-axial images.
```

### Published CAUTION Examples
```
CAUTION
Never interrupt the electric current to the computer when it is on. Doing so could cause
damage to the computing system or to the software.
```
```
CAUTION
You must back up images before deleting them from the scanner. Blocked images will be
lost when deleted from the scanner.
```
```
CAUTION
Before proceeding to Exam Card selection, verify that the patient information loaded into
the Demographic fields is correct. Failure to do so could result in scanning a patient with
the wrong information and may require another scan, resulting in additional radiation exposure.
```
```
CAUTION
When loading data into an application, ensure the orientation shown on the images is
consistent with the image appearance. Data that contains wrong orientation information
will be incorrectly presented within the application.
```

### Published NOTE Examples
```
NOTE: You must have permission to access the Dose Management which is provided by the
IT Administrator when defining your username and password.
```
```
NOTE: The Pause button is not enabled during scan initialization.
```
```
NOTE: Do not skip the Automatic Centering step. This vertically aligns the phantom and
correctly positions the table for the scan.
```
```
NOTE: Two Results will be reconstructed: with and without O-MAR. Both Results must be
reviewed by the reading physician.
```
```
NOTE: During an active scan, the CTDIvol, SSDE, and DLP values are displayed but cannot be edited.
```
```
NOTE: Chest-Abdomen Results have a continuous link with an overlap of 30mm.
```

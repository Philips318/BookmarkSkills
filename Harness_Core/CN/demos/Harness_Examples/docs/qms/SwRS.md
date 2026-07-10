# Software Requirements Specification

## PURPOSE

<!-- GUIDANCE: [本段简要说明软件需求适用于什么内容。 -->
<!-- GUIDANCE: 示例：本文档旨在为 DMS Service SW 提供需求规格。该 SW 提供可监视、控制和分析 DMS HW 反馈的应用。] -->

## SCOPE

<!-- GUIDANCE: 本记录适用于 Philips CT/AMI。 -->

## INTRODUCTION

<!-- GUIDANCE: <Introduction 应包含 requirements specification 的生成方式定义。定义必须包括： -->
<!-- GUIDANCE: Tool Used（例如 DOORS） -->
<!-- GUIDANCE: Repository Version（例如 DOORS baseline） -->
<!-- GUIDANCE: Export Method（例如 RPE、CSV） -->
<!-- GUIDANCE: Query/Filter Parameters（例如 RPE template name/rev、Product 和 Target Release 等） -->
<!-- GUIDANCE: Post Processing Steps required（例如 headings alignment 等） -->
<!-- GUIDANCE: > -->

## OVERVIEW

<!-- GUIDANCE: <简要描述 SW modules 及其用途，不描述其设计。> -->

## Modules Requirements

<!-- GUIDANCE: <按 SW module 指定功能和非功能需求（针对 features 和 functions）。包括 interface requirements、User Interface requirements 和 quality aspects requirements> -->
<!-- GUIDANCE: repeatable example section: <Module 1> -->
<!-- GUIDANCE: < 以下是在指定需求时应按适用情况考虑的主题示例（并非详尽列表）： -->
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
<!-- GUIDANCE: <针对每个 SOUP 或 legacy SW item，按适用情况指定其预期用途所需需求：Functional and performance requirements、支持该 item 正常运行所需的 system hardware and software、item acceptance criteria> -->
<!-- GUIDANCE: <如果没有 SOUP 或 Legacy items，请移除此小节> -->
<!-- GUIDANCE: repeatable example section: <Item 1> -->
<!-- GUIDANCE: <如果没有 SOUP 或 Legacy items，请移除此小节> -->

### Image Display — WW/WC Reset to Default (IEC 62304 Class B)

**IEC 62304 Safety Class：** B  
**Slug：** `ww-wc-reset`  
**Feature：** CT 图像显示面板中的 Reset Window/Level to Default 按钮。

#### Functional Requirements

| ID | 需求 | 优先级 | DICOM Reference |
|----|-------------|----------|-----------------|
| FR-01 | 图像显示面板 **shall** 包含一个可见的 Reset Window/Level 按钮，并在活动视口加载 CT series 时始终存在。 | 必须 | — |
| FR-02 | 激活时，Reset Window/Level 按钮 **shall** 将活动视口 WW 和 WC 设置为 DICOM tags (0028,1051) 和 (0028,1050) 在 index 0 的值。应用的 WW 值 **shall** 大于 0。 | 必须 | DICOM PS3.3 §C.7.6.3.1.5 |
| FR-03 | 当 DICOM tags (0028,1051)/(0028,1050) 为多值（VM > 1）时，系统 **shall** 默认 reset 到 index-0 preset，并且在 tag (0028,1055)[0] 存在时，**shall** 在确认消息中包含该 preset name。 | 必须 | DICOM PS3.3 §C.7.6.3.1.5 |
| FR-04 | 当 tags (0028,1051) 或 (0028,1050) 缺失时，系统 **shall** 应用来自应用配置的可配置软件定义 fallback WW/WC（默认 WW = 400，WC = 40）。 | 必须 | — |
| FR-05 | reset 动作后，系统 **shall** 显示一条非阻塞确认消息，至少可见 ≥ 2 秒，并在无用户交互时自动消失。 | 必须 | — |
| FR-06 | reset 动作 **shall** 可通过标准应用 Undo 功能撤销，恢复 reset 前立即生效的 WW/WC 值。 | 必须 | — |
| FR-07 | 系统 **shall** 拒绝来自任意来源（DICOM tag、fallback config、Undo stack）的 WW 值 ≤ 0，记录 warning，应用可配置 fallback，并通知用户。 | 必须 | — |

#### Non-Functional Requirements

| ID | 需求 | 指标 | 类别 |
|----|-------------|--------|----------|
| NFR-01 | Reset action shall 在最低规格硬件上于 200 ms 内完成（95th percentile）。 | ≤ 200 ms p95 | 性能 |
| NFR-02 | Reset button shall 可通过文档化快捷键（默认：Ctrl+Shift+W）键盘操作，并 shall 有描述性 tooltip；可通过 Tab navigation 到达。 | Keyboard + tooltip | Usability / Accessibility |
| NFR-03 | reset 功能 shall 不因 reset 动作记录、传输或持久化 PHI。 | Code review pass | 安全性 |
| NFR-04 | reset 功能 shall 不在任何受支持 DICOM dataset 变体下抛出未处理异常；失败 shall 以 WARNING level 记录并保留当前 WW/WC。 | Zero unhandled exceptions | 可靠性 |
| NFR-05 | 所有访问 DICOM tags (0028,1050)、(0028,1051)、(0028,1055) 的代码 shall 在 XML doc comments 中引用 tag numbers 和 DICOM PS3.3 §C.7.6.3.1.5；tag literals shall 位于专用 DICOM constants 文件。 | Code review pass | Maintainability |

## TESTABILITY REQUIREMENTS

<!-- GUIDANCE: <描述 SW module 的 testability requirements> -->
<!-- GUIDANCE: [本节可按 SW modules 划分，或适用于所有 modules] -->
<!-- GUIDANCE: repeatable example section: <Module 1> -->
<!-- GUIDANCE: <当本节需求适用于所有 SW modules 时，移除 sub-section header> -->

### Image Display — WW/WC Reset：需求到验证的可追溯性

WW/WC Reset 功能的所有功能和非功能需求都追溯到一个或多个 verification scenarios（BDD Gherkin）、unit tests 或 review activities。下列 scenario names 是不可变契约 — 它们与 feature specifications 中的 Gherkin `Scenario:` 行逐字对应。

| 需求 | 验证方法 | Verification Scenario(s) |
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

<!-- GUIDANCE: < 指定 SW installation、upgrade、uninstall、backup 和 restore requirements。 > -->
<!-- GUIDANCE: repeatable example section: <Module 1> -->
<!-- GUIDANCE: <描述 SW module 的 installation requirements> -->

## CROSS MODULES REQUIREMENTS

<!-- GUIDANCE: <指定适用于所有 SW modules 的 cross modules requirements。> -->
<!-- GUIDANCE: 当没有 Cross Modules Requirements 时，移除此节。 -->
<!-- GUIDANCE: 当本节需求适用于所有 SW modules 时，移除 sub-section headers -->
<!-- GUIDANCE: 以下是在指定需求时应按适用情况考虑的主题示例（并非详尽列表）： -->
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
<!-- GUIDANCE: < 当本节需求适用于所有 SW modules 时，移除 sub-section header> -->
<!-- GUIDANCE: repeatable example section: <Topic n> -->
<!-- GUIDANCE: < 当本节需求适用于所有 SW modules 时，移除 sub-section header> -->

## TERMS AND ABBREVIATIONS

| 术语 / 缩写 | 描述 |
| --- | --- |
| WW | Window Width — DICOM tag (0028,1051)；控制显示 CT 图像的对比范围。必须 > 0。 |
| WC | Window Center (Window Level) — DICOM tag (0028,1050)；控制显示 CT 图像的亮度中点。 |
| WL | Window Level — Window Center (WC) 的同义词。 |
| DICOM | Digital Imaging and Communications in Medicine — 医学影像数据国际标准。 |
| IEC 62304 | 医疗器械软件生命周期过程国际标准。 |
| VM | Value Multiplicity — DICOM attribute 可持有值的数量。 |
| DS | Decimal String — 用于 WW 和 WC 的 DICOM Value Representation。 |
| PHI | Protected Health Information。 |
| MVVM | Model-View-ViewModel — 此应用中使用的 WPF architectural pattern。 |

## APPENDICES

| 附录 | 标题 |
| --- | --- |

## REFERENCES

### External References

| ?? ID | 文档标题 |
| --- | --- |

### Internal References

| ?? ID | 文档标题 |
| --- | --- |

## RECORD CHANGE SUMMARY

| Revision | 文档变更号 | 文档编辑者 | Description of Change |
| --- | --- | --- | --- |
| 1.0 | TBD | @analyst (AI) | Initial entry：新增 Image Display — WW/WC Reset to Default requirements（FR-01 到 FR-07，NFR-01 到 NFR-05）；IEC 62304 Class B。日期：2026-07-02。 |
| 1.1 | TBD | @product-owner (AI) | 新增 TESTABILITY REQUIREMENTS：Requirement-to-Verification Traceability 表，将 FR-01–FR-07 和 NFR-01–NFR-05 映射到 4 个任务中的 17 个 Gherkin scenarios。日期：2026-07-02。 |

## RECORD APPROVALS

| 签名原因 | 职能 | 名称 |
| --- | --- | --- |

<!-- GUIDANCE: Signatures 和 dates 作为 document change order 的一部分在 PLM tool 中捕获。 -->
<!-- GUIDANCE: repeatable example section: APPENDIX <x> – <Title> -->
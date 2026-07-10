---
created: "2026-07-02T00:00:00Z"
updated: "2026-07-02T00:00:00Z"
slug: ww-wc-reset
---

# WW/WC Reset to Default — 需求

**Source artifacts：** 功能请求描述（内联，无外部 PRD 文件）

## 用户故事

作为一名 **放射科医生或 CT 技师**，我希望在图像显示面板中有一个 **Reset Window/Level 按钮**，这样在交互式调整后，我可以立即恢复 DICOM 提供的默认 WW（Window Width）和 WC（Window Center）值，并避免由非标准窗宽/窗位设置导致的诊断错误。

---

## IEC 62304 安全分类

- **Assigned Class：** B
- **Justification：** reset 功能直接影响诊断 CT 图像的渲染。不正确或非默认的 WW/WC 设置可能导致具有临床意义的特征（例如肺结节、出血）不可见或被误读，从而导致漏诊或延迟诊断。根据临床上下文，漏诊可能构成从非严重到严重的患者伤害。reset 功能本身是一项安全缓解措施 — 它恢复已知良好的 DICOM defaults — 但它属于具有诊断影响的 image-display pipeline。按 IEC 62304 分类表，Class B 适用于故障可能导致非严重伤害的软件。
- **OQ-01 resolved (2026-07-02)：** 部署上下文确认为 **primary diagnostic workstation**。保留 Class B（用户在 Gate 1 approval 中确认）。
- **OQ-02 resolved (2026-07-02)：** Undo stack scope 为 **per-viewport**（不是全局 application undo）。
- **Patient-data path：** 是。该功能从 loaded series dataset 读取 DICOM image display attributes（tags 0028,1050 和 0028,1051），并将派生的 WW/WC 值写入活动 display viewport state。它不修改、存储或传输 patient data — 对 DICOM dataset 而言是只读的。

---

## 功能需求

### FR-01：Reset 按钮存在与位置

图像显示面板 **shall** 包含一个可见的 "Reset Window/Level" 按钮（或等价 icon-button），位于 WW/WC 控件区域内或紧邻该区域。只要活动视口中加载了 CT series，该按钮 **shall** 存在。

**Rationale：** 临床用户必须无需导航菜单即可定位并调用 reset 动作，从而减少阅片期间的操作时间。

**Source：** 功能请求 — "Add a 'reset window/level to default' button to the image display panel"

**Verification method：** BDD / Manual — 验证加载 series 时按钮正确渲染并定位；验证未加载 series 时按钮不存在（或被禁用）。

---

### FR-02：恢复 DICOM 默认 WW/WC

当 Reset Window/Level 按钮被激活时，系统 **shall** 将活动视口的 WW 和 WC 设置为当前 loaded series 中 DICOM tags (0028,1051) Window Width 和 (0028,1050) Window Center 保存的值，并使用每个多值属性的 **第一个值**（index 0）。

**Rationale：** DICOM tags 0028,1051（VR: DS，VM: 1–n）和 0028,1050（VR: DS，VM: 1–n）携带由扫描仪或 protocol 定义的默认 window presets。使用第一个值（index 0）对应 primary preset，符合 DICOM PS3.3 C.7.6.3.1.5。恢复的 WW 值 **must** 大于 0。

**Source：** 功能请求；DICOM PS3.3 §C.7.6.3.1.5；domain-knowledge WW/WC domain trigger

**Verification method：** Unit Test + BDD — 注入带已知 WW/WC 值的 mock DICOM dataset；激活 reset；断言 viewport WW/WC 等于注入值。

---

### FR-03：多 Preset 选择

当 loaded DICOM series 包含 **多个** WW/WC preset（即 tags 0028,1051 和 0028,1050 的 VM > 1）时，系统 **shall** 默认 reset 到第一个 preset（index 0）。当可选 tag (0028,1055) Window Center & Width Explanation 存在时，**shall** 使用它在确认消息中显示已应用 preset 的名称。

**Rationale：** 多值 windowing attributes 在 CT 中很常见（例如 soft tissue、lung、bone presets）。默认使用 index 0 可预测且符合 DICOM 惯例。显示 preset 名称可增强操作者信心。

**Source：** 功能请求；DICOM PS3.3 §C.7.6.3.1.5

**Verification method：** Unit Test — 注入多值 dataset；断言 index-0 值被应用，且来自 0028,1055[0] 的 preset name 出现在反馈消息中。

---

### FR-04：DICOM Defaults 缺失时的 Fallback

当 loaded series 中 tags (0028,1051) 或 (0028,1050) **缺失** 时，系统 **shall** 应用一个可配置的软件定义 fallback WW/WC 对（默认：WW = 400，WC = 40，代表标准 soft-tissue window）。fallback 值 **shall** 可在应用设置中配置，且不要求软件 rebuild。

**Rationale：** 并非所有 DICOM datasets 都包含 windowing attributes（它们在某些 IOD 中是 Type 1C/optional）。在 tags 缺失时硬失败对 diagnostic viewer 来说不可接受。fallback 对（WW 400，WC 40）是 CT soft tissue 的成熟临床起点。WW 必须保持 > 0。

**Source：** 功能请求（"If no DICOM default exists, a sensible fallback is applied"）；domain-knowledge WW > 0 rule

**Verification method：** Unit Test — 注入不含 windowing tags 的 dataset；激活 reset；断言 fallback 值被应用。Integration test — 验证 fallback 值从配置读取，而不是硬编码。

---

### FR-05：用户确认 / 反馈消息

reset 动作完成后，系统 **shall** 显示一条简短、非阻塞的确认消息（例如瞬态 tooltip 或状态栏通知），指示 window/level 已 reset。消息 **shall** 至少可见 2 秒，并无需用户关闭即可自动消失。如果 DICOM preset name 可用（来自 tag 0028,1055[0]），消息 **shall** 包含它（例如 "Window/Level reset to 'Soft Tissue' (WW 400 / WC 40)"）。

**Rationale：** 用户必须获得即时反馈，确认动作已经执行，尤其是在图像外观已经接近默认值时（视觉变化可能很细微或不可见）。非阻塞消息避免打断诊断工作流。

**Source：** 功能请求 — "shows a brief confirmation message to the user after the reset"

**Verification method：** BDD — 激活 reset；断言确认消息在 500 ms 内出现；断言消息在 ≥ 2 s 后自动消失。

---

### FR-06：Undo 支持

reset 动作 **shall** 可通过标准应用 Undo 功能撤销（例如 Ctrl+Z 或工具栏中的 Undo 按钮）。单次 Undo **shall** 恢复 reset 调用前立即生效的 WW/WC 值。

**Rationale：** 临床用户可能误触 reset，或需要比较调整后的视图与默认视图。支持 Undo 可保持工作流连续性并减少诊断干扰。如果应用的 Undo stack 尚不支持 WW/WC state，本需求要求将该支持作为此功能的一部分添加。

**Source：** 功能请求 — "The reset action is undoable"

**Verification method：** BDD — 设置非默认 WW/WC；激活 reset；调用 Undo；断言 WW/WC 返回 reset 前值。

---

### FR-07：WW 有效性约束执行

系统 **shall** 拒绝来自 DICOM tags、fallback configuration 或 Undo stack 的任何 WW 值 ≤ 0。遇到此类值时，系统 **shall** 记录 warning、应用可配置 fallback WW/WC，并通过确认消息通知用户由于 DICOM 值无效而使用了 fallback。

**Rationale：** WW = 0 或负值在 DICOM 标准中未定义，会产生黑图或反相图像，可能遮蔽全部图像内容。这是患者安全守卫。领域知识：WW > 0 是强制领域规则。

**Source：** Domain-knowledge skill — WW/WC domain trigger ("WW > 0")

**Verification method：** Unit Test — 注入 WW = 0 或 WW = -100 的 dataset；断言 fallback 被应用且 warning 被记录。

---

## 非功能需求

### NFR-01：响应时间（性能）

reset 动作 — 从按钮激活到图像渲染更新和确认消息出现 — 在最低规格目标硬件上测量时 **shall** 在 **200 ms** 内完成。用户 **shall** 感知为瞬时完成。

**Rationale：** 临床工作流对时间敏感。简单显示参数更新出现可感知延迟会降低可用性和用户对工具的信心。200 ms 低于人类 UI 延迟感知阈值。

**Source：** 功能请求 — "user perceives as instant"

**Verification method：** Performance test — 测量 100 次调用中按钮激活到 viewport repaint 的耗时；断言第 95 百分位 ≤ 200 ms。

---

### NFR-02：可访问性（可用性）

Reset Window/Level 按钮 **shall** 可仅通过键盘操作，并带有文档化键盘快捷键（默认：**Ctrl+Shift+W** 或应用标准等价项）。按钮 **shall** 具有描述性 tooltip（例如 "Reset Window/Level to DICOM default (Ctrl+Shift+W)"），并在 hover 时显示。按钮 **shall** 可通过图像显示面板内的标准 Tab-key navigation 访问。

**Rationale：** 放射科医生经常在无鼠标场景下操作 CT viewer（例如使用 trackball、dictation microphone 或纯键盘导航）。临床环境中的 IEC 62304 Class B 软件也要求 accessibility compliance。

**Source：** 功能请求 — "Accessibility (keyboard shortcut or tooltip)"

**Verification method：** Manual / BDD (FlaUI) — 验证键盘快捷键触发 reset；验证 tooltip 文本；验证 Tab navigation 可到达按钮。

---

### NFR-03：安全与 PHI 处理

reset 功能 **shall not** 因 reset 动作而记录、传输或持久化任何 patient-identifying information（PHI）。写入应用状态的 WW/WC 值只是显示参数，不构成 PHI。可配置 fallback 值 **shall** 存储在应用配置中，而不是任何 patient record 中。

**Rationale：** OWASP 和 HIPAA 要求禁止非预期 PHI 泄漏。显示状态变化不得把 patient context 泄露到日志或遥测中，超过应用既有 audit trail 已捕获的范围。

**Source：** IEC 62304 Class B security obligation；OWASP Top 10 A02 (Cryptographic Failures / sensitive data exposure)

**Verification method：** Code review — 检查 reset 触发的所有代码路径；确认没有 PHI fields 被访问或记录。

---

### NFR-04：可靠性与降级模式行为

reset 功能 **shall** 不会在任何受支持的 DICOM dataset 变体（missing tags、corrupt tag values、multi-frame series、empty series）下抛出未处理异常或导致应用崩溃。如果 reset 内部发生错误，系统 **shall** 以 WARNING level 记录错误，保持当前 WW/WC 不变，并告知用户 reset 无法完成。

**Rationale：** diagnostic viewer 中的未处理异常可能中断临床工作流并丢失未保存会话状态。系统必须优雅且信息明确地失败。

**Source：** IEC 62304 — no uncontrolled crashes；domain-knowledge — clinical practice 中 DICOM dataset 形式多样

**Verification method：** Unit Test — 注入损坏 tag values（非数字 DS strings、极端值）；断言没有异常传播；断言 log entry 被写入。

---

### NFR-05：DICOM 标准可追溯性（可维护性）

所有读取 DICOM tags (0028,1050)、(0028,1051) 和 (0028,1055) 的源代码 **shall** 在 inline XML documentation comment 中引用 tag group/element numbers 和 DICOM standard section（PS3.3 §C.7.6.3.1.5）。除专用 DICOM constants class 或文件外，不得出现 magic-number tag literals。

**Rationale：** 对 DICOM 标准的可维护性和审计可追溯性。集中 tag literals 能验证访问的是正确 tags，并简化未来标准更新。

**Source：** 功能请求 — "Traceability to DICOM standard tags"；source-code instructions — no magic numbers

**Verification method：** Code review — 验证 DICOM tag constants 位于专用 constants 文件；验证 XML doc comments 引用标准章节。

---

## 约束

- **WPF/MVVM architecture：** 按钮和确认消息必须通过带 ViewModel command binding（`ICommand`）的 XAML 实现。WPF views 中无 code-behind。
- **Namespace：** 新类必须遵循 `Philips.CT.Host.{ComponentName}.ImageDisplay` namespace pattern。
- **UI strings from resources：** 按钮标签、tooltip 文本和确认消息文本必须来自 resource files（`.resx`），不得硬编码。
- **WW > 0：** 系统绝不能允许 WW 值 0 或负数到达 display pipeline。
- **DICOM tag access：** Tags 必须通过项目既有 DICOM tag abstraction layer 访问；禁止在 ViewModel 中直接操作 DICOM dataset。
- **Zero build warnings：** 实现必须以零 warnings 编译。
- **IEC 62304 Class B lifecycle：** 交付前必须完成 unit tests、integration tests、code review 和 traceability documentation。

---

## 范围外

- 编辑或保存自定义 WW/WC presets 到 DICOM dataset 或 user profile。
- 跨多个 linked viewports 同步 WW/WC reset（除非已有架构支持，否则仅限 single-viewport scope）。
- 将 WW/WC 应用于非 CT modalities（MR、PET）— 此功能仅适用于 CT-series。
- series load 时自动 reset（reset 仅为显式/手动）。
- 远程/网络化 reset commands（例如通过 DICOM Softcopy Presentation State）。

---

## 风险

| # | 风险 | 可能性 | 影响 | 缓解措施 |
|---|------|-----------|--------|------------|
| R-01 | 来自 legacy scanner 的 DICOM dataset 缺少或包含零 WW/WC tags，导致 fallback 被静默应用 | 中 | 中 — 如果 fallback 与操作者预期不同，可能出现意外图像外观 | FR-04（fallback）+ FR-05（确认消息说明来源）；通过 IFU 培训操作者 |
| R-02 | 从 tag 读取到的 WW 值为 0 或负数（格式错误的 dataset） | 低 | 高 — 黑图/反相图像，漏诊 | FR-07（显式校验守卫） |
| R-03 | 当前应用中不存在 Undo stack；FR-06 需要新基础设施 | 中 | 高 — 进度/范围风险 | 标记为开放问题 OQ-02；实现前升级给 architect |
| R-04 | 确认消息在小显示配置中遮挡临床图像内容 | 低 | 低 | 优先使用非阻塞状态栏，而不是 overlay；需要 UX review |
| R-05 | 部署到 primary diagnostic workstation 导致安全等级从 B 提升到 C | 低 | 高 — 额外 lifecycle obligations | OQ-01 跟踪此项；若确认则需要 Class C work products |

---

## 开放问题

| # | 问题 | 负责人 | 是否阻塞？ |
|---|----------|-------|-----------|
| OQ-01 | 该组件是部署在 primary diagnostic workstation（IEC 62304 Class C 领域）还是 secondary review workstation（Class B）？这决定是否需要 Class C lifecycle activities。 | Product Owner / Regulatory | 是 — 必须在 architecture ADR 前解决 |
| OQ-02 | 应用当前是否有用于 WW/WC display state 的 Undo/Redo stack？如果没有，FR-06 需要新基础设施，功能范围扩大。 | SW Architect | 是 — 范围定义 |
| OQ-03 | 应用既有的 DICOM tag abstraction layer 是什么？FR-05 和 NFR-05 要求所有 tag access 都通过它。 | Developer / Architect | 否 — 可在 design phase 解决 |
| OQ-04 | 当前哪个组件拥有 WW/WC viewport state？（ViewModel、ViewService 或 image-rendering engine？）这决定 reset command 在哪里实现。 | SW Architect | 否 — 设计阶段问题 |
| OQ-05 | 键盘快捷键 Ctrl+Shift+W 是否已绑定到其他动作？确认或提出替代方案。 | Product Owner | 否 |
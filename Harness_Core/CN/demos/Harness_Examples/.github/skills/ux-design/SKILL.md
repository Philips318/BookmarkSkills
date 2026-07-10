---
name: ux-design
description: CT Windows desktop application 的 UX design tokens、colour palette、layout grid、typography 和 operator-facing UI terminology。设计新 screens、审查 UI layouts 或确保视觉一致性时使用。
---

# UX Design Skill

## Design Token Vocabulary

一致使用这些 tokens。绝不使用 raw hex colours 或 hard-coded pixel values。

| 类别 | 令牌 | 含义 |
|----------|-------|---------|
| Status — nominal | `StatusGreen` | Device operating within bounds |
| Status — warning | `StatusAmber` | Device operating outside soft limits |
| Status — fault | `StatusRed` | Device fault；requires operator action |
| Status — inactive | `StatusGrey` | Device not connected or powered off |
| 排版 | `HeaderFont` | Panel headers、window titles |
| 排版 | `DataFont` | Live numeric readouts |
| 排版 | `LabelFont` | Field labels |
| 间距 | `CompactSpacing` (4px) | Within a control group |
| 间距 | `StandardSpacing` (8px) | Between controls |
| 间距 | `SectionSpacing` (16px) | Between panel sections |

## Component Naming Convention

使用与 operator 词汇匹配的 domain-specific names。这些名称还必须设置为每个 control 的 `AutomationId` — `@feature-demonstrator` 使用 AutomationIds 定位 elements。

| 领域组件 | AutomationId | 描述 |
|-----------------|-------------|-------------|
| `PositionDisplay` | `PositionDisplay` | Device position/value 的 live numeric readout |
| `AlarmIndicator` | `AlarmIndicator` | Active alarms 的 colour/icon status |
| `ConnectButton` | `ConnectButton` | 建立 device connection |
| `ExportButton` | `ExportButton` | 触发 data export |
| `DevicePanel` | `DevicePanel` | Main device control panel |
| `StatusBar` | `StatusBar` | 窗口底部 connection status |

**绝不使用 `Button1`、`TextBlock2` 或 `Panel3` 这样的 generic names。**

## Layout Principles

- **Device status top-left** — operator 首先读取的信息；始终可见
- **Critical alarms 使用 `StatusRed`**，同时配合 visual flash 和 text label — 绝不只静默提示
- **Movement commands require confirmation** — 任何会物理移动设备的 action 都必须显示 confirmation dialog
- **Read-only displays 使用 `DataFont`**；editable inputs 使用视觉上不同的 background
- **Consistent grouping** — 相关 controls 放在带 labelled border 的 `GroupBox` 中

## Accessibility

- 所有 interactive elements 都必须设置 `AutomationId`（UI automation testing 所需）
- 所有 labels 都必须通过 `AutomationProperties.Name` attached property 设置 `AccessibleName`
- 状态不能仅靠颜色传达 — 搭配 icon 或 text label
- 最小 interactive target size：44×44px

## Gherkin Scenario Language for UX

为 UI features 编写 Gherkin 时，引用 AutomationIds 和 operator language：

```gherkin
# Correct — references observable UI element with exact value
Then the PositionDisplay shows "42.5 mm"
Then the AlarmIndicator is red

# Correct — uses operator language
When the operator clicks Connect

# Wrong — implementation detail
Then the PositionLabel.Text property equals "42.5 mm"
```

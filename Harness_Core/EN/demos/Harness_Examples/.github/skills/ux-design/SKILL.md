---
name: ux-design
description: UX design tokens, colour palette, layout grid, typography, and operator-facing UI terminology for the CT Windows desktop application. Use when designing new screens, reviewing UI layouts, or ensuring visual consistency.
---

# UX Design Skill

## Design Token Vocabulary

Use these tokens consistently. Never use raw hex colours or hard-coded pixel values.

| Category | Token | Meaning |
|----------|-------|---------|
| Status — nominal | `StatusGreen` | Device operating within bounds |
| Status — warning | `StatusAmber` | Device operating outside soft limits |
| Status — fault | `StatusRed` | Device fault; requires operator action |
| Status — inactive | `StatusGrey` | Device not connected or powered off |
| Typography | `HeaderFont` | Panel headers, window titles |
| Typography | `DataFont` | Live numeric readouts |
| Typography | `LabelFont` | Field labels |
| Spacing | `CompactSpacing` (4px) | Within a control group |
| Spacing | `StandardSpacing` (8px) | Between controls |
| Spacing | `SectionSpacing` (16px) | Between panel sections |

## Component Naming Convention

Use domain-specific names that match the operator's vocabulary. These names must also be set as `AutomationId` on each control — `@feature-demonstrator` uses AutomationIds to locate elements.

| Domain Component | AutomationId | Description |
|-----------------|-------------|-------------|
| `PositionDisplay` | `PositionDisplay` | Live numeric readout of device position/value |
| `AlarmIndicator` | `AlarmIndicator` | Colour/icon status for active alarms |
| `ConnectButton` | `ConnectButton` | Establishes device connection |
| `ExportButton` | `ExportButton` | Triggers data export |
| `DevicePanel` | `DevicePanel` | Main device control panel |
| `StatusBar` | `StatusBar` | Bottom-of-window connection status |

**Never use generic names like `Button1`, `TextBlock2`, or `Panel3`.**

## Layout Principles

- **Device status top-left** — first information the operator reads; always visible
- **Critical alarms use `StatusRed`** with both visual flash and text label — never silent-only
- **Movement commands require confirmation** — any action that physically moves the device must show a confirmation dialog
- **Read-only displays use `DataFont`**; editable inputs use a visually distinct background
- **Consistent grouping** — related controls in a `GroupBox` with a labelled border

## Accessibility

- All interactive elements must have `AutomationId` set (required for UI automation testing)
- All labels must set `AccessibleName` via the `AutomationProperties.Name` attached property
- Colour alone must not convey status — pair with an icon or text label
- Minimum interactive target size: 44×44px

## Gherkin Scenario Language for UX

When writing Gherkin for UI features, reference AutomationIds and operator language:

```gherkin
# Correct — references observable UI element with exact value
Then the PositionDisplay shows "42.5 mm"
Then the AlarmIndicator is red

# Correct — uses operator language
When the operator clicks Connect

# Wrong — implementation detail
Then the PositionLabel.Text property equals "42.5 mm"
```

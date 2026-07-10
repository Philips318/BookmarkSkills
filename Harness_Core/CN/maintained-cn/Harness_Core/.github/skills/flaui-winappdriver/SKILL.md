---
name: flaui-winappdriver
description: 面向 WPF/WinForms Windows desktop applications 的 FlaUI UI automation：通过 AutomationId 定位控件、与元素交互、实现 page objects、处理 async UI state。编写或调试 UI automation test code 时使用。
---

# FlaUI / WinAppDriver Skill

面向 Windows desktop CT application 的 WPF 和 WinForms UI automation。

## FlaUI vs WinAppDriver

| | FlaUI | WinAppDriver |
|--|-------|-------------|
| Setup | 只需 NuGet packages，无 external server | 需要运行 WinAppDriver.exe service |
| Speed | 快（in-process COM automation） | 较慢（到 server 的 HTTP round-trips） |
| WPF support | 优秀 | 良好 |
| Recommendation | **默认使用 FlaUI** | 仅在 FlaUI 不足时使用 |

所需 NuGet packages：`FlaUI.Core`、`FlaUI.UIA3`

## Application Launch

```csharp
using FlaUI.Core;
using FlaUI.UIA3;

var automation = new UIA3Automation();
var app = Application.Launch(
    Path.GetFullPath("Src/CT.App/bin/Debug/CT.App.exe"),
    "--simulator --seed=demo");
var window = app.GetMainWindow(automation, TimeSpan.FromSeconds(10));
```

## Element Location（优先使用 AutomationId）

```csharp
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;

// By AutomationId (most stable)
var connectButton = window
    .FindFirstDescendant(cf => cf.ByAutomationId("ConnectButton"))
    ?.AsButton();

// By Name (fallback — names can change with localisation)
var label = window
    .FindFirstDescendant(cf => cf.ByName("Position Display"))
    ?.AsLabel();

// Find all of a type
var allLabels = window.FindAllDescendants(cf => cf.ByControlType(ControlType.Text));
```

## Wait Patterns — Never Use Thread.Sleep

```csharp
using FlaUI.Core.Tools;

// Wait for element to appear (up to 5 seconds)
var element = Retry.WhileNull(
    () => window.FindFirstDescendant(cf => cf.ByAutomationId("PositionDisplay")),
    TimeSpan.FromSeconds(5),
    TimeSpan.FromMilliseconds(200)).Result;

// Wait for a value to change from placeholder
Retry.WhileFalse(
    () => positionLabel.Text != "---",
    TimeSpan.FromSeconds(3));
```

## Interactions

```csharp
button.Click();                              // left click
textBox.Enter("42.5");                       // type text (clears first)
comboBox.Select("Simulator Mode");           // dropdown selection
var displayedText = label.Text;              // read displayed string
var isChecked    = checkBox.IsChecked;       // boolean toggle state
```

## Evidence Capture

`@feature-demonstrator` 使用 ffmpeg 只录制应用程序窗口的压缩视频（见 `feature-demonstrator.agent.md` Step 2b）。对于 unit/integration tests 中的失败诊断：

```csharp
using FlaUI.Core.Capturing;

var screenshot = Capture.MainScreen();
var evidencePath = Path.Combine(
    ".harness", "demo_evidence", slug, $"task-{taskId}-failure.png");
Directory.CreateDirectory(Path.GetDirectoryName(evidencePath)!);
screenshot.ToFile(evidencePath);
```

> **注意：**Demo evidence 中绝不捕获完整桌面 — 只捕获 app window。这可以防止隐私泄露。

## Full Scenario Lifecycle (Hooks)

```csharp
[Binding]
public class AppLifecycleHooks
{
    private Application   _app;
    private UIA3Automation _automation;

    [BeforeScenario]
    public void LaunchApplication()
    {
        _automation = new UIA3Automation();
        _app = Application.Launch(
            Path.GetFullPath("Src/CT.App/bin/Debug/CT.App.exe"),
            "--simulator --seed=demo");
    }

    [AfterScenario]
    public void CloseApplication()
    {
        _app?.Close();
        _automation?.Dispose();
    }
}
```

## 常见 Demo Assertion Patterns

```csharp
// Verify a numeric display value
var positionText = window
    .FindFirstDescendant(cf => cf.ByAutomationId("PositionDisplay"))
    .AsLabel().Text;
Assert.That(positionText, Is.EqualTo("42.5 mm"));

// Verify alarm indicator colour
var indicator = window
    .FindFirstDescendant(cf => cf.ByAutomationId("AlarmIndicator"));
// For WPF: read a custom automation property or check the Name/HelpText set by the VM
Assert.That(indicator.HelpText, Is.EqualTo("Fault"));

// Verify a file was created by an export action
Assert.That(File.Exists("output/export.csv"), Is.True);
```

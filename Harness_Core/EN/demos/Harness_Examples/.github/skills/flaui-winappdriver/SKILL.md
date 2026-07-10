---
name: flaui-winappdriver
description: FlaUI UI automation for WPF/WinForms Windows desktop applications: locating controls by AutomationId, interacting with elements, implementing page objects, and handling async UI state. Use when writing or debugging UI automation test code.
---

# FlaUI / WinAppDriver Skill

WPF and WinForms UI automation for the Windows desktop CT application.

## FlaUI vs WinAppDriver

| | FlaUI | WinAppDriver |
|--|-------|-------------|
| Setup | NuGet packages only, no external server | Requires WinAppDriver.exe service running |
| Speed | Fast (in-process COM automation) | Slower (HTTP round-trips to server) |
| WPF support | Excellent | Good |
| Recommendation | **Use FlaUI by default** | Only if FlaUI is insufficient |

NuGet packages needed: `FlaUI.Core`, `FlaUI.UIA3`

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

## Element Location (Prefer AutomationId)

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

The `@feature-demonstrator` records a compressed video of only the application window using ffmpeg (see `feature-demonstrator.agent.md` Step 2b). For failure diagnostics in unit/integration tests:

```csharp
using FlaUI.Core.Capturing;

var screenshot = Capture.MainScreen();
var evidencePath = Path.Combine(
    ".harness", "demo_evidence", slug, $"task-{taskId}-failure.png");
Directory.CreateDirectory(Path.GetDirectoryName(evidencePath)!);
screenshot.ToFile(evidencePath);
```

> **Note:** Never capture the full desktop in demo evidence — only the app window. This prevents privacy leaks.

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

## Common Demo Assertion Patterns

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

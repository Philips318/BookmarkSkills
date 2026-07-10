---
name: ui-automation
description: UI automation tool selection, page object pattern, reliability practices (retry, waits, stable locators), and test isolation for Windows desktop applications. Use when designing or reviewing UI test architecture, choosing automation tools, or applying the page object pattern.
---

# UI Automation Skill

## Tool Selection

| Application Type | Recommended Tool |
|-----------------|-----------------|
| WPF (Windows) | FlaUI — see skill `flaui-winappdriver` |
| WinForms | FlaUI |
| Web / Electron | Playwright |
| Console output | Terminal output capture |

## Page Object Pattern

Never embed UI locators directly in test/step code. Use the Page Object pattern to centralise element access:

```csharp
public class MainWindowPage
{
    private readonly Window _window;

    public MainWindowPage(Window window) => _window = window;

    public string PositionDisplayText =>
        _window.FindFirstDescendant(cf => cf.ByAutomationId("PositionDisplay"))
               ?.AsLabel()?.Text ?? string.Empty;

    public void ClickConnectButton() =>
        _window.FindFirstDescendant(cf => cf.ByAutomationId("ConnectButton"))
               ?.AsButton()?.Click();

    public bool IsAlarmIndicatorRed =>
        _window.FindFirstDescendant(cf => cf.ByAutomationId("AlarmIndicator"))
               ?.BackgroundColor == Color.Red;
}
```

## Reliability Rules

- **Use AutomationId**, not Name or ClassName — names change; AutomationId is stable
- **Wait for element readiness**, never use `Thread.Sleep`:
  ```csharp
  var element = Retry.WhileNull(
      () => _window.FindFirstDescendant(cf => cf.ByAutomationId("ConnectButton")),
      TimeSpan.FromSeconds(5));
  ```
- **Retry on transient failures** — wrap interactions in a short retry policy for flaky elements
- **Launch in simulator/test mode** — pass `--simulator` flag so no real hardware is required
- **Clean up after each scenario** — kill the application process in `[AfterScenario]`
- **Record video, not screenshots** — the `@feature-demonstrator` uses ffmpeg to capture only the app window

## Application Launch Pattern (Scenario Hooks)

```csharp
[Binding]
public class AppLifecycleHooks
{
    private Application _app;
    private UIA3Automation _automation;

    [BeforeScenario]
    public void LaunchApp()
    {
        _automation = new UIA3Automation();
        var exePath = Path.GetFullPath("Src/CT.App/bin/Debug/CT.App.exe");
        _app = Application.Launch(exePath, "--simulator --seed=demo");
    }

    [AfterScenario]
    public void CloseApp()
    {
        _app?.Close();
        _automation?.Dispose();
    }
}
```

## Evidence Capture

The `@feature-demonstrator` records a compressed video of only the application window using ffmpeg. For failure diagnostics in unit/integration tests, capture a window screenshot:

```csharp
using FlaUI.Core.Capturing;

var screenshot = Capture.MainScreen();
var path = $".harness/demo_evidence/task-{taskId}-failure.png";
screenshot.ToFile(path);
```

> **Note:** Never capture the full desktop in demo evidence — only the app window. This prevents privacy leaks.

---
name: ui-automation
description: Windows desktop applications 的 UI automation tool selection、page object pattern、reliability practices（retry、waits、stable locators）和 test isolation。设计或审查 UI test architecture、选择 automation tools 或应用 page object pattern 时使用。
---

# UI Automation Skill

## Tool Selection

| Application Type | Recommended Tool |
|-----------------|-----------------|
| WPF (Windows) | FlaUI — 见 skill `flaui-winappdriver` |
| WinForms | FlaUI |
| Web / Electron | Playwright |
| Console output | Terminal output capture |

## Page Object Pattern

绝不要在 test/step code 中直接嵌入 UI locators。使用 Page Object pattern 集中 element access：

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

- **使用 AutomationId**，不要用 Name 或 ClassName — names 会变化；AutomationId 稳定
- **等待 element readiness**，绝不用 `Thread.Sleep`：
  ```csharp
  var element = Retry.WhileNull(
      () => _window.FindFirstDescendant(cf => cf.ByAutomationId("ConnectButton")),
      TimeSpan.FromSeconds(5));
  ```
- **对 transient failures 重试** — 用短 retry policy 包裹 flaky elements 的 interactions
- **以 simulator/test mode 启动** — 传递 `--simulator` flag，这样不需要真实硬件
- **每个 scenario 后清理** — 在 `[AfterScenario]` 中 kill application process
- **录制 video，而不是 screenshots** — `@feature-demonstrator` 使用 ffmpeg 只捕获 app window

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

`@feature-demonstrator` 使用 ffmpeg 只录制应用程序窗口的压缩视频。对于 unit/integration tests 中的失败诊断，捕获窗口 screenshot：

```csharp
using FlaUI.Core.Capturing;

var screenshot = Capture.MainScreen();
var path = $".harness/demo_evidence/task-{taskId}-failure.png";
screenshot.ToFile(path);
```

> **注意：**Demo evidence 中绝不捕获完整桌面 — 只捕获 app window。这可以防止隐私泄露。

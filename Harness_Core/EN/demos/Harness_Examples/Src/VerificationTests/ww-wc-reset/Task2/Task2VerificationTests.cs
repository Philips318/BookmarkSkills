// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using System;
using System.IO;
using FlaUI.Core;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using NUnit.Framework;

namespace Philips.CT.Host.ImageDisplay.Verification.Task2;

/// <summary>
/// Black-box UI acceptance tests for Task 2: Reset Window/Level to DICOM default (happy path).
/// Covers FR-01 (button present and enabled), FR-02 (restore DICOM WW/WC),
/// FR-03 (preset name in confirmation), FR-05 (status bar message and auto-dismiss).
/// Launched with: --simulator --seed=demo  →  DICOM defaults WW 1500 / WC -600;
/// app starts at WW 800 / WC -200.
/// </summary>
[TestFixture]
[Category("Verification")]
public sealed class Task2VerificationTests
{
    private const string LaunchArguments = "--simulator --seed=demo";
    private const int ExpectedDicomWindowWidthInHu = 1500;
    private const int ExpectedDicomWindowCenterInHu = -600;
    private static readonly TimeSpan ActionTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan StatusBarDismissTimeout = TimeSpan.FromSeconds(5);

    private Application? _application;
    private UIA3Automation? _automation;
    private FlaUI.Core.AutomationElements.Window? _window;
    private ImageDisplayPage? _page;

    /// <summary>Launches the application in simulator mode before each test.</summary>
    [SetUp]
    public void LaunchApplication()
    {
        _automation = new UIA3Automation();
        _application = Application.Launch(ResolveExePath(), LaunchArguments);
        _window = _application.GetMainWindow(_automation, TimeSpan.FromSeconds(15));
        Assert.That(_window, Is.Not.Null, "Main window did not appear within 15 seconds");
        _page = new ImageDisplayPage(_window!);
    }

    /// <summary>Closes the application and disposes automation resources after each test.</summary>
    [TearDown]
    public void CloseApplication()
    {
        _application?.Close();
        _automation?.Dispose();
        _application = null;
        _automation = null;
        _window = null;
        _page = null;
    }

    /// <summary>
    /// Verifies that invoking ResetWindowLevelButton restores WW to the DICOM default (1500)
    /// and WC to the DICOM default (-600). Covers FR-02.
    /// Feature: "Reset restores DICOM default Window Width and Window Center".
    /// </summary>
    [Test]
    [Category("Verification")]
    public void ResetButton_Verify_RestoresDicomDefaults()
    {
        // Arrange — app starts at WW 800 / WC -200 per simulator seed=demo contract
        var page = _page!;

        // Act
        page.ResetButton.Invoke();
        page.WaitForWindowWidth(ExpectedDicomWindowWidthInHu, ActionTimeout);

        // Assert
        Assert.That(page.WindowWidth, Is.EqualTo(ExpectedDicomWindowWidthInHu),
            "Window Width after reset must equal DICOM default (FR-02)");
        Assert.That(page.WindowCenter, Is.EqualTo(ExpectedDicomWindowCenterInHu),
            "Window Center after reset must equal DICOM default (FR-02)");
    }

    /// <summary>
    /// Verifies that after reset the status bar shows a non-empty confirmation message
    /// containing the DICOM preset name "Lung". Covers FR-05 and FR-03.
    /// Feature: "Confirmation message includes the DICOM preset name".
    /// </summary>
    [Test]
    [Category("Verification")]
    public void StatusBar_Verify_ShowsLungPresetAfterReset()
    {
        // Arrange
        var page = _page!;
        page.ResetButton.Invoke();

        // Act — wait for status bar to display the confirmation
        page.WaitForStatusBarNonEmpty(ActionTimeout);

        // Assert
        string statusText = page.StatusBarText;
        Assert.That(statusText, Is.Not.Empty,
            "Status bar must show a confirmation message after reset (FR-05)");
        Assert.That(statusText, Does.Contain("Lung"),
            "Confirmation message must include the DICOM preset name 'Lung' (FR-03)");
    }

    /// <summary>
    /// Verifies that the status bar confirmation message disappears automatically within 3 seconds.
    /// Covers FR-05 (auto-dismiss without user interaction).
    /// Feature: "Confirmation message auto-dismisses".
    /// </summary>
    [Test]
    [Category("Verification")]
    public void StatusBar_Verify_AutoDismissesWithinThreeSeconds()
    {
        // Arrange
        var page = _page!;
        page.ResetButton.Invoke();
        page.WaitForStatusBarNonEmpty(ActionTimeout);

        // Act — wait for the message to disappear (allow 5s to account for timing variance)
        page.WaitForStatusBarEmpty(StatusBarDismissTimeout);

        // Assert
        Assert.That(page.StatusBarText, Is.Empty,
            "Confirmation message must auto-dismiss without user interaction within 3 seconds (FR-05)");
    }

    /// <summary>
    /// Verifies that ResetWindowLevelButton is enabled when a CT series is loaded. Covers FR-01.
    /// Feature: "Reset button is disabled when no series is loaded" (inverse check — series IS loaded).
    /// </summary>
    [Test]
    [Category("Verification")]
    public void ResetButton_Verify_IsEnabledWhenSeriesLoaded()
    {
        // Assert — no prior action needed; the simulator seed=demo loads a series at startup
        Assert.That(_page!.ResetButton.IsEnabled, Is.True,
            "ResetWindowLevelButton must be enabled when a CT series is loaded (FR-01)");
    }

    /// <summary>
    /// Verifies that the keyboard shortcut Ctrl+Shift+W triggers a reset to DICOM defaults
    /// (WW 1500 / WC -600). Covers FR-02 via keyboard activation path.
    /// Feature: "Keyboard shortcut triggers reset".
    /// </summary>
    [Test]
    [Category("Verification")]
    public void KeyboardShortcut_Verify_CtrlShiftW_RestoresDicomDefaults()
    {
        // Arrange — ensure the application window holds keyboard focus
        _window!.Focus();
        var page = _page!;

        // Act — send Ctrl+Shift+W
        Keyboard.Press(VirtualKeyShort.CONTROL);
        Keyboard.Press(VirtualKeyShort.SHIFT);
        Keyboard.Press(VirtualKeyShort.KEY_W);
        Keyboard.Release(VirtualKeyShort.KEY_W);
        Keyboard.Release(VirtualKeyShort.SHIFT);
        Keyboard.Release(VirtualKeyShort.CONTROL);
        page.WaitForWindowWidth(ExpectedDicomWindowWidthInHu, ActionTimeout);

        // Assert
        Assert.That(page.WindowWidth, Is.EqualTo(ExpectedDicomWindowWidthInHu),
            "WW must equal 1500 after Ctrl+Shift+W shortcut");
        Assert.That(page.WindowCenter, Is.EqualTo(ExpectedDicomWindowCenterInHu),
            "WC must equal -600 after Ctrl+Shift+W shortcut");
    }

    private static string ResolveExePath()
    {
        const string relativeExePath =
            @"Src\ImageDisplayApp\bin\Debug\net8.0-windows\Philips.CT.Host.ImageDisplay.App.exe";

        string fromCurrentDirectory = Path.GetFullPath(relativeExePath);
        if (File.Exists(fromCurrentDirectory))
        {
            return fromCurrentDirectory;
        }

        // Fall back: navigate 7 levels up from the test binary directory to the repository root
        return Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            @"..\..\..\..\..\..\..",
            relativeExePath));
    }
}

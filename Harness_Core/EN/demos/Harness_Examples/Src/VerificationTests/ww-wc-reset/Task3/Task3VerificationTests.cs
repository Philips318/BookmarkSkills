// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using System;
using System.IO;
using FlaUI.Core;
using FlaUI.UIA3;
using NUnit.Framework;

namespace Philips.CT.Host.ImageDisplay.Verification.Task3;

/// <summary>
/// Black-box UI acceptance tests for Task 3: Fallback when DICOM default Window/Level is unavailable.
/// Covers FR-01 (button enabled without tags), FR-04 (fallback WW 400 / WC 40),
/// FR-05 (status bar feedback for fallback).
/// Launched with: --simulator --seed=demo --no-dicom-ww  →  no DICOM windowing tags;
/// fallback WW 400 / WC 40; app starts at WW 800 / WC -200.
///
/// NOTE — scenarios NOT UI-observable (excluded from this test class):
///   "Warning is logged when fallback values are used" — log output is not surfaced through any
///     AutomationId-accessible UI element; verified at the unit/module test level.
///   "Fallback applied when DICOM Window Width is zero/negative" — these seedless edge cases
///     are not configurable via the public --simulator launch contract; verified at unit level.
/// </summary>
[TestFixture]
[Category("Verification")]
public sealed class Task3VerificationTests
{
    private const string LaunchArguments = "--simulator --seed=demo --no-dicom-ww";
    private const int ExpectedFallbackWindowWidthInHu = 400;
    private const int ExpectedFallbackWindowCenterInHu = 40;
    private static readonly TimeSpan ActionTimeout = TimeSpan.FromSeconds(5);

    private Application? _application;
    private UIA3Automation? _automation;
    private ImageDisplayPage? _page;

    /// <summary>Launches the application in fallback-mode simulator before each test.</summary>
    [SetUp]
    public void LaunchApplication()
    {
        _automation = new UIA3Automation();
        _application = Application.Launch(ResolveExePath(), LaunchArguments);
        var window = _application.GetMainWindow(_automation, TimeSpan.FromSeconds(15));
        Assert.That(window, Is.Not.Null, "Main window did not appear within 15 seconds");
        _page = new ImageDisplayPage(window!);
    }

    /// <summary>Closes the application and disposes automation resources after each test.</summary>
    [TearDown]
    public void CloseApplication()
    {
        _application?.Close();
        _automation?.Dispose();
        _application = null;
        _automation = null;
        _page = null;
    }

    /// <summary>
    /// Verifies that when DICOM windowing tags are absent the reset applies the software-defined
    /// fallback values WW 400 and WC 40. Covers FR-04.
    /// Feature: "Fallback values applied when DICOM windowing tags are absent".
    /// </summary>
    [Test]
    [Category("Verification")]
    public void ResetButton_Verify_AppliesFallbackValuesWhenNoTags()
    {
        // Arrange — app starts at WW 800 / WC -200 (no-dicom-ww mode)
        var page = _page!;

        // Act
        page.ResetButton.Invoke();
        page.WaitForWindowWidth(ExpectedFallbackWindowWidthInHu, ActionTimeout);

        // Assert
        Assert.That(page.WindowWidth, Is.EqualTo(ExpectedFallbackWindowWidthInHu),
            "Window Width after reset must equal fallback WW when DICOM tags are absent (FR-04)");
        Assert.That(page.WindowCenter, Is.EqualTo(ExpectedFallbackWindowCenterInHu),
            "Window Center after reset must equal fallback WC when DICOM tags are absent (FR-04)");
    }

    /// <summary>
    /// Verifies that after a fallback reset the status bar displays a non-empty feedback message.
    /// The exact wording is internal; this test confirms a user-visible notification was shown.
    /// Covers FR-04 + FR-05.
    /// Feature: "the confirmation message indicates that fallback defaults were used".
    /// </summary>
    [Test]
    [Category("Verification")]
    public void StatusBar_Verify_FallbackIndicatedAfterReset()
    {
        // Arrange
        var page = _page!;
        page.ResetButton.Invoke();

        // Act — wait for a status bar message to appear
        page.WaitForStatusBarNonEmpty(ActionTimeout);

        // Assert
        Assert.That(page.StatusBarText, Is.Not.Empty,
            "Status bar must display a fallback-reset notification (FR-04, FR-05)");
    }

    /// <summary>
    /// Verifies that ResetWindowLevelButton is enabled even when DICOM windowing tags are absent.
    /// Covers FR-01 (button must be present whenever a series is loaded, tag-presence independent).
    /// Feature: implicit from FR-01 — a tag-less series is still a loaded series.
    /// </summary>
    [Test]
    [Category("Verification")]
    public void ResetButton_Verify_IsEnabledForTaglessSeries()
    {
        // Assert — no prior action; the tag-less simulator series is already loaded at startup
        Assert.That(_page!.ResetButton.IsEnabled, Is.True,
            "ResetWindowLevelButton must be enabled when a tag-less CT series is loaded (FR-01)");
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

        return Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            @"..\..\..\..\..\..\..",
            relativeExePath));
    }
}

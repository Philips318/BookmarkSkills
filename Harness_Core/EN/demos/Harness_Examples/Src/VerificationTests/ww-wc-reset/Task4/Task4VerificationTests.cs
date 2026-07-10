// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using System;
using System.IO;
using FlaUI.Core;
using FlaUI.UIA3;
using NUnit.Framework;

namespace Philips.CT.Host.ImageDisplay.Verification.Task4;

/// <summary>
/// Black-box UI acceptance tests for Task 4: Undo/Redo support for Window/Level reset.
/// Covers FR-06 (undo restores previous WW/WC; undo unavailable before any reset),
/// FR-07 (redo reapplies the reset after an undo), and the REGRESSION defect found during
/// the demo: UndoButton must become enabled after invoking Reset.
/// Launched with: --simulator --seed=demo  →  DICOM defaults WW 1500 / WC -600;
/// app starts at WW 800 / WC -200.
/// </summary>
[TestFixture]
[Category("Verification")]
public sealed class Task4VerificationTests
{
    private const string LaunchArguments = "--simulator --seed=demo";
    private const int InitialWindowWidthInHu = 800;
    private const int InitialWindowCenterInHu = -200;
    private const int ExpectedDicomWindowWidthInHu = 1500;
    private const int ExpectedDicomWindowCenterInHu = -600;
    private static readonly TimeSpan ActionTimeout = TimeSpan.FromSeconds(5);

    private Application? _application;
    private UIA3Automation? _automation;
    private ImageDisplayPage? _page;

    /// <summary>Launches the application in simulator mode before each test.</summary>
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
    /// Verifies that UndoButton is disabled at startup before any reset has been performed.
    /// Covers FR-06 (undo unavailable when no reset has been performed).
    /// Feature: "Undo is unavailable when no reset has been performed".
    /// </summary>
    [Test]
    [Category("Verification")]
    public void UndoButton_Verify_IsDisabledBeforeAnyReset()
    {
        // Assert — no prior action; undo stack must be empty at startup
        Assert.That(_page!.UndoButton.IsEnabled, Is.False,
            "UndoButton must be disabled before any Window/Level reset has been performed (FR-06)");
    }

    /// <summary>
    /// REGRESSION test: verifies that UndoButton becomes enabled after invoking ResetWindowLevelButton.
    /// This defect was found during the feature demo — reset was not registering on the undo stack.
    /// Covers FR-06 (undo available after reset).
    /// Feature: "Undo restores the previous Window Width and Window Center" — precondition.
    /// </summary>
    [Test]
    [Category("Verification")]
    public void ResetButton_Verify_UndoButtonBecomesEnabled()
    {
        // Arrange
        var page = _page!;

        // Act — invoke reset and wait for the undo stack to register the action
        page.ResetButton.Invoke();
        page.WaitForUndoButtonEnabled(ActionTimeout);

        // Assert — REGRESSION: UndoButton was found disabled after reset in demo (defect)
        Assert.That(page.UndoButton.IsEnabled, Is.True,
            "UndoButton must become enabled after a reset has been performed (FR-06, REGRESSION)");
    }

    /// <summary>
    /// Verifies that invoking Undo after a reset restores WW to 800 and WC to -200
    /// (the pre-reset values). Covers FR-06.
    /// Feature: "Undo restores the previous Window Width and Window Center".
    /// </summary>
    [Test]
    [Category("Verification")]
    public void Undo_Verify_RestoresPreviousWindowLevel()
    {
        // Arrange — reset to DICOM defaults first, then wait for undo to become available
        var page = _page!;
        page.ResetButton.Invoke();
        page.WaitForWindowWidth(ExpectedDicomWindowWidthInHu, ActionTimeout);
        page.WaitForUndoButtonEnabled(ActionTimeout);

        // Act
        page.UndoButton.Invoke();
        page.WaitForWindowWidth(InitialWindowWidthInHu, ActionTimeout);

        // Assert
        Assert.That(page.WindowWidth, Is.EqualTo(InitialWindowWidthInHu),
            "Window Width after undo must equal the pre-reset value (FR-06)");
        Assert.That(page.WindowCenter, Is.EqualTo(InitialWindowCenterInHu),
            "Window Center after undo must equal the pre-reset value (FR-06)");
    }

    /// <summary>
    /// Verifies that invoking Redo after an undo reapplies the DICOM reset values
    /// WW 1500 / WC -600. Covers FR-07.
    /// Feature: "Redo reapplies the reset after an undo".
    /// </summary>
    [Test]
    [Category("Verification")]
    public void Redo_Verify_ReappliesResetAfterUndo()
    {
        // Arrange — reset → undo → then redo
        var page = _page!;
        page.ResetButton.Invoke();
        page.WaitForWindowWidth(ExpectedDicomWindowWidthInHu, ActionTimeout);
        page.WaitForUndoButtonEnabled(ActionTimeout);
        page.UndoButton.Invoke();
        page.WaitForWindowWidth(InitialWindowWidthInHu, ActionTimeout);
        page.WaitForRedoButtonEnabled(ActionTimeout);

        // Act
        page.RedoButton.Invoke();
        page.WaitForWindowWidth(ExpectedDicomWindowWidthInHu, ActionTimeout);

        // Assert
        Assert.That(page.WindowWidth, Is.EqualTo(ExpectedDicomWindowWidthInHu),
            "Window Width after redo must equal the DICOM default (FR-07)");
        Assert.That(page.WindowCenter, Is.EqualTo(ExpectedDicomWindowCenterInHu),
            "Window Center after redo must equal the DICOM default (FR-07)");
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

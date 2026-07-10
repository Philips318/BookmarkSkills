// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using System;
using System.Globalization;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;

namespace Philips.CT.Host.ImageDisplay.Verification.Task4;

/// <summary>
/// Page object encapsulating stable, AutomationId-based access to the Image Display
/// main window controls. All element lookups are fresh per call so that post-action
/// state is always observed from the live UI tree.
/// </summary>
internal sealed class ImageDisplayPage
{
    private readonly Window _window;

    /// <summary>Initialises the page with a reference to the running application's main window.</summary>
    /// <param name="window">The FlaUI Window element for the main application window.</param>
    internal ImageDisplayPage(Window window) => _window = window;

    /// <summary>Gets the Reset Window/Level button (AutomationId: ResetWindowLevelButton).</summary>
    internal Button ResetButton => RequireElement("ResetWindowLevelButton").AsButton();

    /// <summary>Gets the Undo button (AutomationId: UndoButton).</summary>
    internal Button UndoButton => RequireElement("UndoButton").AsButton();

    /// <summary>Gets the Redo button (AutomationId: RedoButton).</summary>
    internal Button RedoButton => RequireElement("RedoButton").AsButton();

    /// <summary>Gets the raw display text of the Window Width label (may contain thousands comma).</summary>
    internal string WindowWidthText => GetElementText("WindowWidthValue");

    /// <summary>Gets the raw display text of the Window Center label.</summary>
    internal string WindowCenterText => GetElementText("WindowCenterValue");

    /// <summary>Gets the current Window Width as an integer. Returns <see cref="int.MinValue"/> when the display is empty.</summary>
    internal int WindowWidth => ParseWindowValue(WindowWidthText);

    /// <summary>Gets the current Window Center as an integer. Returns <see cref="int.MinValue"/> when the display is empty.</summary>
    internal int WindowCenter => ParseWindowValue(WindowCenterText);

    /// <summary>Waits until the Window Width display reaches <paramref name="expectedWidthInHu"/>, up to <paramref name="timeout"/>.</summary>
    internal void WaitForWindowWidth(int expectedWidthInHu, TimeSpan timeout) =>
        Retry.WhileFalse(
            () => WindowWidth == expectedWidthInHu,
            timeout,
            TimeSpan.FromMilliseconds(100));

    /// <summary>Waits until UndoButton becomes enabled, up to <paramref name="timeout"/>.</summary>
    internal void WaitForUndoButtonEnabled(TimeSpan timeout) =>
        Retry.WhileFalse(
            () => _window.FindFirstDescendant(cf => cf.ByAutomationId("UndoButton"))
                        ?.AsButton()?.IsEnabled ?? false,
            timeout,
            TimeSpan.FromMilliseconds(100));

    /// <summary>Waits until RedoButton becomes enabled, up to <paramref name="timeout"/>.</summary>
    internal void WaitForRedoButtonEnabled(TimeSpan timeout) =>
        Retry.WhileFalse(
            () => _window.FindFirstDescendant(cf => cf.ByAutomationId("RedoButton"))
                        ?.AsButton()?.IsEnabled ?? false,
            timeout,
            TimeSpan.FromMilliseconds(100));

    private AutomationElement RequireElement(string automationId) =>
        _window.FindFirstDescendant(cf => cf.ByAutomationId(automationId))
        ?? throw new InvalidOperationException($"Required UI element '{automationId}' was not found in the application window.");

    private string GetElementText(string automationId)
    {
        var element = _window.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
        if (element == null)
        {
            return string.Empty;
        }

        var valuePattern = element.Patterns.Value.PatternOrDefault;
        return valuePattern != null ? valuePattern.Value ?? string.Empty : element.Name ?? string.Empty;
    }

    private static int ParseWindowValue(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return int.MinValue;
        }

        string cleaned = text.Replace(",", string.Empty, StringComparison.Ordinal).Trim();
        return int.TryParse(cleaned, NumberStyles.Integer | NumberStyles.AllowLeadingSign,
            CultureInfo.InvariantCulture, out int value) ? value : int.MinValue;
    }
}

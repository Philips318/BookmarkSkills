// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using Philips.CT.Host.ImageDisplay.Models;
using Philips.CT.Host.ImageDisplay.Services;
using Reqnroll;

namespace Philips.CT.Host.ImageDisplay.ModuleTests.Steps
{
    /// <summary>
    /// Reqnroll step definitions for the T02 feature:
    /// "Reset Window/Level to DICOM default — happy path".
    /// Exercises the full production stack: <see cref="DicomImageModel"/>,
    /// <see cref="ImageDisplayViewModel"/>, and <see cref="StatusBarNotifier"/>.
    /// </summary>
    [Binding]
    [Scope(Feature = "Reset Window/Level to DICOM default")]
    [Category("Module")]
    public sealed class Task2Steps : IDisposable
    {
        private DicomImageModel _dicomModel = new DicomImageModel();
        private StatusBarNotifier _statusNotifier = new StatusBarNotifier();
        private ImageDisplayViewModel _viewModel = null!;
        private bool _disposed;

        // ── Background ──────────────────────────────────────────────────────────

        [Given(@"a CT series is loaded with DICOM default WW (.*) and WC (.*)")]
        public void GivenSeriesLoadedWithDicomDefaults(double ww, double wc)
        {
            _dicomModel.LoadSeries(ww, wc);
            _viewModel = BuildViewModel();
        }

        // ── Arrange steps ───────────────────────────────────────────────────────

        [Given(@"the current Window Width is (.*) and Window Center is (.*)")]
        public void GivenCurrentWindowWidthAndCenter(double ww, double wc)
        {
            _viewModel.WindowWidth = ww;
            _viewModel.WindowCenter = wc;
        }

        [Given(@"no CT series is loaded in the active viewport")]
        public void GivenNoSeriesLoaded()
        {
            _dicomModel = new DicomImageModel();
            _statusNotifier = new StatusBarNotifier();
            _viewModel = BuildViewModel();
        }

        [Given(@"the CT series includes a preset name ""(.*)"" for the default window")]
        public void GivenSeriesIncludesPresetName(string presetName)
        {
            _dicomModel.LoadSeries(1500.0, -600.0, presetName);
            _viewModel = BuildViewModel();
        }

        // ── Act steps ───────────────────────────────────────────────────────────

        [When(@"the operator activates the ResetWindowLevelButton")]
        public void WhenOperatorActivatesResetButton()
        {
            _viewModel.ResetWindowLevelCommand.Execute(null);
        }

        [When(@"the operator views the image display panel")]
        public void WhenOperatorViewsPanel()
        {
            // Observing the panel; no state change required.
        }

        [When(@"the operator presses Ctrl+Shift+W")]
        public void WhenOperatorPressesKeyboardShortcut()
        {
            // Keyboard shortcut is wired to the same command as the button.
            _viewModel.ResetWindowLevelCommand.Execute(null);
        }

        // ── Assert steps ────────────────────────────────────────────────────────

        [Then(@"the Window Width is (.*)")]
        public void ThenWindowWidthIs(double expectedWw)
        {
            Assert.That(_viewModel.WindowWidth, Is.EqualTo(expectedWw));
        }

        [Then(@"the Window Center is (.*)")]
        public void ThenWindowCenterIs(double expectedWc)
        {
            Assert.That(_viewModel.WindowCenter, Is.EqualTo(expectedWc));
        }

        [Then(@"a confirmation message is displayed in the status bar")]
        public void ThenConfirmationMessageIsDisplayed()
        {
            Assert.That(_statusNotifier.CurrentMessage, Is.Not.Null.And.Not.Empty);
        }

        [Then(@"the ResetWindowLevelButton is disabled")]
        public void ThenResetButtonIsDisabled()
        {
            Assert.That(_viewModel.ResetWindowLevelCommand.CanExecute(null), Is.False);
        }

        [Then(@"the confirmation message includes the text ""(.*)""")]
        public void ThenConfirmationMessageIncludesText(string expectedText)
        {
            Assert.That(_statusNotifier.CurrentMessage, Does.Contain(expectedText));
        }

        [Then(@"the confirmation message disappears automatically within 3 seconds")]
        public void ThenConfirmationMessageDisappearsWithin3Seconds()
        {
            Assert.That(_statusNotifier.LastDisplayDuration, Is.LessThanOrEqualTo(TimeSpan.FromSeconds(3)));
        }

        // ── Helpers ─────────────────────────────────────────────────────────────

        private ImageDisplayViewModel BuildViewModel() =>
            new ImageDisplayViewModel(
                _dicomModel,
                _statusNotifier,
                new StubUndoService(),
                new StubWindowLevelDefaults());

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _statusNotifier.Dispose();
            _disposed = true;
        }

        // ── Test-only stubs ──────────────────────────────────────────────────────

        /// <summary>No-op undo service stub; undo functionality is implemented in T04.</summary>
        private sealed class StubUndoService : IUndoService
        {
            /// <inheritdoc/>
            public bool CanUndo => false;

            /// <inheritdoc/>
            public bool CanRedo => false;

            /// <inheritdoc/>
            public void Push(IUndoableAction action) { }

            /// <inheritdoc/>
            public void Undo() { }

            /// <inheritdoc/>
            public void Redo() { }
        }

        /// <summary>Fallback defaults stub; configurable defaults are implemented in T03.</summary>
        private sealed class StubWindowLevelDefaults : IWindowLevelDefaults
        {
            /// <inheritdoc/>
            public WindowLevel FallbackWindowLevel => WindowLevel.Create(400.0, 40.0);
        }
    }
}

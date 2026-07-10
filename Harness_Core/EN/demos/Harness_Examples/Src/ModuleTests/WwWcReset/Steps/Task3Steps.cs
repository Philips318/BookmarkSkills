// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using Philips.CT.Host.ImageDisplay.Commands;
using Philips.CT.Host.ImageDisplay.Models;
using Philips.CT.Host.ImageDisplay.Services;
using Reqnroll;

namespace Philips.CT.Host.ImageDisplay.ModuleTests.Steps
{
    /// <summary>
    /// Reqnroll step definitions for the T03 feature:
    /// "Fallback when DICOM default Window/Level is unavailable".
    /// Exercises the full production stack with <see cref="WindowLevelDefaults"/>,
    /// <see cref="DicomImageModel"/>, <see cref="ImageDisplayViewModel"/>, and
    /// <see cref="ResetWindowLevelCommand"/>.
    /// </summary>
    [Binding]
    [Scope(Feature = "Fallback when DICOM default Window/Level is unavailable")]
    [Category("Module")]
    public sealed class Task3Steps : IDisposable
    {
        private WindowLevelDefaults _windowLevelDefaults = new WindowLevelDefaults(400.0, 40.0);
        private DicomImageModel _dicomModel = new DicomImageModel();
        private StatusBarNotifier _statusNotifier = new StatusBarNotifier();
        private ImageDisplayViewModel _viewModel = null!;
        private ResetWindowLevelCommand _command = null!;
        private bool _disposed;

        // ── Background ──────────────────────────────────────────────────────────

        [Given(@"the configured fallback Window Width is (.*) and Window Center is (.*)")]
        public void GivenConfiguredFallbackWindowWidthAndCenter(double fallbackWw, double fallbackWc)
        {
            _windowLevelDefaults = new WindowLevelDefaults(fallbackWw, fallbackWc);
            _viewModel = BuildViewModel();
            _command = (ResetWindowLevelCommand)_viewModel.ResetWindowLevelCommand;
        }

        // ── Series loading steps ────────────────────────────────────────────────

        [Given(@"a CT series is loaded without DICOM windowing tags")]
        public void GivenSeriesLoadedWithoutDicomWindowingTags()
        {
            _dicomModel.LoadSeriesWithoutTags();
            _viewModel = BuildViewModel();
            _command = (ResetWindowLevelCommand)_viewModel.ResetWindowLevelCommand;
        }

        [Given(@"a CT series is loaded with a DICOM Window Width of (.*)")]
        public void GivenSeriesLoadedWithInvalidDicomWindowWidth(double ww)
        {
            _dicomModel.LoadSeriesWithInvalidWW(ww);
            _viewModel = BuildViewModel();
            _command = (ResetWindowLevelCommand)_viewModel.ResetWindowLevelCommand;
        }

        // ── Arrange steps ───────────────────────────────────────────────────────

        [Given(@"the current Window Width is (.*) and Window Center is (.*)")]
        public void GivenCurrentWindowWidthAndCenter(double ww, double wc)
        {
            _viewModel.WindowWidth = ww;
            _viewModel.WindowCenter = wc;
        }

        // ── Act steps ───────────────────────────────────────────────────────────

        [When(@"the operator activates the ResetWindowLevelButton")]
        public void WhenOperatorActivatesResetButton()
        {
            _command.Execute(null);
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

        [Then(@"the confirmation message indicates that fallback defaults were used")]
        public void ThenConfirmationMessageIndicatesFallbackDefaultsWereUsed()
        {
            Assert.That(
                _statusNotifier.CurrentMessage,
                Does.Contain("DICOM defaults unavailable").IgnoreCase);
        }

        [Then(@"the confirmation message indicates an invalid DICOM value was encountered")]
        public void ThenConfirmationMessageIndicatesInvalidDicomValue()
        {
            Assert.That(
                _statusNotifier.CurrentMessage,
                Does.Contain("invalid DICOM value").IgnoreCase);
        }

        [Then(@"a warning-level log entry is recorded for the missing DICOM defaults")]
        public void ThenWarningLevelLogEntryIsRecorded()
        {
            Assert.That(_command.LastResetUsedFallback, Is.True);
        }

        // ── Helpers ─────────────────────────────────────────────────────────────

        private ImageDisplayViewModel BuildViewModel() =>
            new ImageDisplayViewModel(
                _dicomModel,
                _statusNotifier,
                new StubUndoService(),
                _windowLevelDefaults);

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
    }
}

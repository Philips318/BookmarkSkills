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
    /// Reqnroll step definitions for the T04 feature:
    /// "Undo support for Window/Level reset".
    /// Exercises the full production stack: <see cref="DicomImageModel"/>,
    /// <see cref="WindowLevelUndoService"/>, <see cref="ImageDisplayViewModel"/>,
    /// and the Undo/Redo commands.
    /// </summary>
    [Binding]
    [Scope(Feature = "Undo support for Window/Level reset")]
    [Category("Module")]
    public sealed class Task4Steps : IDisposable
    {
        private DicomImageModel _dicomModel = new DicomImageModel();
        private StatusBarNotifier _statusNotifier = new StatusBarNotifier();
        private WindowLevelUndoService _undoService = new WindowLevelUndoService();
        private ImageDisplayViewModel _viewModel = null!;
        private bool _disposed;

        // ── Background ──────────────────────────────────────────────────────────

        /// <summary>Loads a CT series with the given DICOM default window level values.</summary>
        [Given(@"a CT series is loaded with DICOM default WW (.*) and WC (.*)")]
        public void GivenSeriesLoadedWithDicomDefaults(double ww, double wc)
        {
            _dicomModel.LoadSeries(ww, wc);
            _viewModel = BuildViewModel();
        }

        // ── Arrange steps ───────────────────────────────────────────────────────

        /// <summary>Sets the current Window Width and Window Center on the ViewModel.</summary>
        [Given(@"the current Window Width is (.*) and Window Center is (.*)")]
        public void GivenCurrentWindowWidthAndCenter(double ww, double wc)
        {
            _viewModel.WindowWidth = ww;
            _viewModel.WindowCenter = wc;
        }

        /// <summary>Executes the reset command (used as a Given step for undo/redo setup).</summary>
        [Given(@"the operator has activated the ResetWindowLevelButton")]
        public void GivenOperatorActivatedResetButton()
        {
            _viewModel.ResetWindowLevelCommand.Execute(null);
        }

        /// <summary>Invokes Undo (used as a Given step to set up redo scenarios).</summary>
        [Given(@"the operator has invoked Undo")]
        public void GivenOperatorInvokedUndo()
        {
            _viewModel.UndoCommand.Execute(null);
        }

        /// <summary>Ensures a clean ViewModel with an empty undo stack (no reset performed yet).</summary>
        /// <remarks>
        /// Forced to <see cref="ExpressionType.RegularExpression"/> because the literal step text
        /// contains a forward slash ("Window/Level"), which Reqnroll would otherwise interpret as a
        /// Cucumber-Expression alternative ("Window" OR "Level") and fail to match.
        /// </remarks>
        [Given(@"no Window/Level changes have been made", ExpressionType = ExpressionType.RegularExpression)]
        public void GivenNoWindowLevelChangesMade()
        {
            // Rebuild the ViewModel over a fresh undo service so the undo stack is guaranteed empty.
            _undoService = new WindowLevelUndoService();
            _viewModel = BuildViewModel();
        }

        // ── Act steps ───────────────────────────────────────────────────────────

        /// <summary>Invokes the Undo command on the ViewModel.</summary>
        [When(@"the operator invokes Undo")]
        public void WhenOperatorInvokesUndo()
        {
            _viewModel.UndoCommand.Execute(null);
        }

        /// <summary>Invokes the Redo command on the ViewModel.</summary>
        [When(@"the operator invokes Redo")]
        public void WhenOperatorInvokesRedo()
        {
            _viewModel.RedoCommand.Execute(null);
        }

        /// <summary>No-op step: the undo-availability check is an observation only.</summary>
        [When(@"the operator checks the Undo availability")]
        public void WhenOperatorChecksUndoAvailability()
        {
            // Observation only; no action required. Assertion happens in the Then step.
        }

        // ── Assert steps ────────────────────────────────────────────────────────

        /// <summary>Asserts that the ViewModel's Window Width equals the expected value.</summary>
        [Then(@"the Window Width is (.*)")]
        public void ThenWindowWidthIs(double expectedWw)
        {
            Assert.That(_viewModel.WindowWidth, Is.EqualTo(expectedWw));
        }

        /// <summary>Asserts that the ViewModel's Window Center equals the expected value.</summary>
        [Then(@"the Window Center is (.*)")]
        public void ThenWindowCenterIs(double expectedWc)
        {
            Assert.That(_viewModel.WindowCenter, Is.EqualTo(expectedWc));
        }

        /// <summary>Asserts that the Undo command is not available (CanExecute returns false).</summary>
        /// <remarks>
        /// Forced to <see cref="ExpressionType.RegularExpression"/> because the literal step text
        /// contains a forward slash ("Window/Level"), which Reqnroll would otherwise interpret as a
        /// Cucumber-Expression alternative ("Window" OR "Level") and fail to match.
        /// </remarks>
        [Then(@"the Undo function for Window/Level changes is not available", ExpressionType = ExpressionType.RegularExpression)]
        public void ThenUndoIsNotAvailable()
        {
            Assert.That(_viewModel.UndoCommand.CanExecute(null), Is.False);
        }

        // ── Helpers ─────────────────────────────────────────────────────────────

        private ImageDisplayViewModel BuildViewModel() =>
            new ImageDisplayViewModel(
                _dicomModel,
                _statusNotifier,
                _undoService,
                new WindowLevelDefaults(400.0, 40.0));

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
    }
}

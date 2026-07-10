// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using System.Diagnostics;
using System.Windows.Input;
using Philips.CT.Host.ImageDisplay.Models;
using Philips.CT.Host.ImageDisplay.Resources;

namespace Philips.CT.Host.ImageDisplay.Commands
{
    /// <summary>
    /// Command that resets the current Window Width and Window Center to the values stored in
    /// the DICOM series default, falling back to configured defaults when no DICOM values are
    /// available.
    /// </summary>
    /// <remarks>
    /// This command is bound to the Reset W/L button and the Ctrl+Shift+W keyboard shortcut.
    /// It is enabled only when a series is loaded (<see cref="IImageDisplayViewModel.IsSeriesLoaded"/>).
    /// </remarks>
    public sealed class ResetWindowLevelCommand : ICommand
    {
        private readonly IImageDisplayViewModel _viewModel;
        private readonly IDicomImageModel _dicomModel;
        private readonly IStatusNotifier _statusNotifier;
        private readonly IUndoService _undoService;
        private readonly IWindowLevelDefaults _windowLevelDefaults;

        /// <summary>
        /// Initialises a new instance of <see cref="ResetWindowLevelCommand"/>.
        /// </summary>
        /// <param name="viewModel">The ViewModel whose WW/WC properties will be updated.</param>
        /// <param name="dicomModel">Source of DICOM-default window level values.</param>
        /// <param name="statusNotifier">Displays the reset confirmation in the status bar.</param>
        /// <param name="undoService">Undo stack; the reset action is pushed before applying values.</param>
        /// <param name="windowLevelDefaults">Fallback defaults used when DICOM values are absent.</param>
        public ResetWindowLevelCommand(
            IImageDisplayViewModel viewModel,
            IDicomImageModel dicomModel,
            IStatusNotifier statusNotifier,
            IUndoService undoService,
            IWindowLevelDefaults windowLevelDefaults)
        {
            _viewModel = viewModel;
            _dicomModel = dicomModel;
            _statusNotifier = statusNotifier;
            _undoService = undoService;
            _windowLevelDefaults = windowLevelDefaults;
        }

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <inheritdoc/>
        public bool CanExecute(object? parameter) => _viewModel.IsSeriesLoaded;

        /// <summary>
        /// Gets a value indicating whether the most recent <see cref="Execute"/> call applied
        /// fallback Window Level values because the DICOM series lacked valid windowing tags.
        /// </summary>
        public bool LastResetUsedFallback { get; private set; }

        /// <inheritdoc/>
        public void Execute(object? parameter)
        {
            try
            {
                var (windowLevel, isFallback, isInvalidDicom) = ResolveWindowLevel();
                LastResetUsedFallback = isFallback;
                var action = new WindowLevelChangeAction(
                    _viewModel.WindowWidth,
                    _viewModel.WindowCenter,
                    windowLevel.Width,
                    windowLevel.Center,
                    (ww, wc) => { _viewModel.WindowWidth = ww; _viewModel.WindowCenter = wc; });
                _undoService.Push(action);
                action.Execute();
                string message = BuildStatusMessage(windowLevel, isFallback, isInvalidDicom);
                _statusNotifier.ShowTransientMessage(message, TimeSpan.FromSeconds(2));
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"ResetWindowLevelCommand.Execute failed: {ex.Message}");
            }
        }

        /// <summary>Raises <see cref="CanExecuteChanged"/> to notify bindings of a state change.</summary>
        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        private (WindowLevel Level, bool IsFallback, bool IsInvalidDicom) ResolveWindowLevel()
        {
            var dicomDefault = _dicomModel.DefaultWindowLevel;
            if (dicomDefault != null)
            {
                return (dicomDefault, false, false);
            }

            bool hasInvalidTags = _dicomModel.HasDicomWindowingTags;
            if (hasInvalidTags)
            {
                Debug.WriteLine("[WARN] ResetWindowLevelCommand: DICOM WW \u2264 0; applying fallback.");
            }
            else
            {
                Debug.WriteLine("[WARN] ResetWindowLevelCommand: DICOM windowing tags absent; applying fallback.");
            }

            return (_windowLevelDefaults.FallbackWindowLevel, true, hasInvalidTags);
        }

        private string BuildStatusMessage(WindowLevel wl, bool isFallback, bool isInvalidDicom)
        {
            if (!isFallback)
            {
                return FormatConfirmationMessage(wl);
            }

            string template = isInvalidDicom
                ? ImageDisplayStrings.ResetInvalidDicomMessage
                : ImageDisplayStrings.ResetFallbackMessage;
            return string.Format(template, wl.Width, wl.Center);
        }

        private string FormatConfirmationMessage(WindowLevel wl)
        {
            if (wl.PresetName != null)
            {
                return string.Format(
                    ImageDisplayStrings.ResetConfirmationMessageWithPreset,
                    wl.PresetName, wl.Width, wl.Center);
            }

            return string.Format(
                ImageDisplayStrings.ResetConfirmationMessage,
                wl.Width, wl.Center);
        }
    }
}

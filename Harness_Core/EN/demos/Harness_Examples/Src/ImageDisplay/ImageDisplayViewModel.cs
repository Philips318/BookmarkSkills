// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Philips.CT.Host.ImageDisplay.Commands;

namespace Philips.CT.Host.ImageDisplay
{
    /// <summary>
    /// ViewModel for the Image Display panel.
    /// Implements <see cref="IImageDisplayViewModel"/> and exposes Window Width/Center,
    /// the reset command, series-loaded state, and the current status message.
    /// </summary>
    public sealed class ImageDisplayViewModel : IImageDisplayViewModel, INotifyPropertyChanged
    {
        private readonly IDicomImageModel _dicomModel;
        private readonly IUndoService _undoService;
        private readonly DelegateCommand _undoCommand;
        private readonly DelegateCommand _redoCommand;
        private double _windowWidth;
        private double _windowCenter;
        private string _statusMessage = string.Empty;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Initialises a new instance of <see cref="ImageDisplayViewModel"/>.
        /// </summary>
        /// <param name="dicomModel">DICOM image model providing default WW/WC values.</param>
        /// <param name="statusNotifier">Status-bar notifier for reset confirmations.</param>
        /// <param name="undoService">Undo stack service for Window/Level changes.</param>
        /// <param name="windowLevelDefaults">Fallback WW/WC defaults when DICOM values are absent.</param>
        public ImageDisplayViewModel(
            IDicomImageModel dicomModel,
            IStatusNotifier statusNotifier,
            IUndoService undoService,
            IWindowLevelDefaults windowLevelDefaults)
        {
            _dicomModel = dicomModel;
            _undoService = undoService;
            _undoCommand = new DelegateCommand(ExecuteUndo, () => _undoService.CanUndo);
            _redoCommand = new DelegateCommand(ExecuteRedo, () => _undoService.CanRedo);
            ResetWindowLevelCommand = new ResetWindowLevelCommand(
                this, dicomModel, statusNotifier, undoService, windowLevelDefaults);
        }

        /// <inheritdoc/>
        public double WindowWidth
        {
            get => _windowWidth;
            set
            {
                _windowWidth = value;
                OnPropertyChanged();
                RefreshUndoRedoState();
            }
        }

        /// <inheritdoc/>
        public double WindowCenter
        {
            get => _windowCenter;
            set
            {
                _windowCenter = value;
                OnPropertyChanged();
                RefreshUndoRedoState();
            }
        }

        /// <inheritdoc/>
        public ICommand ResetWindowLevelCommand { get; }

        /// <inheritdoc/>
        public ICommand UndoCommand => _undoCommand;

        /// <inheritdoc/>
        public ICommand RedoCommand => _redoCommand;

        /// <inheritdoc/>
        public bool IsSeriesLoaded => _dicomModel.IsSeriesLoaded;

        /// <summary>
        /// Gets the most recent status message set by the reset command.
        /// Mirrors the current message from the injected <see cref="IStatusNotifier"/>.
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            internal set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        private void ExecuteUndo()
        {
            _undoService.Undo();
            RefreshUndoRedoState();
        }

        private void ExecuteRedo()
        {
            _undoService.Redo();
            RefreshUndoRedoState();
        }

        /// <summary>
        /// Re-evaluates the enabled state of the Undo and Redo commands so bound UI controls
        /// (e.g. WPF buttons) refresh whenever the undo stack may have changed. The reset command
        /// applies its new values through the Window Width/Center setters, so refreshing here also
        /// enables the Undo button immediately after a reset, not only after an undo/redo.
        /// </summary>
        private void RefreshUndoRedoState()
        {
            _undoCommand.RaiseCanExecuteChanged();
            _redoCommand.RaiseCanExecuteChanged();
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>Lightweight command implementation that delegates execution and CanExecute evaluation.</summary>
        private sealed class DelegateCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;

            /// <summary>Initialises a new <see cref="DelegateCommand"/>.</summary>
            /// <param name="execute">The action to invoke on <see cref="Execute"/>.</param>
            /// <param name="canExecute">The predicate evaluated by <see cref="CanExecute"/>.</param>
            public DelegateCommand(Action execute, Func<bool> canExecute)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            /// <inheritdoc/>
            public event EventHandler? CanExecuteChanged;

            /// <inheritdoc/>
            public bool CanExecute(object? parameter) => _canExecute();

            /// <inheritdoc/>
            public void Execute(object? parameter) => _execute();

            /// <summary>Raises <see cref="CanExecuteChanged"/> to notify bindings of a state change.</summary>
            public void RaiseCanExecuteChanged() =>
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

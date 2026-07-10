// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using NSubstitute;
using Philips.CT.Host.ImageDisplay.Models;
using Philips.CT.Host.ImageDisplay.Services;

namespace Philips.CT.Host.ImageDisplay.Test
{
    /// <summary>
    /// Unit tests for <see cref="ImageDisplayViewModel"/>.
    /// Verifies WW/WC property changes, <see cref="IImageDisplayViewModel.IsSeriesLoaded"/>,
    /// <see cref="INotifyPropertyChanged"/> notifications, and command delegation.
    /// </summary>
    [TestFixture]
    [Category("Unit")]
    public class ImageDisplayViewModelTests
    {
        private DicomImageModel _dicomModel = null!;
        private StatusBarNotifier _statusNotifier = null!;
        private IUndoService _undoService = null!;
        private IWindowLevelDefaults _windowLevelDefaults = null!;
        private ImageDisplayViewModel _viewModel = null!;

        [SetUp]
        public void SetUp()
        {
            _dicomModel = new DicomImageModel();
            _statusNotifier = new StatusBarNotifier();
            _undoService = Substitute.For<IUndoService>();
            _windowLevelDefaults = Substitute.For<IWindowLevelDefaults>();

            _viewModel = new ImageDisplayViewModel(
                _dicomModel, _statusNotifier, _undoService, _windowLevelDefaults);
        }

        [TearDown]
        public void TearDown() => _statusNotifier.Dispose();

        [Test]
        public void IsSeriesLoaded_ReturnsFalse_WhenNoSeriesLoaded()
        {
            // Assert
            Assert.That(_viewModel.IsSeriesLoaded, Is.False);
        }

        [Test]
        public void IsSeriesLoaded_ReturnsTrue_AfterSeriesLoaded()
        {
            // Arrange
            _dicomModel.LoadSeries(1500.0, -600.0);

            // Assert
            Assert.That(_viewModel.IsSeriesLoaded, Is.True);
        }

        [Test]
        public void ResetWindowLevelCommand_CanExecute_ReturnsFalse_WhenNotLoaded()
        {
            // Assert — no series loaded yet
            Assert.That(_viewModel.ResetWindowLevelCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void ResetWindowLevelCommand_CanExecute_ReturnsTrue_WhenLoaded()
        {
            // Arrange
            _dicomModel.LoadSeries(1500.0, -600.0);

            // Assert
            Assert.That(_viewModel.ResetWindowLevelCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void ResetWindowLevelCommand_CanExecute_ReturnsTrue_WhenLoadedWithoutTags()
        {
            // Arrange — a series is loaded but carries no DICOM windowing tags.
            // Reset must stay enabled so fallback defaults can be applied (FR-01, FR-04).
            _dicomModel.LoadSeriesWithoutTags();

            // Assert
            Assert.That(_viewModel.IsSeriesLoaded, Is.True);
            Assert.That(_viewModel.ResetWindowLevelCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void ResetWindowLevelCommand_CanExecute_ReturnsTrue_WhenLoadedWithInvalidWw()
        {
            // Arrange — a series is loaded but the DICOM WW is invalid (≤ 0).
            // Reset must stay enabled so fallback defaults can be applied (FR-07).
            _dicomModel.LoadSeriesWithInvalidWW(0.0);

            // Assert
            Assert.That(_viewModel.IsSeriesLoaded, Is.True);
            Assert.That(_viewModel.ResetWindowLevelCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void Execute_UpdatesWindowWidth()
        {
            // Arrange
            _dicomModel.LoadSeries(1500.0, -600.0);

            // Act
            _viewModel.ResetWindowLevelCommand.Execute(null);

            // Assert
            Assert.That(_viewModel.WindowWidth, Is.EqualTo(1500.0));
        }

        [Test]
        public void Execute_UpdatesWindowCenter()
        {
            // Arrange
            _dicomModel.LoadSeries(1500.0, -600.0);

            // Act
            _viewModel.ResetWindowLevelCommand.Execute(null);

            // Assert
            Assert.That(_viewModel.WindowCenter, Is.EqualTo(-600.0));
        }

        [Test]
        public void Execute_SetsStatusMessage()
        {
            // Arrange
            _dicomModel.LoadSeries(1500.0, -600.0);

            // Act
            _viewModel.ResetWindowLevelCommand.Execute(null);

            // Assert
            Assert.That(_statusNotifier.CurrentMessage, Is.Not.Empty);
        }

        [Test]
        public void Execute_RaisesUndoAndRedoCanExecuteChanged()
        {
            // Arrange — a reset applies new values through the WW/WC setters. Bound WPF buttons
            // only re-query CanExecute when CanExecuteChanged fires, so the reset must raise it
            // (regression: the Undo button previously stayed disabled after a reset).
            _dicomModel.LoadSeries(1500.0, -600.0);
            var undoRaised = false;
            var redoRaised = false;
            _viewModel.UndoCommand.CanExecuteChanged += (_, _) => undoRaised = true;
            _viewModel.RedoCommand.CanExecuteChanged += (_, _) => redoRaised = true;

            // Act
            _viewModel.ResetWindowLevelCommand.Execute(null);

            // Assert
            Assert.That(undoRaised, Is.True, "Reset must raise UndoCommand.CanExecuteChanged.");
            Assert.That(redoRaised, Is.True, "Reset must raise RedoCommand.CanExecuteChanged.");
        }

        [Test]
        public void Execute_EnablesUndoCommand_WithRealUndoService()
        {
            // Arrange — with the real undo service, a reset pushes an undoable action, so the
            // Undo command must become executable (the bound button becomes enabled).
            var model = new DicomImageModel();
            model.LoadSeries(1500.0, -600.0);
            var notifier = new StatusBarNotifier();
            var viewModel = new ImageDisplayViewModel(
                model, notifier, new WindowLevelUndoService(), Substitute.For<IWindowLevelDefaults>())
            {
                WindowWidth = 800.0,
                WindowCenter = -200.0,
            };
            Assert.That(viewModel.UndoCommand.CanExecute(null), Is.False, "Undo disabled before any reset.");

            // Act
            viewModel.ResetWindowLevelCommand.Execute(null);

            // Assert
            Assert.That(viewModel.UndoCommand.CanExecute(null), Is.True, "Undo enabled after a reset.");
            notifier.Dispose();
        }

        [Test]
        public void WindowWidth_Setter_RaisesPropertyChangedEvent()
        {
            // Arrange
            string? raisedProperty = null;
            _viewModel.PropertyChanged += (_, e) => raisedProperty = e.PropertyName;

            // Act
            _viewModel.WindowWidth = 800.0;

            // Assert
            Assert.That(raisedProperty, Is.EqualTo(nameof(ImageDisplayViewModel.WindowWidth)));
        }

        [Test]
        public void WindowCenter_Setter_RaisesPropertyChangedEvent()
        {
            // Arrange
            string? raisedProperty = null;
            _viewModel.PropertyChanged += (_, e) => raisedProperty = e.PropertyName;

            // Act
            _viewModel.WindowCenter = -200.0;

            // Assert
            Assert.That(raisedProperty, Is.EqualTo(nameof(ImageDisplayViewModel.WindowCenter)));
        }

        [Test]
        public void WindowWidth_SetThenGet_ReturnsExpectedValue()
        {
            // Arrange
            double expectedWidth = 1200.0;

            // Act
            _viewModel.WindowWidth = expectedWidth;

            // Assert
            Assert.That(_viewModel.WindowWidth, Is.EqualTo(expectedWidth));
        }

        [Test]
        public void WindowCenter_SetThenGet_ReturnsExpectedValue()
        {
            // Arrange
            double expectedCenter = -300.0;

            // Act
            _viewModel.WindowCenter = expectedCenter;

            // Assert
            Assert.That(_viewModel.WindowCenter, Is.EqualTo(expectedCenter));
        }

        // ── T04: Undo/Redo command tests ─────────────────────────────────────

        [Test]
        public void UndoCommand_CanExecute_ReturnsFalse_Initially()
        {
            // Assert — no reset has been performed; undo stack is empty
            Assert.That(_viewModel.UndoCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void RedoCommand_CanExecute_ReturnsFalse_Initially()
        {
            // Assert — redo stack is empty before any undo
            Assert.That(_viewModel.RedoCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void UndoCommand_CanExecute_ReturnsTrue_AfterReset()
        {
            // Arrange — use a real undo service so Push actually works
            using var statusNotifier = new StatusBarNotifier();
            var dicomModel = new DicomImageModel();
            dicomModel.LoadSeries(1500.0, -600.0);
            var undoService = new WindowLevelUndoService();
            var vm = new ImageDisplayViewModel(
                dicomModel, statusNotifier, undoService, _windowLevelDefaults);

            // Act
            vm.ResetWindowLevelCommand.Execute(null);

            // Assert
            Assert.That(vm.UndoCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void UndoCommand_Execute_RestoresWindowWidthAndCenter()
        {
            // Arrange — use a real undo service
            using var statusNotifier = new StatusBarNotifier();
            var dicomModel = new DicomImageModel();
            dicomModel.LoadSeries(1500.0, -600.0);
            var undoService = new WindowLevelUndoService();
            var vm = new ImageDisplayViewModel(
                dicomModel, statusNotifier, undoService, _windowLevelDefaults);
            vm.WindowWidth = 800.0;
            vm.WindowCenter = -200.0;
            vm.ResetWindowLevelCommand.Execute(null);

            // Act
            vm.UndoCommand.Execute(null);

            // Assert
            Assert.That(vm.WindowWidth, Is.EqualTo(800.0));
            Assert.That(vm.WindowCenter, Is.EqualTo(-200.0));
        }
    }
}

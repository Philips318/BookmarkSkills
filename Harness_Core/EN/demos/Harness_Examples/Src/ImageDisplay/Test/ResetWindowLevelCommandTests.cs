// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using NSubstitute;
using Philips.CT.Host.ImageDisplay.Commands;
using Philips.CT.Host.ImageDisplay.Services;

namespace Philips.CT.Host.ImageDisplay.Test
{
    /// <summary>
    /// Unit tests for <see cref="ResetWindowLevelCommand"/>.
    /// Verifies <see cref="ResetWindowLevelCommand.CanExecute"/>,
    /// <see cref="ResetWindowLevelCommand.Execute"/> happy path, and fallback behaviour.
    /// </summary>
    [TestFixture]
    [Category("Unit")]
    public class ResetWindowLevelCommandTests
    {
        private IImageDisplayViewModel _viewModel = null!;
        private IDicomImageModel _dicomModel = null!;
        private StatusBarNotifier _statusNotifier = null!;
        private IUndoService _undoService = null!;
        private IWindowLevelDefaults _windowLevelDefaults = null!;
        private ResetWindowLevelCommand _command = null!;

        [SetUp]
        public void SetUp()
        {
            _viewModel = Substitute.For<IImageDisplayViewModel>();
            _dicomModel = Substitute.For<IDicomImageModel>();
            _statusNotifier = new StatusBarNotifier();
            _undoService = Substitute.For<IUndoService>();
            _windowLevelDefaults = Substitute.For<IWindowLevelDefaults>();

            _command = new ResetWindowLevelCommand(
                _viewModel, _dicomModel, _statusNotifier, _undoService, _windowLevelDefaults);
        }

        [TearDown]
        public void TearDown() => _statusNotifier.Dispose();

        [Test]
        public void CanExecute_ReturnsFalse_WhenIsSeriesLoadedFalse()
        {
            // Arrange
            _viewModel.IsSeriesLoaded.Returns(false);

            // Act / Assert
            Assert.That(_command.CanExecute(null), Is.False);
        }

        [Test]
        public void CanExecute_ReturnsTrue_WhenIsSeriesLoadedTrue()
        {
            // Arrange
            _viewModel.IsSeriesLoaded.Returns(true);

            // Act / Assert
            Assert.That(_command.CanExecute(null), Is.True);
        }

        [Test]
        public void Execute_HappyPath_SetsViewModelWindowWidth()
        {
            // Arrange
            _dicomModel.DefaultWindowLevel.Returns(WindowLevel.Create(1500.0, -600.0));
            double capturedWidth = 0;
            _viewModel.When(v => v.WindowWidth = Arg.Any<double>())
                      .Do(ci => capturedWidth = ci.ArgAt<double>(0));

            // Act
            _command.Execute(null);

            // Assert
            Assert.That(capturedWidth, Is.EqualTo(1500.0));
        }

        [Test]
        public void Execute_HappyPath_SetsViewModelWindowCenter()
        {
            // Arrange
            _dicomModel.DefaultWindowLevel.Returns(WindowLevel.Create(1500.0, -600.0));
            double capturedCenter = 0;
            _viewModel.When(v => v.WindowCenter = Arg.Any<double>())
                      .Do(ci => capturedCenter = ci.ArgAt<double>(0));

            // Act
            _command.Execute(null);

            // Assert
            Assert.That(capturedCenter, Is.EqualTo(-600.0));
        }

        [Test]
        public void Execute_HappyPath_ShowsNonEmptyStatusMessage()
        {
            // Arrange
            _dicomModel.DefaultWindowLevel.Returns(WindowLevel.Create(1500.0, -600.0));

            // Act
            _command.Execute(null);

            // Assert
            Assert.That(_statusNotifier.CurrentMessage, Is.Not.Empty);
        }

        [Test]
        public void Execute_HappyPath_StatusMessageContainsWindowWidth()
        {
            // Arrange
            _dicomModel.DefaultWindowLevel.Returns(WindowLevel.Create(1500.0, -600.0));

            // Act
            _command.Execute(null);

            // Assert — the message must reference the WW value
            Assert.That(_statusNotifier.CurrentMessage, Does.Contain("1500"));
        }

        [Test]
        public void Execute_WithPresetName_StatusMessageContainsPresetName()
        {
            // Arrange
            _dicomModel.DefaultWindowLevel.Returns(WindowLevel.Create(1500.0, -600.0, "Lung"));

            // Act
            _command.Execute(null);

            // Assert
            Assert.That(_statusNotifier.CurrentMessage, Does.Contain("Lung"));
        }

        [Test]
        public void Execute_WhenDicomDefaultIsNull_UsesFallbackWindowLevel()
        {
            // Arrange
            _dicomModel.DefaultWindowLevel.Returns((WindowLevel?)null);
            var fallback = WindowLevel.Create(400.0, 40.0);
            _windowLevelDefaults.FallbackWindowLevel.Returns(fallback);
            double capturedWidth = 0;
            _viewModel.When(v => v.WindowWidth = Arg.Any<double>())
                      .Do(ci => capturedWidth = ci.ArgAt<double>(0));

            // Act
            _command.Execute(null);

            // Assert
            Assert.That(capturedWidth, Is.EqualTo(400.0));
        }

        [Test]
        public void Execute_ShowsMessageWithDurationOfTwoSeconds()
        {
            // Arrange
            _dicomModel.DefaultWindowLevel.Returns(WindowLevel.Create(1500.0, -600.0));

            // Act
            _command.Execute(null);

            // Assert — per FR-05 the minimum visible duration is 2 seconds
            Assert.That(_statusNotifier.LastDisplayDuration, Is.EqualTo(TimeSpan.FromSeconds(2)));
        }

        [Test]
        public void Execute_AbsentDicomTags_LastResetUsedFallbackIsTrue()
        {
            // Arrange — null DefaultWindowLevel + no windowing tags
            _dicomModel.DefaultWindowLevel.Returns((WindowLevel?)null);
            _dicomModel.HasDicomWindowingTags.Returns(false);
            _windowLevelDefaults.FallbackWindowLevel.Returns(WindowLevel.Create(400.0, 40.0));

            // Act
            _command.Execute(null);

            // Assert
            Assert.That(_command.LastResetUsedFallback, Is.True);
        }

        [Test]
        public void Execute_AbsentDicomTags_AppliesFallbackWindowWidth()
        {
            // Arrange
            _dicomModel.DefaultWindowLevel.Returns((WindowLevel?)null);
            _dicomModel.HasDicomWindowingTags.Returns(false);
            _windowLevelDefaults.FallbackWindowLevel.Returns(WindowLevel.Create(400.0, 40.0));
            double capturedWidth = 0;
            _viewModel.When(v => v.WindowWidth = Arg.Any<double>())
                      .Do(ci => capturedWidth = ci.ArgAt<double>(0));

            // Act
            _command.Execute(null);

            // Assert
            Assert.That(capturedWidth, Is.EqualTo(400.0));
        }

        [Test]
        public void Execute_ZeroDicomWw_AppliesFallbackWindowWidth()
        {
            // Arrange — DefaultWindowLevel is null because WW=0 is stored; HasDicomWindowingTags=true
            _dicomModel.DefaultWindowLevel.Returns((WindowLevel?)null);
            _dicomModel.HasDicomWindowingTags.Returns(true);
            _windowLevelDefaults.FallbackWindowLevel.Returns(WindowLevel.Create(400.0, 40.0));
            double capturedWidth = 0;
            _viewModel.When(v => v.WindowWidth = Arg.Any<double>())
                      .Do(ci => capturedWidth = ci.ArgAt<double>(0));

            // Act
            _command.Execute(null);

            // Assert
            Assert.That(capturedWidth, Is.EqualTo(400.0));
        }

        [Test]
        public void Execute_NegativeDicomWw_AppliesFallbackWindowWidth()
        {
            // Arrange — DefaultWindowLevel is null because WW=-100; HasDicomWindowingTags=true
            _dicomModel.DefaultWindowLevel.Returns((WindowLevel?)null);
            _dicomModel.HasDicomWindowingTags.Returns(true);
            _windowLevelDefaults.FallbackWindowLevel.Returns(WindowLevel.Create(400.0, 40.0));
            double capturedWidth = 0;
            _viewModel.When(v => v.WindowWidth = Arg.Any<double>())
                      .Do(ci => capturedWidth = ci.ArgAt<double>(0));

            // Act
            _command.Execute(null);

            // Assert
            Assert.That(capturedWidth, Is.EqualTo(400.0));
        }

        [Test]
        public void Execute_InvalidDicomWw_LastResetUsedFallbackIsTrue()
        {
            // Arrange
            _dicomModel.DefaultWindowLevel.Returns((WindowLevel?)null);
            _dicomModel.HasDicomWindowingTags.Returns(true);
            _windowLevelDefaults.FallbackWindowLevel.Returns(WindowLevel.Create(400.0, 40.0));

            // Act
            _command.Execute(null);

            // Assert
            Assert.That(_command.LastResetUsedFallback, Is.True);
        }

        [Test]
        public void Execute_AbsentDicomTags_StatusMessageContainsDicomDefaultsUnavailable()
        {
            // Arrange
            _dicomModel.DefaultWindowLevel.Returns((WindowLevel?)null);
            _dicomModel.HasDicomWindowingTags.Returns(false);
            _windowLevelDefaults.FallbackWindowLevel.Returns(WindowLevel.Create(400.0, 40.0));

            // Act
            _command.Execute(null);

            // Assert
            Assert.That(_statusNotifier.CurrentMessage, Does.Contain("DICOM defaults unavailable").IgnoreCase);
        }

        [Test]
        public void Execute_InvalidDicomWw_StatusMessageContainsInvalidDicomValue()
        {
            // Arrange
            _dicomModel.DefaultWindowLevel.Returns((WindowLevel?)null);
            _dicomModel.HasDicomWindowingTags.Returns(true);
            _windowLevelDefaults.FallbackWindowLevel.Returns(WindowLevel.Create(400.0, 40.0));

            // Act
            _command.Execute(null);

            // Assert
            Assert.That(_statusNotifier.CurrentMessage, Does.Contain("invalid DICOM value").IgnoreCase);
        }
    }
}

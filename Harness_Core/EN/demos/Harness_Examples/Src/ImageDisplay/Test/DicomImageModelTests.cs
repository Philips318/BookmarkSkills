// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using NSubstitute;
using Philips.CT.Host.ImageDisplay.Models;

namespace Philips.CT.Host.ImageDisplay.Test
{
    /// <summary>
    /// Unit tests for <see cref="DicomImageModel"/>.
    /// Verifies series loading, <see cref="IDicomImageModel.DefaultWindowLevel"/>, and
    /// <see cref="IDicomImageModel.HasValidDefaultWindowLevel"/> behaviour.
    /// </summary>
    [TestFixture]
    [Category("Unit")]
    public class DicomImageModelTests
    {
        private DicomImageModel _model = null!;

        [SetUp]
        public void SetUp()
        {
            _model = new DicomImageModel();
        }

        [Test]
        public void DefaultWindowLevel_ReturnsNull_WhenSeriesNotLoaded()
        {
            // Assert
            Assert.That(_model.DefaultWindowLevel, Is.Null);
        }

        [Test]
        public void HasValidDefaultWindowLevel_ReturnsFalse_WhenSeriesNotLoaded()
        {
            // Assert
            Assert.That(_model.HasValidDefaultWindowLevel, Is.False);
        }

        [Test]
        public void LoadSeries_SetsDefaultWindowLevel()
        {
            // Act
            _model.LoadSeries(1500.0, -600.0);

            // Assert
            Assert.That(_model.DefaultWindowLevel, Is.Not.Null);
        }

        [Test]
        public void LoadSeries_SetsCorrectWindowWidth()
        {
            // Arrange
            double expectedWw = 1500.0;

            // Act
            _model.LoadSeries(expectedWw, -600.0);

            // Assert
            Assert.That(_model.DefaultWindowLevel!.Width, Is.EqualTo(expectedWw));
        }

        [Test]
        public void LoadSeries_SetsCorrectWindowCenter()
        {
            // Arrange
            double expectedWc = -600.0;

            // Act
            _model.LoadSeries(1500.0, expectedWc);

            // Assert
            Assert.That(_model.DefaultWindowLevel!.Center, Is.EqualTo(expectedWc));
        }

        [Test]
        public void HasValidDefaultWindowLevel_ReturnsTrue_AfterLoadSeries()
        {
            // Act
            _model.LoadSeries(1500.0, -600.0);

            // Assert
            Assert.That(_model.HasValidDefaultWindowLevel, Is.True);
        }

        [Test]
        public void LoadSeries_WithPresetName_StoresPresetName()
        {
            // Arrange
            string expectedPreset = "Lung";

            // Act
            _model.LoadSeries(1500.0, -600.0, expectedPreset);

            // Assert
            Assert.That(_model.DefaultWindowLevel!.PresetName, Is.EqualTo(expectedPreset));
        }

        [Test]
        public void LoadSeries_WithoutPresetName_PresetNameIsNull()
        {
            // Act
            _model.LoadSeries(1500.0, -600.0);

            // Assert
            Assert.That(_model.DefaultWindowLevel!.PresetName, Is.Null);
        }

        [Test]
        public void LoadSeries_SecondLoad_OverridesFirstLoad()
        {
            // Arrange
            _model.LoadSeries(400.0, 40.0);

            // Act
            _model.LoadSeries(1500.0, -600.0, "Lung");

            // Assert
            Assert.That(_model.DefaultWindowLevel!.Width, Is.EqualTo(1500.0));
            Assert.That(_model.DefaultWindowLevel.Center, Is.EqualTo(-600.0));
            Assert.That(_model.DefaultWindowLevel.PresetName, Is.EqualTo("Lung"));
        }

        [Test]
        public void LoadSeries_ZeroWindowWidth_ThrowsArgumentOutOfRangeException()
        {
            // Act / Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _model.LoadSeries(0.0, 40.0));
        }
    }
}

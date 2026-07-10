// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using Philips.CT.Host.ImageDisplay;

namespace Philips.CT.Host.ImageDisplay.Test
{
    /// <summary>
    /// Unit tests for <see cref="WindowLevel"/> value object.
    /// Verifies the WW &gt; 0 invariant, property storage, and preset name preservation.
    /// </summary>
    [TestFixture]
    [Category("Unit")]
    public class WindowLevelTests
    {
        [Test]
        public void Create_ValidWidthAndCenter_ReturnsWindowLevelWithCorrectValues()
        {
            // Arrange
            double expectedWidth = 400.0;
            double expectedCenter = 40.0;

            // Act
            WindowLevel result = WindowLevel.Create(expectedWidth, expectedCenter);

            // Assert
            Assert.That(result.Width, Is.EqualTo(expectedWidth));
            Assert.That(result.Center, Is.EqualTo(expectedCenter));
        }

        [Test]
        public void Create_ZeroWidth_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            double zeroWidth = 0.0;
            double center = 40.0;

            // Act / Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => WindowLevel.Create(zeroWidth, center));
        }

        [Test]
        public void Create_NegativeWidth_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            double negativeWidth = -100.0;
            double center = 40.0;

            // Act / Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => WindowLevel.Create(negativeWidth, center));
        }

        [Test]
        public void Create_WithPresetName_PreservesPresetName()
        {
            // Arrange
            double width = 400.0;
            double center = 40.0;
            string expectedName = "Soft Tissue";

            // Act
            WindowLevel result = WindowLevel.Create(width, center, expectedName);

            // Assert
            Assert.That(result.PresetName, Is.EqualTo(expectedName));
        }

        [Test]
        public void Create_WithoutPresetName_PresetNameIsNull()
        {
            // Arrange / Act
            WindowLevel result = WindowLevel.Create(400.0, 40.0);

            // Assert
            Assert.That(result.PresetName, Is.Null);
        }

        [Test]
        public void Create_SmallestPositiveWidth_DoesNotThrow()
        {
            // Arrange
            double smallestPositive = double.Epsilon;

            // Act / Assert
            Assert.DoesNotThrow(() => WindowLevel.Create(smallestPositive, 0.0));
        }

        [TestCase(-1.0)]
        [TestCase(-0.001)]
        [TestCase(double.NegativeInfinity)]
        public void Create_WidthBelowOrAtZero_ThrowsArgumentOutOfRangeException(double invalidWidth)
        {
            // Act / Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => WindowLevel.Create(invalidWidth, 0.0));
        }

        [Test]
        public void Create_NegativeCenter_Succeeds()
        {
            // Arrange
            double negativeCenter = -200.0;

            // Act
            WindowLevel result = WindowLevel.Create(400.0, negativeCenter);

            // Assert
            Assert.That(result.Center, Is.EqualTo(negativeCenter));
        }

        [Test]
        public void Create_ExceptionMessage_ContainsParameterName()
        {
            // Act
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => WindowLevel.Create(0.0, 40.0));

            // Assert
            Assert.That(ex!.ParamName, Is.EqualTo("ww"));
        }
    }
}

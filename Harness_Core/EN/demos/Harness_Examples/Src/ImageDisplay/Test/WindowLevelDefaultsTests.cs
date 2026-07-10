// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using Philips.CT.Host.ImageDisplay.Services;

namespace Philips.CT.Host.ImageDisplay.Test
{
    /// <summary>
    /// Unit tests for <see cref="WindowLevelDefaults"/>.
    /// Verifies default WW/WC values, configurable constructor, and invalid-WW rejection.
    /// </summary>
    [TestFixture]
    [Category("Unit")]
    public class WindowLevelDefaultsTests
    {
        [Test]
        public void FallbackWindowLevel_DefaultConstructor_ReturnsWw400()
        {
            // Arrange
            var defaults = new WindowLevelDefaults();

            // Act
            var result = defaults.FallbackWindowLevel;

            // Assert
            Assert.That(result.Width, Is.EqualTo(400.0));
        }

        [Test]
        public void FallbackWindowLevel_DefaultConstructor_ReturnsWc40()
        {
            // Arrange
            var defaults = new WindowLevelDefaults();

            // Act
            var result = defaults.FallbackWindowLevel;

            // Assert
            Assert.That(result.Center, Is.EqualTo(40.0));
        }

        [Test]
        public void FallbackWindowLevel_ConfigurableConstructor_ReturnsConfiguredWw()
        {
            // Arrange
            double expectedWw = 1500.0;
            var defaults = new WindowLevelDefaults(fallbackWw: expectedWw, fallbackWc: -600.0);

            // Act
            var result = defaults.FallbackWindowLevel;

            // Assert
            Assert.That(result.Width, Is.EqualTo(expectedWw));
        }

        [Test]
        public void FallbackWindowLevel_ConfigurableConstructor_ReturnsConfiguredWc()
        {
            // Arrange
            double expectedWc = -600.0;
            var defaults = new WindowLevelDefaults(fallbackWw: 1500.0, fallbackWc: expectedWc);

            // Act
            var result = defaults.FallbackWindowLevel;

            // Assert
            Assert.That(result.Center, Is.EqualTo(expectedWc));
        }

        [Test]
        public void Constructor_ZeroFallbackWw_ThrowsArgumentOutOfRangeException()
        {
            // Act / Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new WindowLevelDefaults(fallbackWw: 0.0));
        }

        [Test]
        public void Constructor_NegativeFallbackWw_ThrowsArgumentOutOfRangeException()
        {
            // Act / Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new WindowLevelDefaults(fallbackWw: -50.0));
        }
    }
}

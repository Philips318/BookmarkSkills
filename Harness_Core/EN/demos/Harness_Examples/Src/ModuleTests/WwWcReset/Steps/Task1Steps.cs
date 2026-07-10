// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using Philips.CT.Host.ImageDisplay;
using Philips.CT.Host.ImageDisplay.Constants;
using Reqnroll;

namespace Philips.CT.Host.ImageDisplay.ModuleTests.Steps
{
    /// <summary>
    /// Reqnroll step definitions for the T01 feature:
    /// "Domain model and interface contracts for WW/WC Reset".
    /// Covers <see cref="WindowLevel"/> creation invariants and
    /// <see cref="DicomDisplayTags"/> constant verification.
    /// </summary>
    [Binding]
    [Category("Module")]
    public sealed class Task1Steps
    {
        private double _windowWidth;
        private double _windowCenter;
        private string? _presetName;
        private WindowLevel? _createdWindowLevel;
        private Exception? _thrownException;

        // ── Arrange steps ──────────────────────────────────────────────────────

        [Given(@"a Window Width of (.*) and a Window Center of (.*)")]
        public void GivenAWindowWidthAndCenter(double width, double center)
        {
            _windowWidth = width;
            _windowCenter = center;
        }

        [Given(@"a Window Width of (.*) and a Window Center of (.*) with preset name ""(.*)""")]
        public void GivenAWindowWidthCenterAndPresetName(double width, double center, string presetName)
        {
            _windowWidth = width;
            _windowCenter = center;
            _presetName = presetName;
        }

        [Given(@"the DICOM display tag constants")]
        public void GivenTheDicomDisplayTagConstants()
        {
            // No setup required — the constants are static and always available.
        }

        // ── Act steps ──────────────────────────────────────────────────────────

        [When(@"a WindowLevel is created with those values")]
        public void WhenAWindowLevelIsCreatedWithThoseValues()
        {
            try
            {
                _createdWindowLevel = WindowLevel.Create(_windowWidth, _windowCenter);
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }
        }

        [When(@"a WindowLevel is created with those values and preset name")]
        public void WhenAWindowLevelIsCreatedWithThoseValuesAndPresetName()
        {
            try
            {
                _createdWindowLevel = WindowLevel.Create(_windowWidth, _windowCenter, _presetName);
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }
        }

        // ── Assert steps ───────────────────────────────────────────────────────

        [Then(@"the WindowLevel has Width (.*) and Center (.*)")]
        public void ThenTheWindowLevelHasWidthAndCenter(double expectedWidth, double expectedCenter)
        {
            Assert.That(_createdWindowLevel, Is.Not.Null);
            Assert.That(_createdWindowLevel!.Width, Is.EqualTo(expectedWidth));
            Assert.That(_createdWindowLevel.Center, Is.EqualTo(expectedCenter));
        }

        [Then(@"the creation fails with a validation error")]
        public void ThenTheCreationFailsWithAValidationError()
        {
            Assert.That(_thrownException, Is.Not.Null);
            Assert.That(_thrownException, Is.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Then(@"the WindowLevel preset name is ""(.*)""")]
        public void ThenTheWindowLevelPresetNameIs(string expectedPresetName)
        {
            Assert.That(_createdWindowLevel, Is.Not.Null);
            Assert.That(_createdWindowLevel!.PresetName, Is.EqualTo(expectedPresetName));
        }

        [Then(@"the WindowCenter constant maps to DICOM tag 0028,1050")]
        public void ThenTheWindowCenterConstantMapsToDicomTag00281050()
        {
            Assert.That(DicomDisplayTags.WindowCenter, Is.EqualTo("0028,1050"));
        }

        [Then(@"the WindowWidth constant maps to DICOM tag 0028,1051")]
        public void ThenTheWindowWidthConstantMapsToDicomTag00281051()
        {
            Assert.That(DicomDisplayTags.WindowWidth, Is.EqualTo("0028,1051"));
        }

        [Then(@"the WindowCenterWidthExplanation constant maps to DICOM tag 0028,1055")]
        public void ThenTheWindowCenterWidthExplanationConstantMapsToDicomTag00281055()
        {
            Assert.That(DicomDisplayTags.WindowCenterWidthExplanation, Is.EqualTo("0028,1055"));
        }
    }
}

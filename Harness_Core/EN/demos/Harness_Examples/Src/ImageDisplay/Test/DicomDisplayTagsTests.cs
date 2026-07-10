// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using Philips.CT.Host.ImageDisplay.Constants;

namespace Philips.CT.Host.ImageDisplay.Test
{
    /// <summary>
    /// Unit tests for <see cref="DicomDisplayTags"/> constants.
    /// Verifies that each constant holds the correct DICOM tag string as defined in
    /// DICOM PS3.3 §C.7.6.3.1.5, Table C.7-24.
    /// </summary>
    [TestFixture]
    [Category("Unit")]
    public class DicomDisplayTagsTests
    {
        [Test]
        public void WindowCenter_MapsToCorrectDicomTag()
        {
            // Assert
            Assert.That(DicomDisplayTags.WindowCenter, Is.EqualTo("0028,1050"));
        }

        [Test]
        public void WindowWidth_MapsToCorrectDicomTag()
        {
            // Assert
            Assert.That(DicomDisplayTags.WindowWidth, Is.EqualTo("0028,1051"));
        }

        [Test]
        public void WindowCenterWidthExplanation_MapsToCorrectDicomTag()
        {
            // Assert
            Assert.That(DicomDisplayTags.WindowCenterWidthExplanation, Is.EqualTo("0028,1055"));
        }

        [Test]
        public void AllTagConstants_AreNonNullAndNonEmpty()
        {
            // Assert
            Assert.That(DicomDisplayTags.WindowCenter, Is.Not.Null.And.Not.Empty);
            Assert.That(DicomDisplayTags.WindowWidth, Is.Not.Null.And.Not.Empty);
            Assert.That(DicomDisplayTags.WindowCenterWidthExplanation, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void AllTagConstants_AreDistinct()
        {
            // Assert — each tag constant must have a unique value
            var tags = new[]
            {
                DicomDisplayTags.WindowCenter,
                DicomDisplayTags.WindowWidth,
                DicomDisplayTags.WindowCenterWidthExplanation
            };

            Assert.That(tags, Is.Unique);
        }
    }
}

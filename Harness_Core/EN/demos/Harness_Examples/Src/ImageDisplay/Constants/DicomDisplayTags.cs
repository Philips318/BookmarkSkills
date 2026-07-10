// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

namespace Philips.CT.Host.ImageDisplay.Constants
{
    /// <summary>
    /// Named constants for DICOM Image Presentation attributes used by the Image Display module.
    /// All tag identifiers are taken from DICOM PS3.3 §C.7.6.3.1.5 — VOI LUT Module.
    /// </summary>
    /// <remarks>
    /// No magic-number DICOM tag literals are permitted elsewhere in the codebase.
    /// All DICOM tag access must reference these constants.
    /// Reference: DICOM PS3.3 2024a §C.7.6.3.1.5, Table C.7-24.
    /// </remarks>
    public static class DicomDisplayTags
    {
        /// <summary>
        /// DICOM tag (0028,1050) — Window Center.
        /// Defines the value mapped to the center of the display range.
        /// Multi-valued; use the first value (index 0) per FR-03.
        /// Reference: DICOM PS3.3 §C.7.6.3.1.5, Table C.7-24.
        /// </summary>
        public static readonly string WindowCenter = "0028,1050";

        /// <summary>
        /// DICOM tag (0028,1051) — Window Width.
        /// Defines the range of input values mapped to the full display range.
        /// Must be strictly greater than zero; enforced by <see cref="Philips.CT.Host.ImageDisplay.WindowLevel.Create"/>.
        /// Multi-valued; use the first value (index 0) per FR-03.
        /// Reference: DICOM PS3.3 §C.7.6.3.1.5, Table C.7-24.
        /// </summary>
        public static readonly string WindowWidth = "0028,1051";

        /// <summary>
        /// DICOM tag (0028,1055) — Window Center &amp; Width Explanation.
        /// Optional free-text explanation of the window center and width pair
        /// (e.g. "Soft Tissue", "Lung", "Bone").
        /// Stored as the <see cref="Philips.CT.Host.ImageDisplay.WindowLevel.PresetName"/> when present.
        /// Reference: DICOM PS3.3 §C.7.6.3.1.5, Table C.7-24.
        /// </summary>
        public static readonly string WindowCenterWidthExplanation = "0028,1055";
    }
}

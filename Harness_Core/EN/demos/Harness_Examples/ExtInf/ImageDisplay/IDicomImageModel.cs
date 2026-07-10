// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

namespace Philips.CT.Host.ImageDisplay
{
    /// <summary>
    /// Contract for the DICOM image model.
    /// Exposes the DICOM-default Window Width and Window Center for the currently loaded series.
    /// </summary>
    public interface IDicomImageModel
    {
        /// <summary>
        /// Gets the default <see cref="WindowLevel"/> parsed from DICOM tags (0028,1050) and
        /// (0028,1051) when the series was loaded.
        /// Returns <see langword="null"/> when no series is loaded, the DICOM tags are absent,
        /// or the stored WW value is not strictly greater than zero.
        /// </summary>
        WindowLevel? DefaultWindowLevel { get; }

        /// <summary>
        /// Gets a value indicating whether the model holds a valid DICOM default Window Level
        /// (i.e. <see cref="DefaultWindowLevel"/> is non-<see langword="null"/> and WW &gt; 0).
        /// </summary>
        bool HasValidDefaultWindowLevel { get; }

        /// <summary>
        /// Gets a value indicating whether a CT series is currently loaded in the viewport,
        /// independent of whether that series carries valid DICOM windowing tags.
        /// A series loaded without windowing tags, or with an invalid WW, is still
        /// <em>loaded</em> — the reset action must remain available so that fallback
        /// defaults can be applied (FR-01, FR-04, FR-07).
        /// Returns <see langword="false"/> only when no series has been loaded.
        /// </summary>
        bool IsSeriesLoaded { get; }

        /// <summary>
        /// Gets a value indicating whether the series was loaded with windowing tag data
        /// (DICOM tags 0028,1050 / 0028,1051), regardless of whether the stored values are valid.
        /// Returns <see langword="false"/> when no series is loaded or the tags were absent.
        /// Used by callers to distinguish &quot;tags absent&quot; from &quot;tags present but invalid WW&quot;.
        /// </summary>
        bool HasDicomWindowingTags { get; }
    }
}

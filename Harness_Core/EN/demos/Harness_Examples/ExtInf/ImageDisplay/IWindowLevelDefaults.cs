// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

namespace Philips.CT.Host.ImageDisplay
{
    /// <summary>
    /// Contract for the fallback Window Level configuration service.
    /// Provides a configurable default <see cref="WindowLevel"/> when the DICOM series does not
    /// carry valid WW/WC tags (FR-04).
    /// </summary>
    public interface IWindowLevelDefaults
    {
        /// <summary>
        /// Gets the fallback <see cref="WindowLevel"/> to apply when the DICOM image model does
        /// not contain valid default values.
        /// The returned value is guaranteed to have WW &gt; 0.
        /// Typical defaults: WW = 400, WC = 40.
        /// </summary>
        WindowLevel FallbackWindowLevel { get; }
    }
}

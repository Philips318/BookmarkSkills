// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

namespace Philips.CT.Host.ImageDisplay.Services
{
    /// <summary>
    /// Provides the configurable fallback Window Level applied when a DICOM series does not
    /// carry valid WW/WC tags.  Implements <see cref="IWindowLevelDefaults"/>.
    /// </summary>
    /// <remarks>
    /// The fallback values are injected at construction time (defaults: WW = 400, WC = 40).
    /// This keeps the class testable without configuration infrastructure.
    /// </remarks>
    public sealed class WindowLevelDefaults : IWindowLevelDefaults
    {
        private readonly WindowLevel _fallbackWindowLevel;

        /// <summary>
        /// Initialises a new instance of <see cref="WindowLevelDefaults"/> with configurable
        /// fallback values.
        /// </summary>
        /// <param name="fallbackWw">
        /// Fallback Window Width.  Must be strictly greater than zero.
        /// </param>
        /// <param name="fallbackWc">Fallback Window Center (any finite value is permitted).</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="fallbackWw"/> is less than or equal to zero.
        /// </exception>
        public WindowLevelDefaults(double fallbackWw = 400.0, double fallbackWc = 40.0)
        {
            _fallbackWindowLevel = WindowLevel.Create(fallbackWw, fallbackWc);
        }

        /// <inheritdoc/>
        public WindowLevel FallbackWindowLevel => _fallbackWindowLevel;
    }
}

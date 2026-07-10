// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

namespace Philips.CT.Host.ImageDisplay
{
    /// <summary>
    /// Represents a validated DICOM window level pair (Window Width and Window Center).
    /// The Window Width is always strictly greater than zero; a zero or negative value
    /// produces a black or inverted image and is therefore rejected at construction time.
    /// </summary>
    /// <remarks>
    /// All code paths that produce a <see cref="WindowLevel"/> must go through
    /// <see cref="Create(double, double, string?)"/> so the WW &gt; 0 invariant is
    /// enforced structurally and cannot be bypassed.
    /// </remarks>
    public sealed class WindowLevel
    {
        /// <summary>Gets the Window Width. Always strictly greater than zero.</summary>
        public double Width { get; }

        /// <summary>Gets the Window Center.</summary>
        public double Center { get; }

        /// <summary>
        /// Gets the optional DICOM preset name (e.g. "Soft Tissue", "Lung", "Bone").
        /// <see langword="null"/> when no preset name is associated.
        /// </summary>
        public string? PresetName { get; }

        private WindowLevel(double width, double center, string? presetName)
        {
            Width = width;
            Center = center;
            PresetName = presetName;
        }

        /// <summary>
        /// Creates a <see cref="WindowLevel"/> with the specified Window Width, Window Center,
        /// and an optional preset name.
        /// </summary>
        /// <param name="ww">
        /// Window Width. Must be strictly greater than zero.
        /// A value of zero or below produces a black or inverted image and is rejected.
        /// </param>
        /// <param name="wc">Window Center (any finite value is permitted).</param>
        /// <param name="name">
        /// Optional DICOM preset name. Pass <see langword="null"/> when no preset name is available.
        /// </param>
        /// <returns>A new, immutable <see cref="WindowLevel"/> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="ww"/> is less than or equal to zero.
        /// </exception>
        public static WindowLevel Create(double ww, double wc, string? name = null)
        {
            if (ww <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(ww),
                    ww,
                    "Window Width must be strictly greater than zero. A value of zero or below produces a black or inverted image.");
            }

            return new WindowLevel(ww, wc, name);
        }
    }
}

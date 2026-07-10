// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

namespace Philips.CT.Host.ImageDisplay.Models
{
    /// <summary>
    /// Concrete DICOM image model that stores the window-level defaults loaded from a DICOM series.
    /// Implements <see cref="IDicomImageModel"/> and exposes <see cref="LoadSeries"/>,
    /// <see cref="LoadSeriesWithoutTags"/>, and <see cref="LoadSeriesWithInvalidWW"/> for
    /// simulation and test support.
    /// </summary>
    public sealed class DicomImageModel : IDicomImageModel
    {
        private double? _rawWindowWidth;
        private double? _rawWindowCenter;
        private string? _rawPresetName;
        private bool _isSeriesLoaded;

        /// <inheritdoc/>
        public WindowLevel? DefaultWindowLevel =>
            _rawWindowWidth.HasValue && _rawWindowWidth.Value > 0
                ? WindowLevel.Create(_rawWindowWidth.Value, _rawWindowCenter ?? 0.0, _rawPresetName)
                : null;

        /// <inheritdoc/>
        public bool HasValidDefaultWindowLevel =>
            _rawWindowWidth.HasValue && _rawWindowWidth.Value > 0;

        /// <inheritdoc/>
        public bool HasDicomWindowingTags => _rawWindowWidth.HasValue;

        /// <inheritdoc/>
        public bool IsSeriesLoaded => _isSeriesLoaded;

        /// <summary>Gets the raw Window Width value stored in the model, or <see langword="null"/> when no series is loaded.</summary>
        public double? RawWindowWidth => _rawWindowWidth;

        /// <summary>Gets the raw Window Center value stored in the model, or <see langword="null"/> when no series is loaded.</summary>
        public double? RawWindowCenter => _rawWindowCenter;

        /// <summary>
        /// Loads a CT series into the model, recording the DICOM default Window Width and
        /// Window Center values parsed from tags (0028,1051) and (0028,1050).
        /// </summary>
        /// <param name="ww">
        /// Window Width from the DICOM series. Must be strictly greater than zero.
        /// </param>
        /// <param name="wc">Window Center from the DICOM series.</param>
        /// <param name="presetName">
        /// Optional preset name from DICOM tag (0028,1055). Pass <see langword="null"/> when absent.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="ww"/> is less than or equal to zero.
        /// </exception>
        public void LoadSeries(double ww, double wc, string? presetName = null)
        {
            if (ww <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(ww), ww, "Window Width must be strictly greater than zero.");
            }

            _rawWindowWidth = ww;
            _rawWindowCenter = wc;
            _rawPresetName = presetName;
            _isSeriesLoaded = true;
        }

        /// <summary>
        /// Simulates loading a CT series that carries no DICOM windowing tags (0028,1050 / 0028,1051).
        /// After this call <see cref="DefaultWindowLevel"/> returns <see langword="null"/> and
        /// <see cref="HasDicomWindowingTags"/> returns <see langword="false"/>.
        /// </summary>
        public void LoadSeriesWithoutTags()
        {
            _rawWindowWidth = null;
            _rawWindowCenter = null;
            _rawPresetName = null;
            _isSeriesLoaded = true;
        }

        /// <summary>
        /// Simulates loading a CT series whose DICOM WW tag carries an invalid (zero or negative)
        /// value.  The raw values are stored as-is; <see cref="DefaultWindowLevel"/> returns
        /// <see langword="null"/> and <see cref="HasDicomWindowingTags"/> returns <see langword="true"/>.
        /// </summary>
        /// <param name="ww">Invalid Window Width (zero or negative).</param>
        /// <param name="wc">Window Center to store alongside the invalid WW.</param>
        public void LoadSeriesWithInvalidWW(double ww, double wc = 40.0)
        {
            _rawWindowWidth = ww;
            _rawWindowCenter = wc;
            _rawPresetName = null;
            _isSeriesLoaded = true;
        }
    }
}

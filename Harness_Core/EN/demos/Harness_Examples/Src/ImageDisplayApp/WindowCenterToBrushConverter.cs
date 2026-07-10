// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Philips.CT.Host.ImageDisplay.Shell
{
    /// <summary>
    /// Converts a DICOM Window Center (HU) value into a grayscale <see cref="Brush"/> so the demo
    /// viewport visibly changes brightness when the Window/Level is reset. This is a presentation-only
    /// helper; it carries no clinical meaning beyond making the reset visible during the demonstration.
    /// </summary>
    public sealed class WindowCenterToBrushConverter : IValueConverter
    {
        private const double MinCenter = -1000.0;
        private const double MaxCenter = 3000.0;

        /// <inheritdoc/>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var center = value is double d ? d : 0.0;
            var normalized = Math.Clamp((center - MinCenter) / (MaxCenter - MinCenter), 0.0, 1.0);
            var gray = (byte)(normalized * 255.0);
            var brush = new SolidColorBrush(Color.FromRgb(gray, gray, gray));
            brush.Freeze();
            return brush;
        }

        /// <inheritdoc/>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException("WindowCenterToBrushConverter is a one-way converter.");
    }
}

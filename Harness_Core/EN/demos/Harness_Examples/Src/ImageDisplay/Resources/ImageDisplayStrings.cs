// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using System.Resources;

namespace Philips.CT.Host.ImageDisplay.Resources
{
    /// <summary>
    /// Strongly-typed accessor for the <c>ImageDisplayStrings.resx</c> resource file.
    /// All user-visible strings in the ImageDisplay module must be retrieved through this class.
    /// Falls back to compile-time constants when the embedded resource cannot be loaded
    /// (e.g. during unit tests running against a standalone assembly).
    /// </summary>
    internal static class ImageDisplayStrings
    {
        private static readonly ResourceManager ResourceManagerInstance =
            new ResourceManager(
                "Philips.CT.Host.ImageDisplay.Resources.ImageDisplayStrings",
                typeof(ImageDisplayStrings).Assembly);

        /// <summary>Gets the label text for the Reset Window/Level button.</summary>
        internal static string ResetButtonLabel =>
            GetString(nameof(ResetButtonLabel), "Reset W/L");

        /// <summary>Gets the tooltip text for the Reset Window/Level button.</summary>
        internal static string ResetButtonTooltip =>
            GetString(nameof(ResetButtonTooltip),
                "Reset Window/Level to DICOM default (Ctrl+Shift+W)");

        /// <summary>
        /// Gets the confirmation message format string (without preset name).
        /// Placeholder 0 = Window Width, placeholder 1 = Window Center.
        /// </summary>
        internal static string ResetConfirmationMessage =>
            GetString(nameof(ResetConfirmationMessage), "Window/Level reset to WW {0} / WC {1}");

        /// <summary>
        /// Gets the fallback confirmation message format string used when DICOM windowing tags are absent.
        /// Placeholder 0 = Window Width, placeholder 1 = Window Center.
        /// </summary>
        internal static string ResetFallbackMessage =>
            GetString(nameof(ResetFallbackMessage),
                "Window/Level reset to fallback defaults (WW {0} / WC {1}) \u2014 DICOM defaults unavailable");

        /// <summary>
        /// Gets the fallback confirmation message format string used when an invalid DICOM WW value is encountered.
        /// Placeholder 0 = Window Width, placeholder 1 = Window Center.
        /// </summary>
        internal static string ResetInvalidDicomMessage =>
            GetString(nameof(ResetInvalidDicomMessage),
                "Window/Level reset to fallback defaults (WW {0} / WC {1}) \u2014 invalid DICOM value encountered");

        /// <summary>
        /// Gets the confirmation message format string (with preset name).
        /// Placeholder 0 = preset name, placeholder 1 = Window Width, placeholder 2 = Window Center.
        /// </summary>
        internal static string ResetConfirmationMessageWithPreset =>
            GetString(nameof(ResetConfirmationMessageWithPreset),
                "Window/Level reset to '{0}' (WW {1} / WC {2})");

        private static string GetString(string key, string fallback)
        {
            try
            {
                return ResourceManagerInstance.GetString(key) ?? fallback;
            }
            catch (MissingManifestResourceException)
            {
                return fallback;
            }
        }
    }
}

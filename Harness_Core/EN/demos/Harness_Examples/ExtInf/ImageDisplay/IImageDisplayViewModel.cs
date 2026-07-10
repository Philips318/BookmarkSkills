// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using System.Windows.Input;

namespace Philips.CT.Host.ImageDisplay
{
    /// <summary>
    /// Contract for the Image Display ViewModel.
    /// Exposes Window Width, Window Center, the reset command, and series-loaded state.
    /// </summary>
    public interface IImageDisplayViewModel
    {
        /// <summary>Gets or sets the current Window Width displayed in the viewport.</summary>
        double WindowWidth { get; set; }

        /// <summary>Gets or sets the current Window Center displayed in the viewport.</summary>
        double WindowCenter { get; set; }

        /// <summary>
        /// Gets the command that resets Window Width and Window Center to DICOM defaults.
        /// The command is enabled only when a series is loaded.
        /// </summary>
        ICommand ResetWindowLevelCommand { get; }

        /// <summary>
        /// Gets a value indicating whether a DICOM series is currently loaded in the viewport.
        /// </summary>
        bool IsSeriesLoaded { get; }

        /// <summary>
        /// Gets the command that undoes the most recent Window/Level reset.
        /// The command is enabled only when at least one undoable action is available.
        /// </summary>
        ICommand UndoCommand { get; }

        /// <summary>
        /// Gets the command that reapplies the most recently undone Window/Level reset.
        /// The command is enabled only when at least one redoable action is available.
        /// </summary>
        ICommand RedoCommand { get; }
    }
}

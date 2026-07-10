// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using System.Windows;

namespace Philips.CT.Host.ImageDisplay.Shell
{
    /// <summary>
    /// The demo host window presenting the Window/Level controls, a viewport whose brightness
    /// reflects the current Window Center, and a status bar bound to the transient notifier.
    /// </summary>
    public partial class ImageDisplayView : Window
    {
        /// <summary>
        /// Gets the status notifier whose <see cref="DispatcherStatusNotifier.Message"/> is shown
        /// in the status bar. The same instance is injected into the ViewModel's reset command.
        /// </summary>
        public DispatcherStatusNotifier Status { get; }

        /// <summary>Initialises the view with its ViewModel and the shared status notifier.</summary>
        /// <param name="viewModel">The image-display ViewModel used as the data context.</param>
        /// <param name="status">The notifier whose message is displayed in the status bar.</param>
        public ImageDisplayView(IImageDisplayViewModel viewModel, DispatcherStatusNotifier status)
        {
            Status = status;
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

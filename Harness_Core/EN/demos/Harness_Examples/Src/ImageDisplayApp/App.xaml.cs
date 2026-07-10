// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using System.Windows;
using Philips.CT.Host.ImageDisplay.Models;
using Philips.CT.Host.ImageDisplay.Services;

namespace Philips.CT.Host.ImageDisplay.Shell
{
    /// <summary>
    /// Application entry point and composition root for the Window/Level Reset demo host.
    /// Wires up the DICOM model, status notifier, undo service, and fallback defaults, then
    /// constructs the ViewModel and shows the main window.
    /// </summary>
    /// <remarks>
    /// Supported launch arguments:
    /// <list type="bullet">
    /// <item><description><c>--simulator --seed=demo</c> — loads a CT series with DICOM defaults WW 1500 / WC -600 (preset "Lung").</description></item>
    /// <item><description><c>--no-dicom-ww</c> — loads a series without DICOM windowing tags so Reset applies the fallback (WW 400 / WC 40).</description></item>
    /// </list>
    /// </remarks>
    public partial class App : Application
    {
        private const double DemoDefaultWindowWidth = 1500.0;
        private const double DemoDefaultWindowCenter = -600.0;
        private const double DemoStartWindowWidth = 800.0;
        private const double DemoStartWindowCenter = -200.0;
        private const double FallbackWindowWidth = 400.0;
        private const double FallbackWindowCenter = 40.0;

        /// <inheritdoc/>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var model = CreateModel(e.Args);
            var notifier = new DispatcherStatusNotifier();
            var viewModel = new ImageDisplayViewModel(
                model,
                notifier,
                new WindowLevelUndoService(),
                new WindowLevelDefaults(FallbackWindowWidth, FallbackWindowCenter))
            {
                WindowWidth = DemoStartWindowWidth,
                WindowCenter = DemoStartWindowCenter,
            };

            var window = new ImageDisplayView(viewModel, notifier);
            window.Show();
        }

        private static DicomImageModel CreateModel(string[] args)
        {
            var model = new DicomImageModel();
            if (args.Contains("--no-dicom-ww"))
            {
                model.LoadSeriesWithoutTags();
            }
            else
            {
                model.LoadSeries(DemoDefaultWindowWidth, DemoDefaultWindowCenter, "Lung");
            }

            return model;
        }
    }
}

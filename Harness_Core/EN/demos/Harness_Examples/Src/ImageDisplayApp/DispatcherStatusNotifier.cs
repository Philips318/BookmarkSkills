// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using System.ComponentModel;
using System.Windows.Threading;

namespace Philips.CT.Host.ImageDisplay.Shell
{
    /// <summary>
    /// WPF status-bar notifier that displays a transient message and auto-dismisses it using a
    /// <see cref="DispatcherTimer"/> on the UI thread. Implements <see cref="INotifyPropertyChanged"/>
    /// so the status-bar text can data-bind to <see cref="Message"/> and update live.
    /// </summary>
    /// <remarks>
    /// This is the WPF-specific realisation of <see cref="IStatusNotifier"/> envisaged by ADR-001
    /// (§Decision item 4). The tested class library ships a WPF-free variant; the demo host uses this
    /// dispatcher-based one so no cross-thread marshalling is required when the timer clears the text.
    /// </remarks>
    public sealed class DispatcherStatusNotifier : IStatusNotifier, INotifyPropertyChanged
    {
        private static readonly TimeSpan MinimumDuration = TimeSpan.FromSeconds(2);
        private readonly DispatcherTimer _clearTimer = new();
        private string _message = string.Empty;

        /// <summary>Initialises a new instance of the <see cref="DispatcherStatusNotifier"/> class.</summary>
        public DispatcherStatusNotifier()
        {
            _clearTimer.Tick += OnClearTimerTick;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>Gets the message currently shown in the status bar; an empty string means none.</summary>
        public string Message
        {
            get => _message;
            private set
            {
                _message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message)));
            }
        }

        /// <inheritdoc/>
        public void ShowTransientMessage(string message, TimeSpan duration)
        {
            Message = message;
            _clearTimer.Stop();
            _clearTimer.Interval = duration < MinimumDuration ? MinimumDuration : duration;
            _clearTimer.Start();
        }

        private void OnClearTimerTick(object? sender, EventArgs e)
        {
            _clearTimer.Stop();
            Message = string.Empty;
        }
    }
}

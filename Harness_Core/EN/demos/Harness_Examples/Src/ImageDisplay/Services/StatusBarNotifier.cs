// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using System.Threading;

namespace Philips.CT.Host.ImageDisplay.Services
{
    /// <summary>
    /// Lightweight status-bar notifier that displays transient messages and auto-dismisses them
    /// using a <see cref="System.Threading.Timer"/>.
    /// Does not carry any WPF dependency, making it usable in plain class-library projects.
    /// </summary>
    public sealed class StatusBarNotifier : IStatusNotifier, IDisposable
    {
        private Timer? _clearTimer;
        private bool _disposed;

        /// <summary>
        /// Gets the message currently displayed in the status bar.
        /// An empty string indicates no active message.
        /// </summary>
        public string CurrentMessage { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the duration passed to the most recent <see cref="ShowTransientMessage"/> call.
        /// Used by tests to verify the auto-dismiss interval without waiting for the timer.
        /// </summary>
        public TimeSpan LastDisplayDuration { get; private set; }

        /// <inheritdoc/>
        public void ShowTransientMessage(string message, TimeSpan duration)
        {
            CurrentMessage = message;
            LastDisplayDuration = duration;
            _clearTimer?.Dispose();
            _clearTimer = new Timer(ClearMessage, null, duration, Timeout.InfiniteTimeSpan);
        }

        private void ClearMessage(object? state)
        {
            CurrentMessage = string.Empty;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _clearTimer?.Dispose();
            _clearTimer = null;
            _disposed = true;
        }
    }
}

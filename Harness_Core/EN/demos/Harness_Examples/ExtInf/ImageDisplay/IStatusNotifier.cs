// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

namespace Philips.CT.Host.ImageDisplay
{
    /// <summary>
    /// Contract for delivering transient status-bar messages to the user.
    /// Implementations must not overlay the diagnostic image area.
    /// </summary>
    public interface IStatusNotifier
    {
        /// <summary>
        /// Displays <paramref name="message"/> in the status bar for <paramref name="duration"/>,
        /// then automatically dismisses it.
        /// </summary>
        /// <param name="message">The message text to display. Must not be <see langword="null"/> or empty.</param>
        /// <param name="duration">
        /// The length of time to display the message. Implementations must honour a minimum of 2 seconds
        /// per FR-05 to ensure the user can read the confirmation.
        /// </param>
        void ShowTransientMessage(string message, TimeSpan duration);
    }
}

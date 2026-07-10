// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

namespace Philips.CT.Host.ImageDisplay.Models
{
    /// <summary>
    /// Undoable action that captures a Window Width / Window Center change,
    /// allowing the operator to undo or redo a Window/Level reset.
    /// </summary>
    /// <remarks>
    /// The action holds its own before/after state (per-viewport scope, OQ-02).
    /// <see cref="Execute"/> applies the new values; <see cref="Undo"/> restores the previous
    /// values.  Both use the apply-callback supplied at construction time so
    /// that no direct ViewModel reference leaks out of the Models layer.
    /// </remarks>
    public sealed class WindowLevelChangeAction : IUndoableAction
    {
        private readonly double _previousWw;
        private readonly double _previousWc;
        private readonly double _newWw;
        private readonly double _newWc;
        private readonly Action<double, double> _applyCallback;

        /// <summary>
        /// Initialises a new <see cref="WindowLevelChangeAction"/>.
        /// </summary>
        /// <param name="previousWw">Window Width before the reset.</param>
        /// <param name="previousWc">Window Center before the reset.</param>
        /// <param name="newWw">Window Width after the reset.</param>
        /// <param name="newWc">Window Center after the reset.</param>
        /// <param name="applyCallback">
        /// Callback invoked with (ww, wc) to apply values to the owning ViewModel.
        /// Must not be <see langword="null"/>.
        /// </param>
        public WindowLevelChangeAction(
            double previousWw,
            double previousWc,
            double newWw,
            double newWc,
            Action<double, double> applyCallback)
        {
            _previousWw = previousWw;
            _previousWc = previousWc;
            _newWw = newWw;
            _newWc = newWc;
            _applyCallback = applyCallback;
        }

        /// <inheritdoc/>
        public string Description => "Window/Level reset";

        /// <inheritdoc/>
        public void Execute() => _applyCallback(_newWw, _newWc);

        /// <inheritdoc/>
        public void Undo() => _applyCallback(_previousWw, _previousWc);
    }
}

// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

namespace Philips.CT.Host.ImageDisplay
{
    /// <summary>
    /// Represents a single, undoable display action.
    /// Implementations capture the state required to reverse the action in <see cref="Undo"/>.
    /// </summary>
    public interface IUndoableAction
    {
        /// <summary>
        /// Gets a human-readable description of this action (e.g. "Reset Window Level").
        /// Used for diagnostics and logging only — not surfaced in the UI.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Executes (or re-executes) this action.
        /// Called by the redo path of <see cref="IUndoService"/>.
        /// </summary>
        void Execute();

        /// <summary>
        /// Reverses the effect of this action.
        /// Called by <see cref="IUndoService.Undo"/>.
        /// </summary>
        void Undo();
    }
}

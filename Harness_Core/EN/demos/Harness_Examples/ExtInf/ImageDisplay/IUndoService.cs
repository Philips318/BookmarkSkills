// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

namespace Philips.CT.Host.ImageDisplay
{
    /// <summary>
    /// Contract for a scoped, per-viewport undo/redo stack.
    /// Each displayable action that should be undoable must implement <see cref="IUndoableAction"/>
    /// and be pushed onto this service immediately after it executes.
    /// </summary>
    public interface IUndoService
    {
        /// <summary>
        /// Gets a value indicating whether an <see cref="Undo"/> operation is currently available.
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// Gets a value indicating whether a <see cref="Redo"/> operation is currently available.
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// Pushes <paramref name="action"/> onto the undo stack.
        /// Clears the redo stack, consistent with standard undo semantics.
        /// </summary>
        /// <param name="action">The action to push. Must not be <see langword="null"/>.</param>
        void Push(IUndoableAction action);

        /// <summary>
        /// Undoes the most recently pushed action.
        /// Does nothing when <see cref="CanUndo"/> is <see langword="false"/>.
        /// </summary>
        void Undo();

        /// <summary>
        /// Redoes the most recently undone action.
        /// Does nothing when <see cref="CanRedo"/> is <see langword="false"/>.
        /// </summary>
        void Redo();
    }
}

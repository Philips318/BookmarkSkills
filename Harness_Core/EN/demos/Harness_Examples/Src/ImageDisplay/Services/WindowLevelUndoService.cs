// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

namespace Philips.CT.Host.ImageDisplay.Services
{
    /// <summary>
    /// Per-viewport undo/redo stack implementation.
    /// Maintains a bounded undo history (maximum depth <see cref="MaxDepth"/>) and a redo
    /// stack that is cleared whenever a new action is pushed.
    /// </summary>
    public sealed class WindowLevelUndoService : IUndoService
    {
        private const int MaxDepth = 20;

        private readonly LinkedList<IUndoableAction> _undoStack = new();
        private readonly Stack<IUndoableAction> _redoStack = new();

        /// <inheritdoc/>
        public bool CanUndo => _undoStack.Count > 0;

        /// <inheritdoc/>
        public bool CanRedo => _redoStack.Count > 0;

        /// <inheritdoc/>
        /// <remarks>
        /// Pushes <paramref name="action"/> onto the undo stack, clears the redo stack,
        /// and drops the oldest entry when the stack exceeds <see cref="MaxDepth"/>.
        /// </remarks>
        public void Push(IUndoableAction action)
        {
            _undoStack.AddLast(action);
            _redoStack.Clear();

            while (_undoStack.Count > MaxDepth)
            {
                _undoStack.RemoveFirst();
            }
        }

        /// <inheritdoc/>
        public void Undo()
        {
            if (!CanUndo)
            {
                return;
            }

            var action = _undoStack.Last!.Value;
            _undoStack.RemoveLast();
            action.Undo();
            _redoStack.Push(action);
        }

        /// <inheritdoc/>
        public void Redo()
        {
            if (!CanRedo)
            {
                return;
            }

            var action = _redoStack.Pop();
            action.Execute();
            _undoStack.AddLast(action);
        }
    }
}

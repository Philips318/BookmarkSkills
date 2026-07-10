// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using NSubstitute;
using Philips.CT.Host.ImageDisplay.Services;

namespace Philips.CT.Host.ImageDisplay.Test
{
    /// <summary>
    /// Unit tests for <see cref="WindowLevelUndoService"/>.
    /// Verifies Push, Undo, Redo, CanUndo, CanRedo, bounded depth, and initial state.
    /// </summary>
    [TestFixture]
    [Category("Unit")]
    public class WindowLevelUndoServiceTests
    {
        private WindowLevelUndoService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _service = new WindowLevelUndoService();
        }

        [Test]
        public void CanUndo_ReturnsFalse_Initially()
        {
            Assert.That(_service.CanUndo, Is.False);
        }

        [Test]
        public void CanRedo_ReturnsFalse_Initially()
        {
            Assert.That(_service.CanRedo, Is.False);
        }

        [Test]
        public void Push_IncrementsCanUndo()
        {
            // Act
            _service.Push(CreateAction());

            // Assert
            Assert.That(_service.CanUndo, Is.True);
        }

        [Test]
        public void Push_ClearsRedoStack()
        {
            // Arrange — populate redo stack via undo
            _service.Push(CreateAction());
            _service.Undo();
            Assert.That(_service.CanRedo, Is.True);

            // Act — new push must clear redo
            _service.Push(CreateAction());

            // Assert
            Assert.That(_service.CanRedo, Is.False);
        }

        [Test]
        public void Undo_CallsActionUndo()
        {
            // Arrange
            var action = Substitute.For<IUndoableAction>();
            _service.Push(action);

            // Act
            _service.Undo();

            // Assert
            action.Received(1).Undo();
        }

        [Test]
        public void Undo_EnablesCanRedo()
        {
            // Arrange
            _service.Push(CreateAction());

            // Act
            _service.Undo();

            // Assert
            Assert.That(_service.CanRedo, Is.True);
        }

        [Test]
        public void Undo_WhenCanUndoFalse_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _service.Undo());
        }

        [Test]
        public void Redo_CallsActionExecute()
        {
            // Arrange
            var action = Substitute.For<IUndoableAction>();
            _service.Push(action);
            _service.Undo();

            // Act
            _service.Redo();

            // Assert
            action.Received(1).Execute();
        }

        [Test]
        public void Redo_RestoresCanUndo()
        {
            // Arrange
            _service.Push(CreateAction());
            _service.Undo();
            Assert.That(_service.CanUndo, Is.False);

            // Act
            _service.Redo();

            // Assert
            Assert.That(_service.CanUndo, Is.True);
        }

        [Test]
        public void Redo_WhenCanRedoFalse_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _service.Redo());
        }

        [Test]
        public void Push_BoundedDepth_RetainsOnlyTwentyEntries()
        {
            // Arrange — push 25 actions
            for (int i = 0; i < 25; i++)
            {
                _service.Push(CreateAction());
            }

            // Exhaust undo stack and count
            int count = 0;
            while (_service.CanUndo)
            {
                _service.Undo();
                count++;
            }

            // Assert — only 20 retained
            Assert.That(count, Is.EqualTo(20));
        }

        [Test]
        public void Push_BoundedDepth_EvictsOldestNotNewest()
        {
            // Arrange — push 25 distinguishable actions (0..24). Each records its id when undone.
            var undoneOrder = new List<int>();
            for (int i = 0; i < 25; i++)
            {
                int id = i;
                _service.Push(CreateRecordingAction(id, undoneOrder));
            }

            // Act — undo all retained actions (most-recent first)
            while (_service.CanUndo)
            {
                _service.Undo();
            }

            // Assert — the 5 oldest (ids 0..4) were evicted; the 20 newest (5..24) remain.
            // Undo pops LIFO, so the retained ids come out newest-first: 24 down to 5.
            Assert.That(undoneOrder, Has.Count.EqualTo(20));
            Assert.That(undoneOrder.First(), Is.EqualTo(24), "Newest action must be undone first.");
            Assert.That(undoneOrder.Last(), Is.EqualTo(5), "Oldest retained action must be id 5 (ids 0-4 evicted).");
            Assert.That(undoneOrder, Does.Not.Contain(0), "Oldest action (id 0) must have been evicted.");
            Assert.That(undoneOrder, Does.Not.Contain(4), "id 4 must have been evicted.");
        }

        private static IUndoableAction CreateAction()
        {
            var action = Substitute.For<IUndoableAction>();
            return action;
        }

        private static IUndoableAction CreateRecordingAction(int id, List<int> undoneOrder)
        {
            var action = Substitute.For<IUndoableAction>();
            action.When(a => a.Undo()).Do(_ => undoneOrder.Add(id));
            return action;
        }
    }
}

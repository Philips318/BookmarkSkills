// Copyright (c) Koninklijke Philips N.V. 2026
// All rights are reserved. Reproduction or transmission in whole or in part,
// in any form or by any means, electronic, mechanical or otherwise, is
// prohibited without the prior written consent of the copyright owner.

using Philips.CT.Host.ImageDisplay.Models;

namespace Philips.CT.Host.ImageDisplay.Test
{
    /// <summary>
    /// Unit tests for <see cref="WindowLevelChangeAction"/>.
    /// Verifies Execute, Undo, and Description behaviour.
    /// </summary>
    [TestFixture]
    [Category("Unit")]
    public class WindowLevelChangeActionTests
    {
        private double _appliedWw;
        private double _appliedWc;
        private WindowLevelChangeAction _action = null!;

        [SetUp]
        public void SetUp()
        {
            _appliedWw = 0.0;
            _appliedWc = 0.0;
            _action = new WindowLevelChangeAction(
                previousWw: 800.0,
                previousWc: -200.0,
                newWw: 1500.0,
                newWc: -600.0,
                applyCallback: (ww, wc) => { _appliedWw = ww; _appliedWc = wc; });
        }

        [Test]
        public void Execute_AppliesNewWindowWidth()
        {
            // Act
            _action.Execute();

            // Assert
            Assert.That(_appliedWw, Is.EqualTo(1500.0));
        }

        [Test]
        public void Execute_AppliesNewWindowCenter()
        {
            // Act
            _action.Execute();

            // Assert
            Assert.That(_appliedWc, Is.EqualTo(-600.0));
        }

        [Test]
        public void Undo_AppliesPreviousWindowWidth()
        {
            // Act
            _action.Undo();

            // Assert
            Assert.That(_appliedWw, Is.EqualTo(800.0));
        }

        [Test]
        public void Undo_AppliesPreviousWindowCenter()
        {
            // Act
            _action.Undo();

            // Assert
            Assert.That(_appliedWc, Is.EqualTo(-200.0));
        }

        [Test]
        public void Description_ReturnsExpectedText()
        {
            // Assert
            Assert.That(_action.Description, Is.EqualTo("Window/Level reset"));
        }

        [Test]
        public void Execute_ThenUndo_RestoresPreviousValues()
        {
            // Act
            _action.Execute();
            _action.Undo();

            // Assert
            Assert.That(_appliedWw, Is.EqualTo(800.0));
            Assert.That(_appliedWc, Is.EqualTo(-200.0));
        }
    }
}

using CatAndMouseGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CatAndMouse.Tests
{
    [TestClass]
    public class PlayerTests
    {
        [TestMethod]
        public void Constructor_SetsInitialValues()
        {
            // Arrange
            Player player = new Player("Cat");

            // Assert
            Assert.AreEqual("Cat", player.name);
            Assert.AreEqual(-1, player.location);
            Assert.AreEqual(State.NotInGame, player.state);
            Assert.AreEqual(0, player.distanceTraveled);
        }

        [TestMethod]
        public void FirstMove_SetsStartingPositionWithoutAddingDistance()
        {
            // Arrange
            Player player = new Player("Mouse");

            // Act
            player.Move(6, 20);

            // Assert
            Assert.AreEqual(6, player.location);
            Assert.AreEqual(State.Playing, player.state);
            Assert.AreEqual(0, player.distanceTraveled);
        }

        [TestMethod]
        public void MoveForward_CrossesEndOfBoard()
        {
            // Arrange
            Player player = new Player("Mouse");
            player.Move(19, 20);

            // Act
            player.Move(4, 20);

            // Assert: 19 -> 20 -> 1 -> 2 -> 3
            Assert.AreEqual(3, player.location);
            Assert.AreEqual(4, player.distanceTraveled);
        }

        [TestMethod]
        public void MoveBackward_CrossesBeginningOfBoard()
        {
            // Arrange
            Player player = new Player("Mouse");
            player.Move(6, 20);

            // Act
            player.Move(-7, 20);

            // Assert: from cell 6, seven steps backward lands on cell 19
            Assert.AreEqual(19, player.location);
            Assert.AreEqual(7, player.distanceTraveled);
        }

        [TestMethod]
        public void MoveLongerThanBoard_WrapsCorrectly()
        {
            // Arrange
            Player player = new Player("Cat");
            player.Move(4, 10);

            // Act
            player.Move(27, 10);

            // Assert: 27 steps equal two full rounds plus seven cells
            Assert.AreEqual(1, player.location);
            Assert.AreEqual(27, player.distanceTraveled);
        }

        [TestMethod]
        public void SeveralMoves_DistanceIsSumOfAbsoluteSteps()
        {
            // Arrange
            Player player = new Player("Mouse");
            player.Move(5, 20);

            // Act
            player.Move(4, 20);
            player.Move(-6, 20);
            player.Move(2, 20);

            // Assert: 4 + 6 + 2 = 12
            Assert.AreEqual(5, player.location);
            Assert.AreEqual(12, player.distanceTraveled);
        }

        [TestMethod]
        public void ZeroMove_DoesNotChangePositionOrDistance()
        {
            // Arrange
            Player player = new Player("Cat");
            player.Move(5, 10);

            // Act
            player.Move(0, 10);

            // Assert
            Assert.AreEqual(5, player.location);
            Assert.AreEqual(0, player.distanceTraveled);
        }
    }
}

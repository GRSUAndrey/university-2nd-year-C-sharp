using System;
using System.IO;
using CatAndMouseGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CatAndMouse.Tests
{
    [TestClass]
    [DoNotParallelize]
    public class GameTests
    {
        private string _tempDirectory;
        private string _originalInputFile;
        private string _originalOutputFile;

        [TestInitialize]
        public void SetUp()
        {
            _originalInputFile = Game.InputFile;
            _originalOutputFile = Game.OutFile;

            _tempDirectory = Path.Combine(
                Path.GetTempPath(),
                "CatAndMouseTests",
                Guid.NewGuid().ToString("N")
            );

            Directory.CreateDirectory(_tempDirectory);
        }

        [TestCleanup]
        public void TearDown()
        {
            Game.InputFile = _originalInputFile;
            Game.OutFile = _originalOutputFile;

            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }

        [TestMethod]
        public void Constructor_CreatesPlayersNotInGame()
        {
            // Arrange / Act
            Game game = new Game(20);

            // Assert: no GameState is used; player statuses describe the initial state.
            Assert.AreEqual(State.NotInGame, game.cat.state);
            Assert.AreEqual(State.NotInGame, game.mouse.state);
            Assert.AreEqual("Cat", game.cat.name);
            Assert.AreEqual("Mouse", game.mouse.name);
        }

        [TestMethod]
        public void Run_ParsesExtraSpacesTabsAndBlankLines()
        {
            // Arrange
            (Game game, string output) = RunGame(
                "20\n\n   M    6\n   C\t17\nP\n"
            );

            // Assert: cat 17, mouse 6, absolute distance 11
            Assert.AreEqual(17, game.cat.location);
            Assert.AreEqual(6, game.mouse.location);
            StringAssert.Contains(output, " 17     6        11");
        }

        [TestMethod]
        public void Run_PrintBeforePlayersJoin_ShowsQuestionMarks()
        {
            // Arrange
            (Game _, string output) = RunGame("20\nP\n");

            // Assert
            StringAssert.Contains(output, "??    ??");
        }

        [TestMethod]
        public void Run_WhenCatCatchesMouse_StopsProcessingLaterCommands()
        {
            // Arrange: mouse starts at 5; cat starts at 1 and moves to 5.
            // The M 3 and P commands after the catch must not be processed.
            (Game game, string output) = RunGame(
                "10\nM 5\nC 1\nC 4\nM 3\nP\n"
            );

            // Assert
            Assert.AreEqual(State.Winner, game.cat.state);
            Assert.AreEqual(State.Loser, game.mouse.state);
            Assert.AreEqual(5, game.cat.location);
            Assert.AreEqual(5, game.mouse.location);
            Assert.AreEqual(4, game.cat.distanceTraveled);
            Assert.AreEqual(0, game.mouse.distanceTraveled);
            StringAssert.Contains(output, "Mouse caught at:  5");
        }

        [TestMethod]
        public void Run_WhenMouseEvades_MouseWinsAndCatLoses()
        {
            // Arrange
            (Game game, string output) = RunGame(
                "20\nM 6\nC 17\nM -7\nP\n"
            );

            // Assert
            Assert.AreEqual(State.Winner, game.mouse.state);
            Assert.AreEqual(State.Loser, game.cat.state);
            Assert.AreEqual(19, game.mouse.location);
            Assert.AreEqual(7, game.mouse.distanceTraveled);
            StringAssert.Contains(output, "Mouse evaded Cat");
        }

        [TestMethod]
        public void Run_UsesBoardSizeFromInputFile()
        {
            // Game is constructed with size 100, but the input file says 10.
            // Cat moves from cell 9 by +3 and should wrap to cell 2 to catch mouse.
            (Game game, string output) = RunGame(
                "10\nM 2\nC 9\nC 3\n",
                initialSize: 100
            );

            // Assert
            Assert.AreEqual(10, game.size);
            Assert.AreEqual(2, game.cat.location);
            Assert.AreEqual(State.Winner, game.cat.state);
            Assert.AreEqual(State.Loser, game.mouse.state);
            StringAssert.Contains(output, "Mouse caught at:  2");
        }

        [TestMethod]
        public void Run_WhenInputFileDoesNotExist_DoesNotCreateOutput()
        {
            // Arrange
            string missingInput = Path.Combine(_tempDirectory, "missing.txt");
            string outputPath = Path.Combine(_tempDirectory, "output.txt");
            Game.InputFile = missingInput;
            Game.OutFile = outputPath;

            // Act
            Game game = new Game(10);
            game.Run();

            // Assert
            Assert.IsFalse(File.Exists(outputPath));
        }

        private (Game Game, string Output) RunGame(
            string inputText,
            int initialSize = 100
        )
        {
            string inputPath = Path.Combine(_tempDirectory, "ChaseData.txt");
            string outputPath = Path.Combine(_tempDirectory, "PursuitLog.txt");

            File.WriteAllText(inputPath, inputText);
            Game.InputFile = inputPath;
            Game.OutFile = outputPath;

            Game game = new Game(initialSize);
            game.Run();

            string output = File.Exists(outputPath)
                ? File.ReadAllText(outputPath)
                : string.Empty;

            return (game, output);
        }
    }
}

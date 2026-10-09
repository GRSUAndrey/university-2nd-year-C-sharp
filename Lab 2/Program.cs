using System;
using System.IO;

namespace CatAndMouseGame
{
    public enum State
    {
        Winner,
        Loser,
        Playing,
        NotInGame
    }

    public enum GameState
    {
        Start,
        End
    }

    public class Player
    {
        public string name;
        public int location;
        public State state = State.NotInGame;
        public int distanceTraveled = 0;

        public Player(string name)
        {
            this.name = name;
            this.location = -1;
        }

        public void Move(int steps, int boardSize)
        {
            if (state == State.NotInGame)
            {
                location = steps;
                state = State.Playing;
            }
            else
            {
                int zeroBasedIndex = (location - 1 + steps) % boardSize;
                if (zeroBasedIndex < 0)
                {
                    zeroBasedIndex += boardSize;
                }
                location = zeroBasedIndex + 1;
                distanceTraveled += Math.Abs(steps);
            }
        }
    }

    public class Game
    {
        public static string InputFile = "ChaseData.txt";
        public static string OutFile = "PursuitLog.txt";

        public int size;
        public Player cat;
        public Player mouse;
        public GameState state;

        public Game(int size)
        {
            this.size = size;
            cat = new Player("Cat");
            mouse = new Player("Mouse");
            state = GameState.Start;
        }

        public void Run()
        {
            if (!File.Exists(InputFile))
            {
                Console.WriteLine($"Файл {InputFile} не найден.");
                return;
            }

            string[] lines = File.ReadAllLines(InputFile);
            if (lines.Length == 0) return;

            int firstLineSize;
            if (int.TryParse(lines[0].Trim(), out firstLineSize))
            {
                this.size = firstLineSize;
            }

            using (StreamWriter writer = new StreamWriter(OutFile))
            {
                writer.WriteLine("Cat and Mouse");
                writer.WriteLine();
                writer.WriteLine("Cat Mouse  Distance");
                writer.WriteLine("-------------------");

                for (int i = 1; i < lines.Length; i++)
                {
                    if (state == GameState.End) break;

                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    char command = parts[0][0];

                    if (command == 'P')
                    {
                        DoPrintCommand(writer);
                    }
                    else if (command == 'M' || command == 'C')
                    {
                        int steps = int.Parse(parts[1]);
                        DoMoveCommand(command, steps);

                        if (cat.state == State.Playing && mouse.state == State.Playing && cat.location == mouse.location)
                        {
                            cat.state = State.Winner;
                            mouse.state = State.Loser;
                            state = GameState.End;
                            break;
                        }
                    }
                }

                if (state != GameState.End)
                {
                    state = GameState.End;
                    if (cat.state == State.Playing && mouse.state == State.Playing && cat.location == mouse.location)
                    {
                        cat.state = State.Winner;
                        mouse.state = State.Loser;
                    }
                    else
                    {
                        mouse.state = State.Winner;
                        cat.state = State.Loser;
                    }
                }

                writer.WriteLine("-------------------");
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine("Distance traveled:   Mouse    Cat");
                writer.WriteLine($"{mouse.distanceTraveled,26}{cat.distanceTraveled,7}");
                writer.WriteLine();

                if (cat.location == mouse.location && cat.state != State.NotInGame && mouse.state != State.NotInGame)
                {
                    writer.WriteLine($"Mouse caught at: {mouse.location,2}");
                }
                else
                {
                    writer.WriteLine("Mouse evaded Cat");
                }
            }
        }

        private void DoMoveCommand(char command, int steps)
        {
            switch (command)
            {
                case 'M':
                    mouse.Move(steps, size);
                    break;
                case 'C':
                    cat.Move(steps, size);
                    break;
            }
        }

        private void DoPrintCommand(StreamWriter writer)
        {
            string catStr = cat.state == State.NotInGame ? "??" : cat.location.ToString();
            string mouseStr = mouse.state == State.NotInGame ? "??" : mouse.location.ToString();

            if (cat.state != State.NotInGame && mouse.state != State.NotInGame)
            {
                int dist = GetDistance();
                writer.WriteLine($"{catStr,3}{mouseStr,6}{dist,10}");
            }
            else
            {
                writer.WriteLine($"{catStr,3}{mouseStr,6}");
            }
        }

        private int GetDistance()
        {
            return Math.Abs(cat.location - mouse.location);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string[] testInputs = { "1.ChaseData.txt", "2.ChaseData.txt", "3.ChaseData.txt" };
            string[] testOutputs = { "1.PursuitLog.txt", "2.PursuitLog.txt", "3.PursuitLog.txt" };

            for (int i = 0; i < testInputs.Length; i++)
            {
                if (File.Exists(testInputs[i]))
                {
                    Game.InputFile = testInputs[i];
                    Game.OutFile = testOutputs[i];

                    Game game = new Game(100);
                    game.Run();
                    Console.WriteLine($"[УСПЕХ] Обработан тестовый файл {testInputs[i]} -> {testOutputs[i]}");
                }
            }
        }
    }
}
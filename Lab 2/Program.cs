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
            if (!File.Exists(InputFile)) return;

            string[] lines = File.ReadAllLines(InputFile);
            if (lines.Length == 0) return;

            int.TryParse(lines[0].Trim(), out size);

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                char command = parts[0][0];

                if (command == 'M')
                    mouse.Move(int.Parse(parts[1]), size);
                else if (command == 'C')
                    cat.Move(int.Parse(parts[1]), size);
                else if (command == 'P')
                    Console.WriteLine($"[P] Cat: {cat.location}, Mouse: {mouse.location}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(26);
            game.Run();
        }
    }
}
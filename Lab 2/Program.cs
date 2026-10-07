using System;

namespace CatAndMouseGame
{
    public enum State
    {
        Winner,
        Loser,
        Playing,
        NotInGame
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

    class Program
    {
        static void Main(string[] args)
        {
            Player p = new Player("Cat");
            p.Move(17, 26);
            p.Move(-15, 26);
            Console.WriteLine($"Position: {p.location}, Traveled: {p.distanceTraveled}");
        }
    }
}
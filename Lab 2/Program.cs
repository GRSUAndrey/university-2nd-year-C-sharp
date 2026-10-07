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
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Cat and Mouse Game initialized.");
        }
    }
}
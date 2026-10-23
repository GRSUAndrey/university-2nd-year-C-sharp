using System;

namespace CatAndMouseGame
{
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
                int zeroBasedIndex =
                    (location - 1 + steps) % boardSize;

                if (zeroBasedIndex < 0)
                {
                    zeroBasedIndex += boardSize;
                }

                location = zeroBasedIndex + 1;

                distanceTraveled += Math.Abs(steps);
            }
        }
    }
}
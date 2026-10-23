using System;
using System.IO;

namespace CatAndMouseGame
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] testInputs =
            {
                "1.ChaseData.txt",
                "2.ChaseData.txt",
                "3.ChaseData.txt",
                "4.ChaseData.txt",
                "5.ChaseData.txt",
                "6.ChaseData.txt",
                "7.ChaseData.txt",
                "8.ChaseData.txt",
                "9.ChaseData.txt"
            };

            string[] testOutputs =
            {
                "1.PursuitLog.txt",
                "2.PursuitLog.txt",
                "3.PursuitLog.txt",
                "4.PursuitLog.txt",
                "5.PursuitLog.txt",
                "6.PursuitLog.txt",
                "7.PursuitLog.txt",
                "8.PursuitLog.txt",
                "9.PursuitLog.txt"
            };

            for (int i = 0; i < testInputs.Length; i++)
            {
                if (File.Exists(testInputs[i]))
                {
                    Game.InputFile = testInputs[i];
                    Game.OutFile = testOutputs[i];

                    Game game = new Game(100);

                    game.Run();

                    Console.WriteLine(
                        $"[УСПЕХ] Обработан тестовый файл " +
                        $"{testInputs[i]} -> {testOutputs[i]}"
                    );
                }
            }
        }
    }
}
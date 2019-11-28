using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Game.Enigma;
using Game.Enigma.Models;
using Game.Output;

namespace Game
{
    public class Program
    {
        private const int Width = 120;
        private const int Height = 30;
        private const int ChoicesWidth = 30;

        public Program()
        {
        }

        public static void Main(string[] args)
        {


            Program program = new Program();
            program.Spooky();
        }

        public void Run()
        {
            EnigmaM3 enigma = new EnigmaM3(RotorModel.VI, RotorModel.II, RotorModel.VIII, ReflectorModel.UkwB);

            Console.Write("Start typing: ");
            bool needSpace = false;
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Spacebar)
                {
                    continue;
                }

                if (needSpace)
                {
                    Console.Write(' ');
                }

                char result = enigma.PerformTranslate(key.KeyChar, out needSpace);
                Console.Write(result);
            }
        }

        public void Spooky()
        {
            Random random = new Random();
            using (Sink sink = new Sink("Game", Height, Width))
            {
                ////for (int counter = 0; counter < 1000; counter++)
                {
                    CharInfo[,] info = new CharInfo[Width, Height];
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            info[x, y] = new CharInfo(new CharUnion(), CharColors.GetRandom(random));
                        }
                    }

                    sink.WriteRegion(
                        info,
                        0,
                        0);
                }


                sink.Write(
                    "Hello, this is a test of a very long string which is being written with a delay inserted between printing of each character. I want to see if it will properly scroll, or if I'm going to need to do spooky math myself to make it work.",
                    new CharColors(ConsoleColor.Green, ConsoleColor.Black),
                    new Coord(30, 10),
                    50);
            }

            Console.ReadKey();
        }
    }
}

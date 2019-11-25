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
            EnigmaM3 enigma = new EnigmaM3(RotorModel.I, RotorModel.II, RotorModel.III, ReflectorModel.UkwB);

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
                const int width = 5;
                const int height = 3;
                CharInfo[,] info = new CharInfo[width, height];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        info[x, y] = new CharInfo(new CharUnion(), CharColors.GetRandom(random));
                    }
                }

                sink.WriteRegion(
                    info,
                    6,
                    6);
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

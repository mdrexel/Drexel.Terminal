﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Game.Enigma;
using Game.Enigma.Models;
using Game.Output;
using Game.Output.Layout;
using Game.Output.Primitives;

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
                ////CharInfo[,] info = new CharInfo[Height, Width];
                ////for (int x = 0; x < Width; x++)
                ////{
                ////    for (int y = 0; y < Height; y++)
                ////    {
                ////        info[y, x] = new CharInfo(new CharUnion(), CharColors.GetRandom(random));
                ////    }
                ////}

                ////sink.WriteRegion(
                ////    info,
                ////    0,
                ////    0);

                BorderBuilder builder = new BorderBuilder(
                    namePlate: "═══════════\r\nHello World".ToCharInfo(CharColors.Standard),
                    topLeft: "╔═╦\r\n║ ║\r\n╠═╬".ToCharInfo(CharColors.Standard),
                    topRight: "╦═╗\r\n║ ║\r\n╬═╣".ToCharInfo(CharColors.Standard),
                    bottomLeft: "╠═╬\r\n╚═╩".ToCharInfo(CharColors.Standard),
                    bottomRight: "╬═╣\r\n╩═╝".ToCharInfo(CharColors.Standard),
                    leftStroke: "║ ║".ToCharInfo(CharColors.Standard),
                    topStroke: "═\r\n\r\n═".ToCharInfo(CharColors.Standard),
                    rightStroke: "║ ║".ToCharInfo(CharColors.Standard),
                    bottomStroke: "═\r\n═".ToCharInfo(CharColors.Standard));

                Region region = new Region(new Coord(0, 0), new Coord(50, 15));
                Border border = builder.Build(region);
                border.Draw(sink);

                Text foo = new Text(
                    "Foo bar baz bazinga bazongo bingo bango bongo I don't want to leave the Congo oh no no no no no",
                    new CharColors(ConsoleColor.Cyan, ConsoleColor.Black),
                    border.InnerRegion);
                foo.Draw(sink);

                ////sink.Write(
                ////    "Hello, this is a test of a very long string which is being written with a delay inserted between printing of each character. I want to see if it will properly scroll, or if I'm going to need to do spooky math myself to make it work.",
                ////    new CharColors(ConsoleColor.Green, ConsoleColor.Black),
                ////    new Coord(30, 10),
                ////    1000,
                ////    DelayMode.PerWord);

                Console.ReadKey();
            }
        }

        public void Stupid()
        {
            Coord coord = new int[3, 5] { { 0, 1, 2, 3, 4 }, { 5, 6, 7, 8, 9 }, { 10, 11, 12, 13, 14 } }.ToCoord();

            Console.WriteLine(coord);
            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Game.Enigma;
using Game.Enigma.Models;
using Game.Output;
using Game.Output.Layout;
using Game.Output.Layout.Symbols;
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
            using (Source source = new Source())
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

                CharColors borderColors = new CharColors(ConsoleColor.Blue, ConsoleColor.Black);
                BorderBuilder builder = new BorderBuilder(
                    topLeft: new FormattedString("╔═╦\r\n║ ║\r\n╠═╬", borderColors),
                    topRight: new FormattedString("╦═╗\r\n║ ║\r\n╬═╣", borderColors),
                    bottomLeft: new FormattedString("╠═╬\r\n╚═╩", borderColors),
                    bottomRight: new FormattedString("╬═╣\r\n╩═╝", borderColors),
                    leftStroke: new FormattedString("║ ║", borderColors),
                    topStroke: new FormattedString("═\r\n\r\n═", borderColors),
                    rightStroke: new FormattedString("║ ║", borderColors),
                    bottomStroke: new FormattedString("═\r\n═", borderColors));

                Region region = new Region(new Coord(0, 0), new Coord(50, 15));
                Border border = builder.Build(
                    region,
                    new FormattedString(
                        "═══════════\r\n<span fg=Red bg=Green dtime=100>Hello World</span>",
                        borderColors));
                border.Draw(sink);

                Text foo = new Text(
                    new FormattedString(
                        "Foo bar baz bazinga bazongo bingo bango bongo I don't want to leave the Congo oh no no no no no",
                        new CharColors(ConsoleColor.Green, ConsoleColor.Black)),
                    border.InnerRegion);
                foo.Draw(sink);

                Region otherRegion = new Region(new Coord(25, 5), new Coord(115, 28));
                Border otherBorder = builder.Build(otherRegion);
                otherBorder.Draw(sink);

                Text bar = new Text(
                    new FormattedString(
                        LoremIpsum,
                        new CharColors(ConsoleColor.Magenta, ConsoleColor.Black)),
                    otherBorder.InnerRegion);
                bar.Draw(sink);

                ////sink.Write(
                ////    "Hello, this is a test of a very long string which is being written with a delay inserted between printing of each character. I want to see if it will properly scroll, or if I'm going to need to do spooky math myself to make it work.",
                ////    new CharColors(ConsoleColor.Green, ConsoleColor.Black),
                ////    new Coord(30, 10),
                ////    35,
                ////    DelayMode.PerWord);

                LayoutManager layout = new LayoutManager(sink);
                Solid background = new Solid(
                    layout,
                    new Region(new Coord(0, 0), new Coord(Width, Height)),
                    builder,
                    "bar",
                    CharColors.Standard);
                Button button = new Button(
                    layout,
                    new Region(new Coord(12, 12), new Coord(30, 20)),
                    builder,
                    "foo",
                    "Hello");
                layout.Add(background);
                layout.Add(button);

                source.OnKeyPressed +=
                    (obj, e) =>
                    {
                        layout.KeyPressed(e);

                        switch (e.Key)
                        {
                            case ConsoleKey.UpArrow:
                                bar.AdjustLinesSkipped(-1);
                                bar.Draw(sink);
                                break;
                            case ConsoleKey.DownArrow:
                                bar.AdjustLinesSkipped(1);
                                bar.Draw(sink);
                                break;
                            case ConsoleKey.PageDown:
                                bar.AdjustLinesSkipped(bar.MaximumVisibleLines);
                                bar.Draw(sink);
                                break;
                            case ConsoleKey.PageUp:
                                bar.AdjustLinesSkipped(-bar.MaximumVisibleLines);
                                bar.Draw(sink);
                                break;
                            default:
                                break;
                        }
                    };

                source.OnLeftMouse +=
                    (obj, e) =>
                    {
                        layout.LeftMouseEvent(e.Position, e.ButtonDown);
                    };

                source.OnRightMouse +=
                    (obj, e) =>
                    {
                        layout.RightMouseEvent(e.Position, e.ButtonDown);
                    };

                source.OnMouseMove +=
                    (obj, e) =>
                    {
                        layout.MouseMoveEvent(e.PreviousPosition, e.CurrentPosition);
                    };

                source.OnVerticalMouseWheel +=
                    (obj, e) =>
                    {
                        layout.ScrollEvent(e.Position, e.Down);

                        if (e.Down)
                        {
                            bar.AdjustLinesSkipped(3);
                        }
                        else
                        {
                            bar.AdjustLinesSkipped(-3);
                        }

                        bar.Draw(sink);
                    };

                source.DelayUntilExitAccepted(default).Wait();
            }
        }

        public void Stupid()
        {
            Coord coord = new int[3, 5] { { 0, 1, 2, 3, 4 }, { 5, 6, 7, 8, 9 }, { 10, 11, 12, 13, 14 } }.ToCoord();

            Console.WriteLine(coord);
            Console.ReadLine();
        }

        private const string LoremIpsum = @"Moby-Dick; or, The Whale is an 1851 novel by American writer <span fg=Red>Herman Melville</span>. The book is sailor Ishmael's narrative of the obsessive quest of Ahab, captain of the whaling ship Pequod, for revenge on Moby Dick, the giant white sperm whale that on the ship's previous voyage bit off Ahab's leg at the knee. A contribution to the literature of the <span fg=Green>American Renaissance</span>, the work's genre classifications range from late Romantic to early Symbolist. Moby-Dick was published to mixed reviews, was a commercial failure, and was out of print at the time of the author's death in 1891. Its reputation as a 'Great American Novel' was established only in the 20th century, after the centennial of its author's birth. William Faulkner said he wished he had written the book himself,[1] and D. H. Lawrence called it 'one of the strangest and most wonderful books in the world' and 'the greatest book of the sea ever written'.[2] Its opening sentence, 'Call me Ishmael', is among world literature's most famous.[3]

Melville began writing Moby-Dick in February 1850, and finished 18 months later, a year longer than he had anticipated. Writing was interrupted by his meeting Nathaniel Hawthorne in <span bg=Green>August 1850</span>, and by the creation of the 'Mosses from an Old Manse' essay as a result of that friendship. The book is dedicated to Hawthorne, 'in token of my admiration for his genius'.

The basis for the work is Melville's 1841 whaling voyage aboard the Acushnet. The novel also draws on whaling literature, and on literary inspirations such as Shakespeare and the Bible. The white whale is modeled on the notoriously hard-to-catch albino whale Mocha Dick, and the book's ending is based on the sinking of the whaleship Essex in 1820. The detailed and realistic descriptions of whale hunting and of extracting whale oil, as well as life aboard ship among a culturally diverse crew, are mixed with exploration of class and social status, good and evil, and the existence of God. In addition to narrative prose, Melville uses styles and literary devices ranging from songs, poetry, and catalogs to Shakespearean stage directions, soliloquies, and asides.

In October 1851, the chapter 'The Town Ho's Story' was published in Harper's New Monthly Magazine. The same month, the whole book was first published (in three volumes) as The Whale in London, and under its definitive title in a single-volume edition in New York in November. There are hundreds of differences between the two editions, most slight but some important and illuminating. The London publisher, Richard Bentley, censored or changed sensitive passages; Melville made revisions as well, including a last-minute change to the title for the New York edition. The whale, however, appears in the text of both editions as 'Moby Dick', without the hyphen.[4] One factor that led British reviewers to scorn the book was that it seemed to be told by a narrator who perished with the ship: the British edition lacked the Epilogue, which recounts Ishmael's survival. About 3,200 copies were sold during the author's life. ";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drexel.Terminal;
using Drexel.Terminal.Sink;
using Drexel.Terminal.Source;
using Drexel.Terminal.Text;
using Drexel.Terminal.Win32;
using System.Reactive;
using System.Reactive.Linq;
using Drexel.Terminal.Primitives;
using Drexel.Terminal.Layout.Layouts;
using Drexel.Terminal.Layout.Layouts.Symbols;
using Drexel.Terminal.Internals;
using Drexel.Terminal.Layout;

namespace Drexel.Game
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            using (TerminalInstance terminal = await TerminalInstance.GetInstanceAsync(default))
            {
                terminal.Title = "Retro PI - Chill Team";
                terminal.Width = 80;
                terminal.Height = 25;
                terminal.SetCodePage(ConsoleCodePage.Utf8);
                terminal.DisableResize();

                terminal.Source.KeyboardEnabled = true;
                terminal.Source.MouseEnabled = true;

                LayoutManager manager = new LayoutManager(terminal, true);

                Solid solid = new Solid(
                    new Region(new Coord(2, 1), new Coord(77, 24)),
                    "Background",
                    new CharInfo[,] { { new CharInfo(' ', new TerminalColors(TerminalColor.White, TerminalColor.DarkBlue)) } });
                TextField textField = new TextField(
                    new Region(new Coord(7, 21), new Coord(72, 21)),
                    "TextField",
                    new TerminalColors(TerminalColor.White, TerminalColor.DarkGreen));
                var test = new Catena(
                    Short,
                    new TerminalColors(TerminalColor.White, TerminalColor.DarkCyan),
                    5);
                Label displayText = new Label(
                    new Region(new Coord(5, 4), new Coord(74, 15)),
                    "ChallengeText",
                    test,
                    new Alignments(HorizontalAlignment.Left, VerticalAlignment.Top),
                    new TerminalColors(TerminalColor.White, TerminalColor.DarkCyan),
                    false);

                manager.Add(solid);
                DrawBorders(terminal);
                manager.Add(textField);
                manager.Add(displayText);
                manager.Focused = textField;
                ////manager.Active = true;

                textField.OnComplete.Subscribe(
                    new Observer<string>(
                        x =>
                        {
                            terminal.Sink.Write(x, new Coord(7, 5));
                            textField.Clear();
                        }));

                await terminal.Source.DelayUntilExitAccepted(default);
            }
        }

        private static void DrawBorders(ITerminal terminal)
        {
            // TODO: don't have borders yet
            TerminalColors borderColors = new TerminalColors(TerminalColor.White, TerminalColor.DarkCyan);
            CharInfo[,] horizontalLinePattern =
                new CharInfo[,] { { new CharInfo('═', borderColors) } };
            CharInfo[,] verticalLinePattern =
                new CharInfo[,] { { new CharInfo('║', borderColors) } };
            Coord topLeft = new Coord(2, 1);
            Coord topRight = new Coord(77, 1);
            Coord bottomLeft = new Coord(2, 23);
            Coord bottomRight = new Coord(77, 23);
            terminal.Sink.Write(new Line(topLeft, topRight, horizontalLinePattern));
            terminal.Sink.Write(new Line(bottomLeft, bottomRight, horizontalLinePattern));
            terminal.Sink.Write(new Line(topLeft, bottomLeft, verticalLinePattern));
            terminal.Sink.Write(new Line(topRight, bottomRight, verticalLinePattern));
            terminal.Sink.Write(new CharInfo('╔', borderColors), topLeft);
            terminal.Sink.Write(new CharInfo('╗', borderColors), topRight);
            terminal.Sink.Write(new CharInfo('╚', borderColors), bottomLeft);
            terminal.Sink.Write(new CharInfo('╝', borderColors), bottomRight);
        }

        public static async Task<int> Foo(string[] args)
        {
            using (TerminalInstance terminal = await TerminalInstance.GetInstanceAsync(default))
            {
                terminal.Title = "Foo";
                terminal.Height = 12;
                terminal.Width = 40;

                terminal.SetCodePage(ConsoleCodePage.Utf8);

                terminal.Source.MouseEnabled = true;
                terminal.Source.KeyboardEnabled = true;

                terminal.Source.Mouse.OnMove.Subscribe(
                    x =>
                    {
                        terminal.Sink.Write($"Pos: {x.CurrentPosition.X}, {x.CurrentPosition.Y}  ", Coord.Zero);
                    });

                terminal.Source.Mouse.OnMove.Subscribe(
                    e =>
                    {
                        if (terminal.Source.Mouse.LeftButton.Down && terminal.Source.Mouse.RightButton.Down)
                        {
                            terminal.Sink.Write(
                                new CharInfo(' ', new TerminalColors(TerminalColor.Black, TerminalColor.Magenta)),
                                e.CurrentPosition);
                        }
                        else if (terminal.Source.Mouse.LeftButton.Down)
                        {
                            terminal.Sink.Write(
                                new CharInfo(' ', new TerminalColors(TerminalColor.Black, TerminalColor.Red)),
                                e.CurrentPosition);
                        }
                        else if (terminal.Source.Mouse.RightButton.Down)
                        {
                            terminal.Sink.Write(
                                new CharInfo(' ', new TerminalColors(TerminalColor.Black, TerminalColor.Blue)),
                                e.CurrentPosition);
                        }
                    });

                terminal.Source.OnKeyPressed.Subscribe(
                    e =>
                    {
                        if (char.IsLetterOrDigit(e.KeyChar) || char.IsPunctuation(e.KeyChar))
                        {
                            terminal.Sink.Write(new CharInfo(e.KeyChar, TerminalColors.Default));
                        }
                        else if (e.Key == TerminalKey.Spacebar)
                        {
                            terminal.Sink.Write();
                        }
                        else if (e.Key == TerminalKey.Enter)
                        {
                            terminal.Sink.WriteLine();
                        }
                        else if (e.Key == TerminalKey.UpArrow)
                        {
                            terminal.Sink.CursorPosition -= Coord.OneYOffset;
                        }
                        else if (e.Key == TerminalKey.DownArrow)
                        {
                            terminal.Sink.CursorPosition += Coord.OneYOffset;
                        }
                        else if (e.Key == TerminalKey.LeftArrow)
                        {
                            terminal.Sink.CursorPosition -= Coord.OneXOffset;
                        }
                        else if (e.Key == TerminalKey.RightArrow)
                        {
                            terminal.Sink.CursorPosition += Coord.OneXOffset;
                        }

                        ////terminal.Source.MouseEnabled = !terminal.Source.MouseEnabled;
                    });

                TerminalColors color = new TerminalColors(TerminalColor.Black, TerminalColor.Blue);
                terminal.Sink.Write(
                    new Fill(3, 3, 8, 8, new CharInfo[,] { { new CharInfo(' ', color) } }));
                terminal.Sink.Write(new Line(0, 0, 10, 10, new CharInfo[,] { { '*', '*', '*' }, { '~', '~', '~' } }));

                string test = await terminal.ReadLineAsync();

                await terminal.Source.DelayUntilExitAccepted(default);
            }

            return 0;
        }

        private const string Short = "The inventory discovered by Managed PI on a machine is not available on that machine. It is transmitted to the NOC where it is analyzed, but other applications that may need to look at the local inventory do not have access to this information.";

        private const string LoremIpsum = @"Moby-Dick; or, The Whale is an 1851 novel by American writer <span fg=Red>Herman Melville</span>. The book is sailor Ishmael's narrative of the obsessive quest of Ahab, captain of the whaling ship Pequod, for revenge on Moby Dick, the giant white sperm whale that on the ship's previous voyage bit off Ahab's leg at the knee. A contribution to the literature of the <span fg=Green>American Renaissance</span>, the work's genre classifications range from late Romantic to early Symbolist. Moby-Dick was published to mixed reviews, was a commercial failure, and was out of print at the time of the author's death in 1891. Its reputation as a 'Great American Novel' was established only in the 20th century, after the centennial of its author's birth. William Faulkner said he wished he had written the book himself,[1] and D. H. Lawrence called it 'one of the strangest and most wonderful books in the world' and 'the greatest book of the sea ever written'.[2] Its opening sentence, 'Call me Ishmael', is among world literature's most famous.[3]

Melville began writing Moby-Dick in February 1850, and finished 18 months later, a year longer than he had anticipated. Writing was interrupted by his meeting Nathaniel Hawthorne in <span bg=Green>August 1850</span>, and by the creation of the 'Mosses from an Old Manse' essay as a result of that friendship. The book is dedicated to Hawthorne, 'in token of my admiration for his genius'.

The basis for the work is Melville's 1841 whaling voyage aboard the Acushnet. The novel also draws on whaling literature, and on literary inspirations such as Shakespeare and the Bible. The white whale is modeled on the notoriously hard-to-catch albino whale Mocha Dick, and the book's ending is based on the sinking of the whaleship Essex in 1820. The detailed and realistic descriptions of whale hunting and of extracting whale oil, as well as life aboard ship among a culturally diverse crew, are mixed with exploration of class and social status, good and evil, and the existence of God. In addition to narrative prose, Melville uses styles and literary devices ranging from songs, poetry, and catalogs to Shakespearean stage directions, soliloquies, and asides.

In October 1851, the chapter 'The Town Ho's Story' was published in Harper's New Monthly Magazine. The same month, the whole book was first published (in three volumes) as The Whale in London, and under its definitive title in a single-volume edition in New York in November. There are hundreds of differences between the two editions, most slight but some important and illuminating. The London publisher, Richard Bentley, censored or changed sensitive passages; Melville made revisions as well, including a last-minute change to the title for the New York edition. The whale, however, appears in the text of both editions as 'Moby Dick', without the hyphen.[4] One factor that led British reviewers to scorn the book was that it seemed to be told by a narrator who perished with the ship: the British edition lacked the Epilogue, which recounts Ishmael's survival. About 3,200 copies were sold during the author's life. ";
    }
}

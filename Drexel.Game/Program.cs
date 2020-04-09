using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drexel.Terminal;
using Drexel.Terminal.Sink;
using Drexel.Terminal.Text;
using Drexel.Terminal.Win32;
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

                TerminalColors normalFill = new TerminalColors(TerminalColor.White, TerminalColor.DarkRed);
                TerminalColors pressedFill = new TerminalColors(TerminalColor.White, TerminalColor.DarkBlue);
                Button button = new Button(
                    new Region(new Coord(3, 3), new Coord(18, 5)),
                    "Button",
                    new Catena("Foo bar baz", normalFill),
                    new Alignments(HorizontalAlignment.Center, VerticalAlignment.Center),
                    normalFill);

                button.OnFired.Subscribe(
                    new Observer<bool>(
                        x =>
                        {
                            if (x)
                            {
                                using (manager.BufferOperation())
                                {
                                    button.BackgroundFill = pressedFill;
                                    button.Content = new Catena(button.Content.Value, pressedFill);
                                    manager.Draw();
                                }
                            }
                            else
                            {
                                using (manager.BufferOperation())
                                {
                                    button.BackgroundFill = normalFill;
                                    button.Content = new Catena(button.Content.Value, normalFill);
                                    manager.Draw();
                                }
                            }
                        }));

                manager.Add(button);

                manager.Active = true;

                await terminal.Source.DelayUntilExitAccepted(default);
            }
        }

        private const string Short = "The inventory discovered by Managed PI on a machine is not available on that machine. It is transmitted to the NOC where it is analyzed, but other applications that may need to look at the local inventory do not have access to this information.";

        private const string LoremIpsum = @"Moby-Dick; or, The Whale is an 1851 novel by American writer <span fg=Red>Herman Melville</span>. The book is sailor Ishmael's narrative of the obsessive quest of Ahab, captain of the whaling ship Pequod, for revenge on Moby Dick, the giant white sperm whale that on the ship's previous voyage bit off Ahab's leg at the knee. A contribution to the literature of the <span fg=Green>American Renaissance</span>, the work's genre classifications range from late Romantic to early Symbolist. Moby-Dick was published to mixed reviews, was a commercial failure, and was out of print at the time of the author's death in 1891. Its reputation as a 'Great American Novel' was established only in the 20th century, after the centennial of its author's birth. William Faulkner said he wished he had written the book himself,[1] and D. H. Lawrence called it 'one of the strangest and most wonderful books in the world' and 'the greatest book of the sea ever written'.[2] Its opening sentence, 'Call me Ishmael', is among world literature's most famous.[3]

Melville began writing Moby-Dick in February 1850, and finished 18 months later, a year longer than he had anticipated. Writing was interrupted by his meeting Nathaniel Hawthorne in <span bg=Green>August 1850</span>, and by the creation of the 'Mosses from an Old Manse' essay as a result of that friendship. The book is dedicated to Hawthorne, 'in token of my admiration for his genius'.

The basis for the work is Melville's 1841 whaling voyage aboard the Acushnet. The novel also draws on whaling literature, and on literary inspirations such as Shakespeare and the Bible. The white whale is modeled on the notoriously hard-to-catch albino whale Mocha Dick, and the book's ending is based on the sinking of the whaleship Essex in 1820. The detailed and realistic descriptions of whale hunting and of extracting whale oil, as well as life aboard ship among a culturally diverse crew, are mixed with exploration of class and social status, good and evil, and the existence of God. In addition to narrative prose, Melville uses styles and literary devices ranging from songs, poetry, and catalogs to Shakespearean stage directions, soliloquies, and asides.

In October 1851, the chapter 'The Town Ho's Story' was published in Harper's New Monthly Magazine. The same month, the whole book was first published (in three volumes) as The Whale in London, and under its definitive title in a single-volume edition in New York in November. There are hundreds of differences between the two editions, most slight but some important and illuminating. The London publisher, Richard Bentley, censored or changed sensitive passages; Melville made revisions as well, including a last-minute change to the title for the New York edition. The whale, however, appears in the text of both editions as 'Moby Dick', without the hyphen.[4] One factor that led British reviewers to scorn the book was that it seemed to be told by a narrator who perished with the ship: the British edition lacked the Epilogue, which recounts Ishmael's survival. About 3,200 copies were sold during the author's life. ";
    }
}

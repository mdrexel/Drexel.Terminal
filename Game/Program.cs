using System;
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

                CharColors borderColors = new CharColors(ConsoleColor.Blue, ConsoleColor.Black);
                BorderBuilder builder = new BorderBuilder(
                    namePlate: new FormattedString("═══════════\r\nHello World", borderColors),
                    topLeft: new FormattedString("╔═╦\r\n║ ║\r\n╠═╬", borderColors),
                    topRight: new FormattedString("╦═╗\r\n║ ║\r\n╬═╣", borderColors),
                    bottomLeft: new FormattedString("╠═╬\r\n╚═╩", borderColors),
                    bottomRight: new FormattedString("╬═╣\r\n╩═╝", borderColors),
                    leftStroke: new FormattedString("║ ║", borderColors),
                    topStroke: new FormattedString("═\r\n\r\n═", borderColors),
                    rightStroke: new FormattedString("║ ║", borderColors),
                    bottomStroke: new FormattedString("═\r\n═", borderColors));

                Region region = new Region(new Coord(0, 0), new Coord(50, 15));
                Border border = builder.Build(region);
                border.Draw(sink);

                Text foo = new Text(
                    "Foo bar baz bazinga bazongo bingo bango bongo I don't want to leave the Congo oh no no no no no",
                    new CharColors(ConsoleColor.Green, ConsoleColor.Black),
                    border.InnerRegion);
                foo.Draw(sink);

                Region otherRegion = new Region(new Coord(25, 5), new Coord(115, 28));
                Border otherBorder = builder.Build(otherRegion);
                otherBorder.Draw(sink);

                Text bar = new Text(
                    LoremIpsum,
                    new CharColors(ConsoleColor.Magenta, ConsoleColor.Black),
                    otherBorder.InnerRegion);
                bar.PreceedingLinesSkipped = 1;
                bar.Draw(sink);

                ////sink.Write(
                ////    "Hello, this is a test of a very long string which is being written with a delay inserted between printing of each character. I want to see if it will properly scroll, or if I'm going to need to do spooky math myself to make it work.",
                ////    new CharColors(ConsoleColor.Green, ConsoleColor.Black),
                ////    new Coord(30, 10),
                ////    35,
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

        private const string LoremIpsum = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tempus quam pellentesque nec nam aliquam sem et tortor. Nulla aliquet porttitor lacus luctus accumsan tortor. Enim nec dui nunc mattis enim. Placerat vestibulum lectus mauris ultrices eros in cursus. Diam phasellus vestibulum lorem sed risus ultricies tristique. Sed adipiscing diam donec adipiscing tristique risus nec feugiat in. Augue interdum velit euismod in pellentesque massa placerat duis. Aliquet sagittis id consectetur purus ut faucibus pulvinar elementum. Neque sodales ut etiam sit amet nisl purus in. Velit scelerisque in dictum non consectetur a erat nam. Eu augue ut lectus arcu bibendum at varius vel pharetra.

Aliquet porttitor lacus luctus accumsan tortor posuere. Massa vitae tortor condimentum lacinia quis vel eros. Adipiscing elit ut aliquam purus sit amet luctus venenatis. Blandit turpis cursus in hac habitasse. Nec ultrices dui sapien eget mi proin sed. Non enim praesent elementum facilisis leo vel. Id venenatis a condimentum vitae sapien pellentesque. Sem viverra aliquet eget sit amet tellus. Eget gravida cum sociis natoque penatibus et magnis dis parturient. Laoreet id donec ultrices tincidunt arcu. Leo a diam sollicitudin tempor. Et ultrices neque ornare aenean euismod elementum nisi. Purus sit amet luctus venenatis lectus magna fringilla. Molestie at elementum eu facilisis sed odio morbi. Interdum varius sit amet mattis vulputate enim nulla aliquet. Arcu felis bibendum ut tristique et. In mollis nunc sed id semper risus in hendrerit. Dui id ornare arcu odio ut.

Magna etiam tempor orci eu. Viverra orci sagittis eu volutpat odio facilisis. Commodo viverra maecenas accumsan lacus vel facilisis volutpat est. Diam maecenas ultricies mi eget. Massa tempor nec feugiat nisl pretium fusce id velit ut. Sagittis orci a scelerisque purus semper eget duis. Consectetur libero id faucibus nisl tincidunt eget nullam non. Malesuada pellentesque elit eget gravida cum sociis natoque penatibus. Mauris pellentesque pulvinar pellentesque habitant. Ipsum nunc aliquet bibendum enim facilisis gravida. Mi quis hendrerit dolor magna. Eget felis eget nunc lobortis mattis aliquam faucibus purus in. Quis blandit turpis cursus in. Imperdiet proin fermentum leo vel orci porta non pulvinar. Adipiscing vitae proin sagittis nisl rhoncus mattis. Consequat mauris nunc congue nisi vitae suscipit tellus. Nisl purus in mollis nunc sed. Quis eleifend quam adipiscing vitae proin sagittis nisl rhoncus mattis. Sagittis nisl rhoncus mattis rhoncus urna. In arcu cursus euismod quis viverra nibh.

Ac odio tempor orci dapibus ultrices in iaculis nunc. Ipsum suspendisse ultrices gravida dictum fusce ut placerat orci nulla. Mollis aliquam ut porttitor leo a. Enim sed faucibus turpis in. Nulla posuere sollicitudin aliquam ultrices sagittis orci a. Risus quis varius quam quisque id. Vestibulum rhoncus est pellentesque elit ullamcorper dignissim cras tincidunt lobortis. Est ante in nibh mauris. Urna cursus eget nunc scelerisque viverra mauris in aliquam. In egestas erat imperdiet sed euismod nisi porta lorem. Id venenatis a condimentum vitae sapien pellentesque habitant morbi tristique. Odio eu feugiat pretium nibh ipsum consequat. Auctor elit sed vulputate mi sit.

Urna porttitor rhoncus dolor purus non enim praesent elementum facilisis. Tristique nulla aliquet enim tortor at auctor urna. Nisl rhoncus mattis rhoncus urna neque viverra justo. Vitae aliquet nec ullamcorper sit amet risus nullam. Sit amet nisl purus in mollis. Pellentesque elit eget gravida cum sociis natoque. Orci nulla pellentesque dignissim enim sit amet venenatis. Cursus vitae congue mauris rhoncus aenean vel elit. Consectetur adipiscing elit pellentesque habitant. Egestas maecenas pharetra convallis posuere morbi leo urna. Volutpat blandit aliquam etiam erat velit.

Sit amet volutpat consequat mauris nunc congue. Augue interdum velit euismod in pellentesque. Neque laoreet suspendisse interdum consectetur libero id faucibus. Ut tortor pretium viverra suspendisse potenti nullam ac. Ipsum dolor sit amet consectetur adipiscing elit duis tristique sollicitudin. Sit amet consectetur adipiscing elit ut. Egestas egestas fringilla phasellus faucibus scelerisque eleifend donec pretium. Imperdiet sed euismod nisi porta lorem. Neque vitae tempus quam pellentesque nec. Feugiat nisl pretium fusce id velit ut tortor. Erat imperdiet sed euismod nisi. Enim nec dui nunc mattis enim ut tellus. Quisque sagittis purus sit amet volutpat consequat. Aenean pharetra magna ac placerat vestibulum lectus mauris. Quis blandit turpis cursus in hac habitasse. In metus vulputate eu scelerisque felis. In pellentesque massa placerat duis ultricies. Amet nulla facilisi morbi tempus iaculis. At lectus urna duis convallis convallis.

Venenatis tellus in metus vulputate eu scelerisque felis imperdiet. Netus et malesuada fames ac turpis egestas integer eget aliquet. Ante metus dictum at tempor. Platea dictumst vestibulum rhoncus est pellentesque. Turpis egestas maecenas pharetra convallis posuere morbi leo. Convallis convallis tellus id interdum velit. Habitasse platea dictumst quisque sagittis purus sit amet. Sit amet consectetur adipiscing elit duis tristique. Lobortis feugiat vivamus at augue eget arcu dictum varius. Eget est lorem ipsum dolor sit amet. Rutrum tellus pellentesque eu tincidunt. Enim lobortis scelerisque fermentum dui faucibus in ornare. Fusce id velit ut tortor. Nibh venenatis cras sed felis eget velit. Sed elementum tempus egestas sed sed risus pretium quam vulputate. Turpis nunc eget lorem dolor sed viverra ipsum. Pulvinar etiam non quam lacus suspendisse faucibus interdum posuere lorem. Netus et malesuada fames ac. Neque sodales ut etiam sit.

Convallis posuere morbi leo urna molestie at elementum eu. Sed ullamcorper morbi tincidunt ornare. Nec feugiat in fermentum posuere urna nec tincidunt. Turpis massa tincidunt dui ut ornare lectus. Consectetur purus ut faucibus pulvinar. Senectus et netus et malesuada fames. Blandit aliquam etiam erat velit scelerisque in. Lectus sit amet est placerat in egestas erat imperdiet sed. Eget duis at tellus at. Diam phasellus vestibulum lorem sed risus ultricies tristique. Ornare aenean euismod elementum nisi quis eleifend quam adipiscing. Justo laoreet sit amet cursus sit. Libero volutpat sed cras ornare arcu. Lorem dolor sed viverra ipsum nunc. Ut morbi tincidunt augue interdum.

Et leo duis ut diam quam nulla. Vulputate dignissim suspendisse in est ante in nibh mauris. Cras fermentum odio eu feugiat pretium nibh ipsum. Commodo ullamcorper a lacus vestibulum sed arcu non odio. Vel orci porta non pulvinar neque laoreet suspendisse. Convallis posuere morbi leo urna. Consectetur libero id faucibus nisl. Faucibus interdum posuere lorem ipsum dolor. Pellentesque diam volutpat commodo sed egestas egestas fringilla phasellus. Rutrum quisque non tellus orci ac auctor augue mauris augue. Sollicitudin aliquam ultrices sagittis orci a scelerisque purus semper eget. Id diam vel quam elementum pulvinar.

Dignissim sodales ut eu sem. Augue eget arcu dictum varius duis at consectetur lorem donec. Aliquet risus feugiat in ante. Eu facilisis sed odio morbi. Ut sem viverra aliquet eget sit amet tellus cras. Et magnis dis parturient montes nascetur ridiculus. Volutpat consequat mauris nunc congue nisi vitae suscipit tellus. Lacus sed turpis tincidunt id aliquet risus feugiat in. Venenatis urna cursus eget nunc scelerisque viverra mauris. Cursus risus at ultrices mi tempus imperdiet nulla. Curabitur vitae nunc sed velit. Id donec ultrices tincidunt arcu. Gravida quis blandit turpis cursus in. Justo nec ultrices dui sapien eget mi proin. Tincidunt augue interdum velit euismod in pellentesque massa placerat. Interdum velit euismod in pellentesque massa placerat duis. Vitae proin sagittis nisl rhoncus. Arcu non sodales neque sodales ut. Laoreet suspendisse interdum consectetur libero id faucibus nisl tincidunt.";
    }
}

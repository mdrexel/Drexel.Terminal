using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Game.Enigma;
using Game.Enigma.Models;

namespace Game
{
    public class Program
    {
        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_CLOSE = 0xF060;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_SIZE = 0xF000;

        private const int Width = 120;
        private const int Height = 28;
        private const int ChoicesWidth = 30;

        public Program()
        {
        }

        public static void Main(string[] args)
        {
            Console.WindowWidth = Width;
            Console.WindowHeight = Height + 2;

            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            if (handle != IntPtr.Zero)
            {
                DeleteMenu(sysMenu, SC_CLOSE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }

            Program program = new Program();
            program.Run();
        }

        public void Run()
        {
            EnigmaM3 enigma = new EnigmaM3(RotorModel.I, RotorModel.II, RotorModel.III, ReflectorModel.UkwB);

            Console.Write("Start typing: ");
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                Console.Write(enigma.PerformTranslate(key.KeyChar));
            }
        }

        [DllImport("user32.dll")]
        private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
    }
}

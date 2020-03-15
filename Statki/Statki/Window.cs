using System;
using System.Threading;
using System.Text;

namespace Statki
{
    public enum Keys : int { LEFT, RIGHT, UP, DOWN, ENTER, ESCAPE, ROTATE, NONE };
    class Window
    {
        private readonly string[] options = {
	/*0*/"GRA Z KOMPUTEREM",
	/*1*/"GRA Z PRZECIWNIKIEM",
	/*2*/"WCZYTAJ GRE",
	/*3*/"WYJDZ Z GRY",
	/*4*/"KONTYNUUJ GRE",
	/*5*/"ZAPISZ GRE"
        };
        private static readonly Window _Instance = new Window();
        private int chosenOption;
        private int nrKomunikatu;
        public bool CanLoadGame;
        private bool isHighlighted = true;
        private string _updown = new string(' ', Board.WIDTH + 4);
        private string _space = "\t\t\t\t\t\t";
        private Window()
        {
        }
        public static Window Instance
        {
            get
            {
                return _Instance;
            }
        }
        public void PrintBoard()
        {
            Console.Clear();
            PrintUpDown();
            for (int x = 0; x < Board.HEIGHT; ++x)
            {
                PrintLine(x);
            }
            PrintUpDown();
        }
        private void PrintLine(int line)
        {
            PrintFrame();
            for (int y = 0; y < Board.WIDTH; ++y)
            {
                PrintShipArea(Board.Instance[line, y]);
                if (y == Board.PLAYER_BOARD_WIDTH - 1)
                {
                    PrintFrame();
                    Console.Write("{0}", _space);
                    PrintFrame();
                }
            }
            PrintFrame();
            Console.WriteLine();
        }
        private void PrintFrame()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write("  ");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        private void PrintUpDown()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(_updown);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(_space);
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(_updown);
            Console.BackgroundColor = ConsoleColor.Black;
        }
        private void PrintShipArea(int index)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            switch (index)
            {
                case int i when (i < 11 && i > 0):
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    break;
                case int i when (i < 21 && i > 10):
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
                case int i when (i < 31 && i > 20):
                    Console.BackgroundColor = ConsoleColor.Blue;
                    break;
                case (int)Marker.CHOSEN_TO_ADD:
                    Console.BackgroundColor = ConsoleColor.Green;
                    break;
                case (int)Marker.NEAR_SHIP:
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    break;
                case (int)Marker.CANNOT_ADD:
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case (int)Marker.CHOSEN_TO_SHOOT:
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    break;
                case (int)Marker.ALREADY_SHOT:
                    Console.BackgroundColor = ConsoleColor.Gray;
                    break;
                case (int)Marker.CANNOT_SHOOT:
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case (int)Marker.NEAR_SUNKEN_SHIP:
                case (int)Marker.EMPTY_FIELD:
                    break;
            }
            Console.Write("  ");
        }
        public Keys ReadKey()
        {
            ConsoleKey key = Console.ReadKey(false).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    return Keys.UP;
                case ConsoleKey.DownArrow:
                    return Keys.DOWN;
                case ConsoleKey.LeftArrow:
                    return Keys.LEFT;
                case ConsoleKey.RightArrow:
                    return Keys.RIGHT;
                case ConsoleKey.Enter:
                    return Keys.ENTER;
                case ConsoleKey.Escape:
                    return Keys.ESCAPE;
                case ConsoleKey.R:
                    return Keys.ROTATE;
                default:
                    return Keys.NONE;
            }
        }
        public int ShowMenu()
        {
            while (ShowMenuOptions()) ;
            return chosenOption;
        }
        private bool ShowMenuOptions()
        {
            Console.Clear();
            int menuSize = CanLoadGame ? options.Length : options.Length - 2;
            for (int i = 0; i < menuSize; i++)
            {
                Console.Write(("").PadRight(40, ' '));
                if (i == chosenOption)
                {
                    if (isHighlighted)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    isHighlighted = isHighlighted ? false : true;
                }
                Console.WriteLine(options[i]);

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (ReadOption(menuSize))
            {
                return false;
            }
            return true;
        }
        private bool ReadOption(int size) {
            Keys key = Keys.NONE;
            if (Console.KeyAvailable)
            {
                key = ReadKey();
            }
            switch (key)
            {
                case Keys.UP:
                    {
                        chosenOption = chosenOption == 0 ? size - 1 : chosenOption - 1;
                        break;
                    }
                case Keys.DOWN:
                    {
                        chosenOption = chosenOption == size - 1 ? 0 : chosenOption + 1;
                        break;
                    }
                case Keys.ENTER:
                    {
                        Thread.Sleep(200);
                        return true;
                    }
            }
            Thread.Sleep(100);
            return false;
        }
    }
}

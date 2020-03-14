using System;

namespace Statki {
    public enum Marker : int {
        PUSTE_POLE, WYBRANE_DO_USTAWIENIA = 41, W_POBLIZU, NIE_MOZNA_USTAWIC = 44,
        WYBRANE_DO_STRZALU = 46, JUZ_STRZELANO = 48, NIE_MOZNA_STRZELAC = 50, OKOLICA_ZATOPIONEGO = 51
    };
    public enum Keys : int { LEFT, RIGHT, UP, DOWN, ENTER, ESCAPE, ROTATE, NONE};
    public sealed class Board
    {
        private static readonly Board _Instance = new Board();

        public const int HEIGHT = 10, WIDTH = 20;
        private int[,] _board = new int[HEIGHT, WIDTH];
        private string _updown = new string(' ', WIDTH + 4);
        private string _space = "\t\t\t\t\t\t";

        private Board()
        {
        }
        public static Board Instance
        {
            get
            {
                return _Instance;
            }
        }
        public int this[int indx, int indy, bool whichBoard = false]
        {
            get
            {
                if (indx >= 0 && indx < HEIGHT && indy >= 0 && indy < WIDTH)
                {
                    int shift = whichBoard ? WIDTH / 2 : 0;
                    return _board[indx, indy + shift];
                }
                return -1;
            }
            set
            {
                if (indx >= 0 && indx < HEIGHT && indy >= 0 && indy < WIDTH)
                {
                    int shift = whichBoard ? WIDTH / 2:0;
                    _board[indx, indy + shift] = value;
                }
            }
        }

        public void Init()
        {
            Console.BackgroundColor = ConsoleColor.Black;
        }
        public int GetArea(int i, int j, bool whichBoard)
        {
            return this[i, j, whichBoard];
        }
        public void SetArea(int x, int y, int val, bool whichBoard)
        {
            this[x, y, whichBoard] = val;
        }
        public void SetAreaIf(int x, int y, int val, int condition, bool whichBoard)
        {
            if (x >= 0 && x < WIDTH / 2 && y >= 0 && y < WIDTH / 2)
            {
                if (this[x, y, whichBoard] == condition)
                    this[x, y, whichBoard] = val;
            }
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
        public void PrintBoard()
        {
            Console.Clear();
            PrintUpDown();
            for (int x = 0; x < HEIGHT; ++x)
            {
                PrintLine(x);
            }
            PrintUpDown();
        }
        private void PrintLine(int line)
        {
            PrintFrame();
            for (int y = 0; y < WIDTH; ++y)
            {
                PrintShipArea(this[line, y]);
                if (y == WIDTH / 2 - 1)
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
                case int i when (i < 11 && i > 0): //pole z ustawionym statkiem
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    break;
                case int i when (i < 21 && i > 10): //pole z trafionym statkiem
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
                case int i when (i < 31 && i > 20): //zatopiony statek
                    Console.BackgroundColor = ConsoleColor.Blue;
                    break;
                case (int)Marker.WYBRANE_DO_USTAWIENIA:
                    Console.BackgroundColor = ConsoleColor.Green;
                    break;
                case (int)Marker.W_POBLIZU:
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    break;
                case (int)Marker.NIE_MOZNA_USTAWIC:
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case (int)Marker.WYBRANE_DO_STRZALU:
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    break;
                case (int)Marker.JUZ_STRZELANO:
                    Console.BackgroundColor = ConsoleColor.Gray;
                    break;
                case (int)Marker.NIE_MOZNA_STRZELAC:
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case (int)Marker.OKOLICA_ZATOPIONEGO:
                case (int)Marker.PUSTE_POLE:
                    break;
            }
            Console.Write("  ");
        }

        public void ClearMarks()
        {
            for (int x = 0; x < HEIGHT; ++x)
            {
                for (int y = 0; y < WIDTH; ++y)
                {
                    if (this[x, y] == (int)Marker.W_POBLIZU)
                        this[x, y] = (int)Marker.PUSTE_POLE;
                }
            }
        }
        public void ClearBoard()
        {
            for (int x = 0; x < HEIGHT; ++x)
            {
                for (int y = 0; y < WIDTH; ++y)
                {
                    this[x, y] = (int)Marker.PUSTE_POLE;
                }
            }
        }
    }
}
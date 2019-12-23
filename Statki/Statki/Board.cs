using System;

namespace Statki {
    public enum Marker : int {
        PUSTE_POLE, WYBRANE_DO_USTAWIENIA = 41, W_POBLIZU, NIE_MOZNA_USTAWIC = 44,
        WYBRANE_DO_STRZALU = 46, JUZ_STRZELANO = 48, NIE_MOZNA_STRZELAC = 50, OKOLICA_ZATOPIONEGO = 51
    };
    public sealed class Board {
        private const int height = 10, width = 20;
        private static Board _instance = null;
        private static int[,] _board;
        private static string _updown = new string(' ', width + 4);
        private static string _space = "\t\t\t\t\t\t";
        private Board() {
        }
        public static Board Instance {
            get {
                if (_instance == null) {
                    _instance = new Board();
                    _board = new int[height, width];
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                return _instance;
            }
        }
        public static int GetArea(int i, int j)
        {
            return _board[i, j];
        }
        public static void SetArea(int i, int j, int val, bool whichBoard)
        {
            int shift = whichBoard ? width / 2 : 0;
            _board[i, j + shift] = val;
        }
        public static void SetAreaIf(int x, int y, int val, int condition, bool whichBoard)
        {
            if (x >= 0 && x < width / 2 && y >= 0 && y < width / 2)
            {
                int shift = whichBoard ? width / 2 : 0;
                if (_board[x, y + shift] == condition)
                    _board[x, y + shift] = val;
            }
        }
        public static void PrintBoard()
        {
            PrintUpDown();
            for (int i = 0; i < height; ++i)
            {
                PrintLine(i);
            }
            PrintUpDown();
        }
        private static void PrintLine(int numb)
        {
            PrintFrame();
            for (int j = 0; j < width; ++j)
            {
                PrintShipArea(_board[numb, j]);
                if (j == width / 2 - 1)
                {
                    PrintFrame();
                    Console.Write("{0}", _space);
                    PrintFrame();
                }
            }
            PrintFrame();
            Console.WriteLine();
        }
        private static void PrintFrame()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write("  ");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        private static void PrintUpDown()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(_updown);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(_space);
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(_updown);
            Console.BackgroundColor = ConsoleColor.Black;
        }
        private static void PrintShipArea(int index)
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

        public static void ClearMarks()
        {
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    if (_board[i,j] == (int)Marker.W_POBLIZU)
                        _board[i, j] = (int)Marker.PUSTE_POLE;
                }
            }
        }
        public static void ClearBoard()
        {
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    _board[i, j] = 0;
                }
            }
        }
    }
}
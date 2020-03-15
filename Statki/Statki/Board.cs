using System;

namespace Statki {
    public enum Marker : int {
        EMPTY_FIELD, CHOSEN_TO_ADD = 41, NEAR_SHIP, CANNOT_ADD = 44,
        CHOSEN_TO_SHOOT = 46, ALREADY_SHOT = 48, CANNOT_SHOOT = 50, NEAR_SUNKEN_SHIP = 51
    };

    public sealed class Board
    {
        private static readonly Board _Instance = new Board();

        public const int HEIGHT = 10, WIDTH = 20, PLAYER_BOARD_WIDTH = WIDTH/2;
        private int[,] _board;

        private Board()
        {
            _board = new int[HEIGHT, WIDTH];
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
        public void ClearMarks()
        {
            for (int x = 0; x < HEIGHT; ++x)
            {
                for (int y = 0; y < WIDTH; ++y)
                {
                    if (this[x, y] == (int)Marker.NEAR_SHIP)
                        this[x, y] = (int)Marker.EMPTY_FIELD;
                }
            }
        }
        public void ClearBoard()
        {
            for (int x = 0; x < HEIGHT; ++x)
            {
                for (int y = 0; y < WIDTH; ++y)
                {
                    this[x, y] = (int)Marker.EMPTY_FIELD;
                }
            }
        }
    }
}
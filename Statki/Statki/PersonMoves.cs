using System.Threading;

namespace Statki
{
    class PersonMoves : Moves
    {
        public PersonMoves(bool boardNum, Moves opponent = null) : base(boardNum, Players.PERSON, opponent)
        {
            //AddShips();
            AddShipsTest();
        }

        public override bool Shoot()
        {
            bool wasHit = false;
            do
            {
                wasHit = false;
                int selectedField = 0;
                bool canShoot = false;
                int shoot = 0;
                do
                {
                    canShoot = false;
                    selectedField = Board.Instance[_x, _y, _opponentBoard];
                    if (selectedField >= 0 && selectedField <= 10)
                    {
                        Board.Instance[_x, _y, _opponentBoard] = (int)Marker.CHOSEN_TO_SHOOT;
                        canShoot = true;
                    }
                    else
                    {
                        Board.Instance[_x, _y, _opponentBoard] = (int)Marker.CANNOT_SHOOT;
                    }
                    Window.Instance.PrintBoard();
                    Board.Instance[_x, _y, _opponentBoard] = selectedField;

                    shoot = readKeyforShoot(ref _x, ref _y);
                    if (shoot == -1)
                    { //back to menu
                        return false;
                    }
                } while (shoot != 1 || !canShoot);

                if (selectedField > 0 && selectedField <= 10)
                {
                    if (_opponent.GetShip(selectedField - 1).HitShip(_x, _y))
                    {
                        ++_sunkenShips;
                        if (_sunkenShips == 10)
                        {
                            return false;
                        }
                    }
                    wasHit = true;
                }
                else
                {
                    Board.Instance[_x, _y, _opponentBoard] = (int)Marker.ALREADY_SHOT;
                }
            } while (wasHit);
            return true;
        }

        private int readKeyforShoot(ref int x, ref int y)
        {
            bool isLoaded = false;
            while (!isLoaded)
            {
                Keys key = Window.Instance.ReadKey();
                switch (key)
                {
                    case Keys.DOWN:
                        if (++x > Board.HEIGHT - 1)
                        { x = 0; }
                        isLoaded = true;
                        break;
                    case Keys.UP:
                        if (--x < 0)
                        { x = Board.HEIGHT - 1; }
                        isLoaded = true;
                        break;
                    case Keys.RIGHT:
                        if (++y > Board.PLAYER_BOARD_WIDTH - 1)
                        { y = 0; }
                        isLoaded = true;
                        break;
                    case Keys.LEFT:
                        if (--y < 0)
                        { y = Board.PLAYER_BOARD_WIDTH - 1; }
                        isLoaded = true;
                        break;
                    case Keys.ESCAPE:
                        Thread.Sleep(100);
                        return -1;
                    case Keys.ENTER:
                        Thread.Sleep(100);
                        return 1;
                }
            }
            Thread.Sleep(100);
            return 0;
        }
        private void AddShipsTest()
        {
            for (int i = 1; i < 6; i++)
            {
                _playerShips[i - 1] = new Ship(2 * (i - 1),0, 5, i, false, _whichBoard);
            }
            for (int i = 6; i <= 10; i++)
            {
                _playerShips[i - 1] = new Ship(2 * (i - 6), 6, 4, i, false, _whichBoard);
            }
            Board.Instance.ClearMarks();
        }
        protected override void AddShips()
        {
            int shipNumb = 1;
            int x = 0, y = 0;

            foreach (int currSize in _shipSize)
            {
                bool couldAdd = false;
                bool isFit = true;
                bool isVertical = false;
                y = y + currSize - 1 > Board.PLAYER_BOARD_WIDTH - 1 ? 0 : y;

                do
                {
                    isFit = IsFit(x, y, currSize, isVertical, _whichBoard);
                    couldAdd = ReadKey(ref x, ref y, currSize, ref isVertical);
                    IsInBoard(ref x, ref y, currSize, isVertical);
                } while (!couldAdd || !isFit);
                _playerShips[shipNumb - 1] = new Ship(x, y, currSize, shipNumb, isVertical, _whichBoard);
                ++shipNumb;
            }
            Board.Instance.ClearMarks();
        }
        private void IsInBoard(ref int x, ref int y, int size, bool isVertical)
        {
            int coord1 = isVertical ? x : y;
            int coord2 = isVertical ? y : x;
            if (coord2 > Board.HEIGHT - 1)
                coord2 = 0;
            else if (coord1 + size > Board.HEIGHT)
                coord1 = 0;
            else if (coord2 < 0)
                coord2 = Board.HEIGHT - 1;
            else if (coord1 < 0)
                coord1 = Board.HEIGHT - size;
            x = isVertical ? coord1 : coord2;
            y = isVertical ? coord2 : coord1;
        }
        private bool ReadKey(ref int x, ref int y, int currSize, ref bool isVertical)
        {
            Keys key = Window.Instance.ReadKey();
            switch (key)
            {
                case Keys.DOWN:
                    x++;
                    break;
                case Keys.UP:
                    x--;
                    break;
                case Keys.RIGHT:
                    y++;
                    break;
                case Keys.LEFT:
                    y--;
                    break;
                case Keys.ROTATE:
                    RotateShip(ref x, ref y, currSize, ref isVertical);
                    break;
                case Keys.ENTER:
                    return true;

            }
            return false;
        }
        private void RotateShip(ref int x, ref int y, int size, ref bool isVertical)
        {
            int coord = isVertical ? y : x;
            if (coord + size > Board.HEIGHT)
            {
                coord += - 9 - size + 1;
            }
            x = isVertical ? x : coord;
            y = isVertical ? coord : y;
            isVertical = !isVertical;
        }
        private bool IsFit(int x, int y, int currSize, bool isVertical, bool _whichBoard)
        {
            int[] tempShip = new int[currSize];
            bool isFit = true;
            for (int i = 0, j = 0; i < currSize && j < currSize;)
            {
                if (Board.Instance[x + j, y + i, _whichBoard] == 0)
                    Board.Instance[x + j, y + i, _whichBoard] = 41;
                else
                {
                    int c = isVertical ? j : i;
                    tempShip[c] = Board.Instance[x + j, y + i, _whichBoard];
                    Board.Instance[x + j, y + i, _whichBoard] = 44;
                    isFit = false;

                }
                if (isVertical) ++j;
                else ++i;
            }
            Window.Instance.PrintBoard();
            for (int i = 0, j = 0; i < currSize && j < currSize;)
            {
                int c = isVertical ? j : i;
                int val = Board.Instance[x + j, y + i, _whichBoard] == 44 ? tempShip[c] : 0;
                Board.Instance[x + j, y + i, _whichBoard] = val;
                if (isVertical) ++j;
                else ++i;
            }
            return isFit;
        }
    }
}

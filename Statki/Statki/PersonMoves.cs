using System;
using System.Collections.Generic;
using System.Text;

namespace Statki
{
    class PersonMoves : Moves
    {
        public PersonMoves(bool boardNum) : base(boardNum, Players._PERSON)
        {
            AddShips();
        }
        public PersonMoves(bool boardNum, Moves opponent) : base(boardNum, Players._PERSON, opponent)
        {
            AddShips();
        }

        public override bool Shoot()
        {
            throw new NotImplementedException();
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
                y = y + currSize - 1 > 9 ? 0 : y;

                do
                {
                    isFit = IsFit(x, y, currSize, isVertical, _whichBoard);
                    couldAdd = ReadKey(ref x, ref y, currSize, ref isVertical);
                    IsInBoard(ref x, ref y, currSize, isVertical);
                } while (!couldAdd || !isFit);
                _playerShips[shipNumb - 1] = new Ship(x, y, currSize, shipNumb, isVertical, _whichBoard);
                ++shipNumb;
            }
            Board.ClearMarks();
        }
        private void IsInBoard(ref int x, ref int y, int size, bool isVertical)
        {
            int coord1 = isVertical ? x : y;
            int coord2 = isVertical ? y : x;
            if (coord2 > Board.height - 1)
                coord2 = 0;
            else if (coord1 + size > Board.height)
                coord1 = 0;
            else if (coord2 < 0)
                coord2 = Board.height - 1;
            else if (coord1 < 0)
                coord1 = Board.height - size;
            x = isVertical ? coord1 : coord2;
            y = isVertical ? coord2 : coord1;
        }
        private bool ReadKey(ref int x, ref int y, int currSize, ref bool isVertical)
        {
            Keys key = Board.ReadKey();
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
            if (coord + size > Board.height)
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
                if (Board.GetArea(x + j, y + i, _whichBoard) == 0)
                    Board.SetArea(x + j, y + i, 41, _whichBoard);
                else
                {
                    int c = isVertical ? j : i;
                    tempShip[c] = Board.GetArea(x + j, y + i, _whichBoard);
                    Board.SetArea(x + j, y + i, 44, _whichBoard);
                    isFit = false;

                }
                if (isVertical) ++j;
                else ++i;
            }
            Board.PrintBoard();
            for (int i = 0, j = 0; i < currSize && j < currSize;)
            {
                int c = isVertical ? j : i;
                int val = Board.GetArea(x + j, y + i, _whichBoard) == 44 ? tempShip[c] : 0;
                Board.SetArea(x + j, y + i, val, _whichBoard);
                if (isVertical) ++j;
                else ++i;
            }
            return isFit;
        }
    }
}

using System;
using System.Threading;
using System.Linq;

namespace Statki
{
    class ComputerMoves : Moves
    {
        private enum Directions { _DOWN = 0, _UP, _RIGHT, _LEFT };
        private int dirOfShooting;
        private bool[] chosenDir;
        private int lastX, lastY;
        private bool wasHitAfterDraw;
        private bool wasHitAfterChoosingDir;
        private bool sameDirection;

        public ComputerMoves(bool boardNum, Moves opponent = null) : base(boardNum, Players.COMPUTER, opponent)
        {
            AddShips();
            chosenDir = Enumerable.Repeat(false, 4).ToArray();
        }

        protected override void AddShips()
        {
            int shipNumb = 1;
            foreach (int currSize in _shipSize)
            {
                MakeShip(currSize, out int x, out int y, out bool isVertical);
                _playerShips[shipNumb - 1] = new Ship(x, y, currSize, shipNumb, isVertical, _whichBoard);
                ++shipNumb;
            }
        }

        private void MakeShip(int size, out int x, out int y, out bool isVertical)
        {
            Random rnd = new Random();
            bool isFit = false;
            x = y = 0;
            isVertical = Convert.ToBoolean(rnd.Next(2));

            while (!isFit)
            {
                x = rnd.Next(10);
                y = rnd.Next(10);

                int coord = isVertical ? x : y;
                isFit = true;
                if (coord + size > Board.HEIGHT)
                {
                    x = isVertical ? 0 : x;
                    y = isVertical ? y : 0;
                }

                for (int i = 0; i < size; i++)
                {
                    int px = isVertical ? i : 0;
                    int py = isVertical ? 0 : i;
                    if (Board.Instance[x + px, y + py, _whichBoard] != (int)Marker.EMPTY_FIELD)
                    {
                        isFit = false;
                        break;
                    }
                }
            }
        }
        public override bool Shoot()
        {
            bool wasHit =  false;
            do
            {
                if (!wasHitAfterDraw)
                {
                    Random rnd = new Random();
                    do
                    {
                        lastX = _x = rnd.Next(Board.PLAYER_BOARD_WIDTH);
                        lastY = _y = rnd.Next(Board.HEIGHT);
                    } while (Board.Instance[_x, _y, _opponentBoard] > 10);
                    wasHit = ShotAfterCoordDraw();
                }
                else
                {
                    ChooseDir();
                    wasHit = ShotAfertDirDraw();
                }
                if (_sunkenShips == 10)
                {
                    return false;
                }
                Window.Instance.PrintBoard();
                Thread.Sleep(1000);
            } while (wasHit);
            return true;
        }
        private bool ShotAfterCoordDraw()
        {
            if (Board.Instance[lastX, lastY, _opponentBoard] != (int)Marker.EMPTY_FIELD)
            {
                HitShip(true);
                return true;
            }
            else
            {
                Board.Instance[lastX, lastY, _opponentBoard] = (int)Marker.ALREADY_SHOT;
                return false;
            }
        }
        private bool ShotAfertDirDraw()
        {
            if (Board.Instance[lastX, lastY, _opponentBoard] != (int)Marker.EMPTY_FIELD)
            {
                HitShip(false);
                return true;
            }
            else
            {
                if (wasHitAfterChoosingDir)
                { 
                    sameDirection = true;
                    ReverseDirection();
                    if (chosenDir[dirOfShooting])
                    {
                        wasHitAfterDraw = false;
                    }
                    wasHitAfterChoosingDir = false;
                }
                else
                {
                    sameDirection = false;
                    chosenDir[dirOfShooting] = true;
                }
                Board.Instance[lastX, lastY, _opponentBoard] = (int)Marker.ALREADY_SHOT;
                lastX = _x;
                lastY = _y;
                return false;
            }
        }
        private void ReverseDirection()
        {
            chosenDir[dirOfShooting] = true;
            switch (dirOfShooting)
            {
                case int i when ( i == (int)Directions._UP || i == (int)Directions._DOWN):
                    {
                        dirOfShooting = dirOfShooting == (int)Directions._UP ? (int)Directions._DOWN : (int)Directions._UP;
                        break;
                    }
                case int i when (i == (int)Directions._RIGHT || i == (int)Directions._LEFT):
                    {
                        dirOfShooting = dirOfShooting == (int)Directions._RIGHT ? (int)Directions._LEFT : (int)Directions._RIGHT;
                        break;
                    }
            }
        }
        private void ChooseDir()
        {
            bool canShootHere = false;
            bool isOutsideBoard = false;
            while (!canShootHere || isOutsideBoard)
            {
                if (!sameDirection)
                {
                    do
                    {
                        dirOfShooting = new Random().Next(4);
                    } while (chosenDir[dirOfShooting]);
                }
                isOutsideBoard = GoTowards();
                if (Board.Instance[lastX, lastY, _opponentBoard] > 10 || isOutsideBoard)
                {
                    lastX = _x;
                    lastY = _y;
                    ReverseDirection();
                    canShootHere = false;
                }
                else
                {
                    canShootHere = true;
                }
            }
        }
        private bool GoTowards()
        {
            switch (dirOfShooting)
            {
                case int i when (i == (int)Directions._RIGHT || i == (int)Directions._DOWN):
                    {
                        int wsp = dirOfShooting == (int)Directions._DOWN ? lastX : lastY;
                        if (wsp + 1 > Board.PLAYER_BOARD_WIDTH - 1)
                            return true;
                        else
                            wsp += 1;
                        lastX = dirOfShooting == (int)Directions._DOWN ? wsp : lastX;
                        lastY = dirOfShooting == (int)Directions._DOWN ? lastY : wsp;
                        break;
                    }
                case int i when (i == (int)Directions._LEFT || i == (int)Directions._UP):
                    {
                        int wsp = dirOfShooting == (int)Directions._UP ? lastX : lastY;
                        if (wsp - 1 < 0)
                            return true;
                        else
                            wsp -= 1;
                        lastX = dirOfShooting == (int)Directions._UP ? wsp : lastX;
                        lastY = dirOfShooting == (int)Directions._UP ? lastY : wsp;
                        break;
                    }
            }
            return false;
        }
        private void HitShip(bool isAfterDraw)
        {
            int numbOfHitShip = Board.Instance[lastX, lastY, _opponentBoard];
            if (_opponent.GetShip(numbOfHitShip - 1).HitShip(lastX, lastY))
            {
                if (!isAfterDraw)
                {
                    sameDirection = wasHitAfterChoosingDir = wasHitAfterDraw = false;
                    for(int i = 0; i < chosenDir.Length; ++i)
                    {
                        chosenDir[i] = false;
                    }
                }
                ++_sunkenShips;
            }
            else if (!isAfterDraw)
            {
                sameDirection = wasHitAfterChoosingDir = true;
            }
            else
            {
                wasHitAfterDraw = true;
            }
        }
    } 
}

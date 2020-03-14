using System;
using System.Collections.Generic;
using System.Threading;

namespace Statki
{
    class ComputerMoves : Moves
    {
        private enum Directions { _Down = 0, _Up, _Right, _Left };
        private int dirOfShooting;
        private bool[] chosenDir = new bool[4];
        private int lastX, lastY;
        bool wasHitAfterDraw;
        bool wasHitAfterChoosingDir;
        bool sameDirection;

        public ComputerMoves(bool boardNum) : base(boardNum, Players._COMPUTER)
        {
            AddShips();
        }
        public ComputerMoves(bool boardNum, Moves opponent) : base(boardNum, Players._COMPUTER, opponent)
        {
            AddShips();
        }

        protected override void AddShips()
        {
            int shipNumb = 1;
            foreach (int currSize in _shipSize)
            {
                (int x, int y, int isVertical) = makeShip(currSize);
                _playerShips[shipNumb - 1] = new Ship(x, y, currSize, shipNumb, isVertical == 1, _whichBoard);
                Board.Instance.PrintBoard();
                ++shipNumb;
            }
        }

        private Tuple<int, int, int> makeShip(int size)
        {
            Random rnd = new Random();
            bool isFit = false;
            int x = 0, y = 0;
            int isVertical = rnd.Next(2);
            while (!isFit)
            {
                x = rnd.Next(10);
                y = rnd.Next(10);

                int coord = isVertical == 1 ? x : y;
                isFit = true;
                if (coord + size - 1 > 9)
                    coord = 0;
                x = isVertical == 1 ? coord : x;
                y = isVertical == 1 ? y : coord;

                for (int i = 0; i < size; i++)
                {
                    int px = isVertical == 1 ? i : 0;
                    int py = isVertical == 1 ? 0 : i;
                    if (Board.Instance[x + px, y + py, _whichBoard] != (int)Marker.PUSTE_POLE)
                    {
                        isFit = false;
                        break;
                    }
                }
            }
            return new Tuple<int, int, int>( x, y, isVertical );
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
                        lastX = _x = rnd.Next(10);
                        lastY = _y = rnd.Next(10);
                    } while (Board.Instance[_x, _y, !_whichBoard] > 10);
                    wasHit = shotAfterCoordDraw();
                }
                else
                {
                    chooseDir();
                    wasHit = shotAfterCoordDraw();
                }
                if (_sunkenShips == 10)
                {
                    return false;
                }
                Board.Instance.PrintBoard();
                Thread.Sleep(1000);
            } while (wasHit);
            return true;
        }
        private bool shotAfterCoordDraw()
        {
            if (Board.Instance[lastX, lastY, !_whichBoard] != (int)Marker.PUSTE_POLE)
            {
                hitShip(true);
                return true;
            }
            else
            {
                Board.Instance[lastX, lastY, !_whichBoard] = (int)Marker.JUZ_STRZELANO;
                return false;
            }
        }
        private bool shotAfertDirDraw()
        {
            if (Board.Instance[lastX, lastY, !_whichBoard] != (int)Marker.PUSTE_POLE)
            {
                hitShip(false);
                return true;
            }
            else
            {
                if (wasHitAfterChoosingDir)
                { //jesli idac w tym kierunku chociaz raz trafil, a pozniej chybil, to idzie w druga strone od miejsca pierwszego trafienia
                    sameDirection = true;
                    reverseDirection();
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
                Board.Instance[lastX, lastY, !_whichBoard] = (int)Marker.JUZ_STRZELANO;
                lastX = _x;
                lastY = _y;
                return false;
            }
        }
        private void reverseDirection()
        {
            chosenDir[dirOfShooting] = true;
            if (dirOfShooting <= (int)Directions._Up)
                dirOfShooting = dirOfShooting == (int)Directions._Up ? (int)Directions._Down : (int)Directions._Up;
            else
                dirOfShooting = dirOfShooting == (int)Directions._Right ? (int)Directions._Left : (int)Directions._Right;
        }
        private void chooseDir()
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
                isOutsideBoard = goTowards();
                if (Board.Instance[lastX, lastY, !_whichBoard] > 10 || isOutsideBoard)
                {
                    lastX = _x;
                    lastY = _y;
                    reverseDirection();
                    canShootHere = false;
                }
                else
                {
                    canShootHere = true;
                }
            }
        }
        bool goTowards()
        {
            if (dirOfShooting % 2 == 1)
            {
                int wsp = dirOfShooting == (int)Directions._Down ? lastX : lastY;
                if (wsp + 1 > 9)
                    return true;
                else
                    wsp += 1;
                lastX = dirOfShooting == (int)Directions._Down ? wsp : lastX;
                lastY = dirOfShooting == (int)Directions._Down ? lastY : wsp;
            }
            else
            {
                int wsp = dirOfShooting == (int)Directions._Up ? lastX : lastY;
                if (wsp - 1 < 0)
                    return true;
                else
                    wsp -= 1;
                lastX = dirOfShooting == (int)Directions._Up ? wsp : lastX;
                lastY = dirOfShooting == (int)Directions._Up ? lastY : wsp;
            }
            return false;
        }
        void hitShip(bool isAfterDraw)
        {
            int numbOfHitShip = Board.Instance[lastX, lastY, !_whichBoard];
            if (_opponent.GetShip(numbOfHitShip - 1).HitShip(_x, _y))
            {
                if (!isAfterDraw)
                {
                    sameDirection = wasHitAfterChoosingDir = wasHitAfterDraw = false;
                    chosenDir[0] = chosenDir[1] = chosenDir[2] = chosenDir[3] = false; // CORRECT!
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

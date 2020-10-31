using System;
using System.Threading;
using System.Linq;

namespace Statki
{
	class ComputerMoves : Moves
	{
		private enum Directions { Down, Up, Right, Left };
		private int dirOfShooting;
		private bool[] chosenDir;
		private int lastX, lastY;
		private bool wasHitAfterDraw;
		private bool wasHitAfterChoosingDir;
		private bool sameDirection;

		public ComputerMoves(BoardSide boardNum, Moves opponent = null, bool afterLoad = false) : base(boardNum, Players.Computer, opponent)
		{
			if (!afterLoad)
			{
				AddShips();
			}
			chosenDir = Enumerable.Repeat(false, 4).ToArray();
		}

		protected override void AddShips()
		{
			int shipNumb = 0;
			foreach (int currSize in _shipSize)
			{
				PlayerShips[shipNumb] = MakeShip(currSize, shipNumb);
				DrawShip(shipNumb);
				MarkShipNeighborhood(false, shipNumb);
				++shipNumb;
			}
			ClearNearShipMarks();
		}

		private Ship MakeShip(int size, int shipNumb)
		{
			Random rnd = new Random();
			bool isFit = false;
			int x = 0, y = 0;
			bool isVertical = Convert.ToBoolean(rnd.Next(2));

			while (!isFit)
			{
				x = rnd.Next(10);
				y = rnd.Next(10);

				int coord = isVertical ? x : y;
				isFit = true;
				if (coord + size > Height)
				{
					x = isVertical ? UpperEdge : x;
					y = isVertical ? y : LeftEdge;
				}
				for (int i = 0; i < size; i++)
				{
					int px = isVertical ? i : UpperEdge;
					int py = isVertical ? LeftEdge : i;
					if (this[x + px, y + py] != (int)Marker.EmptyField)
					{
						isFit = false;
						break;
					}
				}
			}
			return new Ship(x, y, size, shipNumb, isVertical);
		}
		public override Actions Shoot()
		{
			bool wasHit =  false;		
			do
			{
				if (!wasHitAfterDraw)
				{
					Random rnd = new Random();
					do
					{
						lastX = _x = rnd.Next(Width);
						lastY = _y = rnd.Next(Height);
					} while (Opponent[_x, _y] != (int)Marker.EmptyField && Opponent[_x, _y] > (int)Marker.LastShip);
					wasHit = ShotAfterCoordDraw();
				}
				else
				{
					ChooseDir();
					wasHit = ShotAfertDirDraw();
				}
				if (SunkenShips == 10)
				{
					return Actions.END_GAME;
				}
				Moves leftPlayer = WhichBoard == BoardSide.Left ? this : Opponent;
				Moves rightPlayer = WhichBoard == BoardSide.Right ? this : Opponent;
				Window.PrintBoard(leftPlayer, rightPlayer);

				Thread.Sleep(1000);
			} while (wasHit);
			return Actions.MISSED;
		}
		private bool ShotAfterCoordDraw()
		{
			if (Opponent[lastX, lastY] != (int)Marker.EmptyField)
			{
				HitShip(true);
				return true;
			}
			else
			{
				Opponent[lastX, lastY] = (int)Marker.AlreadyShot;
				return false;
			}
		}
		private bool ShotAfertDirDraw()
		{
			if (Opponent[lastX, lastY] != (int)Marker.EmptyField)
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
				Opponent[lastX, lastY] = (int)Marker.AlreadyShot;
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
				case int i when (i == (int)Directions.Up || i == (int)Directions.Down):
					{
						dirOfShooting = dirOfShooting == (int)Directions.Up ? (int)Directions.Down : (int)Directions.Up;
						break;
					}
				case int i when (i == (int)Directions.Right || i == (int)Directions.Left):
					{
						dirOfShooting = dirOfShooting == (int)Directions.Right ? (int)Directions.Left : (int)Directions.Right;
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
				if (Opponent[lastX, lastY] > (int)Marker.LastShip || isOutsideBoard)
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
				case int i when (i == (int)Directions.Right || i == (int)Directions.Down):
					{
						int coord = dirOfShooting == (int)Directions.Down ? lastX : lastY;
						if (coord + 1 > LowerEdge)
						{
							return true;
						}
						else
						{
							++coord;
						}
						lastX = dirOfShooting == (int)Directions.Down ? coord : lastX;
						lastY = dirOfShooting == (int)Directions.Down ? lastY : coord;
						break;
					}
				case int i when (i == (int)Directions.Left || i == (int)Directions.Up):
					{
						int coord = dirOfShooting == (int)Directions.Up ? lastX : lastY;
						if (coord - 1 < LeftEdge)
						{
							return true;
						}
						else
						{
							--coord;
						}
						lastX = dirOfShooting == (int)Directions.Up ? coord : lastX;
						lastY = dirOfShooting == (int)Directions.Up ? lastY : coord;
						break;
					}
			}
			return false;
		}
		private void HitShip(bool isAfterDraw)
		{
			int numbOfHitShip = Opponent[lastX, lastY];
			if (Opponent.PlayerShips[numbOfHitShip].HitShip(lastX, lastY))
			{
				if (!isAfterDraw)
				{
					sameDirection = wasHitAfterChoosingDir = wasHitAfterDraw = false;
					for (int i = 0; i < chosenDir.Length; ++i)
					{
						chosenDir[i] = false;
					}
				}
				Opponent.MarkShipNeighborhood(true, numbOfHitShip);
				++SunkenShips;
			}
			else if (!isAfterDraw)
			{
				sameDirection = wasHitAfterChoosingDir = true;
			}
			else
			{
				wasHitAfterDraw = true;
			}
			Opponent.DrawShip(numbOfHitShip);
		}
	} 
}

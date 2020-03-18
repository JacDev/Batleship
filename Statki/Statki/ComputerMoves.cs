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

		public ComputerMoves(BoardSide boardNum, Moves opponent = null) : base(boardNum, Players.Computer, opponent)
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
				PlayerShips[shipNumb - 1] = new Ship(x, y, currSize, shipNumb, isVertical, WhichBoard);
				++shipNumb;
			}
			Board.Instance.ClearMarks();
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
				if (coord + size > Board.Height)
				{
					x = isVertical ? Board.UpperEdge : x;
					y = isVertical ? y : Board.LeftEdge;
				}
				for (int i = 0; i < size; i++)
				{
					int px = isVertical ? i : Board.UpperEdge;
					int py = isVertical ? Board.LeftEdge : i;
					if (Board.Instance[x + px, y + py, WhichBoard] != (int)Marker.EmptyField)
					{
						isFit = false;
						break;
					}
				}
			}
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
						lastX = _x = rnd.Next(Board.PlayerBoardWidth);
						lastY = _y = rnd.Next(Board.Height);
					} while (Board.Instance[_x, _y, OpponentBoard] > (int)Marker.LastShip);
					wasHit = ShotAfterCoordDraw();
				}
				else
				{
					ChooseDir();
					wasHit = ShotAfertDirDraw();
				}
				if (_sunkenShips == 10)
				{
					return Actions.END_GAME;
				}
				Window.Instance.PrintBoard();

				Thread.Sleep(1000);
			} while (wasHit);
			return Actions.MISSED;
		}
		private bool ShotAfterCoordDraw()
		{
			if (Board.Instance[lastX, lastY, OpponentBoard] != (int)Marker.EmptyField)
			{
				HitShip(true);
				return true;
			}
			else
			{
				Board.Instance[lastX, lastY, OpponentBoard] = (int)Marker.AlreadyShot;
				return false;
			}
		}
		private bool ShotAfertDirDraw()
		{
			if (Board.Instance[lastX, lastY, OpponentBoard] != (int)Marker.EmptyField)
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
				Board.Instance[lastX, lastY, OpponentBoard] = (int)Marker.AlreadyShot;
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
				if (Board.Instance[lastX, lastY, OpponentBoard] > (int)Marker.LastShip || isOutsideBoard)
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
						if (coord + 1 > Board.LowerEdge)
							return true;
						else
							coord += 1;
						lastX = dirOfShooting == (int)Directions.Down ? coord : lastX;
						lastY = dirOfShooting == (int)Directions.Down ? lastY : coord;
						break;
					}
				case int i when (i == (int)Directions.Left || i == (int)Directions.Up):
					{
						int coord = dirOfShooting == (int)Directions.Up ? lastX : lastY;
						if (coord - 1 < Board.LeftEdge)
							return true;
						else
							coord -= 1;
						lastX = dirOfShooting == (int)Directions.Up ? coord : lastX;
						lastY = dirOfShooting == (int)Directions.Up ? lastY : coord;
						break;
					}
			}
			return false;
		}
		private void HitShip(bool isAfterDraw)
		{
			int numbOfHitShip = Board.Instance[lastX, lastY, OpponentBoard];
			if (Opponent.PlayerShips[numbOfHitShip - 1].HitShip(lastX, lastY, isComputer: true))
			{
				if (!isAfterDraw)
				{
					sameDirection = wasHitAfterChoosingDir = wasHitAfterDraw = false;
					for (int i = 0; i < chosenDir.Length; ++i)
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

using System;
using System.Threading;
using System.Linq;
using Battleship.Consts;
using Battleship.Interfaces;

namespace Battleship
{
	class Computer : Player
	{
		private enum Directions { Down, Up, Right, Left };
		private int _dirOfShooting;
		private bool[] _chosenDir;
		private int _lastX, _lastY;
		private bool _wasHitAfterDraw;
		private bool _wasHitAfterChoosingDir;
		private bool _sameDirection;

		public Computer(IOutputDevice outputDevice, BoardSide boardNum, Player opponent = null, bool afterLoad = false) : base(boardNum, Players.Computer, opponent, outputDevice)
		{
			if (!afterLoad)
			{
				AddShips();
			}
			_chosenDir = Enumerable.Repeat(false, 4).ToArray();
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
			Board.ClearNearShipMarks();
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
				if (coord + size > BoardSize.Height)
				{
					x = isVertical ? BoardSize.TopEdge : x;
					y = isVertical ? y : BoardSize.LeftEdge;
				}
				for (int i = 0; i < size; i++)
				{
					int px = isVertical ? i : BoardSize.TopEdge;
					int py = isVertical ? BoardSize.LeftEdge : i;
					if (Board.GetField(x + px, y + py) != (int)Marker.EmptyField)
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
			bool wasHit;
			do
			{
				if (!_wasHitAfterDraw)
				{
					Random rnd = new Random();
					do
					{
						_lastX = _x = rnd.Next(BoardSize.Width);
						_lastY = _y = rnd.Next(BoardSize.Height);
					} while (Opponent.Board.GetField(_x, _y) != (int)Marker.EmptyField 
						&& Opponent.Board.GetField(_x, _y) > (int)Marker.LastShip);
					wasHit = ShotAfterCoordDraw();
				}
				else
				{
					ChooseDir();
					wasHit = ShotAfertDirDraw();
				}
				if (SunkenShips == 10)
				{
					return Actions.EndGame;
				}
				Player leftPlayer = WhichBoard == BoardSide.Left ? this : Opponent;
				Player rightPlayer = WhichBoard == BoardSide.Right ? this : Opponent;
				_outputDevice.PrintBoard(leftPlayer.Board, rightPlayer.Board, false);

				Thread.Sleep(1000);
			} while (wasHit);
			return Actions.Missed;
		}
		private bool ShotAfterCoordDraw()
		{
			if (Opponent.Board.GetField(_lastX, _lastY) != (int)Marker.EmptyField)
			{
				HitShip(true);
				return true;
			}
			else
			{
				Opponent.Board.SetField(_lastX, _lastY, (int)Marker.AlreadyShot);
				return false;
			}
		}
		private bool ShotAfertDirDraw()
		{
			if (Opponent.Board.GetField(_lastX, _lastY) != (int)Marker.EmptyField)
			{
				HitShip(false);				
				return true;
			}
			else
			{
				if (_wasHitAfterChoosingDir)
				{ 
					_sameDirection = true;
					ReverseDirection();
					if (_chosenDir[_dirOfShooting])
					{
						_wasHitAfterDraw = false;
					}
					_wasHitAfterChoosingDir = false;
				}
				else
				{
					_sameDirection = false;
					_chosenDir[_dirOfShooting] = true;
				}
				Opponent.Board.SetField(_lastX, _lastY, (int)Marker.AlreadyShot);
				_lastX = _x;
				_lastY = _y;
				return false;
			}
		}
		private void ReverseDirection()
		{
			_chosenDir[_dirOfShooting] = true;
			switch (_dirOfShooting)
			{
				case int i when (i == (int)Directions.Up || i == (int)Directions.Down):
					{
						_dirOfShooting = _dirOfShooting == (int)Directions.Up ? (int)Directions.Down : (int)Directions.Up;
						break;
					}
				case int i when (i == (int)Directions.Right || i == (int)Directions.Left):
					{
						_dirOfShooting = _dirOfShooting == (int)Directions.Right ? (int)Directions.Left : (int)Directions.Right;
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
				if (!_sameDirection)
				{
					do
					{
						_dirOfShooting = new Random().Next(4);
					} while (_chosenDir[_dirOfShooting]);
				}
				isOutsideBoard = GoTowards();
				if (Opponent.Board.GetField(_lastX, _lastY) > (int)Marker.LastShip || isOutsideBoard)
				{
					_lastX = _x;
					_lastY = _y;
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
			switch (_dirOfShooting)
			{
				case int i when (i == (int)Directions.Right || i == (int)Directions.Down):
					{
						int coord = _dirOfShooting == (int)Directions.Down ? _lastX : _lastY;
						if (coord + 1 > BoardSize.BottomEdge)
						{
							return true;
						}
						else
						{
							++coord;
						}
						_lastX = _dirOfShooting == (int)Directions.Down ? coord : _lastX;
						_lastY = _dirOfShooting == (int)Directions.Down ? _lastY : coord;
						break;
					}
				case int i when (i == (int)Directions.Left || i == (int)Directions.Up):
					{
						int coord = _dirOfShooting == (int)Directions.Up ? _lastX : _lastY;
						if (coord - 1 < BoardSize.LeftEdge)
						{
							return true;
						}
						else
						{
							--coord;
						}
						_lastX = _dirOfShooting == (int)Directions.Up ? coord : _lastX;
						_lastY = _dirOfShooting == (int)Directions.Up ? _lastY : coord;
						break;
					}
			}
			return false;
		}
		private void HitShip(bool isAfterDraw)
		{
			int numbOfHitShip = Opponent.Board.GetField(_lastX, _lastY);
			if (Opponent.PlayerShips[numbOfHitShip].HitShip(_lastX, _lastY))
			{
				if (!isAfterDraw)
				{
					_sameDirection = _wasHitAfterChoosingDir = _wasHitAfterDraw = false;
					for (int i = 0; i < _chosenDir.Length; ++i)
					{
						_chosenDir[i] = false;
					}
				}
				Opponent.MarkShipNeighborhood(true, numbOfHitShip);
				++SunkenShips;
			}
			else if (!isAfterDraw)
			{
				_sameDirection = _wasHitAfterChoosingDir = true;
			}
			else
			{
				_wasHitAfterDraw = true;
			}
			Opponent.DrawShip(numbOfHitShip);
		}
	} 
}

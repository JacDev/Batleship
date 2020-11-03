using System;
using System.Threading;
using System.Collections.Generic;
using Battleship.Interfaces;
using Battleship.Consts;

namespace Battleship
{
	public class Person : Player
	{
		class ShotEvent
		{
			public ShotEvent(int coordX, int coordY, int shipNumb, bool wasHit, Player player)
			{
				CoordX = coordX;
				CoordY = coordY;
				ShipNumb = shipNumb;
				WasHit = wasHit;
				_player = player;
			}
			private Player _player;

			public int ShipNumb { get ; }
			public int CoordX { get; }
			public int CoordY { get; }
			public bool WasHit { get; }

			public ShotEvent UndoShot()
			{
				if (WasHit)
				{
					_player.Opponent.PlayerShips[ShipNumb].UndoHit(CoordX, CoordY);					
				}
				else
				{
					_player.Opponent.Board[CoordX, CoordY] = (int)Marker.EmptyField;
				}
				return this;
			}
		}
		private enum LoadedAction : int { BackToMenu = -1, DontShot, Shot, Undo }
		private Stack<ShotEvent> _shots;
		public Person(Window window, BoardSide boardNum, Player opponent = null, bool afterLoad = false) 
			: base(boardNum, Players.Person, opponent, window)
		{
			if (!afterLoad)
			{
				//AddShips();
				AddShipsTest();
			}
			_shots = new Stack<ShotEvent>();
		}
		public override Actions Shoot()
		{
			bool wasHit;
			do
			{
				wasHit = false;
				int selectedField;
				bool canShoot;
				LoadedAction shoot;
				do
				{
					selectedField = Opponent.Board[_x, _y];
					canShoot = CanShoot(selectedField);

					Player leftPlayer = WhichBoard == BoardSide.Left ? this : Opponent;
					Player rightPlayer = WhichBoard == BoardSide.Right ? this : Opponent;
					_window.PrintBoard(leftPlayer.Board, rightPlayer.Board);

					Opponent.Board[_x, _y] = selectedField;

					shoot = ReadKeyforShoot();
					if (shoot == LoadedAction.BackToMenu)
					{
						return Actions.BACK_TO_MENU;
					}
					else if (shoot == LoadedAction.Undo)
					{
						UndoShot();
					}
				} while (shoot != LoadedAction.Shot || !canShoot);

				if(MarkField(selectedField, ref wasHit))
				{
					return Actions.END_GAME;
				}
				_shots.Push(new ShotEvent(_x, _y, selectedField, wasHit, this));
			} while (wasHit);
			_shots.Clear();
			return Actions.MISSED;
		}
		private bool CanShoot(int selectedField)
		{
			if (selectedField >= (int)Marker.FirstShip && selectedField <= (int)Marker.LastShip || selectedField == (int)Marker.EmptyField)
			{
				Opponent.Board[_x, _y] = (int)Marker.ChosenToShoot;
				return true;
			}
			else
			{
				Opponent.Board[_x, _y] = (int)Marker.CannotShoot;
				return false;
			}
		}
		private bool MarkField(int selectedField, ref bool wasHit)
		{
			if (selectedField >= (int)Marker.FirstShip && selectedField <= (int)Marker.LastShip)
			{
				if (Opponent.PlayerShips[selectedField].HitShip(_x, _y))
				{
					++SunkenShips;
					Opponent.MarkShipNeighborhood(true, selectedField);
					if (SunkenShips == 10)
					{
						return true;
					}
				}
				Opponent.DrawShip(selectedField);
				wasHit = true;
			}
			else
			{
				Opponent.Board[_x, _y] = (int)Marker.AlreadyShot;
			}
			return false;
		}
		private void UndoAdded(int shipNumb, bool deleteAll)
		{
			UndoAddedShip(shipNumb);
			if (!deleteAll)
			{
				Board.ClearBoard();	
			}
			else
			{
				DrawShip(shipNumb);
			}
		}
		public void UndoAddedShip(int shipNumb)
		{
			for (int i = 0; i < PlayerShips[shipNumb].Size; ++i)
			{
				(int coordX, int coordY) = PlayerShips[shipNumb][i];
				Board[coordX, coordY] = (int)Marker.EmptyField;
			}
			for (int i = 0; i < PlayerShips[shipNumb].Size; ++i)
			{
				(int coordX, int coordY) = PlayerShips[shipNumb][i];
				for (int j = -1; j < 2; ++j)
				{
					for (int k = -1; k < 2; ++k)
						Board[coordX, coordY] = (int)Marker.EmptyField;
				}
			}
		}
		private void UndoShot()
		{
			if(_shots.Count != 0)
			{
				ShotEvent shot = _shots.Pop().UndoShot();
				if (shot.WasHit)
				{
					Opponent.UndoHit(shot.CoordX, shot.CoordY, shot.ShipNumb);
				}
			}
			else
			{
				Console.WriteLine("You can't undo the game.");
			}
		}
		private LoadedAction ReadKeyforShoot()
		{
			bool isLoaded = false;
			while (!isLoaded)
			{
				Keys key = _window.ReadKey();
				switch (key)
				{
					case Keys.Down:
						{
							if (++_x > BoardSize.BottomEdge)
							{ 
								_x = BoardSize.TopEdge; 
							}
							isLoaded = true;
							break;
						}
					case Keys.Up:
						{
							if (--_x < BoardSize.TopEdge)
							{ 
								_x = BoardSize.BottomEdge;
							}
							isLoaded = true;
							break;
						}
					case Keys.Right:
						{
							if (++_y > BoardSize.RightEdge)
							{ 
								_y = BoardSize.LeftEdge; 
							}
							isLoaded = true;
							break;
						}
					case Keys.Left:
						{
							if (--_y < BoardSize.LeftEdge)
							{ 
								_y = BoardSize.RightEdge;
							}
							isLoaded = true;
							break;
						}
					case Keys.Undo:
						{
							Thread.Sleep(100);
							return LoadedAction.Undo;
						}
					case Keys.Escape:
						{
							Thread.Sleep(100);
							return LoadedAction.BackToMenu;
						}
					case Keys.Enter:
						{
							Thread.Sleep(100);
							return LoadedAction.Shot;
						}
					default:
						break;
				}
			}
			Thread.Sleep(100);
			return LoadedAction.DontShot;
		}

		private void AddShipsTest()
		{
			for (int i = 0; i < 5; i++)
			{
				PlayerShips[i] = new Ship(2 * i ,0, 5, i, false);
				DrawShip(i);
				MarkShipNeighborhood(false, i);
			}
			for (int i = 5; i < 10; i++)
			{
				PlayerShips[i] = new Ship(2 * (i - 5), 6, 4, i, false);
				DrawShip(i);
				MarkShipNeighborhood(false, i);
			}
			Board.ClearNearShipMarks();
		}
		protected override void AddShips()
		{
			int shipNumb = 0;
			int x = 0, y = 0;

			for (; shipNumb < _shipSize.Length; ++shipNumb)
			{
				int currSize = _shipSize[shipNumb];
				bool couldAdd = false;
				bool isFit = true;
				bool isVertical = false;
				y = (y + currSize > BoardSize.Width) ? BoardSize.LeftEdge : y;

				do
				{
					isFit = IsFit(x, y, currSize, isVertical);
					couldAdd = ReadKey(ref x, ref y, currSize, ref isVertical, ref shipNumb);
					currSize = _shipSize[shipNumb]; //when shipNumb change (when undo), size of ship can be bigger and ship can go outside
					IsInBoard(ref x, ref y, currSize, isVertical);
				} while (!couldAdd || !isFit);
				PlayerShips[shipNumb] = new Ship(x, y, currSize, shipNumb, isVertical);
				DrawShip(shipNumb);
			}
			Board.ClearNearShipMarks();
		}
		private void IsInBoard(ref int x, ref int y, int size, bool isVertical)
		{
			int coord1 = isVertical ? x : y;
			int coord2 = isVertical ? y : x;
			if (coord2 > BoardSize.BottomEdge)
				coord2 = BoardSize.TopEdge;
			else if (coord1 + size > BoardSize.Height)
				coord1 = BoardSize.TopEdge;
			else if (coord2 < BoardSize.TopEdge)
				coord2 = BoardSize.BottomEdge;
			else if (coord1 < BoardSize.TopEdge)
				coord1 = BoardSize.Height - size;
			x = isVertical ? coord1 : coord2;
			y = isVertical ? coord2 : coord1;
		}
		private bool ReadKey(ref int x, ref int y, int currSize, ref bool isVertical, ref int shipNumb)
		{
			Keys key = _window.ReadKey();
			switch (key)
			{
				case Keys.Down:
					x++;
					break;
				case Keys.Up:
					x--;
					break;
				case Keys.Right:
					y++;
					break;
				case Keys.Left:
					y--;
					break;
				case Keys.Rotate:
					RotateShip(ref x, ref y, currSize, ref isVertical);
					break;
				case Keys.Undo:
				case Keys.Clear:
					{
						do
						{
							if (shipNumb > 0)
							{
								PlayerShips[shipNumb] = null;
								UndoAdded(shipNumb, key == Keys.Clear);
								--shipNumb;
							}
						} while (shipNumb > 0 && key != Keys.Undo);
						break;
					}
				case Keys.Enter:
					return true;

			}
			return false;
		}
		private void RotateShip(ref int x, ref int y, int size, ref bool isVertical)
		{
			int coord = isVertical ? y : x;
			if (coord + size > BoardSize.Height)
			{
				coord -= BoardSize.Height + size;
			}
			x = isVertical ? x : coord;
			y = isVertical ? coord : y;
			isVertical = !isVertical;
		}
		private bool IsFit(int x, int y, int currSize, bool isVertical)
		{
			int[] coveringFields = new int[currSize];
			bool isFit = true;
			for (int i = 0, j = 0; i < currSize && j < currSize;)
			{
				if (Board[x + j, y + i] == (int)Marker.EmptyField)
					Board[x + j, y + i] = (int)Marker.ChosenToAdd;
				else
				{
					int changedFiled = isVertical ? j : i;
					coveringFields[changedFiled] = Board[x + j, y + i];
					Board[x + j, y + i] = (int)Marker.CannotAdd;
					isFit = false;

				}
				if (isVertical) ++j;
				else ++i;
			}
			Player leftPlayer = WhichBoard == BoardSide.Left ? this : Opponent;
			Player rightPlayer = WhichBoard == BoardSide.Right ? this : Opponent;
			_window.PrintBoard(leftPlayer.Board, rightPlayer.Board);
			for (int i = 0, j = 0; i < currSize && j < currSize;)
			{
				int changedFiled = isVertical ? j : i;
				int previousVal = Board[x + j, y + i] == (int)Marker.CannotAdd ? coveringFields[changedFiled] : (int)Marker.EmptyField;
				Board[x + j, y + i] = previousVal;
				if (isVertical)
				{
					++j;
				}
				else
				{
					++i;
				}
			}
			return isFit;
		}
	}
}
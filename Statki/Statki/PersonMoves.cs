using System;
using System.Threading;
using System.Collections.Generic;

namespace Statki
{
	class PersonMoves : Moves
	{
		class ShotEvent
		{
			public ShotEvent(int coordX, int coordY, int shipNumb, bool wasHit, Moves player)
			{
				_coordX = coordX;
				_coordY = coordY;
				_shipNumb = shipNumb;
				_wasHit = wasHit;
				_player = player;
			}
			private int _shipNumb;
			private int _coordX;
			private int _coordY;
			private bool _wasHit;
			private Moves _player;
			public void UndoShot()
			{
				if (_wasHit)
				{
					_player.Opponent.PlayerShips[_shipNumb].UndoHit(_coordX, _coordY);
				}
				else
				{
					Board.Instance[_coordX, _coordY, _player.OpponentBoard] = (int)Marker.EmptyField;
				}
			}
		}
		private enum LoadedAction : int { BackToMenu = -1, DontShot, Shot, Undo }
		private Stack<ShotEvent> shots;
		public PersonMoves(BoardSide boardNum, Moves opponent = null, bool afterLoad = false) : base(boardNum, Players.Person, opponent)
		{
			if (!afterLoad)
			{
				//AddShips();
				AddShipsTest();
			}
			shots = new Stack<ShotEvent>();
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
					selectedField = Board.Instance[_x, _y, OpponentBoard];
					canShoot = CanShoot(selectedField);

					Window.Instance.PrintBoard();
					Board.Instance[_x, _y, OpponentBoard] = selectedField;

					shoot = readKeyforShoot();
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
				shots.Push(new ShotEvent(_x, _y, selectedField - 1, wasHit, this));
			} while (wasHit);
			shots.Clear();
			return Actions.MISSED;
		}
		private bool CanShoot(int selectedField)
		{
			if (selectedField >= (int)Marker.FirstShip && selectedField <= (int)Marker.LastShip || selectedField == (int)Marker.EmptyField)
			{
				Board.Instance[_x, _y, OpponentBoard] = (int)Marker.ChosenToShoot;
				return true;
			}
			else
			{
				Board.Instance[_x, _y, OpponentBoard] = (int)Marker.CannotShoot;
				return false;
			}
		}
		private bool MarkField(int selectedField, ref bool wasHit)
		{
			if (selectedField >= (int)Marker.FirstShip && selectedField <= (int)Marker.LastShip)
			{
				if (Opponent.PlayerShips[selectedField - 1].HitShip(_x, _y))
				{
					++SunkenShips;
					if (SunkenShips == 10)
					{
						return true;
					}
				}
				wasHit = true;
			}
			else
			{
				Board.Instance[_x, _y, OpponentBoard] = (int)Marker.AlreadyShot;
			}
			return false;
		}
		private void UndoAdded(int shipNumb, bool deleteAll)
		{
			PlayerShips[shipNumb - 1].UndoAdded();
			if (!deleteAll)
			{
				for (int i = 0; i < shipNumb - 1; ++i)
				{
					PlayerShips[i].MarkNeighborhood(false);
				}
			}
		}
		private void UndoShot()
		{
			if(shots.Count != 0)
			{
				shots.Pop().UndoShot();
			}
			else
			{
				Console.WriteLine("You can't undo the game.");//
			}
		}
		private LoadedAction readKeyforShoot()
		{
			bool isLoaded = false;
			while (!isLoaded)
			{
				Keys key = Window.Instance.ReadKey();
				switch (key)
				{
					case Keys.Down:
						{
							if (++_x > Board.LowerEdge)
							{ 
								_x = Board.UpperEdge; 
							}
							isLoaded = true;
							break;
						}
					case Keys.Up:
						{
							if (--_x < Board.UpperEdge)
							{ 
								_x = Board.LowerEdge;
							}
							isLoaded = true;
							break;
						}
					case Keys.Right:
						{
							if (++_y > Board.RightEdge)
							{ 
								_y = Board.LeftEdge; 
							}
							isLoaded = true;
							break;
						}
					case Keys.Left:
						{
							if (--_y < Board.LeftEdge)
							{ 
								_y = Board.RightEdge;
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
			for (int i = 1; i < 6; i++)
			{
				PlayerShips[i - 1] = new Ship(2 * (i - 1),0, 5, i, false, WhichBoard);
			}
			for (int i = 6; i <= 10; i++)
			{
				PlayerShips[i - 1] = new Ship(2 * (i - 6), 6, 4, i, false, WhichBoard);
			}
			Board.Instance.ClearMarks();
		}
		protected override void AddShips()
		{
			int shipNumb = 1;
			int x = 0, y = 0;

			for (; shipNumb <= _shipSize.Length; ++shipNumb)
			{
				int currSize = _shipSize[shipNumb - 1];
				bool couldAdd = false;
				bool isFit = true;
				bool isVertical = false;
				y = (y + currSize > Board.PlayerBoardWidth) ? Board.LeftEdge : y;

				do
				{
					isFit = IsFit(x, y, currSize, isVertical, WhichBoard);
					couldAdd = ReadKey(ref x, ref y, currSize, ref isVertical, ref shipNumb);
					currSize = _shipSize[shipNumb - 1]; //when shipNumb change, size of ship can be bigger and ship can go outside
					IsInBoard(ref x, ref y, currSize, isVertical);
				} while (!couldAdd || !isFit);
				PlayerShips[shipNumb - 1] = new Ship(x, y, currSize, shipNumb, isVertical, WhichBoard);
			}
			Board.Instance.ClearMarks();
		}
		private void IsInBoard(ref int x, ref int y, int size, bool isVertical)
		{
			int coord1 = isVertical ? x : y;
			int coord2 = isVertical ? y : x;
			if (coord2 > Board.LowerEdge)
				coord2 = Board.UpperEdge;
			else if (coord1 + size > Board.Height)
				coord1 = Board.UpperEdge;
			else if (coord2 < Board.UpperEdge)
				coord2 = Board.LowerEdge;
			else if (coord1 < Board.UpperEdge)
				coord1 = Board.Height - size;
			x = isVertical ? coord1 : coord2;
			y = isVertical ? coord2 : coord1;
		}
		private bool ReadKey(ref int x, ref int y, int currSize, ref bool isVertical, ref int shipNumb)
		{
			Keys key = Window.Instance.ReadKey();
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
							if (shipNumb > 1)
							{
								PlayerShips[shipNumb - 1] = null;
								UndoAdded(shipNumb - 1, key == Keys.Clear);
								--shipNumb;
							}
						} while (shipNumb > 1 && key != Keys.Undo);
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
			if (coord + size > Board.Height)
			{
				coord -= Board.Height + size;
			}
			x = isVertical ? x : coord;
			y = isVertical ? coord : y;
			isVertical = !isVertical;
		}
		private bool IsFit(int x, int y, int currSize, bool isVertical, BoardSide _whichBoard)
		{
			int[] coveringFields = new int[currSize];
			bool isFit = true;
			for (int i = 0, j = 0; i < currSize && j < currSize;)
			{
				if (Board.Instance[x + j, y + i, _whichBoard] == (int)Marker.EmptyField)
					Board.Instance[x + j, y + i, _whichBoard] = (int)Marker.ChosenToAdd;
				else
				{
					int changedFiled = isVertical ? j : i;
					coveringFields[changedFiled] = Board.Instance[x + j, y + i, _whichBoard];
					Board.Instance[x + j, y + i, _whichBoard] = (int)Marker.CannotAdd;
					isFit = false;

				}
				if (isVertical) ++j;
				else ++i;
			}
			Window.Instance.PrintBoard();
			for (int i = 0, j = 0; i < currSize && j < currSize;)
			{
				int changedFiled = isVertical ? j : i;
				int previousVal = Board.Instance[x + j, y + i, _whichBoard] == (int)Marker.CannotAdd ? coveringFields[changedFiled] : (int)Marker.EmptyField;
				Board.Instance[x + j, y + i, _whichBoard] = previousVal;
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
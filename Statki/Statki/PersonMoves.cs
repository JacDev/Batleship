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
				CoordX = coordX;
				CoordY = coordY;
				ShipNumb = shipNumb;
				WasHit = wasHit;
				_player = player;
			}
			private Moves _player;

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
					_player.Opponent[CoordX, CoordY] = (int)Marker.EmptyField;
				}
				return this;
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
					selectedField = Opponent[_x, _y];
					canShoot = CanShoot(selectedField);

					Moves leftPlayer = WhichBoard == BoardSide.Left ? this : Opponent;
					Moves rightPlayer = WhichBoard == BoardSide.Right ? this : Opponent;
					Window.PrintBoard(leftPlayer, rightPlayer);

					Opponent[_x, _y] = selectedField;

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
				shots.Push(new ShotEvent(_x, _y, selectedField, wasHit, this));
			} while (wasHit);
			shots.Clear();
			return Actions.MISSED;
		}
		private bool CanShoot(int selectedField)
		{
			if (selectedField >= (int)Marker.FirstShip && selectedField <= (int)Marker.LastShip || selectedField == (int)Marker.EmptyField)
			{
				Opponent[_x, _y] = (int)Marker.ChosenToShoot;
				return true;
			}
			else
			{
				Opponent[_x, _y] = (int)Marker.CannotShoot;
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
				Opponent[_x, _y] = (int)Marker.AlreadyShot;
			}
			return false;
		}
		private void UndoAdded(int shipNumb, bool deleteAll)
		{
			UndoAddedShip(shipNumb);
			if (!deleteAll)
			{
				ClearBoard();	
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
				this[coordX, coordY] = (int)Marker.EmptyField;
			}
			for (int i = 0; i < PlayerShips[shipNumb].Size; ++i)
			{
				(int coordX, int coordY) = PlayerShips[shipNumb][i];
				for (int j = -1; j < 2; ++j)
				{
					for (int k = -1; k < 2; ++k)
						this[coordX, coordY] = (int)Marker.EmptyField;
				}
			}
		}
		private void UndoShot()
		{
			if(shots.Count != 0)
			{
				ShotEvent shot = shots.Pop().UndoShot();
				if (shot.WasHit)
				{
					Opponent.UndoHit(shot.CoordX, shot.CoordY, shot.ShipNumb);
				}
			}
			else
			{
				Console.WriteLine("You can't undo the game.");//
			}
		}
		private LoadedAction ReadKeyforShoot()
		{
			bool isLoaded = false;
			while (!isLoaded)
			{
				Keys key = Window.ReadKey();
				switch (key)
				{
					case Keys.Down:
						{
							if (++_x > LowerEdge)
							{ 
								_x = UpperEdge; 
							}
							isLoaded = true;
							break;
						}
					case Keys.Up:
						{
							if (--_x < UpperEdge)
							{ 
								_x = LowerEdge;
							}
							isLoaded = true;
							break;
						}
					case Keys.Right:
						{
							if (++_y > RightEdge)
							{ 
								_y = LeftEdge; 
							}
							isLoaded = true;
							break;
						}
					case Keys.Left:
						{
							if (--_y < LeftEdge)
							{ 
								_y = RightEdge;
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
			ClearNearShipMarks();
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
				y = (y + currSize > Width) ? LeftEdge : y;

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
			ClearNearShipMarks();
		}
		private void IsInBoard(ref int x, ref int y, int size, bool isVertical)
		{
			int coord1 = isVertical ? x : y;
			int coord2 = isVertical ? y : x;
			if (coord2 > LowerEdge)
				coord2 = UpperEdge;
			else if (coord1 + size > Height)
				coord1 = UpperEdge;
			else if (coord2 < UpperEdge)
				coord2 = LowerEdge;
			else if (coord1 < UpperEdge)
				coord1 = Height - size;
			x = isVertical ? coord1 : coord2;
			y = isVertical ? coord2 : coord1;
		}
		private bool ReadKey(ref int x, ref int y, int currSize, ref bool isVertical, ref int shipNumb)
		{
			Keys key = Window.ReadKey();
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
			if (coord + size > Height)
			{
				coord -= Height + size;
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
				if (this[x + j, y + i] == (int)Marker.EmptyField)
					this[x + j, y + i] = (int)Marker.ChosenToAdd;
				else
				{
					int changedFiled = isVertical ? j : i;
					coveringFields[changedFiled] = this[x + j, y + i];
					this[x + j, y + i] = (int)Marker.CannotAdd;
					isFit = false;

				}
				if (isVertical) ++j;
				else ++i;
			}
			Moves leftPlayer = WhichBoard == BoardSide.Left ? this : Opponent;
			Moves rightPlayer = WhichBoard == BoardSide.Right ? this : Opponent;
			Window.PrintBoard(leftPlayer, rightPlayer);
			for (int i = 0, j = 0; i < currSize && j < currSize;)
			{
				int changedFiled = isVertical ? j : i;
				int previousVal = this[x + j, y + i] == (int)Marker.CannotAdd ? coveringFields[changedFiled] : (int)Marker.EmptyField;
				this[x + j, y + i] = previousVal;
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
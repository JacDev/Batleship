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
					selectedField = Board.Instance[_x, _y, OpponentBoard];
					if (selectedField >= 0 && selectedField <= 10)
					{
						Board.Instance[_x, _y, OpponentBoard] = (int)Marker.CHOSEN_TO_SHOOT;
						canShoot = true;
					}
					else
					{
						Board.Instance[_x, _y, OpponentBoard] = (int)Marker.CANNOT_SHOOT;
					}
					Window.Instance.PrintBoard();
					Board.Instance[_x, _y, OpponentBoard] = selectedField;

					shoot = readKeyforShoot();
					if (shoot == -1)
					{ //back to menu
						return false;
					}
				} while (shoot != 1 || !canShoot);

				if (selectedField > 0 && selectedField <= 10)
				{
					if (Opponent.GetShip(selectedField - 1).HitShip(_x, _y))
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
					Board.Instance[_x, _y, OpponentBoard] = (int)Marker.ALREADY_SHOT;
				}
				Events.Instance.AddShotEvent(_x, _y, selectedField - 1, wasHit, this);
			} while (wasHit);
			return true;
		}

		private int readKeyforShoot()
		{
			bool isLoaded = false;
			while (!isLoaded)
			{
				Keys key = Window.Instance.ReadKey();
				switch (key)
				{
					case Keys.DOWN:
						{
							if (++_x > Board.HEIGHT - 1)
							{ _x = 0; }
							isLoaded = true;
							break;
						}
					case Keys.UP:
						{
							if (--_x < 0)
							{ _x = Board.HEIGHT - 1; }
							isLoaded = true;
							break;
						}
					case Keys.RIGHT:
						{
							if (++_y > Board.PLAYER_BOARD_WIDTH - 1)
							{ _y = 0; }
							isLoaded = true;
							break;
						}
					case Keys.LEFT:
						{
							if (--_y < 0)
							{ _y = Board.PLAYER_BOARD_WIDTH - 1; }
							isLoaded = true;
							break;
						}
					case Keys.UNDO:
						{
							Events.Instance.UndoShotEvent();
							isLoaded = true;
							break;
						}
					case Keys.ESCAPE:
						{
							Thread.Sleep(100);
							return -1;
						}
					case Keys.ENTER:
						{
							Thread.Sleep(100);
							return 1;
						}
					default:
						break;
				}
			}
			Thread.Sleep(100);
			return 0;
		}
		private void AddShipsTest()
		{
			for (int i = 1; i < 6; i++)
			{
				PlayerShips[i - 1] = new Ship(2 * (i - 1),0, 5, i, false, _whichBoard);
			}
			for (int i = 6; i <= 10; i++)
			{
				PlayerShips[i - 1] = new Ship(2 * (i - 6), 6, 4, i, false, _whichBoard);
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
				y = (y + currSize > Board.PLAYER_BOARD_WIDTH) ? 0 : y;

				do
				{
					isFit = IsFit(x, y, currSize, isVertical, _whichBoard);
					couldAdd = ReadKey(ref x, ref y, currSize, ref isVertical, ref shipNumb);
					currSize = _shipSize[shipNumb - 1]; //when shipNumb change, size of ship can be bigger and go outside
					IsInBoard(ref x, ref y, currSize, isVertical);
				} while (!couldAdd || !isFit);
				PlayerShips[shipNumb - 1] = new Ship(x, y, currSize, shipNumb, isVertical, _whichBoard);
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
		private bool ReadKey(ref int x, ref int y, int currSize, ref bool isVertical, ref int shipNumb)
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
				case Keys.UNDO:
				case Keys.CLEAR:
					{
						do
						{
							if (shipNumb > 1)
							{
								PlayerShips[shipNumb - 1] = null;
								UndoAdded(shipNumb - 1, key == Keys.CLEAR);
								--shipNumb;
							}
						} while (shipNumb > 1 && key != Keys.UNDO);
						break;
					}
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
				coord -= Board.HEIGHT + size;
			}
			x = isVertical ? x : coord;
			y = isVertical ? coord : y;
			isVertical = !isVertical;
		}
		private bool IsFit(int x, int y, int currSize, bool isVertical, bool _whichBoard)
		{
			int[] coveringFields = new int[currSize];
			bool isFit = true;
			for (int i = 0, j = 0; i < currSize && j < currSize;)
			{
				if (Board.Instance[x + j, y + i, _whichBoard] == (int)Marker.EMPTY_FIELD)
					Board.Instance[x + j, y + i, _whichBoard] = (int)Marker.CHOSEN_TO_ADD;
				else
				{
					int changedFiled = isVertical ? j : i;
					coveringFields[changedFiled] = Board.Instance[x + j, y + i, _whichBoard];
					Board.Instance[x + j, y + i, _whichBoard] = (int)Marker.CANNOT_ADD;
					isFit = false;

				}
				if (isVertical) ++j;
				else ++i;
			}
			Window.Instance.PrintBoard();
			for (int i = 0, j = 0; i < currSize && j < currSize;)
			{
				int changedFiled = isVertical ? j : i;
				int previousVal = Board.Instance[x + j, y + i, _whichBoard] == (int)Marker.CANNOT_ADD ? coveringFields[changedFiled] : (int)Marker.EMPTY_FIELD;
				Board.Instance[x + j, y + i, _whichBoard] = previousVal;
				if (isVertical) ++j;
				else ++i;
			}
			return isFit;
		}
	}
}

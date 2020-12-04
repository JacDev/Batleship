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

			public int ShipNumb { get; }
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
					_player.Opponent.Board.SetField(CoordX, CoordY, (int)Marker.EmptyField);
				}
				return this;
			}
		}
		private readonly IInputDevice _inputDevice;
		private enum LoadedAction : int { BackToMenu = -1, DontShot, Shot, Undo }
		private Stack<ShotEvent> _shots;
		private int _sizeOfCurrentAddingShip;
		private int _numbOfCurrentAddingShip;
		private int _coordXAddingShip = 0;
		private int _coordYAddingShip = 0;
		private bool _isVerticalAddingShip = false;
		private bool _isFitAddingShip = true;
		private int[] _coveringFieldsAddingShip;
		public Person(IOutputDevice outputDevice,
			BoardSide boardNum,
			IInputDevice inputDevice,
			Player opponent = null,
			bool afterLoad = false)
			: base(boardNum, Players.Person, opponent, outputDevice)
		{
			_shots = new Stack<ShotEvent>();
			_inputDevice = inputDevice;
			if (!afterLoad)
			{
				AddShips();
				//AddShipsTest();
			}
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
					selectedField = Opponent.Board.GetField(_x, _y);
					canShoot = CanShoot(selectedField);
					if (!canShoot && _outputDevice.GetBottomMessage() ==-1)
					{
						_outputDevice.SetBottomMessage(6);
					}

					Player leftPlayer = WhichBoard == BoardSide.Left ? this : Opponent;
					Player rightPlayer = WhichBoard == BoardSide.Right ? this : Opponent;

					_outputDevice.PrintBoard(leftPlayer.Board, rightPlayer.Board, false);
					_outputDevice.SetBottomMessage(-1);

					Opponent.Board.SetField(_x, _y, selectedField);

					shoot = ReadKeyforShoot();
					if (shoot == LoadedAction.BackToMenu)
					{
						return Actions.BackToMenu;
					}
					else if (shoot == LoadedAction.Undo)
					{
						UndoShot();
					}
				} while (shoot != LoadedAction.Shot || !canShoot);
				int result = MarkField(selectedField, ref wasHit);
				if (result == -1)
				{
					return Actions.EndGame;
				}
				else if (result == 1)
				{
					_outputDevice.SetBottomMessage(0);
				}
				else if (result == 2)
				{
					_outputDevice.SetBottomMessage(3);
				}
				_shots.Push(new ShotEvent(_x, _y, selectedField, wasHit, this));
			} while (wasHit);
			_shots.Clear();
			return Actions.Missed;
		}
		private bool CanShoot(int selectedField)
		{
			if (selectedField >= (int)Marker.FirstShip && selectedField <= (int)Marker.LastShip || selectedField == (int)Marker.EmptyField)
			{
				Opponent.Board.SetField(_x, _y, (int)Marker.ChosenToShoot);
				return true;
			}
			else
			{
				Opponent.Board.SetField(_x, _y, (int)Marker.CannotShoot);
				return false;
			}
		}
		private int MarkField(int selectedField, ref bool wasHit)
		{
			if (selectedField >= (int)Marker.FirstShip && selectedField <= (int)Marker.LastShip)
			{
				if (Opponent.PlayerShips[selectedField].HitShip(_x, _y))
				{
					++SunkenShips;
					Opponent.SinkShip(selectedField);
					if (SunkenShips == 10)
					{
						return -1;
					}
					else
					{
						Opponent.DrawShip(selectedField);
						wasHit = true;
						return 2;
					}
				}
				Opponent.DrawShip(selectedField);
				wasHit = true;
				return 1;
			}
			else
			{
				Opponent.Board.SetField(_x, _y, (int)Marker.AlreadyShot);
			}
			return 0;
		}
		private void UndoAdded(int shipNumb, bool deleteAll)
		{
			UndoAddedShip(shipNumb);
			Board.ClearBoard();
			if (!deleteAll)
			{
				for (int i = 0; i < shipNumb; ++i)
				{
					DrawShip(i);
					MarkShipNeighborhood(false, i);
				}
			}
		}
		public void UndoAddedShip(int shipNumb)
		{
			for (int i = 0; i < PlayerShips[shipNumb].Size; ++i)
			{
				(int coordX, int coordY) = PlayerShips[shipNumb][i];
				Board.SetField(coordX, coordY, (int)Marker.EmptyField);
			}
			for (int i = 0; i < PlayerShips[shipNumb].Size; ++i)
			{
				(int coordX, int coordY) = PlayerShips[shipNumb][i];
				for (int j = -1; j < 2; ++j)
				{
					for (int k = -1; k < 2; ++k)
						Board.SetField(coordX, coordY, (int)Marker.EmptyField);
				}
			}
		}
		private void UndoShot()
		{
			if (_shots.Count != 0)
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
				Keys key = _inputDevice.ReadKey();
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
			_inputDevice.ClearStram();
			return LoadedAction.DontShot;
		}
		private void AddShipsTest()
		{
			for (int i = 0; i < 5; i++)
			{
				PlayerShips[i] = new Ship(2 * i, 0, 5, i, false);
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
			_numbOfCurrentAddingShip = 0;

			for (; _numbOfCurrentAddingShip < _shipSize.Length; ++_numbOfCurrentAddingShip)
			{
				_sizeOfCurrentAddingShip = _shipSize[_numbOfCurrentAddingShip];
				bool wantAdd;
				_coordYAddingShip = (_coordYAddingShip + _sizeOfCurrentAddingShip > BoardSize.Width) ? BoardSize.LeftEdge : _coordYAddingShip;

				do
				{
					_isFitAddingShip = true;
					PrepareToAdd();
					if (!_isFitAddingShip)
					{
						_outputDevice.SetBottomMessage(0);
					}

					Player leftPlayer = WhichBoard == BoardSide.Left ? this : Opponent;
					Player rightPlayer = WhichBoard == BoardSide.Right ? this : Opponent;
					_outputDevice.PrintBoard(leftPlayer.Board, rightPlayer.Board, true);

					_outputDevice.SetBottomMessage(-1);

					wantAdd = WantAdd();
					_sizeOfCurrentAddingShip = _shipSize[_numbOfCurrentAddingShip];
					IsInBoard();

				} while (!wantAdd || !_isFitAddingShip);

				CreateShip();
			}
			Board.ClearNearShipMarks();
		}
		private void CreateShip()
		{
			PlayerShips[_numbOfCurrentAddingShip] = new Ship(_coordXAddingShip, _coordYAddingShip, _sizeOfCurrentAddingShip, _numbOfCurrentAddingShip, _isVerticalAddingShip);
			DrawShip(_numbOfCurrentAddingShip);
			MarkShipNeighborhood(false, _numbOfCurrentAddingShip);
		}
		private void IsInBoard()
		{
			int coord1 = _isVerticalAddingShip ? _coordXAddingShip : _coordYAddingShip;
			int coord2 = _isVerticalAddingShip ? _coordYAddingShip : _coordXAddingShip;
			if (coord2 > BoardSize.BottomEdge)
				coord2 = BoardSize.TopEdge;
			else if (coord1 + _sizeOfCurrentAddingShip > BoardSize.Height)
				coord1 = BoardSize.TopEdge;
			else if (coord2 < BoardSize.TopEdge)
				coord2 = BoardSize.BottomEdge;
			else if (coord1 < BoardSize.TopEdge)
				coord1 = BoardSize.Height - _sizeOfCurrentAddingShip;
			_coordXAddingShip = _isVerticalAddingShip ? coord1 : coord2;
			_coordYAddingShip = _isVerticalAddingShip ? coord2 : coord1;
		}
		private bool ReadKey()
		{
			Keys key = _inputDevice.ReadKey();
			switch (key)
			{
				case Keys.Down:
					_coordXAddingShip++;
					break;
				case Keys.Up:
					_coordXAddingShip--;
					break;
				case Keys.Right:
					_coordYAddingShip++;
					break;
				case Keys.Left:
					_coordYAddingShip--;
					break;
				case Keys.Rotate:
					RotateShip();
					break;
				case Keys.Undo:
				case Keys.Clear:
					{
						do
						{
							if (_numbOfCurrentAddingShip > 0)
							{
								UndoAdded(_numbOfCurrentAddingShip - 1, key == Keys.Clear);
								PlayerShips[_numbOfCurrentAddingShip - 1] = null;
								--_numbOfCurrentAddingShip;
							}
						} while (_numbOfCurrentAddingShip > 0 && key != Keys.Undo);
						break;
					}
				case Keys.Enter:
					return true;

			}
			_inputDevice.ClearStram();
			return false;
		}
		private void RotateShip()
		{
			int coord = _isVerticalAddingShip ? _coordYAddingShip : _coordXAddingShip;
			if (coord + _sizeOfCurrentAddingShip > BoardSize.Height)
			{
				coord -= BoardSize.Height + _sizeOfCurrentAddingShip;
			}
			_coordXAddingShip = _isVerticalAddingShip ? _coordXAddingShip : coord;
			_coordYAddingShip = _isVerticalAddingShip ? coord : _coordYAddingShip;
			_isVerticalAddingShip = !_isVerticalAddingShip;
		}
		public void PrepareToAdd()
		{
			_coveringFieldsAddingShip = new int[_sizeOfCurrentAddingShip];
			for (int i = 0, j = 0; i < _sizeOfCurrentAddingShip && j < _sizeOfCurrentAddingShip;)
			{
				if (Board.GetField(_coordXAddingShip + j, _coordYAddingShip + i) == (int)Marker.EmptyField)
					Board.SetField(_coordXAddingShip + j, _coordYAddingShip + i, (int)Marker.ChosenToAdd);
				else
				{
					int changedFiled = _isVerticalAddingShip ? j : i;
					_coveringFieldsAddingShip[changedFiled] = Board.GetField(_coordXAddingShip + j, _coordYAddingShip + i);
					Board.SetField(_coordXAddingShip + j, _coordYAddingShip + i, (int)Marker.CannotAdd);
					_isFitAddingShip = false;
				}
				if (_isVerticalAddingShip) ++j;
				else ++i;
			}
		}
		private bool WantAdd()
		{
			for (int i = 0, j = 0; i < _sizeOfCurrentAddingShip && j < _sizeOfCurrentAddingShip;)
			{
				int changedFiled = _isVerticalAddingShip ? j : i;
				int previousVal = Board.GetField(_coordXAddingShip + j, _coordYAddingShip + i) == (int)Marker.CannotAdd ? _coveringFieldsAddingShip[changedFiled] : (int)Marker.EmptyField;
				Board.SetField(_coordXAddingShip + j, _coordYAddingShip + i, previousVal);
				if (_isVerticalAddingShip)
					++j;
				else
					++i;
			}
			return ReadKey();
		}
	}
}
using System;

namespace Statki
{
	enum Players : int { Person, Computer };
	enum BoardSide : int { Left, Right }
	abstract class Moves : PlayerBoard
	{
		public int SunkenShips { get; set; }
		public Moves Opponent { get; set; }
		public Ship[] PlayerShips { get; private set; }
		public BoardSide WhichBoard { get; }
		protected readonly int[] _shipSize = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
		protected int _x, _y;
		protected Players _player;
		protected abstract void AddShips();
		public void AddShipAfterLoadGame(string line, int shipNumb)
		{
			PlayerShips[shipNumb] = new Ship(line, shipNumb);
		}
		public abstract Actions Shoot();
		public Moves(BoardSide boardNum, Players player, Moves opponent) : base()
		{
			WhichBoard = boardNum;
			SunkenShips = _x = _y = 0;
			_player = player;
			PlayerShips = new Ship[_shipSize.Length];
			Opponent = opponent;
		}
		public bool IsPerson() //for saving game
		{
			return _player == Players.Person;
		}
		public string GetShipsAsString()
		{
			string allShips = "";
			foreach(Ship ship in PlayerShips)
			{
				allShips += ship.GetShipAsString() + "\n";
			}
			return allShips;
		}
		public void DrawShip(int shipNumber)
		{
			for (int i = 0; i < PlayerShips[shipNumber].Size; ++i)
			{
				Tuple<int, int> shipCoord = PlayerShips[shipNumber][i];
				this[shipCoord.Item1, shipCoord.Item2] = shipNumber + 10 * PlayerShips[shipNumber].GetFieldMark(i);
			}
		}
		virtual public void MarkShipNeighborhood(bool isSink, int shipNumber)
		{
			int mark = isSink ? (int)Marker.NearSunkenShip : (int)Marker.NearShip;
			for (int i = 0; i < PlayerShips[shipNumber ].Size; ++i)
			{
				Tuple<int, int> shipCoord = PlayerShips[shipNumber ][i];
				for (int j = -1; j < 2; ++j)
				{
					for (int k = -1; k < 2; ++k)
						SetAreaIf(shipCoord.Item1 + k, shipCoord.Item2 + j, mark, (int)Marker.EmptyField);
				}
			}
		}
		virtual public void UndoHit(int coordX, int coordY, int shipNumber)
		{
			PlayerShips[shipNumber].UndoHit(coordX, coordY);
			DrawShip(shipNumber + 1);
		}
	}
}

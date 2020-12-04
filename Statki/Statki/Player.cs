using Battleship.Interfaces;
using System;
using System.Text;

namespace Battleship
{
	public enum Players : int { Person, Computer };
	public enum BoardSide : int { Left, Right }
	abstract public class Player
	{
		public int SunkenShips { get; set; }
		public BoardSide WhichBoard { get; }
		public Player Opponent { get; set; }
		public Ship[] PlayerShips { get; private set; }
		public IBoard Board { get; set; }
		protected readonly IOutputDevice _outputDevice;
		protected readonly int[] _shipSize = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
		protected int _x, _y;
		protected Players _player;
		protected abstract void AddShips();
		public abstract Actions Shoot();
		public void AddShipAfterLoadGame(string line, int shipNumb)
		{
			PlayerShips[shipNumb] = new Ship(line, shipNumb);
		}

		public Player(BoardSide boardNum, Players player, Player opponent, IOutputDevice outputDevice)
		{
			WhichBoard = boardNum;
			SunkenShips = _x = _y = 0;
			_player = player;
			PlayerShips = new Ship[_shipSize.Length];
			Opponent = opponent;
			_outputDevice = outputDevice;
			Board = new Board();
		}
		public bool IsPerson() //for saving game
		{
			return _player == Players.Person;
		}
		public string GetShipsAsString()
		{
			StringBuilder allShips = new StringBuilder(string.Empty);
			foreach(Ship ship in PlayerShips)
			{
				allShips.Append( ship.GetShipAsString() + "\n");
			}
			return allShips.ToString();
		}
		public void DrawShip(int shipNumber)
		{
			for (int i = 0; i < PlayerShips[shipNumber].Size; ++i)
			{
				Tuple<int, int> shipCoord = PlayerShips[shipNumber][i];
				Board.SetField(shipCoord.Item1, shipCoord.Item2, shipNumber + 10 * PlayerShips[shipNumber].GetFieldMark(i));
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
						Board.SetFieldIf(shipCoord.Item1 + k, shipCoord.Item2 + j, mark, (int)Marker.EmptyField);
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

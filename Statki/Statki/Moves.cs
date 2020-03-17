using System.Collections.Generic;

namespace Statki
{
	public enum Players : int { PERSON, COMPUTER };
	public enum BoardSide : int { _LEFT, _RIGHT };
	abstract class Moves
	{
		protected readonly int[] _shipSize = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
		protected int _x, _y, _sunkenShips;
		protected readonly bool _whichBoard;
		private readonly bool opponentBoard;
		public bool OpponentBoard => opponentBoard;
		protected readonly Players _player;
		public Moves Opponent { get; set; }
		public Ship[] PlayerShips { get; set; }

		protected abstract void AddShips();
		public abstract bool Shoot();
		public Moves(bool boardNum, Players player, Moves opponent)
		{
			_whichBoard = boardNum;
			opponentBoard = !boardNum;
			_sunkenShips = _x = _y = 0;
			_player = player;
			PlayerShips = new Ship[_shipSize.Length];
			Opponent = opponent;
		}
		public Ship GetShip(int index)
		{
			return PlayerShips[index];
		}
		public int SunkenShips
		{
			get { return _sunkenShips; }
		}

		public bool IsPerson()
		{
			return _player == Players.PERSON;
		}
	}
}

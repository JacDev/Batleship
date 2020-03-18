namespace Statki
{
	public enum Players : int { Person, Computer };
	public enum BoardSide : int { Left, Right };
	abstract class Moves
	{
		public BoardSide WhichBoard { get; }
		public BoardSide OpponentBoard { get; }
		public Moves Opponent { get; set; }
		public Ship[] PlayerShips { get; private set; }

		protected readonly int[] _shipSize = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
		protected int _x, _y, _sunkenShips;
		protected Players _player;
		protected abstract void AddShips();
		public abstract Actions Shoot();
		public Moves(BoardSide boardNum, Players player, Moves opponent)
		{
			WhichBoard = boardNum;
			OpponentBoard = boardNum == BoardSide.Left ? BoardSide.Right : BoardSide.Left;
			_sunkenShips = _x = _y = 0;
			_player = player;
			PlayerShips = new Ship[_shipSize.Length];
			Opponent = opponent;	
		}
		public bool IsPerson() //for saving game
		{
			return _player == Players.Person;
		}
	}
}

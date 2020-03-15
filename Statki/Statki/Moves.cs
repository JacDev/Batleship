namespace Statki
{
    public enum Players : int { PERSON, COMPUTER };
    public enum BoardSide : int { _LEFT, _RIGHT };
    abstract class Moves
    {
        protected readonly int[] _shipSize = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
        protected int _x, _y, _sunkenShips;
        protected readonly bool _whichBoard;
        protected readonly bool _opponentBoard;
        protected readonly Players _player;
        protected Moves _opponent;
        protected Ship[] _playerShips;


        protected abstract void AddShips();
        public abstract bool Shoot();
        public Moves(bool boardNum, Players player, Moves opponent)
        {
            _whichBoard = boardNum;
            _opponentBoard = !boardNum;
            _sunkenShips = _x = _y = 0;
            _player = player;
            _playerShips = new Ship[_shipSize.Length];
            _opponent = opponent;
        }
        public Ship GetShip(int index)
        {
            return _playerShips[index];
        }
        public int SunkenShips
        {
            get { return _sunkenShips; }
        }
        public Moves Opponent
        {
            set
            {
                _opponent = value;
            }
        }
        public bool IsPerson()
        {
            return _player == Players.PERSON;
        }
    }
}

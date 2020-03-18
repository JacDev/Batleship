using System;

namespace Statki
{
	class Ship
	{
		enum State : int { Missed, Hit, Sunk };
		private int _coordX;
		private int _coordY;
		private BoardSide _whichBoard;
		private int _size;
		private int _left;
		private int _shipNumber;
		private bool _isVertical;
		private int[,] _shipCoord;
		public Ship (int coordX, int coordY, int size, int shipNumber, bool isVertical, BoardSide whichBoard)
		{
			_coordX = coordX;
			_coordY = coordY;
			_shipNumber = shipNumber;
			_isVertical = isVertical;
			_whichBoard = whichBoard;
			_left = size;
			_size = size;
			_shipCoord = new int[size, 2];
			MakeShip();
			DrawShip();
			MarkNeighborhood(false);
		}
		public void UndoAdded()
		{
			for (int i = 0; i < _size; ++i)
			{
				Board.Instance[this[i].Item1, this[i].Item2, _whichBoard] = (int)Marker.EmptyField;
			}
			for (int i = 0; i < _size; ++i)
			{
				for (int j = -1; j < 2; ++j)
				{
					for (int k = -1; k < 2; ++k)
						Board.Instance[this[i].Item1 + k, this[i].Item2 + j, _whichBoard] = (int)Marker.EmptyField;
				}
			}
		}
		public void UndoHit(int x, int y)
		{
			if (_left == 0) 
			{
				UndoSunken();
			}
			int hitArea = _isVertical ? x : y;
			for (int i = 0; i < _size; ++i)
			{
				if (_shipCoord[i, 0] == hitArea)
				{
					_shipCoord[i, 1] = (int)State.Missed;
					break;
				}
			}				
			DrawShip();
			++_left;
		}
		public void MarkNeighborhood(bool isSink)
		{
			int oznaczenie = isSink ? (int)Marker.NearSunkenShip : (int)Marker.NearShip;
			for (int i = 0; i < _size; ++i)
			{
				for (int j = -1; j < 2; ++j)
				{
					for (int k = -1; k < 2; ++k)
						Board.Instance.SetAreaIf(this[i].Item1 + k, this[i].Item2 + j, oznaczenie, (int)Marker.EmptyField, _whichBoard);
				}
			}
		}
		public bool HitShip(int coordX, int coordY, bool isComputer = false)
		{
			int hitArea = _isVertical ? coordX : coordY;
			for (int i = 0; i < _size; ++i)
			{
				if (_shipCoord[i, 0] == hitArea)
				{
					_shipCoord[i, 1] = (int)State.Hit;
					--_left;
					if (_left == 0)
					{
						SinkShip();
						if (isComputer)
						{
							MarkNeighborhood(true);
						}
					}
					DrawShip();
					break;
				}
			}
			return _left == 0;
		}
		private void UndoSunken()
		{
			for (int i = 0; i < _size; ++i)
			{
				_shipCoord[i, 1] = (int)State.Hit;
			}
		}
		private void MakeShip()
		{
			int firstCoord = _isVertical ? _coordX : _coordY;
			_shipCoord[0, 0] = firstCoord;
			_shipCoord[0, 1] = (int)State.Missed;
			for (int i = 1; i < _size; ++i)
			{
				_shipCoord[i, 0] = _shipCoord[i - 1, 0] + 1;
				_shipCoord[i, 1] = (int)State.Missed;
			}
		}
		private void DrawShip()
		{
			for (int i = 0; i < _size; ++i)
			{
				Board.Instance[this[i].Item1, this[i].Item2, _whichBoard] = _shipNumber + 10 * _shipCoord[i, 1];
			}
		}

		private Tuple<int, int> this[int i]
		{
			get
			{
				int x = _isVertical ? _shipCoord[i, 0] : _coordX;
				int y = _isVertical ? _coordY : _shipCoord[i, 0];
				return new Tuple<int, int> (x,y);				
			}
		}
		private void SinkShip()
		{
			for (int i = 0; i < _size; ++i)
			{
				_shipCoord[i, 1] = (int)State.Sunk;
			}
		}
	}
}
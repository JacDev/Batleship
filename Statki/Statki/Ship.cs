using System;
using System.Text;

namespace Statki
{
	class Ship
	{
		enum State : int { Missed, Hit, Sunk };
		private int _coordX;
		private int _coordY;
		private int _left;
		private int _shipNumber;
		private bool _isVertical;
		private int[,] _shipCoord;
		public int Size { get; }
		public Ship (int coordX, int coordY, int size, int shipNumber, bool isVertical)
		{
			_coordX = coordX;
			_coordY = coordY;
			_shipNumber = shipNumber;
			_isVertical = isVertical;
			_left = size;
			Size = size;
			_shipCoord = new int[size, 2];
			MakeShip();
		}
		public Ship(string line, int shipNumber)
		{
			char separator = ' ';
			string[] substrings = line.Split(separator);
			_coordX = Convert.ToInt32(substrings[0]);
			_coordY = Convert.ToInt32(substrings[1]);
			_isVertical = Convert.ToBoolean(substrings[2] == "True");
			_left = Convert.ToInt32(substrings[3]);
			Size = Convert.ToInt32(substrings[4]);
			_shipCoord = new int[Size, 2];
			_shipNumber = shipNumber;
			MakeShip();
			for(int i = 0; i < Size; ++i)
			{
				_shipCoord[i, 1] = Convert.ToInt32(substrings[5 + i]);
			}
		}
		public string GetShipAsString()
		{
			return new string(_coordX.ToString() + " " + _coordY.ToString() + " " + _isVertical.ToString() +
			" " + _left.ToString() + " " + Size.ToString() + " " + GetShipCoordAsString());
		}
		private string GetShipCoordAsString()
		{
			StringBuilder result = new StringBuilder(string.Empty);
			for (int j = 0; j < Size; j++)
			{
				result.Append(_shipCoord[j, 1].ToString() + " ");
			}
			return result.ToString();
		}
		public void UndoHit(int x, int y)
		{
			if (_left == 0) 
			{
				UndoSunken();
			}
			int hitArea = _isVertical ? x : y;
			for (int i = 0; i < Size; ++i)
			{
				if (_shipCoord[i, 0] == hitArea)
				{
					_shipCoord[i, 1] = (int)State.Missed;
					break;
				}
			}				
			++_left;
		}
		public bool HitShip(int coordX, int coordY, bool isComputer = false)
		{
			int hitArea = _isVertical ? coordX : coordY;
			for (int i = 0; i < Size; ++i)
			{
				if (_shipCoord[i, 0] == hitArea)
				{
					_shipCoord[i, 1] = (int)State.Hit;
					--_left;
				}
			}
			return _left == 0;
		}
		private void UndoSunken()
		{
			for (int i = 0; i < Size; ++i)
			{
				_shipCoord[i, 1] = (int)State.Hit;
			}
		}
		private void MakeShip()
		{
			int firstCoord = _isVertical ? _coordX : _coordY;
			_shipCoord[0, 0] = firstCoord;
			_shipCoord[0, 1] = (int)State.Missed;
			for (int i = 1; i < Size; ++i)
			{
				_shipCoord[i, 0] = _shipCoord[i - 1, 0] + 1;
				_shipCoord[i, 1] = (int)State.Missed;
			}
		}
		public Tuple<int, int> this[int i]
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
			for (int i = 0; i < Size; ++i)
			{
				_shipCoord[i, 1] = (int)State.Sunk;
			}
		}
		public int GetFieldMark(int coord)
		{
			return _shipCoord[coord, 1];
		}
	}
}
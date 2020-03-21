using System;
using System.Linq;
using System.Text;

namespace Statki
{
	enum Marker : int
	{
		EmptyField = 50, FirstShip = 0, LastShip = 9, FirstHitShip, LastHitShip = 19, FirstSunkShip, LastSunkShip = 29,
		ChosenToAdd, NearShip, CannotAdd, ChosenToShoot, AlreadyShot, CannotShoot, NearSunkenShip
	};

	abstract class PlayerBoard
	{
		static public int Height = 10, Width = 10;
		public const int LeftEdge = 0, RightEdge = 9;
		public const int UpperEdge = 0, LowerEdge = 9;
		private int[] _board;

		public PlayerBoard()
		{
			_board = new int[Height * Width];
			_board = Enumerable.Repeat((int)Marker.EmptyField, Height * Width).ToArray();
		}
		public int this[int indx, int indy]
		{
			get
			{
				if (IsInBoard(indx, indy))
				{
					return _board[indx * Width + indy];
				}
				return -1; //add throwing exception
			}
			set
			{
				if (IsInBoard(indx, indy))
				{
					_board[indx * Width + indy] = value;
				}
			}
		}
		public string GetBoardAsString()
		{
			StringBuilder result = new StringBuilder(string.Empty);
			for (int i = 0; i < Height; i++)
			{
				for (int j = 0; j < Width; j++)
				{
					result.Append(this[i, j].ToString() + " ");
				}
			}
			return result.ToString();
		}
		public void LoadBoard(string line)
		{
			char separator = ' ';
			string[] substrings = line.Split(separator);
			for (int i = 0; i < Height; ++i)
			{
				for (int j = 0; j < Width; ++j)
				{
					this[i, j] = Convert.ToInt32(substrings[i * Width + j]);
				}
			}
		}
		private bool IsInBoard(int indx, int indy)
		{
			if (indx >= UpperEdge && indx <= LowerEdge && indy >= LeftEdge && indy <= RightEdge)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public void SetAreaIf(int x, int y, int val, int condition)
		{
			if(IsInBoard(x, y))
			{
				if (this[x, y] == condition)
				{
					this[x, y] = val;
				}
			}
		}
		public void ClearMarks()
		{
			for (int x = 0; x < Height; ++x)
			{
				for (int y = 0; y < Width; ++y)
				{
					if (this[x, y] == (int)Marker.NearShip)
					{
						this[x, y] = (int)Marker.EmptyField;
					}
				}
			}
		}
		public void ClearBoard()
		{
			_board = Enumerable.Repeat((int)Marker.EmptyField, Height * Width).ToArray();
		}
	}
}
using System;
using System.Text;

namespace Statki {
	enum Marker : int { EmptyField = 0, FirstShip = 1, LastShip = 10, FirstHitShip, LastHitShip = 20, FirstSunkShip, LastSunkShip = 30,
		ChosenToAdd, NearShip, CannotAdd, ChosenToShoot, AlreadyShot, CannotShoot, NearSunkenShip
	};

	public sealed class Board
	{
		private static readonly Board _Instance = new Board();
		private enum Shift : int { None, NextBoard = 10 };
		public const int Height = 10, Width = 20;
		public const int PlayerBoardWidth = Width / 2;
		public const int LeftEdge = 0, UpperEdge = 0;
		public const int RightEdge = 9, LowerEdge = 9;
		private int[,] _board;

		private Board()
		{
			_board = new int[Height, Width];
			Console.BackgroundColor = ConsoleColor.Black;
		}
		public static Board Instance
		{
			get
			{
				return _Instance;
			}
		}
		public int this[int indx, int indy, BoardSide whichBoard = BoardSide.Left]
		{
			get
			{
				int shift = whichBoard == BoardSide.Right ? (int)Shift.NextBoard : (int)Shift.None;
				if (IsInBoard(indx, indy + shift)) 
				{ 
					return _board[indx, indy + shift];
				}
				return -1; //add throwing exception
			}
			set
			{
				int shift = whichBoard == BoardSide.Right ? (int)Shift.NextBoard : (int)Shift.None;
				if (IsInBoard(indx, indy + shift))
				{ 
					_board[indx, indy + shift] = value;
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
				for(int j = 0; j < Width; ++j)
				{
					this[i, j] = Convert.ToInt32(substrings[i * Width + j]);
				}
			}
		}
		private bool IsInBoard(int indx, int indy)
		{
			if (indx >= UpperEdge && indx <= LowerEdge && indy >= LeftEdge && indy <= Width)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public void SetAreaIf(int x, int y, int val, int condition, BoardSide whichBoard)
		{
			if (x >= UpperEdge && x <= LowerEdge && y >= LeftEdge && y <= RightEdge)
			{
				if (this[x, y, whichBoard] == condition)
				{
					this[x, y, whichBoard] = val;
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
			for (int x = 0; x < Height; ++x)
			{
				for (int y = 0; y < Width; ++y)
				{
					this[x, y] = (int)Marker.EmptyField;
				}
			}
		}
	}
}
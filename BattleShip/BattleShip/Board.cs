using Battleship.Consts;
using Battleship.Interfaces;
using System;
using System.Linq;
using System.Text;

namespace Battleship
{
	public enum Marker : int
	{
		EmptyField = 50, FirstShip = 0, LastShip = 9, FirstHitShip, LastHitShip = 19, FirstSunkShip, LastSunkShip = 29,
		ChosenToAdd, NearShip, CannotAdd, ChosenToShoot, AlreadyShot, CannotShoot, NearSunkenShip
	};

	public class Board : IBoard, ILoadable
	{
		private int[] _board;

		public Board()
		{
			_board = new int[BoardSize.Height * BoardSize.Width];
			_board = Enumerable.Repeat((int)Marker.EmptyField, BoardSize.Height * BoardSize.Width).ToArray();
		}
		public int[] GetBoard()
		{
			return _board;
		}
		public int GetField(int indx, int indy)
		{
			if (IsInBoard(indx, indy))
			{
				return _board[indx * BoardSize.Width + indy];
			}
			throw new ArgumentOutOfRangeException("Field ({1};{2}) is outside the board.", indx.ToString(), indy.ToString());
		}
		public void SetField(int indx, int indy, int value)
		{
			if (IsInBoard(indx, indy))
			{
				_board[indx * BoardSize.Width + indy] = value;
			}
		}
		public void SetFieldIf(int x, int y, int val, int condition)
		{
			if (IsInBoard(x, y))
			{
				if (GetField(x, y) == condition)
				{
					SetField(x, y, val);
				}
			}
		}
		public override string ToString()
		{
			StringBuilder result = new StringBuilder(string.Empty);
			for (int i = 0; i < BoardSize.Height * BoardSize.Width; i++)
			{

				result.Append(_board[i].ToString() + (i == BoardSize.Height * BoardSize.Width - 1 ? "" : " "));
			}
			return result.ToString();
		}
		public void ReadFromString(string line)
		{
			if (line != string.Empty)
			{
				char separator = ' ';
				string[] substrings = line.Split(separator);
				if (substrings.Length == BoardSize.Height * BoardSize.Width)
				{
					for (int i = 0; i < BoardSize.Height; ++i)
					{
						for (int j = 0; j < BoardSize.Width; ++j)
						{
							SetField(i, j, Convert.ToInt32(substrings[i * BoardSize.Width + j]));
						}
					}
				}
				else
				{
					throw new NotImplementedException();
				}

			}
		}
		private bool IsInBoard(int indx, int indy)
		{
			if (indx >= BoardSize.TopEdge && indx <= BoardSize.BottomEdge && indy >= BoardSize.LeftEdge && indy <= BoardSize.RightEdge)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public void ClearNearShipMarks()
		{
			for (int x = 0; x < BoardSize.Height; ++x)
			{
				for (int y = 0; y < BoardSize.Width; ++y)
				{
					if (GetField(x, y) == (int)Marker.NearShip)
					{
						SetField(x, y, (int)Marker.EmptyField);
					}
				}
			}
		}
		public void ClearBoard()
		{
			_board = Enumerable.Repeat((int)Marker.EmptyField, BoardSize.Height * BoardSize.Width).ToArray();
		}		
	}
}
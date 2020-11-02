using Statki.Interfaces;
using System;
using System.Threading;

namespace Battleship
{
	enum WindowEdge : int { Left, Right, Top, Bottom };
	public class Window : IOutputDevice, IInputDevicee
	{
		private const int _separatorSize = 3;
		private const int _spaceBeetweenBoardsSize = 40;
		private const int _boardEdgeSize = 24;
		private const int _oneBlockWidth = 2;
		private const char _boardMarker = ' ';
		private const string _doubleBoardMarker = "  ";

		private readonly string _spaceBetweenBoards = new string(_boardMarker, _spaceBeetweenBoardsSize);
		private readonly string _boardEdge = new string(_boardMarker, _boardEdgeSize);
		private readonly string _windowEdge = new string(_boardMarker, 2 * _boardEdgeSize + _spaceBeetweenBoardsSize + 4 * _separatorSize + 2 * _oneBlockWidth);
		private readonly string _horizontalEdgeSeparator = new string(_boardMarker, _separatorSize * 2);

		private int _chosenOption;
		private bool _isHighlighted = true;

		public void PrintBoard(Board leftBoard, Board rightBoard)
		{
			Console.Clear();
			PrintWindowEdge(WindowEdge.Top);
			PrintUpDown();
			for (int x = 0; x < Board.Height; ++x)
			{
				PrintWindowEdge(WindowEdge.Left);
				PrintLine(x, leftBoard, rightBoard);
				PrintWindowEdge(WindowEdge.Right);
			}
			PrintUpDown();
			PrintWindowEdge(WindowEdge.Bottom);

		}
		private void PrintLine(int line, Board leftBoard, Board rightBoard)
		{
			PrintFrame();
			for (int y = 0; y < Board.Width; ++y)
			{
				PrintShipArea(leftBoard[line, y]);
			}
			PrintFrame();
			Console.Write("{0}", _spaceBetweenBoards);
			PrintFrame();
			for (int y = 0; y < Board.Width; ++y)
			{
				PrintShipArea(rightBoard[line, y]);
			}
			PrintFrame();
		}
		private void PrintFrame()
		{
			Console.BackgroundColor = ConsoleColor.White;
			Console.Write(_doubleBoardMarker);
			Console.BackgroundColor = ConsoleColor.Black;
		}
		private void PrintUpDown()
		{
			PrintWindowEdge(WindowEdge.Left);
			Console.BackgroundColor = ConsoleColor.White;
			Console.Write(_boardEdge);
			Console.BackgroundColor = ConsoleColor.Black;
			Console.Write(_spaceBetweenBoards);
			Console.BackgroundColor = ConsoleColor.White;
			Console.Write(_boardEdge);
			Console.BackgroundColor = ConsoleColor.Black;
			PrintWindowEdge(WindowEdge.Right);
		}
		private void PrintWindowEdge(WindowEdge windowEdge)
		{
			switch (windowEdge)
			{
				case WindowEdge.Left:
					Console.BackgroundColor = ConsoleColor.White;
					Console.Write(_doubleBoardMarker);
					Console.BackgroundColor = ConsoleColor.Black;
					Console.Write(_horizontalEdgeSeparator);
					break;
				case WindowEdge.Right:
					Console.BackgroundColor = ConsoleColor.Black;
					Console.Write(_horizontalEdgeSeparator);
					Console.BackgroundColor = ConsoleColor.White;
					Console.WriteLine(_doubleBoardMarker);
					Console.BackgroundColor = ConsoleColor.Black;
					break;
				case WindowEdge.Top:
					Console.BackgroundColor = ConsoleColor.White;
					Console.WriteLine(_windowEdge);
					Console.BackgroundColor = ConsoleColor.Black;
					for (int i = 0; i < _separatorSize; ++i)
					{
						Console.BackgroundColor = ConsoleColor.White;
						Console.Write(_doubleBoardMarker);
						Console.BackgroundColor = ConsoleColor.Black;
						Console.Write(new string(_boardMarker, _windowEdge.Length - 4));
						Console.BackgroundColor = ConsoleColor.White;
						Console.WriteLine(_doubleBoardMarker);
						Console.BackgroundColor = ConsoleColor.Black;
					}
					break;
				case WindowEdge.Bottom:
					for (int i = 0; i < _separatorSize; ++i)
					{
						Console.BackgroundColor = ConsoleColor.White;
						Console.Write(_doubleBoardMarker);
						Console.BackgroundColor = ConsoleColor.Black;
						Console.Write(new string(_boardMarker, _windowEdge.Length - 4));
						Console.BackgroundColor = ConsoleColor.White;
						Console.WriteLine(_doubleBoardMarker);
						Console.BackgroundColor = ConsoleColor.Black;
					}
					Console.BackgroundColor = ConsoleColor.White;
					Console.WriteLine(_windowEdge);
					Console.BackgroundColor = ConsoleColor.Black;
					break;
			}
		}
		private void PrintShipArea(int index)
		{
			Console.BackgroundColor = ConsoleColor.Black;
			switch (index)
			{
				case int i when (i >= (int)Marker.FirstShip && i <= (int)Marker.LastShip):
					Console.BackgroundColor = ConsoleColor.DarkBlue;
					break;
				case int i when (i >= (int)Marker.FirstHitShip && i <= (int)Marker.LastHitShip):
					Console.BackgroundColor = ConsoleColor.DarkRed;
					break;
				case int i when (i >= (int)Marker.FirstSunkShip && i <= (int)Marker.LastSunkShip):
					Console.BackgroundColor = ConsoleColor.Blue;
					break;
				case (int)Marker.ChosenToAdd:
					Console.BackgroundColor = ConsoleColor.Green;
					break;
				case (int)Marker.NearShip:
					Console.BackgroundColor = ConsoleColor.Magenta;
					break;
				case (int)Marker.CannotAdd:
					Console.BackgroundColor = ConsoleColor.Red;
					break;
				case (int)Marker.ChosenToShoot:
					Console.BackgroundColor = ConsoleColor.Yellow;
					break;
				case (int)Marker.AlreadyShot:
					Console.BackgroundColor = ConsoleColor.Gray;
					break;
				case (int)Marker.CannotShoot:
					Console.BackgroundColor = ConsoleColor.Red;
					break;
				case (int)Marker.NearSunkenShip:
				case (int)Marker.EmptyField:
					Console.BackgroundColor = ConsoleColor.Cyan;
					break;
			}
			//Console.Write(index + " "); //for debuging
			Console.Write(_doubleBoardMarker);
		}
		public Keys ReadKey()
		{
			ConsoleKey key = Console.ReadKey(false).Key;
			return key switch
			{
				ConsoleKey.UpArrow => Keys.Up,
				ConsoleKey.DownArrow => Keys.Down,
				ConsoleKey.LeftArrow => Keys.Left,
				ConsoleKey.RightArrow => Keys.Right,
				ConsoleKey.Enter => Keys.Enter,
				ConsoleKey.Escape => Keys.Escape,
				ConsoleKey.R => Keys.Rotate,
				ConsoleKey.U => Keys.Undo,
				ConsoleKey.C => Keys.Clear,
				_ => Keys.None,
			};
		}
		public int ShowMenu(string[] options, bool hideLastMenuOptions = false)
		{
			while (ShowMenuOptions(options, hideLastMenuOptions)) ;
			return _chosenOption;
		}
		private bool ShowMenuOptions(string[] options, bool hideLastMenuOptions)
		{
			Console.Clear();
			int menuSize = hideLastMenuOptions ? options.Length - 2 : options.Length;
			for (int i = 0; i < menuSize; i++)
			{
				Console.Write(("").PadRight(40, _boardMarker));
				if (i == _chosenOption)
				{
					if (_isHighlighted)
					{
						Console.BackgroundColor = ConsoleColor.Red;
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.Red;
					}
					_isHighlighted = !_isHighlighted;
				}
				Console.WriteLine(options[i]);

				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.White;
			}
			return ReadOption(menuSize);
		}
		private bool ReadOption(int size)
		{
			Keys key = Keys.None;
			if (Console.KeyAvailable)
			{
				key = ReadKey();
			}
			switch (key)
			{
				case Keys.Up:
					{
						_chosenOption = _chosenOption == 0 ? size - 1 : _chosenOption - 1;
						break;
					}
				case Keys.Down:
					{
						_chosenOption = _chosenOption == size - 1 ? 0 : _chosenOption + 1;
						break;
					}
				case Keys.Enter:
					{
						Thread.Sleep(200);
						return false;
					}
			}
			Thread.Sleep(200);
			return true;
		}
	}
}
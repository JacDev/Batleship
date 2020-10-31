using System;
using System.Security.Authentication;
using System.Threading;

namespace Statki
{
	enum Keys : int { Left, Right, Up, Down, Enter, Escape, Rotate, Undo, Clear, None };
	enum WindowEdge : int { Left, Right, Top, Bottom};
	static class Window
	{
		private const int _separatorSize = 3;
		private static string _spaceBetweenBoards = new string(' ', 40);
		private static string _boardEdge = new string(' ', 24);
		private static string _windowEdge = new string(' ', 2*_boardEdge.Length + _spaceBetweenBoards.Length + 4*_separatorSize+4);
		private static string _horizontalEdgeSeparator = new string(' ', _separatorSize*2);
		public static bool CanLoadGame { get; set; }

		private static int _chosenOption;
		private static bool _isHighlighted = true;

		public static void PrintBoard(Moves leftBoard, Moves rightBoard)
		{
			Console.Clear();
			PrintWindowEdge(WindowEdge.Top);
			PrintUpDown();
			for (int x = 0; x < Moves.Height; ++x)
			{
				PrintWindowEdge(WindowEdge.Left);
				PrintLine(x, leftBoard, rightBoard);
				PrintWindowEdge(WindowEdge.Right);
			}
			PrintUpDown();
			PrintWindowEdge(WindowEdge.Bottom);

		}
		private static void PrintLine(int line, Moves leftBoard, Moves rightBoard)
		{
			PrintFrame();
			for (int y = 0; y < Moves.Width; ++y)
			{
				PrintShipArea(leftBoard[line, y]);
			}
			PrintFrame();
			Console.Write("{0}", _spaceBetweenBoards);
			PrintFrame();
			for (int y = 0; y < Moves.Width; ++y)
			{
				PrintShipArea(rightBoard[line, y]);
			}
			PrintFrame();
		}
		private static void PrintFrame()
		{
			Console.BackgroundColor = ConsoleColor.White;
			Console.Write("  ");
			Console.BackgroundColor = ConsoleColor.Black;
		}
		private static void PrintUpDown()
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
		private static void PrintWindowEdge(WindowEdge windowEdge)
		{
			switch (windowEdge)
			{
				case WindowEdge.Left:
					Console.BackgroundColor = ConsoleColor.White;
					Console.Write("  ");
					Console.BackgroundColor = ConsoleColor.Black;
					Console.Write(_horizontalEdgeSeparator);
					break;
				case WindowEdge.Right:
					Console.BackgroundColor = ConsoleColor.Black;
					Console.Write(_horizontalEdgeSeparator);
					Console.BackgroundColor = ConsoleColor.White;
					Console.WriteLine("  ");
					Console.BackgroundColor = ConsoleColor.Black;
					break;
				case WindowEdge.Top:
					Console.BackgroundColor = ConsoleColor.White;
					Console.WriteLine(_windowEdge);
					Console.BackgroundColor = ConsoleColor.Black;
					for(int i=0;i<_separatorSize; ++i)
					{
						Console.BackgroundColor = ConsoleColor.White;
						Console.Write("  ");
						Console.BackgroundColor = ConsoleColor.Black;
						Console.Write(new string(' ', _windowEdge.Length-4));
						Console.BackgroundColor = ConsoleColor.White;
						Console.WriteLine("  ");
						Console.BackgroundColor = ConsoleColor.Black;
					}
					break;
				case WindowEdge.Bottom:
					for (int i = 0; i < _separatorSize; ++i)
					{
						Console.BackgroundColor = ConsoleColor.White;
						Console.Write("  ");
						Console.BackgroundColor = ConsoleColor.Black;
						Console.Write(new string(' ', _windowEdge.Length - 4));
						Console.BackgroundColor = ConsoleColor.White;
						Console.WriteLine("  ");
						Console.BackgroundColor = ConsoleColor.Black;
					}
					Console.BackgroundColor = ConsoleColor.White;
					Console.WriteLine(_windowEdge);
					Console.BackgroundColor = ConsoleColor.Black;
					break;
			}
		}
		private static void PrintShipArea(int index)
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
			Console.Write("  ");
		}
		public static Keys ReadKey()
		{
			ConsoleKey key = Console.ReadKey(false).Key;
			switch (key)
			{
				case ConsoleKey.UpArrow:
					return Keys.Up;
				case ConsoleKey.DownArrow:
					return Keys.Down;
				case ConsoleKey.LeftArrow:
					return Keys.Left;
				case ConsoleKey.RightArrow:
					return Keys.Right;
				case ConsoleKey.Enter:
					return Keys.Enter;
				case ConsoleKey.Escape:
					return Keys.Escape;
				case ConsoleKey.R:
					return Keys.Rotate;
				case ConsoleKey.U:
					return Keys.Undo;
				case ConsoleKey.C:
					return Keys.Clear;
				default:
					return Keys.None;
			}
		}
		public static int ShowMenu()
		{
			while (ShowMenuOptions()) ;
			return _chosenOption;
		}
		private static bool ShowMenuOptions()
		{
			Console.Clear();
			int menuSize = CanLoadGame ? LanguageOptions.MenuOptions.Length : LanguageOptions.MenuOptions.Length - 2;
			for (int i = 0; i < menuSize; i++)
			{
				Console.Write(("").PadRight(40, ' '));
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
				Console.WriteLine(LanguageOptions.MenuOptions[i]);

				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.White;
			}
			return ReadOption(menuSize);
		}
		private static bool ReadOption(int size)
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
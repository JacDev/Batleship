using Battleship.Consts;
using Battleship.Interfaces;
using System;
using System.Linq;
using System.Threading;

namespace Battleship
{
	enum WindowEdge : int { Left, Right, Top, Bottom };
	public class Window : IOutputDevice
	{
		private readonly string _spaceBetweenBoards = new string(WindowSize.BoardMarker, WindowSize.SpaceBeetweenBoardsSize);
		private readonly string _boardEdge = new string(WindowSize.BoardMarker, WindowSize.BoardEdgeSize);
		private readonly string _windowEdge = new string(WindowSize.BoardMarker, 2 * WindowSize.BoardEdgeSize
			+ WindowSize.SpaceBeetweenBoardsSize + 4 * WindowSize.SeparatorSize + 2 * WindowSize.OneBlockWidth);
		private readonly string _horizontalEdgeSeparator = new string(WindowSize.BoardMarker, WindowSize.SeparatorSize * 2);

		private int _chosenOption;
		private bool _isHighlighted = true;
		private LanguageOptions _languageOptions;
		private readonly ILoggerService _loggerService;
		private readonly IInputDevice _inputDevice;

		public Window(LanguageOptions languageOptions, ILoggerService loggerService, IInputDevice inputDevice)
		{
			_languageOptions = languageOptions;
			_loggerService = loggerService;
			_inputDevice = inputDevice;
		}

		public void PrintBoard(IBoard leftBoard, IBoard rightBoard, bool duringAdding)
		{
			Console.Clear();
			PrintWindowEdge(WindowEdge.Top);
			PrintUpDown();
			for (int x = 0; x < BoardSize.Height; ++x)
			{
				PrintWindowEdge(WindowEdge.Left);
				PrintLine(x, leftBoard, rightBoard, duringAdding);
				PrintWindowEdge(WindowEdge.Right);
			}
			PrintUpDown();
			PrintWindowEdge(WindowEdge.Bottom);

		}
		private void PrintLine(int lineNumber, IBoard leftBoard, IBoard rightBoard, bool duringAdding)
		{
			try
			{
				string[] vall = Enum.GetNames(typeof(Marker));
				PrintFrame();
				for (int y = 0; y < BoardSize.Width; ++y)
				{
					PrintShipArea(leftBoard.GetField(lineNumber, y));
				}
				int currentMessagesLenght = duringAdding ? _languageOptions.ChosenLanguage.DuringAdding.SignsMeaningList.Count: _languageOptions.ChosenLanguage.DuringGame.SignsMeaningList.Count;
				PrintFrame();

				var currentMessagesList = duringAdding ? _languageOptions.ChosenLanguage.DuringAdding.SignsMeaningList : _languageOptions.ChosenLanguage.DuringGame.SignsMeaningList;

				if (lineNumber < currentMessagesLenght)
				{
					string markerName = vall.FirstOrDefault(x => x.Equals(currentMessagesList.ElementAt(lineNumber).Item1));
					int markerValue = (int)Enum.Parse(typeof(Marker), markerName);
					PrintMessage(currentMessagesList.ElementAt(lineNumber).Item2, markerValue);
				}
				else
				{
					Console.Write("{0}", _spaceBetweenBoards);
				}
				PrintFrame();
				for (int y = 0; y < BoardSize.Width; ++y)
				{
					PrintShipArea(rightBoard.GetField(lineNumber, y));
				}
				PrintFrame();
			}
			catch (Exception ex)
			{
				_loggerService.Error(ex);
			}
		}
		private void PrintMessage(string message, int marker)
		{
			Console.Write(WindowSize.DoubleBoardMarker);
			PrintShipArea(marker);
			Console.Write(" -  " + message);
			Console.Write("{0}", new string(WindowSize.BoardMarker, WindowSize.SpaceBeetweenBoardsSize - WindowSize.DoubleBoardMarker.Length * 4 - message.Length));

		}
		private void PrintFrame()
		{
			Console.BackgroundColor = ConsoleColor.White;
			Console.Write(WindowSize.DoubleBoardMarker);
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
					Console.Write(WindowSize.DoubleBoardMarker);
					Console.BackgroundColor = ConsoleColor.Black;
					Console.Write(_horizontalEdgeSeparator);
					break;
				case WindowEdge.Right:
					Console.BackgroundColor = ConsoleColor.Black;
					Console.Write(_horizontalEdgeSeparator);
					Console.BackgroundColor = ConsoleColor.White;
					Console.WriteLine(WindowSize.DoubleBoardMarker);
					Console.BackgroundColor = ConsoleColor.Black;
					break;
				case WindowEdge.Top:
					Console.BackgroundColor = ConsoleColor.White;
					Console.WriteLine(_windowEdge);
					Console.BackgroundColor = ConsoleColor.Black;
					for (int i = 0; i < WindowSize.SeparatorSize; ++i)
					{
						PrintSideEdge();
					}
					break;
				case WindowEdge.Bottom:
					for (int i = 0; i < WindowSize.SeparatorSize; ++i)
					{
						PrintSideEdge();
					}
					Console.BackgroundColor = ConsoleColor.White;
					Console.WriteLine(_windowEdge);
					Console.BackgroundColor = ConsoleColor.Black;
					break;
			}
		}
		private void PrintSideEdge()
		{
			Console.BackgroundColor = ConsoleColor.White;
			Console.Write(WindowSize.DoubleBoardMarker);
			Console.BackgroundColor = ConsoleColor.Black;
			Console.Write(new string(WindowSize.BoardMarker, _windowEdge.Length - 2 * WindowSize.OneBlockWidth));
			Console.BackgroundColor = ConsoleColor.White;
			Console.WriteLine(WindowSize.DoubleBoardMarker);
			Console.BackgroundColor = ConsoleColor.Black;
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
			Console.Write(WindowSize.DoubleBoardMarker);
			Console.BackgroundColor = ConsoleColor.Black;
		}	
		public int ChoseLanguage()
		{
			while (ShowMenuOptions(_languageOptions.AvailableLanguages.Languages)) ;
			return _chosenOption;
		}
		public int ShowMenu(bool hideLastMenuOptions = false)
		{
			while (ShowMenuOptions(_languageOptions.ChosenLanguage.MenuOptions, hideLastMenuOptions)) ;
			return _chosenOption;
		}
		private bool ShowMenuOptions(string[] options, bool hideLastMenuOptions = false)
		{
			Console.Clear();
			PrintWindowEdge(WindowEdge.Top);
			int menuSize = hideLastMenuOptions ? options.Length - 2 : options.Length;
			for (int i = 0; i < WindowSize.Height; i++)
			{

				if (i < menuSize)
				{
					PrintWindowEdge(WindowEdge.Left);
					Console.Write(("")
						.PadRight(WindowSize.BoardEdgeSize + WindowSize.SpaceBeetweenBoardsSize / 2 - (int)Math.Floor(options[i].Length / 2.0)
						, WindowSize.BoardMarker));
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

					Console.Write(options[i]);
					Console.BackgroundColor = ConsoleColor.Black;
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write(("")
						.PadRight(WindowSize.BoardEdgeSize + WindowSize.SpaceBeetweenBoardsSize / 2 - (int)Math.Ceiling(options[i].Length / 2.0)
						, WindowSize.BoardMarker));
					PrintWindowEdge(WindowEdge.Right);
				}
				else
				{
					PrintSideEdge();
				}
			}
			PrintWindowEdge(WindowEdge.Bottom);
			return ReadOption(menuSize);
		}
		private bool ReadOption(int size)
		{
			Keys key = Keys.None;
			if (_inputDevice.KeyAvailable())
			{
				key = _inputDevice.ReadKey();
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
			//_inputDevice.ClearStram();
			return true;
		}
	}
}
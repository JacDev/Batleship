using Battleship.LanguageServices;
using Battleship.Interfaces;
using System;
using System.IO;
using System.Threading;

namespace Battleship
{
	public enum Actions : int { BackToMenu = -1, Missed, EndGame, Undo }

	public class Game
	{

		private int _chosenOption;
		private Player _leftPlayer = null;
		private Player _rightPlayer = null;
		private BoardSide _whoseTurn;
		private readonly ChosenLanguageModel _chosenLanguage;
		private readonly IOutputDevice _outputDevice;
		private readonly IInputDevice _inputDevice;
		private bool _canLoadGame;
		public Game(ChosenLanguageModel chosenLanguage, IOutputDevice outputDevice, IInputDevice inputDevice)
		{
			_chosenLanguage = chosenLanguage;
			_outputDevice = outputDevice;
			_inputDevice = inputDevice;
			_canLoadGame = false;
			MakeGame();
		}
		private void MakeGame()
		{
			bool closeWindow = false;
			while (!closeWindow)
			{
				_chosenOption = _outputDevice.ShowMenu(!_canLoadGame);
				bool endGame = false;
				switch (_chosenOption)
				{
					case 0:
						{
							EndCurrentGame();

							_rightPlayer = new Computer(_outputDevice, BoardSide.Right);
							_leftPlayer = new Person(_outputDevice, BoardSide.Left, _inputDevice, _rightPlayer);
							_rightPlayer.Opponent = _leftPlayer;
							_canLoadGame = true;
							_whoseTurn = BoardSide.Left;
							endGame = Play();
							break;
						}
					case 1:
						{
							EndCurrentGame();
							_leftPlayer = new Person(_outputDevice, BoardSide.Left, _inputDevice);
							_rightPlayer = new Person(_outputDevice, BoardSide.Right, _inputDevice, _leftPlayer);
							_leftPlayer.Opponent = _rightPlayer;
							_canLoadGame = true;
							_whoseTurn = BoardSide.Left;
							endGame = Play();
							break;
						}
					case 2:
						{
							EndCurrentGame();
							LoadGame();
							endGame = Play();
							_canLoadGame = true;
							break;
						}
					case 3:
						{
							closeWindow = true;
							break;
						}
					case 6:
						{
							endGame = Play();
							break;
						}
					case 7:
						{
							SaveGame();
							break;
						}
				}
			}
		}
		private bool Play()
		{
			Player winer;
			while (true)
			{
				Player currentPlayer = _whoseTurn == BoardSide.Left ? _leftPlayer : _rightPlayer;
				Actions currentPlayerAction = currentPlayer.Shoot();
				if (currentPlayerAction == Actions.EndGame)
				{
					winer = currentPlayer;
					break;
				}
				else if (currentPlayerAction == Actions.BackToMenu)
				{
					return false;
				}
				else
				{
					_whoseTurn = _whoseTurn == BoardSide.Left ? BoardSide.Right : BoardSide.Left;
				}
			}
			if (winer.WhichBoard == BoardSide.Left)
			{
				_outputDevice.SetBottomMessage(8);
			}
			else
			{
				_outputDevice.SetBottomMessage(9);
			}
			_outputDevice.PrintBoard(_leftPlayer.Board, _rightPlayer.Board, false);
			Console.WriteLine();
			Console.Write(("").PadRight(40, ' '));
			ReadEnter();
			return true;
		}
		private void EndCurrentGame()
		{
			if (_rightPlayer != null)
			{
				_rightPlayer.Board.ClearBoard();
			}
			if (_leftPlayer != null)
			{
				_leftPlayer.Board.ClearBoard();
			}
			_rightPlayer = _leftPlayer = null;
		}
		public void ReadEnter()
		{
			Console.WriteLine("Press ENTER to continue\n");
			while (_inputDevice.ReadKey() != Keys.Enter) ;
			Thread.Sleep(200);
		}
		private void SaveGame()
		{
			string directory = Directory.GetCurrentDirectory();
			Console.WriteLine(directory);
			using StreamWriter outputFile = File.CreateText(Path.Combine(directory, "savedGame.txt"));
			outputFile.WriteLine(_leftPlayer.IsPerson().ToString() + " " + _rightPlayer.IsPerson().ToString() + " " + _leftPlayer.SunkenShips + " " + _rightPlayer.SunkenShips + " " + _whoseTurn.ToString() + " ");
			outputFile.WriteLine(_leftPlayer.GetShipsAsString() + _rightPlayer.GetShipsAsString() + _leftPlayer.Board.ToString() + "\n" + _rightPlayer.Board.ToString());
			outputFile.Close();
		}
		private bool LoadGame()
		{
			string directory = Directory.GetCurrentDirectory();
			string inputFilePath = Path.Combine(directory, "savedGame.txt");
			if (File.Exists(inputFilePath))
			{
				StreamReader inputFile = File.OpenText(inputFilePath);
				string game = inputFile.ReadToEnd();
				inputFile.Close();
				StringReader reader = new StringReader(game);
				LoadPlayers(reader.ReadLine());

				for (int i = 0; i < 10; ++i)
				{
					_leftPlayer.AddShipAfterLoadGame(reader.ReadLine(), i);
				}
				for (int i = 0; i < 10; ++i)
				{
					_rightPlayer.AddShipAfterLoadGame(reader.ReadLine(), i);
				}

				(_leftPlayer.Board as ILoadable).ReadFromString(reader.ReadLine());
				(_rightPlayer.Board as ILoadable).ReadFromString(reader.ReadLine());
				return true;
			}
			return false;
		}
		private void LoadPlayers(string line)
		{
			char separator = ' ';
			string[] substrings = line.Split(separator);

			_leftPlayer = CreatePlayer(substrings[0], BoardSide.Left);
			_rightPlayer = CreatePlayer(substrings[1], BoardSide.Right);
			_leftPlayer.Opponent = _rightPlayer;
			_rightPlayer.Opponent = _leftPlayer;

			_leftPlayer.SunkenShips = Convert.ToInt32(substrings[2]);
			_rightPlayer.SunkenShips = Convert.ToInt32(substrings[3]);
			_whoseTurn = substrings[4] == "Left"? BoardSide.Left : BoardSide.Right;
		}
		private Player CreatePlayer(string isPerson, BoardSide side)
		{
			if(isPerson == "True")
			{
				return new Person(_outputDevice, side, _inputDevice, afterLoad: true);
			}
			else
			{
				return new Computer(_outputDevice, side, afterLoad: true);			
			}
		}
	}
}
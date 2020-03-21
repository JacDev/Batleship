using System;
using System.IO;
using System.Threading;

namespace Statki
{
	enum Actions : int { BACK_TO_MENU = -1, MISSED, END_GAME, UNDO }

	class Game
	{

		private int chosenOption;
		private Moves leftPlayer = null, rightPlayer = null;
		private BoardSide whoseTurn;
		public Game()
		{
			MakeGame();
		}
		private void MakeGame()
		{
			bool closeWindow = false;
			while (!closeWindow)
			{
				chosenOption = Window.ShowMenu();
				bool endGame = false;
				switch (chosenOption)
				{
					case 0:
						{
							EndCurrentGame();
							leftPlayer = new PersonMoves(BoardSide.Left);
							rightPlayer = new ComputerMoves(BoardSide.Right, leftPlayer);
							leftPlayer.Opponent = rightPlayer;
							Window.CanLoadGame = true;
							whoseTurn = BoardSide.Left;
							endGame = Play();
							break;
						}
					case 1:
						{
							EndCurrentGame();
							leftPlayer = new PersonMoves(BoardSide.Left);
							rightPlayer = new PersonMoves(BoardSide.Right, leftPlayer);
							leftPlayer.Opponent = rightPlayer;
							Window.CanLoadGame = true;
							whoseTurn = BoardSide.Left;
							endGame = Play();
							break;
						}
					case 2:
						{
							EndCurrentGame();
							LoadGame();
							endGame = Play();
							Window.CanLoadGame = true;
							break;
						}
					case 3:
						{
							closeWindow = true;
							break;
						}
					case 4:
						{
							endGame = Play();
							break;
						}
					case 5:
						{
							SaveGame();
							break;
						}
				}
			}
		}
		private bool Play()
		{
			while (true)
			{
				Moves currentPlayer = whoseTurn == BoardSide.Left ? leftPlayer : rightPlayer;
				Actions currentPlayerAction = currentPlayer.Shoot();			
				if (currentPlayerAction == Actions.END_GAME)
				{
					break;
				}
				else if (currentPlayerAction == Actions.BACK_TO_MENU)
				{
					return false;
				}
				else
				{
					whoseTurn = whoseTurn == BoardSide.Left ? BoardSide.Right : BoardSide.Left;
				}
			}

			Window.PrintBoard(leftPlayer, rightPlayer);
			Console.Write(("").PadRight(40, ' '));
			ReadEnter();
			return true;
		}
		private void EndCurrentGame()
		{
			if (rightPlayer != null)
			{
				rightPlayer.ClearBoard();
			}
			if (leftPlayer != null)
			{
				leftPlayer.ClearBoard();
			}
			rightPlayer = leftPlayer = null;
		}
		public void ReadEnter()
		{
			Console.WriteLine("Press ENTER to continue\n");
			while (Window.ReadKey() != Keys.Enter) ;
			Thread.Sleep(200);
		}
		private void SaveGame()
		{
			string directory = Directory.GetCurrentDirectory();
			Console.WriteLine(directory);
			using (StreamWriter outputFile = File.CreateText(Path.Combine(directory, "savedGame.txt")))
			{
				outputFile.WriteLine(leftPlayer.IsPerson().ToString() + " " + rightPlayer.IsPerson().ToString() + " " + leftPlayer.SunkenShips + " " + rightPlayer.SunkenShips + " " + whoseTurn.ToString() + " ");
				outputFile.WriteLine(leftPlayer.GetShipsAsString() + rightPlayer.GetShipsAsString() + leftPlayer.GetBoardAsString() + rightPlayer.GetBoardAsString());
				outputFile.Close();
			}
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
					leftPlayer.AddShipAfterLoadGame(reader.ReadLine(), i + 1);
				}
				for (int i = 0; i < 10; ++i)
				{
					rightPlayer.AddShipAfterLoadGame(reader.ReadLine(), i + 1);
				}
				leftPlayer.LoadBoard(reader.ReadToEnd());
				rightPlayer.LoadBoard(reader.ReadToEnd());
				return true;
			}
			return false;
		}
		private void LoadPlayers(string line)
		{
			char separator = ' ';
			string[] substrings = line.Split(separator);

			leftPlayer = MakePlayer(substrings[0], BoardSide.Left);
			rightPlayer = MakePlayer(substrings[1], BoardSide.Right);
			leftPlayer.Opponent = rightPlayer;
			rightPlayer.Opponent = leftPlayer;

			leftPlayer.SunkenShips = Convert.ToInt32(substrings[2]);
			rightPlayer.SunkenShips = Convert.ToInt32(substrings[3]);
			whoseTurn = substrings[4] == "Left"? BoardSide.Left : BoardSide.Right;
		}
		private Moves MakePlayer(string isPerson, BoardSide side)
		{
			if(isPerson == "True")
			{
				return new PersonMoves(side, afterLoad: true);
			}
			else
			{
				return new ComputerMoves(side, afterLoad: true);			
			}
		}
	}
}
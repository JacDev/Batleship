using System;
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
				chosenOption = Window.Instance.ShowMenu();
				bool endGame = false;
				switch (chosenOption)
				{
					case 0:
						{
							EndCurrentGame();
							Window.Instance.PrintBoard();
							leftPlayer = new PersonMoves(BoardSide.Left);
							rightPlayer = new ComputerMoves(BoardSide.Right, leftPlayer);
							leftPlayer.Opponent = rightPlayer;
							Window.Instance.CanLoadGame = true;
							whoseTurn = BoardSide.Left;
							endGame = Play();
							break;
						}
					case 1:
						{
							EndCurrentGame();
							leftPlayer = new PersonMoves(BoardSide.Left);
							rightPlayer = new ComputerMoves(BoardSide.Right, leftPlayer);
							leftPlayer.Opponent = rightPlayer;
							Window.Instance.CanLoadGame = true;
							whoseTurn = BoardSide.Left;
							endGame = Play();
							break;
						}
					case 2:
						{
							EndCurrentGame();
							//LoadGame
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
							//SaveGame
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
				whoseTurn = whoseTurn == BoardSide.Left ? BoardSide.Right : BoardSide.Left;
				Actions currentPlayerAction = currentPlayer.Shoot();

				if (currentPlayerAction == Actions.END_GAME)
				{
					break;
				}
				else if (currentPlayerAction == Actions.BACK_TO_MENU)
				{
					return false;
				}
			}

			Window.Instance.PrintBoard();
			Console.Write(("").PadRight(40, ' '));
			ReadEnter();
			return true;
		}
		private void EndCurrentGame()
		{
			Board.Instance.ClearBoard();
			rightPlayer = leftPlayer = null;
		}
		public void ReadEnter()
		{
			Console.WriteLine("Press ENTER to continue\n");
			while (Window.Instance.ReadKey() != Keys.Enter) ;
			Thread.Sleep(200);
		}
	}
}
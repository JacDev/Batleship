using System;
using System.Threading;

namespace Statki
{
	class Game
	{
		private int chosenOption;
		private Moves Left = null, Right = null;
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
							Left = new PersonMoves(false);
							Right = new ComputerMoves(true, Left);
							Left.Opponent = Right;
							Window.Instance.CanLoadGame = true;
							endGame = Play();
							break;
						}
					case 1:
						{
							EndCurrentGame();
							Left = new PersonMoves(false);
							Right = new PersonMoves(true, Left);
							Left.Opponent = Right;
							Window.Instance.CanLoadGame = true;
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
				if (!Left.Shoot() || !Right.Shoot())
				{
					if (Left.SunkenShips == 10 || Right.SunkenShips == 10)
					{
						break;
					}
					else
					{
						return false;
					}
				}
			}
			//planszaGry->setCzyJuzUstawilPierwszy(false);
			//planszaGry->setCzyJuzUstawilDrugi(false);
			Window.Instance.PrintBoard();
			Console.Write(("").PadRight(40, ' '));
			Console.WriteLine("Press ENTER to continue\n");
			ReadEnter();
			return true;
		}
		private void EndCurrentGame()
		{
			Board.Instance.ClearBoard();
			Right = Left = null;

		}
		private void ReadEnter()
		{
			while (Window.Instance.ReadKey()!=Keys.ENTER) ;
			Thread.Sleep(200);
		}
	}
}

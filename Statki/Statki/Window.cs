﻿using System;
using System.Threading;

namespace Statki
{
	enum Keys : int { Left, Right, Up, Down, Enter, Escape, Rotate, Undo, Clear, None };
	class Window
	{
		private readonly string[] _options = {
	/*0*/"GRA Z KOMPUTEREM",
	/*1*/"GRA Z PRZECIWNIKIEM",
	/*2*/"WCZYTAJ GRE",
	/*3*/"WYJDZ Z GRY",
	/*4*/"KONTYNUUJ GRE",
	/*5*/"ZAPISZ GRE"
		};
		private string _space = "\t\t\t\t\t\t";
		private string _updown = new string(' ', Board.Width + 4);
		public bool CanLoadGame { get; set; }

		private static readonly Window _Instance = new Window();
		private int _chosenOption;
		private bool _isHighlighted = true;

		private Window()
		{
		}
		public static Window Instance
		{
			get
			{
				return _Instance;
			}
		}
		public void PrintBoard()
		{
			Console.Clear();
			PrintUpDown();
			for (int x = 0; x < Board.Height; ++x)
			{
				PrintLine(x);
			}
			PrintUpDown();
		}
		private void PrintLine(int line)
		{
			PrintFrame();
			for (int y = 0; y < Board.Width; ++y)
			{
				PrintShipArea(Board.Instance[line, y]);
				if (y == Board.RightEdge)
				{
					PrintFrame();
					Console.Write("{0}", _space);
					PrintFrame();
				}
			}
			PrintFrame();
			Console.WriteLine();
		}
		private void PrintFrame()
		{
			Console.BackgroundColor = ConsoleColor.White;
			Console.Write("  ");
			Console.BackgroundColor = ConsoleColor.Black;
		}
		private void PrintUpDown()
		{
			Console.BackgroundColor = ConsoleColor.White;
			Console.Write(_updown);
			Console.BackgroundColor = ConsoleColor.Black;
			Console.Write(_space);
			Console.BackgroundColor = ConsoleColor.White;
			Console.WriteLine(_updown);
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
					break;
			}
			//Console.Write(index + " "); //for debuging
			Console.Write("  ");
		}
		public Keys ReadKey()
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
		public int ShowMenu()
		{
			while (ShowMenuOptions()) ;
			return _chosenOption;
		}
		private bool ShowMenuOptions()
		{
			Console.Clear();
			int menuSize = CanLoadGame ? _options.Length : _options.Length - 2;
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
					_isHighlighted = _isHighlighted ? false : true;
				}
				Console.WriteLine(_options[i]);

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
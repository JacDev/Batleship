using Battleship.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship
{
	public class KeyboardReader : IInputDevice
	{
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
		public bool KeyAvailable()
		{
			return Console.KeyAvailable;
		}
		public void ClearStram()
		{
			while (Console.KeyAvailable)
				Console.ReadKey(true);
		}
	}
}

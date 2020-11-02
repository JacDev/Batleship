using System;
using System.Collections.Generic;
using System.Text;

namespace Statki.Interfaces
{
	public enum Keys : int { Left, Right, Up, Down, Enter, Escape, Rotate, Undo, Clear, None };
	public interface IInputDevicee
	{
		Keys ReadKey();
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Interfaces
{
	public interface ILoadable
	{
		void ReadFromString(string line);
	}
}

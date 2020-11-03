using System;

namespace Battleship.Interfaces
{
	public interface ILoggerService
	{
		public void Warning(Exception ex);
		public void Info(Exception ex);
		public void Error(Exception ex);
	}
}

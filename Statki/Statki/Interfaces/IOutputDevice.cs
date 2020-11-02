using Battleship;

namespace Statki.Interfaces
{
	public interface IOutputDevice
	{
		void PrintBoard(Board leftBoard, Board rightBoard);
		int ShowMenu(string[] options, bool hideLastMenuOptions = false);
	}
}

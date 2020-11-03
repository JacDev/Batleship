namespace Battleship.Interfaces
{
	public interface IOutputDevice
	{
		void PrintBoard(Board leftBoard, Board rightBoard);
		int ShowMenu(bool hideLastMenuOptions = false);
	}
}

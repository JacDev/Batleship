namespace Battleship.Interfaces
{
	public interface IOutputDevice
	{
		void PrintBoard(IBoard leftBoard, IBoard rightBoard);
		int ShowMenu(bool hideLastMenuOptions = false);
		int ChoseLanguage();
	}
}

namespace Battleship.Interfaces
{
	public interface IOutputDevice
	{
		void PrintBoard(IBoard leftBoard, IBoard rightBoard, bool duringAdding);
		int ShowMenu(bool hideLastMenuOptions = false);
		int ChoseLanguage();
	}
}

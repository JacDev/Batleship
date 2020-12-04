namespace Battleship.Interfaces
{
	public interface IOutputDevice
	{
		void PrintBoard(IBoard leftBoard, IBoard rightBoard, bool duringAdding, bool hideLeft = false);
		int ShowMenu(bool hideLastMenuOptions = false);
		int ChoseLanguage();
		void SetBottomMessage(int index);
		int GetBottomMessage();
		void SetTopMessage(int index);
	}
}

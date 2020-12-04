namespace Battleship.Interfaces
{
	public interface IBoard
	{
		int[] GetBoard();
		int GetField(int indx, int indy);
		void SetField(int indx, int indy, int value);
		void SetFieldIf(int x, int y, int val, int condition);
		void ClearNearShipMarks();
		void ClearBoard();
	}
}

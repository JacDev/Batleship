namespace Battleship.Interfaces
{
	public enum Keys : int { Left, Right, Up, Down, Enter, Escape, Rotate, Undo, Clear, None };
	public interface IInputDevice
	{
		Keys ReadKey();
		public bool KeyAvailable();
		void ClearStram();
	}
}

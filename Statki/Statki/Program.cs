using System;
using System.Text;

namespace Statki
{
    class Program
    {
        static void Main(string[] args)
        {
            Board board = Board.Instance;
            Ship a = new Ship(4, 4, 3, 1, false, true);

            Board.PrintBoard();
            Console.ReadKey();
            a.HitShip(4,5);
            Console.Clear();
            //Board.ClearMarks();
            Board.PrintBoard();
        }
    }
}

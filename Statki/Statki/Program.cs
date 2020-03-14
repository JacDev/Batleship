using System;
using System.Text;
using System.Threading;

namespace Statki
{
    class Program
    {
        static void Main(string[] args)
        {
            Moves komp = new ComputerMoves(false);
            //Moves gracz = new PersonMoves(true);

            Board.Instance.PrintBoard();
        }
    }
}

using System;
using System.Text;
using System.Threading;

namespace Statki
{
    class Program
    {
        static void Main(string[] args)
        {
            Board.Init();
            Moves gracz = new PersonMoves(false);
        }
    }
}

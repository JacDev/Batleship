using System;

namespace Statki
{
    class Ship
    {
        enum State : int { _missed, _hit, _sunk };
        private readonly int _coordX;
        private readonly int _coordY;
        private readonly bool _whichBoard;
        public Ship (int icoordX, int icoordY, int isize, int ishipNumber, bool iisVertical, bool whichBoard)
        {
            _coordX = icoordX;
            _coordY = icoordY;
            ShipNumber = ishipNumber;
            IsVertical = iisVertical;
            _whichBoard = whichBoard;
            Left = isize;
            GetShip = new int[isize, 2];
            MakeShip();
            DrawShip();
            MarkNeighborhood(false);
        }
        private void MakeShip()
        {
            int firstCoord = IsVertical ? _coordX : _coordY;
            GetShip[0, 0] = firstCoord;
            GetShip[0, 1] = (int)State._missed;
            for (int i = 1; i < GetShip.GetLength(0); ++i)
            {
                GetShip[i, 0] = GetShip[i - 1, 0] + 1;
                GetShip[i, 1] = (int)State._missed;
            }
        }
        public int Size { get => GetShip.GetLength(0);  }
        public bool IsSink { get => Left == 0; }
        public bool IsVertical { get; }
        public int ShipNumber { get; }
        public int[,] GetShip { get; }
        public int Left { get; private set; }

        private void DrawShip()
        {
            for (int i = 0; i < GetShip.GetLength(0); ++i)
                Board.Instance[this[i].Item1, this[i].Item2, _whichBoard] = ShipNumber + 10 * GetShip[i, 1];
        }
        private void MarkNeighborhood(bool isSink)
        {
            int oznaczenie = 42 + (isSink ? 9 : 0);
            for (int i = 0; i < GetShip.GetLength(0); ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    for (int k = -1; k < 2; ++k)
                        Board.Instance.SetAreaIf(this[i].Item1 + k, this[i].Item2 + j, oznaczenie, (int)Marker.PUSTE_POLE, _whichBoard);
                }
            }
        }
        public Tuple<int, int> this[int i]
        {
            get
            {
                int x = IsVertical ? GetShip[i, 0] : _coordX;
                int y = IsVertical ? _coordY : GetShip[i, 0];
                return new Tuple<int, int> (x,y);
                
            }
        }
        public bool HitShip(int coordX, int coordY)
        {
            int hitArea = IsVertical ? coordX : coordY;
            for(int i = 0; i < GetShip.GetLength(0); ++i)
            {
                if(GetShip[i,0] == hitArea)
                {
                    GetShip[i, 1] = (int)State._hit;
                    --Left;
                    if (Left == 0)
                        SinkShip();
                    DrawShip();
                    break;
                }
            }
            return Left == 0;
        }
        private void SinkShip()
        {
            for (int i = 0; i < GetShip.GetLength(0); ++i)
                GetShip[i, 1] = (int)State._sunk;
        }
    }
}

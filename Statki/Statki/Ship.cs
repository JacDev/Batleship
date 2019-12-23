using System;

namespace Statki
{
    class Ship
    {
        enum state : int { _missed, _hit, _sunk };
        private int _coordX;
        private int _coordY;
        private int _left;
        bool _whichBoard;
        public Ship (int icoordX, int icoordY, int isize, int ishipNumber, bool iisVertical, bool whichBoard)
        {
            _coordX = icoordX;
            _coordY = icoordY;
            ShipNumber = ishipNumber;
            IsVertical = iisVertical;
            _whichBoard = whichBoard;
            _left = isize;
            GetShip = new int[isize, 2];
            MakeShip();
            DrawShip();
            MarkNeighborhood(false);
        }
        private void MakeShip()
        {
            int firstCoord = IsVertical ? _coordX : _coordY;
            GetShip[0, 0] = firstCoord;
            GetShip[0, 1] = (int)state._missed;
            for (int i = 1; i < GetShip.GetLength(0); ++i)
            {
                GetShip[i, 0] = GetShip[i - 1, 0] + 1;
                GetShip[i, 1] = (int)state._missed;
            }
        }
        public int Size { get => GetShip.Length;  }
        public bool IsSink { get => Left == 0; }
        public bool IsVertical { get; }
        public int ShipNumber { get; }
        public int[,] GetShip { get; }
        public int Left { get => _left; }

        private void DrawShip()
        {
            for (int i = 0; i < GetShip.GetLength(0); ++i)
                Board.SetArea(this[i].Item1, this[i].Item2, ShipNumber + 10 * GetShip[i, 1], _whichBoard);
        }
        private void MarkNeighborhood(bool isSink)
        {
            int oznaczenie = 42 + (isSink ? 9 : 0);
            for (int i = 0; i < GetShip.GetLength(0); ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    for (int k = -1; k < 2; ++k)
                        Board.SetAreaIf(this[i].Item1 + k, this[i].Item2 + j, oznaczenie, (int)Marker.PUSTE_POLE, _whichBoard);
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
                    GetShip[i, 1] = (int)state._hit;
                    --_left;
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
                GetShip[i, 1] = (int)state._sunk;
        }
    }
}

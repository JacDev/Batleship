using System;
using System.Collections.Generic;
using System.Text;

namespace Statki
{
    class ShotEvent
    {
		public ShotEvent(int coordX, int coordY, int shipNumb, bool wasHit, Moves whichPlayer)
		{
			CoordX = coordX;
			CoordY = coordY;
			ShipNumb = shipNumb;
			WasHit = wasHit;
			WhichPlayer = whichPlayer;
		}
		public int ShipNumb { get; }
		public int CoordX { get; }
		public int CoordY { get; }
		public bool WasHit { get; }
		public Moves WhichPlayer { get; }

		public void UndoShot()
		{
			if (WasHit)
			{
				WhichPlayer.Opponent.PlayerShips[ShipNumb].UndoHit(CoordX, CoordY);
			}
			else
			{
				Board.Instance[CoordX, CoordY, WhichPlayer.OpponentBoard] = (int)Marker.EMPTY_FIELD;
			}

		}

	}
}

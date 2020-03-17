using System;
using System.Collections.Generic;

namespace Statki
{
    class Events
    {
        private Stack<ShotEvent> shots = new Stack<ShotEvent>();
        private Events()
        {
        }
        public static Events Instance { get; } = new Events();

        public void AddShotEvent(int coordX, int coordY, int shipNumb, bool wasHit, Moves whichPlayer)
        {
            shots.Push(new ShotEvent(coordX, coordY, shipNumb, wasHit, whichPlayer));
        }
        public void UndoShotEvent()
        {
            if (shots.Count != 0)
            {
                shots.Pop().UndoShot();
            }
        }
        public void ClearEvents()
        {
            shots.Clear();
        }
    }
}

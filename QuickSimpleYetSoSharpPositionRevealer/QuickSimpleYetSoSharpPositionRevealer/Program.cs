using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace QuickSimpleYetSoSharpPositionRevealer
{
    class Program
    {
        private static Obj_AI_Hero myHero = ObjectManager.Player;
        private static float LastX, LastY, LastZ = 0;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;         
        }
        private static void OnGameLoad(EventArgs args)
        {
            printPos();
        }
        private static void printPos()
        {
            while (true)
            {
                if (!(LastX == myHero.Position.X) && !(LastY == myHero.Position.Y) && !(LastZ == myHero.Position.Z))
                {
                    Game.PrintChat("Your champion's current position is X: {0} Y: {1} Z: {2}", myHero.Position.X.ToString(), myHero.Position.Y.ToString(), myHero.Position.Z.ToString());
                    LastX = myHero.Position.X;
                    LastY = myHero.Position.Y;
                    LastZ = myHero.Position.Z;
                }
            }
        }
    }
}

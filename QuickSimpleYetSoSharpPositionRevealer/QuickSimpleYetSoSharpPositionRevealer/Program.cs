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
        private static string INODIWPY = "";

        static void Main(string[] args)
        {
            while (1 == 1)
            {
                if (LastX != myHero.Position.X || LastY != myHero.Position.Y || LastZ != myHero.Position.Z)
                {
                    LastX = myHero.Position.X;
                    LastY = myHero.Position.Y;
                    LastZ = myHero.Position.Z;
                    INODIWPY = "Your champion's current position is X: " + LastX.ToString("R") + " Y: " + LastY.ToString("R") + " Z: " + LastZ.ToString("R");
                    Game.PrintChat(INODIWPY);
                }
                System.Threading.Thread.Sleep(3000);
            }
        }
    }
}

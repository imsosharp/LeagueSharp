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
            Game.OnGameUpdate += OnGameUpdate;
        }
        public static void OnGameUpdate(EventArgs args)
        {
            if (LastX != myHero.Position.X || LastY != myHero.Position.Y || LastZ != myHero.Position.Z)
            {
                LastX = myHero.Position.X;
                LastY = myHero.Position.Y;
                LastZ = myHero.Position.Z;
                Game.PrintChat("Your champion's current position is <font color='#FF0011'>X:</font> " + LastX.ToString("R") + " <font color='#07D100'>Y:</font> " + LastY.ToString("R") + " <font color='#0018D1'>Z:</font> " + LastZ.ToString("R"));
            }
        }
    }
}

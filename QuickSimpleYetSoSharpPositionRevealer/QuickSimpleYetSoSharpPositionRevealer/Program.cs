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

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;         
        }
        private static void OnGameLoad(EventArgs args)
        {
            spamPos();
        }
        private static void spamPos()
        {
            while(true)
            {
                Game.PrintChat("Your champion's current position is X: {0} Y: {1} Z: {2}", myHero.Position.X.ToString(), myHero.Position.Y.ToString(), myHero.Position.Z.ToString());
                System.Threading.Thread.Sleep(1500);
            }
        }
    }
}

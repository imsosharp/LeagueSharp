﻿using System;
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
            while (myHero.Level >= 1)
            {
                if (myHero.Position.X != null && myHero.Position.Y != null && myHero.Position.Z != null)
                {
                    Game.PrintChat("myHero.Position not null");
                    if (!(LastX == myHero.Position.X) && !(LastY == myHero.Position.Y) && !(LastZ == myHero.Position.Z))
                    {
                        INODIWPY = "Your champion's current position is X: " + myHero.Position.X.ToString() + " Y: " + myHero.Position.Y.ToString() + " Z: " + myHero.Position.Z.ToString();
                        Game.PrintChat(INODIWPY);

                        LastX = myHero.Position.X;
                        LastY = myHero.Position.Y;
                        LastZ = myHero.Position.Z;
                    }
                }
            }
        }
    }
}

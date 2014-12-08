//Autoplay Plugin of h3h3's AIO Support
//
//All credits go to him. I only wrote whatever is in this file.
//The core is based on version 2.0.7.2
//which you can find here:
//https://github.com/h3h3/LeagueSharp/tree/master/Support

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Support
{
    internal class Autoplay
    {
        public static Obj_AI_Hero bot = ObjectManager.Player;
        private static Obj_AI_Hero carry = null;
        private static Vector3 fountainpos;
        private static Vector3 lanepos;
        private static Vector3 orbwalkingpos1;
        private static Vector3 orbwalkingpos2;
        

        public Autoplay()
        {
            Game.OnGameUpdate += OnUpdate;
            Game.OnGameEnd += OnGameEnd;
        }
        private static void OnUpdate(EventArgs args)
        {
            metaHandler.doChecks();
            doAutoplay();
        }
        
        public void OnGameEnd(EventArgs args)
        {

        }

        private static void doAutoplay()
        {
            lanepos.X = 12557; lanepos.Y = 2578; lanepos.Z = 51; //in front of botlane turret
            fountainpos.X = 424; fountainpos.Y = 396; fountainpos.Z = 182; //middle of fountain
            if (carry == null)
            {
                bot.IssueOrder(GameObjectOrder.MoveTo, lanepos);
                if ((bot.Position.X - lanepos.X < 100) && (bot.Position.Y - lanepos.Y < 100))
                {
                    if (!(ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.Distance(ObjectManager.Player) < 4000) == null))
                    {
                    carry = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.Distance(ObjectManager.Player) < 4000);
                    }
                }
            }
            if (carry != null)
            {
                orbwalkingpos1.X = carry.Position.X + 150; orbwalkingpos1.Y = carry.Position.Y + 150; orbwalkingpos1.Z = carry.Position.Z;
                orbwalkingpos2.X = carry.Position.X - 150; orbwalkingpos2.Y = carry.Position.Y - 150; orbwalkingpos2.Z = carry.Position.Z;
                if (carry.Distance(bot.Position) > 500)
                {
                    bot.IssueOrder(GameObjectOrder.MoveTo, carry.Position);
                }
            }
        }
            

        }

    }

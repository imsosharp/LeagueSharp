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
        private static Obj_AI_Hero tempcarry = null;
        private static Obj_AI_Turret nearestAllyTurret = null;
        private static Vector3 fountainpos;
        private static Vector3 lanepos;
        private static Random rand = new Random(42 * DateTime.Now.Millisecond);
        private static int randint = rand.Next(0, 150);
        private static int blue = 200;
        private static int purple = -200;
        private static int chosen = 0;
        private static Vector3 frontline;
        private static Vector3 safepos;
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
            if (bot.Team == GameObjectTeam.Order) chosen = blue;
            if (bot.Team == GameObjectTeam.Chaos) chosen = purple;
            if (carry == null && tempcarry == null)
            {
                bot.IssueOrder(GameObjectOrder.MoveTo, lanepos);
                if ((bot.Position.X - lanepos.X < 100) && (bot.Position.Y - lanepos.Y < 100))
                {
                    if (!(ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.Distance(ObjectManager.Player) < 4000 && x.IsAlly) == null))
                    {
                    carry = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.Distance(ObjectManager.Player) < 4000 && x.IsAlly);
                    }
                }
            }
            if (carry != null && !(bot.HealthPercentage() < 25))
            {
                frontline.X = carry.Position.X + chosen;
                frontline.Y = carry.Position.Y + chosen;
                frontline.Z = carry.Position.Z;
                orbwalkingpos1.X = carry.Position.X + randint; orbwalkingpos1.Y = carry.Position.Y - randint; orbwalkingpos1.Z = carry.Position.Z;
                orbwalkingpos2.X = carry.Position.X - randint; orbwalkingpos2.Y = carry.Position.Y + randint; orbwalkingpos2.Z = carry.Position.Z;
                if (carry.Distance(bot.Position) > 600 && !(Utility.UnderTurret(carry, true)))
                {
                    bot.IssueOrder(GameObjectOrder.MoveTo, frontline);
                }
                if (Utility.UnderTurret(bot, true))
                {
                    safepos.X = (bot.Position.X + chosen);
                    safepos.Y = (bot.Position.Y + chosen);
                    safepos.Z = (bot.Position.Z);
                    bot.IssueOrder(GameObjectOrder.MoveTo, safepos);
                }
                if (carry.IsDead || carry.Distance(bot.Position) > 6000 || bot.HealthPercentage() < 25)
                {
                    if (ObjectManager.Get<Obj_AI_Turret>().First(x => !x.IsMe && x.Distance(ObjectManager.Player) < 6000 && x.IsAlly) != null)
                    {
                        tempcarry = carry;
                        carry = null;
                        nearestAllyTurret = ObjectManager.Get<Obj_AI_Turret>().First(x => !x.IsMe && x.Distance(ObjectManager.Player) < 6000 && x.IsAlly);
                        if (bot.Distance(nearestAllyTurret) > 100)
                        {
                            bot.IssueOrder(GameObjectOrder.MoveTo, nearestAllyTurret.Position);
                        }
                        else 
                        {
                            ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Recall);
                        }
                        if (Utility.InFountain())
                        {
                            metaHandler.doChecks();
                            carry = tempcarry;
                            tempcarry = null;
                        }
                    }
                    
                }
            }
        }
            

        }

    }

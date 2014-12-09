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
        public static Obj_AI_Hero carry = null;
        private static Obj_AI_Hero tempcarry = null;
        private static Obj_AI_Turret nearestAllyTurret = null;
        private static Vector3 bluefountainpos;
        private static Vector3 purplefountainpos;
        private static Vector3 lanepos;
        private static Random rand = new Random(42 * DateTime.Now.Millisecond);
        private static int randint = rand.Next(0, 150);
        private static int blue = 200;
        private static int purple = -200;
        private static int chosen = 0;
        private static int safe = 0;
        private static Vector3 frontline;
        private static Vector3 safepos;
        private static Vector3 saferecall;
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
            bluefountainpos.X = 424; bluefountainpos.Y = 396; bluefountainpos.Z = 182; //middle of blue fountain
            purplefountainpos.X = 14354; purplefountainpos.Y = 14428; purplefountainpos.Z = 171; //middle of purple fountain
            if (bot.Team == GameObjectTeam.Order) { chosen = blue; safe = purple; saferecall.X = 7836; saferecall.Y = 804; saferecall.Z = 49.4561234F; }
            if (bot.Team == GameObjectTeam.Chaos) { chosen = purple; safe = blue; saferecall.X = 14128; saferecall.Y = 6908; saferecall.Z = 52.3063F; }
            if (carry == null && tempcarry != null)
            {
                if (Utility.InFountain())
                {
                    metaHandler.doChecks();
                    if (tempcarry != null && ((bot.Health / bot.MaxHealth) * 100) > 80)
                    {
                        carry = tempcarry;
                    }
                }
            }
            if (carry == null && tempcarry == null)
            {
                if (Utility.InFountain() && ((bot.Health / bot.MaxHealth) * 100) > 80)
                {

                    bot.IssueOrder(GameObjectOrder.MoveTo, lanepos);                    
                }
                if ((bot.Position.X - lanepos.X < 100) && (bot.Position.Y - lanepos.Y < 100))
                {
                    if (!(ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.Distance(bot) < 4000 && x.IsAlly) == null))
                    {
                        carry = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.Distance(bot) < 4000 && x.IsAlly);
                    }
                }
            }
            if (carry != null)
            {
                frontline.X = carry.Position.X + chosen;
                frontline.Y = carry.Position.Y + chosen;
                frontline.Z = carry.Position.Z;
                orbwalkingpos1.X = frontline.X + randint; orbwalkingpos1.Y = frontline.Y - randint; orbwalkingpos1.Z = frontline.Z;
                orbwalkingpos2.X = frontline.X - randint; orbwalkingpos2.Y = frontline.Y + randint; orbwalkingpos2.Z = frontline.Z;
                if (carry.Distance(bot.Position) > 600 && !(Utility.UnderTurret(carry, true)))
                {
                    bot.IssueOrder(GameObjectOrder.MoveTo, frontline);
                }

                if (Utility.UnderTurret(bot, true))
                {
                    safepos.X = (bot.Position.X + safe);
                    safepos.Y = (bot.Position.Y + safe);
                    safepos.Z = (bot.Position.Z);
                    bot.IssueOrder(GameObjectOrder.MoveTo, safepos);
                }
                if ((carry.IsDead || ((carry.Distance(bluefountainpos) < 1000 || carry.Distance(purplefountainpos) < 1000) && bot.Distance(carry) > 3000) || ((bot.Health / bot.MaxHealth) * 100) < 25) && !(Utility.InFountain()))
                {
                    nearestAllyTurret = ObjectManager.Get<Obj_AI_Turret>().First(x => !x.IsMe && x.Distance(bot) < 6000 && x.IsAlly);
                    if (nearestAllyTurret != null)
                    {
                        tempcarry = carry;
                        carry = null;
                        
                        bot.IssueOrder(GameObjectOrder.MoveTo, nearestAllyTurret.Position);
                        Util.Helpers.PrintMessage("moving to nearest turret");
                        
                    }
                    
                }
                if (bot.UnderTurret(false) && !(carry.IsDead) && (carry.Distance(bluefountainpos) > 3000 || carry.Distance(purplefountainpos) > 3000) && ((bot.Health / bot.MaxHealth) * 100) < 25)
                {
                    tempcarry = carry;
                }

                
            }
            if (!(tempcarry == null) && carry == null && !(Utility.InFountain()))
            {
                if (bot.UnderTurret(false))
                {
                    Util.Helpers.PrintMessage("hide on bush");
                    bot.IssueOrder(GameObjectOrder.MoveTo, saferecall);
                }
                if (bot.Position == saferecall || bot.Distance(saferecall) < 100)
                {
                    Util.Helpers.PrintMessage("Trying to recall");
                    //Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(ObjectManager.Player.NetworkId, SpellSlot.Recall)).Send(); //disabled packet casting
                    bot.Spellbook.CastSpell(SpellSlot.Recall);
                }
            }
        }
            

        }

    }

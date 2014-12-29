//Autoplay Plugin of h3h3's AIO Support
//
//All credits go to him. I only wrote whatever is in this file.
//The core is based on version 2.0.7.2
//which you can find here:
//https://github.com/h3h3/LeagueSharp/tree/master/Support

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Support
{
    internal class Autoplay
    {
        private const int Blue = 200;
        private const int Purple = -200;
        public static Obj_AI_Hero Bot = ObjectManager.Player;
        public static Obj_AI_Hero Carry;
        private static Obj_AI_Hero _tempcarry;
        private static Obj_AI_Turret _nearestAllyTurret;
        private static Vector3 _bluefountainpos;
        private static Vector3 _purplefountainpos;
        private static Vector3 _lanepos;
        private static int _chosen;
        private static int _safe;
        private static Vector3 _frontline;
        private static Vector3 _safepos;
        private static Vector3 _saferecall;
        private static Vector3 _orbwalkingpos1;
        private static Vector3 _orbwalkingpos2;
        private static int _loaded;
        private static int _lastTimeSeenWalking;
        private static Vector3 _lastPosition;

        public Autoplay()
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
            Game.OnGameUpdate += OnUpdate;
            Game.OnGameEnd += OnGameEnd;
        }

        private static void OnGameLoad(EventArgs args)
        {
            _loaded = Environment.TickCount;
            _bluefountainpos.X = 424;
            _bluefountainpos.Y = 396;
            _bluefountainpos.Z = 182; //middle of blue fountain
            _purplefountainpos.X = 14354;
            _purplefountainpos.Y = 14428;
            _purplefountainpos.Z = 171; //middle of purple fountain
            if (Bot.Team == GameObjectTeam.Order)
            {
                _chosen = Blue;
                _safe = Purple;
                _saferecall.X = 7836;
                _saferecall.Y = 804;
                _saferecall.Z = 49.4561234F;
                _lanepos.X = 11376;
                _lanepos.Y = 1062;
                _lanepos.Z = 50.7677F;
            }
            if (Bot.Team == GameObjectTeam.Chaos)
            {
                _chosen = Purple;
                _safe = Blue;
                _saferecall.X = 14128;
                _saferecall.Y = 6908;
                _saferecall.Z = 52.3063F;
                _lanepos.X = 13496;
                _lanepos.Y = 4218;
                _lanepos.Z = 51.97616F;
            }
            Game.PrintChat("Loaded: " + _loaded);
        }

        private static void OnUpdate(EventArgs args)
        {
            DoAutoplay();
            MetaHandler.DoChecks();
        }

        public void OnGameEnd(EventArgs args)
        {
            Game.Say("gg");
        }

        public static bool AllyInFountain(Obj_AI_Hero hero)
        {
            float fountainRange = 750;
            var map = Utility.Map.GetMap();
            if (map != null && map._MapType == Utility.Map.MapType.SummonersRift)
            {
                fountainRange = 1050;
            }
            return
            ObjectManager.Get<GameObject>()
            .Where(spawnPoint => spawnPoint is Obj_SpawnPoint && spawnPoint.Team == hero.Team)
            .Any(
            spawnPoint =>
            Vector2.Distance(hero.Position.To2D(), spawnPoint.Position.To2D()) <
            fountainRange);
        }

        public static void DoAutoplay()
        {
            var timeElapsed = Environment.TickCount - _loaded;
            if (!Bot.IsDead)
            {
                try
                {
                    if (Bot.UnderTurret(true))
                    {
                        _safepos.X = (Bot.Position.X + _safe);
                        _safepos.Y = (Bot.Position.Y + _safe);
                        _safepos.Z = (Bot.Position.Z);
                        Bot.IssueOrder(GameObjectOrder.MoveTo, _safepos);
                    }
                    #region Carry is null
                    if (Carry == null && timeElapsed > 15000 && timeElapsed < 60000)
                    {
                        if (Utility.InFountain())
                        {
                            Bot.IssueOrder(GameObjectOrder.MoveTo, _lanepos);
                        }
                        if (Geometry.Distance(Bot, _lanepos) < 300)
                        {
                            if (ObjectManager.Get<Obj_AI_Hero>()
                                    .FirstOrDefault(x => !x.IsMe && Geometry.Distance(x, Bot) < 5000 && x.IsAlly) != null)
                            {
                                Carry =
                                    ObjectManager.Get<Obj_AI_Hero>()
                                        .FirstOrDefault(x => !x.IsMe && Geometry.Distance(x, Bot) < 5000 && x.IsAlly);
                            }
                        }
                    }
                    #endregion
                    #region Carry is dead
                    if (Carry != null)
                    {
                        if (Carry.IsDead || AllyInFountain(Carry) && !((Bot.Health / Bot.MaxHealth) * 100 < 30))
                        {
                            Game.PrintChat("Carry dead, following: " + _tempcarry.ChampionName);
                            _tempcarry = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.IsAlly && !AllyInFountain(x));
                            if (_tempcarry != null)
                            {
                                _frontline.X = _tempcarry.Position.X + _chosen;
                                _frontline.Y = _tempcarry.Position.Y + _chosen;
                                _frontline.Z = _tempcarry.Position.Z;
                                if (!(_tempcarry.UnderTurret(true)))
                                {
                                    if (Geometry.Distance(_tempcarry, Bot) < 500)
                                    {
                                        Bot.IssueOrder(GameObjectOrder.MoveTo, _frontline);
                                    }
                                }
                            }
                        }
                    }
                    #endregion Carry is dead
                    #region Following
                    if (Carry != null && Geometry.Distance(Carry, Bot) > 500 && !Carry.IsDead &&
                        !((Bot.Health / Bot.MaxHealth) * 100 < 30) && !(Carry.UnderTurret(true)))
                    {
                        Game.PrintChat("All good, following: " + Carry.ChampionName);
                        _frontline.X = Carry.Position.X + _chosen;
                        _frontline.Y = Carry.Position.Y + _chosen;
                        _frontline.Z = Carry.Position.Z;
                        Bot.IssueOrder(GameObjectOrder.MoveTo, _frontline);
                    }
                    #endregion Following
                    #region Carry not found
                    if (timeElapsed > 60000 &&
                        Carry == null && !((Bot.Health / Bot.MaxHealth) * 100 < 30))
                    {
                        Game.PrintChat("Carry not found, following: " + _tempcarry.ChampionName);
                        _tempcarry =
                            ObjectManager.Get<Obj_AI_Hero>()
                                .FirstOrDefault(x => !x.IsMe && x.IsAlly);
                        _frontline.X = _tempcarry.Position.X + _chosen;
                        _frontline.Y = _tempcarry.Position.Y + _chosen;
                        _frontline.Z = _tempcarry.Position.Z;
                        if (!(_tempcarry.UnderTurret(true)))
                        {
                            if (Bot.Distance(_frontline) < 500)
                            {
                                Bot.IssueOrder(GameObjectOrder.MoveTo, _frontline);
                            }
                        }
                    }
                    #endregion
                    #region Lowhealth mode
                    if ((Bot.Health / Bot.MaxHealth) * 100 < 30)
                    {
                        _nearestAllyTurret =
                            ObjectManager.Get<Obj_AI_Turret>()
                                .FirstOrDefault(x => !x.IsMe && Geometry.Distance(x, Bot) < 6000 && x.IsAlly);
                        if (_nearestAllyTurret != null)
                        {
                            _saferecall.X = _nearestAllyTurret.Position.X + _safe;
                            _saferecall.Y = _nearestAllyTurret.Position.Y;
                            _saferecall.Z = _nearestAllyTurret.Position.Z;
                            if (Bot.Position.Distance(_saferecall) < 200)
                            {
                                Bot.Spellbook.CastSpell(SpellSlot.Recall);
                            }
                            else
                            {

                                Bot.IssueOrder(GameObjectOrder.MoveTo, _saferecall);
                            }

                        }

                    }
                    #endregion
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }



            /*if (carry == null && tempcarry != null)
            {
                if (Utility.InFountain())
                {
                    if (tempcarry != null && ((bot.Health / bot.MaxHealth) * 100) > 80)
                    {
                        carry = tempcarry;
                        tempcarry = null;
                        
                    }
                }
            }
            if (carry != null && Utility.InFountain())
            {
                frontline.X = carry.Position.X + chosen;
                frontline.Y = carry.Position.Y + chosen;
                frontline.Z = carry.Position.Z;

                bot.IssueOrder(GameObjectOrder.MoveTo, frontline);
            }
            if (carry != null && Utility.InFountain() || Utility.InShopRange())
            {

                frontline.X = carry.Position.X + chosen;
                frontline.Y = carry.Position.Y + chosen;
                frontline.Z = carry.Position.Z;
                bot.IssueOrder(GameObjectOrder.MoveTo, frontline);
            }
            if (carry == null && tempcarry == null)
            {
                if (Utility.InFountain() && ((bot.Health / bot.MaxHealth) * 100) > 80)
                {

                    bot.IssueOrder(GameObjectOrder.MoveTo, lanepos);
                }
                if ((bot.Position.X - lanepos.X < 100) && (bot.Position.Y - lanepos.Y < 100))
                {
                    if (!(ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && Geometry.Distance(x, Bot) < 4000 && x.IsAlly) == null))
                    {
                        carry = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && Geometry.Distance(x, Bot) < 4000 && x.IsAlly);
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
                if ((carry.IsDead || ((carry.Distance(bluefountainpos) < 1000 || carry.Distance(purplefountainpos) < 1000) && bot.Distance(carry, false) > 3000) || ((bot.Health / bot.MaxHealth) * 100) < 25) && !(Utility.InFountain()))
                {
                    nearestAllyTurret = ObjectManager.Get<Obj_AI_Turret>().First(x => !x.IsMe && Geometry.Distance(x, Bot) < 6000 && x.IsAlly);
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
            if (bot.UnderTurret(false) && carry == null && tempcarry != null && !Utility.InFountain())
            {
                Util.Helpers.PrintMessage("hide on bush");
                bot.IssueOrder(GameObjectOrder.MoveTo, saferecall);
            }*/
        }

        private static readonly Random rand = new Random(42 * DateTime.Now.Millisecond);
        private static int randint = rand.Next(0, 150);
    }
}
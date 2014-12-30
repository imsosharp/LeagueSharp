//Autoplay Plugin of h3h3's AIO Support
//
//All credits go to him. I only wrote whatever is in this file.
//The core is based on version 2.0.7.2
//which you can find here:
//https://github.com/h3h3/LeagueSharp/tree/master/Support

using System;
using System.Drawing.Text;
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
        private static Vector3 _lanepos;
        private static int _chosen;
        private static int _safe;
        private static Vector3 _frontline;
        private static Vector3 _safepos;
        private static Vector3 _saferecall;
        private static Vector3 _orbwalkingpos;
        private static int _loaded;
        private static  Random rand = new Random(42 * DateTime.Now.Millisecond);
        private static int _randSeconds, _randRange, _stepTime = 0;

        public Autoplay()
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
            Game.OnGameUpdate += OnUpdate;
            Game.OnGameEnd += OnGameEnd;
        }

        private static void OnGameLoad(EventArgs args)
        {
            _loaded = Environment.TickCount;
            _stepTime = Environment.TickCount;
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
            Game.PrintChat("AutoSharpporting Loaded: " + _loaded);
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

        private static bool IsBotSafe()
        {
            if (Bot.InFountain() && !((Bot.Health / Bot.MaxHealth) * 100 < 90))
            {
                return false;
            }
                return true;

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
                    if (Carry == null && timeElapsed > 15000 && timeElapsed < 125000)
                    {
                        if (Bot.InFountain())
                        {
                            Bot.IssueOrder(GameObjectOrder.MoveTo, _lanepos);
                        }
                        if (Geometry.Distance(Bot, _lanepos) < 450)
                        {
                            if (ObjectManager.Get<Obj_AI_Hero>()
                                    .FirstOrDefault(x => !x.IsMe && Geometry.Distance(x, Bot) < 6000 && x.IsAlly) != null)
                            {
                                Carry =
                                    ObjectManager.Get<Obj_AI_Hero>()
                                        .FirstOrDefault(x => !x.IsMe && Geometry.Distance(x, Bot) < 6000 && x.IsAlly);
                            }
                            WalkAround();
                        }
                    }
                    #endregion
                    #region Carry is dead
                    if (Carry != null)
                    {
                        if (Carry.IsDead || Carry.InFountain() && !((Bot.Health / Bot.MaxHealth) * 100 < 30))
                        {
                            if (
                                ObjectManager.Get<Obj_AI_Hero>()
                                        .FirstOrDefault(x => !x.IsMe && x.IsAlly && !x.InFountain() && !x.IsDead && x.ChampionName != Carry.ChampionName) != null)
                            {
                                _tempcarry =
                                    ObjectManager.Get<Obj_AI_Hero>()
                                        .FirstOrDefault(x => !x.IsMe && x.IsAlly && !x.InFountain() && !x.IsDead && x.ChampionName != Carry.ChampionName);
                            }
                            if (_tempcarry != null)
                            {

                                Console.WriteLine("Carry dead or afk, following: " + _tempcarry.ChampionName);
                                _frontline.X = _tempcarry.Position.X + _chosen;
                                _frontline.Y = _tempcarry.Position.Y + _chosen;
                                _frontline.Z = _tempcarry.Position.Z;
                                if (!(_tempcarry.UnderTurret(true)) && IsBotSafe())
                                {
                                    if (Geometry.Distance(_tempcarry, Bot) > 450)
                                    {
                                        Bot.IssueOrder(GameObjectOrder.MoveTo, _frontline);
                                        WalkAround(_tempcarry);
                                    }
                                }
                            }
                        }
                    }
                    #endregion Carry is dead
                    #region Following
                    if (Carry != null && !Carry.IsDead && !Carry.InFountain() &&
                        !((Bot.Health / Bot.MaxHealth) * 100 < 30) && !(Carry.UnderTurret(true)))
                    {
                        Console.WriteLine("All good, following: " + Carry.ChampionName);
                        _frontline.X = Carry.Position.X + _chosen;
                        _frontline.Y = Carry.Position.Y + _chosen;
                        _frontline.Z = Carry.Position.Z;
                        if (!Carry.UnderTurret() && Geometry.Distance(Carry, Bot) > 450 && IsBotSafe())
                        {
                            Bot.IssueOrder(GameObjectOrder.MoveTo, _frontline);
                        }

                        WalkAround(Carry);
                    }
                    #endregion Following
                    #region Carry not found
                    if (timeElapsed > 125000 &&
                        Carry == null && !((Bot.Health / Bot.MaxHealth) * 100 < 30))
                    {
                        if (
                                ObjectManager.Get<Obj_AI_Hero>()
                                        .FirstOrDefault(x => !x.IsMe && x.IsAlly && !x.InFountain() && !x.IsDead && x.ChampionName != Carry.ChampionName) != null)
                        {
                            _tempcarry =
                                ObjectManager.Get<Obj_AI_Hero>()
                                    .FirstOrDefault(x => !x.IsMe && x.IsAlly && !x.InFountain() && !x.IsDead && x.ChampionName != Carry.ChampionName);
                        }
                        if (_tempcarry != null)
                        {
                            Console.WriteLine("Carry not found, following: " + _tempcarry.ChampionName);
                            _frontline.X = _tempcarry.Position.X + _chosen;
                            _frontline.Y = _tempcarry.Position.Y + _chosen;
                            _frontline.Z = _tempcarry.Position.Z;
                            if (!(_tempcarry.UnderTurret(true)) && IsBotSafe())
                            {
                                if (Bot.Distance(_frontline) > 450)
                                {
                                    Bot.IssueOrder(GameObjectOrder.MoveTo, _frontline);
                                }
                                WalkAround(_tempcarry);
                            }
                        }
                    }
                    #endregion
                    #region Lowhealth mode
                    if ((Bot.Health / Bot.MaxHealth) * 100 < 30)
                    {
                        _nearestAllyTurret =
                            ObjectManager.Get<Obj_AI_Turret>()
                                .FirstOrDefault(x => !x.IsMe && x.IsAlly);
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
        } //end of DoAutoplay()

        private static void WalkAround()
        {
            _randRange = rand.Next(-267, 276);
            _randSeconds = rand.Next(1000, 4000);
            if (Environment.TickCount - _stepTime >= _randSeconds)
            {
                if (Bot.Team == GameObjectTeam.Order)
                {
                    int orbwalkingAdditionInteger = _randRange * (-1);
                    _orbwalkingpos.X = Bot.Position.X + orbwalkingAdditionInteger;
                    _orbwalkingpos.Y = Bot.Position.Y + orbwalkingAdditionInteger;
                    _orbwalkingpos.Z = Bot.Position.Z;
                }
                else
                {
                    int orbwalkingAdditionInteger = _randRange;
                    _orbwalkingpos.X = Bot.Position.X + orbwalkingAdditionInteger;
                    _orbwalkingpos.Y = Bot.Position.Y + orbwalkingAdditionInteger;
                    _orbwalkingpos.Z = Bot.Position.Z;
                }
                if (_orbwalkingpos != null)
                {
                    Bot.IssueOrder(GameObjectOrder.MoveTo, _orbwalkingpos);
                    _stepTime = Environment.TickCount;
                }
            }

        } //end of WalkAround()

        private static void WalkAround(Obj_AI_Hero follow)
        {
            _randRange = rand.Next(-267, 276);
            _randSeconds = rand.Next(1000, 7000);
            if (Environment.TickCount - _stepTime >= _randSeconds)
            {
                if (Bot.Team == GameObjectTeam.Order)
                {
                    int orbwalkingAdditionInteger = _randRange * (-1);
                    _orbwalkingpos.X = follow.Position.X + orbwalkingAdditionInteger;
                    _orbwalkingpos.Y = follow.Position.Y + orbwalkingAdditionInteger;
                    _orbwalkingpos.Z = follow.Position.Z;
                }
                else
                {
                    int orbwalkingAdditionInteger = _randRange;
                    _orbwalkingpos.X = follow.Position.X + orbwalkingAdditionInteger;
                    _orbwalkingpos.Y = follow.Position.Y + orbwalkingAdditionInteger;
                    _orbwalkingpos.Z = follow.Position.Z;
                }
                if (_orbwalkingpos != null)
                {
                    Bot.IssueOrder(GameObjectOrder.MoveTo, _orbwalkingpos);
                    _stepTime = Environment.TickCount;
                }
            }

        } //end of WalkAround(Obj_AI_Hero)
    }
}
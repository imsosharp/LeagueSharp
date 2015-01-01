/* Autoplay Plugin of h3h3's AIO Support
*
* All credits go to him. I only wrote this and
* MetaHandler.cs.
* The core is always updated to latest version.
* which you can find here:
* https://github.com/h3h3/LeagueSharp/tree/master/Support
*/

using System;
using System.Collections.Generic;
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
        public static Obj_AI_Hero NearestAllyHero;
        public static Obj_AI_Turret NearestAllyTurret;
        public static Obj_AI_Hero Jungler;
        private static Obj_AI_Hero _tempcarry;
        private static Vector2 _lanepos;
        private static int _chosen;
        private static int _safe;
        private static Vector2 _frontline;
        private static Vector2 _safepos;
        private static Vector2 _saferecall;
        private static Vector2 _orbwalkingpos;
        private static int _loaded;
        private static bool _byPassLoadedCheck = false;
        private static readonly Random Rand = new Random(42 * DateTime.Now.Millisecond);
        private static int _randSeconds, _randRange, _stepTime;
        private static float _lowHealthRatio = 0.3f;
        private static bool _byPassFountainCheck = false;

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
            var map = Utility.Map.GetMap();
            if (map != null && map.Type == Utility.Map.MapType.SummonersRift)
            {
                if (Bot.Team == GameObjectTeam.Order)
                {
                    _chosen = Blue;
                    _safe = Purple;
                    _lanepos.X = 11376;
                    _lanepos.Y = 1062;
                }
                if (Bot.Team == GameObjectTeam.Chaos)
                {
                    _chosen = Purple;
                    _safe = Blue;
                    _lanepos.X = 13496;
                    _lanepos.Y = 4218;
                }
            }
            else
            {
                _byPassLoadedCheck = true;
                if (Bot.Team == GameObjectTeam.Order)
                {
                    _chosen = Blue;
                    _safe = Purple;
                }
                if (Bot.Team == GameObjectTeam.Chaos)
                {
                    _chosen = Purple;
                    _safe = Blue;
                }
            }
            if (map != null && map.Type == Utility.Map.MapType.HowlingAbyss)
            {
                _lowHealthRatio = 0.0f;
                _byPassFountainCheck = true;
            }
            if (MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && MetaHandler.HasSmite(hero)) != null)
            {
                Jungler = MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && MetaHandler.HasSmite(hero));
            }
            Game.PrintChat("AutoSharpporting Loaded: " + _loaded);
            AutoLevel levelUpSpells = new AutoLevel(TreesAutoLevel.GetSequence());
            AutoLevel.Enabled(true);
            SpellHumanizer.Enabled = true;
        }

        private static void OnUpdate(EventArgs args)
        {
            DoAutoplay();
            MetaHandler.DoChecks();
            MetaHandler.UpdateObjects();
        }

        public static void OnGameEnd(EventArgs args)
        {
            Game.Say("gg");
        }

        private static bool IsBotSafe()
        {
            if (Bot.InFountain())
            {
                return (Bot.Health > Bot.MaxHealth * 0.9f) || _byPassFountainCheck;
            }
            return (Bot.Health > Bot.MaxHealth * _lowHealthRatio) && !Bot.IsRecalling();

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
                        Bot.IssueOrder(GameObjectOrder.MoveTo, _safepos.To3D());
                    }
                    #region Carry is null
                    if (Carry == null && timeElapsed > 15000 && timeElapsed < 135000 && !_byPassLoadedCheck)
                    {
                        if (Bot.InFountain() || Bot.Distance(_lanepos) > 400)
                        {
                            Bot.IssueOrder(GameObjectOrder.MoveTo, _lanepos.To3D());
                        }
                        if (Bot.Distance(_lanepos) < 400)
                        {

                            WalkAround(_lanepos.To3D());
                            if (MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && hero.Distance(Bot) < 6000 && hero != Jungler) != null)
                            {
                                Carry = MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && hero.Distance(Bot) < 6000 && hero != Jungler && !MetaHandler.HasSmite(hero));
                            }
                        }
                    }
                    if (_byPassLoadedCheck && Carry == null)
                    {
                        if (MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && hero != Jungler) != null)
                        {
                            Carry = MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && hero != Jungler);
                        }
                    }
                    #endregion
                    #region Carry is dead
                    if (Carry != null)
                    {
                        if (IsBotSafe() && Carry.IsDead || Carry.InFountain())
                        {
                            if (MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead && hero != Jungler) != null)
                            {
                                _tempcarry = MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead && hero != Jungler);
                            }
                            if (MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead && hero != Jungler) == null &&
                                MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead) != null)
                            {
                                //well fuck, let's follow the jungler -sighs-
                                _tempcarry = MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead);
                            }
                            if (_tempcarry != null)
                            {

                                Console.WriteLine("Carry dead or afk, following: " + _tempcarry.ChampionName);
                                _frontline.X = _tempcarry.Position.X + _chosen;
                                _frontline.Y = _tempcarry.Position.Y + _chosen;
                                if (!(_tempcarry.UnderTurret(true)) && IsBotSafe())
                                {
                                    if (_tempcarry.Distance(Bot) > 450)
                                    {
                                        Bot.IssueOrder(GameObjectOrder.MoveTo, _frontline.To3D());
                                        WalkAround(_tempcarry);
                                    }
                                }
                            }
                        }
                    }
                    #endregion Carry is dead
                    #region Following
                    if (Carry != null && !Carry.IsDead && !Carry.InFountain() && IsBotSafe() && !(Carry.UnderTurret(true)))
                    {
                        Console.WriteLine("All good, following: " + Carry.ChampionName);
                        _frontline.X = Carry.Position.X + _chosen;
                        _frontline.Y = Carry.Position.Y + _chosen;
                        if (!Carry.UnderTurret() && Carry.Distance(Bot) > 450)
                        {
                            Bot.IssueOrder(GameObjectOrder.MoveTo, _frontline.To3D());
                        }

                        WalkAround(Carry);
                    }
                    #endregion Following
                    #region Carry not found
                    if (timeElapsed > 135000 &&
                        Carry == null && IsBotSafe())
                    {
                        if (MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead && hero != Jungler) != null)
                        {
                            _tempcarry = MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead && hero != Jungler);
                        }
                        if (MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead && hero != Jungler) == null &&
                            MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead) != null)
                        {
                            //well fuck, let's follow the jungler -sighs-
                            _tempcarry = MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead);
                        }
                        if (_tempcarry != null)
                        {
                            Console.WriteLine("Carry not found, following: " + _tempcarry.ChampionName);
                            _frontline.X = _tempcarry.Position.X + _chosen;
                            _frontline.Y = _tempcarry.Position.Y + _chosen;
                            if (!(_tempcarry.UnderTurret(true)) && IsBotSafe())
                            {
                                if (Bot.Distance(_frontline) > 450)
                                {
                                    Bot.IssueOrder(GameObjectOrder.MoveTo, _frontline.To3D());
                                }
                                WalkAround(_tempcarry);
                            }
                        }
                    }
                    #endregion
                    #region Lowhealth mode
                    if (!IsBotSafe() && !Bot.InFountain())
                    {
                        NearestAllyTurret = MetaHandler.AllyTurrets.FirstOrDefault();
;                        if (NearestAllyTurret != null)
                        {
                            _saferecall.X = NearestAllyTurret.Position.X + _safe;
                            _saferecall.Y = NearestAllyTurret.Position.Y;
                            if (Bot.Position.Distance(_saferecall.To3D()) < 200)
                            {
                                Bot.Spellbook.CastSpell(SpellSlot.Recall);
                            }
                            else
                            {

                                Bot.IssueOrder(GameObjectOrder.MoveTo, _saferecall.To3D());
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

        private static void WalkAround(Vector3 pos)
        {
            _randRange = Rand.Next(-267, 276);
            _randSeconds = Rand.Next(1000, 4000);
            if (Environment.TickCount - _stepTime >= _randSeconds)
            {
                if (Bot.Team == GameObjectTeam.Order)
                {
                    int orbwalkingAdditionInteger = _randRange * (-1);
                    _orbwalkingpos.X = Bot.Position.X + orbwalkingAdditionInteger;
                    _orbwalkingpos.Y = Bot.Position.Y + orbwalkingAdditionInteger;
                }
                else
                {
                    int orbwalkingAdditionInteger = _randRange;
                    _orbwalkingpos.X = Bot.Position.X + orbwalkingAdditionInteger;
                    _orbwalkingpos.Y = Bot.Position.Y + orbwalkingAdditionInteger;
                }
                if (_orbwalkingpos != null)
                {
                    Bot.IssueOrder(GameObjectOrder.MoveTo, _orbwalkingpos.To3D());
                    _stepTime = Environment.TickCount;
                }
            }

        } //end of WalkAround()

        private static void WalkAround(Obj_AI_Hero follow)
        {
            _randRange = Rand.Next(-267, 276);
            _randSeconds = Rand.Next(1000, 4000);
            if (Environment.TickCount - _stepTime >= _randSeconds)
            {
                if (Bot.Team == GameObjectTeam.Order)
                {
                    int orbwalkingAdditionInteger = _randRange * (-1);
                    _orbwalkingpos.X = follow.Position.X + orbwalkingAdditionInteger;
                    _orbwalkingpos.Y = follow.Position.Y + orbwalkingAdditionInteger;
                }
                else
                {
                    int orbwalkingAdditionInteger = _randRange;
                    _orbwalkingpos.X = follow.Position.X + orbwalkingAdditionInteger;
                    _orbwalkingpos.Y = follow.Position.Y + orbwalkingAdditionInteger;
                }
                if (_orbwalkingpos != null)
                {
                    Bot.IssueOrder(GameObjectOrder.MoveTo, _orbwalkingpos.To3D());
                    _stepTime = Environment.TickCount;
                }
            }

        } //end of WalkAround(Obj_AI_Hero)
    }
}
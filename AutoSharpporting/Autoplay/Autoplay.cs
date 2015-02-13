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
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Net.Mime;
using Support.Util;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Support
{
    internal class Autoplay
    {
        public const int Blue = 200;
        public const int Purple = -200;
        public static List<string> Manaless = new List<string> { "DrMundo", "Aatrox", "Mordekaiser",
                                                                "Vladimir", "Zac", "Akali", "Kennen", "LeeSin", 
                                                                "Shen", "Zed", "Garen", "Gnar", "Katarina", 
                                                                "RekSai", "Renekton", "Rengar", "Riven", "Rumble", 
                                                                "Shyvana", "Tryndamere", "Yasuo" };
        public static Obj_AI_Hero Bot = ObjectManager.Player;
        public static Obj_AI_Hero Carry;
        public static Obj_AI_Hero NearestAllyHero;
        public static Obj_AI_Turret NearestAllyTurret;
        public static Obj_AI_Hero Jungler;
        public static readonly Random Rand = new Random((42 / 13 * DateTime.Now.Millisecond) + DateTime.Now.Second + Environment.TickCount);
        private static Obj_AI_Hero _tempcarry;
        public static Vector2 BotLanePos;
        public static Vector2 TopLanePos;
        private static int _chosen;
        private static int _safe;
        private static Vector2 _frontline;
        private static Vector2 _safepos;
        private static Vector2 _saferecall;
        private static Vector2 _orbwalkingpos;
        private static int _loaded;
        private static bool _byPassLoadedCheck = false;
        private static int _randSeconds, _randRange, _stepTime;
        private static float _lowHealthRatio = 0.3f;
        private static float _lowManaRatio = 0.1f;
        private static float _lowHealthIfLowManaRatio = 0.6f;
        private static int _neededGoldToBack = 2200 + Rand.Next(0, 1100);
        private static bool _overrideAttackUnitAction = false;
        private static int _lastSwitched = 0;
        private static bool _tookRecallDecision = false;
        private static int _lastTimeTookRecallDecision = 0;
        private static int _lastRecallAttempt = 0;

        public Autoplay()
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
            Game.OnGameUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        public static bool RandomDecision()
        {
            return Rand.Next(0, 20) > 10; //Hi there riot games ^^
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && sender.UnderTurret(true) && args.Target.IsEnemy && args.Target.Type == GameObjectType.obj_AI_Hero)
            {
                _overrideAttackUnitAction = true;
            }
        }

        private static void OnGameLoad(EventArgs args)
        {
            _loaded = (Bot.Level == 1) ? Environment.TickCount:Environment.TickCount - 140000;
            _stepTime = Environment.TickCount;
            var map = Utility.Map.GetMap();
            if (map != null && map.Type == Utility.Map.MapType.SummonersRift)
            {
                if (Bot.Team == GameObjectTeam.Order)
                {
                    _chosen = Blue + Rand.Next(-76, 76);
                    _safe = Purple + Rand.Next(-67, 67);
                    BotLanePos.X = 11376 + Rand.Next(-50, 50);
                    BotLanePos.Y = 1062 + Rand.Next(-50, 50);
                    TopLanePos.X = 1302 + Rand.Next(-50, 50);
                    TopLanePos.Y = 10249 + Rand.Next(-50, 50);
                }
                if (Bot.Team == GameObjectTeam.Chaos)
                {
                    _chosen = Purple + Rand.Next(-67, 67);
                    _safe = Blue + Rand.Next(-76, 76);
                    BotLanePos.X = 13496 + Rand.Next(-50, 50);
                    BotLanePos.Y = 4218 + Rand.Next(-50, 50);
                    TopLanePos.X = 4849 + Rand.Next(-50, 50);
                    TopLanePos.Y = 13535 + Rand.Next(-50, 50);
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
            AutoLevel levelUpSpells = new AutoLevel(TreesAutoLevel.GetSequence());
            AutoLevel.Enabled(true);
            FileHandler.DoChecks();
            MetaHandler.LoadObjects();
            
        }

        private static void OnUpdate(EventArgs args)
        {
            DoAutoplay();
            MetaHandler.DoChecks();
            MetaHandler.UpdateObjects();
            if (Bot.InFountain() || !Carry.IsDead) _tookRecallDecision = false;
            if (Carry != null && Carry.IsDead && RandomDecision() &&
                Environment.TickCount - _lastTimeTookRecallDecision > Rand.Next(60000, 300000))
            {
                _tookRecallDecision = true;
                _lastTimeTookRecallDecision = Environment.TickCount;
            }
        }

        private static bool IsBotSafe()
        {
            var map = Utility.Map.GetMap();
            if (map != null && map.Type == Utility.Map.MapType.HowlingAbyss)
            {
                return true;
            }
            if (_tookRecallDecision)
            {
                return false;
            }
            if (Bot.InFountain())
            {
                if ((Bot.Health > Bot.MaxHealth * 0.9f) && (Bot.Mana > Bot.MaxMana * 0.8f) || (Bot.Health > Bot.MaxHealth * 0.9f) && Manaless.Any(s => s.Equals(ObjectManager.Player.ChampionName, StringComparison.OrdinalIgnoreCase)))
                {  
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (Bot.Mana < Bot.MaxMana * _lowManaRatio)
            {
                return Bot.Health > Bot.MaxHealth * _lowHealthIfLowManaRatio && !Bot.IsRecalling() && !(Bot.Gold > _neededGoldToBack && !MetaHandler.HasSixItems());
            }
            return (Bot.Health > Bot.MaxHealth * _lowHealthRatio) && !Bot.IsRecalling() && !(Bot.Gold > _neededGoldToBack && !MetaHandler.HasSixItems());

        }

        public static void DoAutoplay()
        {
            if (Bot.InFountain() && RandomDecision())
            {
                WalkAround(Bot.Position);
            }
            var timeElapsed = Environment.TickCount - _loaded;
            if (!Bot.IsDead)
            {
                try
                {
                    var turret = MetaHandler.EnemyTurrets.FirstOrDefault(t => t.Distance(Bot.ServerPosition) < 1200);
                    if (_overrideAttackUnitAction && !_tookRecallDecision)
                    {
                        Bot.IssueOrder(GameObjectOrder.MoveTo, _safepos.To3D());
                    }
                    if (!Bot.UnderTurret(true))
                    {
                        _overrideAttackUnitAction = false;
                    }
                    if (Bot.UnderTurret(true) && MetaHandler.NearbyAllyMinions(turret, 750) > 2 && IsBotSafe() && !_tookRecallDecision)
                    {
                            if (turret.Distance(Bot.ServerPosition) < Bot.AttackRange && !_overrideAttackUnitAction)
                                Bot.IssueOrder(GameObjectOrder.AttackUnit, turret);
                    }
                    else
                    {
                        if (TargetSelector.GetTarget(Bot.AttackRange, TargetSelector.DamageType.Physical) != null)
                        {
                            Obj_AI_Hero target = TargetSelector.GetTarget(Bot.AttackRange, TargetSelector.DamageType.Physical);
                            if (target != null && target.IsValid && !target.IsDead && IsBotSafe() &&
                                !target.UnderTurret(true) && !_overrideAttackUnitAction && !_tookRecallDecision)
                            {
                                Bot.IssueOrder(GameObjectOrder.AttackUnit, target);
                            }
                        }
                    }
                    if (Bot.UnderTurret(true) && MetaHandler.NearbyAllyMinions(turret, 750) < 2)
                    {
                        _safepos.X = (Bot.Position.X + _safe);
                        _safepos.Y = (Bot.Position.Y + _safe);
                        Bot.IssueOrder(GameObjectOrder.MoveTo, _safepos.To3D());
                    }
                    #region Carry is null
                    if (Carry == null && timeElapsed > 15000 && timeElapsed < 135000 && !_byPassLoadedCheck)
                    {
                        if (Bot.InFountain() || Bot.Distance(BotLanePos) > 400)
                        {
                            Bot.IssueOrder(GameObjectOrder.MoveTo, BotLanePos.To3D());
                        }
                        if (Bot.Distance(BotLanePos) < 1000)
                        {
                            WalkAround(BotLanePos.To3D());
                            if (timeElapsed > 60000 && !MetaHandler.ShouldSupportTopLane)
                            {
                                if (
                                    MetaHandler.AllyHeroes.FirstOrDefault(
                                        hero => !hero.IsMe && hero.Distance(Bot.ServerPosition) < 4500 && !MetaHandler.HasSmite(hero)) !=
                                    null)
                                {
                                    Carry =
                                        MetaHandler.AllyHeroes.FirstOrDefault(
                                            hero =>
                                                !hero.IsMe && hero.Distance(Bot.ServerPosition) < 4500 && !MetaHandler.HasSmite(hero));
                                }
                            }
                            if (timeElapsed > 60000 && MetaHandler.ShouldSupportTopLane)
                            {
                                if (
                                    MetaHandler.AllyHeroes.FirstOrDefault(
                                        hero => !hero.IsMe && hero.Distance(TopLanePos) < 4500 && !MetaHandler.HasSmite(hero)) !=
                                    null)
                                {
                                    Carry =
                                        MetaHandler.AllyHeroes.FirstOrDefault(
                                            hero =>
                                                !hero.IsMe && hero.Distance(TopLanePos) < 4500 && !MetaHandler.HasSmite(hero));
                                }
                            }
                        }
                    }
                    if (_byPassLoadedCheck && Carry == null)
                    {
                        if (MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && !hero.InFountain() && !MetaHandler.HasSmite(hero)) != null)
                        {
                            Carry = MetaHandler.AllyHeroes.FirstOrDefault(hero => !hero.IsMe && !hero.InFountain() && !MetaHandler.HasSmite(hero));
                        }
                    }
                    #endregion
                    #region Carry is dead
                    if (Carry != null)
                    {
                        if (IsBotSafe() && Carry.IsDead || Carry.InFountain())
                        {
                            if (_tempcarry == null || _tempcarry.IsDead || _tempcarry.InFountain())
                            {
                                if (
                                    MetaHandler.AllyHeroes.FirstOrDefault(
                                        hero =>
                                            !hero.IsMe && !hero.InFountain() && !hero.IsDead &&
                                            !MetaHandler.HasSmite(hero)) != null)
                                {
                                    _tempcarry =
                                        MetaHandler.AllyHeroes.FirstOrDefault(
                                            hero =>
                                                !hero.IsMe && !hero.InFountain() && !hero.IsDead &&
                                                !MetaHandler.HasSmite(hero));
                                }
                                if (
                                    MetaHandler.AllyHeroes.FirstOrDefault(
                                        hero =>
                                            !hero.IsMe && !hero.InFountain() && !hero.IsDead &&
                                            !MetaHandler.HasSmite(hero)) == null &&
                                    MetaHandler.AllyHeroes.FirstOrDefault(
                                        hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead) != null)
                                {
                                    //well fuck, let's follow the jungler -sighs-
                                    _tempcarry =
                                        MetaHandler.AllyHeroes.FirstOrDefault(
                                            hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead);
                                }
                                if (!MetaHandler.AllyHeroes.Any(hero => !hero.IsMe && !hero.IsDead))
                                    //everyone is dead
                                {
                                    if (!Bot.InFountain())
                                    {
                                        NearestAllyTurret = MetaHandler.AllyTurrets.FirstOrDefault();
                                        if (NearestAllyTurret != null)
                                        {
                                            _saferecall.X = NearestAllyTurret.Position.X + _safe;
                                            _saferecall.Y = NearestAllyTurret.Position.Y;
                                            _tookRecallDecision = true;
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
                                }
                            }
                            if (_tempcarry != null)
                            {

                                Console.WriteLine("Carry dead or afk, following: " + _tempcarry.ChampionName);
                                _frontline.X = _tempcarry.Position.X + _chosen;
                                _frontline.Y = _tempcarry.Position.Y + _chosen;
                                if (!(_tempcarry.UnderTurret(true) && MetaHandler.NearbyAllyMinions(_tempcarry, 400) < 2) && IsBotSafe())
                                {
                                    if (_tempcarry.Distance(Bot.ServerPosition) > 550 && !_tookRecallDecision)
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
                    if (Carry != null && !Carry.IsDead && !Carry.InFountain() && IsBotSafe() && !(Carry.UnderTurret(true) && MetaHandler.NearbyAllyMinions(Carry, 400) < 2))
                    {
                        Console.WriteLine("All good, following: " + Carry.ChampionName);
                        _frontline.X = Carry.Position.X + _chosen;
                        _frontline.Y = Carry.Position.Y + _chosen;
                        if (Carry.Distance(Bot.ServerPosition) > 550 && !_tookRecallDecision)
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
                        if (_tempcarry == null || _tempcarry.IsDead || _tempcarry.InFountain())
                        {
                            if (
                                MetaHandler.AllyHeroes.FirstOrDefault(
                                    hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead && !MetaHandler.HasSmite(hero)) != null)
                            {
                                _tempcarry =
                                    MetaHandler.AllyHeroes.FirstOrDefault(
                                        hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead && !MetaHandler.HasSmite(hero));
                            }
                            if (
                                MetaHandler.AllyHeroes.FirstOrDefault(
                                    hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead && !MetaHandler.HasSmite(hero)) == null &&
                                MetaHandler.AllyHeroes.FirstOrDefault(
                                    hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead) != null)
                            {
                                //well fuck, let's follow the jungler -sighs-
                                _tempcarry =
                                    MetaHandler.AllyHeroes.FirstOrDefault(
                                        hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead);
                            }
                        }
                        if (_tempcarry != null)
                        {
                            Console.WriteLine("Carry not found, following: " + _tempcarry.ChampionName);
                            _frontline.X = _tempcarry.Position.X + _chosen;
                            _frontline.Y = _tempcarry.Position.Y + _chosen;
                            if (!(_tempcarry.UnderTurret(true) && MetaHandler.NearbyAllyMinions(_tempcarry, 400) < 2) && IsBotSafe())
                            {
                                if (Bot.Distance(_frontline) > 550 && !_tookRecallDecision)
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
                        if (NearestAllyTurret != null)
                        {
                            _saferecall.X = NearestAllyTurret.Position.X + _safe;
                            _saferecall.Y = NearestAllyTurret.Position.Y;
                            if (Bot.Position.Distance(_saferecall.To3D()) < 200)
                            {
                                if (Environment.TickCount - _lastRecallAttempt > Rand.Next(500, 2000))
                                {
                                    Bot.Spellbook.CastSpell(SpellSlot.Recall);
                                    _lastRecallAttempt = Environment.TickCount;
                                }
                            }
                            else
                            {

                                Bot.IssueOrder(GameObjectOrder.MoveTo, _saferecall.To3D());
                            }
                        }

                    }
                    #endregion
                    #region Carry Switching

                    if ((Bot.Level > 8 || Environment.TickCount - _loaded > 900000) && Environment.TickCount - _lastSwitched > 180000)
                    {
                        var alliesSortedByKDA =
                            MetaHandler.AllyHeroes.OrderByDescending(hero => (hero.ChampionsKilled / ((hero.Deaths != 0) ? hero.Deaths : 1))); //AsunaChan2Kawaii
                        if (alliesSortedByKDA.FirstOrDefault() != null)
                        {
                            Carry = alliesSortedByKDA.FirstOrDefault();
                            _lastSwitched = Environment.TickCount;
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
            if (Environment.TickCount - _stepTime >= _randSeconds && !_overrideAttackUnitAction)
            {
                if (Bot.Team == GameObjectTeam.Order)
                {
                    int orbwalkingAdditionInteger = _randRange * (-1);
                    _orbwalkingpos.X = pos.X + orbwalkingAdditionInteger;
                    _orbwalkingpos.Y = pos.Y + orbwalkingAdditionInteger;
                }
                else
                {
                    int orbwalkingAdditionInteger = _randRange;
                    _orbwalkingpos.X = pos.X + orbwalkingAdditionInteger;
                    _orbwalkingpos.Y = pos.Y + orbwalkingAdditionInteger;
                }
                if (_orbwalkingpos != null && !_tookRecallDecision)
                {
                    Bot.IssueOrder(GameObjectOrder.MoveTo, _orbwalkingpos.To3D());
                    _stepTime = Environment.TickCount;
                }
            }

        } //end of WalkAround()

        private static void WalkAround(Obj_AI_Hero follow)
        {
            _randRange = Rand.Next(-367, 376);
            _randSeconds = Rand.Next(500, 3500);
            if (Environment.TickCount - _stepTime >= _randSeconds && !_overrideAttackUnitAction)
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
                if (_orbwalkingpos != null && Bot.Distance(follow.ServerPosition) < 550 && !_tookRecallDecision)
                {
                    Bot.IssueOrder(GameObjectOrder.MoveTo, _orbwalkingpos.To3D());
                    _stepTime = Environment.TickCount;
                }
            }

        } //end of WalkAround(Obj_AI_Hero)
    }
}
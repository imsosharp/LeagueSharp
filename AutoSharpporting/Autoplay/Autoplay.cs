/* Autoplay Plugin of h3h3's AIO Support
*
* All credits go to him. I only wrote this and
* MetaHandler.cs.
* The core is always updated to latest version.
* which you can find here:
* https://github.com/h3h3/LeagueSharp/tree/master/Support
*/

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

/// This code is so shit but
/// I'll try to show it some love :)

namespace AutoSharpporting.Autoplay
{
    internal class Autoplay
    {
        public const int Blue = 200;
        public const int Purple = -200;
        public static Obj_AI_Hero Bot = ObjectManager.Player;
        public static Obj_AI_Hero Carry { get; set; }
        public static Obj_AI_Hero NearestAllyHero { get; set; }
        public static Obj_AI_Turret NearestAllyTurret { get; set; }
        public static Obj_AI_Hero Jungler { get; set; }
        public static Obj_AI_Hero TempCarry { get; set; }
        public static Vector2 BotLanePos;
        public static Vector2 TopLanePos;
        public static int _chosen;
        public static int _safe;
        public static Vector2 _frontline;
        public static Vector2 _safepos;
        public static Vector2 _saferecall;
        public static int _loaded;
        public static bool _byPassLoadedCheck;
        public static float _lowHealthRatio = 0.3f;
        public static float _lowManaRatio = 0.1f;
        public static float _lowHealthIfLowManaRatio = 0.6f;
        public static bool _overrideAttackUnitAction;
        public static int _lastSwitched = 0;
        public static bool _tookRecallDecision;
        public static int _lastTimeTookRecallDecision;
        public static int _lastRecallAttempt = 0;

        public Autoplay()
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        public static bool RandomDecision()
        {
            return Rand.Next(0, 20) > 10; //Hi there riot games ^^
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && sender.UnderTurret(true) && args.Target.IsEnemy &&
                args.Target.Type == GameObjectType.obj_AI_Hero)
            {
                _overrideAttackUnitAction = true;
            }
        }

        public static void OnGameLoad(EventArgs args)
        {
            _loaded = (Bot.Level == 1) ? Environment.TickCount : Environment.TickCount - 140000;
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
            new AutoLevel(TreesAutoLevel.GetSequence().Select(l => l - 1));
            AutoLevel.Enable();
            MetaHandler.LoadObjects();
            MetaHandler.DoChecks();
        }

        public static void OnUpdate(EventArgs args)
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

        public static bool IsBotSafe()
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
                return (Bot.Health > Bot.MaxHealth*0.9f) && (Bot.Mana > Bot.MaxMana*0.8f);
            }
            if (Bot.Mana < Bot.MaxMana*_lowManaRatio)
            {
                return Bot.Health > Bot.MaxHealth*_lowHealthIfLowManaRatio && !Bot.IsRecalling() &&
                       !(Bot.Gold > _neededGoldToBack && !MetaHandler.HasSixItems());
            }
            return (Bot.Health > Bot.MaxHealth*_lowHealthRatio) && !Bot.IsRecalling() &&
                   !(Bot.Gold > _neededGoldToBack && !MetaHandler.HasSixItems());
        }

        public static void DoAutoplay()
        {
            if (Bot.InFountain() && RandomDecision())
            {
                Bot.Position.WalkAround();
            }

            if (!IsBotSafe() && !Bot.InFountain())
                Behavior.LowHealth();
            if (Carry == null && Environment.TickCount - _loaded > 15000 && Environment.TickCount - _loaded < 135000 &&
                !_byPassLoadedCheck)
                Behavior.CarryIsNull();
            if (Carry != null && IsBotSafe() && Carry.IsDead || Carry.InFountain())
                Behavior.CarryIsDead();
            if (Carry != null && !Carry.IsDead && !Carry.InFountain() && IsBotSafe() &&
                !(Carry.UnderTurret(true) && MetaHandler.NearbyAllyMinions(Carry, 400) < 2))
                Behavior.Follow();
            if (Environment.TickCount - _loaded > 135000 &&
                Carry == null && IsBotSafe())
                Behavior.NoCarryFound();
            if ((Bot.Level > 8 || Environment.TickCount - _loaded > 900000) &&
                Environment.TickCount - _lastSwitched > 180000)
                Behavior.SwitchCarry();


            if (!Bot.IsDead)
            {
                try
                {
                    var turret = MetaHandler.EnemyTurrets.FirstOrDefault(t => t.Distance(Bot.Position) < 1200);
                    if (_overrideAttackUnitAction && !_tookRecallDecision)
                    {
                        Bot.IssueOrder(GameObjectOrder.MoveTo, _safepos.To3D());
                    }
                    if (!Bot.UnderTurret(true))
                    {
                        _overrideAttackUnitAction = false;
                    }
                    if (Bot.UnderTurret(true) && MetaHandler.NearbyAllyMinions(turret, 750) > 2 && IsBotSafe() &&
                        !_tookRecallDecision)
                    {
                        if (turret.Distance(Bot.Position) < Bot.AttackRange && !_overrideAttackUnitAction)
                            Bot.IssueOrder(GameObjectOrder.AttackUnit, turret);
                    }
                    else
                    {
                        if (TargetSelector.GetTarget(Bot.AttackRange, TargetSelector.DamageType.Physical) != null)
                        {
                            var target = TargetSelector.GetTarget(Bot.AttackRange, TargetSelector.DamageType.Physical);
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
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public static readonly Random Rand =
            new Random((42/13*DateTime.Now.Millisecond) + DateTime.Now.Second + Environment.TickCount);

        public static int _neededGoldToBack = 2200 + Rand.Next(0, 1100);
    }
}
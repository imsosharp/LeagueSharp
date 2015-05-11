using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AutoSharpporting.Autoplay
{
    internal static class Wizard
    {
        public const int Blue = 200;
        public const int Purple = -200;
        public static Obj_AI_Hero Bot = ObjectManager.Player;

        public static readonly Random Rand =
            new Random((42/13*DateTime.Now.Millisecond) + DateTime.Now.Second + Environment.TickCount);

        private static int _randSeconds, _randRange, _stepTime;
        private static Vector2 _orbwalkingpos;

        public static void WalkAround(this Vector3 pos)
        {
            _randRange = Rand.Next(-267, 276);
            _randSeconds = Rand.Next(1000, 4000);
            if (Environment.TickCount - _stepTime >= _randSeconds && !Autoplay._overrideAttackUnitAction)
            {
                if (Bot.Team == GameObjectTeam.Order)
                {
                    var orbwalkingAdditionInteger = _randRange*(-1);
                    _orbwalkingpos.X = pos.X + orbwalkingAdditionInteger;
                    _orbwalkingpos.Y = pos.Y + orbwalkingAdditionInteger;
                }
                else
                {
                    var orbwalkingAdditionInteger = _randRange;
                    _orbwalkingpos.X = pos.X + orbwalkingAdditionInteger;
                    _orbwalkingpos.Y = pos.Y + orbwalkingAdditionInteger;
                }
                if (_orbwalkingpos != null && !Autoplay._tookRecallDecision)
                {
                    PluginBase.Orbwalker.SetOrbwalkingPoint(_orbwalkingpos.To3D());
                    PluginBase.Orbwalker.ActiveMode = Orbwalking.OrbwalkingMode.Combo;
                }
                if (Autoplay._tookRecallDecision)
                {
                    PluginBase.Orbwalker.ActiveMode = Orbwalking.OrbwalkingMode.None;
                }
            }
        }

        public static void WalkAround(this Obj_AI_Hero follow)
        {
            _randRange = Rand.Next(-367, 376);
            _randSeconds = Rand.Next(500, 3500);
            if (Environment.TickCount - _stepTime >= _randSeconds && !Autoplay._overrideAttackUnitAction)
            {
                if (Bot.Team == GameObjectTeam.Order)
                {
                    var orbwalkingAdditionInteger = _randRange*(-1);
                    _orbwalkingpos.X = follow.Position.X + orbwalkingAdditionInteger;
                    _orbwalkingpos.Y = follow.Position.Y + orbwalkingAdditionInteger;
                }
                else
                {
                    var orbwalkingAdditionInteger = _randRange;
                    _orbwalkingpos.X = follow.Position.X + orbwalkingAdditionInteger;
                    _orbwalkingpos.Y = follow.Position.Y + orbwalkingAdditionInteger;
                }
                if (_orbwalkingpos != null && Bot.Distance(follow) < 550 && !Autoplay._tookRecallDecision)
                {
                    PluginBase.Orbwalker.SetOrbwalkingPoint(_orbwalkingpos.To3D());
                    PluginBase.Orbwalker.ActiveMode = Orbwalking.OrbwalkingMode.Combo;
                }
                if (Autoplay._tookRecallDecision)
                {
                    PluginBase.Orbwalker.ActiveMode = Orbwalking.OrbwalkingMode.None;
                }
            }
        }
    }
}
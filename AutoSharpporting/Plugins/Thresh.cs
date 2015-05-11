#region LICENSE

// Copyright 2014-2015 Support
// Thresh.cs is part of Support.
// 
// Support is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Support is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Support. If not, see <http://www.gnu.org/licenses/>.
// 
// Filename: Support/Support/Thresh.cs
// Created:  06/10/2014
// Date:     20/01/2015/11:20
// Author:   h3h3

#endregion

namespace AutoSharpporting.Plugins
{
    #region

    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using AutoSharpporting.Util;
    using ActiveGapcloser = AutoSharpporting.Util.ActiveGapcloser;

    #endregion

    public class Thresh : PluginBase
    {
        private const int QFollowTime = 3000;
        private Obj_AI_Hero _qTarget;
        private int _qTick;

        public Thresh()
        {
            Q = new Spell(SpellSlot.Q, 1025);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, 400);

            Q.SetSkillshot(0.5f, 70f, 1900, true, SkillshotType.SkillshotLine);
        }

        private bool FollowQ
        {
            get { return Environment.TickCount <= _qTick + QFollowTime; }
        }

        private bool FollowQBlock
        {
            get { return Environment.TickCount - _qTick >= QFollowTime; }
        }

        public override void OnUpdate(EventArgs args)
        {
            try
            {
                if (_qTarget != null)
                {
                    if (Environment.TickCount - _qTick >= QFollowTime)
                    {
                        _qTarget = null;
                    }
                }

                if (ComboMode)
                {
                    if (Q.CastCheck(Target, "ComboQ") && FollowQBlock)
                    {
                        if (Q.Cast(Target) == Spell.CastStates.SuccessfullyCasted)
                        {
                            _qTick = Environment.TickCount;
                            _qTarget = Target;
                        }
                    }
                    if (Q.CastCheck(_qTarget, "ComboQFollow"))
                    {
                        if (FollowQ)
                        {
                            Q.Cast();
                        }
                    }

                    if (W.CastCheck(Target, "ComboW"))
                    {
                        EngageFriendLatern();
                    }

                    if (E.CastCheck(Target, "ComboE"))
                    {
                        if (Helpers.AllyBelowHp(ConfigValue<Slider>("ComboHealthE").Value, E.Range) != null)
                        {
                            E.Cast(Target.Position);
                        }
                        else
                        {
                            E.Cast(Helpers.ReversePosition(ObjectManager.Player.Position, Target.Position));
                        }
                    }

                    if (R.CastCheck(Target, "ComboR"))
                    {
                        if (Helpers.EnemyInRange(ConfigValue<Slider>("ComboCountR").Value, R.Range))
                        {
                            R.Cast();
                        }
                    }
                }

                if (HarassMode)
                {
                    if (Q.CastCheck(Target, "HarassQ") && FollowQBlock)
                    {
                        Q.Cast(Target);
                    }

                    if (W.CastCheck(Target, "HarassW"))
                    {
                        SafeFriendLatern();
                    }

                    if (E.CastCheck(Target, "HarassE"))
                    {
                        if (Helpers.AllyBelowHp(ConfigValue<Slider>("HarassHealthE").Value, E.Range) != null)
                        {
                            E.Cast(Target.Position);
                        }
                        else
                        {
                            E.Cast(Helpers.ReversePosition(ObjectManager.Player.Position, Target.Position));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
            {
                return;
            }

            if (E.CastCheck(gapcloser.Sender, "GapcloserE"))
            {
                E.Cast(gapcloser.Start);
            }
        }

        public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (E.CastCheck(unit, "InterruptE"))
            {
                E.Cast(unit.Position);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboQFollow", "Use Q Follow", true);
            config.AddBool("ComboW", "Use W for Engage", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddSlider("ComboHealthE", "Push Targets away if low HP", 20, 1, 100);
            config.AddBool("ComboR", "Use R", true);
            config.AddSlider("ComboCountR", "Targets in range to Ult", 2, 1, 5);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassQ", "Use Q", true);
            config.AddBool("HarassW", "Use W for Safe", true);
            config.AddBool("HarassE", "Use E", true);
            config.AddSlider("HarassHealthE", "Push Targets away if low HP", 20, 1, 100);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("GapcloserE", "Use E to Interrupt Gapcloser", true);

            config.AddBool("InterruptE", "Use E to Interrupt Spells", true);
        }

        /// <summary>
        ///     Credit
        ///     https://github.com/LXMedia1/UltimateCarry2/blob/master/LexxersAIOCarry/Thresh.cs
        /// </summary>
        private void EngageFriendLatern()
        {
            if (!W.IsReady())
            {
                return;
            }

            var bestcastposition = new Vector3(0f, 0f, 0f);

            foreach (var friend in
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        hero =>
                            hero.IsAlly && !hero.IsMe && hero.Distance(Player) <= W.Range + 300 &&
                            hero.Distance(Player) <= W.Range - 300 && hero.Health / hero.MaxHealth * 100 >= 20 &&
                            Player.CountEnemiesInRange(150) >= 1))
            {
                var center = Player.Position;
                const int points = 36;
                var radius = W.Range;
                const double slice = 2 * Math.PI / points;

                for (var i = 0; i < points; i++)
                {
                    var angle = slice * i;
                    var newX = (int) (center.X + radius * Math.Cos(angle));
                    var newY = (int) (center.Y + radius * Math.Sin(angle));
                    var p = new Vector3(newX, newY, 0);
                    if (p.Distance(friend.Position) <= bestcastposition.Distance(friend.Position))
                    {
                        bestcastposition = p;
                    }
                }

                if (friend.Distance(ObjectManager.Player) <= W.Range)
                {
                    W.Cast(bestcastposition, true);
                    return;
                }
            }

            if (bestcastposition.Distance(new Vector3(0f, 0f, 0f)) >= 100)
            {
                W.Cast(bestcastposition, true);
            }
        }

        /// <summary>
        ///     Credit
        ///     https://github.com/LXMedia1/UltimateCarry2/blob/master/LexxersAIOCarry/Thresh.cs
        /// </summary>
        private void SafeFriendLatern()
        {
            if (!W.IsReady())
            {
                return;
            }

            var bestcastposition = new Vector3(0f, 0f, 0f);

            foreach (var friend in
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        hero =>
                            hero.IsAlly && !hero.IsMe && hero.Distance(ObjectManager.Player) <= W.Range + 300 &&
                            hero.Distance(ObjectManager.Player) <= W.Range - 200 &&
                            hero.Health / hero.MaxHealth * 100 >= 20 && !hero.IsDead))
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsEnemy))
                {
                    if (friend == null || !(friend.Distance(enemy) <= 300))
                    {
                        continue;
                    }

                    var center = ObjectManager.Player.Position;
                    const int points = 36;
                    var radius = W.Range;
                    const double slice = 2 * Math.PI / points;

                    for (var i = 0; i < points; i++)
                    {
                        var angle = slice * i;
                        var newX = (int) (center.X + radius * Math.Cos(angle));
                        var newY = (int) (center.Y + radius * Math.Sin(angle));
                        var p = new Vector3(newX, newY, 0);
                        if (p.Distance(friend.Position) <= bestcastposition.Distance(friend.Position))
                        {
                            bestcastposition = p;
                        }
                    }

                    if (friend.Distance(ObjectManager.Player) <= W.Range)
                    {
                        W.Cast(bestcastposition, true);
                        return;
                    }
                }

                if (bestcastposition.Distance(new Vector3(0f, 0f, 0f)) >= 100)
                {
                    W.Cast(bestcastposition, true);
                }
            }
        }
    }
}
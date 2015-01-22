#region LICENSE

// Copyright 2014-2015 Support
// Morgana.cs is part of Support.
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
// Filename: Support/Support/Morgana.cs
// Created:  01/10/2014
// Date:     20/01/2015/11:20
// Author:   h3h3

#endregion

namespace Support.Plugins
{
    #region

    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Support.Util;
    using ActiveGapcloser = Support.Util.ActiveGapcloser;

    #endregion

    public class Morgana : PluginBase
    {
        public Morgana()
        {
            Q = new Spell(SpellSlot.Q, 1175);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 750);
            R = new Spell(SpellSlot.R, 550);

            Q.SetSkillshot(0.25f, 80f, 1200f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.28f, 175f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            try
            {
                if (ComboMode)
                {
                    if (Q.CastCheck(Target, "ComboQ"))
                    {
                        Q.Cast(Target);
                    }

                    if (W.CastCheck(Target, "ComboW"))
                    {
                        foreach (var enemy in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(
                                    hero =>
                                        (hero.HasBuffOfType(BuffType.Snare) || hero.HasBuffOfType(BuffType.Stun) ||
                                         hero.HasBuffOfType(BuffType.Taunt) && hero.IsValidTarget(W.Range))))
                        {
                            W.Cast(enemy.Position);
                            return;
                        }

                        foreach (var enemy in
                            ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(W.Range)))
                        {
                            W.CastIfWillHit(enemy, 1);
                            return;
                        }
                    }

                    if (R.CastCheck(Target, "ComboR") &&
                        Helpers.EnemyInRange(ConfigValue<Slider>("ComboCountR").Value, R.Range))
                    {
                        R.Cast();
                    }
                }

                if (HarassMode)
                {
                    if (Q.CastCheck(Target, "HarassQ"))
                    {
                        Q.Cast(Target);
                    }

                    if (W.CastCheck(Target, "HarassW"))
                    {
                        foreach (var enemy in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(
                                    hero =>
                                        (hero.HasBuffOfType(BuffType.Snare) || hero.HasBuffOfType(BuffType.Stun) ||
                                         hero.HasBuffOfType(BuffType.Taunt) && hero.IsValidTarget(W.Range))))
                        {
                            W.Cast(enemy.Position);
                            return;
                        }

                        foreach (var enemy in
                            ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(W.Range)))
                        {
                            W.CastIfWillHit(enemy, 1);
                            return;
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

            if (Q.CastCheck(gapcloser.Sender, "GapcloserQ"))
            {
                Q.Cast(gapcloser.Sender);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
            config.AddSlider("ComboCountR", "Targets in range to Ult", 2, 1, 5);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassQ", "Use Q", true);
            config.AddBool("HarassW", "Use W", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("GapcloserQ", "Use Q to Interrupt Gapcloser", true);
        }
    }
}
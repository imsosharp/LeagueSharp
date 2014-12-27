#region LICENSE

// Copyright 2014 Support
// Alistar.cs is part of Support.
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
// Filename: Support/Support/Alistar.cs
// Created:  01/10/2014
// Date:     26/12/2014/16:23
// Author:   h3h3

#endregion

namespace Support.Plugins
{
    #region

    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Support.Util;
    using ActiveGapcloser = Support.Util.ActiveGapcloser;

    #endregion

    public class Alistar : PluginBase
    {
        public Alistar()
        {
            Q = new Spell(SpellSlot.Q, 365);
            W = new Spell(SpellSlot.W, 650);
            E = new Spell(SpellSlot.E, 575);
            R = new Spell(SpellSlot.R, 0);

            W.SetTargetted(0.5f, float.MaxValue);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast();
                }

                if (Q.IsReady() && W.CastCheck(Target, "ComboW"))
                {
                    W.CastOnUnit(Target, UsePackets);
                    var jumpTime = Math.Max(0, Player.Distance(Target) - 500) * 10 / 25 + 25;
                    Utility.DelayAction.Add((int) jumpTime, () => Q.Cast());
                }

                var ally = Helpers.AllyBelowHp(ConfigValue<Slider>("ComboHealthE").Value, E.Range);
                if (E.CastCheck(ally, "ComboE", true, false))
                {
                    E.Cast();
                }
            }

            if (HarassMode)
            {
                if (Q.CastCheck(Target, "HarassQ"))
                {
                    Q.Cast();
                }

                var ally = Helpers.AllyBelowHp(ConfigValue<Slider>("HarassHealthR").Value, E.Range);
                if (E.CastCheck(ally, "HarassE", true, false))
                {
                    E.Cast();
                }
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
                Q.Cast();
            }

            if (W.CastCheck(gapcloser.Sender, "GapcloserW"))
            {
                W.CastOnUnit(gapcloser.Sender, UsePackets);
            }
        }

        public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (Q.CastCheck(unit, "InterruptQ"))
            {
                Q.Cast();
            }

            if (W.CastCheck(unit, "InterruptW"))
            {
                W.CastOnUnit(unit, UsePackets);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use WQ", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddSlider("ComboHealthR", "Health to Heal", 20, 1, 100);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassQ", "Use Q", true);
            config.AddBool("HarassE", "Use E", true);
            config.AddSlider("HarassHealthR", "Health to Heal", 20, 1, 100);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("GapcloserQ", "Use Q to Interrupt Gapcloser", true);
            config.AddBool("GapcloserW", "Use W to Interrupt Gapcloser", true);

            config.AddBool("InterruptQ", "Use Q to Interrupt Spells", true);
            config.AddBool("InterruptW", "Use W to Interrupt Spells", true);
        }
    }
}
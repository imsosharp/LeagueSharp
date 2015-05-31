#region LICENSE

// Copyright 2014-2015 Support
// Nunu.cs is part of Support.
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
// Filename: Support/Support/Nunu.cs
// Created:  16/11/2014
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

    public class Nunu : PluginBase
    {
        public Nunu()
        {
            Q = new Spell(SpellSlot.Q, 125);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 550);
            R = new Spell(SpellSlot.R, 650);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.IsReady() && ConfigValue<bool>("Combo.Q") &&
                    Player.HealthPercentage() < ConfigValue<Slider>("Combo.Q.Health").Value)
                {
                    var minion = MinionManager.GetMinions(Player.Position, Q.Range).FirstOrDefault();
                    if (minion.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(minion);
                    }
                }

                var allys = Helpers.AllyInRange(W.Range).OrderByDescending(h => h.FlatPhysicalDamageMod).ToList();
                if (W.IsReady() && allys.Count > 0 && ConfigValue<bool>("Combo.W"))
                {
                    W.CastOnUnit(allys.FirstOrDefault());
                }

                if (W.IsReady() && Target.IsValidTarget(AttackRange) && ConfigValue<bool>("Combo.W"))
                {
                    W.CastOnUnit(Player);
                }

                if (E.IsReady() && Target.IsValidTarget(E.Range) && ConfigValue<bool>("Combo.E"))
                {
                    E.CastOnUnit(Target);
                }
            }

            if (HarassMode)
            {
                if (Q.IsReady() && ConfigValue<bool>("Harass.Q") &&
                    Player.HealthPercentage() < ConfigValue<Slider>("Harass.Q.Health").Value)
                {
                    var minion = MinionManager.GetMinions(Player.Position, Q.Range).FirstOrDefault();
                    if (minion.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(minion);
                    }
                }

                var allys = Helpers.AllyInRange(W.Range).OrderByDescending(h => h.FlatPhysicalDamageMod).ToList();
                if (W.IsReady() && allys.Count > 0 && ConfigValue<bool>("Harass.W"))
                {
                    W.CastOnUnit(allys.FirstOrDefault());
                }

                if (W.IsReady() && Target.IsValidTarget(AttackRange) && ConfigValue<bool>("Harass.W"))
                {
                    W.CastOnUnit(Player);
                }

                if (E.IsReady() && Target.IsValidTarget(E.Range) && ConfigValue<bool>("Harass.E"))
                {
                    E.CastOnUnit(Target);
                }
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
            {
                return;
            }

            if (E.CastCheck(gapcloser.Sender, "Gapcloser.E"))
            {
                E.CastOnUnit(gapcloser.Sender);

                if (W.IsReady())
                {
                    W.CastOnUnit(Player);
                }
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("Combo.Q", "Use Q", true);
            config.AddBool("Combo.W", "Use W", true);
            config.AddBool("Combo.E", "Use E", true);
            config.AddSlider("Combo.Q.Health", "Consume below %HP", 50, 1, 100);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("Harass.Q", "Use Q", true);
            config.AddBool("Harass.W", "Use W", false);
            config.AddBool("Harass.E", "Use E", true);
            config.AddSlider("Harass.Q.Health", "Consume below %HP", 50, 1, 100);
        }

        public override void MiscMenu(Menu config)
        {
            config.AddList("Misc.Laugh", "Laugh Emote", new[] { "OFF", "ON", "ON + Mute" });
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Gapcloser.E", "Use E to Interrupt Gapcloser", true);
        }
    }
}
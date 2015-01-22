#region LICENSE

// Copyright 2014-2015 Support
// Zilean.cs is part of Support.
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
// Filename: Support/Support/Zilean.cs
// Created:  05/10/2014
// Date:     20/01/2015/11:20
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

    public class Zilean : PluginBase
    {
        public Zilean()
        {
            Q = new Spell(SpellSlot.Q, 700);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 900);
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

                    if (W.IsReady() && !Q.IsReady() && ConfigValue<bool>("ComboW"))
                    {
                        W.Cast();
                    }

                    // TODO: speed adc/jungler/engage
                    if (E.IsReady() && Player.CountEnemiesInRange(2000) > 0 && ConfigValue<bool>("ComboE"))
                    {
                        E.Cast(Player);
                    }
                }

                if (HarassMode)
                {
                    if (Q.CastCheck(Target, "HarassQ"))
                    {
                        Q.Cast(Target);
                    }

                    if (W.IsReady() && !Q.IsReady() && ConfigValue<bool>("HarassW"))
                    {
                        W.Cast();
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
                E.Cast(gapcloser.Sender);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassQ", "Use Q", true);
            config.AddBool("HarassW", "Use W", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("GapcloserE", "Use E to Interrupt Gapcloser", true);
        }
    }
}
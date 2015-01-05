#region LICENSE

// Copyright 2014 Support
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
// Date:     26/12/2014/16:23
// Author:   h3h3

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Support.Evade;
using Support.Util;
using ActiveGapcloser = Support.Util.ActiveGapcloser;
using SpellData = LeagueSharp.SpellData;

namespace Support.Plugins
{
    public class Tristana : PluginBase
    {
        public Tristana()
        {
            Q = new Spell(SpellSlot.Q, 703);
            E = new Spell(SpellSlot.E, 703);
            R = new Spell(SpellSlot.R, 703);
        }

        public override void OnUpdate(EventArgs args)
        {
                if (ComboMode)
                {
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast();
                }
                if (W.CastCheck(Target, "ComboW") && W.IsKillable(Target))
                {
                    W.Cast(Target, true);
                }
                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast(Target, true);
                }
                if (R.CastCheck(Target, "ComboR") && R.IsKillable(Target))
                {
                    R.Cast(Target, true);
                }
            }

            if (HarassMode)
            {
                if (E.CastCheck(Target, "HarassE"))
                {
                    E.Cast(Target, true);
                }
            }
        }

        public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (R.CastCheck(unit, "Interrupt.R"))
            {
                R.Cast(unit, UsePackets);
                return;
            }

        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassE", "Use E", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Interrupt.R", "Use R to Interrupt Spells", true);
        }


    }
}

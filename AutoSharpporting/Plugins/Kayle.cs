#region LICENSE

// Copyright 2014 - 2014 Support
// Kayle.cs is part of Support.
// Support is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// Support is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with Support. If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using LeagueSharp;
using LeagueSharp.Common;
using Support.Util;
using ActiveGapcloser = Support.Util.ActiveGapcloser;

#endregion

namespace Support.Plugins
{
    public class Kayle : PluginBase
    {
        public Kayle()
        {
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 525);
            R = new Spell(SpellSlot.R, 900);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(Target);
                }
                if (Player.Distance(Target) <= E.Range && Player.HealthPercentage() > 30 && E.CastCheck(Target, "ComboE"))
                {
                    E.Cast();
                    Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
                }
            }
        }

        public override void OnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
        }

        public override void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
        }


        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
            config.AddSlider("ComboCountR", "Targets in range to Ult", 2, 0, 5);
            config.AddSlider("ComboHealthR", "Health to Ult", 20, 1, 100);
        }



        public override void MiscMenu(Menu config)
        {

        }
    }
}
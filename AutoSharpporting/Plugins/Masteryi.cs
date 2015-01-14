#region LICENSE

// Copyright 2014 - 2014 Support
// Annie.cs is part of Support.
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
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Support.Evade;
using Support.Util;
using ActiveGapcloser = Support.Util.ActiveGapcloser;
using SpellData = LeagueSharp.SpellData;

#endregion

namespace Support.Plugins
{
    public class Masteryi : PluginBase
    {
        public Masteryi()
        {
            Q = new Spell(SpellSlot.Q, 600);
            Q.SetTargetted(0.5f, 2000);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);
        }


        // some part from Prunes

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.IsReady())
                {
                    Qlogic();
                }
                if (R.IsReady() && Player.CountEnemysInRange(Q.Range) >= 2)
                {

                    R.Cast();
                }
                
                if (E.IsReady() && Orbwalking.InAutoAttackRange(Target))
                {
                    E.Cast();
                }
                else if (W.IsReady() && Orbwalking.InAutoAttackRange(Target) && W.IsReady())
                {
                    Player.IssueOrder(GameObjectOrder.AttackTo, Target);
                    Utility.DelayAction.Add(400, () => W.Cast());
                    W.Cast();
                    Player.IssueOrder(GameObjectOrder.AttackTo, Target);
                    Orbwalking.ResetAutoAttackTimer();
                }
            }
        }

        public void Qlogic()
        {
            //  var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (Q.GetDamage(target) >= target.Health)
            {
                Q.CastOnUnit(target);
            }

            if ((Player.MoveSpeed - target.MoveSpeed) < 50 && target.IsMoving)
            {

                Q.CastOnUnit(target);
            }
            if ((target.IsDashing() || target.LastCastedSpellName() == "SummonerFlash") )
            {
                Q.CastOnUnit(target);
            }
            if (Player.Health < Player.MaxHealth / 4 )
            {
                Q.CastOnUnit(target);
            }
            if (Q.IsReady())
            {
                Q.CastOnUnit(target);

            }
        }


        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }

    }
}
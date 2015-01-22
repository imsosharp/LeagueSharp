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
    public class Vayne : PluginBase
    {
        public Vayne()
        {
            Q = new Spell(SpellSlot.Q);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast(Target);
                }
                if (Orbwalking.InAutoAttackRange(Target) && Player.HealthPercentage() > 20)
                {
                    if (R.IsReady())
                    {
                        R.Cast();
                    }
                    if (Q.IsReady())
                    {
                        Q.Cast();
                    }

                    Player.IssueOrder(GameObjectOrder.AttackUnit, Target);


                }
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

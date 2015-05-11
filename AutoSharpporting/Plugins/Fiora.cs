using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using AutoSharpporting.Evade;
using AutoSharpporting.Util;
using ActiveGapcloser = AutoSharpporting.Util.ActiveGapcloser;
using SpellData = LeagueSharp.SpellData;

namespace AutoSharpporting.Plugins
{
    public class Fiora : PluginBase
    {
        public Fiora()
        {
            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 350f);
        }

        public override void OnUpdate(EventArgs args)
        {


            if (ComboMode)
            {
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(Target);
                    W.Cast();
                    E.Cast();
                }
                if (R.IsKillable(Target))
                {
                    R.CastOnUnit(Target);
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

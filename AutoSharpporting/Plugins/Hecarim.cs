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
    public class Hecarim : PluginBase
    {

        public Hecarim()
        {
            Q = new Spell(SpellSlot.Q, 350);
            W = new Spell(SpellSlot.W, 525);
            E = new Spell(SpellSlot.E, 0);
            R = new Spell(SpellSlot.R, 1350);

            R.SetSkillshot(0.5f, 200f, 1200f, false, SkillshotType.SkillshotLine);
        }
        public override void OnUpdate(EventArgs args)
        {

            if (ComboMode)
            {
                if (Q.IsReady() && Target.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }
                if (W.IsReady() && Target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
                if (R.CastCheck(Target, "ComboR"))
                {
                    R.Cast(Target,UsePackets);
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

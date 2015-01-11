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
    public class Evelynn : PluginBase
    {
        public Evelynn()
        {
            Q = new Spell(SpellSlot.Q, 500f);
            W = new Spell(SpellSlot.W, Q.Range);
            E = new Spell(SpellSlot.E, 225f + 2 * 65f);
            R = new Spell(SpellSlot.R, 650f);

            R.SetSkillshot(0.25f, 350f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {

            if (ComboMode)
            {

                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast();
                }
                if (W.IsReady() && ObjectManager.Player.HasBuffOfType(BuffType.Slow))
                {
                    W.Cast();
                }

                if (E.CastCheck(Target, "ComboE"))
                {
                    E.CastOnUnit(Target);
                }
                if (R.IsReady())
                {
                    R.CastIfWillHit(Target, 3);
                }
                if (R.IsReady() && R.IsKillable(Target))
                {
                    R.Cast(Target, UsePackets);
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

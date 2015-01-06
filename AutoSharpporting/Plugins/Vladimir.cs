
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
    public class Vladimir : PluginBase
    {
        public Vladimir()
        {

            Q = new Spell(SpellSlot.Q, 600);
            E = new Spell(SpellSlot.E, 610);
            R = new Spell(SpellSlot.R, 700);
            R.SetSkillshot(0.25f, 175, 700, false, SkillshotType.SkillshotCircle);

        }


        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {

                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast(Target, UsePackets);
                }
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(Target, UsePackets);
                }
                if (R.CastCheck(Target, "ComboR"))
                {
                    if (R.IsKillable(Target))
                    {
                        R.Cast(Target, UsePackets);
                    }
                    else 
                    { 
                        R.CastIfWillHit(Target, 2, UsePackets); 
                    }

                }
                if (W.CastCheck(Target, "ComboW") && Player.HealthPercentage() <=20 )
                {
                    W.Cast();
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



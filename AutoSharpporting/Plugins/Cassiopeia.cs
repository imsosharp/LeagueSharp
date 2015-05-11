
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
    public class Cassiopeia : PluginBase
    {
        public Cassiopeia()
        {
            Q = new Spell(SpellSlot.Q, 850);
            Q.SetSkillshot(0.6f, 40f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            W = new Spell(SpellSlot.W, 850);
            W.SetSkillshot(0.5f, 90f, 2500, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 700);
            E.SetTargetted(0.2f, float.MaxValue);

            R = new Spell(SpellSlot.R, 800);
            R.SetSkillshot(0.6f, (float)(80 * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
        }

        public override void OnUpdate(EventArgs args)
        {

            if (ComboMode)
            {
                if(E.CastCheck(Target,"ComboE") && Target.HasBuffOfType(BuffType.Poison))
                {
                    E.CastOnUnit(Target);
                }
                if(Q.CastCheck(Target,"ComboQ"))
                {
                    W.Cast(Target,UsePackets);
                }
                if(W.CastCheck(Target,"ComboW"))
                {
                    W.Cast(Target,UsePackets);
                }
                if(E.CastCheck(Target,"ComboE") )
                {
                    E.CastOnUnit(Target);
                }
                if(R.CastCheck(Target,"ComboR") && R.IsKillable(Target))
                {
                    R.Cast(Target,UsePackets);
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
                R.Cast(unit);
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



        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Interrupt.R", "Use R to Interrupt Spells", true);
        }
    }
}

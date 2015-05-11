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
    public class Aatrox : PluginBase
    {
        public Aatrox()
        {
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, Player.AttackRange + 25);
            E = new Spell(SpellSlot.E, 950); //1000?
            R = new Spell(SpellSlot.R, 550); //300?

            Q.SetSkillshot(0.5f, 180f, 1800f, false, SkillshotType.SkillshotCircle); //width tuned
            E.SetSkillshot(0.5f, 150f, 1200f, false, SkillshotType.SkillshotCone);
        }

        public override void OnUpdate(EventArgs args)
        {

            if (ComboMode)
            {
                if (Q.CastCheck(Target, "ComboQ") )
                {
                    Q.Cast(Target);
                }
                if (W.IsReady() && W.Instance.ToggleState == 1 && Player.HealthPercentage() > 30)
                {
                    W.Cast();
                }
                if (W.IsReady() && W.Instance.ToggleState == 2 && Player.HealthPercentage() < 30)
                {
                    W.Cast();
                }

                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast(Target,UsePackets);
                }

                if (R.CastCheck(Target, "ComboR"))
                {
                    R.Cast(Target);
                }
            }


        }

        public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (Q.CastCheck(unit, "Interrupt.Q"))
            {
                Q.Cast(unit);
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
            config.AddBool("Interrupt.Q", "Use Q to Interrupt Spells", true);
        }
    }
}

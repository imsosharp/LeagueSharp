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
    public class Ahri : PluginBase
    {
        public Ahri()
        {
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 875);
            R = new Spell(SpellSlot.R, 850);

            Q.SetSkillshot(0.25f, 100, 1600, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60, 1200, true, SkillshotType.SkillshotLine);
        }

        public override void OnUpdate(EventArgs args)
        {

            if (ComboMode)
            {

                if (E.CastCheck(Target, "ComboE"))
                {
                    E.CastIfHitchanceEquals(Target, HitChance.High);
                }
                if (Q.CastCheck(Target, "ComboQ") && Target.HasBuffOfType(BuffType.Charm))
                {
                    Q.Cast(Target);
                }
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(Target,UsePackets);
                }
                if (W.CastCheck(Target, "ComboW"))
                {
                    W.Cast();
                }
                if (R.IsReady() && (R.IsKillable(Target) || IsRActive()))
                {
                    R.Cast(Target);
                }           
            }


        }


        bool IsRActive()
        {
            return ObjectManager.Player.HasBuff("AhriTumble", true);
        }

        public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (E.CastCheck(unit, "Interrupt.E"))
            {
                E.CastIfHitchanceEquals(unit, HitChance.Medium);
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
            config.AddBool("Interrupt.E", "Use E to Interrupt Spells", true);
        }
    }
}

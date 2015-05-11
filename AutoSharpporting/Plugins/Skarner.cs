using System;
using AutoSharpporting.Util;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharpporting.Plugins
{
    public class Skarner : PluginBase
    {
        public Skarner()
        {
            Q = new Spell(SpellSlot.Q, 350);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R, 350);


            E.SetSkillshot(0.50f, 60, 1200, false, SkillshotType.SkillshotLine);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.IsReady() && Player.CountEnemiesInRange(Q.Range) > 0)
                {
                    Q.Cast();
                    if (W.IsReady())
                    {
                        W.Cast();
                    }
                }
                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast(Target);
                }
                if (R.IsReady() && Target.IsValidTarget(R.Range))
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

            if (R.IsReady() && unit.IsValidTarget(R.Range))
            {
                R.Cast(unit);
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
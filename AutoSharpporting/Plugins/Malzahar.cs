using System;
using Support.Util;
using LeagueSharp;
using LeagueSharp.Common;

namespace Support.Plugins
{
    public class Malzahar : PluginBase
    {
        public Malzahar()
        {
            //spelldata from Mechanics-StackOverflow Galio
            Q = new Spell(SpellSlot.Q, 850);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 700);

            Q.SetSkillshot(.5f, 30, 1600, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.50f, 50, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {

                if (Q.CastCheck(Target, "ComboQ") && Q.GetPrediction(Target).Hitchance >= HitChance.High)
                {
                    Q.Cast(Target);
                }
                if (E.CastCheck(Target, "ComboE"))
                {
                    E.CastOnUnit(Target);
                }

                if (W.IsReady() && Target.IsValidTarget(W.Range))
                {
                    W.Cast(Target);
                }
                if (R.IsReady() && Target.IsValidTarget(R.Range))
                {
                    R.Cast(Target);
                }

            }
            if (HarassMode)
            {
                if (Q.CastCheck(Target, "ComboQ") && Q.GetPrediction(Target).Hitchance >= HitChance.High)
                {
                    Q.Cast(Target);
                }
                if (E.CastCheck(Target, "ComboE"))
                {
                    E.CastOnUnit(Target);
                }

                if (W.IsReady() && Target.IsValidTarget(W.Range))
                {
                    W.Cast(Target);
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
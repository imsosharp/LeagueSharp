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
    public class Azir : PluginBase
    {


        public Azir()
        {
            Q = new Spell(SpellSlot.Q, 850);
            W = new Spell(SpellSlot.W, 450);
            E = new Spell(SpellSlot.E, 2000);
            R = new Spell(SpellSlot.R, 450);

            Q.SetSkillshot(0.1f, 100, 1700, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 100, 1200, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 700, 1400, false, SkillshotType.SkillshotLine);
        }

        public override void OnUpdate(EventArgs args)
        {

            if (ComboMode)
            {

                if (ShouldR(Target) && R.CastCheck(Target, "ComboR"))
                {
                    R.Cast(Target);
                }
                if (W.CastCheck(Target, "ComboW"))
                {
                    W.Cast(Target);
                }
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(Target);
                }
                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast(Target);
                }

            }


        }



        private bool ShouldR(Obj_AI_Hero target)
        {
            if (Player.GetSpellDamage(target, SpellSlot.R) > target.Health - 150)
                return true;

            var hpPercent = Player.Health / Player.MaxHealth * 100;
            if (hpPercent < 20)
                return true;

            var pred = R.GetPrediction(target);
            if (pred.AoeTargetsHitCount >= 2)
                return true;

            return false;
        }



        public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (E.CastCheck(unit, "Interrupt.R"))
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

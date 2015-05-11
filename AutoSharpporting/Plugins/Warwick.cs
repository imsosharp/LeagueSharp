using System;
using AutoSharpporting.Util;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharpporting.Plugins
{
    public class Warwick : PluginBase
    {
        public Warwick()
        {
            Q = new Spell(SpellSlot.Q, 400);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 1500);
            R = new Spell(SpellSlot.R, 700);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(Target);
                }
                if (R.CastCheck(Target, "ComboR") && R.IsKillable(Target))
                {
                    R.Cast(Target);
                }
                if (Player.HealthPercentage() > 20 && Player.Distance(Target) < 300)
                {
                    if (W.IsReady())
                    {
                        W.Cast();
                    }
                    Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
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
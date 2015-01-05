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
    public class Tristana : PluginBase
    {
        public Tristana()
        {
            Q = new Spell(SpellSlot.Q, 703);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 703);
            R = new Spell(SpellSlot.R, 703);

            W.SetSkillshot(500,270,1500,false,SkillshotType.SkillshotCone);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.CastCheck(Target, "ComboQ") && Orbwalking.InAutoAttackRange(Target))
                {
                    Q.Cast();
                }
                if (W.CastCheck(Target, "ComboW") && W.IsKillable(Target))
                {
                    W.Cast(Target, UsePackets);
                }
                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast(Target, UsePackets);
                }

                if (R.CastCheck(Target, "ComboR") && R.IsKillable(Target))
                {
                    R.Cast(Target, UsePackets);
                }
                if (Orbwalking.InAutoAttackRange(Target))
                {
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
                R.Cast(unit, UsePackets);
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

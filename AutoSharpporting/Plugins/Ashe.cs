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
    public class Ashe : PluginBase
    {
        public Ashe()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1200);
            E = new Spell(SpellSlot.E, 2500);
            R = new Spell(SpellSlot.R, 20000);


            W.SetSkillshot(250f, (float)(24.32f * Math.PI / 180), 902f, true, SkillshotType.SkillshotCone);
            E.SetSkillshot(377f, 299f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(250f, 130f, 1600f, false, SkillshotType.SkillshotLine);
        }

        public override void OnUpdate(EventArgs args)
        {

            var targetR = TargetSelector.GetTarget(10000, TargetSelector.DamageType.Magical);
            if (ComboMode)
            {
               /* if (Orbwalking.InAutoAttackRange(Target) && Q.Instance.ToggleState == 1)
                {
                    Q.Cast();
                }*/
                if (W.CastCheck(Target, "ComboW"))
                {
                    W.Cast(Target, UsePackets);
                }

                if (R.CastCheck(targetR, "ComboR") && R.IsKillable(targetR))
                {
                    R.Cast(targetR, UsePackets);
                }
                if (Orbwalking.InAutoAttackRange(Target) && Player.HealthPercentage() > 30)
                {
                    Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
                }
            }

            if (HarassMode)
            {
                if (W.CastCheck(Target, "HarassW"))
                {
                    W.Cast(Target, UsePackets);
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

        public bool IsQActive
        {
            get
            {
                return ObjectManager.Player.HasBuff("FrostShot");
            }
        }

        public override void ComboMenu(Menu config)
        {
           // config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboR", "Use R", true);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassW", "Use W", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Interrupt.R", "Use R to Interrupt Spells", true);
        }
    }
}

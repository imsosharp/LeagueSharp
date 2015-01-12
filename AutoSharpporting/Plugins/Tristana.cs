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
                KS();
                if (Q.CastCheck(Target, "ComboQ") && Orbwalking.InAutoAttackRange(Target))
                {
                    Q.Cast();
                }
                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast(Target, UsePackets);
                }
            }
        

        }

        public void KS()
        {

            foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => Player.Distance(x) < 900 && x.IsValidTarget() && x.IsEnemy && !x.IsDead))
            {
                if (target != null)
                {
                    //R
                    if (Player.Distance(target.ServerPosition) <= R.Range &&
                        (Player.GetSpellDamage(target, SpellSlot.R)) > target.Health + 50)
                    {
                        if (R.CastCheck(Target, "ComboRKS"))
                        {
                            R.CastOnUnit(target, UsePackets);
                            return;
                        }
                        if (W.CastCheck(Target, "ComboW"))
                        {
                            W.Cast(Target, UsePackets);
                        }
                    }


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
            config.AddBool("ComboRKS", "Use R KS", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Interrupt.R", "Use R to Interrupt Spells", true);
        }
    }
}

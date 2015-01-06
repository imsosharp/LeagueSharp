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
    public class Veigar : PluginBase
    {
        public Veigar()
        {

            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 1005);
            R = new Spell(SpellSlot.R, 650);
            W.SetSkillshot(1.25f, 230f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(.2f, 330f, float.MaxValue, false, SkillshotType.SkillshotCircle);

        }


        public override void OnUpdate(EventArgs args)
        {
            KS();
            if (ComboMode)
            {

                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast(Target, true);
                }
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(Target, true);
                }
                if (W.CastCheck(Target, "ComboW"))
                {
                    W.Cast(Target.Position, true);
                }

            }

            if (HarassMode)
            {
                if (Q.CastCheck(Target, "HarassQ"))
                {
                    Q.Cast(Target, true);
                }
                if (W.CastCheck(Target, "HarassW"))
                {
                    W.Cast(Target, true);
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

            if (E.CastCheck(unit, "Interrupt.E"))
            {
                E.Cast(unit, true);
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

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassQ", "Use Q", true);
            config.AddBool("HarassW", "Use W", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Interrupt.E", "Use E to Interrupt Spells", true);
        }

    }
}



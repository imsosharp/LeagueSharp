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
    public class Katarina : PluginBase
    {
        public Katarina()
        {
            Q = new Spell(SpellSlot.Q, 675);
            W = new Spell(SpellSlot.W, 375);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 550);

            Q.SetTargetted(400, 1400);
            R.SetCharged("KatarinaR", "KatarinaR", 550, 550, 1.0f);
        }


        public override void OnUpdate(EventArgs args)
        {
            if (!R.IsCharging)
            {
                Orbwalker.SetMovement(true);
                Orbwalker.SetAttack(true);
            }

            KS();


            if (Player.IsChannelingImportantSpell() || Player.HasBuff("katarinarsound", true) )
            {
                return;
            }

            if (ComboMode)
            {
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(Target);
                }
                if (W.IsReady() && Player.Distance(Target) <= W.Range)
                {
                    W.Cast();
                }

            }
        }

        

        public void KS()
        {

            foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => Player.Distance(x) < 900 && x.IsValidTarget() && x.IsEnemy && !x.IsDead))
            {
                if (target != null)
                {
                   
                    if (Player.Distance(target.ServerPosition) <= E.Range &&
                        (Player.GetSpellDamage(target, SpellSlot.E)) > target.Health)
                    {
                        if (E.CastCheck(Target, "ComboE"))
                        {                       
                            E.CastOnUnit(target);
                            if (R.IsReady())
                            {
                                R.CastIfWillHit(Target, 2);
                                Orbwalker.SetMovement(false);
                                Orbwalker.SetAttack(false);
                            }
                        }
                    }


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

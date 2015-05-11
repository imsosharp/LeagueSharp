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
    public class Akali : PluginBase
    {
        public Akali()
        {
            Q = new Spell(SpellSlot.Q, 600);

            W = new Spell(SpellSlot.W, 700);

            E = new Spell(SpellSlot.E, 325);

            R = new Spell(SpellSlot.R, 800);
        }

        public override void OnUpdate(EventArgs args)
        {
            KS();
            if (ComboMode)
            {
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(Target);
                }
                if (W.IsReady() && (Player.HealthPercentage() < 20 || (!Q.IsReady() && !E.IsReady() && !R.IsReady())))
                {
                    W.Cast(Player.Position);
                }
                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast(Target);
                }
                if (R.CastCheck(Target, "ComboRKS"))
                {
                    R.Cast(Target);
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
                            R.CastOnUnit(target);
                            return;
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
            config.AddBool("ComboRKS", "Use R", true);
        }
    }
}

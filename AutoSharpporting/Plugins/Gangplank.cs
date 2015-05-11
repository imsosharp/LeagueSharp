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
    public class Gangplank : PluginBase
    {

        public Gangplank()
        {
            //Spell
            Q = new Spell(SpellSlot.Q, 625);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 25000);
        }
        public override void OnUpdate(EventArgs args)
        {
            autoW();
            KS();


            if (ComboMode)
            {
                Combo(Target);
            }


        }

        private void Combo(Obj_AI_Hero Target)
        {


            if (E.IsReady())
            {
                E.Cast();
            }
            if (Q.IsReady() && Player.Distance(Target) <= Q.Range)
            {
                Q.CastOnUnit(Target);
            }

            if (R.IsReady() && Player.Distance(Target) <= R.Range )
            {
                R.CastIfWillHit(Target, 2);
            }


        }


        private void autoW()
        {
            if (Player.HasBuffOfType(BuffType.Taunt) || Player.HasBuffOfType(BuffType.Stun) || Player.HasBuffOfType(BuffType.Snare) ||
                Player.HasBuffOfType(BuffType.Polymorph) || Player.HasBuffOfType(BuffType.Blind) || Player.HasBuffOfType(BuffType.Fear) || Player.HasBuffOfType(BuffType.Silence))
            {
                if (W.IsReady())
                {
                    W.Cast();
                }
            }
            return;

        }

        private void KS()
        {


            foreach (var target in ObjectManager.Get<Obj_AI_Hero>())
            {

                if (!target.IsDead && Q.IsReady() && !target.IsAlly && Player.Distance(target.Position) < Q.Range && Player.GetSpellDamage(target, SpellSlot.Q) > (target.Health + 20))
                {
                    //Game.PrintChat("Qks");
                    Q.CastOnUnit(target);
                }


                if (R.IsReady() && !target.IsDead && !target.IsAlly && Player.Distance(target.Position) < R.Range && Player.GetSpellDamage(target, SpellSlot.R) > (target.Health))
                {
                    //Game.PrintChat("Rks");
                    R.Cast(target);
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


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
    public class Diana : PluginBase
    {
        public Diana()
        {
            Q = new Spell(SpellSlot.Q, 830f);
            W = new Spell(SpellSlot.W, 200f);
            E = new Spell(SpellSlot.E, 420f);
            R = new Spell(SpellSlot.R, 825f);

            Q.SetSkillshot(0.35f, 200f, 1800, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                Random rnd = new Random();
                int mode = rnd.Next(0, 1);
                if (mode == 1)
                {
                    Combo(Target);
                }
                else
                {
                    Misaya(Target);
                }
            }

        }

        //From Diabaths
        private void Misaya(Obj_AI_Hero t)
        {
            var target = t;
            var qmana = Player.Spellbook.GetSpell(SpellSlot.Q).ManaCost;
            var rmana = Player.Spellbook.GetSpell(SpellSlot.R).ManaCost;


            if (Player.Distance(target) <= Q.Range && Q.IsReady() && R.IsReady())
            {
                if (Q.GetPrediction(target).Hitchance >= HitChance.High && Player.Mana > qmana + rmana)
                {
                    R.Cast(target, UsePackets);
                    Q.CastIfHitchanceEquals(target, HitChance.High, UsePackets);

                }
            }
            if (Player.Distance(target) <= W.Range && W.IsReady())
            {
                W.Cast();
            }
            if (Player.Distance(target) <= E.Range && Player.Distance(target) >= W.Range && E.IsReady() && !W.IsReady())
            {
                E.Cast();
            }
            if (Player.Distance(target) <= R.Range && R.IsReady() && !W.IsReady() && !Q.IsReady())
            {
                R.Cast(target, UsePackets);
            }
        }

        //From Diabaths
        private void Combo(Obj_AI_Hero t)
        {
            var target = t;

            if (Player.Distance(target) <= Q.Range && Q.IsReady() &&
                Q.GetPrediction(target).Hitchance >= HitChance.High)
            {
                Q.CastIfHitchanceEquals(target, HitChance.High);
            }

            if (Player.Distance(target) <= R.Range && R.IsReady() && (target.HasBuff("dianamoonlight", true)))
            {
                R.Cast(target, UsePackets);
            }

            if (Player.Distance(target) <= W.Range && W.IsReady() &&
                !Q.IsReady())
            {
                W.Cast();
            }
            if (Player.Distance(target) <= E.Range && Player.Distance(target) >= W.Range && E.IsReady() && !W.IsReady())
            {
                E.Cast();
            }
            if (Player.Distance(target) <= R.Range && R.IsReady() && !W.IsReady() && !Q.IsReady())
            {
                R.Cast(target, UsePackets);
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

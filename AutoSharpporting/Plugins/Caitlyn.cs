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
    public class Caitlyn : PluginBase
    {
        public Caitlyn()
        {
            Q = new Spell(SpellSlot.Q, 1240);
            W = new Spell(SpellSlot.W, 820);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 2000);

            Q.SetSkillshot(0.25f, 60f, 2000f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 80f, 1600f, true, SkillshotType.SkillshotLine);
        }


        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser) 
        {
            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
                E.CastOnUnit(gapcloser.Sender);
        }

        public override void OnUpdate(EventArgs args)
        {
            R.Range = 500 * R.Level + 1500;
            Obj_AI_Hero t;

            if (ComboMode)
            {



                //Auto W (Stun/Snare/Taunt)
                if (W.IsReady())
                {
                    t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                    if (t.IsValidTarget(W.Range) &&
                        (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) ||
                        t.HasBuffOfType(BuffType.Taunt) || t.HasBuff("zhonyasringshield") ||
                        t.HasBuff("Recall")))
                    {
                        W.Cast(t.Position);
                    }
                }

                //Auto Q (Stun/Snare/Taunt/Slow)
                if (Q.IsReady())
                {
                    t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                    if (t.IsValidTarget(Q.Range) &&
                        (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) ||
                         t.HasBuffOfType(BuffType.Taunt) || t.HasBuffOfType(BuffType.Slow)))
                    {
                        Q.Cast(t, false, true);
                    }
                }

                if (R.IsReady())
                {
                    t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                    if (t != null && t.Health <= R.GetDamage(t) &&
                        !Orbwalking.InAutoAttackRange(t))
                    {
                        R.CastOnUnit(t);
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

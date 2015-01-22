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
    public class JarvanIV : PluginBase
    {
        private bool haveUlti = false;
        private static Vector3 _epos = default(Vector3);
        public JarvanIV()
        {

            //spelldata from Mechanics-StackOverflow Galio
            Q = new Spell(SpellSlot.Q, 770f);
            W = new Spell(SpellSlot.W, 300f);
            E = new Spell(SpellSlot.E, 830f);
            R = new Spell(SpellSlot.R, 650f);
        }


        public override void OnUpdate(EventArgs args)
        {

            if (ComboMode)
            {
                Combo(Target);
            }


        }

        private int GetNumberHitByR(Obj_AI_Hero target)
        {
            int Enemys = 0;
            foreach (Obj_AI_Hero enemys in ObjectManager.Get<Obj_AI_Hero>())
            {
                var pred = R.GetPrediction(enemys, true);
                if (pred.Hitchance >= HitChance.High && !enemys.IsMe && enemys.IsEnemy && Vector3.Distance(Player.Position, pred.UnitPosition) <= R.Range)
                {
                    Enemys = Enemys + 1;
                }
            }
            return Enemys;
        }

        //some part of combo from diabaths
        private void Combo(Obj_AI_Hero t)
        {
 

            if (R.IsReady())
            {
                if (t.IsValidTarget(Q.Range) && !haveUlti)
                    if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage"))
                        R.CastIfHitchanceEquals(t, HitChance.Medium);
            }
            if ( E.IsReady() && t.IsValidTarget(Q.Range) && Q.IsReady())
            {
                //xsalice Code
                var vec = t.ServerPosition - Player.ServerPosition;
                var castBehind = E.GetPrediction(t).CastPosition + Vector3.Normalize(vec) * 100;
                E.Cast(castBehind);
            }
            if (t.IsValidTarget(Q.Range) && Q.IsReady() && _epos != default(Vector3) &&
                t.IsValidTarget(200, true, _epos))
            {
                Q.Cast(_epos);
            }

            if (W.IsReady())
            {
                if (t.IsValidTarget(W.Range))
                    W.Cast();
            }
            if (Q.IsReady() && !E.IsReady())
            {
                if (t.IsValidTarget(Q.Range))
                    Q.Cast(t, UsePackets, true);
            }
            if (R.IsReady() && !haveUlti)
            {
                if (GetNumberHitByR(t) >= 2)
                    R.Cast(t, UsePackets, true);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("AutoCombo", "AutoCombo", true);
        }

    }
}

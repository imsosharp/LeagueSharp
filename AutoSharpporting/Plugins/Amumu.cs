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
    public class Amumu : PluginBase
    {

        private bool wUse = false;
        public Amumu()
        {
            Q = new Spell(SpellSlot.Q, 1100);
            Q.SetSkillshot(Q.Instance.SData.SpellCastTime, Q.Instance.SData.LineWidth, Q.Instance.SData.MissileSpeed, true, SkillshotType.SkillshotLine);


            W = new Spell(SpellSlot.W, 300);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R, 550);

        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {

                

                var qPred = Q.GetPrediction(Target);

                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(qPred.CastPosition);
                }

                if (W.IsReady() && !wUse && Player.CountEnemiesInRange(R.Range) >= 1)
                {
                    W.Cast();
                    wUse = true;
                }
                if (wUse && Player.CountEnemiesInRange(R.Range) == 0)
                {
                    W.Cast();
                    wUse = false;
                }

                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast();
                }

                if (R.CastCheck(Target, "ComboR"))
                {
                    R.CastIfWillHit(Target, 2);
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

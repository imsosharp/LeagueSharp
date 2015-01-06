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
    public class Amumu : PluginBase
    {
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

                if (Target == null)
                {
                    if (W.Instance.ToggleState == 2)
                    {
                        W.Cast();
                    }
                    return;
                }

                var qPred = Q.GetPrediction(Target);

                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(qPred.CastPosition, UsePackets);
                }

                if (W.Instance.ToggleState == 1 && R.CastCheck(Target, "ComboW"))
                {
                    W.Cast();
                }

                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast();
                }

                if (R.CastCheck(Target, "ComboR"))
                {
                    R.CastIfWillHit(Target, 2, UsePackets);
                }
                if ((Player.Distance(Target) < 400 || Orbwalking.InAutoAttackRange(Target) )&& Player.HealthPercentage() > 30)
                {
                    Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
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

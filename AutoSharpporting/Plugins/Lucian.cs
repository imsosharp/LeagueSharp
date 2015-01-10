//xaxixeo *-*
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
    public class Lucian : PluginBase
    {
        public Lucian()
        {
            Q = new Spell(SpellSlot.Q, 675);
            Q.SetTargetted(0.25f, float.MaxValue);

            W = new Spell(SpellSlot.W, 1000);
            W.SetSkillshot(0.3f, 80, 1600, true, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 425);
            E.SetSkillshot(.25f, 1f, float.MaxValue, false, SkillshotType.SkillshotLine);

            R = new Spell(SpellSlot.R, 1400);
            R.SetSkillshot(.1f, 110, 2800, true, SkillshotType.SkillshotLine);
        }

        public override void OnUpdate(EventArgs args)
        {

            var targetR = TargetSelector.GetTarget(10000, TargetSelector.DamageType.Magical);
            if (ComboMode)
            {

                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(Target, UsePackets);
                }
                if (W.CastCheck(Target, "ComboW"))
                {
                    W.Cast(Target, UsePackets);
                }
                if (R.CastCheck(Target, "ComboR"))
                {
                    if (R.IsKillable(Target))
                    {
                        R.Cast(Target, UsePackets);
                    }
                    if (!Q.IsReady() && !W.IsReady())
                    {
                        R.Cast(Target, UsePackets);
                    }
                }
                if (Orbwalking.InAutoAttackRange(Target) && Player.HealthPercentage() > 20)
                {
                    Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
                }
            }

           
        }

       


        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboR", "Use R", true);
        }

    }
}

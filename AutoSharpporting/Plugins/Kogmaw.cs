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
    public class Kogmaw : PluginBase
    {

        public Kogmaw()
        {

            Q = new Spell(SpellSlot.Q, 1000f);
            W = new Spell(SpellSlot.W, float.MaxValue);
            E = new Spell(SpellSlot.E, 1360f);
            R = new Spell(SpellSlot.R, float.MaxValue);

            Q.SetSkillshot(0.25f, 70f, 1650f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1.2f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }
        public override void OnUpdate(EventArgs args)
        {

            if (ComboMode)
            {
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.CastIfHitchanceEquals(Target, HitChance.Medium, UsePackets);
                }
                if (W.IsReady() && Orbwalking.InAutoAttackRange(Target))
                {
                    W.Cast();
                }
                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast(Target, UsePackets);
                }
                if (R.IsReady() && GetUltimateBuffStacks() < 3)
                {
                    var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                    if (t != null)
                    R.Cast(t, false, true);
                }

            }


        }

        private int GetUltimateBuffStacks()
        {
            return (from buff in ObjectManager.Player.Buffs
                    where buff.DisplayName.ToLower() == "kogmawlivingartillery"
                    select buff.Count).FirstOrDefault();
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

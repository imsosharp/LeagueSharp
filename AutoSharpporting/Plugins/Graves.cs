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
    public class Graves : PluginBase
    {

        public Graves()
        {
            Q = new Spell(SpellSlot.Q, 900f); 
            Q.SetSkillshot(0.25f, 15f * 1.5f * (float)Math.PI / 180, 2000f, false, SkillshotType.SkillshotCone);

            W = new Spell(SpellSlot.W, 1100f);
            W.SetSkillshot(0.25f, 250f, 1650f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 1100f);
            R.SetSkillshot(0.25f, 100f, 2100f, true, SkillshotType.SkillshotLine);
        }
        public override void OnUpdate(EventArgs args)
        {

            if (ComboMode)
            {

                if (Q.IsReady())
                {
                  Q.Cast(Target, UsePackets, true);
                }

                if (W.IsReady())
                {
                  W.Cast(Target, UsePackets, true);
                }

                if (R.IsReady())
                {
                    foreach (
                        var hero in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(
                                    hero =>
                                        hero.IsValidTarget(R.Range) &&
                                        ObjectManager.Player.GetSpellDamage(hero, SpellSlot.R, 1) - 20 > hero.Health))
                        R.Cast(hero, UsePackets, true);
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


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
    public class Drmundo : PluginBase
    {
        public Drmundo()
        {
            Q = new Spell(SpellSlot.Q, 930);
            W = new Spell(SpellSlot.W, 320);
            E = new Spell(SpellSlot.E, 225);
            R = new Spell(SpellSlot.R, 0);

            Q.SetSkillshot(0.50f, 75f, 1500f, true, SkillshotType.SkillshotLine);

        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                Combo(Target);
            }

        }

        //from mundo TheKushStyle
        private void Combo(Obj_AI_Hero target)
        {
            bool ActiveW = false;
            if (Player.HasBuff("BurningAgony"))
            {
                ActiveW = true;
            }
            else
            {
                ActiveW = false;
            }

            if (Q.CastCheck(target,"ComboQ"))
            {
                Q.Cast(target, UsePackets);
            }

            if (target.IsValidTarget() && W.IsReady() && Player.Distance(target) <= W.Range && !ActiveW)
            {
                W.Cast();
            }
            if (target.IsValidTarget() && Player.Distance(target) > 700f && ActiveW)
            {
                W.Cast();
            }

            if (E.IsReady() && Player.Distance(target) <= E.Range)
            {
                E.Cast();
            }

            int EnInRang = Utility.CountEnemysInRange(Player,1000);

            if (Player.HealthPercentage() < 30 && R.IsReady() && EnInRang >= 1 || EnInRang == 1)
            {
                R.Cast();
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

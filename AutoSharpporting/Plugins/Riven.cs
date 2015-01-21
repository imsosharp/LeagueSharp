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
    public class Riven : PluginBase
    {
        public bool RActive = false;

        public Riven()
        {
        E = new Spell(SpellSlot.E, 390f);
        Q = new Spell(SpellSlot.Q, 250f);
        W = new Spell(SpellSlot.W, 150f);
        R = new Spell(SpellSlot.R, 900f);
        }


        public override void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {

            var t = target as Obj_AI_Hero;
            if (t != null && unit.IsMe) //lmao //this make castQ after autoattack champions
            {
                if (Q.IsReady())
                {
                    Q.Cast();
                    if (R.IsReady() && !RActive)
                    {
                        R.Cast();
                        RActive = true;
                    }
                    Player.IssueOrder(GameObjectOrder.AttackTo, t);
                }
            }

        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (W.CastCheck(Target, "ComboW"))
                {
                    W.Cast(Target, UsePackets);
                }
                if (RActive && Player.Distance(Target) < R.Range && R.IsKillable(Target))
                {
                    R.Cast(Target);
                    RActive = false;
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

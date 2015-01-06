﻿using System;
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
    public class Garen : PluginBase
    {
        public Garen()
        {

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E,165);
            R = new Spell(SpellSlot.R,400);

        }


        public override void OnUpdate(EventArgs args)
        {
            KS();
            if (ComboMode)
            {
                if (Player.HealthPercentage() > 20 && Player.Distance(Target) < R.Range)
                {
                    if(W.IsReady()){
                        W.Cast();
                    }
                    if(E.IsReady()){
                        E.Cast();
                    }
                    if(Q.IsReady()){
                        Q.Cast();
                    }
                    Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
                    
                }
            }

        }

        public void KS()
        {

            foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => Player.Distance(x) < 900 && x.IsValidTarget() && x.IsEnemy && !x.IsDead))
            {
                if (target != null)
                {
                    //R
                    if (Player.Distance(target.ServerPosition) <= R.Range &&
                        (Player.GetSpellDamage(target, SpellSlot.R)) > target.Health + 50)
                    {
                        if (R.CastCheck(Target, "ComboRKS"))
                        {
                            R.CastOnUnit(target, UsePackets);
                            return;
                        }
                    }
                }
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboRKS", "Use R KS", true);
        }

    }
}



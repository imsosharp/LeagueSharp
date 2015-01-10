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
    public class Darius : PluginBase
    {
        public Darius()
        {
            Q = new Spell(SpellSlot.Q, 425);
            W = new Spell(SpellSlot.W, 145);
            E = new Spell(SpellSlot.E, 550);
            R = new Spell(SpellSlot.R, 460);

        }

        public override void OnUpdate(EventArgs args)
        {
            ExecuteKillsteal();
            AutoQ();
            if (ComboMode)
            {
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast();
                }
                if (W.CastCheck(Target, "ComboW"))
                {
                    W.Cast();
                }
                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast();
                }

            }
        }

        public void AutoQ()
        {
            if (!Q.IsReady()) return;
            foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => Player.Distance(x) < Q.Range-10 && Player.Distance(x) > 270 && x.IsValidTarget() && x.IsEnemy && !x.IsDead))
            {
                if (Q.IsReady())
                {
                    Q.Cast();
                }
            }
        }
           
        public void ExecuteKillsteal()
        {

            foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => Player.Distance(x) < 900 && x.IsValidTarget() && x.IsEnemy && !x.IsDead))
            {

                if (Q.IsReady() && Player.Distance(target.Position) < Q.Range && Player.GetSpellDamage(target, SpellSlot.Q) > (target.Health+20))
                {
                    Q.Cast();
                }


                if (R.IsReady() && Player.Distance(target.Position) < R.Range )
                {
                    CastR(target);
                }

            }
        }

        // R Calculate Credit TC-Crew
        public void CastR(Obj_AI_Base target)
        {
            if (!target.IsValidTarget(R.Range) || !R.IsReady()) return;

            if (!(ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q, 1) > target.Health))
            {
                foreach (var buff in target.Buffs)
                {
                    if (buff.Name == "dariushemo")
                    {
                        if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.R, 1) *
                            (1 + buff.Count / 5) - 1 > (target.Health))
                        {
                            R.CastOnUnit(target, true);
                        }
                    }
                }
            }
            else if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.R, 1) - 15 >
                     (target.Health))
            {
                R.CastOnUnit(target, true);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
        }
    }
}

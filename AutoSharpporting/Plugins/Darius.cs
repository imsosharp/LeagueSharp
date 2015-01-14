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

        public override void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {

            if (!unit.IsMe)
            {
                return;
            }

            var t = target as Obj_AI_Hero;
            if (unit.IsMe && t != null)
            {
                if(W.IsReady())
                {
                    W.Cast();
                    Orbwalking.ResetAutoAttackTimer();
                }

            }

        }

        public override void OnUpdate(EventArgs args)
        {
            ExecuteKillsteal();
            if (ComboMode)
            {

                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast(Target);
                }
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast();
                }
                if (W.CastCheck(Target, "ComboW"))
                {
                    W.Cast();
                }

            }
        }
           
        public void ExecuteKillsteal()
        {

            foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => Player.Distance(x) < R.Range && x.IsEnemy && !x.IsDead))
            {

                if (R.IsReady() && Player.Distance(target) <= R.Range  && R.IsKillable(target))
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

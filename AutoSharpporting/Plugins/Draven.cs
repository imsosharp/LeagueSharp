using System;
using System.Linq;
using AutoSharpporting.Util;
using LeagueSharp;
using LeagueSharp.Common;
using ActiveGapcloser = AutoSharpporting.Util.ActiveGapcloser;

namespace AutoSharpporting.Plugins
{
    public class Draven : PluginBase
    {
        private bool blockR2;

        public Draven()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 1100);
            R = new Spell(SpellSlot.R, 20000);
            E.SetSkillshot(250f, 130f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(400f, 160f, 2000f, false, SkillshotType.SkillshotLine);
        }

        public void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
            {
                E.Cast(gapcloser.Sender);
            }
        }

        public void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (E.IsReady() && unit.IsValidTarget(E.Range))
            {
                E.Cast(unit);
            }
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
                W.Cast();
                Q.Cast();
            }
        }

        public override void OnUpdate(EventArgs args)
        {
            if (R.Instance.Cooldown > 0)
            {
                blockR2 = false;
            }
            KS();

            if (ComboMode)
            {
                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast(Target);
                }
            }
        }

        public void KS()
        {
            foreach (
                var target in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => Player.Distance(x) < 2000 && x.IsValidTarget() && x.IsEnemy && !x.IsDead))
            {
                if (target != null)
                {
                    //R
                    if (Player.Distance(target.ServerPosition) <= R.Range &&
                        (Player.GetSpellDamage(target, SpellSlot.R)) > target.Health + 100)
                    {
                        if (R.CastCheck(Target, "ComboR") && !blockR2)
                        {
                            R.Cast(target);
                            blockR2 = true;
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
            config.AddBool("ComboR", "Use R", true);
        }
    }
}
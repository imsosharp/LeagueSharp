//Get Some Part From xSaliceReligionAIO Credit xSalice

using System;
using System.Linq;
using AutoSharpporting.Util;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AutoSharpporting.Plugins
{
    public class Anivia : PluginBase
    {
        //R
        public Anivia()
        {
            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 625);

            Q.SetSkillshot(.5f, 110f, 750f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(.25f, 1f, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(.25f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (E.CastCheck(Target, "ComboE") && ShouldE(Target))
                {
                    E.CastOnUnit(Target);
                }

                //Q
                if (Q.CastCheck(Target, "ComboQ") && ShouldQ())
                {
                    Q.CastIfHitchanceEquals(Target, HitChance.Medium);
                }

                if (W.CastCheck(Target, "ComboW"))
                {
                    CastW(Target);
                }

                if (R.CastCheck(Target, "ComboR") &&
                    R.GetPrediction(Target).Hitchance >= HitChance.High)
                {
                    R.Cast(Target);
                }
            }
        }

        private void SmartKs()
        {
            foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(1300)))
            {
                //ER
                if (Player.Distance(target.ServerPosition) <= R.Range && R.Instance.ToggleState == 1 &&
                    (Player.GetSpellDamage(target, SpellSlot.R) + Player.GetSpellDamage(target, SpellSlot.E)*2) >
                    target.Health + 50)
                {
                    if (R.IsReady() && E.IsReady())
                    {
                        E.CastOnUnit(target);
                        R.CastOnUnit(target);
                        return;
                    }
                }

                //QR
                if (Player.Distance(target.ServerPosition) <= R.Range && ShouldQ() &&
                    (Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.R)) >
                    target.Health + 30)
                {
                    if (W.IsReady() && R.IsReady())
                    {
                        W.Cast(target);
                        return;
                    }
                }

                //Q
                if (Player.Distance(target.ServerPosition) <= Q.Range && ShouldQ() &&
                    (Player.GetSpellDamage(target, SpellSlot.Q)) > target.Health + 30)
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(target);
                        return;
                    }
                }

                //E
                if (Player.Distance(target.ServerPosition) <= E.Range &&
                    (Player.GetSpellDamage(target, SpellSlot.E)) > target.Health + 30)
                {
                    if (E.IsReady())
                    {
                        E.CastOnUnit(target);
                        return;
                    }
                }
            }
        }

        private void CastW(Obj_AI_Hero target)
        {
            var pred = W.GetPrediction(target);
            var vec = new Vector3(pred.CastPosition.X - Player.ServerPosition.X, 0,
                pred.CastPosition.Z - Player.ServerPosition.Z);
            var castBehind = pred.CastPosition + Vector3.Normalize(vec)*125;

            if (W.IsReady())
                W.Cast(castBehind);
        }

        private bool ShouldE(Obj_AI_Hero target)
        {
            if (checkChilled(target))
                return true;

            if (Player.GetSpellDamage(target, SpellSlot.E) > target.Health)
                return true;

            if (R.IsReady() && Player.Distance(target) <= R.Range - 25 && Player.Distance(target.ServerPosition) > 250)
                return true;

            return false;
        }

        private bool checkChilled(Obj_AI_Hero target)
        {
            return target.HasBuff("Chilled");
        }

        private bool ShouldQ()
        {
            if (Environment.TickCount - Q.LastCastAttemptT > 2000)
                return true;

            return false;
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
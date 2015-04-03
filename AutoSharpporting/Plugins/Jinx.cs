// Jinx Logic From Markmans 
using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using AIM.Util;

namespace AIM.Plugins
{
    public class Jinx : PluginBase
    {
        public Jinx()
        {

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1500f);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 25000f);

            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.7f, 120f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.6f, 140f, 1700f, false, SkillshotType.SkillshotLine);
        }


        public override void OnUpdate(EventArgs args)
        {

            AutoE();

            if (FishBoneActive &&
               (!ComboMode && TargetSelector.GetTarget(675f + QAddRange, TargetSelector.DamageType.Physical) == null))
            {
                Q.Cast();
            }

            if (ComboMode)
            {
                Combo(Target);
            }


        }

        private void AutoE()
        {

            foreach (
                var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsValidTarget(E.Range - 150)))
            {
                if (E.IsReady() && enemy.HasBuffOfType(BuffType.Slow))
                {
                    var castPosition =
                        Prediction.GetPrediction(
                            new PredictionInput
                            {
                                Unit = enemy,
                                Delay = 0.7f,
                                Radius = 120f,
                                Speed = 1750f,
                                Range = 900f,
                                Type = SkillshotType.SkillshotCircle,
                            }).CastPosition;


                    if (GetSlowEndTime(enemy) >= (Game.Time + E.Delay + 0.5f))
                    {
                        E.Cast(castPosition);
                    }
                }

                if (E.IsReady() &&
                    (enemy.HasBuffOfType(BuffType.Stun) || enemy.HasBuffOfType(BuffType.Snare) ||
                     enemy.HasBuffOfType(BuffType.Charm) || enemy.HasBuffOfType(BuffType.Fear) ||
                     enemy.HasBuffOfType(BuffType.Taunt) || enemy.HasBuff("zhonyasringshield") ||
                     enemy.HasBuff("Recall")))
                {
                    E.CastIfHitchanceEquals(enemy, HitChance.High);
                }

                if (E.IsReady() && enemy.IsDashing())
                {
                    E.CastIfHitchanceEquals(enemy, HitChance.Dashing);
                }
            }
        }

        private void Combo(Obj_AI_Hero target)
        {
            if (W.IsReady())
            {
                var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget() && GetRealDistance(t) >= 800)
                {
                    if (W.Cast(t) == Spell.CastStates.SuccessfullyCasted)
                    {
                        return;
                    }
                }
            }

            var useQ = true;
            if (useQ)
            {
                foreach (var t in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(t => t.IsValidTarget(GetRealPowPowRange(t) + QAddRange + 20f)))
                {
                    var swapDistance = true;
                    var swapAoe = true;
                    var distance = GetRealDistance(t);
                    var powPowRange = GetRealPowPowRange(t);

                    if (swapDistance && Q.IsReady())
                    {
                        if (distance > powPowRange && !FishBoneActive)
                        {
                            if (Q.Cast())
                            {
                                return;
                            }
                        }
                        else if (distance < powPowRange && FishBoneActive)
                        {
                            if (Q.Cast())
                            {
                                return;
                            }
                        }
                    }

                    if (swapAoe && Q.IsReady())
                    {
                        if (distance > powPowRange && PowPowStacks > 2 && !FishBoneActive && CountEnemies(t, 150) > 1)
                        {
                            if (Q.Cast())
                            {
                                return;
                            }
                        }
                    }
                }
            }


            if (R.IsReady())
            {
                var checkRok = true;
                var minR = 1200;
                var maxR = 2500;
                var t = TargetSelector.GetTarget(maxR, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget())
                {
                    var distance = GetRealDistance(t);

                    if (!checkRok)
                    {
                        if (ObjectManager.Player.GetSpellDamage(t, SpellSlot.R, 1) > t.Health)
                        {
                            if (R.Cast(t) == Spell.CastStates.SuccessfullyCasted) { }
                        }
                    }
                    else if (distance > minR)
                    {
                        var aDamage = ObjectManager.Player.GetAutoAttackDamage(t);
                        var wDamage = ObjectManager.Player.GetSpellDamage(t, SpellSlot.W);
                        var rDamage = ObjectManager.Player.GetSpellDamage(t, SpellSlot.R);
                        var powPowRange = GetRealPowPowRange(t);

                        if (distance < (powPowRange + QAddRange) && !(aDamage * 3.5 > t.Health))
                        {
                            if (!W.IsReady() || !(wDamage > t.Health) || W.GetPrediction(t).CollisionObjects.Count > 0)
                            {
                                if (CountAlliesNearTarget(t, 500) <= 3)
                                {
                                    if (rDamage > t.Health /*&& !ObjectManager.Player.IsAutoAttacking &&
                                        !ObjectManager.Player.IsChanneling*/)
                                    {
                                        if (R.Cast(t) == Spell.CastStates.SuccessfullyCasted) { }
                                    }
                                }
                            }
                        }
                        else if (distance > (powPowRange + QAddRange))
                        {
                            if (!W.IsReady() || !(wDamage > t.Health) || distance > W.Range ||
                                W.GetPrediction(t).CollisionObjects.Count > 0)
                            {
                                if (CountAlliesNearTarget(t, 500) <= 3)
                                {
                                    if (rDamage > t.Health /*&& !ObjectManager.Player.IsAutoAttacking &&
                                        !ObjectManager.Player.IsChanneling*/)
                                    {
                                        if (R.Cast(t) == Spell.CastStates.SuccessfullyCasted) { }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private int CountEnemies(Obj_AI_Base target, float range)
        {
            return
                ObjectManager.Get<Obj_AI_Hero>()
                    .Count(
                        hero =>
                            hero.IsValidTarget() && hero.Team != ObjectManager.Player.Team &&
                            hero.ServerPosition.Distance(target.ServerPosition) <= range);
        }

        private int CountAlliesNearTarget(Obj_AI_Base target, float range)
        {
            return
                ObjectManager.Get<Obj_AI_Hero>()
                    .Count(
                        hero =>
                            hero.Team == ObjectManager.Player.Team &&
                            hero.ServerPosition.Distance(target.ServerPosition) <= range);
        }

        private float GetRealPowPowRange(GameObject target)
        {
            return 525f + ObjectManager.Player.BoundingRadius + target.BoundingRadius;
        }

        private float GetRealDistance(GameObject target)
        {
            return ObjectManager.Player.Position.Distance(target.Position) + ObjectManager.Player.BoundingRadius +
                   target.BoundingRadius;
        }

        private float GetSlowEndTime(Obj_AI_Base target)
        {
            return
                target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => buff.Type == BuffType.Slow)
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault();
        }

        public float QAddRange
        {
            get { return 50 + 25 * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level; }
        }

        private bool FishBoneActive
        {
            get { return ObjectManager.Player.AttackRange > 565f; }
        }

        private int PowPowStacks
        {
            get
            {
                return
                    ObjectManager.Player.Buffs.Where(buff => buff.DisplayName.ToLower() == "jinxqramp")
                        .Select(buff => buff.Count)
                        .FirstOrDefault();
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("AutoCombo", "AutoCombo", true);
            config.AddBool("AutoE", "AutoE", true);
        }

    }
}

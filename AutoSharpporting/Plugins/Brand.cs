//Plugins Brand Take Combo Part From Hellsing Brand Credit to Hellsing

using System;
using System.Collections.Generic;
using System.Linq;
using AutoSharpporting.Util;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AutoSharpporting.Plugins
{
    public class Brand : PluginBase
    {
        private const int BOUNCE_RADIUS = 450;
        public readonly List<Spell> spellList = new List<Spell>();

        public Brand()
        {
            Q = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R, 750);

            spellList.AddRange(new[] {Q, W, E, R});
            Q.SetSkillshot(0.25f, 60, 1600, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1, 240, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetTargetted(0.25f, float.MaxValue);
            R.SetTargetted(0.25f, 1000);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                OnCombo(Target);
            }
        }

        private void OnCombo(Obj_AI_Hero target)
        {
            // Target validation
            if (target == null)
                return;

            // Spell usage
            var useQ = true;
            var useW = true;
            var useE = true;
            var useR = true;

            // Killable status
            var mainComboKillable = IsMainComboKillable(target);
            var bounceComboKillable = IsBounceComboKillable(target);
            var inMinimumRange = target.ServerPosition.Distance(Player.Position, true) < E.Range*E.Range;

            foreach (var spell in spellList)
            {
                // Continue if spell not ready
                if (!spell.IsReady())
                    continue;

                // Q
                if (spell.Slot == SpellSlot.Q && useQ)
                {
                    if ((mainComboKillable && inMinimumRange) || // Main combo killable
                        (!useW && !useE) || // Casting when not using W and E
                        (IsAblazed(target)) || // Ablazed
                        (Player.GetSpellDamage(target, SpellSlot.Q) > target.Health) || // Killable
                        (useW && !useE && !W.IsReady() &&
                         W.IsReady((int) (Player.Spellbook.GetSpell(SpellSlot.Q).Cooldown*1000))) ||
                        // Cooldown substraction W ready
                        ((useE && !useW || useW && useE) && !E.IsReady() &&
                         E.IsReady((int) (Player.Spellbook.GetSpell(SpellSlot.Q).Cooldown*1000))))
                        // Cooldown substraction E ready
                    {
                        // Cast Q on high hitchance
                        Q.CastIfHitchanceEquals(target, HitChance.High);
                    }
                }
                // W
                else if (spell.Slot == SpellSlot.W && useW)
                {
                    if ((mainComboKillable && inMinimumRange) || // Main combo killable
                        (!useE) || // Casting when not using E
                        (IsAblazed(target)) || // Ablazed
                        (Player.GetSpellDamage(target, SpellSlot.W) > target.Health) || // Killable
                        (target.ServerPosition.Distance(Player.Position, true) > E.Range*E.Range) ||
                        (!E.IsReady() && E.IsReady((int) (Player.Spellbook.GetSpell(SpellSlot.W).Cooldown*1000))))
                        // Cooldown substraction E ready
                    {
                        // Cast W on high hitchance
                        W.CastIfHitchanceEquals(target, HitChance.High);
                    }
                }
                // E
                else if (spell.Slot == SpellSlot.E && useE)
                {
                    // Distance check
                    if (Vector2.DistanceSquared(target.ServerPosition.To2D(), Player.Position.To2D()) < E.Range*E.Range)
                    {
                        if ((mainComboKillable) || // Main combo killable
                            (!useQ && !useW) || // Casting when not using Q and W
                            (E.Level >= 4) || // E level high, damage output higher
                            (useQ && (Q.IsReady() || Player.Spellbook.GetSpell(SpellSlot.Q).Cooldown < 5)) || // Q ready
                            (useW && W.IsReady())) // W ready
                        {
                            // Cast E on target
                            E.CastOnUnit(target);
                        }
                    }
                }
                // R
                else if (spell.Slot == SpellSlot.R && useR)
                {
                    // Distance check
                    if (target.ServerPosition.Distance(Player.Position, true) < R.Range*R.Range)
                    {
                        // Logic prechecks
                        if ((useQ && Q.IsReady() && Q.GetPrediction(target).Hitchance == HitChance.High ||
                             useW && W.IsReady()) && Player.Health/Player.MaxHealth > 0.4f)
                            continue;

                        // Single hit
                        if (mainComboKillable && inMinimumRange ||
                            Player.GetSpellDamage(target, SpellSlot.R) > target.Health)
                            R.CastOnUnit(target);
                        // Double bounce combo
                        else if (bounceComboKillable && inMinimumRange ||
                                 Player.GetSpellDamage(target, SpellSlot.R)*2 > target.Health)
                        {
                            if (
                                ObjectManager.Get<Obj_AI_Base>()
                                    .Count(
                                        enemy =>
                                            (enemy.Type == GameObjectType.obj_AI_Minion ||
                                             enemy.NetworkId != target.NetworkId &&
                                             enemy.Type == GameObjectType.obj_AI_Hero) && enemy.IsValidTarget() &&
                                            enemy.ServerPosition.Distance(target.ServerPosition, true) <
                                            BOUNCE_RADIUS*BOUNCE_RADIUS) > 0)
                                R.CastOnUnit(target);
                        }
                    }
                }
            }
        }

        public double GetMainComboDamage(Obj_AI_Base target)
        {
            var damage = Player.GetAutoAttackDamage(target);

            if (Q.IsReady())
                damage += Player.GetSpellDamage(target, SpellSlot.Q);

            if (W.IsReady())
                damage += Player.GetSpellDamage(target, SpellSlot.W)*(IsAblazed(target) ? 2 : 1);

            if (E.IsReady())
                damage += Player.GetSpellDamage(target, SpellSlot.E);

            if (R.IsReady())
                damage += Player.GetSpellDamage(target, SpellSlot.R);


            return damage;
        }

        public bool IsMainComboKillable(Obj_AI_Base target)
        {
            return GetMainComboDamage(target) > target.Health;
        }

        public double GetBounceComboDamage(Obj_AI_Base target)
        {
            var damage = GetMainComboDamage(target);

            if (R.IsReady())
                damage += Player.GetSpellDamage(target, SpellSlot.R);

            return damage;
        }

        public bool IsBounceComboKillable(Obj_AI_Base target)
        {
            return GetBounceComboDamage(target) > target.Health;
        }

        public bool IsAblazed(Obj_AI_Base target)
        {
            return target.HasBuff("brandablaze", true);
        }

        public bool IsSpellKillable(Obj_AI_Base target, SpellSlot spellSlot)
        {
            return Player.GetSpellDamage(target, spellSlot) > target.Health;
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
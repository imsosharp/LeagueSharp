using System;
using System.Collections.Generic;
using System.Linq;
using Support.Util;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;


namespace Support.Plugins
{
    public class Karthus : PluginBase
    {
        private const float SpellQWidth = 160f;
        private const float SpellWWidth = 160f;
        private bool _comboE;

        public Karthus()
        {
            Q = new Spell(SpellSlot.Q, 875);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 505);
            R = new Spell(SpellSlot.R, 20000f);

            Q.SetSkillshot(1f, 160, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(.5f, 70, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(1f, 505, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(3f, float.MaxValue, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            UltKs();
            if (ComboMode)
            {
                Combo();
            }
        }


        //from Beaving KarthusSharp
        private void Combo()
        {

            CastW(TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical), 20);

            if (E.IsReady() && !IsInPassiveForm())
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

                if (target != null)
                {
                    var enoughMana = GetManaPercent() >= 20;

                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1)
                    {
                        if (ObjectManager.Player.Distance(target.ServerPosition) <= E.Range && enoughMana)
                        {
                            _comboE = true;
                            E.Cast();
                        }
                    }
                    else if (!enoughMana)
                        RegulateEState(true);
                }
                else
                    RegulateEState();
            }

            if (Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                if (target != null)
                {
                    CastQ(target);
                }
            }
        }

        private void CastQ(Obj_AI_Base target, int minManaPercent = 0)
        {
            if (!Q.IsReady() || !(GetManaPercent() >= minManaPercent))
                return;
            if (target == null)
                return;
            Q.Width = GetDynamicQWidth(target);
            Q.Cast(target);
        }

        private void CastQ(Vector2 pos, int minManaPercent = 0)
        {
            if (!Q.IsReady())
                return;
            if (GetManaPercent() >= minManaPercent)
                Q.Cast(pos);
        }

        private bool IsInPassiveForm()
        {
            return ObjectManager.Player.IsZombie; //!ObjectManager.Player.IsHPBarRendered;
        }

        private float GetDynamicQWidth(Obj_AI_Base target)
        {
            return Math.Max(30, (1f - (Player.Distance(target) / Q.Range)) * SpellQWidth);
        }

        private void RegulateEState(bool ignoreTargetChecks = false)
        {
            if (!E.IsReady() || IsInPassiveForm() ||
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState != 2)
                return;
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (!ignoreTargetChecks && (target != null || (!_comboE && minions.Count != 0)))
                return;
            E.CastOnUnit(ObjectManager.Player);
            _comboE = false;
        }

        private void CastW(Obj_AI_Base target, int minManaPercent = 0)
        {
            if (!W.IsReady() || !(GetManaPercent() >= minManaPercent))
                return;
            if (target == null)
                return;
            W.Width = GetDynamicWWidth(target);
            W.Cast(target);
        }

        public float GetManaPercent()
        {
            return (ObjectManager.Player.Mana / ObjectManager.Player.MaxMana) * 100f;
        }

        private float GetDynamicWWidth(Obj_AI_Base target)
        {
            return Math.Max(70, (1f - (Player.Distance(target) / W.Range)) * SpellWWidth);
        }

        public void UltKs()
        {
            foreach (
                var target in
                    ObjectManager.Get<Obj_AI_Hero>().Where(x => Player.Distance(x) < R.Range && x.IsEnemy && !x.IsDead))
            {
                if (R.IsReady() && Player.Distance(target) <= R.Range && R.IsKillable(target) && (Player.IsZombie || Player.CountEnemiesInRange(1000) < 1))
                {
                    R.Cast();
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
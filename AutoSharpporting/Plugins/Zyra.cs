#region LICENSE

// Copyright 2014-2015 Support
// Zyra.cs is part of Support.
// 
// Support is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Support is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Support. If not, see <http://www.gnu.org/licenses/>.
// 
// Filename: Support/Support/Zyra.cs
// Created:  25/10/2014
// Date:     20/01/2015/11:20
// Author:   h3h3

#endregion

namespace Support.Plugins
{
    #region

    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Support.Util;
    using ActiveGapcloser = Support.Util.ActiveGapcloser;

    #endregion

    public class Zyra : PluginBase
    {
        public Zyra()
        {
            Q = new Spell(SpellSlot.Q, 800);
            W = new Spell(SpellSlot.W, 825);
            E = new Spell(SpellSlot.E, 1100);
            R = new Spell(SpellSlot.R, 700);
            Passive = new Spell(SpellSlot.Q, 1470);

            Q.SetSkillshot(0.8f, 60f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.5f, 70f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 500f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Passive.SetSkillshot(0.5f, 70f, 1400f, false, SkillshotType.SkillshotLine);
        }

        private Spell Passive { get; set; }

        private int WCount
        {
            get { return W.Instance.Level > 0 ? W.Instance.Ammo : 0; }
        }

        private void CastW(Vector3 v)
        {
            if (!W.IsReady())
            {
                return;
            }

            if (WCount == 1)
            {
                Utility.DelayAction.Add(50, () => W.Cast(new Vector2(v.X - 5, v.Y - 5)));
            }

            if (WCount == 2)
            {
                Utility.DelayAction.Add(50, () => W.Cast(new Vector2(v.X - 5, v.Y - 5)));
                Utility.DelayAction.Add(180, () => W.Cast(new Vector2(v.X - 5, v.Y - 5)));
            }
        }

        public override void OnUpdate(EventArgs args)
        {
            try
            {
                if (ZyraisZombie())
                {
                    CastPassive();
                    return;
                }

                if (ComboMode)
                {
                    if (Q.CastCheck(Target, "Combo.Q"))
                    {
                        if (Q.Cast(Target) == Spell.CastStates.SuccessfullyCasted)
                        {
                            CastW(Q.GetPrediction(Target).CastPosition);
                        }
                    }

                    if (E.CastCheck(Target, "Combo.E"))
                    {
                        if (E.Cast(Target) == Spell.CastStates.SuccessfullyCasted)
                        {
                            CastW(E.GetPrediction(Target).CastPosition);
                        }
                    }

                    if (R.CastCheck(Target, "Combo.R"))
                    {
                        R.CastIfWillHit(Target, ConfigValue<Slider>("Combo.R.Count").Value);
                    }
                }

                if (HarassMode)
                {
                    if (Q.CastCheck(Target, "Harass.Q"))
                    {
                        if (Q.Cast(Target) == Spell.CastStates.SuccessfullyCasted)
                        {
                            CastW(Q.GetPrediction(Target).CastPosition);
                        }
                    }

                    if (E.CastCheck(Target, "Harass.E"))
                    {
                        if (E.Cast(Target) == Spell.CastStates.SuccessfullyCasted)
                        {
                            CastW(E.GetPrediction(Target).CastPosition);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.CastCheck(gapcloser.Sender, "Gapcloser.E"))
            {
                if (E.Cast(Target) == Spell.CastStates.SuccessfullyCasted)
                {
                    CastW(E.GetPrediction(Target).CastPosition);
                }
            }
        }

        public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (R.CastCheck(unit, "Interrupt.R"))
            {
                R.Cast(unit);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("Combo.Q", "Use Q", true);
            config.AddBool("Combo.E", "Use E", true);
            config.AddBool("Combo.R", "Use R", true);
            config.AddSlider("Combo.R.Count", "Targets hit to Ult", 3, 0, 5);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("Harass.Q", "Use Q", true);
            config.AddBool("Harass.E", "Use E", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Gapcloser.E", "Use E to Interrupt Gapcloser", true);
            config.AddBool("Interrupt.R", "Use R to Interrupt Spells", true);
        }

        #region UltimateCarry2 https://github.com/LXMedia1/UltimateCarry2/blob/master/LexxersAIOCarry/Zyra.cs

        private bool ZyraisZombie()
        {
            return Player.Spellbook.GetSpell(SpellSlot.Q).Name == Player.Spellbook.GetSpell(SpellSlot.E).Name ||
                   Player.Spellbook.GetSpell(SpellSlot.W).Name == Player.Spellbook.GetSpell(SpellSlot.R).Name;
        }

        private void CastPassive()
        {
            if (!Passive.IsReady())
            {
                return;
            }
            if (!Target.IsValidTarget(E.Range))
            {
                return;
            }
            Passive.CastIfHitchanceEquals(Target, HitChance.High);
        }

        #endregion
    }
}
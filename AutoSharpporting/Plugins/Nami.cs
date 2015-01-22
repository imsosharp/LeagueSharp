#region LICENSE

// Copyright 2014-2015 Support
// Nami.cs is part of Support.
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
// Filename: Support/Support/Nami.cs
// Created:  01/10/2014
// Date:     20/01/2015/11:20
// Author:   h3h3

#endregion

namespace Support.Plugins
{
    #region

    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Support.Util;
    using ActiveGapcloser = Support.Util.ActiveGapcloser;

    #endregion

    public class Nami : PluginBase
    {
        public Nami()
        {
            Q = new Spell(SpellSlot.Q, 875);
            W = new Spell(SpellSlot.W, 725);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 2200);

            Q.SetSkillshot(1f, 150f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.5f, 260f, 850f, false, SkillshotType.SkillshotLine);
            GameObject.OnCreate += RangeAttackOnCreate;
        }

        private double WHeal
        {
            get
            {
                int[] heal = { 0, 65, 95, 125, 155, 185 };
                return heal[W.Level] + Player.FlatMagicDamageMod * 0.3;
            }
        }

        private void RangeAttackOnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<Obj_SpellMissile>())
            {
                return;
            }

            var missile = (Obj_SpellMissile) sender;

            // Caster ally hero / not me
            if (!missile.SpellCaster.IsValid<Obj_AI_Hero>() || !missile.SpellCaster.IsAlly || missile.SpellCaster.IsMe ||
                missile.SpellCaster.IsMelee())
            {
                return;
            }

            // Target enemy hero
            if (!missile.Target.IsValid<Obj_AI_Hero>() || !missile.Target.IsEnemy)
            {
                return;
            }

            var caster = (Obj_AI_Hero) missile.SpellCaster;

            if (E.IsReady() && E.IsInRange(missile.SpellCaster) && ConfigValue<bool>("Misc.E.AA." + caster.ChampionName))
            {
                E.CastOnUnit(caster); // add delay
            }
        }

        public override void OnUpdate(EventArgs args)
        {
            try
            {
                if (ComboMode)
                {
                    if (Q.CastCheck(Target, "ComboQ")) // TODO: add check for slowed targets by E or FrostQeen
                    {
                        Q.Cast(Target);
                    }

                    if (W.IsReady() && ConfigValue<bool>("ComboW"))
                    {
                        HealLogic();
                    }

                    if (R.CastCheck(Target, "ComboR"))
                    {
                        R.CastIfWillHit(Target, ConfigValue<Slider>("ComboCountR").Value);
                    }
                }

                if (HarassMode)
                {
                    if (Q.CastCheck(Target, "HarassQ"))
                    {
                        Q.Cast(Target);
                    }

                    if (W.IsReady() && ConfigValue<bool>("HarassW"))
                    {
                        HealLogic();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void HealLogic()
        {
            var ally = Helpers.AllyBelowHp(ConfigValue<Slider>("ComboHealthW").Value, W.Range);
            if (ally != null) // force heal low ally
            {
                W.CastOnUnit(ally);
                return;
            }

            if (Player.Distance(Target) > W.Range) // target out of range try bounce
            {
                var bounceTarget =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .SingleOrDefault(hero => hero.IsValidAlly(W.Range) && hero.Distance(Target) < W.Range);

                if (bounceTarget != null && bounceTarget.MaxHealth - bounceTarget.Health > WHeal) // use bounce & heal
                {
                    W.CastOnUnit(bounceTarget);
                }
            }
            else // target in range
            {
                W.CastOnUnit(Target);
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
            {
                return;
            }

            if (Q.CastCheck(gapcloser.Sender, "GapcloserQ"))
            {
                Q.Cast(gapcloser.Sender);
            }

            if (R.CastCheck(gapcloser.Sender, "GapcloserR"))
            {
                R.Cast(gapcloser.Sender);
            }
        }

        public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (Q.CastCheck(unit, "InterruptQ"))
            {
                Q.Cast(unit);
            }

            if (!Q.IsReady() && R.CastCheck(unit, "InterruptR"))
            {
                R.Cast(unit);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboR", "Use R", true);
            config.AddSlider("ComboCountR", "Targets hit to Ult", 2, 1, 5);
            config.AddSlider("ComboHealthW", "Health to Heal", 20, 1, 100);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassQ", "Use Q", true);
            config.AddBool("HarassW", "Use W", true);
            config.AddSlider("HarassHealthW", "Health to Heal", 20, 1, 100);
        }

        public override void MiscMenu(Menu config)
        {
            var sub = config.AddSubMenu(new Menu("Use E on Attacks", "Misc.E.AA.Menu"));
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsAlly && !h.IsMe))
            {
                sub.AddBool("Misc.E.AA." + hero.ChampionName, hero.ChampionName, true);
            }
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("GapcloserQ", "Use Q to Interrupt Gapcloser", true);

            config.AddBool("InterruptQ", "Use Q to Interrupt Spells", true);
            config.AddBool("InterruptR", "Use R to Interrupt Spells", true);
        }
    }
}
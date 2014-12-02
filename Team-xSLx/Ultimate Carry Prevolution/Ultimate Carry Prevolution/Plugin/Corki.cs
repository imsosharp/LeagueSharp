using System;
using System.Collections.Generic;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using xSLx_Orbwalker;

namespace Ultimate_Carry_Prevolution.Plugin
{
    internal class Corki : Champion
    {
        public Corki()
        {
            SetSpells();
            LoadMenu();
        }

        private void SetSpells()
        {
            Q = new Spell(SpellSlot.Q, 825);
            Q.SetSkillshot(300, 250, 1225, false, SkillshotType.SkillshotCircle);

            W = new Spell(SpellSlot.W, 800);

            E = new Spell(SpellSlot.E, 600);
            E.SetSkillshot(10, (float) (45*Math.PI/180), 1500, false, SkillshotType.SkillshotCone);

            R = new Spell(SpellSlot.R, 1500);
            R.SetSkillshot(0, 40, 1500, true, SkillshotType.SkillshotLine);
        }

        private void LoadMenu()
        {
            var champMenu = new Menu("Corki Plugin", "Corki");
            {
                var comboMenu = new Menu("Combo", "Combo");
                {
                    AddSpelltoMenu(comboMenu, "Q", true);
                    AddSpelltoMenu(comboMenu, "E", true);
                    AddSpelltoMenu(comboMenu, "R", true);
                    comboMenu.AddItem(new MenuItem("ComboR_Limit", "Save R Stack").SetValue(new Slider(0, 0, 7)));
                    champMenu.AddSubMenu(comboMenu);
                }

                var harassMenu = new Menu("Harass", "Harass");
                {
                    AddSpelltoMenu(harassMenu, "Q", true);
                    AddSpelltoMenu(harassMenu, "E", true);
                    AddSpelltoMenu(harassMenu, "R", true);
                    harassMenu.AddItem(new MenuItem("HarassR_Limit", "Save R Stack").SetValue(new Slider(5, 0, 7)));
                    AddManaManagertoMenu(harassMenu, 30);
                    champMenu.AddSubMenu(harassMenu);
                }
                var laneClearMenu = new Menu("LaneClear", "LaneClear");
                {
                    AddSpelltoMenu(laneClearMenu, "Q", true);
                    AddSpelltoMenu(laneClearMenu, "E", true);
                    AddSpelltoMenu(laneClearMenu, "R", true);
                    laneClearMenu.AddItem(new MenuItem("LaneClearR_Limit", "Save R Stack").SetValue(new Slider(5, 0, 7)));
                    AddManaManagertoMenu(laneClearMenu, 0);
                    champMenu.AddSubMenu(laneClearMenu);
                }

                var miscMenu = new Menu("Misc", "Misc");
                {
                    miscMenu.AddItem(new MenuItem("Misc_W_To_Mouse", "W To Mouse")).SetValue(new KeyBind("W".ToCharArray()[0], KeyBindType.Press));
                    champMenu.AddSubMenu(miscMenu);
                }

                var drawMenu = new Menu("Drawing", "Drawing");
                {
                    drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
                    drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
                    drawMenu.AddItem(new MenuItem("Draw_W", "Draw W").SetValue(true));
                    drawMenu.AddItem(new MenuItem("Draw_E", "Draw E").SetValue(true));
                    drawMenu.AddItem(new MenuItem("Draw_R", "Draw R").SetValue(true));

                    MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage").SetValue(true);
                    drawMenu.AddItem(drawComboDamageMenu);
                    Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
                    Utility.HpBarDamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                    drawComboDamageMenu.ValueChanged +=
                        delegate(object sender, OnValueChangeEventArgs eventArgs)
                        {
                            Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                        };

                    champMenu.AddSubMenu(drawMenu);
                }
            }

            Menu.AddSubMenu(champMenu);
            Menu.AddToMainMenu();
        }

        private IEnumerable<SpellSlot> GetSpellCombo()
        {
            var spellCombo = new List<SpellSlot>();
            if (Q.IsReady())
                spellCombo.Add(SpellSlot.Q);
            if (E.IsReady())
                spellCombo.Add(SpellSlot.E);
            if (R.IsReady())
                spellCombo.Add(SpellSlot.R);
            return spellCombo;
        }

        private float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = (float) ObjectManager.Player.GetComboDamage(target, GetSpellCombo());
            return (float) (comboDamage + ObjectManager.Player.GetAutoAttackDamage(target));
        }

        
        public override void OnDraw()
        {
            if (Menu.Item("Draw_Disabled").GetValue<bool>())
            {
                xSLxOrbwalker.DisableDrawing();
                return;
            }
            xSLxOrbwalker.EnableDrawing();

            if (Menu.Item("Draw_Q").GetValue<bool>())
                if (Q.Level > 0)
                    Utility.DrawCircle(MyHero.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_W").GetValue<bool>())
                if (W.Level > 0)
                    Utility.DrawCircle(MyHero.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_E").GetValue<bool>())
                if (E.Level > 0)
                    Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_R").GetValue<bool>())
                if (R.Level > 0)
                    Utility.DrawCircle(MyHero.Position, MyHero.HasBuff("CorkiMissileBarrageCounterBig") ? R.Range : 1300, R.IsReady() ? Color.Green : Color.Red);
        }

        public override void OnPassive()
        {
            if (Menu.Item("Misc_W_To_Mouse").GetValue<KeyBind>().Active)
                W.Cast(Game.CursorPos);
        }

        public override void OnCombo()
        {
            if (IsSpellActive("R"))
                Cast_R(1);
            if (IsSpellActive("Q"))
                Cast_BasicSkillshot_Enemy(Q, SimpleTs.DamageType.Magical);
            if (IsSpellActive("E"))
                Cast_BasicSkillshot_Enemy(E, SimpleTs.DamageType.Physical);
        }

        public override void OnHarass()
        {
            if (ManaManagerAllowCast())
            {
                if (IsSpellActive("Q"))
                    Cast_BasicSkillshot_Enemy(Q, SimpleTs.DamageType.Magical);
                if (IsSpellActive("E"))
                    Cast_BasicSkillshot_Enemy(E, SimpleTs.DamageType.Physical);
                if (IsSpellActive("R"))
                    Cast_R(2);
            }
        }
        
        public override void OnLaneClear()
        {
            if (ManaManagerAllowCast())
            {
                if (IsSpellActive("Q"))
                    Cast_BasicSkillshot_AOE_Farm(Q);
                if (IsSpellActive("E"))
                    Cast_BasicSkillshot_AOE_Farm(E);
                if (IsSpellActive("R"))
                    Cast_R(3);
            }
        }

        private void Cast_R(int mode)
        {
	        var range = MyHero.HasBuff("CorkiMissileBarrageCounterBig") ? 1500 : 1300;
            var target = SimpleTs.GetTarget(range, SimpleTs.DamageType.Magical);
            if (target != null)
            {
                R.UpdateSourcePosition();

                if (mode == 1 &&
                    Menu.Item("ComboR_Limit").GetValue<Slider>().Value <
                    ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo)
                {
                    if (R.GetPrediction(target).Hitchance >= HitChance.Medium)
                        R.Cast(target, UsePackets());
                }
                else if (mode == 2 &&
                         Menu.Item("HarassR_Limit").GetValue<Slider>().Value <
                         ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo)
                {
                    if (R.GetPrediction(target).Hitchance >= HitChance.Medium)
                        R.Cast(target, UsePackets());
                }
                else if (mode == 3 &&
                         Menu.Item("LaneClearR_Limit").GetValue<Slider>().Value <
                         ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo) 
                    Cast_BasicSkillshot_AOE_Farm(R, 1300);
            }
        }
    }
}
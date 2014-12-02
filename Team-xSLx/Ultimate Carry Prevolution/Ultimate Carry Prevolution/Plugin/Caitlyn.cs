using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
    class Caitlyn : Champion
    {
        public Caitlyn()
        {
            SetSpells();
            LoadMenu();
        }

        private void SetSpells()
        {
            Q = new Spell(SpellSlot.Q, 1150);
            Q.SetSkillshot(750, 60, 1800, false, SkillshotType.SkillshotLine);
            
            W = new Spell(SpellSlot.W, 800);

            E = new Spell(SpellSlot.E, 850);
            E.SetSkillshot(250, 80, 1600, true, SkillshotType.SkillshotLine);

            R = new Spell(SpellSlot.R, 2000);
        }

        private void LoadMenu()
        {
            var champMenu = new Menu("Caitlyn Plugin", "Caitlyn");
            {
                var comboMenu = new Menu("Combo", "Combo");
                {
                    AddSpelltoMenu(comboMenu, "Q", true);
                    AddSpelltoMenu(comboMenu, "W", true);
                    AddSpelltoMenu(comboMenu, "W_StunCombo", true, "W On Stun Enemy");
                    AddSpelltoMenu(comboMenu, "W_SlowCombo", true, "W On Slow Enemy");
                    AddSpelltoMenu(comboMenu, "E", true);
                    comboMenu.AddItem(new MenuItem("R_Nearest_Killable", "R Nearest Killable").SetValue(new KeyBind("R".ToCharArray()[0], KeyBindType.Press)));
                    champMenu.AddSubMenu(comboMenu);
                }

                var harassMenu = new Menu("Harass", "Harass");
                {
                    AddSpelltoMenu(harassMenu, "Q", true);
                    AddSpelltoMenu(harassMenu, "E", true);
                    AddManaManagertoMenu(harassMenu, 30);
                    champMenu.AddSubMenu(harassMenu);
                }

                var laneClearMenu = new Menu("LaneClear", "LaneClear");
                {
                    AddSpelltoMenu(laneClearMenu, "Q", true);
                    AddManaManagertoMenu(laneClearMenu, 0);
                    champMenu.AddSubMenu(laneClearMenu);
                }

				var fleeMenu = new Menu("Flee", "Flee");
				{
					AddSpelltoMenu(fleeMenu, "E", true, "Use E to Mouse");
					champMenu.AddSubMenu(fleeMenu);
				}

                var miscMenu = new Menu("Misc", "Misc");
                {
                    miscMenu.AddItem(new MenuItem("Cast_EQ", "Cast EQ nearest target").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                    miscMenu.AddItem(new MenuItem("W_Stun", "Auto W On Stun Enemy").SetValue(true));
                    miscMenu.AddItem(new MenuItem("W_Slow", "Auto W On Slow Enemy").SetValue(true));
                    miscMenu.AddItem(new MenuItem("E_Gap_Closer", "Use E On Gap Closer").SetValue(true));
                    champMenu.AddSubMenu(miscMenu);
                }

                var drawMenu = new Menu("Drawing", "Drawing");
                {
                    drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
                    drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
                    drawMenu.AddItem(new MenuItem("Draw_W", "Draw W").SetValue(true));
                    drawMenu.AddItem(new MenuItem("Draw_E", "Draw E").SetValue(true));
                    drawMenu.AddItem(new MenuItem("Draw_R", "Draw R").SetValue(true));
                    drawMenu.AddItem(new MenuItem("Draw_R_Killable", "Draw R Mark on Killable").SetValue(true));

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
			double comboDamage = (float)MyHero.GetComboDamage(target, GetSpellCombo());
			return (float)(comboDamage + MyHero.GetAutoAttackDamage(target) * 2);
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
                    Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_R_Killable").GetValue<bool>() && R.IsReady())
            {
                foreach (var unit in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(R.Range) && !x.IsDead && x.IsEnemy).OrderBy(x => x.Health))
                {
                    var health = unit.Health + unit.HPRegenRate + 10;
                    if (ObjectManager.Player.GetSpellDamage(unit, SpellSlot.R) > health)
                    {
                        Vector2 wts = Drawing.WorldToScreen(unit.Position);
                        Drawing.DrawText(wts[0] - 20, wts[1], Color.White, "KILL!!!");
                    }
                }
            }
        }

        public override void OnPassive()
        {
            //dynamic r range
            if(R.IsReady() && R.Level > 0)
                R.Range = 500 * R.Level + 1500;

            if (Menu.Item("R_Nearest_Killable").GetValue<KeyBind>().Active)
                Cast_R();

            if (Menu.Item("Cast_EQ").GetValue<KeyBind>().Active)
                Cast_EQ();

            if(W.IsReady())
                Cast_W(Menu.Item("W_Stun").GetValue<bool>(), Menu.Item("W_Slow").GetValue<bool>());
        }

        public override void OnCombo()
        {
            if (IsSpellActive("Q"))
                Cast_BasicSkillshot_Enemy(Q, SimpleTs.DamageType.Physical);
            if (IsSpellActive("W"))
                Cast_W(IsSpellActive("W_StunCombo"), IsSpellActive("W_SlowCombo"));
        }

        public override void OnHarass()
        {
            if (ManaManagerAllowCast())
            {
                if (IsSpellActive("Q"))
                    Cast_BasicSkillshot_Enemy(Q, SimpleTs.DamageType.Physical);
            }
        }
        
        public override void OnLaneClear()
        {
            if (IsSpellActive("Q") && ManaManagerAllowCast())
                Cast_BasicSkillshot_AOE_Farm(Q);
        }

        public override void OnFlee()
        {
            if(IsSpellActive("E"))
                Cast_Inverted_E(Game.CursorPos);
        }


		public override void OnGapClose(ActiveGapcloser gapcloser)
		{
			if (!Menu.Item("E_Gap_Closer").GetValue<bool>()) return;

		    if (MyHero.Distance(gapcloser.End) < MyHero.Distance(gapcloser.Start))
		    {
		        if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
		            E.Cast(gapcloser.Sender, UsePackets());
		    }
		}

        private void Cast_Inverted_E(Vector3 position)
        {
            if (!E.IsReady())
                return;

            var pos = ObjectManager.Player.ServerPosition.To2D().Extend(position.To2D(), -300).To3D();

            E.Cast(pos, UsePackets());
        }

        private void Cast_EQ()
        {
            if (!Q.IsReady() || !E.IsReady())
                return;

            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);

            E.Cast(target, UsePackets());
            Q.Cast(target, UsePackets());
        }

        private void Cast_R()
        {
            foreach (var unit in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(R.Range) && !x.IsDead && x.IsEnemy).OrderBy(x => x.Health))
            {
                var health = unit.Health + unit.HPRegenRate + 25;
                if (ObjectManager.Player.GetSpellDamage(unit, SpellSlot.R) > health)
                {
                    R.CastOnUnit(unit, UsePackets());
                    return;
                }
            }
        }

        private void Cast_W(bool stun, bool slow)
        {
            if ((!stun && !slow) || !W.IsReady())
                return;

            foreach (var unit in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(W.Range) && !x.IsDead && x.IsEnemy))
            {
                if (stun && W.GetPrediction(unit).Hitchance == HitChance.Immobile && MyHero.Distance(unit) < W.Range)
                {
                    W.Cast(unit.ServerPosition);
                    return;
                }

                if (slow && unit.HasBuffOfType(BuffType.Slow) && MyHero.Distance(unit) < W.Range)
                {
                    W.Cast(unit.ServerPosition);
                    return;
                }
            }
        }
    }
}

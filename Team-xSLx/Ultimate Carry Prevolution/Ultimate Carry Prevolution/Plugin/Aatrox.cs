using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
	class Aatrox : Champion
	{
		public Aatrox()
		{
			SetSpells();
			LoadMenu();
		}

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q, 650);
			Q.SetSkillshot(270, 280, 1800, false, SkillshotType.SkillshotCircle);

			W = new Spell(SpellSlot.W);

			E = new Spell(SpellSlot.E, 1000);
			E.SetSkillshot(270, 80, 1200, false, SkillshotType.SkillshotLine);

			R = new Spell(SpellSlot.R, 300);			
		}

		private void LoadMenu()
		{
			var champMenu = new Menu("Aatrox Plugin", "Aatrox");
			{
				var comboMenu = new Menu("Combo", "Combo");
				{
					AddSpelltoMenu(comboMenu, "Q", true);
					AddSpelltoMenu(comboMenu, "E", true);
					comboMenu.AddItem(new MenuItem("Combo_useR_onAmount", "R on Enemys in Range").SetValue(new Slider(2, 5, 0)));
					comboMenu.AddItem(new MenuItem("Combo_useR_onEnemyHealth", "R on Enemy %Health <").SetValue(new Slider(60, 100, 0)));
					champMenu.AddSubMenu(comboMenu);
				}
				var harassMenu = new Menu("Harass", "Harass");
				{
					AddSpelltoMenu(harassMenu, "Q", true);
					AddSpelltoMenu(harassMenu, "Q_Danger", true,"Q inside Dangerzone");
					AddSpelltoMenu(harassMenu, "E", true);
					champMenu.AddSubMenu(harassMenu);
				}
				var laneClearMenu = new Menu("LaneClear", "LaneClear");
				{
					AddSpelltoMenu(laneClearMenu, "Q", true);
					AddSpelltoMenu(laneClearMenu, "E", true);
					champMenu.AddSubMenu(laneClearMenu);
				}
				var fleeMenu = new Menu("Flee", "Flee");
				{
					AddSpelltoMenu(fleeMenu, "Q", true, "Use Q to Mouse");
					AddSpelltoMenu(fleeMenu, "E", true, "Use E to slow Enemy");
					champMenu.AddSubMenu(fleeMenu);
				}
				var miscMenu = new Menu("Misc", "Misc");
				{
					miscMenu.AddItem(new MenuItem("Misc_useW_Autoswitch", "Switch W Automatic").SetValue(true));
					miscMenu.AddItem(new MenuItem("Misc_useW_Autoswitch_health", "Use E to slow Enemy").SetValue(new Slider(60, 100, 0)));
					miscMenu.AddItem(new MenuItem("Misc_useW_Autoswitch_priorityhealth", "Heal Priority farming").SetValue(true));
					champMenu.AddSubMenu(miscMenu);
				}
				var drawMenu = new Menu("Drawing", "Drawing");
				{
					drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
					drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_E", "Draw E").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_R", "Draw R").SetValue(true));
					
					var drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage").SetValue(true);			
					drawMenu.AddItem(drawComboDamageMenu);
						Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
					Utility.HpBarDamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
					drawComboDamageMenu.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
					{
						Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
					};
					
					champMenu.AddSubMenu(drawMenu);
				}
			}

			Menu.AddSubMenu(champMenu);
			Menu.AddToMainMenu();

		}

		IEnumerable<SpellSlot> GetSpellCombo()
		{
			var spellCombo = new List<SpellSlot>();
			if(Q.IsReady())
				spellCombo.Add(SpellSlot.Q);
			if(E.IsReady())
				spellCombo.Add(SpellSlot.E);
			if(R.IsReady())
				spellCombo.Add(SpellSlot.R);
			return spellCombo;
		}

		float GetComboDamage(Obj_AI_Base target)
		{
			double comboDamage = (float)ObjectManager.Player.GetComboDamage(target, GetSpellCombo());
			return (float)(comboDamage + (MyHero.GetAutoAttackDamage(target) * (Q.IsReady() ? 2 : 1)));
		}

		public override void OnDraw()
		{
			if(Menu.Item("Draw_Disabled").GetValue<bool>())
			{
				xSLxOrbwalker.DisableDrawing();
				return;
			}
			xSLxOrbwalker.EnableDrawing();

			if(Menu.Item("Draw_Q").GetValue<bool>())
				if(Q.Level > 0)
					Utility.DrawCircle(MyHero.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_E").GetValue<bool>())
				if(E.Level > 0)
					Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_R").GetValue<bool>())
				if(R.Level > 0)
					Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);
		}

		public override void OnPassive()
		{
			AutomaticW();
		}

		public override void OnCombo()
		{
			if(IsSpellActive("E"))
				Cast_E();
			if(IsSpellActive("Q"))
				Cast_Q();
			Cast_R();
		}

		public override void OnHarass()
		{
			if(IsSpellActive("E"))
				Cast_E();
			if(IsSpellActive("Q"))
				Cast_Q();
		}

		public override void OnLaneClear()
		{
			if(IsSpellActive("E"))
				Cast_BasicSkillshot_AOE_Farm(Q);
			if(IsSpellActive("Q"))
				Cast_BasicSkillshot_AOE_Farm(E);
		}

		public override void OnFlee()
		{
			if(IsSpellActive("E"))
				Cast_E();
			if(IsSpellActive("Q"))
				if(Game.CursorPos.Distance(MyHero.Position) > Q.Range && Q.IsReady())
				{
					Q.UpdateSourcePosition();
					Q.Cast(Game.CursorPos, UsePackets());
				}
		}

		private void AutomaticW()
		{
			if(!W.IsReady())
				return;
			if(!Menu.Item("Misc_useW_Autoswitch").GetValue<bool>())
				return;
			if(xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Combo)
			{
				if(MyHero.Health / MyHero.MaxHealth * 100 <
					Menu.Item("Misc_useW_Autoswitch_health").GetValue<Slider>().Value)
				{
					WtoHeal();
					return;
				}
				WtoDamage();
				return;
			}
			if(Menu.Item("Misc_useW_Autoswitch_priorityhealth").GetValue<bool>())
			{
				if(MyHero.Health / MyHero.MaxHealth * 100 < 95)
					WtoHeal();
				else
					WtoDamage();
			}
			else
			{
				if(MyHero.Health / MyHero.MaxHealth * 100 <
					Menu.Item("Misc_useW_Autoswitch_health").GetValue<Slider>().Value)
					WtoHeal();
				else
					WtoDamage();
			}
		}

		private void WtoDamage()
		{
			if(GetSpellName(SpellSlot.W) == "aatroxw2" || !W.IsReady())
				return;
			W.Cast();
		}

		private void WtoHeal()
		{
			if(GetSpellName(SpellSlot.W) == "AatroxW" || !W.IsReady())
				return;
			W.Cast();
		}

		private void Cast_Q()
		{
			var target = SimpleTs.GetTarget(Q.Range + Q.Width / 2,SimpleTs.DamageType.Physical);
			switch(xSLxOrbwalker.CurrentMode)
			{
				case xSLxOrbwalker.Mode.Combo:
					if(target != null && !xSLxOrbwalker.InAutoAttackRange(target ))
						Cast_BasicSkillshot_Enemy(Q, SimpleTs.DamageType.Physical);
					break;
				case xSLxOrbwalker.Mode.Harass:
					if(target != null && !xSLxOrbwalker.InAutoAttackRange(target))
						if(!IsSpellActive("Q_Danger"))
						{
							if(!IsInsideEnemyTower(Q.GetPrediction(target).CastPosition))
								Cast_BasicSkillshot_Enemy(Q, SimpleTs.DamageType.Physical);
						}
						else
							Cast_BasicSkillshot_Enemy(Q, SimpleTs.DamageType.Physical);

					break;
			}
		}

		private void Cast_E()
		{
			var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
			if(target != null)
				Cast_BasicSkillshot_Enemy(E,SimpleTs.DamageType.Physical);
		}

		private void Cast_R()
		{
			if(!R.IsReady())
				return;
			if(Menu.Item("Combo_useR_onAmount").GetValue<Slider>().Value > 0)
				if(Utility.CountEnemysInRange((int)R.Range) >= Menu.Item("Combo_useR_onAmount").GetValue<Slider>().Value)
				{
					R.Cast();
					return;
				}
			if(Menu.Item("Combo_useR_onEnemyHealth").GetValue<Slider>().Value <= 0)
				return;
			if(!AllHerosEnemy.Any(hero => hero.IsValidTarget(R.Range) &&
											  hero.Health / hero.MaxHealth * 100 <
											  Menu.Item("Combo_useR_onEnemyHealth").GetValue<Slider>().Value))
				return;
			R.Cast();
		}

		
	}
}

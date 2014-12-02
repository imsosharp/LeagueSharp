using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using xSLx_Orbwalker;

namespace Ultimate_Carry_Prevolution.Plugin
{
	class Ashe:Champion
	{
		private bool _qActive;
		private int _qCheckTimer;

		public Ashe()
		{
			SetSpells();
			LoadMenu();
		}

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q);

			W = new Spell(SpellSlot.W, 1200);
			W.SetSkillshot(250, (float)(50 * Math.PI / 180), 900, true, SkillshotType.SkillshotCone);
			
			E = new Spell(SpellSlot.E, 2500);
			E.SetSkillshot(350, 300, 1400, false, SkillshotType.SkillshotLine);

			R = new Spell(SpellSlot.R, 20000);
			R.SetSkillshot(250, 130f, 1600, false, SkillshotType.SkillshotLine);
		}

		private void LoadMenu()
		{
			var champMenu = new Menu("Ashe Plugin", "Ashe");
			{
				var comboMenu = new Menu("Combo", "Combo");
				{
					AddSpelltoMenu(comboMenu, "Q", true);
					AddSpelltoMenu(comboMenu, "W", true);
					AddSpelltoMenu(comboMenu, "E", true);
					comboMenu.AddItem(new MenuItem("Combo_useR_onKillHelp", "Use R to Safe Kill").SetValue(true));
					comboMenu.AddItem(new MenuItem("Combo_useR_onoutofRange", "Use R KS if Out of Range").SetValue(true));				
					champMenu.AddSubMenu(comboMenu);
				}
				var harassMenu = new Menu("Harass", "Harass");
				{
					AddSpelltoMenu(harassMenu, "Q", true);
					AddSpelltoMenu(harassMenu, "W", true);
					AddManaManagertoMenu(harassMenu, 30);
					champMenu.AddSubMenu(harassMenu);
				}
				var laneClearMenu = new Menu("LaneClear", "LaneClear");
				{
					AddSpelltoMenu(laneClearMenu, "W", true);
					AddManaManagertoMenu(laneClearMenu, 20);
					champMenu.AddSubMenu(laneClearMenu);
				}
				var miscMenu = new Menu("Misc", "Misc");
				{
					miscMenu.AddItem(new MenuItem("Misc_useR_Interrupt", "Use R interrupt").SetValue(true));
					miscMenu.AddItem(new MenuItem("Misc_useE_Flash", "Use E on Enemy Flash").SetValue(true));
					champMenu.AddSubMenu(miscMenu);
				}
				var drawMenu = new Menu("Drawing", "Drawing");
				{
					drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
					drawMenu.AddItem(new MenuItem("Draw_W", "Draw W").SetValue(true));
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
			if(W.IsReady())
				spellCombo.Add(SpellSlot.W);
			if(R.IsReady())
				spellCombo.Add(SpellSlot.R);
			return spellCombo;
		}

		private float GetComboDamage(Obj_AI_Base target)
		{
			double comboDamage = (float)MyHero.GetComboDamage(target, GetSpellCombo());
			return (float)(comboDamage + MyHero.GetAutoAttackDamage(target) * (R.IsReady() ? 3 : 1));
		}

		public override void OnDraw()
		{
			if(Menu.Item("Draw_Disabled").GetValue<bool>())
			{
				xSLxOrbwalker.DisableDrawing();
				return;
			}
			xSLxOrbwalker.EnableDrawing();

			if(Menu.Item("Draw_W").GetValue<bool>())
				if(W.Level > 0)
					Utility.DrawCircle(MyHero.Position, W.Range - 2, W.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_E").GetValue<bool>())
				if(E.Level > 0)
					Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_R").GetValue<bool>())
				if(R.Level > 0)
					Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

		}

		public override void OnPassive()
		{
			QHandler();
			SetERange();
		}

		private void QHandler()
		{
			if(Environment.TickCount - _qCheckTimer > 300)
			{
				_qCheckTimer = Environment.TickCount;
				_qActive = MyHero.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 2;
				if(AllHerosEnemy.Where(xSLxOrbwalker.InAutoAttackRange).Where(enemy => (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Combo || xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Harass) && IsSpellActive("Q")).Any(enemy => !_qActive))
				{
					Q.Cast();
					return;
				}
				if((!AllHerosEnemy.Any(xSLxOrbwalker.InAutoAttackRange) ||
					!(xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Harass ||
					  xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Combo)) && _qActive)
					Q.Cast();
			}
		}

		public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
		{
			if(R.IsReady() && Menu.Item("Misc_useR_Interrupt").GetValue<bool>() && unit.IsValidTarget(R.Range) &&
				spell.DangerLevel >= InterruptableDangerLevel.Medium)
				R.Cast(unit, UsePackets());
		}

		public override void OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
		{
			if(!Menu.Item("Misc_useE_Flash").GetValue<bool>() || unit.Team == MyHero.Team)
				return;
			if(spell.SData.Name.ToLower() == "summonerflash" && spell.End.Distance(MyHero.Position) <= E.Range + E.Width / 2)
				E.Cast(spell.End, UsePackets());
		}

		public override void OnCombo()
		{
			if (IsSpellActive("W"))
				Cast_W(true);
			if(Menu.Item("Combo_useR_onKillHelp").GetValue< bool>())
				Cast_R(1);
			if(Menu.Item("Combo_useR_onoutofRange").GetValue<bool>())
				Cast_R(2);
		}


		public override void OnHarass()
		{
			if(IsSpellActive("W") && ManaManagerAllowCast())
				Cast_W(true);
		}

		public override void OnLaneClear()
		{
			if(IsSpellActive("W") && ManaManagerAllowCast())
				Cast_W(false);
		}

		private void Cast_W(bool mode)
		{
			if (!W.IsReady())
				return;
			if (mode)
			{
				foreach (var enemy in AllHerosEnemy.Where( enemy => enemy.IsValidTarget( W.Range)))
				{
					if(ObjectManager.Get<Obj_AI_Base>().Any(obj => obj.IsValidTarget() && obj.IsMinion &&
																	MyHero.ServerPosition.To2D()
																		.Distance(W.GetPrediction(enemy).CastPosition.To2D(),
																			obj.Position.To2D(), true) <
																	25))
						return;
					W.Cast(enemy, UsePackets());
				}
			}
			else
			{
				var allMinions = MinionManager.GetMinions(MyHero.Position, W.Range, MinionTypes.All, MinionTeam.NotAlly);
				var farmlocation = W.GetCircularFarmLocation(allMinions, 300);
				if(farmlocation.Position.Distance(MyHero.Position) > 300 && farmlocation.Position.Distance(MyHero.Position)  < W.Range && (farmlocation.MinionsHit >= 3 || allMinions.Any(x => x.Team == GameObjectTeam.Neutral)))
					W.Cast(farmlocation.Position, UsePackets());
			}
		}

		private void Cast_R(int mode)
		{
			if (!R.IsReady())
				return;
			if (
				AllHerosEnemy.Where(hero => hero.IsValidTarget(R.Range))
					.Select(enemy => R.CastIfWillHit(enemy, Menu.Item("Combo_useR_enemyCount").GetValue<Slider>().Value, UsePackets()))
					.Any(casted => casted))
				return;


			var targetMode1 = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Physical);
			var targetMode2 = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical);
			switch (mode)
			{
				case 1:
					if (targetMode1 == null)
						return;
					if (targetMode1.Health < GetComboDamage(targetMode1))
						R.Cast(targetMode1, UsePackets());
					break;
				case 2:
					if(targetMode2 == null)
						return;
					if (targetMode2.Health < MyHero.GetSpellDamage(targetMode2, SpellSlot.R)*0.9 && !targetMode2.IsValidTarget(W.Range))
						R.Cast(targetMode2, UsePackets());
					break;
			}
		}

		private void SetERange()
		{
			var range = 1750 + (E.Level*750);
			E.Range = range;
		}

	}
}

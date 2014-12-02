using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using xSLx_Orbwalker;

namespace Ultimate_Carry_Prevolution.Plugin
{
	class Amumu : Champion
	{
		public Amumu()
		{
			SetSpells();
			LoadMenu();
		}

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q, 1080);
			Q.SetSkillshot(250, 90, 2000, true, SkillshotType.SkillshotLine);

			W = new Spell(SpellSlot.W, 300);
			W.SetSkillshot(150, 300, float.MaxValue, false, SkillshotType.SkillshotCircle);

			E = new Spell(SpellSlot.E, 350);
			E.SetSkillshot(225, 350, float.MaxValue, false, SkillshotType.SkillshotCircle);

			R = new Spell(SpellSlot.R, 550);
			R.SetSkillshot(300, 550, float.MaxValue, false, SkillshotType.SkillshotCircle); 
		}

		private void LoadMenu()
		{
			var champMenu = new Menu("Amumu Plugin", "Amumu");
			{
				var comboMenu = new Menu("Combo", "Combo");
				{
					comboMenu.AddItem(new MenuItem("Combo_useQ_Mode", "Use Q").SetValue(new StringList(new[] { "Off", "On", "Only out of range" }, 1)));
					AddSpelltoMenu(comboMenu, "W", true);
					AddSpelltoMenu(comboMenu, "E", true);
					comboMenu.AddItem(new MenuItem("Combo_useR_onAmount", "R on Enemys in Range").SetValue(new Slider(2, 5, 0)));
					champMenu.AddSubMenu(comboMenu);
				}
				var harassMenu = new Menu("Harass", "Harass");
				{
					AddSpelltoMenu(harassMenu, "W", true);
					AddSpelltoMenu(harassMenu, "E", true);
					AddManaManagertoMenu(harassMenu,30);
					champMenu.AddSubMenu(harassMenu);
				}
				var laneClearMenu = new Menu("LaneClear", "LaneClear");
				{
					laneClearMenu.AddItem(new MenuItem("LaneClear_useQ_Mode", "Use Q").SetValue(new StringList(new[] { "Off", "On", "Only out of range" }, 2)));
					AddSpelltoMenu(laneClearMenu, "W", true);
					AddSpelltoMenu(laneClearMenu, "E", true);
					AddManaManagertoMenu(laneClearMenu, 0);
					champMenu.AddSubMenu(laneClearMenu);
				}

				var miscMenu = new Menu("Misc", "Misc");
				{
					miscMenu.AddItem(new MenuItem("Misc_useW_turnOff", "turn off W if no Enemy in Range").SetValue(new Slider(300, 600, 0)));
					champMenu.AddSubMenu(miscMenu);
				}
				var drawMenu = new Menu("Drawing", "Drawing");
				{
					drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
					drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
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
			if(Q.IsReady())
				spellCombo.Add(SpellSlot.Q);
			if(W.IsReady())
				spellCombo.Add(SpellSlot.W);
			if(E.IsReady())
				spellCombo.Add(SpellSlot.E);
			if(R.IsReady())
				spellCombo.Add(SpellSlot.R);
			return spellCombo;
		}

		private float GetComboDamage(Obj_AI_Base target)
		{
			double comboDamage = (float)MyHero.GetComboDamage(target, GetSpellCombo());
			return (float)(comboDamage + MyHero.GetAutoAttackDamage(target));
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

			if(Menu.Item("Draw_W").GetValue<bool>())
				if(W.Level > 0)
					Utility.DrawCircle(MyHero.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_E").GetValue<bool>())
				if(E.Level > 0)
					Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_R").GetValue<bool>())
				if(R.Level > 0)
					Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);
		}

		public override void OnPassive()
		{
			WTurnOffCheck();
		}

		public override void OnCombo()
		{
			if (Menu.Item("Combo_useQ_Mode").GetValue<StringList>().SelectedIndex > 0)
				Cast_Q(Menu.Item("Combo_useQ_Mode").GetValue<StringList>().SelectedIndex,true);
			if(IsSpellActive("W"))
				Cast_W(true);
			if(IsSpellActive("E"))
				Cast_E(true);
			Cast_R();
		}

		public override void OnHarass()
		{
			if(IsSpellActive("W") && ManaManagerAllowCast() )
				Cast_W(true);
			if(IsSpellActive("E") && ManaManagerAllowCast() )
				Cast_E(true);
		}

		public override void OnLaneClear()
		{
			if(Menu.Item("LaneClear_useQ_Mode").GetValue<StringList>().SelectedIndex > 0 && ManaManagerAllowCast())
				Cast_Q(Menu.Item("LaneClear_useQ_Mode").GetValue<StringList>().SelectedIndex, false);
			if(IsSpellActive("W") && ManaManagerAllowCast())
				Cast_W(false);
			if(IsSpellActive("E") && ManaManagerAllowCast())
				Cast_E(false);
			Cast_R();
		}

		private void WTurnOffCheck()
		{
			if(MyHero.Spellbook.GetSpell(SpellSlot.W).ToggleState != 2 || !W.IsReady())
				return;
			var target = SimpleTs.GetTarget(W.Range + Menu.Item("Misc_useW_turnOff").GetValue<Slider>().Value, SimpleTs.DamageType.Magical);
			var target2 = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
			var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.NotAlly);
				
			if(xSLxOrbwalker.Mode.Combo == xSLxOrbwalker.CurrentMode && target == null)
			{
				W.Cast();
				return;
			}
			if(xSLxOrbwalker.Mode.Combo != xSLxOrbwalker.CurrentMode && (target2 == null && minions.Count == 0))
				W.Cast();
		}
		private void Cast_Q(int mode, bool mode2)
		{
			if(!Q.IsReady())
				return;
			if(mode2)
			{
				Obj_AI_Hero target;
				if (! R.IsReady( ))
				{
					target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
					if(target == null)
						return;
					if(mode == 1 || (mode == 2 && !xSLxOrbwalker.InAutoAttackRange(target)))
						Cast_BasicSkillshot_Enemy(Q);
				}
				if(R.IsReady() && Menu.Item("Combo_useR_onAmount").GetValue<Slider>().Value > 0)
				{
					var couldHitTargets = 0;
					Obj_AI_Base bestTarget = null;
					foreach(var unit in ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsValidTarget(Q.Range) && Q.GetPrediction(x).Hitchance >= HitChance.High))
					{
						var targetsHit = unit.CountEnemysInRange((int)R.Range);

						if(targetsHit <= couldHitTargets &&
							(bestTarget == null || targetsHit < couldHitTargets || unit.Type != GameObjectType.obj_AI_Hero))
							continue;
						couldHitTargets = targetsHit;
						bestTarget = unit;
					}
					if(couldHitTargets >= Menu.Item("Combo_useR_onAmount").GetValue<Slider>().Value)
						Q.Cast(bestTarget, UsePackets());
				}
				target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
				if(target == null)
					return;
				if (mode == 1 || (mode == 2 && !xSLxOrbwalker.InAutoAttackRange(target)))
					Cast_BasicSkillshot_Enemy(Q);
			}
			else
			{
				var minion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth).FirstOrDefault();

				if(minion == null)
					return;
				if(mode == 1 || (mode == 2 && !xSLxOrbwalker.InAutoAttackRange(minion)))
					Q.Cast(minion, UsePackets());
			}
		}

		private void Cast_W(bool mode)
		{
			if(!W.IsReady() || MyHero.Spellbook.GetSpell(SpellSlot.W).ToggleState == 2)
				return;
			if(mode)
			{
				var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
				if(target != null)
					W.Cast();
			}
			else
			{
				var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.NotAlly);
				if(minions.Count >= 3 || minions.Any(x => x.Team == GameObjectTeam.Neutral))
					W.Cast();
			}
		}

		private void Cast_E(bool mode)
		{
			if(!W.IsReady())
				return;
			if(mode)
			{
				var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);
				if(target != null)
					E.Cast();
			}
			else
			{
				var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);
				if(minions.Count >= 3 || minions.Any(x => x.Team == GameObjectTeam.Neutral))
					E.Cast();
			}
		}

		private void Cast_R()
		{
			if(!R.IsReady() || Menu.Item("Combo_useR_onAmount").GetValue<Slider>().Value <= 0)
				return;
			var hits = 0;
			var kills = 0;
			foreach(var enemy in from enemy in AllHerosEnemy.Where(x => x.IsValidTarget(R.Range))
								 let prediction = Prediction.GetPrediction(enemy, R.Delay)
								 where prediction != null && prediction.UnitPosition.Distance(MyHero.ServerPosition) <= R.Range
								 select enemy)
			{
				hits += 1;
				if(MyHero.GetSpellDamage(enemy, SpellSlot.R) >= enemy.Health)
					kills += 1;
			}
			if(hits >= Menu.Item("Combo_useR_onAmount").GetValue<Slider>().Value || (kills >= 1 && GetHealthPercent() <= 15))
				R.Cast();
		}




	}
}

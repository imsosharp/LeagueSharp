using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
	class Leona : Champion
	{
		public Leona()
		{
			SetSpells();
			LoadMenu();
		}

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q);

			W = new Spell(SpellSlot.W);

			E = new Spell(SpellSlot.E, 800);
			E.SetSkillshot(50, 100, 2050, false, SkillshotType.SkillshotLine);

			R = new Spell(SpellSlot.R, 1200);
			R.SetSkillshot(1000, 300, float.MaxValue, false, SkillshotType.SkillshotCircle);
		}

		private void LoadMenu()
		{
			var champMenu = new Menu("Leona Plugin", "Leona");
			{
				var comboMenu = new Menu("Combo", "Combo");
				{
					AddSpelltoMenu(comboMenu, "Q", true);
					AddSpelltoMenu(comboMenu, "W", true);
					AddSpelltoMenu(comboMenu, "E", true);
					comboMenu.AddItem(new MenuItem("Combo_useR_enemyCount", "R if Hit #").SetValue(new Slider(3, 1, 5)));
					champMenu.AddSubMenu(comboMenu);
				}
				var harassMenu = new Menu("Harass", "Harass");
				{
					AddSpelltoMenu(harassMenu, "Q", true);
					AddSpelltoMenu(harassMenu, "W", true);
					AddSpelltoMenu(harassMenu, "E", true);
					AddManaManagertoMenu(harassMenu, 30);
					champMenu.AddSubMenu(harassMenu);
				}

				var miscMenu = new Menu("Misc", "Misc");
				{
					miscMenu.AddItem(new MenuItem("Q_AutoDetonate", "Detonate Q Automatic").SetValue(true));
					miscMenu.AddItem(new MenuItem("Q_Interrupt", "Use Q to Interrupt").SetValue(true));
					miscMenu.AddItem(new MenuItem("Q_UnderTurret", "Use Q Under Turret").SetValue(true));
					miscMenu.AddItem(new MenuItem("Q_AgainstWard", "Use Q to Kill Ward").SetValue(true));
					miscMenu.AddItem(new MenuItem("W_AntiGapClose", "Use W Anti GapClose").SetValue(true));
					miscMenu.AddItem(new MenuItem("W_Interrupt", "Use W to Interrupt").SetValue(true));
					miscMenu.AddItem(new MenuItem("W_AgainstAA", "Use W against AA").SetValue(true));
					miscMenu.AddItem(new MenuItem("W_AgainstSpells", "Use W against Spells").SetValue(true));
					miscMenu.AddItem(new MenuItem("R_Interrupt", "Use R to Interrupt").SetValue(true));
					champMenu.AddSubMenu(miscMenu);
				}
				var drawMenu = new Menu("Drawing", "Drawing");
				{
					drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
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

		private float GetComboDamage(Obj_AI_Base enemy)
		{
			var damage = 0d;

			if(Q.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.Q);

			if(W.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.W);

			if(E.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.E);

			if(R.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.R);

			return (float)damage;
		}
		public override void OnGapClose(ActiveGapcloser gapcloser)
		{
			if(Menu.Item("Q_AntiGapClose").GetValue<bool>())
			{
				if(xSLxOrbwalker.InAutoAttackRange(gapcloser.Sender))
				{
					MyHero.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
					Cast_Q(gapcloser.Sender);
				}
			}
			if(Menu.Item("W_AntiGapClose").GetValue<bool>())
			{
				Cast_W();
			}
		}

		public override void OnAttack(Obj_AI_Base unit, Obj_AI_Base target)
		{
			if(!unit.IsEnemy || !target.IsMe || unit.IsMinion)
				return;
			if(Menu.Item("W_AgainstAA").GetValue<bool>())
				Cast_W();
		}

		public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
		{
			if(Menu.Item("Q_Interrupt").GetValue<bool>())
			{
				if(xSLxOrbwalker.InAutoAttackRange(unit))
				{
					MyHero.IssueOrder(GameObjectOrder.AttackUnit, unit);
					Cast_Q(unit);
				}
			}

			if(Menu.Item("W_Interrupt").GetValue<bool>() && spell.DangerLevel >= InterruptableDangerLevel.Medium)
			{
				Cast_W();
			}

			if(Menu.Item("R_Interrupt").GetValue<bool>() && unit.IsValidTarget(R.Range) && spell.DangerLevel >= InterruptableDangerLevel.High)
			{
				var pred = R.GetPrediction(unit, true);
				R.Cast(pred.CastPosition, UsePackets());
			}
		}

		public override void OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
		{
			if(Menu.Item("W_AgainstSpells").GetValue<bool>())
			{
				if(spell.Target != null && spell.Target.IsMe)
					Cast_W();

				if(spell.End.Distance(MyHero.ServerPosition) < 200)
					Cast_W();
			}
		}

		public override void ObjSpellMissileOnOnCreate(GameObject sender, EventArgs args)
		{
			if(Menu.Item("Q_AgainstWard").GetValue<bool>() && sender.Name.ToLower().Contains("ward") && MyHero.Distance(sender.Position) <= MyHero.AttackRange)
			{
				var query = AllHerosEnemy.Where(xSLxOrbwalker.InAutoAttackRange); // dont focus ward if enemy in range

				if(!query.Any())
					MyHero.IssueOrder(GameObjectOrder.AttackUnit, sender);
			}
		}


		public override void OnDraw()
		{
			if(Menu.Item("Draw_Disabled").GetValue<bool>())
			{
				xSLxOrbwalker.DisableDrawing();
				return;
			}
			xSLxOrbwalker.EnableDrawing();

			if(Menu.Item("Draw_E").GetValue<bool>())
				if(E.Level > 0)
					Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_R").GetValue<bool>())
				if(R.Level > 0)
					Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

		}


		public override void OnAfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
		{
			// double AA with reset
			if(unit.IsMe && target is Obj_AI_Hero && IsSpellActive("Q"))
				Cast_Q(target);

			// Q against ward
			if(unit.IsMe && target.Name.ToLower().Contains("ward"))
				Cast_Q(target);
		}


		public override void OnCombo()
		{
			if(IsSpellActive("W"))
				Cast_W();
			if (IsSpellActive("E"))
				Cast_BasicSkillshot_Enemy(E);

			Cast_R_MinHit();
			Cast_Q_UnderTower();
		}

		public override void OnHarass()
		{
			if(IsSpellActive("E") && ManaManagerAllowCast())
				Cast_BasicSkillshot_Enemy(E);
			if(IsSpellActive("W") && ManaManagerAllowCast())
				Cast_W();

			Cast_R_MinHit();
			Cast_Q_UnderTower();
		}


		private void Cast_Q(Obj_AI_Base lastTarget = null)
		{
			if(!Q.IsReady())
				return;

			Q.Cast(UsePackets());

			if(lastTarget != null)
				MyHero.IssueOrder(GameObjectOrder.AttackUnit, lastTarget);
		}

		private void Cast_W()
		{
			if(!W.IsReady())
				return;
			var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.True);
			if (target == null || !E.IsReady() || !IsSpellActive("E")) return;
			W.Cast();
		}

		private void Cast_R_MinHit()
		{
			// What do you think about GetCircularFarmLocation, this will use MEC

			var minEnemyCount = 5;
			if(xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Combo)
				minEnemyCount = Menu.Item("Combo_useR_enemyCount").GetValue<Slider>().Value;

			var query = AllHerosEnemy
				.Where(x => x.IsValidTarget(R.Range))
				.Select(x => x.ServerPosition.To2D());

			var rLocation = R.GetCircularFarmLocation(query.ToList());
			if(rLocation.MinionsHit >= minEnemyCount)
				R.Cast(rLocation.Position);
		}

		private void Cast_Q_UnderTower()
		{
			if(!Menu.Item("Q_UnderTurret").GetValue<bool>())
				return;
			// need some improve to use E to gapcloser on enemy and stun under tower
			var query = AllHerosEnemy.Where(x => xSLxOrbwalker.InAutoAttackRange(x) && Utility.UnderTurret(x));
			var objAiHeroes = query as Obj_AI_Hero[] ?? query.ToArray();
			if(!objAiHeroes.Any())
				return;
			var target = objAiHeroes.First();
			MyHero.IssueOrder(GameObjectOrder.AttackUnit, target);
			Cast_Q(target);
		}

	}
}

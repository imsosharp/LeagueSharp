﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;


namespace Ultimate_Carry_Prevolution.Plugin
{
	class Fiora : Champion
	{
		private int _qCastedtime;

		public Fiora()
		{
			SetSpells();
			LoadMenu();
		}

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q, 600);
			Q.SetTargetted(0.25f, float.MaxValue);

			W = new Spell(SpellSlot.W);

			E = new Spell(SpellSlot.E);

			R = new Spell(SpellSlot.R, 400);
			R.SetTargetted(0.25f, float.MaxValue);
		}

		private void LoadMenu()
		{
			var champMenu = new Menu("Fiora Plugin", "Fiora");
			{
				var comboMenu = new Menu("Combo", "Combo");
				{
					AddSpelltoMenu(comboMenu, "Q", true);
					AddSpelltoMenu(comboMenu, "W", true);
					AddSpelltoMenu(comboMenu, "E", true);
					comboMenu.AddItem(new MenuItem("Combo_useR_enemyHitCount", "R if Hit #").SetValue(new Slider(2, 0, 5)));
					comboMenu.AddItem(new MenuItem("Combo_useR_enemyKillCount", "R if Kill #").SetValue(new Slider(1, 0, 5)));
					champMenu.AddSubMenu(comboMenu);
				}
				var harassMenu = new Menu("Harass", "Harass");
				{
					AddSpelltoMenu(harassMenu, "W", true);
					AddSpelltoMenu(harassMenu, "E", true);
					AddManaManagertoMenu(harassMenu, 30);
					champMenu.AddSubMenu(harassMenu);
				}
				var laneClearMenu = new Menu("LaneClear", "LaneClear");
				{
					AddSpelltoMenu(laneClearMenu, "Q", true);
					AddSpelltoMenu(laneClearMenu, "E", true);
					AddManaManagertoMenu(laneClearMenu, 30);
					champMenu.AddSubMenu(laneClearMenu);
				}

				var miscMenu = new Menu("Misc", "Misc");
				{
					miscMenu.AddItem(new MenuItem("W_AgainstAA", "Cast W Against AA").SetValue(true));
					miscMenu.AddItem(new MenuItem("W_AfterAttack", "Cast W After Attack").SetValue(true));
					miscMenu.AddItem(new MenuItem("E_AfterAttack", "Cast E After Attack").SetValue(true));
					champMenu.AddSubMenu(miscMenu);
				}
				var drawMenu = new Menu("Drawing", "Drawing");
				{
					drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
					drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
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

			if(Q.IsReady() || W.IsReady() || E.IsReady())
				damage += MyHero.GetAutoAttackDamage(enemy) * 2;
			else
				damage += MyHero.GetAutoAttackDamage(enemy);

			if(R.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.R);

			return (float)damage;
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

			if(Menu.Item("Draw_R").GetValue<bool>())
				if(R.Level > 0)
					Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);
		}


		public override void OnCombo()
		{
			if(IsSpellActive("Q"))
			{
				Cast_Q();
				Cast_SecondQ();
			}
			if(IsSpellActive("W"))
				Cast_W();
			if(IsSpellActive("E"))
				Cast_E();

			Cast_R_MinHit();
		}

		public override void OnHarass()
		{

			if(IsSpellActive("W") && ManaManagerAllowCast())
				Cast_W();
			if(IsSpellActive("E") && ManaManagerAllowCast())
				Cast_E();
		}

		public override void OnLaneClear()
		{
			var minionList = MinionManager.GetMinions(MyHero.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
			var jungleList = MinionManager.GetMinions(MyHero.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

			if(jungleList.Any())
			{
				var jungle = jungleList.First();
				if(IsSpellActive("Q") && Q.IsReady() && ManaManagerAllowCast())
					Q.CastOnUnit(jungle, UsePackets());
				if(IsSpellActive("E") && E.IsReady() && ManaManagerAllowCast())
					E.CastOnUnit(jungle, UsePackets());
			}

			if(minionList.Any())
			{
				var minion = minionList.First();
				if(IsSpellActive("Q") && Q.IsReady())
					Q.CastOnUnit(minion, UsePackets());
				if(IsSpellActive("E") && E.IsReady())
					E.CastOnUnit(minion, UsePackets());
			}
		}

		public override void OnAttack(Obj_AI_Base unit, Obj_AI_Base target)
		{
			if(unit.IsEnemy && !unit.IsMinion && target.IsMe && Menu.Item("W_AgainstAA").GetValue<bool>() && W.IsReady())
				W.Cast(UsePackets());
		}

		public override void OnAfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
		{
			if(unit.IsMe && target.IsEnemy && !target.IsMinion)
			{
				if(Menu.Item("W_AfterAttack").GetValue<bool>() && IsSpellActive("W") && W.IsReady())
				{
					W.Cast(UsePackets());
				}

				if(Menu.Item("E_AfterAttack").GetValue<bool>() && IsSpellActive("E") && E.IsReady())
				{
					E.Cast(UsePackets());
				}
			}
		}

		private bool HasSecondQBuff()
		{
			return MyHero.HasBuff("FioraQCD");
		}

		private void Cast_Q()
		{
			if(!Q.IsReady() || HasSecondQBuff() || Environment.TickCount - _qCastedtime < 3800)
				return;

			var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
			if(target == null)
				return;

			if(xSLxOrbwalker.InAutoAttackRange(target))
				return;
			_qCastedtime = Environment.TickCount;
			Q.CastOnUnit(target, UsePackets());

		}

		private void Cast_SecondQ()
		{
			if(!HasSecondQBuff())
				return;

			var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
			if(target == null)
				return;

			if(Environment.TickCount - _qCastedtime  > 3600)
			{
				Q.CastOnUnit(target, UsePackets());
			}

			if(MyHero.GetSpellDamage(target, SpellSlot.Q) * 0.95 > target.Health)
			{
				Q.CastOnUnit(target, UsePackets());
			}

			if(!xSLxOrbwalker.InAutoAttackRange( target ))
			{
				Q.CastOnUnit(target, UsePackets());
			}
		}


		public override void OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
		{
			if(unit.IsMe && MyHero.GetSpellSlot(spell.SData.Name, false) == SpellSlot.Q)
			{
				_qCastedtime = Environment.TickCount;
			}

		}


		private void Cast_W()
		{
			if(!W.IsReady())
				return;

			var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
			if(target == null)
				return;

			if(!xSLxOrbwalker.InAutoAttackRange(target))
				return;

			W.Cast(UsePackets());
		}

		private void Cast_E()
		{
			if(!E.IsReady())
				return;

			var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
			if(target == null)
				return;

			if(!xSLxOrbwalker.InAutoAttackRange(target))
				return;

			E.Cast(UsePackets());
		}

		private void Cast_R_MinHit()
		{
			var minEnemyHitCount = Menu.Item("Combo_useR_enemyHitCount").GetValue<Slider>().Value;
			var minEnemyKillCount = Menu.Item("Combo_useR_enemyKillCount").GetValue<Slider>().Value;

			if(minEnemyHitCount == 0)
				return;

			var enemies = xSLxOrbwalker.AllEnemys.Where(hero => hero.IsValidTarget(R.Range) && CountEnemyInPositionRange(hero.ServerPosition, 250) >= minEnemyHitCount).OrderBy(hero => hero.Health);
			if(enemies.Any())
			{
				var enemy = enemies.First();
				var enemyKillable = xSLxOrbwalker.AllEnemys.Where(x => x.ServerPosition.Distance(enemy.ServerPosition) <= 250 && R.GetDamage(x) > x.Health);
				if(enemyKillable.Count() >= minEnemyKillCount)
					R.CastOnUnit(enemy, UsePackets());
			}

		}
		public static int CountEnemyInPositionRange(Vector3 position, float range)
		{
			return xSLxOrbwalker.AllEnemys.Count(x => x.ServerPosition.Distance(position) <= range);
		}

	}
}
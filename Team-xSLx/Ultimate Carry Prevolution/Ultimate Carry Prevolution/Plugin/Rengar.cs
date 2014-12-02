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
	class Rengar : Champion
	{
		// in progress

		private int _LastQCast;
		public Rengar()
		{
			SetSpells();
			LoadMenu();
		}

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q);

			W = new Spell(SpellSlot.W,490);

			E = new Spell(SpellSlot.E, 1000);
			E.SetSkillshot(0.5f, 70, 1500, true, SkillshotType.SkillshotLine);

			E = new Spell(SpellSlot.R, 1800);

		}
		private void LoadMenu()
		{
			var champMenu = new Menu("Rengar Plugin", "Rengar");
			{
				var comboMenu = new Menu("Combo", "Combo");
				{
					AddSpelltoMenu(comboMenu, "Q", true);
					AddSpelltoMenu(comboMenu, "W", true);
					AddSpelltoMenu(comboMenu, "E", true);
					AddSpelltoMenu(comboMenu, "R", true);
					champMenu.AddSubMenu(comboMenu);
				}
				var harassMenu = new Menu("Harass", "Harass");
				{
					AddSpelltoMenu(harassMenu, "Q", true);

					champMenu.AddSubMenu(harassMenu);
				}
				var laneClearMenu = new Menu("LaneClear", "LaneClear");
				{
					AddSpelltoMenu(laneClearMenu, "Q", true);

					champMenu.AddSubMenu(laneClearMenu);
				}

				var drawMenu = new Menu("Drawing", "Drawing");
				{
					drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
					drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_W", "Draw W").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_E", "Draw E").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_R", "Draw R").SetValue(true));

					//var drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage").SetValue(true);
					//drawMenu.AddItem(drawComboDamageMenu);
					//Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
					//Utility.HpBarDamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
					//drawComboDamageMenu.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
					//{
					//	Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
					//};

					//champMenu.AddSubMenu(drawMenu);
				}
			}

			Menu.AddSubMenu(champMenu);
			Menu.AddToMainMenu();

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
				if (Q.Level > 0)
					Utility.DrawCircle(MyHero.Position, xSLxOrbwalker.GetAutoAttackRange() + 10, Q.IsReady() ? Color.Green : Color.Red);

			//if(Menu.Item("Draw_E").GetValue<bool>())
			//	if(E.Level > 0)
			//		Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

			//if(Menu.Item("Draw_R").GetValue<bool>())
			//	if(R.Level > 0)
			//		Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);
		}

		public override void OnAfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
		{
			if(unit.IsMe)
			{
				if(Q.IsReady() && IsSpellActive("Q"))
				{
					if(xSLxOrbwalker.CurrentMode != xSLxOrbwalker.Mode.LaneClear)
						if(!(target is Obj_AI_Hero))
							return;

					if (target.Health - (MyHero.GetAutoAttackDamage(target) - 35) > 0)
						Cast_Q(target);
				}
			}
		}

		public override void OnCombo()
		{
			if(IsSpellActive("Q"))
				Cast_Q();
			if(IsSpellActive("W"))
				Cast_W();
			if(IsSpellActive("E"))
				Cast_E();
		}

		public override void ObjSpellMissileOnOnCreate(GameObject sender, EventArgs args)
		{
			if (sender.Name == "Rengar_LeapSound.troy")
				Utility.DelayAction.Add(250,() => OnAfterAttack(MyHero, xSLxOrbwalker.GetPossibleTarget()));
		}

		private void Cast_Q(Obj_AI_Base target = null)
		{
			if(!Q.IsReady())
				return;

			if(target != null)
			{
				if(!(target.IsMinion || target is Obj_AI_Hero))
					return;
				Q.Cast();
				MyHero.IssueOrder(GameObjectOrder.AttackUnit, target);
				_LastQCast = Environment.TickCount;
			}
			else
			{
				target = xSLxOrbwalker.GetPossibleTarget();
				if(target == null)
					return;
				if(!(target.IsMinion || target is Obj_AI_Hero))
					return;
				if(!(Environment.TickCount - _LastQCast < xSLxOrbwalker.GetNextAATime()))
					return;
				Q.Cast();
				MyHero.IssueOrder(GameObjectOrder.AttackUnit, target);
				_LastQCast = Environment.TickCount;
			}
		}

		private void Cast_W()
		{
			if(!W.IsReady())
				return;
			var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Physical);
			var stage = MyHero.Mana;
			if(stage > 3)
			{
				if(MyHero.CountEnemysInRange((int)W.Range) >= 1 && GetHealthPercent(MyHero) < 10)
					W.Cast();
				if(MyHero.CountEnemysInRange((int)W.Range) >= 2 && GetHealthPercent(MyHero) < 30)
					W.Cast();
			}
			else
			{
				if(MyHero.CountEnemysInRange((int)W.Range) >= 1)
					W.Cast();
			}


		}

		private void Cast_E()
		{
			if(!E.IsReady())
				return;
			var stage = MyHero.Mana;
			var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
			if (stage < 4 || stage > 4)
				E.Cast(target, UsePackets());
		}

		private void Cast_R()
		{
			if(!R.IsReady() || !Q.IsReady())
				return;
			var stage = MyHero.Mana;
			var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical);
			if((int)stage == 5 && target.IsValidTarget(R.Range))
				R.Cast();
		}
	}
}

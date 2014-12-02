using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
	class Ryze : Champion 
	{
		// in progress

		public Ryze()
		{
			SetSpells();
			LoadMenu();
		}

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q, 625);
			W = new Spell(SpellSlot.W, 600);
			E = new Spell(SpellSlot.E, 600);
			R = new Spell(SpellSlot.R);
		}

		private void LoadMenu()
		{
			var champMenu = new Menu("Ryze Plugin", "Ryze");
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
					AddSpelltoMenu(harassMenu, "W", true);
					AddSpelltoMenu(harassMenu, "E", true);
					AddManaManagertoMenu(harassMenu, 30);
					champMenu.AddSubMenu(harassMenu);
				}
				var laneClearMenu = new Menu("LaneClear", "LaneClear");
				{
					AddSpelltoMenu(laneClearMenu, "Q", true);
					AddSpelltoMenu(laneClearMenu, "W", true);
					AddManaManagertoMenu(laneClearMenu, 20);
					champMenu.AddSubMenu(laneClearMenu);
				}
				var fleeMenu = new Menu("Flee", "Flee");
				{
					AddSpelltoMenu(fleeMenu, "W", true, "Use W to block enemy");
					champMenu.AddSubMenu(fleeMenu);
				}
				var drawMenu = new Menu("Drawing", "Drawing");
				{
					drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
					drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_W", "Draw W").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_E", "Draw E").SetValue(true));

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
			if (Q.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.Q) * 2;
			if (W.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.W);
			if (E.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.E);
			if (R.IsReady())
				damage += damage * 0.5;
			return (float)(damage);
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
					Utility.DrawCircle(MyHero.Position, W.Range - 2, W.IsReady() ? Color.Green : Color.Red);
			
			if(Menu.Item("Draw_E").GetValue<bool>())
				if(E.Level > 0)
					Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

		}

		public override void OnCombo()
		{

			if (IsSpellActive("Q"))
				Cast_Q();
			if (IsSpellActive("R"))
				Cast_R();
			if (IsSpellActive("E"))
				Cast_E();
			if (IsSpellActive("W"))
				Cast_W();
			if (IsSpellActive("Q"))
				Cast_Q();
		}

		public override void OnHarass()
		{
			if (IsSpellActive("Q"))
				Cast_Q();
			if (IsSpellActive("E"))
				Cast_E();
			if (IsSpellActive("W"))
				Cast_W();
			if (IsSpellActive("Q"))
				Cast_Q();
		}

		public override void OnLaneClear()
		{
			if (IsSpellActive("Q") && ManaManagerAllowCast())
				Cast_Q(true);
			if (IsSpellActive("E"))
				Cast_E(true);
		}

		public override void OnFlee()
		{
			if (IsSpellActive("W"))
				Cast_W();
		}

		private void Cast_Q(bool minion = false)
		{
			if (!Q.IsReady())
				return;

			Obj_AI_Base target;

			if (minion)
			{
				var allMinions =
					MinionManager.GetMinions(MyHero.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly).ToList();
				var minionLastHit = allMinions.Where(x => HealthPrediction.LaneClearHealthPrediction(x, (int)Q.Delay ) < MyHero.GetSpellDamage(x, SpellSlot.Q) * 0.8).OrderBy(x => x.Health);

				if(!minionLastHit.Any())
					return;

				target = minionLastHit.First();
			}
			else
				target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);

			Q.CastOnUnit(target, UsePackets());
		}

		private void Cast_W()
		{
			if (!W.IsReady())
				return;

			var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);

			W.CastOnUnit(target, UsePackets());
		}

		private void Cast_E(bool minion = false)
		{
			if (!E.IsReady())
				return;

			Obj_AI_Base target;
			
			if (minion)
			{
				var allMinions =
					MinionManager.GetMinions(MyHero.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly).ToList();
				var minionLastHit = allMinions.Where(x => HealthPrediction.LaneClearHealthPrediction(x, (int)E.Delay ) < MyHero.GetSpellDamage(x, SpellSlot.E) * 0.8).OrderBy(x => x.Health);

				if(!minionLastHit.Any())
					return;

				target = minionLastHit.First();
			}
			else
				target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);

			E.CastOnUnit(target, UsePackets());
		}

		private void Cast_R()
		{
			if(!R.IsReady())
				return;
			R.Cast();
		}
	}
}

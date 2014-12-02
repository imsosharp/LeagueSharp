using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
	class Gnar : Champion
	{

		private GnarStage _stage = GnarStage.NORMAL ;
		public Gnar()
        {
            SetSpells();
            LoadMenu();
        }

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q, 1100);
			Q.SetSkillshot(66, 60, 1400, true, SkillshotType.SkillshotLine);

			Q2 = new Spell(SpellSlot.Q, 1100);
			Q2.SetSkillshot(600, 90, 2100, true, SkillshotType.SkillshotLine);

			W = new Spell(SpellSlot.W, 525);
			W.SetSkillshot(250, 80, 1200, false, SkillshotType.SkillshotLine);

			E = new Spell(SpellSlot.E, 500);
			E.SetSkillshot(695, 150, 2000, false, SkillshotType.SkillshotCircle);

			E2 = new Spell(SpellSlot.E, 500);
			E2.SetSkillshot(695, 350, 2000, false, SkillshotType.SkillshotCircle);

			R = new Spell(SpellSlot.R, 10);
			R.SetSkillshot(66, 400, 1400, false, SkillshotType.SkillshotCircle);
		}

		private void LoadMenu()
		{
			var champMenu = new Menu("Gnar Plugin", "Gnar");
			{
				var comboMenu = new Menu("Combo", "Combo");
				{
					AddSpelltoMenu(comboMenu, "Q", true);
					AddSpelltoMenu(comboMenu, "W", true);
					comboMenu.AddItem(new MenuItem("Combo_useE_Mode", "Use E").SetValue(new StringList(new[] { "Off", "On", "Only in Mega" }, 2)));
					AddSpelltoMenu(comboMenu, "R", true);
					champMenu.AddSubMenu(comboMenu);
				}

				var harassMenu = new Menu("Harass", "Harass");
				{
					AddSpelltoMenu(harassMenu, "Q", true);
					AddSpelltoMenu(harassMenu, "W", true);
					champMenu.AddSubMenu(harassMenu);
				}
				var laneClearMenu = new Menu("LaneClear", "LaneClear");
				{
					AddSpelltoMenu(laneClearMenu, "Q", true);
					AddSpelltoMenu(laneClearMenu, "W", true);
					AddSpelltoMenu(laneClearMenu, "E", true);
					champMenu.AddSubMenu(laneClearMenu);
				}

				var lasthitMenu = new Menu("Lasthit", "Lasthit");
				{
					AddSpelltoMenu(lasthitMenu, "Q", true);
				}

				var miscMenu = new Menu("Misc", "Misc");
				{
					miscMenu.AddItem(new MenuItem("Misc_useR_ifcanKill", "inCombo R if canKill").SetValue(false));
					miscMenu.AddItem(new MenuItem("Misc_useR_ifStun", "inCombo R if Stun").SetValue(true));
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
					Utility.DrawCircle(MyHero.Position, R.Width, R.IsReady() ? Color.Green : Color.Red);
		}
		private IEnumerable<SpellSlot> GetSpellCombo()
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

		private float GetComboDamage(Obj_AI_Base target)
		{
			double comboDamage = (float)MyHero.GetComboDamage(target, GetSpellCombo());
			return (float)(comboDamage + MyHero.GetAutoAttackDamage(target) * 2);
		}
		private enum GnarStage
		{
			NORMAL,
			TRANSFORMING,
			MEGA,
		}

		public override void OnPassive()
		{
			CheckState();
		}

		public override void OnCombo()
		{
			if (IsSpellActive("Q"))
				Cast_Q(true);
			if(IsSpellActive("W"))
				Cast_W();
			if(Menu.Item("Combo_useE_Mode").GetValue<StringList>().SelectedIndex > 0)
				Cast_E();
			if(IsSpellActive("R"))
				Cast_R();
		}

		public override void OnHarass()
		{
			if(IsSpellActive("Q"))
				Cast_Q(true);
			if(IsSpellActive("W"))
				Cast_W();
		}

		public override void OnLaneClear()
		{
			if (IsSpellActive("Q"))
				Cast_Q(false);
			if (IsSpellActive("W") && _stage != GnarStage.NORMAL)
				Cast_BasicSkillshot_AOE_Farm(W);
			if(IsSpellActive("E") && _stage != GnarStage.NORMAL)
				Cast_BasicSkillshot_AOE_Farm(E2);
		}

		public override void OnLasthit()
		{
			if(IsSpellActive("Q"))
				Cast_Q(false);
		}

		private void Cast_Q(bool mode)
		{
			var tempSpell = _stage == GnarStage.NORMAL ? Q : Q2;
			if(!tempSpell.IsReady())
				return;
			if (mode)
			{
				var target = SimpleTs.GetTarget(tempSpell.Range, SimpleTs.DamageType.Physical);
				if(target == null)
					return;
				if(target.IsValidTarget(tempSpell.Range) && tempSpell.GetPrediction(target).Hitchance >= HitChance.High)
					tempSpell.Cast(target, UsePackets());
				if(tempSpell.GetPrediction(target).Hitchance != HitChance.Collision)
					return;
				var qCollision = tempSpell.GetPrediction(target).CollisionObjects;
				if((!qCollision.Exists(coll => coll.Distance(target) > 180 && _stage == GnarStage.NORMAL)) || (!qCollision.Exists(coll => coll.Distance(target) > 40)))
					tempSpell.Cast(target.Position, UsePackets());			
			}
			else
			{
				var allMinions = MinionManager.GetMinions(MyHero.ServerPosition, tempSpell.Range, MinionTypes.All, MinionTeam.NotAlly);
				foreach(var minion in allMinions)
				{
					if(!minion.IsValidTarget())
						continue;
					var minionInRangeAa = xSLxOrbwalker.InAutoAttackRange( minion);
					var minionInRangeSpell = minion.IsValidTarget( tempSpell.Range);
					var minionKillableAa = MyHero.GetAutoAttackDamage(minion) >= minion.Health;
					var minionKillableSpell = MyHero.GetSpellDamage(minion, SpellSlot.Q) >= minion.Health;
					var lastHit = xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Lasthit;
					var laneClear = xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.LaneClear;

					if((lastHit && minionInRangeSpell && minionKillableSpell) && ((minionInRangeAa && !minionKillableAa) || !minionInRangeAa))
						tempSpell.Cast(minion.Position, UsePackets());
					else if((laneClear && minionInRangeSpell && !minionKillableSpell) && ((minionInRangeAa && !minionKillableAa) || !minionInRangeAa))
						tempSpell.Cast(minion.Position, UsePackets());
				}
			}
		}

		private void Cast_W()
		{
			if(!W.IsReady())
				return;
				var target = SimpleTs.GetTarget(W.Range,SimpleTs.DamageType.Physical);
				if(target == null)
					return;
				if(target.IsValidTarget(W.Range) && W.GetPrediction(target).Hitchance >= HitChance.High)
					W.Cast(target, UsePackets());			
		}

		private void Cast_E()
		{
			if(xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Combo)
				if(Menu.Item("Combo_useE_Mode").GetValue<StringList>().SelectedIndex == 2 && _stage == GnarStage.NORMAL)
					return;
			var tempSpell = _stage == GnarStage.NORMAL ? E : E2;
			if(!tempSpell.IsReady())
				return;

			var target = SimpleTs.GetTarget(tempSpell.Range + tempSpell.Width / 2, SimpleTs.DamageType.Physical);
			if(target == null)
				return;
			if(target.IsValidTarget(tempSpell.Range + tempSpell.Width / 2) && tempSpell.GetPrediction(target).Hitchance >= HitChance.High)
				tempSpell.Cast(target, UsePackets());

		}

		private void Cast_R()
		{
			if(!R.IsReady())
				return;

			if(Menu.Item("Misc_useR_ifStun").GetValue<bool>())
				foreach(var target in AllHerosEnemy.Where(hero => hero.IsValidTarget(R.Width)))
					CastRToCollision(GetCollision(target));

			if(Menu.Item("Misc_useR_ifcanKill").GetValue<bool>())
			{
				var target = SimpleTs.GetTarget(390, SimpleTs.DamageType.Physical);
				if(target.Health < GetComboDamage(target) && target.IsValidTarget(MyHero.Distance(R.GetPrediction(target).CastPosition)))
					R.Cast(V2E(MyHero.Position, R.GetPrediction(target).CastPosition, -50), UsePackets());
			}
		}
		private void CastRToCollision(int collisionId)
		{
			if(collisionId == -1)
				return;
			var center =MyHero.ServerPosition;
			const int points = 36;
			const int radius = 300;

			const double slice = 2 * Math.PI / points;
			for(var i = 0; i < points; i++)
			{
				var angle = slice * i;
				var newX = (int)(center.X + radius * Math.Cos(angle));
				var newY = (int)(center.Y + radius * Math.Sin(angle));
				var p = new Vector3(newX, newY, 0);
				if(collisionId == i)
					R.Cast(p, UsePackets());
			}
		}

		private int GetCollision(GameObject enemy)
		{
			var center = enemy.Position;
			const int points = 36;
			const int radius = 300;
			var positionList = new List<Vector3>();

			const double slice = 2 * Math.PI / points;
			for(var i = 0; i < points; i++)
			{
				var angle = slice * i;
				var newX = (int)(center.X + radius * Math.Cos(angle));
				var newY = (int)(center.Y + radius * Math.Sin(angle));
				var p = new Vector3(newX, newY, 0);

				if(NavMesh.GetCollisionFlags(p) == CollisionFlags.Wall || NavMesh.GetCollisionFlags(p) == CollisionFlags.Building)
					return i;
			}
			return -1;
		}
		private void CheckState()
		{
			var tempState = GnarStage.NORMAL;
			foreach(var buff in MyHero.Buffs)
			{
				if(buff.Name == "gnartransformsoon")
					tempState = GnarStage.TRANSFORMING ;
				if(buff.Name == "gnartransform")
					tempState = GnarStage.MEGA ;
			}
			_stage = tempState;
		}
	}
}

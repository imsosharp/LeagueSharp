using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
	class Ahri : Champion 
	{
		public Ahri()
		{
			SetSpells();
			LoadMenu();
		}

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q, 950);
			Q.SetSkillshot(215, 100, 1600, false, SkillshotType.SkillshotLine);

			W = new Spell(SpellSlot.W, 700);
			W.SetSkillshot(710,700, float.MaxValue, false, SkillshotType.SkillshotLine);
			
			E = new Spell(SpellSlot.E, 950);
			E.SetSkillshot(230, 60, 1500, true, SkillshotType.SkillshotLine);

			R = new Spell(SpellSlot.R, 900);
		}

		private void LoadMenu()
		{
			var champMenu = new Menu("Ahri Plugin", "Ahri");
			{
				var comboMenu = new Menu("Combo", "Combo");
				{
					AddSpelltoMenu(comboMenu, "Q", true);
					AddSpelltoMenu(comboMenu, "W", true);
					AddSpelltoMenu(comboMenu, "E", true);
					AddSpelltoMenu(comboMenu, "R", true);
					comboMenu.AddItem(new MenuItem("Combo_useR_onUseActivate", "Use just Stack 2-3 R").SetValue(false));
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
					AddSpelltoMenu(fleeMenu, "R", true, "Use R to Mouse");
					AddSpelltoMenu(fleeMenu, "E", true, "Use E to charm Enemy");
					champMenu.AddSubMenu(fleeMenu);
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
					Utility.DrawCircle(MyHero.Position, W.Range - 2, W.IsReady() ? Color.Green : Color.Red);
			
			if(Menu.Item("Draw_E").GetValue<bool>())
				if(E.Level > 0)
					Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_R").GetValue<bool>())
				if(R.Level > 0)
					Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

		}

		public override void OnCombo()
		{
			if(IsSpellActive("E"))
				Cast_BasicSkillshot_Enemy(E, SimpleTs.DamageType.Magical);
			if(IsSpellActive("Q"))
				Cast_BasicSkillshot_Enemy(Q,SimpleTs.DamageType.Magical);
			if(IsSpellActive("W"))
				Cast_W();
			if(IsSpellActive("R"))
				Cast_R();
		}

		public override void OnHarass()
		{
			if(IsSpellActive("E") && ManaManagerAllowCast())
				Cast_BasicSkillshot_Enemy(E, SimpleTs.DamageType.Magical);
			if(IsSpellActive("Q") && ManaManagerAllowCast())
				Cast_BasicSkillshot_Enemy(Q, SimpleTs.DamageType.Magical);
			if(IsSpellActive("W") && ManaManagerAllowCast())
				Cast_W();
		}

		public override void OnLaneClear()
		{
			if(IsSpellActive("Q") && ManaManagerAllowCast() )
				Cast_BasicSkillshot_AOE_Farm(Q);
			if(IsSpellActive("W") && ManaManagerAllowCast())
			{
				var minions = MinionManager.GetMinions(MyHero.ServerPosition, W.Range, MinionTypes.All, MinionTeam.NotAlly);
				if(minions.Any())
				{
					Cast_W(true);
				}
			}
		}

		public override void OnFlee()
		{
			if(IsSpellActive("E"))
				Cast_BasicSkillshot_Enemy(E, SimpleTs.DamageType.Magical);
			if(IsSpellActive("R"))
				if(Game.CursorPos.Distance(MyHero.Position) > R.Range && R.IsReady() && EnemysinRange(1500))
				{
					R.UpdateSourcePosition();
					R.Cast(Game.CursorPos, UsePackets());
				}
		}

		private void Cast_W(bool ignoreTargetCheck = false)
		{
			if(!W.IsReady())
				return;

			var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);

			if(target != null || ignoreTargetCheck)
				W.Cast();
		}

		private void Cast_R()
		{
			if(!R.IsReady())
				return;
			if (ShouldUlt())
				R.Cast(Game.CursorPos,UsePackets());
		}
		private bool ShouldUlt()
		{
			if(EnemysinRange(500))
				return true;
			var mousePos = Game.CursorPos;
			var enemiesNearMouse = AllHerosEnemy.Where(x => x.Distance(MyHero) < R.Range && x.Distance(mousePos) < 650);
			var objAiHeroes = enemiesNearMouse as Obj_AI_Hero[] ?? enemiesNearMouse.ToArray();
			if(!objAiHeroes.Any())
				return false;
			if(IsRActive())
				return true;
			var enoughMana = MyHero.Mana > MyHero.Spellbook.GetSpell(SpellSlot.Q).ManaCost + MyHero.Spellbook.GetSpell(SpellSlot.E).ManaCost + MyHero.Spellbook.GetSpell(SpellSlot.R).ManaCost;
			if(Menu.Item("Combo_useR_onUseActivate").GetValue<bool>() || !(Q.IsReady() && E.IsReady()) || !enoughMana)
				return false;
			var friendsNearMouse = AllHerosFriend.Where(x => x.IsMe || x.Distance(mousePos) < 650);

			if(objAiHeroes.Count() == 1)
			{
				var enemy = objAiHeroes.FirstOrDefault();
				var underTower = Utility.UnderTurret(enemy);
				return enemy != null && GetComboDamage(enemy) / enemy.Health >= (underTower ? 1.25f : 1);
			}
			var lowHealthEnemies = objAiHeroes.Count(x => x.Health / x.MaxHealth <= 0.1);
			var totalEnemyHealth = objAiHeroes.Sum(x => x.Health);
			return friendsNearMouse.Count() - (objAiHeroes.Count() - lowHealthEnemies) >= -1 || MyHero.Health / totalEnemyHealth >= 0.8;
		}

		private bool IsRActive()
		{
			return MyHero.HasBuff("AhriTumble", true);
		}

	}
}

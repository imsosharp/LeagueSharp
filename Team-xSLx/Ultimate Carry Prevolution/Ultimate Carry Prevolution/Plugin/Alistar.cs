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
	class Alistar : Champion
	{
		private bool _moveOrder;
		private int _movetimer;
		private bool _qonDelay;
		public Alistar()
		{
			SetSpells();
			LoadMenu();
		}

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q, 182);

			W = new Spell(SpellSlot.W, 650);
			W.SetTargetted(500, float.MaxValue);

			E = new Spell(SpellSlot.E, 287);

			R = new Spell(SpellSlot.R);
		}

		private void LoadMenu()
		{
			var champMenu = new Menu("Alistar Plugin", "Alistar");
			{
				var comboMenu = new Menu("Combo", "Combo");
				{
					AddSpelltoMenu(comboMenu, "Q", true);
					AddSpelltoMenu(comboMenu, "W", true);
					AddSpelltoMenu(comboMenu, "R", true);
					comboMenu.AddItem(new MenuItem("Combo_useR_enemyCount", "Use R if x Enemys Arround")).SetValue(new Slider(2, 1, 5));
					comboMenu.AddItem(new MenuItem("Combo_useR_Health", "Use R if health below").SetValue(new Slider(70)));
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
					AddSpelltoMenu(laneClearMenu, "Q", true);
					laneClearMenu.AddItem(new MenuItem("LaneClear_useQ_minHit", "Use Q if min. hit").SetValue(new Slider(2, 1, 6)));
					AddManaManagertoMenu(laneClearMenu, 20);
					champMenu.AddSubMenu(laneClearMenu);
				}

				var miscMenu = new Menu("Misc", "Misc");
				{
					miscMenu.AddItem(new MenuItem("Misc_useE_Health", "Use E Heal at ").SetValue(new Slider(70)));
					miscMenu.AddItem(new MenuItem("Misc_useE_Friends", "Use E for Friends").SetValue(true));
					miscMenu.AddItem(new MenuItem("Misc_useQ_AntiGapClose", "Use Q for AntiGapClose").SetValue(true));
					miscMenu.AddItem(new MenuItem("Misc_useQ_Interrupt", "Use Q for Interrupt").SetValue(true));
					miscMenu.AddItem(new MenuItem("Misc_useW_AntiGapClose", "Use W for AntiGapClose").SetValue(true));
					miscMenu.AddItem(new MenuItem("Misc_useW_Interrupt", "Use W for Interrupt").SetValue(true));
					champMenu.AddSubMenu(miscMenu);
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

		public override void OnGapClose(ActiveGapcloser gapcloser)
		{
			if(gapcloser.Sender.IsAlly)
				return;

			if (Menu.Item("Misc_useQ_AntiGapClose").GetValue<bool>())
				Q.Cast();

			if (Menu.Item("Misc_useW_AntiGapClose").GetValue<bool>())
				W.Cast(gapcloser.Sender, UsePackets());
		}

		public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
		{
			if(spell.DangerLevel < InterruptableDangerLevel.Medium  || unit.IsAlly)
				return;

			if (Menu.Item("Misc_useQ_Interrupt").GetValue<bool>())
				Q.Cast();

			if (Menu.Item("Misc_useW_Interrupt").GetValue<bool>())
				W.Cast(unit, UsePackets());
		}
		public override void OnPassive()
		{
			CheckHeals();		
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
					Utility.DrawCircle(MyHero.Position, Q.Range * 2, Q.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_W").GetValue<bool>())
				if(W.Level > 0)
					Utility.DrawCircle(MyHero.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_E").GetValue<bool>())
				if(E.Level > 0)
					Utility.DrawCircle(MyHero.Position, E.Range*2, E.IsReady() ? Color.Green : Color.Red);
		}

		public override void OnCombo()
		{
			if (IsSpellActive("Q") && IsSpellActive("W"))
				Cast_QW_Combo();
			else
			{
				if (IsSpellActive("Q"))
					Cast_Q(true);
				if (IsSpellActive("W"))
					Cast_W();
			}
			if(IsSpellActive("R"))
				Cast_R();
		}

		public override void OnHarass()
		{
			if(IsSpellActive("Q") && ManaManagerAllowCast())
				Cast_Q(true);
			if(IsSpellActive("W") && ManaManagerAllowCast())
				Cast_W();
		}

		public override void OnLaneClear()
		{
			if(IsSpellActive("Q") && ManaManagerAllowCast())
				Cast_Q(false);
		}

		private void Cast_Q(bool mode)
		{
			if(!Q.IsReady())
				return;
			if(mode)
			{
				var target = SimpleTs.GetTarget(Q.Range * 2, SimpleTs.DamageType.Magical);
				if(target == null || !target.IsValidTarget(Q.Range))
					return;
				Q.Cast();
			}
			else
			{
				if(MinionManager.GetMinions(MyHero.Position, Q.Range * 2).Count >= Menu.Item("LaneClear_useQ_minHit").GetValue<Slider>().Value)
					Q.Cast();
				foreach(var minion in MinionManager.GetMinions(MyHero.ServerPosition, Q.Range, MinionTypes.All,
					MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Where(minion => MyHero.Distance(minion) <= Q.Range))
					Q.Cast();
			}
		}

		private void Cast_W()
		{
			if(!W.IsReady())
				return;
			var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
			if(target != null)
				W.Cast(target, UsePackets());
		}

		private void Cast_QW_Combo()
		{
			if (_qonDelay)
			{
				if (!Q.IsReady())
					_qonDelay = false;
				return;
			}
			if(_moveOrder)
			{
				if(Environment.TickCount - _movetimer > 2000)
				{
					xSLxOrbwalker.CustomOrbwalkMode = false;
					xSLxOrbwalker.SetAttack(true);
					_moveOrder = false;
					return;
				}
				var reached = Runto(GetMovePosition(SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical)));
				if(reached)
				{
					W.Cast(SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical),UsePackets());
					return;
				}
			}
			else
			{
				xSLxOrbwalker.CustomOrbwalkMode = false;
				xSLxOrbwalker.SetAttack(true);
			}
			if(Q.IsReady() && W.IsReady())
			{
				var enoughMana = MyHero.Mana > MyHero.Spellbook.GetSpell(SpellSlot.Q).ManaCost + MyHero.Spellbook.GetSpell(SpellSlot.W).ManaCost;
				if(enoughMana)
				{
					var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
					if(target.IsValidTarget(Q.Range))
					{
						Q.Cast();
						_moveOrder = true;
						_movetimer = Environment.TickCount;
						return;
					}
					if (target.IsValidTarget( W.Range))
					{
						W.Cast(target, UsePackets());
						//var jumpTime = Math.Max(1, 1000 * MyHero.Distance(target) / 2500) ;
						var jumpTime = Math.Max(0, MyHero.Distance(target) - 500) * 10 / 25 + 25 ;
						Utility.DelayAction.Add((int)jumpTime, () => Q.Cast());
						_qonDelay = true;
						return;
					}
				}
			}
			if (W.IsReady() && !Q.IsReady())
			{
				var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
				foreach (var friend in AllHerosFriend.Where(hero => !hero.IsMe && xSLxOrbwalker.GetAutoAttackRange(target,hero) > hero.Distance( target) && GetHealthPercent( hero) < GetHealthPercent( target) && GetHealthPercent( hero ) < 20))
				{
					W.Cast(target,UsePackets());
				}
			}
			if(Q.IsReady() && !W.IsReady())
			{
				var target = SimpleTs.GetTarget(Q.Range*2, SimpleTs.DamageType.Magical);
				Q.Cast();			
			}
		}

		private bool Runto(Vector3 position)
		{
			if (position == default (Vector3))
				return true;
			if (MyHero.Position.Distance(position) > 15)
			{
				xSLxOrbwalker.CustomOrbwalkMode = true;
				xSLxOrbwalker.SetAttack(false);
				MyHero.IssueOrder(GameObjectOrder.MoveTo, position);
				return false;
			}
			xSLxOrbwalker.CustomOrbwalkMode = false;
			xSLxOrbwalker.SetAttack(true);
			return true;
		}

		private Vector3 GetMovePosition(Obj_AI_Base target)
		{
			foreach(var tower in ObjectManager.Get<Obj_AI_Turret>().Where(tower => tower.IsAlly && tower.Health > 0 && tower.Distance(MyHero) < 1500))
			{
				return V3E(target.Position, tower.Position, -50);
			}
			foreach(var friend in AllHerosFriend.Where(hero => !hero.IsMe && hero.Distance(MyHero) < 800 + hero.AttackRange && GetHealthPercent(hero) > 30))
			{
				return V3E(target.Position, friend.Position, -50);
			}
			return default(Vector3);
		}

		private void Cast_R()
		{
			if(Utility.CountEnemysInRange(500) >= Menu.Item("Combo_useR_enemyCount").GetValue<Slider>().Value &&
				GetHealthPercent() <= Menu.Item("Combo_useR_Health").GetValue<Slider>().Value)
				R.Cast();
		}

		private void CheckHeals()
		{
			if (MyHero.HasBuff("Recall") || Utility.InShopRange() || Utility.InFountain())
						return;
			if(Menu.Item("Misc_useE_Friends").GetValue<bool>())
			{
				foreach(
					var friend in
						AllHerosFriend.Where(
							hero =>
								GetHealthPercent(hero) < Menu.Item("Misc_useE_Health").GetValue<Slider>().Value &&
								hero.Distance(MyHero) < E.Range && hero.IsValid))
				{
					E.Cast();
				}
			}
			else
			{
				if (GetHealthPercent(MyHero) < Menu.Item("Misc_useE_Health").GetValue<Slider>().Value)
					E.Cast();
			}
		}
	}
}

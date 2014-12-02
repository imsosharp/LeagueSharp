using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using xSLx_Orbwalker;

namespace Ultimate_Carry_Prevolution.Plugin
{
	internal class Annie : Champion
	{
		public Annie()
		{
			SetSpells();
			LoadMenu();
		}

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q, 650);
			Q.SetTargetted(250, 1400);

			W = new Spell(SpellSlot.W, 625);
			W.SetSkillshot(600, (float) (50*Math.PI/180), float.MaxValue, false, SkillshotType.SkillshotCone);

			E = new Spell(SpellSlot.E);

			R = new Spell(SpellSlot.R, 600);
			R.SetSkillshot(250, 200, float.MaxValue, false, SkillshotType.SkillshotCircle);
		}

		private void LoadMenu()
		{
			var champMenu = new Menu("Annie Plugin", "Annie");
			{
				var comboMenu = new Menu("Combo", "Combo");
				{
					AddSpelltoMenu(comboMenu, "Q", true);
					AddSpelltoMenu(comboMenu, "W", true);
					AddSpelltoMenu(comboMenu, "E", true);
					AddSpelltoMenu(comboMenu, "R", true);
					comboMenu.AddItem(new MenuItem("R_StunCount", "Use R if Stun # Enemys").SetValue(new Slider(2, 1, 5)));
					champMenu.AddSubMenu(comboMenu);
				}
				var harassMenu = new Menu("Harass", "Harass");
				{
					AddSpelltoMenu(harassMenu, "Q", true);
					AddSpelltoMenu(harassMenu, "W", true);
					harassMenu.AddItem(new MenuItem("Q_Lasthit_Harass", "Use Q LastHit").SetValue(true));
					harassMenu.AddItem(new MenuItem("Q_Lasthit_Harass_stun", "Use Q on minion if stun up").SetValue(false));
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

				var lasthitmenu = new Menu("Lasthit", "Lasthit");
				{
					lasthitmenu.AddItem(new MenuItem("Q_Lasthit_Lasthit", "Use Q LastHit").SetValue(true));
					lasthitmenu.AddItem(new MenuItem("Q_Lasthit_Lasthit_stun", "Use Q on minion if stun up").SetValue(true));
					champMenu.AddSubMenu(lasthitmenu);
				}

				var miscMenu = new Menu("Misc", "Misc");
				{
					miscMenu.AddItem(new MenuItem("E_AgainAA", "Cast E Against AA").SetValue(true));
					miscMenu.AddItem(new MenuItem("E_InBase", "Cast E while in Base").SetValue(true));
					miscMenu.AddItem(new MenuItem("Q_Interrupt", "Use Q/Stun Anti GapClose").SetValue(true));
					miscMenu.AddItem(new MenuItem("W_Interrupt", "Use W/Stun Anti GapClose").SetValue(true));
					miscMenu.AddItem(new MenuItem("R_Interrupt", "Use R/Stun Anti GapClose").SetValue(true));
					miscMenu.AddItem(new MenuItem("Q_GapClose", "Use Q/Stun Anti GapClose").SetValue(true));
					miscMenu.AddItem(new MenuItem("W_GapClose", "Use W/Stun Anti GapClose").SetValue(true));
					miscMenu.AddItem(new MenuItem("R_GapClose", "Use R/Stun Anti GapClose").SetValue(true));
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

		private float GetComboDamage(Obj_AI_Base enemy)
		{
			var damage = 0d;

			if (Q.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.Q);

			if (W.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.W);

			if (R.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.R);

			return (float) damage;
		}

		public override void OnAttack(Obj_AI_Base unit, Obj_AI_Base target)
		{
			if(unit.IsEnemy && !unit.IsMinion && target.IsMe && Menu.Item("E_AgainAA").GetValue<bool>() && E.IsReady())
				E.Cast();
			if(xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.LaneClear && unit.IsEnemy && target.IsMe && Menu.Item("E_AgainAA").GetValue<bool>() && E.IsReady())
				E.Cast();
		}

		public override void OnGapClose(ActiveGapcloser gapcloser)
		{
			if (!gapcloser.Sender.IsEnemy)
				return;

			var buffstate = GetPassiveStacks();
			var myMana = MyHero.Mana;
			var qManaCost = MyHero.Spellbook.GetSpell(SpellSlot.Q).ManaCost;
			var wManaCost = MyHero.Spellbook.GetSpell(SpellSlot.W).ManaCost;
			var eManaCost = MyHero.Spellbook.GetSpell(SpellSlot.E).ManaCost;
			var rManaCost = MyHero.Spellbook.GetSpell(SpellSlot.R).ManaCost;
			var canCastEq = myMana > qManaCost + eManaCost;
			var canCastEw = myMana > wManaCost + eManaCost;
			var canCastEr = myMana > rManaCost + eManaCost;

			var unit = gapcloser.Sender;
			switch (buffstate)
			{
				case 4:
				{
					if (W.IsReady() && Menu.Item("W_GapClose").GetValue<bool>() && unit.IsValidTarget(W.Range))
					{
						W.Cast(unit, UsePackets());
						return;
					}
					if (Q.IsReady() && Menu.Item("Q_GapClose").GetValue<bool>() && unit.IsValidTarget(Q.Range))
					{
						Q.CastOnUnit(unit, UsePackets());
						return;
					}
					if (R.IsReady() && Menu.Item("R_GapClose").GetValue<bool>() && unit.IsValidTarget(R.Range))
					{
						R.Cast(unit, UsePackets());
					}
				}
					break;
				case 3:
				{
					if (!E.IsReady())
						return;
					if (W.IsReady() && Menu.Item("W_GapClose").GetValue<bool>() && unit.IsValidTarget(W.Range) && canCastEw)
					{
						E.Cast();
						Utility.DelayAction.Add(100, () => W.Cast(unit, UsePackets()));
						return;
					}
					if (Q.IsReady() && Menu.Item("Q_GapClose").GetValue<bool>() && unit.IsValidTarget(Q.Range) && canCastEq)
					{
						E.Cast();
						Utility.DelayAction.Add(100, () => Q.CastOnUnit(unit, UsePackets()));
						return;
					}
					if (R.IsReady() && Menu.Item("R_GapClose").GetValue<bool>() && unit.IsValidTarget(R.Range) && canCastEr)
					{
						E.Cast();
						Utility.DelayAction.Add(100, () => R.Cast(unit, UsePackets()));
					}
				}
					break;
			}
		}

		public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
		{
			var buffstate = GetPassiveStacks();
			var myMana = MyHero.Mana;
			var qManaCost = MyHero.Spellbook.GetSpell(SpellSlot.Q).ManaCost;
			var wManaCost = MyHero.Spellbook.GetSpell(SpellSlot.W).ManaCost;
			var eManaCost = MyHero.Spellbook.GetSpell(SpellSlot.E).ManaCost;
			var rManaCost = MyHero.Spellbook.GetSpell(SpellSlot.R).ManaCost;
			var canCastEq = myMana > qManaCost + eManaCost;
			var canCastEw = myMana > wManaCost + eManaCost;
			var canCastEr = myMana > rManaCost + eManaCost;
			if (!unit.IsEnemy)
				return;
			switch (buffstate)
			{
				case 4:
				{
					if (W.IsReady() && Menu.Item("W_Interrupt").GetValue<bool>() && unit.IsValidTarget(W.Range))
					{
						W.Cast(unit, UsePackets());
						return;
					}
					if (Q.IsReady() && Menu.Item("Q_Interrupt").GetValue<bool>() && unit.IsValidTarget(Q.Range))
					{
						Q.CastOnUnit(unit, UsePackets());
						return;
					}
					if (R.IsReady() && Menu.Item("R_Interrupt").GetValue<bool>() && unit.IsValidTarget(R.Range) &&
					    spell.DangerLevel == InterruptableDangerLevel.High)
					{
						R.Cast(unit, UsePackets());
					}
				}
					break;
				case 3:
				{
					if (!E.IsReady())
						return;
					if (W.IsReady() && Menu.Item("W_Interrupt").GetValue<bool>() && unit.IsValidTarget(W.Range) && canCastEw)
					{
						E.Cast();
						Utility.DelayAction.Add(100, () => W.Cast(unit, UsePackets()));
						return;
					}
					if (Q.IsReady() && Menu.Item("Q_Interrupt").GetValue<bool>() && unit.IsValidTarget(Q.Range) && canCastEq)
					{
						E.Cast();
						Utility.DelayAction.Add(100, () => Q.CastOnUnit(unit, UsePackets()));
						return;
					}
					if (R.IsReady() && Menu.Item("R_Interrupt").GetValue<bool>() && unit.IsValidTarget(R.Range) && canCastEr &&
					    spell.DangerLevel == InterruptableDangerLevel.High)
					{
						E.Cast();
						Utility.DelayAction.Add(100, () => R.Cast(unit, UsePackets()));
					}
				}
					break;
			}
		}

		public override void OnPassive()
		{
			if(!Menu.Item("E_InBase").GetValue<bool>() || !E.IsReady())
				return;
			if(Utility.InShopRange() && GetPassiveStacks() <= 3)
				E.Cast();
		}

		public override void OnCombo()
		{
			if (IsSpellActive("R"))
				Cast_R();
			if (IsSpellActive("Q"))
				Cast_Q();
			if (IsSpellActive("E"))
				Cast_E();
			if (IsSpellActive("W"))
				Cast_W();
		}


		public override void OnLaneClear()
		{
			if(IsSpellActive("Q"))
				Cast_Q();
			if(IsSpellActive("W") && ManaManagerAllowCast())
				Cast_W();
		}

		public override void OnHarass()
		{
			if (IsSpellActive("Q"))
				Cast_Q();
			if (IsSpellActive("W") && ManaManagerAllowCast() )
				Cast_W();

		}

		public override void OnLasthit()
		{
			Cast_Q();
		}

		private void Cast_Q()
		{
			if (!Q.IsReady())
				return;
			
			if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Combo || xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Harass)
			{
				if (!Q.IsReady() || (!ManaManagerAllowCast() &&xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Combo) )
					return;
				var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
				if (target != null)
				{
					Q.CastOnUnit(target, UsePackets());
					return;
				}
			}
			if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Harass && Menu.Item("Q_Lasthit_Harass").GetValue<bool>())
			{
				if (GetPassiveStacks() == 4 && !Menu.Item("Q_Lasthit_Harass_stun").GetValue<bool>())
					return;
				var allMinions =
					MinionManager.GetMinions(MyHero.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly).ToList();
				var minionLastHit =
					allMinions.Where(
						x =>
							HealthPrediction.LaneClearHealthPrediction(x, (int) Q.Delay) < MyHero.GetSpellDamage(x, SpellSlot.Q)*0.8)
						.OrderBy(x => x.Health);

				if (!minionLastHit.Any())
					return;
				var unit = minionLastHit.First();
				Q.CastOnUnit(unit, UsePackets());
			}

			if(xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Lasthit && Menu.Item("Q_Lasthit_Lasthit").GetValue<bool>())
			{
				if(GetPassiveStacks() == 4 && !Menu.Item("Q_Lasthit_Lasthit_stun").GetValue<bool>())
					return;
				var allMinions =
					MinionManager.GetMinions(MyHero.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly).ToList();
				var minionLastHit = allMinions.Where(x => HealthPrediction.LaneClearHealthPrediction(x, (int)Q.Delay ) < MyHero.GetSpellDamage(x, SpellSlot.Q) * 0.8).OrderBy(x => x.Health);


				if(!minionLastHit.Any())
					return;
				var unit = minionLastHit.First();
				Q.CastOnUnit(unit, UsePackets());
			}

			if(xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.LaneClear )
			{
				var allMinions =MinionManager.GetMinions(MyHero.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly).ToList();
				var minionLastHit =
					allMinions.Where(
						x =>
							HealthPrediction.LaneClearHealthPrediction(x, (int)Q.Delay) < MyHero.GetSpellDamage(x, SpellSlot.Q) * 0.8)
						.OrderBy(x => x.Health);

				if(!minionLastHit.Any())
					return;
				var unit = minionLastHit.First();
				Q.CastOnUnit(unit, UsePackets());
			}
		}
	

	private void Cast_W()
	{
		if (!W.IsReady())
			return;
		if(xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Combo || xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Harass)
		{
			if(!W.IsReady())
				return;
			var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
			if(target != null)
				W.Cast(target, UsePackets());
		}
		else
		{
			var minions = MinionManager.GetMinions(MyHero.ServerPosition, W.Range, MinionTypes.All, MinionTeam.NotAlly).ToList();
			if(minions.Count <= 0)
				return;
			var farm = W.GetLineFarmLocation(minions, W.Width);
			if(farm.MinionsHit >= 3 || minions.Any(x => x.Team == GameObjectTeam.Neutral))
				W.Cast(farm.Position, UsePackets());
		}
	}

		private void Cast_E()
		{
			if(GetPassiveStacks() < 4)
				E.Cast();
		}

		private void Cast_R()
		{
			if(!R.IsReady())
				return;
			var killableEnemy = AllHerosEnemy.FirstOrDefault(enemy => enemy.IsValidTarget(R.Range) && MyHero.GetSpellDamage(enemy, SpellSlot.R) *0.9>= enemy.Health);
			if(killableEnemy != null)
				R.Cast(killableEnemy, UsePackets());
			var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);
			if(GetComboDamage(target) > target.Health && GetPassiveStacks() == 4)
				R.Cast(target, UsePackets());
			if(GetPassiveStacks() == 4)
				R.CastIfWillHit(target, Menu.Item("R_StunCount").GetValue<Slider>().Value, UsePackets());
		}

		private int GetPassiveStacks()
		{
			var buffs = MyHero.Buffs.Where(buff => (buff.Name.ToLower() == "pyromania" || buff.Name.ToLower() == "pyromania_particle"));
			var buffInstances = buffs as BuffInstance[] ?? buffs.ToArray();
			if(!buffInstances.Any())
				return 0;
			var buf = buffInstances.First();
			var count = buf.Count >= 4 ? 4 : buf.Count;
			return buf.Name.ToLower() == "pyromania_particle" ? 4 : count;
		}
	}
}
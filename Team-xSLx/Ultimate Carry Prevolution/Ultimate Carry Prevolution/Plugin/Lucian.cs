using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
	internal class Lucian : Champion
	{
		private const int QMaxRange = 1100;
		private int _passivTimer;
		private bool _canUseSpells = true;
		private bool _waitingForBuff;
		private bool _gainBuff;

		public Lucian()
		{
			SetSpells();
			LoadMenu();
		}

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q, 675);
			Q.SetTargetted(0.5f, float.MaxValue);

			Q2 = new Spell(SpellSlot.Q, 1100);
			Q2.SetSkillshot(0.5f, 5f, float.MaxValue, true, SkillshotType.SkillshotLine);

			W = new Spell(SpellSlot.W, 1000);
			W.SetSkillshot(300, 80, 1600, true, SkillshotType.SkillshotLine);

			E = new Spell(SpellSlot.E, 475);
			E.SetSkillshot(250, 1, float.MaxValue, false, SkillshotType.SkillshotLine);

			
			R = new Spell(SpellSlot.R, 1400);
			R.SetSkillshot(10, 110, 2800, true, SkillshotType.SkillshotLine);

		}

		private void LoadMenu()
		{
			var champMenu = new Menu("Lucian Plugin", "Lucian");
			{
				var comboMenu = new Menu("Combo", "Combo");
				{
					AddSpelltoMenu(comboMenu, "Q", true);
					AddSpelltoMenu(comboMenu, "W", true);
					AddSpelltoMenu(comboMenu, "E", true);
					comboMenu.AddItem(new MenuItem("Combo_useR_Filler", "Use R if no Spells up")).SetValue(true);
					comboMenu.AddItem(new MenuItem("Combo_useR_Kill", "Use R if out of Range could kill")).SetValue(true);
					
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
					AddSpelltoMenu(laneClearMenu, "W", true);
					AddSpelltoMenu(laneClearMenu, "E", true);
					AddManaManagertoMenu(laneClearMenu, 20);
					champMenu.AddSubMenu(laneClearMenu);
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

		private float GetComboDamage(Obj_AI_Base target)
		{
			var comboDamage = 0d;
			if (Q.IsReady())
			{
				comboDamage += MyHero.GetSpellDamage(target, SpellSlot.Q);
				comboDamage += MyHero.GetAutoAttackDamage(target)*1.5;
			}
			if (W.IsReady())
			{
				comboDamage += MyHero.GetSpellDamage(target, SpellSlot.W);
				comboDamage += MyHero.GetAutoAttackDamage(target)*1.5;
			}
			if (E.IsReady())
			{
				comboDamage += MyHero.GetAutoAttackDamage(target)*1.5;
			}
			if (R.IsReady())
			{
				comboDamage += MyHero.GetSpellDamage(target, SpellSlot.R)*0.3;
				comboDamage += MyHero.GetAutoAttackDamage(target)*1.5;
			}

			return (float) (comboDamage + MyHero.GetAutoAttackDamage(target));
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
				{
					Utility.DrawCircle(MyHero.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);
					Utility.DrawCircle(MyHero.Position, QMaxRange, Q.IsReady() ? Color.Green : Color.Red);
				}

			if(Menu.Item("Draw_E").GetValue<bool>())
				if(E.Level > 0)
					Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_R").GetValue<bool>())
				if(R.Level > 0)
					Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);
		}

		public override void OnPassive()
		{
			BuffCheck();
			UltCheck();
		}
		private void UltCheck()
		{
			var tempultactive = false;
			foreach(var buff in MyHero.Buffs.Where(buff => buff.Name == "LucianR"))
				tempultactive = true;

			if(tempultactive)
			{
				xSLxOrbwalker.SetAttack(false);
			}
			if(!tempultactive)
			{
				xSLxOrbwalker.SetAttack(true);
			}
		}

		private void BuffCheck()
		{
			if(_canUseSpells == false && _waitingForBuff == false && _gainBuff == false)
				_waitingForBuff = true;

			if(_waitingForBuff)
				foreach(var buff in MyHero.Buffs.Where(buff => buff.Name == "lucianpassivebuff"))
					_gainBuff = true;

			if(_gainBuff)
			{
				_waitingForBuff = false;
				var tempgotBuff = false;
				foreach(var buff in MyHero.Buffs.Where(buff => buff.Name == "lucianpassivebuff"))
					tempgotBuff = true;
				if(tempgotBuff == false)
				{
					_gainBuff = false;
					_canUseSpells = true;
				}
			}

			if(_passivTimer >= Environment.TickCount - 1000 || _waitingForBuff != true)
				return;
			_waitingForBuff = false;
			_gainBuff = false;
			_canUseSpells = true;
		}

		public override void ObjSpellMissileOnOnDelete(GameObject sender, EventArgs args)
		{
			if(sender.Name == "Lucian_P_buf.troy")
			{
				_waitingForBuff = false;
				_gainBuff = false;
				_canUseSpells = true;
			}
		}

		public override void ObjSpellMissileOnOnCreate(GameObject sender, EventArgs args)
		{
			if(sender.Name == "Lucian_P_buf.troy")
			{
				_canUseSpells = false;
				_passivTimer = Environment.TickCount;
			}
		}

		public override void OnCombo()
		{
			if(IsSpellActive("E") )
				Cast_E(true);
			if(IsSpellActive("Q") )
				Cast_Q(true);
			if(IsSpellActive("W"))
				Cast_W();
			if(Menu.Item("Combo_useR_Filler").GetValue<bool>() )
				Cast_R(1);
			if(Menu.Item("Combo_useR_Kill").GetValue<bool>())
				Cast_R(2);
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
			if(IsSpellActive("W") && ManaManagerAllowCast())
				Cast_BasicSkillshot_AOE_Farm(W, 220);
			if(IsSpellActive("E"))
				Cast_E(false);

		}
		private void UsedSkill()
		{
			_canUseSpells = false;
			_passivTimer = Environment.TickCount;
		}

		private void Cast_Q(bool mode)
		{
			if(!Q.IsReady() || !_canUseSpells)
				return;
			if (mode)
			{
				var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);

				if(target != null)
				{
					if((target.IsValidTarget(Q.Range)))
					{
						Q.Cast(target, UsePackets());
						UsedSkill();
						return;
					}
				}
				target = SimpleTs.GetTarget(Q2.Range, SimpleTs.DamageType.Physical);
				if(target == null)
					return;
				if((!target.IsValidTarget(Q2.Range)) || !_canUseSpells || !Q.IsReady())
					return;
				var qCollision = Q2.GetPrediction(target).CollisionObjects;
				foreach(var qCollisionChar in qCollision.Where(qCollisionChar => qCollisionChar.IsValidTarget(Q.Range)))
				{
					UsedSkill();
					Q.Cast(qCollisionChar, UsePackets());
				}
			}
			else
			{
				var allMinions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
				var minion = allMinions.FirstOrDefault(minionn => minionn.Distance(MyHero) <= Q.Range && HealthPrediction.LaneClearHealthPrediction(minionn, 500) > 0);
				if(minion == null)
					return;
				_passivTimer = Environment.TickCount;
				Q.CastOnUnit(minion, UsePackets());
			}

		}

		private void Cast_W()
		{
			if(!W.IsReady() || !_canUseSpells)
				return;
			var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
			if(target == null)
				return;
			if(target.IsValidTarget(W.Range) && W.GetPrediction(target).Hitchance >= HitChance.High)
			{
				UsedSkill();
				W.Cast(target, UsePackets());
			}
			else if(W.GetPrediction(target).Hitchance == HitChance.Collision)
			{
				var wCollision = W.GetPrediction(target).CollisionObjects;
				foreach(var wCollisionChar in wCollision.Where(wCollisionChar => wCollisionChar.Distance(target) <= 100))
				{
					UsedSkill();
					W.Cast(wCollisionChar.Position, UsePackets());
				}
			}
		}
		private void Cast_E(bool mode)
		{
			if(!E.IsReady() || !_canUseSpells)
				return;
			if (mode)
			{
				var target = SimpleTs.GetTarget(1100, SimpleTs.DamageType.Physical);
				if (target == null)
					return;
				_passivTimer = Environment.TickCount;
				UsedSkill();
				E.Cast(Game.CursorPos, UsePackets());
			}
			else
			{
				var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 1100, MinionTypes.All,
						MinionTeam.NotAlly);
				if(!allMinions.Where(minion => minion != null).Any(minion => minion.IsValidTarget(1100) && E.IsReady()))
					return;
				_passivTimer = Environment.TickCount;
				UsedSkill();
				E.Cast(Game.CursorPos, UsePackets());
			}

		}

		private void Cast_R(int mode)
		{
			if(!R.IsReady() || !_canUseSpells)
				return;
			switch(mode)
			{
				case 1:
					if(Q.IsReady() || W.IsReady() || E.IsReady())
						return;
					var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical);
					if(target.IsValidTarget(R.Range))
					{
						_passivTimer = Environment.TickCount;
						UsedSkill();
						R.Cast(target, UsePackets());
					}
					break;
				case 2:
					target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical);
					if (W.IsReady() && target.IsValidTarget(W.Range))
						return;
					if(W.IsReady() && target.IsValidTarget(W.Range))
						return;
					if(Q.IsReady() && target.IsValidTarget(Q.Range))
						return;
					if(E.IsReady() && target.IsValidTarget(xSLxOrbwalker.GetAutoAttackRange(MyHero,target) + E.Range))
						return;
					if (target.IsValidTarget(xSLxOrbwalker.GetAutoAttackRange(MyHero, target) + E.Range))
						return;
					if (target.Health < MyHero.GetSpellDamage(target, SpellSlot.R)*0.4)
					{
						_passivTimer = Environment.TickCount;
						UsedSkill();
						R.Cast(target, UsePackets());
					}
					break;
			}
		}
	}
}


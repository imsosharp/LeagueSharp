using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;
using Menu = LeagueSharp.Common.Menu;
using MenuItem = LeagueSharp.Common.MenuItem;

namespace Ultimate_Carry_Prevolution.Plugin
{
	class Syndra : Champion
	{
		public Syndra()
		{
			SetSpells();
			LoadMenu();
		}

		//in progress

		private Spell _qe;

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q, 800);
			Q.SetSkillshot(600f, 130f, 2000f, false, SkillshotType.SkillshotCircle);

			W = new Spell(SpellSlot.W, 925);
			W.SetSkillshot(0, 140f, 2000f, false, SkillshotType.SkillshotCircle);

			E = new Spell(SpellSlot.E, 700);
			E.SetSkillshot(300f, (float)(45 * 0.5), 2500f, false, SkillshotType.SkillshotCircle);

			R = new Spell(SpellSlot.R, 750);

			_qe = new Spell(SpellSlot.Q, 1250);
			_qe.SetSkillshot(900f, 60f, 2000f, false, SkillshotType.SkillshotLine);

		}

		private void LoadMenu()
		{
			var champMenu = new Menu("Syndra Plugin", "Syndra");
			{
				var spellMenu = new Menu("SpellMenu", "SpellMenu");
				{
					var qMenu = new Menu("QMenu", "QMenu");
					{
						qMenu.AddItem(new MenuItem("Q_Auto_Immobile", "Auto Q on Immobile").SetValue(true));
						spellMenu.AddSubMenu(qMenu);
					}

					var wMenu = new Menu("WMenu", "WMenu");
					{
						wMenu.AddItem(new MenuItem("W_Only_Orb", "Only Pick Up Orb").SetValue(false));
						spellMenu.AddSubMenu(wMenu);
					}
					var rMenu = new Menu("RMenu", "RMenu");
					{
						rMenu.AddItem(new MenuItem("R_Overkill_Check", "Overkill Check").SetValue(true));

						rMenu.AddSubMenu(new Menu("Don't use R on", "Dont_R"));
						foreach(var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != MyHero.Team)
						)
							rMenu.SubMenu("Dont_R")
								.AddItem(new MenuItem("Dont_R" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(false));

						spellMenu.AddSubMenu(rMenu);
					}

					champMenu.AddSubMenu(spellMenu);
				}

				var comboMenu = new Menu("Combo", "Combo");
				{
					AddSpelltoMenu(comboMenu, "Q", true);
					AddSpelltoMenu(comboMenu, "QE", true, "Use QE");
					AddSpelltoMenu(comboMenu, "W", true);
					AddSpelltoMenu(comboMenu, "E", true);
					AddSpelltoMenu(comboMenu, "R", true);
					AddSpelltoMenu(comboMenu, "Ignite", true, "Use Ignite");
					champMenu.AddSubMenu(comboMenu);
				}

				var harassMenu = new Menu("Harass", "Harass");
				{
					AddSpelltoMenu(harassMenu, "Q", true);
					harassMenu.AddItem(new MenuItem("Q_Auto_Harass", "Q Harass Toggle").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Press)));
					AddSpelltoMenu(harassMenu, "QE", true, "Use QE");
					AddSpelltoMenu(harassMenu, "W", true);
					AddSpelltoMenu(harassMenu, "E", true);
					AddManaManagertoMenu(harassMenu, 30);
					champMenu.AddSubMenu(harassMenu);
				}

				var laneClearMenu = new Menu("LaneClear", "LaneClear");
				{
					AddSpelltoMenu(laneClearMenu, "Q", true);
					AddSpelltoMenu(laneClearMenu, "W", true);
					AddSpelltoMenu(laneClearMenu, "E", true);
					AddManaManagertoMenu(laneClearMenu, 30);
					champMenu.AddSubMenu(laneClearMenu);
				}

				var miscMenu = new Menu("Misc", "Misc");
				{
					miscMenu.AddItem(new MenuItem("Misc_QE_Mouse", "QE to mouse").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
					miscMenu.AddItem(new MenuItem("QE_Interrupt", "Use QE to Interrupt").SetValue(true));
					miscMenu.AddItem(new MenuItem("E_Gap_Closer", "Use E On Gap Closer").SetValue(true));
					champMenu.AddSubMenu(miscMenu);
				}

				var drawMenu = new Menu("Drawing", "Drawing");
				{
					drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
					drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_QE", "Draw QE").SetValue(true));
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

		private IEnumerable<SpellSlot> GetSpellCombo()
		{
			var spellCombo = new List<SpellSlot>();
			if(Q.IsReady())
				spellCombo.Add(SpellSlot.Q);
			if(W.IsReady())
				spellCombo.Add(SpellSlot.W);
			if(E.IsReady())
				spellCombo.Add(SpellSlot.E);
			return spellCombo;
		}

		private float GetComboDamage(Obj_AI_Base target)
		{
			double comboDamage = (float)ObjectManager.Player.GetComboDamage(target, GetSpellCombo());

			comboDamage += Get_Ult_Dmg(target);

			if(Ignite != SpellSlot.Unknown && MyHero.SummonerSpellbook.CanUseSpell(Ignite) == SpellState.Ready)
				comboDamage += MyHero.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

			return (float)(comboDamage + ObjectManager.Player.GetAutoAttackDamage(target));
		}

		private float Get_Ult_Dmg(Obj_AI_Base enemy)
		{
			var damage = 0d;

			if(DFG.IsReady())
				damage += MyHero.GetItemDamage(enemy, Damage.DamageItems.Dfg) / 1.2;

			if(R.IsReady())
				damage += (3 + getOrbCount()) * MyHero.GetSpellDamage(enemy, SpellSlot.R, 1) - 20;

			return (float)damage * (DFG.IsReady() ? 1.2f : 1);
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
			if(Menu.Item("Draw_QE").GetValue<bool>())
				if(Q.Level > 0 && E.Level > 0)
					Utility.DrawCircle(MyHero.Position, _qe.Range, Q.IsReady() && E.IsReady() ? Color.Green : Color.Red);
			if(Menu.Item("Draw_W").GetValue<bool>())
				if(W.Level > 0)
					Utility.DrawCircle(MyHero.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_E").GetValue<bool>())
				if(E.Level > 0)
					Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_R").GetValue<bool>())
				if(R.Level > 0)
					Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_R_Killable").GetValue<bool>() && R.IsReady())
			{
				foreach(var wts in from unit in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(2000) && !x.IsDead && x.IsEnemy).OrderBy(x => x.Health)
								   let health = unit.Health + unit.HPRegenRate + 10
								   where Get_Ult_Dmg(unit) > health
								   select Drawing.WorldToScreen(unit.Position))
				{
					Drawing.DrawText(wts[0] - 20, wts[1], Color.White, "KILL!!!");
				}
			}

		    var qeTarget = SimpleTs.GetTarget(_qe.Range, SimpleTs.DamageType.Magical);
		    if (qeTarget == null)
		        return;

            var qePred = _qe.GetPrediction(qeTarget);
            var predVec = MyHero.ServerPosition + Vector3.Normalize(qePred.UnitPosition - MyHero.ServerPosition) * (E.Range - 100);

            if(qePred.Hitchance >= HitChance.High)
                Utility.DrawCircle(predVec, 50, Color.Green);
            else
                Utility.DrawCircle(predVec, 50, Color.Red);
		}

		public override void OnPassive()
		{
			if(Menu.Item("Q_Auto_Harass").GetValue<KeyBind>().Active)
			{
				Cast_Q();
			}

			if(Menu.Item("Misc_QE_Mouse").GetValue<KeyBind>().Active)
			{
				var vec = MyHero.ServerPosition + Vector3.Normalize(Game.CursorPos - MyHero.ServerPosition) * (E.Range - 50);
				_qe.Cast(vec, UsePackets());
				_qe.LastCastAttemptT = Environment.TickCount;
			}

			var qTarget = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
			if(!Menu.Item("Q_Auto_Immobile").GetValue<bool>() || qTarget == null)
				return;
			if(Q.GetPrediction(qTarget).Hitchance == HitChance.Immobile)
				Q.Cast(qTarget);
		}

		public override void OnCombo()
		{

		    if (IsSpellActive("Q") && Q.IsReady())
		        Cast_Q();

		    if (IsSpellActive("W") && W.IsReady())
		        Cast_W(true);

            var qTarget = SimpleTs.GetTarget(650, SimpleTs.DamageType.Magical);
		    if (qTarget != null)
		    {
		        if (GetComboDamage(qTarget) >= qTarget.Health && IsSpellActive("Ignite") && Ignite != SpellSlot.Unknown &&
		            MyHero.Distance(qTarget) < 650 &&
		            MyHero.SummonerSpellbook.CanUseSpell(Ignite) == SpellState.Ready)
		            Use_Ignite(qTarget);
		    }

		    if (IsSpellActive("E") && E.IsReady())
		        Cast_E();
		    

            if (IsSpellActive("QE") && E.IsReady() && Q.IsReady())
                Cast_QE();
            
		    if (IsSpellActive("R") && R.IsReady())
		        Cast_R();
		    
		}

		public override void OnHarass()
		{
			if(!ManaManagerAllowCast())
				return;
			if(IsSpellActive("Q") && Q.IsReady())
				Cast_Q();
			if(IsSpellActive("W") && W.IsReady())
				Cast_W(true);
			if(IsSpellActive("E") && E.IsReady())
				Cast_E();
			if(IsSpellActive("QE") && E.IsReady() && Q.IsReady())
				Cast_QE();
		}

		public override void OnLaneClear()
		{
			if(!ManaManagerAllowCast())
				return;
			if(IsSpellActive("Q") && Q.IsReady())
				Cast_BasicSkillshot_AOE_Farm(Q);
			if(IsSpellActive("W") && W.IsReady())
				Cast_W(false);
			if(!IsSpellActive("E") || !E.IsReady())
				return;
			Cast_BasicSkillshot_AOE_Farm(E, 100);
			W.LastCastAttemptT = Environment.TickCount + 500;
		}

		public override void OnGapClose(ActiveGapcloser gapcloser)
		{
			if(!Menu.Item("E_Gap_Closer").GetValue<bool>())
				return;

			if(!E.IsReady() || !gapcloser.Sender.IsValidTarget(E.Range))
				return;
			E.Cast(gapcloser.Sender, UsePackets());
			W.LastCastAttemptT = Environment.TickCount + 500;
		}


		public override void OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
		{
			if(!unit.IsMe || !E.IsReady() || (spell.SData.Name != "SyndraQ") ||
				Environment.TickCount - _qe.LastCastAttemptT >= 300)
				return;
			E.Cast(spell.End, UsePackets());
			W.LastCastAttemptT = Environment.TickCount + 500;
		}
		public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
		{
			if(spell.DangerLevel < InterruptableDangerLevel.Medium || unit.IsAlly)
				return;

			if(Menu.Item("QE_Interrupt").GetValue<bool>() && unit.IsValidTarget(_qe.Range))
				Cast_QE(unit);
		}

		private void Cast_Q()
		{
			var qTarget = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);

			Q.UpdateSourcePosition();
			if(Q.IsReady() && qTarget != null && Q.GetPrediction(qTarget).Hitchance >= HitChance.High)
				Q.Cast(qTarget, UsePackets());
		}

		private void Cast_W(bool mode)
		{
			if(mode)
			{
				var wTarget = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);

				var grabbableObj = Get_Nearest_orb();
				var wToggleState = MyHero.Spellbook.GetSpell(SpellSlot.W).ToggleState;

				if(wTarget == null)
					return;
				if(wToggleState == 1 && Environment.TickCount - W.LastCastAttemptT > Game.Ping && W.IsReady() &&
					grabbableObj != null)
				{
					if(grabbableObj.Distance(MyHero) < W.Range)
					{
						W.Cast(grabbableObj.ServerPosition);
						W.LastCastAttemptT = Environment.TickCount + 500;
						return;
					}
				}

				W.UpdateSourcePosition(Get_Current_Orb().ServerPosition, Get_Current_Orb().ServerPosition);

				if(MyHero.Distance(wTarget) < E.Range)
				{
					if(wToggleState != 1 && W.IsReady() && W.GetPrediction(wTarget).Hitchance >= HitChance.High &&
						Environment.TickCount - W.LastCastAttemptT > -500 + Game.Ping)
					{
						W.Cast(wTarget);
						return;
					}
				}

				if(wToggleState != 1 && W.IsReady() && W.GetPrediction(wTarget).Hitchance >= HitChance.High)
				{
					W.Cast(wTarget);
				}
			}
			else
			{
                var allMinionsW = MinionManager.GetMinions(MyHero.ServerPosition, W.Range + W.Width + 20, MinionTypes.All, MinionTeam.NotAlly);

			    if (allMinionsW.Count < 2)
			        return;

                var Grabbable_Obj = Get_Nearest_orb();
                var W_Toggle_State = MyHero.Spellbook.GetSpell(SpellSlot.W).ToggleState;

                if (W_Toggle_State == 1 && Environment.TickCount - W.LastCastAttemptT > Game.Ping && W.IsReady() &&
                        Grabbable_Obj != null)
                {
                    W.Cast(Grabbable_Obj.ServerPosition);
                    W.LastCastAttemptT = Environment.TickCount + 1000;
                    return;
                }

                W.UpdateSourcePosition(Get_Current_Orb().ServerPosition, Get_Current_Orb().ServerPosition);

                var farmLocation = W.GetCircularFarmLocation(allMinionsW);

                if (farmLocation.MinionsHit >= 1)
                    W.Cast(farmLocation.Position);
			}
		}

		private void Cast_E()
		{
			if(getOrbCount() <= 0)
				return;
			var target = SimpleTs.GetTarget(_qe.Range + 100, SimpleTs.DamageType.Magical);

			foreach(var orb in getOrb().Where(x => MyHero.Distance(x) < E.Range))
			{
                var Start_Pos = orb.ServerPosition;
                var End_Pos = MyHero.ServerPosition + (Start_Pos - MyHero.ServerPosition) * _qe.Range;

                E.UpdateSourcePosition();
			    var Target_Pos = _qe.GetPrediction(target);

                var projection = Geometry.ProjectOn(Target_Pos.UnitPosition.To2D(), Start_Pos.To2D(), End_Pos.To2D());

                if (projection.IsOnSegment && E.IsReady() &&
                    projection.LinePoint.Distance(Target_Pos.UnitPosition.To2D()) < _qe.Width + target.BoundingRadius)
                {
                    E.Cast(orb.ServerPosition, UsePackets());
                    W.LastCastAttemptT = Environment.TickCount + 500;
                    return;
                }
				return;
			}
		}

		private void Cast_R()
		{
			var rTarget = SimpleTs.GetTarget(R.Level > 2 ? R.Range : 675, SimpleTs.DamageType.Magical);

			if(rTarget == null)
				return;
			if(Menu.Item("Dont_R" + rTarget.ChampionName) == null)
				return;
			if(Menu.Item("Dont_R" + rTarget.ChampionName).GetValue<bool>())
				return;
			if(Menu.Item("R_Overkill_Check").GetValue<bool>())
			{
				if(MyHero.GetSpellDamage(rTarget, SpellSlot.Q) > rTarget.Health)
				{

				}
				else if(Get_Ult_Dmg(rTarget) > rTarget.Health + 20 && rTarget.Distance(MyHero) < R.Range)
				{
					if(DFG.IsReady())
						Use_DFG(rTarget);

					R.CastOnUnit(rTarget, UsePackets());
				}
			}
			else if(Get_Ult_Dmg(rTarget) > rTarget.Health - 20 && rTarget.Distance(MyHero) < R.Range)
			{
				if(DFG.IsReady())
					Use_DFG(rTarget);

				R.CastOnUnit(rTarget, UsePackets());
			}
		}

		private void Cast_QE(Obj_AI_Base target = null)
		{
			var qeTarget = SimpleTs.GetTarget(_qe.Range, SimpleTs.DamageType.Magical);

			if(target != null)
				qeTarget = (Obj_AI_Hero)target;

			if(qeTarget == null)
				return;

			_qe.UpdateSourcePosition();

			var qePred = _qe.GetPrediction(qeTarget);
            var predVec = MyHero.ServerPosition + Vector3.Normalize(qePred.UnitPosition - MyHero.ServerPosition) * (E.Range - 100);

			if(qePred.Hitchance < HitChance.Medium || !Q.IsReady() || !E.IsReady())
				return;
			Q.Cast(predVec, UsePackets());
			_qe.LastCastAttemptT = Environment.TickCount;
		}

		private int getOrbCount()
		{
			return
				ObjectManager.Get<Obj_AI_Minion>().Count(obj => obj.IsValid && obj.Team == ObjectManager.Player.Team && obj.Name == "Seed");
		}

		private IEnumerable<Obj_AI_Minion> getOrb()
		{
			return ObjectManager.Get<Obj_AI_Minion>().Where(obj => obj.IsValid && obj.Team == ObjectManager.Player.Team && obj.Name == "Seed").ToList();
		}

		private Obj_AI_Minion Get_Nearest_orb()
		{
			var orb =
				ObjectManager.Get<Obj_AI_Minion>()
					.Where(obj => obj.IsValid && obj.Team == ObjectManager.Player.Team && obj.Name == "Seed")
					.ToList()
					.OrderBy(x => MyHero.Distance(x))
					.FirstOrDefault();
			if(orb != null)
				return orb;

			if(Menu.Item("W_Only_Orb").GetValue<bool>())
				return null;
			var minion = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.IsValidTarget(W.Range));

			return minion;
		}

		private Obj_AI_Base Get_Current_Orb()
		{
			var orb = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.Team == MyHero.Team && x.Name == "Seed" && !x.IsTargetable);

			if(orb != null)
				return orb;

			var minion = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.IsInvulnerable && x.Name != "Seed" && x.Name != "k");

			return minion;
		}

	}
}

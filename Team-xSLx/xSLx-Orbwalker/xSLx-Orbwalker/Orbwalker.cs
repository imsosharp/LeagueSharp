using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using Color = System.Drawing.Color;

namespace xSLx_Orbwalker
{
	// ReSharper disable once InconsistentNaming
	public class xSLxOrbwalker
	{

		private static readonly string[] AttackResets = { "dariusnoxiantacticsonh", "fioraflurry", "garenq", "hecarimrapidslash", "jaxempowertwo", "jaycehypercharge", "leonashieldofdaybreak" ,"monkeykingdoubleattack", "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze", "netherblade", "parley", "poppydevastatingblow", "powerfist", "renektonpreexecute", "rengarq", "shyvanadoubleattack", "sivirw", "takedown", "talonnoxiandiplomacy", "trundletrollsmash", "vaynetumble", "vie", "volibearq", "xenzhaocombotarget", "yorickspectral" };
		private static readonly string[] NoAttacks = { "jarvanivcataclysmattack", "monkeykingdoubleattack", "shyvanadoubleattack", "shyvanadoubleattackdragon", "zyragraspingplantattack", "zyragraspingplantattack2", "zyragraspingplantattackfire", "zyragraspingplantattack2fire" };
		private static readonly string[] Attacks = { "caitlynheadshotmissile", "frostarrow", "garenslash2", "kennenmegaproc", "lucianpassiveattack", "masteryidoublestrike", "quinnwenhanced", "renektonexecute", "renektonsuperexecute", "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust", "viktorqbuff", "xenzhaothrust2", "xenzhaothrust3" };


		public static Menu Menu;
		public static Obj_AI_Hero MyHero = ObjectManager.Player;
		public static Obj_AI_Base ForcedTarget = null;
		public static IEnumerable<Obj_AI_Hero> AllEnemys = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy);
		public static IEnumerable<Obj_AI_Hero> AllAllys = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly);
		public static bool CustomOrbwalkMode;

		public delegate void BeforeAttackEvenH(BeforeAttackEventArgs args);
		public delegate void OnTargetChangeH(Obj_AI_Base oldTarget, Obj_AI_Base newTarget);
		public delegate void AfterAttackEvenH(Obj_AI_Base unit, Obj_AI_Base target);
		public delegate void OnAttackEvenH(Obj_AI_Base unit, Obj_AI_Base target);

		public static event BeforeAttackEvenH BeforeAttack;
		public static event OnTargetChangeH OnTargetChange;
		public static event AfterAttackEvenH AfterAttack;
		public static event OnAttackEvenH OnAttack;

		public enum Mode
		{
			Combo,
			Harass,
			LaneClear,
			LaneFreeze,
			Lasthit,
			Flee,
			None,
		}

		private static bool _drawing = true;
		private static bool _attack = true;
		private static bool _movement = true;
		private static bool _disableNextAttack;
		private const float LaneClearWaitTimeMod = 2f;
		private static int _lastAATick;
		private static Obj_AI_Base _lastTarget;
		private static Spell _movementPrediction;
		private static int _lastMovement;
		private static int _windup;
		private static int lastRealAttack;
		public static void AddToMenu(Menu menu)
		{
			_movementPrediction = new Spell(SpellSlot.Unknown, GetAutoAttackRange());
			_movementPrediction.SetTargetted(MyHero.BasicAttack.SpellCastTime, MyHero.BasicAttack.MissileSpeed);

			Menu = menu;

			var menuDrawing = new Menu("Drawing", "orb_Draw");
			menuDrawing.AddItem(new MenuItem("orb_Draw_AARange", "AA Circle").SetValue(new Circle(true, Color.FloralWhite)));
			menuDrawing.AddItem(new MenuItem("orb_Draw_AARange_Enemy", "AA Circle Enemy").SetValue(new Circle(true, Color.Pink)));
			menuDrawing.AddItem(new MenuItem("orb_Draw_Holdzone", "Holdzone").SetValue(new Circle(true, Color.FloralWhite)));
			menuDrawing.AddItem(new MenuItem("orb_Draw_MinionHPBar", "Minion HPBar").SetValue(new Circle(true, Color.Black)));
			menuDrawing.AddItem(new MenuItem("orb_Draw_MinionHPBar_thickness", "^ HPBar Thickness").SetValue(new Slider(1, 1, 3)));
			menuDrawing.AddItem(new MenuItem("orb_Draw_hitbox", "Show HitBoxes").SetValue(new Circle(true, Color.FloralWhite)));
			menuDrawing.AddItem(new MenuItem("orb_Draw_Lasthit", "Minion Lasthit").SetValue(new Circle(true, Color.Lime)));
			menuDrawing.AddItem(new MenuItem("orb_Draw_nearKill", "Minion nearKill").SetValue(new Circle(true, Color.Gold)));
			menu.AddSubMenu(menuDrawing);

			var menuMisc = new Menu("Misc", "orb_Misc");
			menuMisc.AddItem(new MenuItem("orb_Misc_Holdzone", "Hold Position").SetValue(new Slider(50, 100, 0)));
			menuMisc.AddItem(new MenuItem("orb_Misc_Farmdelay", "Farm Delay").SetValue(new Slider(0, 200, 0)));
			menuMisc.AddItem(new MenuItem("orb_Misc_ExtraWindUp", "Extra Winduptime").SetValue(new Slider(80, 200, 0)));
			menuMisc.AddItem(new MenuItem("orb_Misc_AutoWindUp", "Autoset Windup").SetValue(false));
			menuMisc.AddItem(new MenuItem("orb_Misc_Priority_Unit", "Priority Unit").SetValue(new StringList(new[] { "Minion", "Hero" })));
			menuMisc.AddItem(new MenuItem("orb_Misc_Humanizer", "Humanizer Delay").SetValue(new Slider(80, 200, 15)));
			menuMisc.AddItem(new MenuItem("orb_Misc_AllMovementDisabled", "Disable All Movement").SetValue(false));
			menuMisc.AddItem(new MenuItem("orb_Misc_AllAttackDisabled", "Disable All Attacks").SetValue(false));

			menu.AddSubMenu(menuMisc);

			var menuMelee = new Menu("Melee", "orb_Melee");
			menuMelee.AddItem(new MenuItem("orb_Melee_Prediction", "Movement Prediction").SetValue(false));
			menu.AddSubMenu(menuMelee);

			var menuModes = new Menu("Orbwalk Mode", "orb_Modes");
			{
				var modeCombo = new Menu("Combo", "orb_Modes_Combo");
				modeCombo.AddItem(new MenuItem("Combo_Key", "Key").SetValue(new KeyBind(32, KeyBindType.Press)));
				modeCombo.AddItem(new MenuItem("Combo_move", "Movement").SetValue(true));
				modeCombo.AddItem(new MenuItem("Combo_attack", "Attack").SetValue(true));
				menuModes.AddSubMenu(modeCombo);

				var modeHarass = new Menu("Harass", "orb_Modes_Harass");
				modeHarass.AddItem(new MenuItem("Harass_Key", "Key").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
				modeHarass.AddItem(new MenuItem("Harass_move", "Movement").SetValue(true));
				modeHarass.AddItem(new MenuItem("Harass_attack", "Attack").SetValue(true));
				modeHarass.AddItem(new MenuItem("Harass_Lasthit", "Lasthit Minions").SetValue(true));
				menuModes.AddSubMenu(modeHarass);

				var modeLaneClear = new Menu("LaneClear", "orb_Modes_LaneClear");
				modeLaneClear.AddItem(new MenuItem("LaneClear_Key", "Key").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
				modeLaneClear.AddItem(new MenuItem("LaneClear_move", "Movement").SetValue(true));
				modeLaneClear.AddItem(new MenuItem("LaneClear_attack", "Attack").SetValue(true));
				menuModes.AddSubMenu(modeLaneClear);

				var modeLaneFreeze = new Menu("LaneFreeze", "orb_Modes_LaneFreeze");
				modeLaneFreeze.AddItem(new MenuItem("LaneFreeze_Key", "Key").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
				modeLaneFreeze.AddItem(new MenuItem("LaneFreeze_move", "Movement").SetValue(true));
				modeLaneFreeze.AddItem(new MenuItem("LaneFreeze_attack", "Attack").SetValue(true));
				menuModes.AddSubMenu(modeLaneFreeze);

				var modeLasthit = new Menu("LastHit", "orb_Modes_LastHit");
				modeLasthit.AddItem(new MenuItem("LastHit_Key", "Key").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));
				modeLasthit.AddItem(new MenuItem("LastHit_move", "Movement").SetValue(true));
				modeLasthit.AddItem(new MenuItem("LastHit_attack", "Attack").SetValue(true));
				menuModes.AddSubMenu(modeLasthit);

				var modeFlee = new Menu("Flee", "orb_Modes_Flee");
				modeFlee.AddItem(new MenuItem("Flee_Key", "Key").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
				menuModes.AddSubMenu(modeFlee);
			}
			menu.AddSubMenu(menuModes);
			menu.AddItem(new MenuItem("xSLx_info", "Copyright by xSLx"));
			menu.AddItem(new MenuItem("xSLx_info2", "Credits: xSLx & Esk0r"));

			Drawing.OnDraw += OnDraw;
			Game.OnGameUpdate += OnUpdate;
			Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
			GameObject.OnCreate += Obj_SpellMissile_OnCreate;
		}


		private static void Obj_SpellMissile_OnCreate(GameObject sender, EventArgs args)
		{
			if(sender.IsMe)
			{
				var obj = (Obj_AI_Hero)sender;
				if(obj.IsMelee())
					return;
			}
			if(!(sender is Obj_SpellMissile) || !sender.IsValid)
				return;
			var missile = (Obj_SpellMissile)sender;
			if(missile.SpellCaster is Obj_AI_Hero && missile.SpellCaster.IsValid && IsAutoAttack(missile.SData.Name))
			{
				FireAfterAttack(missile.SpellCaster, _lastTarget);
				if (sender.IsMe)
					lastRealAttack = Environment.TickCount;
			}
		}

		private static void OnUpdate(EventArgs args)
		{
			CheckAutoWindUp();
			if(CurrentMode == Mode.None || MenuGUI.IsChatOpen || CustomOrbwalkMode)
				return;
			var target = GetPossibleTarget();
			Orbwalk(Game.CursorPos, target);
		}

		private static void OnDraw(EventArgs args)
		{
			if(!_drawing)
				return;

			if(Menu.Item("orb_Draw_AARange").GetValue<Circle>().Active)
			{
				Utility.DrawCircle(MyHero.Position, GetAutoAttackRange(), Menu.Item("orb_Draw_AARange").GetValue<Circle>().Color);
			}

			if(Menu.Item("orb_Draw_AARange_Enemy").GetValue<Circle>().Active ||
				Menu.Item("orb_Draw_hitbox").GetValue<Circle>().Active)
			{
				foreach(var enemy in AllEnemys.Where(enemy => enemy.IsValidTarget(1500)))
				{
					if(Menu.Item("orb_Draw_AARange_Enemy").GetValue<Circle>().Active)
						Utility.DrawCircle(enemy.Position, GetAutoAttackRange(enemy, MyHero), Menu.Item("orb_Draw_AARange_Enemy").GetValue<Circle>().Color);
					if(Menu.Item("orb_Draw_hitbox").GetValue<Circle>().Active)
						Utility.DrawCircle(enemy.Position, enemy.BoundingRadius, Menu.Item("orb_Draw_hitbox").GetValue<Circle>().Color);
				}
			}

			if(Menu.Item("orb_Draw_AARange_Enemy").GetValue<Circle>().Active)
			{
				foreach(var enemy in AllEnemys.Where(enemy => enemy.IsValidTarget(1500)))
				{
					Utility.DrawCircle(enemy.Position, GetAutoAttackRange(enemy, MyHero), Menu.Item("orb_Draw_AARange_Enemy").GetValue<Circle>().Color);

				}
			}

			if(Menu.Item("orb_Draw_Holdzone").GetValue<Circle>().Active)
			{
				Utility.DrawCircle(MyHero.Position, Menu.Item("orb_Misc_Holdzone").GetValue<Slider>().Value, Menu.Item("orb_Draw_Holdzone").GetValue<Circle>().Color);
			}

			if(Menu.Item("orb_Draw_MinionHPBar").GetValue<Circle>().Active ||
				Menu.Item("orb_Draw_Lasthit").GetValue<Circle>().Active ||
				Menu.Item("orb_Draw_nearKill").GetValue<Circle>().Active)
			{
				var minionList = MinionManager.GetMinions(MyHero.Position, GetAutoAttackRange() + 500, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
				foreach(var minion in minionList.Where(minion => minion.IsValidTarget(GetAutoAttackRange() + 500)))
				{
					var attackToKill = Math.Ceiling(minion.MaxHealth / MyHero.GetAutoAttackDamage(minion, true));
					var hpBarPosition = minion.HPBarPosition;
					var barWidth = minion.IsMelee() ? 75 : 80;
					if(minion.HasBuff("turretshield", true))
						barWidth = 70;
					var barDistance = (float)(barWidth / attackToKill);
					if(Menu.Item("orb_Draw_MinionHPBar").GetValue<Circle>().Active)
					{
						for(var i = 1; i < attackToKill; i++)
						{
							var startposition = hpBarPosition.X + 45 + barDistance * i;
							Drawing.DrawLine(
								new Vector2(startposition, hpBarPosition.Y + 18),
								new Vector2(startposition, hpBarPosition.Y + 23),
								Menu.Item("orb_Draw_MinionHPBar_thickness").GetValue<Slider>().Value,
								Menu.Item("orb_Draw_MinionHPBar").GetValue<Circle>().Color);
						}
					}
					if(Menu.Item("orb_Draw_Lasthit").GetValue<Circle>().Active &&
						minion.Health <= MyHero.GetAutoAttackDamage(minion, true))
						Utility.DrawCircle(minion.Position, minion.BoundingRadius, Menu.Item("orb_Draw_Lasthit").GetValue<Circle>().Color);
					else if(Menu.Item("orb_Draw_nearKill").GetValue<Circle>().Active &&
							 minion.Health <= MyHero.GetAutoAttackDamage(minion, true) * 2)
						Utility.DrawCircle(minion.Position, minion.BoundingRadius, Menu.Item("orb_Draw_nearKill").GetValue<Circle>().Color);
				}
			}
		}

		public static void Orbwalk(Vector3 goalPosition, Obj_AI_Base target)
		{
			if(target != null && (CanAttack()|| HaveCancled()) && IsAllowedToAttack() )
			{
				_disableNextAttack = false;
				FireBeforeAttack(target);
				if(!_disableNextAttack)
				{
					if(CurrentMode != Mode.Harass || !target.IsMinion || Menu.Item("Harass_Lasthit").GetValue<bool>())
					{
						MyHero.IssueOrder(GameObjectOrder.AttackUnit, target);
						_lastAATick = Environment.TickCount + Game.Ping / 2 ;
					}
				}
			}
			if(!CanMove() || !IsAllowedToMove())
				return;
			if(MyHero.IsMelee() && target != null && target.Distance(MyHero) < GetAutoAttackRange(MyHero, target) &&
				Menu.Item("orb_Melee_Prediction").GetValue<bool>() && target is Obj_AI_Hero && Game.CursorPos.Distance(target.Position) < 300)
			{
				_movementPrediction.Delay = MyHero.BasicAttack.SpellCastTime;
				_movementPrediction.Speed = MyHero.BasicAttack.MissileSpeed;
				MoveTo(_movementPrediction.GetPrediction(target).UnitPosition);
			}
			else
				MoveTo(goalPosition);
		}


		private static void MoveTo(Vector3 position)
		{
			var delay = Menu.Item("orb_Misc_Humanizer").GetValue<Slider>().Value;
			if(Environment.TickCount - _lastMovement < delay)
				return;
			_lastMovement = Environment.TickCount;

			var holdAreaRadius = Menu.Item("orb_Misc_Holdzone").GetValue<Slider>().Value;
			if(MyHero.ServerPosition.Distance(position) < holdAreaRadius)
			{
				if(MyHero.Path.Count() > 1)
					MyHero.IssueOrder(GameObjectOrder.HoldPosition, MyHero.Position);
				return;
			}
			var point = MyHero.ServerPosition +
			300 * (position.To2D() - MyHero.ServerPosition.To2D()).Normalized().To3D();
			MyHero.IssueOrder(GameObjectOrder.MoveTo, point);


		}

		private static bool IsAllowedToMove()
		{
			if(!_movement)
				return false;
			if(Menu.Item("orb_Misc_AllMovementDisabled").GetValue<bool>())
				return false;
			if(CurrentMode == Mode.Combo && !Menu.Item("Combo_move").GetValue<bool>())
				return false;
			if(CurrentMode == Mode.Harass && !Menu.Item("Harass_move").GetValue<bool>())
				return false;
			if(CurrentMode == Mode.LaneClear && !Menu.Item("LaneClear_move").GetValue<bool>())
				return false;
			if(CurrentMode == Mode.LaneFreeze && !Menu.Item("LaneFreeze_move").GetValue<bool>())
				return false;
			return CurrentMode != Mode.Lasthit || Menu.Item("LastHit_move").GetValue<bool>();
		}

		private static bool IsAllowedToAttack()
		{
			if(!_attack)
				return false;
			if(Menu.Item("orb_Misc_AllAttackDisabled").GetValue<bool>())
				return false;
			if(CurrentMode == Mode.Combo && !Menu.Item("Combo_attack").GetValue<bool>())
				return false;
			if(CurrentMode == Mode.Harass && !Menu.Item("Harass_attack").GetValue<bool>())
				return false;
			if(CurrentMode == Mode.LaneClear && !Menu.Item("LaneClear_attack").GetValue<bool>())
				return false;
			if(CurrentMode == Mode.LaneFreeze && !Menu.Item("LaneFreeze_attack").GetValue<bool>())
				return false;
			return CurrentMode != Mode.Lasthit || Menu.Item("LastHit_attack").GetValue<bool>();

		}

		private static void OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
		{
			if(IsAutoAttackReset(spell.SData.Name) && unit.IsMe)
				Utility.DelayAction.Add(100, ResetAutoAttackTimer);
			
			if(!IsAutoAttack(spell.SData.Name))
				return;
			if(unit.IsMe)
			{
				_lastAATick = Environment.TickCount - Game.Ping / 2; // need test todo
				// ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
				if(spell.Target is Obj_AI_Base)
				{
					FireOnTargetSwitch((Obj_AI_Base)spell.Target);
					_lastTarget = (Obj_AI_Base)spell.Target;
				}
				if(unit.IsMelee())
					Utility.DelayAction.Add(
						(int)(unit.AttackCastDelay * 1000 + Game.Ping * 0.5) + 50, () => FireAfterAttack(unit, _lastTarget));

				FireOnAttack(unit, _lastTarget);
			}
			else
			{
				FireOnAttack(unit, (Obj_AI_Base)spell.Target);
			}
		}

		public static double GetAzirAASandwarriorDamage(Obj_AI_Base unit)
		{
			var damagelist = new List<int> { 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 110, 120, 130, 140, 150, 160, 170 };
			var dmg = damagelist[MyHero.Level - 1] + (MyHero.BaseAbilityDamage * 0.7);
			if(
				ObjectManager.Get<Obj_AI_Minion>()
					.Count(
						obj =>
							obj.Name == "AzirSoldier" && obj.IsAlly && obj.BoundingRadius < 66 && obj.AttackSpeedMod > 1 &&
							obj.Distance(unit) < 350) == 2)
				return MyHero.CalcDamage(unit, Damage.DamageType.Magical, dmg) +
					   (MyHero.CalcDamage(unit, Damage.DamageType.Magical, dmg) * 0.25);
			return MyHero.CalcDamage(unit, Damage.DamageType.Magical, dmg);
		}

		public static bool InSoldierAttackRange(Obj_AI_Base target)
		{
			return target != null && ObjectManager.Get<Obj_AI_Minion>().Any(obj => obj.Name == "AzirSoldier" && obj.IsAlly && obj.BoundingRadius < 66 && obj.AttackSpeedMod > 1 && obj.Distance(target) < 380);
		}

		public static Obj_AI_Base GetPossibleTarget()
		{
			if (ForcedTarget != null)
			{
				if (InAutoAttackRange(ForcedTarget))
					return ForcedTarget;
				ForcedTarget = null;
			}
		

		Obj_AI_Base tempTarget = null;

			if(Menu.Item("orb_Misc_Priority_Unit").GetValue<StringList>().SelectedIndex == 1 &&
				(CurrentMode == Mode.Harass || CurrentMode == Mode.LaneClear))
			{
				tempTarget = GetBestHeroTarget();
				if(tempTarget != null)
					return tempTarget;
			}

			if(CurrentMode == Mode.Harass || CurrentMode == Mode.Lasthit || CurrentMode == Mode.LaneClear || CurrentMode == Mode.LaneFreeze)
			{
				if(MyHero.ChampionName == "Azir")
				{
					foreach(
					var minion in
						from minion in
							ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget()  && minion.Name != "Beacon" && InSoldierAttackRange(minion))
						let t = (int)(MyHero.AttackCastDelay * 1000) - 100 + Game.Ping / 2 +
								1000 * (int)MyHero.Distance(minion) / (int)MyProjectileSpeed()
						let predHealth = HealthPrediction.GetHealthPrediction(minion, t, FarmDelay(-125))
						where minion.Team != GameObjectTeam.Neutral && predHealth > 0 &&
							  predHealth <= GetAzirAASandwarriorDamage(minion)
						select minion)
						return minion;
				}

				foreach(
					var minion in
						from minion in
							ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget() && minion.Name != "Beacon" && InAutoAttackRange(minion))
						let t = (int)(MyHero.AttackCastDelay * 1000) - 100 + Game.Ping / 2 +
								1000 * (int)MyHero.Distance(minion) / (int)MyProjectileSpeed()
						let predHealth = HealthPrediction.GetHealthPrediction(minion, t, FarmDelay())
						where minion.Team != GameObjectTeam.Neutral && predHealth > 0 &&
							  predHealth <= MyHero.GetAutoAttackDamage(minion, true)
						select minion)
					return minion;
			}

			if(CurrentMode != Mode.Lasthit)
			{
				tempTarget = GetBestHeroTarget();
				if(tempTarget != null)
					return tempTarget;
			}

			if(CurrentMode == Mode.Harass || CurrentMode == Mode.LaneClear || CurrentMode == Mode.LaneFreeze)
			{

				foreach(
					var turret in
						ObjectManager.Get<Obj_AI_Turret>().Where(turret => turret.IsValidTarget(GetAutoAttackRange(MyHero, turret))))
					return turret;
			}

			float[] maxhealth;
			if(CurrentMode == Mode.LaneClear || CurrentMode == Mode.Harass || CurrentMode == Mode.LaneFreeze)
			{
				if(MyHero.ChampionName == "Azir")
				{
					maxhealth = new float[] { 0 };
					var maxhealth1 = maxhealth;
					foreach(
						var minion in
							ObjectManager.Get<Obj_AI_Minion>()
								.Where(minion => InSoldierAttackRange(minion) && minion.Name != "Beacon" && minion.IsValidTarget() && minion.Team == GameObjectTeam.Neutral)
								.Where(minion => minion.MaxHealth >= maxhealth1[0] || Math.Abs(maxhealth1[0] - float.MaxValue) < float.Epsilon))
					{
						tempTarget = minion;
						maxhealth[0] = minion.MaxHealth;
					}
					if(tempTarget != null)
						return tempTarget;
				}

				maxhealth = new float[] { 0 };
				var maxhealth2 = maxhealth;
				foreach(var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget(GetAutoAttackRange(MyHero, minion)) && minion.Name != "Beacon" && minion.Team == GameObjectTeam.Neutral).Where(minion => minion.MaxHealth >= maxhealth2[0] || Math.Abs(maxhealth2[0] - float.MaxValue) < float.Epsilon))
				{
					tempTarget = minion;
					maxhealth[0] = minion.MaxHealth;
				}
				if(tempTarget != null)
					return tempTarget;
			}

			if(CurrentMode != Mode.LaneClear || ShouldWait())
			{
				//ResetAutoAttackTimer();
				return null;
			}

			if(MyHero.ChampionName == "Azir")
			{
				maxhealth = new float[] { 0 };
				float[] maxhealth1 = maxhealth;
				foreach(var minion in from minion in ObjectManager.Get<Obj_AI_Minion>()
				   .Where(minion => minion.IsValidTarget() && minion.Name != "Beacon" && InSoldierAttackRange(minion))
									  let predHealth = HealthPrediction.LaneClearHealthPrediction(minion, (int)((MyHero.AttackDelay * 1000) * LaneClearWaitTimeMod), FarmDelay(-125))
									  where predHealth >=
											GetAzirAASandwarriorDamage(minion) + MyHero.GetAutoAttackDamage(minion, true) ||
											Math.Abs(predHealth - minion.Health) < float.Epsilon
									  where minion.Health >= maxhealth1[0] || Math.Abs(maxhealth1[0] - float.MaxValue) < float.Epsilon
									  select minion)
				{
					tempTarget = minion;
					maxhealth[0] = minion.MaxHealth;
				}
				if(tempTarget != null)
					return tempTarget;
			}

			maxhealth = new float[] { 0 };
			foreach(var minion in from minion in ObjectManager.Get<Obj_AI_Minion>()
			   .Where(minion => minion.IsValidTarget(GetAutoAttackRange(MyHero, minion)) && minion.Name != "Beacon")
								  let predHealth = HealthPrediction.LaneClearHealthPrediction(minion, (int)((MyHero.AttackDelay * 1000) * LaneClearWaitTimeMod), FarmDelay())
								  where predHealth >=
										2 * MyHero.GetAutoAttackDamage(minion, true) ||
										Math.Abs(predHealth - minion.Health) < float.Epsilon
								  where minion.Health >= maxhealth[0] || Math.Abs(maxhealth[0] - float.MaxValue) < float.Epsilon
								  select minion)
			{
				tempTarget = minion;
				maxhealth[0] = minion.MaxHealth;
			}
			if(tempTarget != null)
				return tempTarget;

			return null;
		}

		private static bool ShouldWait()
		{
			return ObjectManager.Get<Obj_AI_Minion>()
			.Any(
			minion =>
			minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral &&
			InAutoAttackRange(minion) &&
			HealthPrediction.LaneClearHealthPrediction(
			minion, (int)((MyHero.AttackDelay * 1000) * LaneClearWaitTimeMod), FarmDelay()) <= MyHero.GetAutoAttackDamage(minion));
		}

		public static bool IsAutoAttack(string name)
		{
			return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower())) ||
			Attacks.Contains(name.ToLower());
		}

		public static void ResetAutoAttackTimer()
		{
			_lastAATick = 0;
		}

		public static bool IsAutoAttackReset(string name)
		{
			return AttackResets.Contains(name.ToLower());
		}

		public static float GetNextAATime()
		{
			return (_lastAATick + MyHero.AttackDelay * 1000) - (Environment.TickCount + Game.Ping / 2 + 25);
		}
		public static bool CanAttack()
		{
			if (_lastAATick <= Environment.TickCount)
				return Environment.TickCount + Game.Ping/2 + 25 >= _lastAATick + MyHero.AttackDelay*1000 && _attack;
			return false;
		}

		private static bool HaveCancled()
		{
			if (_lastAATick - Environment.TickCount > MyHero.AttackCastDelay * 1000 + 25)
				return lastRealAttack < _lastAATick;
			return false;
		}

		public static bool CanMove()
		{
			if(_lastAATick <= Environment.TickCount)
				return Environment.TickCount + Game.Ping / 2 >= _lastAATick + MyHero.AttackCastDelay * 1000 + _windup && _movement;
			return false;
		}

		private static float MyProjectileSpeed()
		{
			return (MyHero.CombatType == GameObjectCombatType.Melee) ? float.MaxValue : MyHero.BasicAttack.MissileSpeed;
		}

		private static int FarmDelay(int offset = 0)
		{
			var ret = offset;
			if(MyHero.ChampionName == "Azir")
				ret += 125;
			return Menu.Item("orb_Misc_Farmdelay").GetValue<Slider>().Value + ret;
		}

		private static Obj_AI_Base GetBestHeroTarget()
		{
			Obj_AI_Hero killableEnemy = null;
			var hitsToKill = double.MaxValue;
			if(MyHero.ChampionName == "Azir")
			{
				foreach(var enemy in AllEnemys.Where(hero => hero.IsValidTarget() && InSoldierAttackRange(hero)))
				{
					var killHits = CountKillhitsAzirSoldier(enemy);
					if(killableEnemy != null && (!(killHits < hitsToKill) || enemy.HasBuffOfType(BuffType.Invulnerability)))
						continue;
					killableEnemy = enemy;
					hitsToKill = killHits;
				}
				if(hitsToKill <= 4)
					return killableEnemy;
				Obj_AI_Hero[] mostdmgenemy = { null };
				foreach(var enemy in AllEnemys.Where(hero => hero.IsValidTarget() && InSoldierAttackRange(hero)).Where(enemy => mostdmgenemy[0] == null || GetAzirAASandwarriorDamage(enemy) > GetAzirAASandwarriorDamage(mostdmgenemy[0])))
				{
					mostdmgenemy[0] = enemy;
				}
				if(mostdmgenemy[0] != null)
					return mostdmgenemy[0];
			}
			foreach(var enemy in AllEnemys.Where(hero => hero.IsValidTarget() && InAutoAttackRange(hero)))
			{
				var killHits = CountKillhits(enemy);
				if(killableEnemy != null && (!(killHits < hitsToKill) || enemy.HasBuffOfType(BuffType.Invulnerability)))
					continue;
				hitsToKill = killHits;
				killableEnemy = enemy;
			}
			return hitsToKill <= 3 ? killableEnemy : SimpleTs.GetTarget(GetAutoAttackRange(), SimpleTs.DamageType.Physical);
		}

		public static double CountKillhits(Obj_AI_Base enemy)
		{
			return enemy.Health / MyHero.GetAutoAttackDamage(enemy);
		}

		public static double CountKillhitsAzirSoldier(Obj_AI_Base enemy)
		{
			return enemy.Health / GetAzirAASandwarriorDamage(enemy);
		}

		private static void CheckAutoWindUp()
		{
			if(!Menu.Item("orb_Misc_AutoWindUp").GetValue<bool>())
			{
				_windup = GetCurrentWindupTime();
				return;
			}
			var additional = 0;
			if(Game.Ping >= 100)
				additional = Game.Ping / 100 * 5;
			else if(Game.Ping > 40 && Game.Ping < 100)
				additional = Game.Ping / 100 * 10;
			else if(Game.Ping <= 40)
				additional = +20;
			var windUp = Game.Ping + additional;
			if(windUp < 40)
				windUp = 40;
			Menu.Item("orb_Misc_ExtraWindUp").SetValue(windUp < 200 ? new Slider(windUp, 200, 0) : new Slider(200, 200, 0));
			_windup = windUp;
		}

		public static int GetCurrentWindupTime()
		{
			return Menu.Item("orb_Misc_ExtraWindUp").GetValue<Slider>().Value;
		}

		public static void EnableDrawing()
		{
			_drawing = true;
		}

		public static void DisableDrawing()
		{
			_drawing = false;
		}

		public static float GetAutoAttackRange(Obj_AI_Base source = null, Obj_AI_Base target = null)
		{
			if(source == null)
				source = MyHero;
			var ret = source.AttackRange + MyHero.BoundingRadius;
			if(target != null)
				ret += target.BoundingRadius;
			return ret;
		}

		public static bool InAutoAttackRange(Obj_AI_Base target)
		{
			if(target == null)
				return false;
			var myRange = GetAutoAttackRange(MyHero, target);
			return target.IsValidTarget(myRange);
		}

		public static Mode CurrentMode
		{
			get
			{
				if(Menu.Item("Combo_Key").GetValue<KeyBind>().Active)
					return Mode.Combo;
				if(Menu.Item("Harass_Key").GetValue<KeyBind>().Active)
					return Mode.Harass;
				if(Menu.Item("LaneClear_Key").GetValue<KeyBind>().Active)
					return Mode.LaneClear;
				if(Menu.Item("LaneFreeze_Key").GetValue<KeyBind>().Active)
					return Mode.LaneFreeze;
				if(Menu.Item("LastHit_Key").GetValue<KeyBind>().Active)
					return Mode.Lasthit;
				return Menu.Item("Flee_Key").GetValue<KeyBind>().Active ? Mode.Flee : Mode.None;
			}
		}

		public static void SetAttack(bool value)
		{
			_attack = value;
		}

		public static void SetMovement(bool value)
		{
			_movement = value;
		}

		public static bool GetAttack()
		{
			return _attack;
		}

		public static bool GetMovement()
		{
			return _movement;
		}

		public class BeforeAttackEventArgs
		{
			public Obj_AI_Base Target;
			public Obj_AI_Base Unit = ObjectManager.Player;
			private bool _process = true;
			public bool Process
			{
				get
				{
					return _process;
				}
				set
				{
					_disableNextAttack = !value;
					_process = value;
				}
			}
		}
		private static void FireBeforeAttack(Obj_AI_Base target)
		{
			if(BeforeAttack != null)
			{
				BeforeAttack(new BeforeAttackEventArgs
				{
					Target = target
				});
			}
			else
			{
				_disableNextAttack = false;
			}
		}

		private static void FireOnTargetSwitch(Obj_AI_Base newTarget)
		{
			if(OnTargetChange != null && (_lastTarget == null || _lastTarget.NetworkId != newTarget.NetworkId))
			{
				OnTargetChange(_lastTarget, newTarget);
			}
		}

		private static void FireAfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
		{
			if(AfterAttack != null)
			{
				AfterAttack(unit, target);
			}
		}

		private static void FireOnAttack(Obj_AI_Base unit, Obj_AI_Base target)
		{
			if(OnAttack != null)
			{
				OnAttack(unit, target);
			}
		}
    }
}

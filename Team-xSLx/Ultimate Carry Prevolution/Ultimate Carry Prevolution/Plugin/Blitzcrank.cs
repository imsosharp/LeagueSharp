using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
    class Blitzcrank : Champion
    {
        public Blitzcrank()
        {
            SetSpells();
            LoadMenu();
        }

        private void SetSpells()
        {
            Q = new Spell(SpellSlot.Q, 950);
            Q.SetSkillshot(0.22f, 70f, 1800, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W);

            E = new Spell(SpellSlot.E, 140);

            R = new Spell(SpellSlot.R, 600);
        }

        private void LoadMenu()
        {
            var champMenu = new Menu("Blitzcrank Plugin", "Blitzcrank");
            {
                var qMenu = new Menu("QMenu", "QMenu");
                {
                    qMenu.AddItem(new MenuItem("Q_Min_Range", "Q Min Range Slider").SetValue(new Slider(300, 1, 950)));
                    qMenu.AddItem(new MenuItem("Q_Max_Range", "Q Max Range Slider").SetValue(new Slider(900, 300, 950)));
                    qMenu.AddItem(new MenuItem("Auto_Q_Slow", "Auto Q Slow").SetValue(true));
                    qMenu.AddItem(new MenuItem("Auto_Q_Immobile", "Auto Q Immobile").SetValue(true));
                    qMenu.AddItem(new MenuItem("Auto_Q_Dashing", "Auto Q Dashing").SetValue(true));
                    qMenu.AddSubMenu(new Menu("Don't use Q on", "Dont_Q"));

                    foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != MyHero.Team))
                        qMenu.SubMenu("Dont_Q").AddItem(new MenuItem("Dont_Q" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(false));

                    champMenu.AddSubMenu(qMenu);
                }

                var comboMenu = new Menu("Combo", "Combo");
                {
                    AddSpelltoMenu(comboMenu, "Q", true);
                    comboMenu.AddItem(new MenuItem("Q_AA_Windup", "Q On Enemy AA Animation").SetValue(true));
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
                    AddSpelltoMenu(laneClearMenu, "R", true);
                    AddManaManagertoMenu(laneClearMenu, 0);
                    champMenu.AddSubMenu(laneClearMenu);
                }

                var fleeMenu = new Menu("Flee", "Flee");
                {
                    AddSpelltoMenu(fleeMenu, "W", true);
                    champMenu.AddSubMenu(fleeMenu);
                }

                var miscMenu = new Menu("Misc", "Misc");
                {
                    miscMenu.AddItem(new MenuItem("Misc_R_Interrupt", "Use R to Interrupt").SetValue(true));
                    miscMenu.AddItem(new MenuItem("Misc_E_Reset", "Use E AA reset Only").SetValue(true));
                    miscMenu.AddItem(new MenuItem("Misc_MEC_R", "Use R if hit").SetValue(new Slider(3, 0, 5)));
                    champMenu.AddSubMenu(miscMenu);
                }

                var drawMenu = new Menu("Drawing", "Drawing");
                {
                    drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
                    drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
                    drawMenu.AddItem(new MenuItem("Draw_W", "Draw W").SetValue(true));
                    drawMenu.AddItem(new MenuItem("Draw_E", "Draw E").SetValue(true));
                    drawMenu.AddItem(new MenuItem("Draw_R", "Draw R").SetValue(true));

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
            if (Q.IsReady())
                spellCombo.Add(SpellSlot.Q);
            if (E.IsReady())
                spellCombo.Add(SpellSlot.E);
            if (R.IsReady())
                spellCombo.Add(SpellSlot.R);
            return spellCombo;
        }

        private float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = (float)ObjectManager.Player.GetComboDamage(target, GetSpellCombo());
            return (float)(comboDamage + ObjectManager.Player.GetAutoAttackDamage(target));
        }


        public override void OnDraw()
        {
            if (Menu.Item("Draw_Disabled").GetValue<bool>())
            {
                xSLxOrbwalker.DisableDrawing();
                return;
            }
            xSLxOrbwalker.EnableDrawing();

            if (Menu.Item("Draw_Q").GetValue<bool>())
                if (Q.Level > 0)
                    Utility.DrawCircle(MyHero.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_W").GetValue<bool>())
                if (W.Level > 0)
                    Utility.DrawCircle(MyHero.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_E").GetValue<bool>())
                if (E.Level > 0)
                    Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_R").GetValue<bool>())
                if (R.Level > 0)
                    Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);
        }

        public override void OnProcessPacket(GamePacketEventArgs args)
        {
            if (IsSpellActive("Q") && Menu.Item("Q_AA_Windup").GetValue<bool>())
            {
                var g = new GamePacket(args.PacketData);
                if (g.Header != 0xFE) return;
                if (Packet.MultiPacket.OnAttack.Decoded(args.PacketData).Type != Packet.AttackTypePacket.TargetedAA)
                    return;
                g.Position = 1;
                var target = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>(g.ReadInteger());
                if (!(target is Obj_AI_Hero) || !target.IsEnemy) return;
                if (!(Vector3.Distance(target.Position, ObjectManager.Player.Position) <= 925)) return;
                var qCollisionCount = Q.GetPrediction(target).CollisionObjects.Count;
                if (qCollisionCount == 0)
                {
                    Q.Cast(target);
                }
            }
        }

        public override void OnPassive()
        {
            //change Q range
            var qMaxRange = Menu.Item("Q_Max_Range").GetValue<Slider>().Value;
            Q.Range = qMaxRange;

            MEC_R();

            //Auto Q
            var qDashing = Menu.Item("Auto_Q_Dashing").GetValue<bool>();
            var qImmobile = Menu.Item("Auto_Q_Immobile").GetValue<bool>();
            var qSlow = Menu.Item("Auto_Q_Slow").GetValue<bool>();

            foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(Q.Range) && x.IsEnemy &&
                Menu.Item("Dont_Q" + x.BaseSkinName) != null && Menu.Item("Dont_Q" + x.BaseSkinName).GetValue<bool>() == false))
            {
                var qPrediction = Q.GetPrediction(target);

                if (qPrediction.Hitchance == HitChance.Immobile && qImmobile && Q.IsReady())
                    Q.Cast(target, UsePackets());

                if (qPrediction.Hitchance == HitChance.Dashing && qDashing && Q.IsReady())
                    Q.Cast(target, UsePackets());

                if (target.HasBuffOfType(BuffType.Slow) && qSlow && Q.IsReady())
                    Q.Cast(target, UsePackets());
            }
        }

        public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.Medium || unit.IsAlly)
                return;

            if (Menu.Item("Misc_R_Interrupt").GetValue<bool>() && unit.IsValidTarget(R.Range))
                R.Cast(UsePackets());
        }

        public override void OnCombo()
        {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);

            if (IsSpellActive("W") && W.IsReady() && MyHero.Distance(target) < 500)
                W.Cast(UsePackets());

            if (IsSpellActive("Q") && Q_Check(target))
                Cast_BasicSkillshot_Enemy(Q, SimpleTs.DamageType.Magical);

            if (IsSpellActive("E") && E.IsReady() && MyHero.Distance(target) < 300 && !Menu.Item("Misc_E_Reset").GetValue<bool>())
                E.Cast();

            if (IsSpellActive("R") && R.IsReady() && !Q.IsReady())
            {
                var rPred = Prediction.GetPrediction(target, .25f);

                if (rPred.Hitchance >= HitChance.High && MyHero.Distance(rPred.UnitPosition) < R.Range)
                    R.Cast(UsePackets());
            }
        }

        public override void OnHarass()
        {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);

            if (IsSpellActive("W") && W.IsReady() && MyHero.Distance(target) < 500)
                W.Cast(UsePackets());

            if (IsSpellActive("Q") && Q_Check(target))
                Cast_BasicSkillshot_Enemy(Q, SimpleTs.DamageType.Magical);

            if (IsSpellActive("E") && E.IsReady() && MyHero.Distance(target) < 300 && !Menu.Item("Misc_E_Reset").GetValue<bool>())
                E.Cast();
        }

        public override void OnFlee()
        {
            if (W.IsReady() && IsSpellActive("W"))
                W.Cast(UsePackets());
        }

		public override void OnAfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
		{
			if(!unit.IsMe || !Menu.Item("Misc_E_Reset").GetValue<bool>())
				return;
			if(IsSpellActive("E") && E.IsReady())
				E.Cast();
		}

	    private void MEC_R()
        {
            var mecRMin = Menu.Item("Misc_MEC_R").GetValue<Slider>().Value;

            var rHit = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(R.Range) && !x.IsDead && x.IsVisible && x.IsEnemy).Select(target => Prediction.GetPrediction(target, .25f)).Count(pred => MyHero.Distance(pred.UnitPosition) < R.Range);

            if (rHit >= mecRMin)
                R.Cast(UsePackets());
        }

        private bool Q_Check(Obj_AI_Base target)
        {
            if (target.HasBuffOfType(BuffType.SpellImmunity))
                return false;

            if (Menu.Item("Dont_Q" + target.BaseSkinName) != null)
                if (Menu.Item("Dont_Q" + target.BaseSkinName).GetValue<bool>())
                    return false;

            var qMinRange = Menu.Item("Q_Min_Range").GetValue<Slider>().Value;
            return !(MyHero.Distance(target) < qMinRange);
        }
    }
}
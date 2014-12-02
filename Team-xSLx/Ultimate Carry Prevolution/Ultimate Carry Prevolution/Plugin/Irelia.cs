using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
    class Irelia : Champion
    {
        public Irelia()
        {
            SetSpells();
            LoadMenu();
        }

        private void SetSpells()
        {
            Q = new Spell(SpellSlot.Q, 650);
            
            W = new Spell(SpellSlot.W);

            E = new Spell(SpellSlot.E, 425);

            R = new Spell(SpellSlot.R, 1000);
            R.SetSkillshot(0, 80f, 1400f, false ,SkillshotType.SkillshotLine);
        }

        private void LoadMenu()
        {
            var champMenu = new Menu("Irelia Plugin", "Irelia");
            {
                var spellMenu = new Menu("SpellMenu", "SpellMenu");
                {
                    var qMenu = new Menu("QMenu", "QMenu");
                    {
                        qMenu.AddItem(new MenuItem("Q_Min_Distance", "Min range to Q").SetValue(new Slider(300, 0, 600)));
                        qMenu.AddItem(new MenuItem("Q_Gap_Close", "Q Minion to Gap Close").SetValue(true));
                        qMenu.AddItem(new MenuItem("Q_Under_Tower", "Q Enemy Under Tower").SetValue(false));
                        spellMenu.AddSubMenu(qMenu);
                    }

                    var eMenu = new Menu("EMenu", "EMenu");
                    {
                        eMenu.AddItem(new MenuItem("E_Only_Stun", "Save E to Stun").SetValue(true));
                        eMenu.AddItem(new MenuItem("E_Running", "E On Running Enemy").SetValue(true));
                        spellMenu.AddSubMenu(eMenu);
                    }

                    var rMenu = new Menu("RMenu", "RMenu");
                    {
                        rMenu.AddItem(new MenuItem("R_If_HP", "R If HP <=").SetValue(new Slider(20)));
                        //rMenu.AddItem(new MenuItem("R_Wait_Sheen", "Wait for Sheen").SetValue(false));

                        spellMenu.AddSubMenu(rMenu);
                    }

                    champMenu.AddSubMenu(spellMenu);
                }

                var comboMenu = new Menu("Combo", "Combo");
                {
                    AddSpelltoMenu(comboMenu, "Q", true);
                    AddSpelltoMenu(comboMenu, "W", true);
                    AddSpelltoMenu(comboMenu, "E", true);
                    AddSpelltoMenu(comboMenu, "R", true);
                    AddSpelltoMenu(comboMenu, "Ignite", true, "Use Ignite");
                    AddSpelltoMenu(comboMenu, "Botrk", true, "Use Botrk/Bilge");
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

                var lastHitMenu = new Menu("Lasthit", "Lasthit");
                {
                    AddSpelltoMenu(lastHitMenu, "Q", true);
                    AddSpelltoMenu(lastHitMenu, "LastHit_Tower_Q", true, "Do not use Under Tower");
                    AddManaManagertoMenu(lastHitMenu, 30);
                    champMenu.AddSubMenu(lastHitMenu);
                }

                var laneClearMenu = new Menu("LaneClear", "LaneClear");
                {
                    AddSpelltoMenu(laneClearMenu, "Q", true);
                    AddSpelltoMenu(laneClearMenu, "LastHit_Tower_Q", true, "Do not use Under Tower");
                    AddSpelltoMenu(laneClearMenu, "W", true);
                    AddSpelltoMenu(laneClearMenu, "R", true);
                    AddManaManagertoMenu(laneClearMenu, 0);
                    champMenu.AddSubMenu(laneClearMenu);
                }

                var miscMenu = new Menu("Misc", "Misc");
                {
                    //miscMenu.AddItem(new MenuItem("Cast_EQ", "Cast EQ nearest target").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                    miscMenu.AddItem(new MenuItem("E_Gap_Closer", "Use E On Gap Closer").SetValue(true));
                    miscMenu.AddItem(new MenuItem("QE_Interrupt", "Use Q/E to interrupt").SetValue(true));
                    champMenu.AddSubMenu(miscMenu);
                }

                var drawMenu = new Menu("Drawing", "Drawing");
                {
                    drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
                    drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
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
            if (Q.IsReady())
                spellCombo.Add(SpellSlot.Q);
            if (W.IsReady())
                spellCombo.Add(SpellSlot.W);
            if (E.IsReady())
                spellCombo.Add(SpellSlot.E);
            return spellCombo;
        }

        private float GetComboDamage(Obj_AI_Base target)
        {
			double comboDamage = (float)MyHero.GetComboDamage(target, GetSpellCombo());
            if (R.IsReady())
                comboDamage += MyHero.GetSpellDamage(target, SpellSlot.R)*4;

            if(Bilge.IsReady())
                comboDamage += MyHero.GetItemDamage(target, Damage.DamageItems.Bilgewater);

            if (Botrk.IsReady())
                comboDamage += MyHero.GetItemDamage(target, Damage.DamageItems.Botrk);

            if (Ignite != SpellSlot.Unknown && MyHero.SummonerSpellbook.CanUseSpell(Ignite) == SpellState.Ready)
                comboDamage += MyHero.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
			return (float)(comboDamage + MyHero.GetAutoAttackDamage(target) * 4);
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

            if (Menu.Item("Draw_E").GetValue<bool>())
                if (E.Level > 0)
                    Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_R").GetValue<bool>())
                if (R.Level > 0)
                    Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_R_Killable").GetValue<bool>())
            {
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(5000) && !x.IsDead && x.IsEnemy).OrderBy(x => x.Health))
                {
                    Vector2 wts = Drawing.WorldToScreen(target.Position);
                    if (getComboDmgPercent(target) < 30 && R.IsReady())
                    {
                        Drawing.DrawText(wts[0] - 20, wts[1], Color.White, "KILL!!!");

                    }

                    var enemyhp = target.Health / target.MaxHealth * 100;
                    if (GetHealthPercent() < enemyhp && E.IsReady())
                        Drawing.DrawText(wts[0] - 20, wts[1] - 30, Color.White, "Stunnable");
                }
            }
        }

        public override void OnLasthit()
        {
            if(IsSpellActive("Q") && ManaManagerAllowCast())
                Cast_Q_Last_Hit();
        }

        public override void OnCombo()
        {

            if (IsSpellActive("Q"))
                Cast_Q(true);
            if (IsSpellActive("W"))
                Cast_W();
            if (IsSpellActive("E"))
                Cast_E();

            var qTarget = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            if (qTarget != null)
            {
                if (GetComboDamage(qTarget) >= qTarget.Health && IsSpellActive("Ignite") &&
                    Ignite != SpellSlot.Unknown && MyHero.Distance(qTarget) < 650 &&
                    MyHero.SummonerSpellbook.CanUseSpell(Ignite) == SpellState.Ready)
                    Use_Ignite(qTarget);

                if (IsSpellActive("Botrk"))
                {
                    if (Bilge.IsReady() && getComboDmgPercent(qTarget) < 5 &&
                        !qTarget.HasBuffOfType(BuffType.Slow))
                        Use_Bilge(qTarget);

                    if (Botrk.IsReady() && getComboDmgPercent(qTarget) < 5 &&
                        !qTarget.HasBuffOfType(BuffType.Slow))
                        Use_Botrk(qTarget);
                }
            }

            if(IsSpellActive("R"))
                Cast_R();
        }

        public override void OnHarass()
        {
            if (IsSpellActive("Q"))
                Cast_Q(false);
            if (IsSpellActive("W"))
                Cast_W();
            if (IsSpellActive("E"))
                Cast_E();
        }

        public override void OnLaneClear()
        {
            if (ManaManagerAllowCast())
            {
                if (IsSpellActive("Q"))
                    Cast_Q_Last_Hit();

                if (IsSpellActive("W") && W.IsReady())
                {
                    var allMinionW = MinionManager.GetMinions(MyHero.ServerPosition, xSLxOrbwalker.GetAutoAttackRange(),
                        MinionTypes.All, MinionTeam.NotAlly);
                    if (allMinionW.Count > 0)
                        W.Cast();
                }

                if (IsSpellActive("R") && R.IsReady())
                {
                    var allMinionR = MinionManager.GetMinions(MyHero.ServerPosition, R.Range, MinionTypes.All,
                        MinionTeam.NotAlly);
                    var R_Pred = R.GetLineFarmLocation(allMinionR);
                    if (R_Pred.MinionsHit > 0)
                        R.Cast(R_Pred.Position);
                }
            }

        }

        public override void OnGapClose(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("E_Gap_Closer").GetValue<bool>()) return;

            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
                E.Cast(gapcloser.Sender, UsePackets());

        }

        public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.Medium || unit.IsAlly)
                return;

            if (Menu.Item("QE_Interrupt").GetValue<bool>())
            {
                var Enemy_hp = unit.Health/unit.MaxHealth*100;
                if (GetHealthPercent() > Enemy_hp)
                    return;

                if (unit.IsValidTarget(E.Range))
                    E.Cast(unit, UsePackets());

                if (unit.IsValidTarget(Q.Range))
                {
                    Q.Cast(unit, UsePackets());
                    E.Cast(unit, UsePackets());
                }
            }
        }

        private void Cast_Q(bool combo)
        {
            var target = SimpleTs.GetTarget(Q.Range * 2, SimpleTs.DamageType.Physical);

            if (Q.IsReady() && target != null)
            {
                var Min_Distance = Menu.Item("Q_Min_Distance").GetValue<Slider>().Value;

                if(!Menu.Item("Q_Under_Tower").GetValue<bool>())
                    if (Utility.UnderTurret(target, true))
                        return;

                if (MyHero.Distance(target) > Q.Range / 2 && Menu.Item("Q_Gap_Close").GetValue<bool>())
                {
                    var allMinionQ = MinionManager.GetMinions(MyHero.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

                    Obj_AI_Base Best_Minion = allMinionQ[0];

                    foreach (var minion in allMinionQ)
                    {
                        double dmg = 0;

                        dmg += MyHero.GetSpellDamage(minion, SpellSlot.Q);
                        if (W.IsReady() || MyHero.HasBuff("ireliahitenstylecharged", true))
                            dmg += MyHero.GetSpellDamage(minion, SpellSlot.W);

                        if (target.Distance(minion) < Q.Range && MyHero.Distance(minion) < Q.Range && target.Distance(minion) < target.Distance(MyHero) && dmg > minion.Health + 40)
                            if (target.Distance(minion) < target.Distance(Best_Minion))
                                Best_Minion = minion;
                    }

                    //check if can Q without activating
                    if (Best_Minion != null)
                    {
                        if (target.Distance(Best_Minion) < Q.Range && MyHero.Distance(Best_Minion) < Q.Range)
                        {
                            var dmg2 = MyHero.GetSpellDamage(Best_Minion, SpellSlot.Q);

                            if (dmg2 > Best_Minion.Health + 40)
                            {
                                Q.Cast(Best_Minion, UsePackets());
                                return;
                            }

                            if (W.IsReady() || MyHero.HasBuff("ireliahitenstylecharged", true))
                                dmg2 += MyHero.GetSpellDamage(Best_Minion, SpellSlot.W);

                            if (dmg2 > Best_Minion.Health)
                            {
                                W.Cast(UsePackets());
                                Q.Cast(Best_Minion, UsePackets());
                                return;
                            }
                        }
                    }
                }

                if (MyHero.Distance(target) > Min_Distance && MyHero.Distance(target) < Q.Range + target.BoundingRadius)
                {
                    Q.Cast(target, UsePackets());
                }
            }
        }

        private void Cast_Q_Last_Hit()
        {
            var allMinionQ = MinionManager.GetMinions(MyHero.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (allMinionQ.Count > 0 && Q.IsReady())
            {
                
                foreach (var minion in allMinionQ)
                {
                    double dmg = MyHero.GetSpellDamage(minion, SpellSlot.Q);

                    if (MyHero.HasBuff("ireliahitenstylecharged", true))
                        dmg += MyHero.GetSpellDamage(minion, SpellSlot.W);
                    

                    if (dmg > minion.Health + 35)
                    {
                        if (IsSpellActive("LastHit_Tower_Q"))
                        {
                            if (!Utility.UnderTurret(minion, true))
                            {
                                Q.Cast(minion, UsePackets());
                                return;
                            }
                        }
                        else
                            Q.Cast(minion, UsePackets());
                    }
                }
            }
        }

        private void Cast_W()
        {
            var target = SimpleTs.GetTarget(200, SimpleTs.DamageType.Physical);

            if (target != null && W.IsReady())
            {
                W.Cast(UsePackets());
            }
        }

        private void Cast_E()
        {
            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);

            if (target != null && E.IsReady())
            {
                if (Menu.Item("E_Only_Stun").GetValue<bool>())
                {
                    var Target_Health_Percent = target.Health/target.MaxHealth*100;

                    if (GetHealthPercent() < Target_Health_Percent)
                    {
                        E.Cast(target, UsePackets());
                        return;
                    }
                }

                if (Menu.Item("E_Running").GetValue<bool>())
                {
                    var pred = Prediction.GetPrediction(target, 1f);

                    if (MyHero.Distance(target) < MyHero.Distance(pred.UnitPosition) && MyHero.Distance(target) > 200)
                        E.Cast(target, UsePackets());
                }
            }
        }

        private void Cast_R()
        {
            var target = SimpleTs.GetTarget(MyHero.Spellbook.GetSpell(SpellSlot.R).ToggleState == 1 ? Q.Range : R.Range,
                SimpleTs.DamageType.Physical);

            if (target != null && R.IsReady())
            {
                if (!MyHero.HasBuff("IreliaTranscendentBlades"))
                {
                    if (getComboDmgPercent(target) < 25)
                        R.Cast(target, UsePackets());

                    var R_Hp_value = Menu.Item("R_If_HP").GetValue<Slider>().Value;
                    if (GetHealthPercent() <= R_Hp_value)
                        R.Cast(target, UsePackets());
                }
                else if (MyHero.HasBuff("IreliaTranscendentBlades"))
                {
                    R.Cast(target, UsePackets());
                }
            }
        }

        private float getComboDmgPercent(Obj_AI_Hero target)
        {
            double comboDamage = (float)MyHero.GetComboDamage(target, GetSpellCombo());
            comboDamage += MyHero.GetSpellDamage(target, SpellSlot.R) * 4;
            comboDamage += MyHero.GetAutoAttackDamage(target) * 4;

            if (Bilge.IsReady())
                comboDamage += MyHero.GetItemDamage(target, Damage.DamageItems.Bilgewater);

            if (Botrk.IsReady())
                comboDamage += MyHero.GetItemDamage(target, Damage.DamageItems.Botrk);

            var Pred_HP = target.Health - comboDamage;
            var Pred_HP_Percent = Pred_HP / target.MaxHealth * 100;

            return (float)Pred_HP_Percent;
        }
    }
}

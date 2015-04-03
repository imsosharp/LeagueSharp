#region LICENSE

// Copyright 2014 Support
// Protector.cs is part of Support.
// 
// Support is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Support is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Support. If not, see <http://www.gnu.org/licenses/>.
// 
// Filename: Support/Support/Protector.cs
// Created:  05/10/2014
// Date:     26/12/2014/16:23
// Author:   h3h3

#endregion

namespace Support
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Support.Evade;
    using Support.Util;
    using Collision = Support.Evade.Collision;
    using SpellData = LeagueSharp.SpellData;

    #endregion

    internal class ProtectorSpell
    {
        public string Name { get; set; }
        public string ChampionName { get; set; }
        public Spell Spell { get; set; }
        public int HpBuffer { get; set; }
        public bool Harass { get; set; }
        public bool Targeted { get; set; }
        public bool Cc { get; set; }

        public bool IsActive(Obj_AI_Hero hero)
        {
            return Protector.Menu.Item("Spells." + Name + "." + hero.ChampionName).GetValue<bool>();
        }
    }

    internal class ProtectorItem
    {
        public string Name { get; set; }
        public Items.Item Item { get; set; }
        public int HpBuffer { get; set; }
        public bool Targeted { get; set; }
        public bool Cc { get; set; }

        public bool IsActive(Obj_AI_Hero hero)
        {
            return Protector.Menu.Item("Items." + Name + "." + hero.ChampionName).GetValue<bool>();
        }
    }

    internal class Protector
    {
        public delegate void OnSkillshotProtectionH(Obj_AI_Hero target, List<Skillshot> skillshots);

        public delegate void OnTargetedProtectionH(Obj_AI_Base caster, Obj_AI_Hero target, SpellData spell);

        public static List<BuffType> CcTypes = new List<BuffType>
        {
            BuffType.Fear,
            BuffType.Polymorph,
            BuffType.Snare,
            BuffType.Stun,
            BuffType.Suppression,
            BuffType.Taunt,
            BuffType.Charm
        };

        public static List<ProtectorSpell> ProtectorSpells = new List<ProtectorSpell>();
        public static List<ProtectorItem> ProtectorItems = new List<ProtectorItem>();
        public static SpellList<Skillshot> DetectedSkillshots = new SpellList<Skillshot>();
        public static Menu Menu;
        public static AutoBushRevealer Revealer;
        private static bool _isInitComplete;

        private static bool UsePackets
        {
            get { return Menu.SubMenu("Misc").Item("UsePackets").GetValue<bool>(); }
        }

        private static int IsAboutToHitTime
        {
            get { return Menu.Item("IsAboutToHitTime").GetValue<Slider>().Value; }
        }

        public static event OnSkillshotProtectionH OnSkillshotProtection;

        public static event OnTargetedProtectionH OnTargetedProtection;

        public static void Init()
        {
            if (!_isInitComplete)
            {
                // Init stuff
                InitSpells();
                CreateMenu();
                Collision.Init();
                Revealer = new AutoBushRevealer(Menu.SubMenu("Misc"));

                // Internal events
<<<<<<< HEAD
                Game.OnUpdate += OnUpdate;
=======
                Game.OnUpdate += OnGameUpdate;
>>>>>>> origin/master
                SkillshotDetector.OnDetectSkillshot += OnDetectSkillshot;
                Obj_AI_Base.OnProcessSpellCast += HeroOnProcessSpellCast;
                Obj_AI_Base.OnProcessSpellCast += TurretOnProcessSpellCast;
                GameObject.OnCreate += SpellMissile_OnCreate;

                // Actives
                Game.OnUpdate += CcCheck;
                OnSkillshotProtection += ProtectorOnOnSkillshotProtection;
                OnTargetedProtection += ProtectorOnOnTargetedProtection;

                // Debug
                //OnSkillshotProtection += Protector_OnSkillshotProtection;
                //OnTargetedProtection += Protector_OnTargetedProtection;

                Helpers.PrintMessage(string.Format("Protector by h3h3 loaded!"));
                Console.WriteLine("Protector Init Complete");
                _isInitComplete = true;
            }
        }

        private static void CreateMenu()
        {
            Menu = new Menu("AutoSharp: Protector", "Protector", true);

            // Marksman Potion Manager
            new PotionManager(Menu);

            // detector
            var detector = Menu.AddSubMenu(new Menu("Detector", "Detector"));

            // detector targeted
            var targeted = detector.AddSubMenu(new Menu("Targeted", "Targeted"));
            targeted.AddItem(new MenuItem("TargetedActive", "Active").SetValue(true));
            foreach (var ally in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsAlly))
            {
                targeted.AddItem(
                    new MenuItem("Detector.Targeted." + ally.ChampionName, ally.ChampionName).SetValue(true));
            }

            // detector skillshots
            var skillshot = detector.AddSubMenu(new Menu("Skillshots", "Skillshots"));
            skillshot.AddItem(new MenuItem("SkillshotsActive", "Active").SetValue(true));
            skillshot.AddItem(new MenuItem("IsAboutToHitTime", "IsAboutToHit Time").SetValue(new Slider(200, 0, 400)));
            foreach (var ally in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsAlly))
            {
                skillshot.AddItem(
                    new MenuItem("Detector.Skillshots." + ally.ChampionName, ally.ChampionName).SetValue(true));
            }

            // spells
            var spells = Menu.AddSubMenu(new Menu("Spells", "Spells"));
            foreach (var spell in ProtectorSpells.Where(s => s.ChampionName == ObjectManager.Player.ChampionName))
            {
                var spellMenu = spells.AddSubMenu(new Menu(spell.Name, spell.Name));
                foreach (var ally in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsAlly))
                {
                    spellMenu.AddItem(
                        new MenuItem("Spells." + spell.Name + "." + ally.ChampionName, ally.ChampionName).SetValue(true));
                }
            }

            // items
            var items = Menu.AddSubMenu(new Menu("Items", "Items"));
            foreach (var item in ProtectorItems)
            {
                var itemsMenu = items.AddSubMenu(new Menu(item.Name, item.Name));
                foreach (var ally in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsAlly))
                {
                    itemsMenu.AddItem(
                        new MenuItem("Items." + item.Name + "." + ally.ChampionName, ally.ChampionName).SetValue(true));
                }
            }

            // cc
            var cc = Menu.AddSubMenu(new Menu("CC", "CC"));
            foreach (var b in CcTypes)
            {
                cc.AddItem(new MenuItem(b.ToString(), b.ToString()).SetValue(true));
            }

            // misc
            var misc = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            misc.AddItem(new MenuItem("UsePackets", "Use Packets").SetValue(true));


            Menu.AddToMainMenu();
        }

        private static void ProtectorOnOnTargetedProtection(Obj_AI_Base caster, Obj_AI_Hero target, SpellData spell)
        {
            ProtectionIntegration(caster, target, spell.Name);
        }

        private static void ProtectorOnOnSkillshotProtection(Obj_AI_Hero target, IEnumerable<Skillshot> skillshots)
        {
            foreach (var skillshot in skillshots)
            {
                ProtectionIntegration(skillshot.Unit, target, skillshot.SpellData.SpellName);
            }
        }

        private static void CcCheck(EventArgs args)
        {
            try
            {
                var mikael = ProtectorItems.First(); // TODO: ugly shit + check if is in range

                if (!mikael.Item.IsReady() || ObjectManager.Player.IsDead ||
                    ObjectManager.Player.IsChannelingImportantSpell())
                {
                    return;
                }

                foreach (var hero in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(h => h.IsAlly && !h.IsDead)
                        .OrderByDescending(h => h.FlatPhysicalDamageMod)
                        .Where(mikael.Item.IsInRange))
                {
                    foreach (var buff in CcTypes)
                    {
                        if (hero.HasBuffOfType(buff) && Menu.SubMenu("CC").Item(buff.ToString()).GetValue<bool>())
                        {
                            if (mikael.IsActive(hero) && hero.CountEnemiesInRange(800) > 0)
                            {
                                mikael.Item.Cast(hero);
                                Console.WriteLine(
                                    "Cast CC: " + mikael.Name + " -> " + hero.ChampionName + "(" + buff + ")");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void ProtectionIntegration(Obj_AI_Base caster, Obj_AI_Hero target, string spell)
        {
            try
            {
                if (ObjectManager.Player.IsDead || ObjectManager.Player.IsChannelingImportantSpell())
                {
                    return;
                }

                foreach (var ps in
                    ProtectorSpells.Where(
                        s =>
                            s.ChampionName == ObjectManager.Player.ChampionName && s.Spell.IsReady() &&
                            s.Spell.IsInRange(target) && s.IsActive(target)))
                {
                    if (ps.Harass || caster.WillKill(target, spell, ps.HpBuffer))
                    {
                        if (ps.Targeted)
                        {
                            ps.Spell.CastOnUnit(target);
                        }
                        else
                        {
                            ps.Spell.Cast();
                        }

                        Console.WriteLine("Cast PS: " + ps.Name + " -> " + target.ChampionName);
                    }
                }

                foreach (var pi in
                    ProtectorItems.Where(i => i.Item.IsReady() && i.Item.IsInRange(target) && i.IsActive(target)))
                {
                    if (caster.WillKill(target, spell, pi.HpBuffer))
                    {
                        if (pi.Targeted)
                        {
                            pi.Item.Cast(target);
                        }
                        else
                        {
                            pi.Item.Cast();
                        }

                        Console.WriteLine("Cast PI: " + pi.Name + " -> " + target.ChampionName);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void TurretOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (!sender.IsValid<Obj_AI_Turret>())
                {
                    return;
                }

                if (!args.Target.IsValid<Obj_AI_Hero>())
                {
                    return;
                }

                if (!Menu.Item("TargetedActive").GetValue<bool>())
                {
                    return;
                }

                if (sender.IsAlly)
                {
                    return;
                }

                var caster = (Obj_AI_Turret) sender;
                var target = (Obj_AI_Hero) args.Target;

                if (
                    Menu.SubMenu("Detector")
                        .SubMenu("Targeted")
                        .Item("Detector.Targeted." + target.ChampionName)
                        .GetValue<bool>())
                {
                    if (OnTargetedProtection != null)
                    {
                        OnTargetedProtection(caster, target, args.SData);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void HeroOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (!sender.IsValid<Obj_AI_Hero>())
                {
                    return;
                }

                if (!Menu.Item("TargetedActive").GetValue<bool>())
                {
                    return;
                }

                if (sender.IsAlly)
                {
                    return;
                }

                if (!args.Target.IsValid<Obj_AI_Hero>() || args.Target.IsEnemy)
                {
                    return;
                }

                var caster = (Obj_AI_Hero) sender;
                var target = (Obj_AI_Hero) args.Target;

                if (
                    Menu.SubMenu("Detector")
                        .SubMenu("Targeted")
                        .Item("Detector.Targeted." + target.ChampionName)
                        .GetValue<bool>())
                {
                    if (OnTargetedProtection != null)
                    {
                        OnTargetedProtection(caster, target, args.SData);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void SpellMissile_OnCreate(GameObject sender, EventArgs args)
        {
            try
            {
                if (!sender.IsValid<Obj_SpellMissile>())
                {
                    return;
                }

                if (!Menu.Item("TargetedActive").GetValue<bool>())
                {
                    return;
                }

                var missile = (Obj_SpellMissile) sender;

                if (!missile.SpellCaster.IsValid<Obj_AI_Hero>() || !missile.SpellCaster.IsEnemy)
                {
                    return;
                }

                if (!missile.Target.IsValid<Obj_AI_Hero>() || !missile.Target.IsAlly)
                {
                    return;
                }

                var caster = (Obj_AI_Hero) missile.SpellCaster;
                var target = (Obj_AI_Hero) missile.Target;

                if (
                    Menu.SubMenu("Detector")
                        .SubMenu("Targeted")
                        .Item("Detector.Targeted." + target.ChampionName)
                        .GetValue<bool>())
                {
                    if (OnTargetedProtection != null)
                    {
                        OnTargetedProtection(caster, target, missile.SData);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            try
            {
                if (!Menu.Item("SkillshotsActive").GetValue<bool>())
                {
                    return;
                }

                //Remove the detected skillshots that have expired.
                DetectedSkillshots.RemoveAll(skillshot => !skillshot.IsActive());

                //Trigger OnUpdate on each skillshot.
                foreach (var skillshot in DetectedSkillshots)
                {
                    skillshot.Game_OnUpdate();
                }

                // Protect
                foreach (var ally in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(h => h.IsAlly && h.IsValidTarget(2000, false))
                        .OrderByDescending(h => h.FlatPhysicalDamageMod))
                {
                    var allySafeResult = IsSafe(ally.ServerPosition.To2D());

                    if (!allySafeResult.IsSafe && IsAboutToHit(ally, IsAboutToHitTime))
                    {
                        if (
                            Menu.SubMenu("Detector")
                                .SubMenu("Skillshots")
                                .Item("Detector.Skillshots." + ally.ChampionName)
                                .GetValue<bool>())
                        {
                            if (OnSkillshotProtection != null)
                            {
                                OnSkillshotProtection(ally, allySafeResult.SkillshotList);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnDetectSkillshot(Skillshot skillshot)
        {
            try
            {
                //Check if the skillshot is already added.
                var alreadyAdded = false;

                // Integration disabled
                if (!Menu.Item("SkillshotsActive").GetValue<bool>())
                {
                    return;
                }

                foreach (var item in DetectedSkillshots)
                {
                    if (item.SpellData.SpellName == skillshot.SpellData.SpellName &&
                        (item.Unit.NetworkId == skillshot.Unit.NetworkId &&
                         (skillshot.Direction).AngleBetween(item.Direction) < 5 &&
                         (skillshot.Start.Distance(item.Start) < 100 || skillshot.SpellData.FromObjects.Length == 0)))
                    {
                        alreadyAdded = true;
                    }
                }

                //Check if the skillshot is from an ally.
                if (skillshot.Unit.IsAlly)
                {
                    return;
                }

                //Check if the skillshot is too far away.
                if (skillshot.Start.Distance(ObjectManager.Player.ServerPosition.To2D()) >
                    (skillshot.SpellData.Range + skillshot.SpellData.Radius + 1000) * 1.5)
                {
                    return;
                }


                //Add the skillshot to the detected skillshot list.
                if (!alreadyAdded)
                {
                    //Multiple skillshots like twisted fate Q.
                    if (skillshot.DetectionType == DetectionType.ProcessSpell)
                    {
                        if (skillshot.SpellData.MultipleNumber != -1)
                        {
                            var originalDirection = skillshot.Direction;

                            for (var i = -(skillshot.SpellData.MultipleNumber - 1) / 2;
                                i <= (skillshot.SpellData.MultipleNumber - 1) / 2;
                                i++)
                            {
                                var end = skillshot.Start +
                                          skillshot.SpellData.Range *
                                          originalDirection.Rotated(skillshot.SpellData.MultipleAngle * i);
                                var skillshotToAdd = new Skillshot(
                                    skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start,
                                    end, skillshot.Unit);

                                DetectedSkillshots.Add(skillshotToAdd);
                            }
                            return;
                        }

                        if (skillshot.SpellData.SpellName == "UFSlash")
                        {
                            skillshot.SpellData.MissileSpeed = 1600 + (int) skillshot.Unit.MoveSpeed;
                        }

                        if (skillshot.SpellData.Invert)
                        {
                            var newDirection = -(skillshot.End - skillshot.Start).Normalized();
                            var end = skillshot.Start + newDirection * skillshot.Start.Distance(skillshot.End);
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end,
                                skillshot.Unit);
                            DetectedSkillshots.Add(skillshotToAdd);
                            return;
                        }

                        if (skillshot.SpellData.Centered)
                        {
                            var start = skillshot.Start - skillshot.Direction * skillshot.SpellData.Range;
                            var end = skillshot.Start + skillshot.Direction * skillshot.SpellData.Range;
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                                skillshot.Unit);
                            DetectedSkillshots.Add(skillshotToAdd);
                            return;
                        }

                        if (skillshot.SpellData.SpellName == "SyndraE" || skillshot.SpellData.SpellName == "syndrae5")
                        {
                            var angle = 60;
                            var edge1 =
                                (skillshot.End - skillshot.Unit.ServerPosition.To2D()).Rotated(
                                    -angle / 2 * (float) Math.PI / 180);
                            var edge2 = edge1.Rotated(angle * (float) Math.PI / 180);

                            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                            {
                                var v = minion.ServerPosition.To2D() - skillshot.Unit.ServerPosition.To2D();
                                if (minion.Name == "Seed" && edge1.CrossProduct(v) > 0 && v.CrossProduct(edge2) > 0 &&
                                    minion.Distance(skillshot.Unit, false) < 800 && (minion.Team != ObjectManager.Player.Team))
                                {
                                    var start = minion.ServerPosition.To2D();
                                    var end = skillshot.Unit.ServerPosition.To2D()
                                        .Extend(
                                            minion.ServerPosition.To2D(),
                                            skillshot.Unit.Distance(minion, false) > 200 ? 1300 : 1000);

                                    var skillshotToAdd = new Skillshot(
                                        skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                                        skillshot.Unit);
                                    DetectedSkillshots.Add(skillshotToAdd);
                                }
                            }
                            return;
                        }

                        if (skillshot.SpellData.SpellName == "AlZaharCalloftheVoid")
                        {
                            var start = skillshot.End - skillshot.Direction.Perpendicular() * 400;
                            var end = skillshot.End + skillshot.Direction.Perpendicular() * 400;
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                                skillshot.Unit);
                            DetectedSkillshots.Add(skillshotToAdd);
                            return;
                        }

                        if (skillshot.SpellData.SpellName == "ZiggsQ")
                        {
                            var d1 = skillshot.Start.Distance(skillshot.End);
                            var d2 = d1 * 0.4f;
                            var d3 = d2 * 0.69f;


                            var bounce1SpellData = SpellDatabase.GetByName("ZiggsQBounce1");
                            var bounce2SpellData = SpellDatabase.GetByName("ZiggsQBounce2");

                            var bounce1Pos = skillshot.End + skillshot.Direction * d2;
                            var bounce2Pos = bounce1Pos + skillshot.Direction * d3;

                            bounce1SpellData.Delay =
                                (int) (skillshot.SpellData.Delay + d1 * 1000f / skillshot.SpellData.MissileSpeed + 500);
                            bounce2SpellData.Delay =
                                (int) (bounce1SpellData.Delay + d2 * 1000f / bounce1SpellData.MissileSpeed + 500);

                            var bounce1 = new Skillshot(
                                skillshot.DetectionType, bounce1SpellData, skillshot.StartTick, skillshot.End,
                                bounce1Pos, skillshot.Unit);
                            var bounce2 = new Skillshot(
                                skillshot.DetectionType, bounce2SpellData, skillshot.StartTick, bounce1Pos, bounce2Pos,
                                skillshot.Unit);

                            DetectedSkillshots.Add(bounce1);
                            DetectedSkillshots.Add(bounce2);
                        }

                        if (skillshot.SpellData.SpellName == "ZiggsR")
                        {
                            skillshot.SpellData.Delay =
                                (int)
                                    (1500 + 1500 * skillshot.End.Distance(skillshot.Start) / skillshot.SpellData.Range);
                        }

                        if (skillshot.SpellData.SpellName == "JarvanIVDragonStrike")
                        {
                            var endPos = new Vector2();

                            foreach (var s in DetectedSkillshots)
                            {
                                if (s.Unit.NetworkId == skillshot.Unit.NetworkId && s.SpellData.Slot == SpellSlot.E)
                                {
                                    endPos = s.End;
                                }
                            }

                            foreach (var m in ObjectManager.Get<Obj_AI_Minion>())
                            {
                                if (m.BaseSkinName == "jarvanivstandard" && m.Team == skillshot.Unit.Team &&
                                    skillshot.IsDanger(m.Position.To2D()))
                                {
                                    endPos = m.Position.To2D();
                                }
                            }

                            if (!endPos.IsValid())
                            {
                                return;
                            }

                            skillshot.End = endPos + 200 * (endPos - skillshot.Start).Normalized();
                            skillshot.Direction = (skillshot.End - skillshot.Start).Normalized();
                        }
                    }

                    if (skillshot.SpellData.SpellName == "OriannasQ")
                    {
                        var endCSpellData = SpellDatabase.GetByName("OriannaQend");

                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, endCSpellData, skillshot.StartTick, skillshot.Start, skillshot.End,
                            skillshot.Unit);

                        DetectedSkillshots.Add(skillshotToAdd);
                    }


                    //Dont allow fow detection.
                    if (skillshot.SpellData.DisableFowDetection && skillshot.DetectionType == DetectionType.RecvPacket)
                    {
                        return;
                    }

                    DetectedSkillshots.Add(skillshot);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        /// <summary>
        ///     Returns true if the point is not inside the detected skillshots.
        /// </summary>
        public static IsSafeResult IsSafe(Vector2 point)
        {
            var result = new IsSafeResult { SkillshotList = new List<Skillshot>() };

            foreach (var skillshot in DetectedSkillshots)
            {
                result.SkillshotList.Add(skillshot);
            }

            result.IsSafe = (result.SkillshotList.Count == 0);

            return result;
        }

        /// <summary>
        ///     Returns true if some detected skillshot is about to hit the unit.
        /// </summary>
        public static bool IsAboutToHit(Obj_AI_Base unit, int time)
        {
            time += 150;
            return DetectedSkillshots.Any(skillshot => skillshot.IsAboutToHit(time, unit));
        }

        private static void Protector_OnTargetedProtection(Obj_AI_Base caster, Obj_AI_Hero target, SpellData spell)
        {
            try
            {
                var text = string.Format(
                    "{0,-15} -> {1,-15} - {3} {2}", caster.BaseSkinName, target.BaseSkinName, spell.Name,
                    Math.Round(caster.GetSpellDamage(target, spell.Name)));

                Console.WriteLine(text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Protector_OnSkillshotProtection(Obj_AI_Hero target, IEnumerable<Skillshot> skillshots)
        {
            try
            {
                foreach (var skillshot in skillshots)
                {
                    var text = string.Format(
                        "{0,-15} -> {1,-15} - {3} {2}", skillshot.Unit.BaseSkinName, target.BaseSkinName,
                        skillshot.SpellData.SpellName,
                        Math.Round(skillshot.Unit.GetSpellDamage(target, skillshot.SpellData.SpellName)));

                    Console.WriteLine(text);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void InitSpells()
        {
            ProtectorSpells.Add(
                new ProtectorSpell
                {
                    Name = "Triumphant Roar",
                    ChampionName = "Alistar",
                    Spell = new Spell(SpellSlot.E, 575),
                    Harass = true
                });

            ProtectorSpells.Add(
                new ProtectorSpell
                {
                    Name = "Eye of the Storm",
                    ChampionName = "Janna",
                    Spell = new Spell(SpellSlot.E, 800),
                    Harass = true,
                    Targeted = true
                });

            ProtectorSpells.Add(
                new ProtectorSpell
                {
                    Name = "Inspire",
                    ChampionName = "Karma",
                    Spell = new Spell(SpellSlot.E, 800),
                    Harass = true,
                    Targeted = true
                });

            ProtectorSpells.Add(
                new ProtectorSpell
                {
                    Name = "Wild Growth",
                    ChampionName = "Lulu",
                    Spell = new Spell(SpellSlot.R, 900),
                    HpBuffer = 10,
                    Targeted = true
                });

            ProtectorSpells.Add(
                new ProtectorSpell
                {
                    Name = "Help, Pix!",
                    ChampionName = "Lulu",
                    Spell = new Spell(SpellSlot.E, 650),
                    Harass = true,
                    Targeted = true
                });

            ProtectorSpells.Add(
                new ProtectorSpell
                {
                    Name = "Ebb and Flow",
                    ChampionName = "Nami",
                    Spell = new Spell(SpellSlot.W, 725),
                    Harass = true,
                    Targeted = true
                });

            ProtectorSpells.Add(
                new ProtectorSpell
                {
                    Name = "Imbue",
                    ChampionName = "Taric",
                    Spell = new Spell(SpellSlot.Q, 750),
                    Harass = true,
                    Targeted = true
                });

            ProtectorSpells.Add(
                new ProtectorSpell
                {
                    Name = "Intervention",
                    ChampionName = "Kayle",
                    Spell = new Spell(SpellSlot.R, 900),
                    HpBuffer = 10,
                    Targeted = true
                });

            ProtectorSpells.Add(
                new ProtectorSpell
                {
                    Name = "Divine Blessing",
                    ChampionName = "Kayle",
                    Spell = new Spell(SpellSlot.W, 900),
                    Harass = true,
                    Targeted = true
                });

            ProtectorSpells.Add(
                new ProtectorSpell
                {
                    Name = "Chrono Shift",
                    ChampionName = "Zilean",
                    Spell = new Spell(SpellSlot.R, 900),
                    HpBuffer = 10,
                    Targeted = true
                });

            ProtectorSpells.Add(
                new ProtectorSpell
                {
                    Name = "Black Shield",
                    ChampionName = "Morgana",
                    Spell = new Spell(SpellSlot.E, 750),
                    Targeted = true,
                    Cc = true
                });

            ProtectorSpells.Add(
                new ProtectorSpell
                {
                    Name = "Dark Passage",
                    ChampionName = "Thresh",
                    Spell = new Spell(SpellSlot.W, 950),
                    HpBuffer = 40
                });

            ProtectorItems.Add(
                new ProtectorItem
                {
                    Name = "Mikael's Crucible",
                    Item = new Items.Item(3222, 750),
                    HpBuffer = 10,
                    Targeted = true,
                    Cc = true
                });

            ProtectorItems.Add(
                new ProtectorItem
                {
                    Name = "Locket of the Iron Solari",
                    Item = new Items.Item(3190, 600),
                    HpBuffer = 10
                });
        }

        public struct IsSafeResult
        {
            public bool IsSafe;
            public List<Skillshot> SkillshotList;
        }
    }
}
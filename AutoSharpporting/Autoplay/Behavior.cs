using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharpporting.Autoplay
{
    public class Behavior
    {
        public static void CarryIsNull()
        {
            Autoplay.BotLanePos.To3D().WalkAround();

            if (Autoplay.Bot.Distance(Autoplay.BotLanePos) < 1000)
            {
                if (Environment.TickCount - Autoplay._loaded > 60000 && !MetaHandler.ShouldSupportTopLane)
                {
                    if (
                        MetaHandler.AllyHeroes.FirstOrDefault(
                            hero =>
                                !hero.IsMe && hero.Distance(Autoplay.Bot.Position) < 4500 && !MetaHandler.HasSmite(hero)) !=
                        null)
                    {
                        Autoplay.Carry =
                            MetaHandler.AllyHeroes.FirstOrDefault(
                                hero =>
                                    !hero.IsMe && hero.Distance(Autoplay.Bot.Position) < 4500 &&
                                    !MetaHandler.HasSmite(hero));
                    }
                }
                if (Environment.TickCount - Autoplay._loaded > 60000 && MetaHandler.ShouldSupportTopLane)
                {
                    if (
                        MetaHandler.AllyHeroes.FirstOrDefault(
                            hero =>
                                !hero.IsMe && hero.Distance(Autoplay.TopLanePos) < 4500 && !MetaHandler.HasSmite(hero)) !=
                        null)
                    {
                        Autoplay.Carry =
                            MetaHandler.AllyHeroes.FirstOrDefault(
                                hero =>
                                    !hero.IsMe && hero.Distance(Autoplay.TopLanePos) < 4500 &&
                                    !MetaHandler.HasSmite(hero));
                    }
                }
            }
            if (Autoplay._byPassLoadedCheck && Autoplay.Carry == null)
            {
                if (
                    MetaHandler.AllyHeroes.FirstOrDefault(
                        hero => !hero.IsMe && !hero.InFountain() && !MetaHandler.HasSmite(hero)) != null)
                {
                    Autoplay.Carry =
                        MetaHandler.AllyHeroes.FirstOrDefault(
                            hero => !hero.IsMe && !hero.InFountain() && !MetaHandler.HasSmite(hero));
                }
            }
        }

        public static void CarryIsDead()
        {
            if (Autoplay.TempCarry == null || Autoplay.TempCarry.IsDead || Autoplay.TempCarry.InFountain())
            {
                if (
                    MetaHandler.AllyHeroes.FirstOrDefault(
                        hero =>
                            !hero.IsMe && !hero.InFountain() && !hero.IsDead &&
                            !MetaHandler.HasSmite(hero)) != null)
                {
                    Autoplay.TempCarry =
                        MetaHandler.AllyHeroes.FirstOrDefault(
                            hero =>
                                !hero.IsMe && !hero.InFountain() && !hero.IsDead &&
                                !MetaHandler.HasSmite(hero));
                }
                if (
                    MetaHandler.AllyHeroes.FirstOrDefault(
                        hero =>
                            !hero.IsMe && !hero.InFountain() && !hero.IsDead &&
                            !MetaHandler.HasSmite(hero)) == null &&
                    MetaHandler.AllyHeroes.FirstOrDefault(
                        hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead) != null)
                {
                    //well fuck, let's follow the jungler -sighs-
                    Autoplay.TempCarry =
                        MetaHandler.AllyHeroes.FirstOrDefault(
                            hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead);
                }
                if (!MetaHandler.AllyHeroes.Any(hero => !hero.IsMe && !hero.IsDead))
                    //everyone is dead
                {
                    if (!Autoplay.Bot.InFountain())
                    {
                        Autoplay.NearestAllyTurret = MetaHandler.AllyTurrets.FirstOrDefault();
                        if (Autoplay.NearestAllyTurret != null)
                        {
                            Autoplay._saferecall.X = Autoplay.NearestAllyTurret.Position.X + Autoplay._safe;
                            Autoplay._saferecall.Y = Autoplay.NearestAllyTurret.Position.Y;
                            Autoplay._tookRecallDecision = true;
                            if (Autoplay.Bot.Position.Distance(Autoplay._saferecall.To3D()) < 200)
                            {
                                Autoplay.Bot.Spellbook.CastSpell(SpellSlot.Recall);
                            }
                            else
                            {
                                Autoplay.Bot.IssueOrder(GameObjectOrder.MoveTo, Autoplay._saferecall.To3D());
                            }
                        }
                    }
                }
            }
            if (Autoplay.TempCarry != null)
            {
                Console.WriteLine("Autoplay.Carry dead or afk, following: " + Autoplay.TempCarry.ChampionName);
                Autoplay._frontline.X = Autoplay.TempCarry.Position.X + Autoplay._chosen;
                Autoplay._frontline.Y = Autoplay.TempCarry.Position.Y + Autoplay._chosen;
                if (
                    !(Autoplay.TempCarry.UnderTurret(true) &&
                      MetaHandler.NearbyAllyMinions(Autoplay.TempCarry, 400) < 2) &&
                    Autoplay.IsBotSafe())
                {
                    if (Autoplay.TempCarry.Distance(Autoplay.Bot) > 550)
                    {
                        Autoplay.TempCarry.WalkAround();
                    }
                }
            }
        }

        public static void Follow()
        {
            Console.WriteLine("All good, following: " + Autoplay.Carry.ChampionName);
            Autoplay._frontline.X = Autoplay.Carry.Position.X + Autoplay._chosen;
            Autoplay._frontline.Y = Autoplay.Carry.Position.Y + Autoplay._chosen;
            if (Autoplay.Carry.Distance(Autoplay.Bot) > 550)
            {
                Autoplay.Carry.WalkAround();
            }
        }

        public static void NoCarryFound()
        {
            if (Autoplay.TempCarry == null || Autoplay.TempCarry.IsDead || Autoplay.TempCarry.InFountain())
            {
                if (
                    MetaHandler.AllyHeroes.FirstOrDefault(
                        hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead && !MetaHandler.HasSmite(hero)) !=
                    null)
                {
                    Autoplay.TempCarry =
                        MetaHandler.AllyHeroes.FirstOrDefault(
                            hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead && !MetaHandler.HasSmite(hero));
                }
                if (
                    MetaHandler.AllyHeroes.FirstOrDefault(
                        hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead && !MetaHandler.HasSmite(hero)) ==
                    null &&
                    MetaHandler.AllyHeroes.FirstOrDefault(
                        hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead) != null)
                {
                    //well fuck, let's follow the jungler -sighs-
                    Autoplay.TempCarry =
                        MetaHandler.AllyHeroes.FirstOrDefault(
                            hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead);
                }
            }
            if (Autoplay.TempCarry != null)
            {
                Console.WriteLine("Autoplay.Carry not found, following: " + Autoplay.TempCarry.ChampionName);
                Autoplay._frontline.X = Autoplay.TempCarry.Position.X + Autoplay._chosen;
                Autoplay._frontline.Y = Autoplay.TempCarry.Position.Y + Autoplay._chosen;
                if (
                    !(Autoplay.TempCarry.UnderTurret(true) &&
                      MetaHandler.NearbyAllyMinions(Autoplay.TempCarry, 400) < 2) &&
                    Autoplay.IsBotSafe())
                {
                    if (Autoplay.Bot.Distance(Autoplay._frontline) > 550)
                    {
                        Autoplay.TempCarry.WalkAround();
                    }
                }
            }
        }

        public static void LowHealth()
        {
            Autoplay.NearestAllyTurret = MetaHandler.AllyTurrets.FirstOrDefault();
            if (Autoplay.NearestAllyTurret != null)
            {
                Autoplay._saferecall.X = Autoplay.NearestAllyTurret.Position.X + Autoplay._safe;
                Autoplay._saferecall.Y = Autoplay.NearestAllyTurret.Position.Y;
                if (Autoplay.Bot.Position.Distance(Autoplay._saferecall.To3D()) < 200)
                {
                    if (Environment.TickCount - Autoplay._lastRecallAttempt > Autoplay.Rand.Next(500, 2000))
                    {
                        Autoplay.Bot.Spellbook.CastSpell(SpellSlot.Recall);
                        Autoplay._lastRecallAttempt = Environment.TickCount;
                    }
                }
                else
                {
                    Autoplay.Bot.IssueOrder(GameObjectOrder.MoveTo, Autoplay._saferecall.To3D());
                }
            }
        }

        public static void SwitchCarry()
        {
            var alliesSortedByKDA =
                MetaHandler.AllyHeroes.OrderByDescending(
                    hero => (hero.ChampionsKilled/((hero.Deaths != 0) ? hero.Deaths : 1)));
            if (alliesSortedByKDA.FirstOrDefault() != null)
            {
                Autoplay.Carry = alliesSortedByKDA.FirstOrDefault();
                Autoplay._lastSwitched = Environment.TickCount;
            }
        }

        

        public static void UnderTurret()
        {
            var turret = MetaHandler.EnemyTurrets.FirstOrDefault(t => t.Distance(Autoplay.Bot.Position) < 1200);
            if (Autoplay._overrideAttackUnitAction && !Autoplay._tookRecallDecision)
            {
                Autoplay.Bot.IssueOrder(GameObjectOrder.MoveTo, Autoplay._safepos.To3D());
            }
            if (!Autoplay.Bot.UnderTurret(true))
            {
                Autoplay._overrideAttackUnitAction = false;
            }
            if (Autoplay.Bot.UnderTurret(true) && MetaHandler.NearbyAllyMinions(turret, 750) > 2 && Autoplay.IsBotSafe() &&
                !Autoplay._tookRecallDecision)
            {
                if (turret.Distance(Autoplay.Bot.Position) < Autoplay.Bot.AttackRange && !Autoplay._overrideAttackUnitAction)
                    Autoplay.Bot.IssueOrder(GameObjectOrder.AttackUnit, turret);
            }
            else
            {
                if (TargetSelector.GetTarget(Autoplay.Bot.AttackRange, TargetSelector.DamageType.Physical) != null)
                {
                    var target = TargetSelector.GetTarget(Autoplay.Bot.AttackRange, TargetSelector.DamageType.Physical);
                    if (target != null && target.IsValid && !target.IsDead && Autoplay.IsBotSafe() &&
                        !target.UnderTurret(true) && !Autoplay._overrideAttackUnitAction && !Autoplay._tookRecallDecision)
                    {
                        Autoplay.Bot.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }
            }
            if (Autoplay.Bot.UnderTurret(true) && MetaHandler.NearbyAllyMinions(turret, 750) < 2)
            {
                Autoplay._safepos.X = (Autoplay.Bot.Position.X + Autoplay._safe);
                Autoplay._safepos.Y = (Autoplay.Bot.Position.Y + Autoplay._safe);
                PluginBase.Orbwalker.SetOrbwalkingPoint(Autoplay._safepos.To3D());
            }
        }
    }
}
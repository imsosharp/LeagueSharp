using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (Autoplay._tempcarry == null || Autoplay._tempcarry.IsDead || Autoplay._tempcarry.InFountain())
            {
                if (
                    MetaHandler.AllyHeroes.FirstOrDefault(
                        hero =>
                            !hero.IsMe && !hero.InFountain() && !hero.IsDead &&
                            !MetaHandler.HasSmite(hero)) != null)
                {
                    Autoplay._tempcarry =
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
                    Autoplay._tempcarry =
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
            if (Autoplay._tempcarry != null)
            {

                Console.WriteLine("Autoplay.Carry dead or afk, following: " + Autoplay._tempcarry.ChampionName);
                Autoplay._frontline.X = Autoplay._tempcarry.Position.X + Autoplay._chosen;
                Autoplay._frontline.Y = Autoplay._tempcarry.Position.Y + Autoplay._chosen;
                if (
                    !(Autoplay._tempcarry.UnderTurret(true) &&
                      MetaHandler.NearbyAllyMinions(Autoplay._tempcarry, 400) < 2) &&
                    Autoplay.IsBotSafe())
                {
                    if (Autoplay._tempcarry.Distance(Autoplay.Bot) > 550)
                    {
                        Autoplay._tempcarry.WalkAround();
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
            if (Autoplay._tempcarry == null || Autoplay._tempcarry.IsDead || Autoplay._tempcarry.InFountain())
            {
                if (
                    MetaHandler.AllyHeroes.FirstOrDefault(
                        hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead && !MetaHandler.HasSmite(hero)) !=
                    null)
                {
                    Autoplay._tempcarry =
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
                    Autoplay._tempcarry =
                        MetaHandler.AllyHeroes.FirstOrDefault(
                            hero => !hero.IsMe && !hero.InFountain() && !hero.IsDead);
                }
            }
            if (Autoplay._tempcarry != null)
            {
                Console.WriteLine("Autoplay.Carry not found, following: " + Autoplay._tempcarry.ChampionName);
                Autoplay._frontline.X = Autoplay._tempcarry.Position.X + Autoplay._chosen;
                Autoplay._frontline.Y = Autoplay._tempcarry.Position.Y + Autoplay._chosen;
                if (
                    !(Autoplay._tempcarry.UnderTurret(true) &&
                      MetaHandler.NearbyAllyMinions(Autoplay._tempcarry, 400) < 2) &&
                    Autoplay.IsBotSafe())
                {
                    if (Autoplay.Bot.Distance(Autoplay._frontline) > 550)
                    {
                        Autoplay._tempcarry.WalkAround();
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
                        hero => (hero.ChampionsKilled/((hero.Deaths != 0) ? hero.Deaths : 1))); //AsunaChan2Kawaii
                if (alliesSortedByKDA.FirstOrDefault() != null)
                {
                    Autoplay.Carry = alliesSortedByKDA.FirstOrDefault();
                    Autoplay._lastSwitched = Environment.TickCount;
                }
        }
    }
}

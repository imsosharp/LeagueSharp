/* Autoplay Plugin of h3h3's AIO Support
*
* All credits go to him. I only wrote this and
* Autoplay.cs.
* The core is always updated to latest version.
* which you can find here:
* https://github.com/h3h3/LeagueSharp/tree/master/Support
*/

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Support
{
    internal class MetaHandler
    {

        public static List<Obj_AI_Turret> AllTurrets;
        public static List<Obj_AI_Turret> AllyTurrets;
        public static List<Obj_AI_Turret> EnemyTurrets;
        public static List<Obj_AI_Hero> AllHeroes;
        public static List<Obj_AI_Hero> AllyHeroes;
        public static List<Obj_AI_Hero> EnemyHeroes;
        public static string[] Supports = { "Alistar", "Annie", "Blitzcrank", "Braum", "Fiddlesticks", "Janna", "Karma", "Kayle", "Leona", "Lulu", "Morgana", "Nunu", "Nami", "Soraka", "Sona", "Taric", "Thresh", "Zilean", "Zyra" }; 
        static readonly ItemId[] SRShopList = { ItemId.Zhonyas_Hourglass, ItemId.Rabadons_Deathcap, ItemId.Mejais_Soulstealer, ItemId.Sorcerers_Shoes, ItemId.Athenes_Unholy_Grail, ItemId.Mikaels_Crucible, ItemId.Frost_Queens_Claim, ItemId.Ruby_Sightstone, ItemId.Locket_of_the_Iron_Solari, ItemId.Morellonomicon, ItemId.Rod_of_Ages };
        static readonly ItemId[] TTShopList = { ItemId.Rod_of_Ages, ItemId.Sorcerers_Shoes, ItemId.Wooglets_Witchcap };
        static readonly ItemId[] ARAMShopListAP = { ItemId.Zhonyas_Hourglass, ItemId.Rod_of_Ages, ItemId.Sorcerers_Shoes, ItemId.Rylais_Crystal_Scepter, ItemId.Will_of_the_Ancients, ItemId.Zekes_Herald, ItemId.Locket_of_the_Iron_Solari, ItemId.Hextech_Sweeper };
        static readonly ItemId[] ARAMShopListAD = { ItemId.Blade_of_the_Ruined_King, ItemId.Berserkers_Greaves, ItemId.Infinity_Edge, ItemId.Phantom_Dancer, ItemId.Statikk_Shiv };
        static readonly ItemId[] OtherMapsShopList = { ItemId.Rod_of_Ages_Crystal_Scar };
        static ItemId[] CustomBuild = { };

        public static void DoChecks()
        {
            var map = Utility.Map.GetMap();

            if (map != null && (map.Type == Utility.Map.MapType.SummonersRift || map.Type == Utility.Map.MapType.TwistedTreeline))
            {
                if (Autoplay.Bot.InFountain() && Autoplay.NearestAllyTurret != null)
                {
                    Autoplay.NearestAllyTurret = null;
                }
            }
            if (Autoplay.Bot.InFountain())
            {
                if (FileHandler.ExistsCustomBuild())
                {
                    if (map.Type == Utility.Map.MapType.HowlingAbyss && !Autoplay.Bot.IsDead)
                    {
                        return;
                    }
                    CustomBuild = FileHandler.GetCustomBuild();
                    foreach (var item in CustomBuild)
                    {
                        if (!HasItem(item))
                        {
                            BuyItem(item);
                        }
                    }
                }
                else
                {
                    if (Autoplay.Bot.InFountain() && (Autoplay.Bot.Gold == 475 || Autoplay.Bot.Gold == 515)) //validates on SR untill 1:55 game time
                    {
                        Autoplay.Bot.BuyItem(ItemId.Spellthiefs_Edge);
                        Autoplay.Bot.BuyItem(ItemId.Warding_Totem_Trinket);
                    }
                    foreach (var item in GetDefaultItemArray())
                    {
                        if (!HasItem(item))
                        {
                            BuyItem(item);
                        }
                    }
                }
            }
        }
        public static bool HasItem(ItemId item)
        {
            return Items.HasItem((int)item, Autoplay.Bot);
        }

        public static void BuyItem(ItemId item)
        {
            Autoplay.Bot.BuyItem(item);
        }

        public static ItemId[] GetDefaultItemArray()
        {

            var map = Utility.Map.GetMap();
            if (map.Type == Utility.Map.MapType.SummonersRift)
            {
                return SRShopList;
            }
            if (map.Type == Utility.Map.MapType.TwistedTreeline)
            {
                return TTShopList;
            }
            if (map.Type == Utility.Map.MapType.HowlingAbyss)
            {
                return ARAMShopListAP;
            }
            return OtherMapsShopList;
        }

        public static bool HasSmite(Obj_AI_Hero hero)
        {
            return hero.GetSpellSlot("SummonerSmite", true) != SpellSlot.Unknown && 
                (hero.Spellbook.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("smite") ||
                hero.Spellbook.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("smite"));
        }

        public static void UpdateObjects()
        {

            //Heroes
            AllHeroes = ObjectManager.Get<Obj_AI_Hero>().ToList();
            AllHeroes = AllHeroes.OrderBy(hero => hero.Distance(Autoplay.Bot)).ToList();
            AllyHeroes = AllHeroes.FindAll(hero => hero.IsAlly && !IsSupport(hero)).ToList();
            AllyHeroes = AllyHeroes.OrderBy(hero => hero.Distance(Autoplay.Bot)).ToList();
            EnemyHeroes = AllHeroes.FindAll(hero => !hero.IsAlly).ToList();
            EnemyHeroes = EnemyHeroes.OrderBy(hero => hero.Distance(Autoplay.Bot)).ToList();

            //Turrets
            AllTurrets = ObjectManager.Get<Obj_AI_Turret>().ToList();
            AllTurrets = AllTurrets.OrderBy(turret => turret.Distance(Autoplay.Bot)).ToList();
            AllyTurrets = AllTurrets.FindAll(turret => turret.IsAlly).ToList();
            AllyTurrets = AllyTurrets.OrderBy(turret => turret.Distance(Autoplay.Bot)).ToList();
            EnemyTurrets = AllTurrets.FindAll(turret => !turret.IsAlly).ToList();
            EnemyTurrets = EnemyTurrets.OrderBy(turret => turret.Distance(Autoplay.Bot)).ToList();

        }

        public static bool IsInBase(Obj_AI_Hero hero)
        {
            var map = Utility.Map.GetMap();
            if (map != null && map.Type == Utility.Map.MapType.SummonersRift)
            {
                var baseRange = 16000000; //4000^2
                return hero.IsVisible &&
                       ObjectManager.Get<Obj_SpawnPoint>()
                           .Any(sp => sp.Team == hero.Team && hero.Distance(sp.Position, true) < baseRange);
            }
            return false;
        }

        public static bool IsSupport(Obj_AI_Hero hero)
        {
            foreach (var support in Supports)
            {
                if (hero.BaseSkinName == support)
                {
                    return true;
                }
            }
            return false;
        }
    }
       
    }
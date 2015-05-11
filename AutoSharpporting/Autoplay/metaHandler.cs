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
using SharpDX;

namespace AutoSharpporting.Autoplay
{
    internal class MetaHandler
    {
        public static List<Obj_AI_Turret> AllTurrets;
        public static List<Obj_AI_Turret> AllyTurrets;
        public static List<Obj_AI_Turret> EnemyTurrets;
        public static List<Obj_AI_Hero> AllHeroes;
        public static List<Obj_AI_Hero> AllyHeroes;
        public static List<Obj_AI_Hero> EnemyHeroes;

        public static string[] Supports =
        {
            "Alistar", "Annie", "Blitzcrank", "Braum", "Fiddlesticks", "Janna", "Karma",
            "Kayle", "Leona", "Lulu", "Morgana", "Nunu", "Nami", "Soraka", "Sona", "Taric", "Thresh", "Zilean", "Zyra"
        };

        public static string[] AP =
        {
            "Ahri", "Alistar", "Anivia", "Annie", "Azir",
            "Brand", "Braum", "Cassiopeia", "Chogath", "Diana", "Elise", "Evelynn", "ezreal", "FiddleSticks", "Fizz",
            "Galio", "Gragas", "Heimerdinger", "Janna", "Karma", "Karthus", "Kassadin",
            "Kayle", "KogMaw", "LeBlanc", "Lissandra", "Lulu", "Lux", "Malphite", "Malzahar",
            "Maokai", "Morderkaiser", "Morgana", "Nami", "Nautilus", "Nidalee", "Nunu", "Orianna", "RekSai", "Rumble",
            "Ryze", "Shaco", "Singed", "Sona", "Soraka", "Swain", "Syndra", "Teemo", "Thresh", "TwistedFate", "veigar",
            "VelKoz", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zilean", "Zyra"
        };

        public static string[] AD =
        {
            "ashe", "caitlyn", "corki", "draven", "ezreal", "graves", "kogmaw",
            "missfortune", "quinn", "sivir", "talon", "tristana", "twitch", "urgot", "varus", "vayne", "zed", "jinx",
            "yasuo", "lucian", "shaco", "masteryi"
        };

        private static readonly string[] ADT =
        {
            "darius", "evelynn", "fiora", "gangplank", "gnar", "jayce", "yorick", "warwick",
            "pantheon", "irelia", "jarvaniv", "jax", "khazix", "nocturne", "olaf",
            "rengar", "riven", "shyvana", "trundle", "tryndamere", "udyr", "vi", "monkeyking", "xinzhao", "aatrox"
        };

        private static readonly string[] APT =
        {
            "galio", "malphite", "poppy", "elise",
            "maokai", "rammus", "sejuani", "shen", "singed", "zac",
            "nunu", "alistar", "nautilus", "rumble"
        };

        private static readonly string[] FT =
        {
            "drmundo", "skarner", "braum", "nasus", "renekton", "chogath", "taric", "leesin", "garen", "volibear",
            "hecarim",
            "Blitzcrank", "amumu"
        };

        private static readonly string[] EN =
        {
            "Akali", "Kennen", "Katarina"
        };

        private static readonly ItemId[] TTShopList =
        {
            ItemId.Wooglets_Witchcap, ItemId.Rod_of_Ages,
            ItemId.Rylais_Crystal_Scepter, ItemId.Lich_Bane, ItemId.Liandrys_Torment, ItemId.Morellonomicon,
            ItemId.Locket_of_the_Iron_Solari, ItemId.Void_Staff, ItemId.Sorcerers_Shoes
        };

        private static readonly ItemId[] ARAMShopListAP =
        {
            ItemId.Zhonyas_Hourglass, ItemId.Rabadons_Deathcap,
            ItemId.Rod_of_Ages, ItemId.Rylais_Crystal_Scepter, ItemId.Athenes_Unholy_Grail,
            ItemId.Will_of_the_Ancients, ItemId.Liandrys_Torment, ItemId.Void_Staff, ItemId.Abyssal_Scepter,
            ItemId.Sorcerers_Shoes
        };

        private static readonly ItemId[] ARAMShopListEN =
        {
            ItemId.Zhonyas_Hourglass, ItemId.Rabadons_Deathcap,
            ItemId.Rylais_Crystal_Scepter,
            ItemId.Will_of_the_Ancients, ItemId.Liandrys_Torment, ItemId.Void_Staff, ItemId.Abyssal_Scepter,
            ItemId.Sorcerers_Shoes
        };

        private static readonly ItemId[] ARAMShopListFT =
        {
            ItemId.Sunfire_Cape, ItemId.Randuins_Omen,
            ItemId.Spirit_Visage, ItemId.Banshees_Veil, ItemId.Frozen_Heart,
            ItemId.Locket_of_the_Iron_Solari, ItemId.Orb_of_Winter, ItemId.Thornmail, ItemId.Mercurys_Treads
        };

        private static readonly ItemId[] ARAMShopListAD =
        {
            ItemId.Blade_of_the_Ruined_King, ItemId.Last_Whisper,
            ItemId.Phantom_Dancer, ItemId.Infinity_Edge, ItemId.Statikk_Shiv, ItemId.Trinity_Force,
            ItemId.Youmuus_Ghostblade, ItemId.Berserkers_Greaves
        };

        private static readonly ItemId[] ARAMShopListADT =
        {
            ItemId.Maw_of_Malmortius, ItemId.Banshees_Veil, ItemId.Sunfire_Cape,
            ItemId.Locket_of_the_Iron_Solari, ItemId.Blade_of_the_Ruined_King, ItemId.Randuins_Omen,
            ItemId.Spirit_Visage, ItemId.Mercurys_Treads
        };

        private static readonly ItemId[] ARAMShopListAPT =
        {
            ItemId.Abyssal_Scepter, ItemId.Banshees_Veil, ItemId.Sunfire_Cape,
            ItemId.Locket_of_the_Iron_Solari, ItemId.Liandrys_Torment, ItemId.Randuins_Omen, ItemId.Spirit_Visage,
            ItemId.Mercurys_Treads
        };

        private static readonly ItemId[] CrystalScar =
        {
            ItemId.Rod_of_Ages_Crystal_Scar, ItemId.Wooglets_Witchcap,
            ItemId.Void_Staff, ItemId.Athenes_Unholy_Grail, ItemId.Abyssal_Scepter, ItemId.Liandrys_Torment,
            ItemId.Morellonomicon, ItemId.Rylais_Crystal_Scepter, ItemId.Sorcerers_Shoes
        };

        private static readonly ItemId[] Other = {};
        private static int LastShopAttempt;

        public static bool ShouldSupportTopLane
        {
            get
            {
                return NearbyAllies(Autoplay.BotLanePos.To3D(), 4500) > 1 &&
                       NearbyAllies(Autoplay.TopLanePos.To3D(), 4500) == 1;
            }
        }

        public static void DoChecks()
        {
            var map = Utility.Map.GetMap();

            if (map != null &&
                (map.Type == Utility.Map.MapType.SummonersRift || map.Type == Utility.Map.MapType.TwistedTreeline))
            {
                if (Autoplay.Bot.InFountain() && Autoplay.NearestAllyTurret != null)
                {
                    Autoplay.NearestAllyTurret = null;
                }
            }
            if (Autoplay.Bot.InFountain())
            {
                if (Autoplay.Bot.InFountain() && (Autoplay.Bot.Gold == 475 || Autoplay.Bot.Gold == 515))
                    //validates on SR untill 1:55 game time
                {
                    if (Supports.Any(c => c.ToLower() == Autoplay.Bot.BaseSkinName.ToLower()))
                    {
                        var startingItem = Autoplay.Rand.Next(-6, 7);
                        if (startingItem < 0)
                        {
                            Autoplay.Bot.BuyItem(ItemId.Spellthiefs_Edge);
                        }
                        if (startingItem == 0)
                        {
                            Autoplay.Bot.BuyItem(ItemId.Dorans_Ring);
                        }
                        if (startingItem > 0)
                        {
                            Autoplay.Bot.BuyItem(ItemId.Ancient_Coin);
                        }
                        Autoplay.Bot.BuyItem(ItemId.Warding_Totem_Trinket);
                    }
                    if (AD.Any(c => c.ToLower() == Autoplay.Bot.BaseSkinName.ToLower()))
                    {
                        Autoplay.Bot.BuyItem(ItemId.Dorans_Blade);
                        Autoplay.Bot.BuyItem(ItemId.Health_Potion);
                    }
                    if (AP.Any(c => c.ToLower() == Autoplay.Bot.BaseSkinName.ToLower()))
                    {
                        Autoplay.Bot.BuyItem(ItemId.Dorans_Ring);
                        Autoplay.Bot.BuyItem(ItemId.Health_Potion);
                        Autoplay.Bot.BuyItem(ItemId.Health_Potion);
                    }
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

        public static bool HasItem(ItemId item)
        {
            if (item == ItemId.Mana_Potion || item == ItemId.Health_Potion)
            {
                return Autoplay.RandomDecision();
            }
            return Items.HasItem((int) item, Autoplay.Bot);
        }

        public static void BuyItem(ItemId item)
        {
            if (Environment.TickCount - LastShopAttempt > Autoplay.Rand.Next(0, 670))
            {
                Autoplay.Bot.BuyItem(item);
                LastShopAttempt = Environment.TickCount;
            }
        }

        public static ItemId[] GetDefaultItemArray()
        {
            var map = Utility.Map.GetMap();
            if (map.Type == Utility.Map.MapType.HowlingAbyss || map.Type == Utility.Map.MapType.SummonersRift)
            {
                if (APT.Any(apchamp => Autoplay.Bot.BaseSkinName.ToLower() == apchamp.ToLower()))
                {
                    Console.WriteLine("APT");
                    return ARAMShopListAPT;
                }
                if (ADT.Any(apchamp => Autoplay.Bot.BaseSkinName.ToLower() == apchamp.ToLower()))
                {
                    Console.WriteLine("ADT");
                    return ARAMShopListADT;
                }
                if (AP.Any(apchamp => Autoplay.Bot.BaseSkinName.ToLower() == apchamp.ToLower()))
                {
                    Console.WriteLine("APD");
                    return ARAMShopListAP;
                }
                if (FT.Any(FTchamp => Autoplay.Bot.BaseSkinName.ToLower() == FTchamp.ToLower()))
                {
                    Console.WriteLine("FT");
                    return ARAMShopListFT;
                }
                if (EN.Any(ENchamp => Autoplay.Bot.BaseSkinName.ToLower() == ENchamp.ToLower()))
                {
                    Console.WriteLine("EN");
                    return ARAMShopListEN;
                }


                if (AD.Any(adchamp => Autoplay.Bot.BaseSkinName.ToLower() == adchamp.ToLower()))
                {
                    Console.WriteLine("ADC");
                    return ARAMShopListAD;
                }
                return ARAMShopListADT;
            }
            if (map.Type == Utility.Map.MapType.TwistedTreeline)
            {
                return TTShopList.OrderBy(item => Autoplay.Rand.Next()).ToArray();
            }
            if (map.Type == Utility.Map.MapType.CrystalScar)
            {
                return CrystalScar.OrderBy(item => Autoplay.Rand.Next()).ToArray();
            }
            return Other;
        }

        public static bool HasSixItems()
        {
            return Autoplay.Bot.InventoryItems.ToList().Count >= 6;
        }

        public static bool HasSmite(Obj_AI_Hero hero)
        {
            //return hero.GetSpellSlot("SummonerSmite", true) != SpellSlot.Unknown; //obsolete, use the one below.
            return hero.GetSpellSlot("SummonerSmite") != SpellSlot.Unknown;
        }

        public static void LoadObjects()
        {
            //Heroes
            AllHeroes = ObjectManager.Get<Obj_AI_Hero>().ToList();
            AllyHeroes = (Utility.Map.GetMap().Type != Utility.Map.MapType.HowlingAbyss)
                ? AllHeroes.FindAll(hero => hero.IsAlly && !IsSupport(hero) && !hero.IsMe && !HasSmite(hero)).ToList()
                : AllHeroes.FindAll(hero => hero.IsAlly && !hero.IsMe).ToList();
            EnemyHeroes = AllHeroes.FindAll(hero => !hero.IsAlly).ToList();
        }

        public static void UpdateObjects()
        {
            //Heroes
            AllHeroes = AllHeroes.OrderBy(hero => hero.Distance(Autoplay.Bot)).ToList();
            AllyHeroes = AllyHeroes.OrderBy(hero => hero.Distance(Autoplay.Bot)).ToList();
            EnemyHeroes = EnemyHeroes.OrderBy(hero => hero.Distance(Autoplay.Bot)).ToList();


            //Turrets
            AllTurrets = ObjectManager.Get<Obj_AI_Turret>().ToList();
            AllyTurrets = AllTurrets.FindAll(turret => turret.IsAlly).ToList();
            EnemyTurrets = AllTurrets.FindAll(turret => !turret.IsAlly).ToList();
            AllTurrets = AllTurrets.OrderBy(turret => turret.Distance(Autoplay.Bot)).ToList();
            AllyTurrets = AllyTurrets.OrderBy(turret => turret.Distance(Autoplay.Bot)).ToList();
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
            return Supports.Any(support => hero.BaseSkinName.ToLower() == support.ToLower());
        }

        public static int NearbyAllyMinions(Obj_AI_Base x, int distance)
        {
            return ObjectManager.Get<Obj_AI_Minion>()
                .Count(minion => minion.IsAlly && !minion.IsDead && minion.Distance(x) < distance);
        }

        public static int NearbyAllies(Obj_AI_Base x, int distance)
        {
            return ObjectManager.Get<Obj_AI_Hero>()
                .Count(
                    hero => hero.IsAlly && !hero.IsDead && !HasSmite(hero) && !hero.IsMe && hero.Distance(x) < distance);
        }

        public static int NearbyAllies(Vector3 x, int distance)
        {
            return ObjectManager.Get<Obj_AI_Hero>()
                .Count(
                    hero => hero.IsAlly && !hero.IsDead && !HasSmite(hero) && !hero.IsMe && hero.Distance(x) < distance);
        }
    }
}
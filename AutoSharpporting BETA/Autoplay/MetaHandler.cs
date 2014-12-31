/* Autoplay Plugin of h3h3's AIO Support
*
* All credits go to him. I only wrote this and
* Autoplay.cs.
* The core is always updated to latest version.
* which you can find here:
* https://github.com/h3h3/LeagueSharp/tree/master/Support
*/

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Support
{
    internal class MetaHandler
    {
        static readonly ItemId[] SRShopList = { ItemId.Zhonyas_Hourglass, ItemId.Rabadons_Deathcap, ItemId.Mejais_Soulstealer, ItemId.Sorcerers_Shoes, ItemId.Athenes_Unholy_Grail, ItemId.Mikaels_Crucible, ItemId.Frost_Queens_Claim, ItemId.Ruby_Sightstone, ItemId.Locket_of_the_Iron_Solari, ItemId.Morellonomicon, ItemId.Rod_of_Ages };
        static readonly ItemId[] TTShopList = { ItemId.Rod_of_Ages, ItemId.Blasting_Wand, ItemId.Catalyst_the_Protector, ItemId.Sorcerers_Shoes };
        public static void DoChecks()
        {
            var map = Utility.Map.GetMap();
            if (map != null && map.Type == Utility.Map.MapType.SummonersRift)
            {
                if (Autoplay.Bot.InFountain() && Autoplay.NearestAllyTurret != null)
                {
                    Autoplay.NearestAllyTurret = null;
                    Autoplay.AllTurrets = null;
                }
                if (Autoplay.Bot.InFountain() && (Autoplay.Bot.Gold == 475 || Autoplay.Bot.Gold == 515))
                {
                    Autoplay.Bot.BuyItem(ItemId.Spellthiefs_Edge);
                    Autoplay.Bot.BuyItem(ItemId.Warding_Totem_Trinket);
                }

                if (Autoplay.Bot.InFountain() && Autoplay.Bot.Gold >= 1000)
                {
                    foreach (ItemId item in SRShopList)
                    {
                        if (!HasItem(item))
                        {
                            Autoplay.Bot.BuyItem(item);
                            Console.WriteLine("Trying to buy Item: " + (int) item);
                        }
                    }
                }
            }
            else
            {
                if (Autoplay.Bot.InFountain() && (Autoplay.Bot.Gold == 815 || Autoplay.Bot.Gold == 855))
                {
                    Autoplay.Bot.BuyItem(ItemId.Boots_of_Speed);
                }
                if (Autoplay.Bot.InFountain() && Autoplay.Bot.Gold >= 1000)
                {
                    foreach (ItemId item in TTShopList)
                    {
                        if (item == ItemId.Sorcerers_Shoes)
                        {
                            if (!HasItem(item))
                            {
                                Autoplay.Bot.BuyItem(item);
                                Console.WriteLine("Trying to buy Item: " + (int) item);
                            }
                        }
                        else
                        {
                            Autoplay.Bot.BuyItem(item);
                            Console.WriteLine("Trying to buy Item: " + (int)item);
                        }
                    }
                }
            }

        }
        public static bool HasItem(ItemId item)
        {
            return Items.HasItem((int)item, Autoplay.Bot);
        }

        public static bool HasSmite(Obj_AI_Hero hero)
        {
            return hero.GetSpellSlot("SummonerSmite", true) != SpellSlot.Unknown;
        }
       
    }
}

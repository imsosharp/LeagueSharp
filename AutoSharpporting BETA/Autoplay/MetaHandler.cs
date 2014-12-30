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
        static readonly ItemId[] ShopList = { ItemId.Mejais_Soulstealer, ItemId.Sorcerers_Shoes, ItemId.Chalice_of_Harmony, ItemId.Athenes_Unholy_Grail, ItemId.Mikaels_Crucible, ItemId.Frost_Queens_Claim, ItemId.Ruby_Sightstone, ItemId.Locket_of_the_Iron_Solari, ItemId.Morellonomicon, ItemId.Rabadons_Deathcap, ItemId.Rod_of_Ages };
        public static void DoChecks()
        {            
            if (Autoplay.Bot.InFountain() && (Autoplay.Bot.Gold == 475 || Autoplay.Bot.Gold == 515))
            {
                Autoplay.Bot.BuyItem(ItemId.Spellthiefs_Edge);
                Autoplay.Bot.BuyItem(ItemId.Warding_Totem_Trinket);
            }

            if (Autoplay.Bot.InFountain() && Autoplay.Bot.Gold >= 1000)
            {
               foreach (ItemId item in ShopList)
                {
                    if (!HasItem(item))
                    {
                        Autoplay.Bot.BuyItem(item);
                        Console.WriteLine("Trying to buy Item: " + (int)item);
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

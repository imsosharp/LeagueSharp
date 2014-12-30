/* Autoplay Plugin of h3h3's AIO Support
*
* All credits go to him. I only wrote this and
* Autoplay.cs.
* The core is always updated to latest version.
* which you can find here:
* https://github.com/h3h3/LeagueSharp/tree/master/Support
*/

using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Support
{
    internal class MetaHandler
    {
        static int _qlvl, _wlvl, _elvl, _rlvl = 0;
        static readonly int[] AbilityOrder = { 1, 2, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3, }; //spell level order
        static readonly ItemId[] ShopList = { ItemId.Mejais_Soulstealer, ItemId.Sorcerers_Shoes, ItemId.Chalice_of_Harmony, ItemId.Athenes_Unholy_Grail, ItemId.Mikaels_Crucible, ItemId.Frost_Queens_Claim, ItemId.Ruby_Sightstone, ItemId.Locket_of_the_Iron_Solari, ItemId.Morellonomicon, ItemId.Rabadons_Deathcap, ItemId.Rod_of_Ages };
        public static void DoChecks()
        {            
            if (Autoplay.Bot.InFountain() && (Autoplay.Bot.Gold == 475 || Autoplay.Bot.Gold == 515))
            {
                Autoplay.Bot.BuyItem(ItemId.Spellthiefs_Edge);
                Autoplay.Bot.BuyItem(ItemId.Warding_Totem_Trinket);
            }
            if ((_qlvl + _wlvl + _elvl + _rlvl) < Autoplay.Bot.Level)
            {
                int i = Autoplay.Bot.Level - 1;
                SpellSlot abilitySlot;
                if (AbilityOrder[i] == 1)
                {
                    abilitySlot = SpellSlot.Q;
                    _qlvl++;
                }
                else if (AbilityOrder[i] == 2)
                {
                    abilitySlot = SpellSlot.W;
                    _wlvl++;
                }
                else if (AbilityOrder[i] == 3)
                {
                    abilitySlot = SpellSlot.E;
                    _elvl++;
                }
                else if (AbilityOrder[i] == 4)
                {
                    abilitySlot = SpellSlot.R;
                    _rlvl++;
                }
                else
                {
                    abilitySlot = SpellSlot.Q;
                }
                ObjectManager.Player.Spellbook.LevelSpell(abilitySlot);
            }
            if (Autoplay.Bot.InFountain() && Autoplay.Bot.Gold >= 1000)
            {
               foreach (ItemId item in ShopList)
                {
                    if (!HasItem(item))
                    {
                        Autoplay.Bot.BuyItem(item);
                        Game.PrintChat("Trying to buy Item: " + (int)item);
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

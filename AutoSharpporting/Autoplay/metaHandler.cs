//Autoplay Plugin of h3h3's AIO Support
//
//All credits go to him. I only wrote whatever is in this file.
//The core is based on version 2.0.7.2
//which you can find here:
//https://github.com/h3h3/LeagueSharp/tree/master/Support

using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Support
{
    internal class MetaHandler
    {
        static int _qlvl, _wlvl, _elvl, _rlvl, _buyIndex = 0;
        static readonly int[] AbilityOrder = { 2, 1, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3, }; //spell level order
        static readonly ItemId[] ShopList = new ItemId[] { ItemId.Sorcerers_Shoes, ItemId.Mejais_Soulstealer, ItemId.Mikaels_Crucible, ItemId.Locket_of_the_Iron_Solari, ItemId.Rabadons_Deathcap, ItemId.Morellonomicon };
        public static void DoChecks()
        {            
            if ((Utility.InFountain() && ObjectManager.Player.Gold == 475) || (Utility.InFountain() && ObjectManager.Player.Gold == 515) ) //now it should work with the wealth mastery
            {
                Autoplay.Bot.BuyItem(ItemId.Boots_of_Speed);
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
            if (Utility.InFountain())
            {
               foreach (ItemId item in ShopList)
                {
                    if (_buyIndex > ShopList.Count())
                    {
                        _buyIndex = 0;
                    }
                    ItemId thisItem = ShopList[_buyIndex];
                    if (!HasItem(item))
                    {
                        Autoplay.Bot.BuyItem(item);
                    }
                    else
                    {
                        _buyIndex += 1;
                    }
                }
            }
        }
        public static bool HasItem(ItemId item)
        {
            return Items.HasItem((int)item, Autoplay.Bot);
        }
    }
}

//Autoplay Plugin of h3h3's AIO Support
//
//All credits go to him. I only wrote whatever is in this file.
//The core is based on version 2.0.7.2
//which you can find here:
//https://github.com/h3h3/LeagueSharp/tree/master/Support

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Support
{
    internal class metaHandler
    {
        static int qlvl, wlvl, elvl, rlvl, buyIndex = 0;
        static int[] abilityOrder = { 1, 2, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3, }; //spell level order
        static int[] shopList = { 3270, 3222, 3092, 3041, 2045, 3190, 0 }; //list of items to buy
        public static void doChecks()
        {
            if (Utility.InFountain() && ObjectManager.Player.Gold == 475)
            {
                Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(1001)).Send();
                Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(2003)).Send();
                Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(3340)).Send();
                System.Threading.Thread.Sleep(1500);
            }
            if ((qlvl + wlvl + elvl + rlvl) < Autoplay.bot.Level)
            {
                int i = Autoplay.bot.Level - 1;
                SpellSlot abilitySlot;
                if (abilityOrder[i] == 1)
                {
                    abilitySlot = SpellSlot.Q;
                    qlvl++;
                }
                else if (abilityOrder[i] == 2)
                {
                    abilitySlot = SpellSlot.W;
                    wlvl++;
                }
                else if (abilityOrder[i] == 3)
                {
                    abilitySlot = SpellSlot.E;
                    elvl++;
                }
                else if (abilityOrder[i] == 4)
                {
                    abilitySlot = SpellSlot.R;
                    rlvl++;
                }
                else
                {
                    abilitySlot = SpellSlot.Q;
                }
                ObjectManager.Player.Spellbook.LevelUpSpell(abilitySlot);
            }
            if (Utility.InFountain())
            {
                if (shopList[buyIndex] != 0)
                {
                    int thisItem = shopList[buyIndex];
                    if (!hasItem(thisItem))
                    {
                        buyItem(thisItem);
                    }
                    if (hasItem(thisItem))
                    {
                        buyIndex += 1;
                    }
                }
            }
        }

        public static bool hasItem(int id)
        {
            return Items.HasItem(id, Autoplay.bot);
        }
        public static void buyItem(int id)
        {
            Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(id, ObjectManager.Player.NetworkId)).Send();
        }
    }
}

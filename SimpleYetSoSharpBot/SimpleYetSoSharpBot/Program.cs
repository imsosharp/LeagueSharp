/*
This shit is copied from Maufeat and blm95 and it comes with no warranty that
it will work as intended, or that it work at all.
I also suck at disclaimers.
hf spending your free IP ^_^
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace SimpleYetSoSharp
{
    internal class Program
    {
        //edit this if you feel like it
        private static string[] shityousaywhenyoudead = { "lulz", "lol", "omg", "noooooob", "help me", "fkin nooooobs", "TEAM WHERE YOU AT???", "WILL YOU HELP?", "HEEEEEEEEEEEELP", "OMG REPORT MY NOOB TEAM", "OMG MY TEAM SHOULD UNINSTALL", "MATCHMAKING SO UNFAIR", "gg", "I just want this game to end fking noobs" };
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static TargetSelector ts;
        private static Menu menu;
        private static int deathcounter = 0;
        private static double timedead;
        private static List<Obj_AI_Hero> allies;
        private static int i = 0;
        public static bool boughtItemOne, boughtItemTwo, boughtItemThree = false;
        static Obj_AI_Hero player = ObjectManager.Player;
        static int qOff, wOff, eOff, rOff = 0;
        static int[] abilityOrder = { 1, 2, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3, }; //spell level order
        public static bool quietm = true;
        public static string tempfollow = "";

        private static Vector3 followpos;
        private static Obj_AI_Hero follow;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Game.Time == null)
            {
                System.Threading.Thread.Sleep(30000);
            }
            if (Game.Time < 60000)
            {
                System.Threading.Thread.Sleep(30000);
            }
            if (player.Level == 1)
            {
                SpellSlot abilitySlot;
                if (abilityOrder[0] == 1)
                {
                    abilitySlot = SpellSlot.Q;
                }
                else if (abilityOrder[0] == 2)
                {
                    abilitySlot = SpellSlot.W;
                }
                else if (abilityOrder[0] == 3)
                {
                    abilitySlot = SpellSlot.E;
                }
                else if (abilityOrder[0] == 4)
                {
                    abilitySlot = SpellSlot.R;
                }
                else
                {
                    abilitySlot = SpellSlot.Q;
                }
                ObjectManager.Player.Spellbook.LevelUpSpell(abilitySlot);
            }
            CustomEvents.Unit.OnLevelUp += OnLevelUp;

            allies = new List<Obj_AI_Hero>();


            if (ObjectManager.Player.ChampionName == "Soraka
            ")
            {
                Q = new Spell(SpellSlot.Q, 650);
                W = new Spell(SpellSlot.W, 625);
                E = new Spell(SpellSlot.E);
                R = new Spell(SpellSlot.R, 600);
            }
            else
            {
                Q = new Spell(SpellSlot.Q);
                W = new Spell(SpellSlot.W);
                E = new Spell(SpellSlot.E);
                R = new Spell(SpellSlot.R);
            }
            ts = new TargetSelector(1025, TargetSelector.TargetingMode.AutoPriority);
            // things you will say when you die
            menu = new Menu("AutoPlay Bot", "syssb", true);
            menu.AddSubMenu(new Menu("Follow:", "follower"));
            menu.AddItem(new MenuItem("quiet", "Quiet mode").SetValue(quietm));
            
            foreach (var ally in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly && !x.IsMe))
            {
                allies.Add(ally);
                menu.SubMenu("follower").AddItem(new MenuItem(ally.ChampionName, ally.ChampionName).SetValue(true));


            }

            menu.AddToMainMenu();
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Game.OnGameEnd += OnGameEnd;
            BuyItems();
        }



        private static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            GamePacket p = new GamePacket(args.PacketData);
            if (p.Header != Packet.S2C.TowerAggro.Header) return;
            if (Packet.S2C.TowerAggro.Decoded(args.PacketData).TargetNetworkId != ObjectManager.Player.NetworkId)
                return;
        }


        private static void Game_OnGameUpdate(EventArgs args)
        {
            
            if (follow == null)
            {
                follow = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.IsAlly);
            }
            followpos = follow.Position;
            if (deathcounter == 14)
                deathcounter = 0;
            BuyItems();
            doFollow();



            if (ObjectManager.Player.IsDead && Game.Time - timedead > 80 && !quietm)
            {
                Game.Say(shityousaywhenyoudead[deathcounter]);
                deathcounter++;
                timedead = Game.Time;
            }


            if (follow.IsDead)
            {
                follow = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.Distance(ObjectManager.Player) < 1300);
            }

            Console.WriteLine(follow.IsDead);

        }
        public static void OnLevelUp(Obj_AI_Base sender, CustomEvents.Unit.OnLevelUpEventArgs args)
        {
            if ((qOff + wOff + eOff + rOff) < player.Level)
            {
                int i = player.Level - 1;
                SpellSlot abilitySlot;
                if (abilityOrder[i] == 1)
                {
                    abilitySlot = SpellSlot.Q;
                }
                else if (abilityOrder[i] == 2)
                {
                    abilitySlot = SpellSlot.W;
                }
                else if (abilityOrder[i] == 3)
                {
                    abilitySlot = SpellSlot.E;
                }
                else if (abilityOrder[i] == 4)
                {
                    abilitySlot = SpellSlot.R;
                }
                else
                {
                    abilitySlot = SpellSlot.Q;
                }
                ObjectManager.Player.Spellbook.LevelUpSpell(abilitySlot);
            }
        }




        public static void doFollow()
        {
			findSillyGoat();
            
            if (follow == null)
            {
                follow = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.IsAlly);
            }
            if (follow.Distance(ObjectManager.Player.Position) > 550 && (Game.Time > 60))
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, follow.Position);
            }
            //Anti-AFK Test
            if (Utility.InFountain() && (Game.Time > 60000))
            {
                follow = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.Distance(ObjectManager.Player) > 1500);
            }
			if (ObjectManager.Player.ChampionName == "Annie")
            {
                if (ts.Target.Distance(ObjectManager.Player) < Q.Range && Q.IsReady() && !Utility.UnderTurret(ObjectManager.Player, true))
                {
                    Q.Cast(ts.Target);
                }

                if (ts.Target.Distance(ObjectManager.Player) < W.Range && W.IsReady() && !Utility.UnderTurret(ObjectManager.Player, true))
                {
                    W.Cast(ts.Target);
                }

                if (ts.Target.Distance(ObjectManager.Player) < R.Range && R.IsReady() && !Utility.UnderTurret(ObjectManager.Player, true))
                {
                    R.Cast(ts.Target);
                }
                if (E.IsReady())
                {
                    E.Cast();
                }
            }
            else
            {
                //if spells available, cast them.
                if (ts.Target.Distance(follow.Position) < 600 && follow.Distance(ObjectManager.Player.Position) < 700 && Q.IsReady() && !Utility.UnderTurret(ObjectManager.Player, true))
                {
                    Q.Cast(ts.Target);
                }

                if (ts.Target.Distance(follow.Position) < 600 && follow.Distance(ObjectManager.Player.Position) < 700 && W.IsReady() && !Utility.UnderTurret(ObjectManager.Player, true))
                {
                    W.Cast(ts.Target);
                }

                if (ts.Target.Distance(follow.Position) < 600 && follow.Distance(ObjectManager.Player.Position) < 700 && R.IsReady() && !Utility.UnderTurret(ObjectManager.Player, true))
                {
                    R.Cast(ts.Target);
                }
                if (ts.Target.Distance(follow.Position) < 600 && follow.Distance(ObjectManager.Player.Position) < 700 && E.IsReady() && !Utility.UnderTurret(ObjectManager.Player, true))
                {
                    E.Cast(ts.Target);
                }
            }
        }

        public static void BuyItems()
        {
            if (ObjectManager.Player.ChampionName == "Annie" || ObjectManager.Player.ChampionName == "Soraka")
            {
                if (Utility.InFountain() && ObjectManager.Player.Gold == 475 && !boughtItemOne)
                {
                    Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(1001)).Send();
                    Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(2003)).Send();
                    Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(3340)).Send();
                    Game.PrintChat("Buying Boots");
                    boughtItemOne = true;
                    Game.PrintChat(Game.Time.ToString());
                    System.Threading.Thread.Sleep(1500);
                }
                if (Utility.InShopRange() && ObjectManager.Player.Gold > 1200 && ObjectManager.Player.Gold < 2800 && !boughtItemTwo)
                {
                    Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(3010)).Send();
                    Game.PrintChat("Buying Catalyst");
                    boughtItemTwo = true;
                    System.Threading.Thread.Sleep(1500);
                }
                if (Utility.InShopRange() && boughtItemTwo && ObjectManager.Player.Gold > 1600)
                {
                    Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(3027)).Send();
                    Game.PrintChat("Buying RoA");
                    boughtItemTwo = false;
                    System.Threading.Thread.Sleep(1500);
                }
                if (Utility.InShopRange() && ObjectManager.Player.Gold > 2800)
                {
                    Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(3027)).Send();
                    Game.PrintChat("Buying RoA");
                    boughtItemTwo = false;
                    System.Threading.Thread.Sleep(1500);
                }
            }
            else
            {
                if (Utility.InFountain() && ObjectManager.Player.Gold == 475 && !boughtItemOne)
                {
                    Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(1001)).Send();
                    Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(2003)).Send();
                    Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(3340)).Send();
                    Game.PrintChat("BOUGHT BOTS");
                    boughtItemOne = true;
                    System.Threading.Thread.Sleep(1500);
                }
                if (Utility.InShopRange() && ObjectManager.Player.Gold > 1900 && ObjectManager.Player.Gold < 2550 && !boughtItemTwo)
                {
                    Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(3105)).Send();
                    Game.PrintChat("BOUGHT AEGIS");
                    boughtItemTwo = true;
                    System.Threading.Thread.Sleep(1500);
                }
                else if (Utility.InShopRange() && ObjectManager.Player.Gold > 2550 && !boughtItemThree)
                {
                    Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(3050)).Send();
                    Game.PrintChat("BOUGHT ZEKES");
                    boughtItemThree = true;
                    System.Threading.Thread.Sleep(1500);
                }
            }

        }
        private static void OnGameEnd(EventArgs args)
        {
            Game.Say("gg");
        }
    }
}

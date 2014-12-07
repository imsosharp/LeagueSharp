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
    class Autoplay
    {
        private static Obj_AI_Hero bot = ObjectManager.Player;
        private static Obj_AI_Hero carry;



        public static void Autoplay()
        {
            Game.OnGameUpdate += OnUpdate;
            Game.OnGameSendPacket += OnSendPacket;
            Game.OnGameProcessPacket += OnProcessPacket;
            Game.OnGameEnd += OnGameEnd;
            CustomEvents.Unit.OnLevelUp += OnLevelUp;
        }
        public static void OnUpdate(EventArgs args)
        {

        }
        public static void OnSendPacket(GamePacketEventArgs args)
        {

        }
        public static void OnProcessPacket(GamePacketEventArgs args)
        {

        }
        public static void OnLevelUp(Obj_AI_Base sender, CustomEvents.Unit.OnLevelUpEventArgs args)
        {

        }
        public static void OnGameEnd(EventArgs args)
        {

        }

        private static void doAutoplay()
        {
            
        }

    }
    
}

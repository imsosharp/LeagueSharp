using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace NiceGuySharp
{
    public class Program
    {
        public static Menu menu;
        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        public static void Game_OnGameLoad(EventArgs args)
        {

            menu = new Menu("Nice Guy Sharp", "niceguy", true);
            menu.AddItem(new MenuItem("enabled", "Enable").SetValue(true));
            
            menu.AddToMainMenu();
            Game.PrintChat("NICE GUY SHARP SUCCESFULLY LOADED.");

            Game.OnGameUpdate += Game_OnGameUpdate;
        }
        public static void Game_OnGameUpdate(EventArgs args)
        {
		
        }
    }
}

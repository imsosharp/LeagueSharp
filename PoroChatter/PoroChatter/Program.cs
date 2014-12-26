using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace PoroChatter
{
    public class Program
    {
        public static string contents = "";
        public static string[] linestoprint;
        public static Menu menu;
        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        public static void Game_OnGameLoad(EventArgs args)
        {

            menu = new Menu("Poro Chatter", "chatting", true);
            menu.AddSubMenu(new Menu("Announce:", "chatsettings"));
            menu.AddItem(new MenuItem("enabled", "Enable").SetValue(true));
            menu.SubMenu("chatsettings").AddItem(new MenuItem("EnemySummoners", "Enemy summoners").SetValue(true));
            menu.SubMenu("chatsettings").AddItem(new MenuItem("Baron", "Baron").SetValue(true));
            menu.SubMenu("chatsettings").AddItem(new MenuItem("Dragon", "Dragon").SetValue(true));
            
            menu.AddToMainMenu();
            Game.PrintChat("Poro Chatter loaded. Check out the menu by pressing the SHIFT key.");

            Game.OnGameUpdate += Game_OnGameUpdate;
        }
        public static void Game_OnGameUpdate(EventArgs args)
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;   
        }

        public static void OnProcessSpellCast(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            string[] summoners = { "Dot", "Flash", "Exhaust" };
            if (hero.IsEnemy && hero.IsValid)
            {
                foreach (var spell in summoners)
                {
                    if (menu.Item("EnemySummoners").GetValue<bool>())
                    {
                        if (args.SData.Name.Contains(spell))
                        {
                            if (spell.Contains("Dot"))
                            {
                                float spellUpIn = Game.ClockTime + 3.5f;
                                string message = hero.BaseSkinName.ToLower() + "ignite" + spellUpIn;
                                Game.PrintChat(message);
                            }
                            if (spell.Contains("Flash"))
                            {
                                float spellUpIn = Game.ClockTime + 5.0f;
                                string message = hero.BaseSkinName.ToLower() + "flash" + spellUpIn;
                                Game.PrintChat(message);
                            }
                            if (spell.Contains("Exhaust"))
                            {
                                float spellUpIn = Game.ClockTime + 3.5f;
                                string message = hero.BaseSkinName.ToLower() + "exhaust" + spellUpIn;
                                Game.PrintChat(message);
                            }
                        }
                    }
                }
            }
        }
    }
}

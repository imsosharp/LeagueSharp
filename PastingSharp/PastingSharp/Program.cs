using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using forms = System.Windows.Forms;

namespace PastingSharp
{
    public class Program
    {
        public static string contents = "";
        public static Menu menu;
        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        public static void Game_OnGameLoad(EventArgs args)
        {

            menu = new Menu("PastingSharp", "pasting", true);
            menu.AddItem(new MenuItem("paste", "Paste")).SetValue(new KeyBind("P".ToCharArray()[0], KeyBindType.Press));
            Game.PrintChat("PastingSharp loaded. Press P to paste.");

            Game.OnGameUpdate += Game_OnGameUpdate;
        }
        public static void Game_OnGameUpdate(EventArgs args)
        {

            if (forms.Clipboard.ContainsText())
            {
                contents = forms.Clipboard.GetText();
            }

            if (menu.Item("paste").GetValue<KeyBind>().Active)
            {
                Game.Say(contents);
            }
            
        }
    }
}

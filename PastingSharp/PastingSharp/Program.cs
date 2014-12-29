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
     class Program
    {
         static string contents = "";
         static string[] linestoprint;
         static Menu menu;
         static int _lastPaste;
         static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

         static void Game_OnGameLoad(EventArgs args)
        {

            menu = new Menu("PastingSharp", "pasting", true);
            menu.AddItem(new MenuItem("sleep", "Pause between pastes (seconds)").SetValue(new Slider(0, 0, 15)));
            menu.AddItem(new MenuItem("paste", "Paste").SetValue(new KeyBind("P".ToCharArray()[0], KeyBindType.Press)));
            menu.AddToMainMenu();
            Game.PrintChat("PastingSharp loaded. Press P to paste.");

            Game.OnGameUpdate += Game_OnGameUpdate;
        }
         static void Game_OnGameUpdate(EventArgs args)
        {

            if (forms.Clipboard.ContainsText())
            {
                contents = forms.Clipboard.GetText();
                if (contents.Contains("\n"))
                {
                    var separator = new string[] {"\n"};
                    linestoprint = contents.Split(separator,StringSplitOptions.None);
                }
            }

            if (menu.Item("paste").GetValue<KeyBind>().Active)
            {
                
                var sleep = (menu.Item("sleep").GetValue<int>()) * 1000;
                if (sleep == 0)
                {
                    if (linestoprint == null && contents != null)
                    {
                        Game.Say(contents);
                        _lastPaste = Environment.TickCount;
                    }
                    else if (linestoprint != null)
                    {
                        foreach (string s in linestoprint)
                        {
                            Game.Say(s);
                        }
                        var linestoprintsize = contents.Count();
                        Array.Clear(linestoprint, 0, linestoprintsize);
                        _lastPaste = Environment.TickCount;
                    }
                    if (linestoprint == null && contents == null)
                    {
                        Game.PrintChat("Your clipboard is empty, are you sure you copied something?");
                    }
                }
                if (sleep != 0)
                {
                    if (Environment.TickCount - _lastPaste > sleep || _lastPaste == 0)
                    {
                        if (linestoprint == null && contents != null)
                        {
                            Game.Say(contents);
                            _lastPaste = Environment.TickCount;
                        }
                        else if (linestoprint != null)
                        {
                            foreach (string s in linestoprint)
                            {
                                Game.Say(s);
                            }
                            var linestoprintsize = contents.Count();
                            Array.Clear(linestoprint, 0, linestoprintsize);
                            _lastPaste = Environment.TickCount;
                        }
                        if (linestoprint == null && contents == null)
                        {
                            Game.PrintChat("Your clipboard is empty, are you sure you copied something?");
                        }


                    }
                }
            }
            
        }
    }
}

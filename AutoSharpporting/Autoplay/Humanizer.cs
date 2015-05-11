//Copyright trees-software XD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharpporting.Autoplay
{
    public class Humanizer
    {
        public Humanizer()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
        public static Menu Menu;
        public static float LastMove;
        public static Obj_AI_Base Player = ObjectManager.Player;
        public static List<String> SpellList = new List<string> { "Q", "W", "E", "R" };
        public static List<float> LastCast = new List<float>();

        private static void Game_OnGameLoad(EventArgs args)
        {
            Menu = new Menu("Humanizer", "Humanizer", true);

            var spells = Menu.AddSubMenu(new Menu("Spells", "Spells"));

            for (var i = 0; i <= 3; i++)
            {
                LastCast.Add(0);
                var spell = SpellList[i];
                var menu = spells.AddSubMenu(new Menu(spell, spell));
                menu.AddItem(new MenuItem("Enabled" + i, "Delay " + spell, true).SetValue(true));
                menu.AddItem(new MenuItem("Delay" + i, "Cast Delay", true).SetValue(new Slider(250, 100, 1000)));
            }

            var move = Menu.AddSubMenu(new Menu("Movement", "Movement"));
            move.AddItem(new MenuItem("MovementEnabled", "Enabled").SetValue(true));
            move.AddItem(new MenuItem("MovementDelay", "Movement Delay")).SetValue(new Slider(250, 100, 1000));

            Menu.AddToMainMenu();

            Obj_AI_Base.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender == null || !sender.Owner.IsMe || Menu.Item("Enabled" + (int)args.Slot).GetValue<bool>())
            {
                return;
            }

            var delay = Menu.Item("Delay" + (int)args.Slot).GetValue<Slider>().Value;

            if (Environment.TickCount - LastCast[(int)args.Slot] < delay)
            {
                args.Process = false;
                return;
            }

            LastCast[(int)args.Slot] = Environment.TickCount;
        }

        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
        {
            if (sender == null || !sender.IsValid || !sender.IsMe || args.Order != GameObjectOrder.MoveTo ||
                !Menu.Item("MovementEnabled").GetValue<bool>())
            {
                return;
            }

            if (Environment.TickCount - LastMove < Menu.Item("MovementDelay").GetValue<Slider>().Value)
            {
                args.Process = false;
                return;
            }

            LastMove = Environment.TickCount;
        }
    }
}

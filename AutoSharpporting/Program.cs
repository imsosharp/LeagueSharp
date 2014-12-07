#region LICENSE

// Copyright 2014 - 2014 Support
// Program.cs is part of Support.
// Support is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// Support is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with Support. If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;
using Support.Util;
using Version = System.Version;

#endregion

namespace Support
{
    internal class Program
    {
        public static Version Version;

        private static void Main(string[] args)
        {
            new Autoplay();
            Version = Assembly.GetExecutingAssembly().GetName().Version;

            CustomEvents.Game.OnGameLoad += a =>
            {
                try
                {
                    var type = Type.GetType("Support.Plugins." + ObjectManager.Player.ChampionName);

                    if (type != null)
                    {
                        Protector.Init();
                        Activator.CreateInstance(type);
                        return;
                    }

                    Helpers.PrintMessage(ObjectManager.Player.ChampionName + " not supported");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            };

            //Utils.EnableConsoleEditMode();

            //Drawing.OnDraw += a =>
            //{
            //    var offset = 0;
            //    foreach (var buff in ObjectManager.Player.Buffs)
            //    {
            //        Drawing.DrawText(100, 100 + offset, Color.Tomato,
            //            string.Format("{0} | {1} | {2} | {3} | {4} | {5} | {6}", buff.Name, buff.DisplayName,
            //                buff.Type.ToString(), buff.Count, buff.IsActive, buff.StartTime, buff.EndTime));
            //        offset += 15;
            //    }
            //};

            //Obj_AI_Base.OnProcessSpellCast += (sender, spell) =>
            //{
            //    if (!sender.IsValid<Obj_AI_Hero>())
            //        return;

            //    try
            //    {
            //        if (!Orbwalking.IsAutoAttack(spell.SData.Name))
            //            Console.WriteLine(sender.Name + " | " + spell.SData.Name.ToLower());
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //    }
            //};

            //GameObject.OnCreate += (sender, eventArgs) =>
            //{
            //    if (!sender.IsValid<Obj_SpellMissile>())
            //        return;

            //    try
            //    {
            //        var miss = (Obj_SpellMissile)sender;
            //        Console.WriteLine(sender.Name + " | " + miss.SData.Name.ToLower());
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //    }
            //};
        }
    }
}
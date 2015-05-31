#region LICENSE

// Copyright 2014 - 2014 Support
// Helpers.cs is part of Support.
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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Version = System.Version;

#endregion

namespace Support.Util
{
    internal static class Helpers
    {
        /// <summary>
        ///     ReversePosition
        /// </summary>
        /// <param name="positionMe"></param>
        /// <param name="positionEnemy"></param>
        /// <remarks>Credit to LXMedia1</remarks>
        /// <returns>Vector3</returns>
        public static Vector3 ReversePosition(Vector3 positionMe, Vector3 positionEnemy)
        {
            var x = positionMe.X - positionEnemy.X;
            var y = positionMe.Y - positionEnemy.Y;
            return new Vector3(positionMe.X + x, positionMe.Y + y, positionMe.Z);
        }

        public static void UpdateCheck()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var c = new WebClient())
                    {
                        var rawVersion =
                            c.DownloadString(
                                "https://raw.githubusercontent.com/imsosharp/LeagueSharp/master/AutoSharpporting/Properties/AssemblyInfo.cs");
                        var match =
                            new Regex(@"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]")
                                .Match
                                (rawVersion);

                        if (match.Success)
                        {
                            var gitVersion =
                                new Version(string.Format("{0}.{1}.{2}.{3}", match.Groups[1], match.Groups[2],
                                    match.Groups[3],
                                    match.Groups[4]));

                            if (gitVersion != Program.Version)
                            {
                                Game.PrintChat("<font color='#15C3AC'>AutoSharpporting:</font> <font color='#FF0000'>" +
                                               "OUTDATED - Please Update to Version: " + gitVersion + "</font>");
                                Game.PrintChat("<font color='#15C3AC'>AutoSharpporting:</font> <font color='#FF0000'>" +
                                               "OUTDATED - Please Update to Version: " + gitVersion + "</font>");
                                Game.PrintChat("<font color='#15C3AC'>AutoSharpporting:</font> <font color='#FF0000'>" +
                                               "OUTDATED - Please Update to Version: " + gitVersion + "</font>");
                                Game.PrintChat("<font color='#15C3AC'>AutoSharpporting:</font> <font color='#FF0000'>" +
                                               "OUTDATED - Please Update to Version: " + gitVersion + "</font>");
                                Game.PrintChat("<font color='#15C3AC'>AutoSharpporting:</font> <font color='#FF0000'>" +
                                               "OUTDATED - Please Update to Version: " + gitVersion + "</font>");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        public static void PrintMessage(string message)
        {
            Game.PrintChat("<font color='#15C3AC'>AutoSharpporting:</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        public static bool EnemyInRange(int numOfEnemy, float range)
        {
            return ObjectManager.Player.CountEnemiesInRange((int) range) >= numOfEnemy;
        }
        public static List<Obj_AI_Hero> AllyInRange(float range)
        {
            return
            ObjectManager.Get<Obj_AI_Hero>()
            .Where(
            h =>
            ObjectManager.Player.Distance(h.Position) < range && h.IsAlly && !h.IsMe && h.IsValid && !h.IsDead).OrderBy(h => ObjectManager.Player.Distance(h.Position)).ToList();
        }
        public static Obj_AI_Hero AllyBelowHp(int percentHp, float range)
        {
            var allyBelowHp = 
                ObjectManager.Get<Obj_AI_Hero>()
                    .First(
                        h =>
                            !h.IsMe && ((h.Health / h.MaxHealth) * 100) < percentHp &&
                            h.Distance(ObjectManager.Player) < range);
            return allyBelowHp;
        }
    }
}
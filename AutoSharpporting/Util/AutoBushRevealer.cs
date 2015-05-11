#region LICENSE

// Copyright 2014-2015 Support
// AutoBushRevealer.cs is part of Support.
// 
// Support is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Support is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Support. If not, see <http://www.gnu.org/licenses/>.
// 
// Filename: Support/Support/AutoBushRevealer.cs
// Created:  26/11/2014
// Date:     20/01/2015/11:20
// Author:   h3h3

#endregion

namespace AutoSharpporting.Util
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    #endregion

    public class AutoBushRevealer
    {
        private static readonly List<KeyValuePair<int, String>> Wards = new List<KeyValuePair<int, String>>
        {
            new KeyValuePair<int, String>(3340, "Warding Totem Trinket"),
            new KeyValuePair<int, String>(3361, "Greater Stealth Totem Trinket"),
            new KeyValuePair<int, String>(3205, "Quill Coat"),
            new KeyValuePair<int, String>(3207, "Spirit Of The Ancient Golem"),
            new KeyValuePair<int, String>(3154, "Wriggle's Lantern"),
            new KeyValuePair<int, String>(2049, "Sight Stone"),
            new KeyValuePair<int, String>(2045, "Ruby Sightstone"),
            new KeyValuePair<int, String>(3160, "Feral Flare"),
            new KeyValuePair<int, String>(2050, "Explorer's Ward"),
            new KeyValuePair<int, String>(2044, "Stealth Ward")
        };

        private readonly List<EnemyInfo> _enemyInfo;
        private readonly Menu _menu;
        private int _lastTimeWarded;

        public AutoBushRevealer(Menu menu)
        {
            _enemyInfo = ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsEnemy).Select(x => new EnemyInfo(x)).ToList();

            _menu = menu.AddSubMenu(new Menu("Auto Bush Revealer", "AutoBushRevealerMenu"));
            _menu.AddItem(new MenuItem("Auto Bush Revealer", "AutoBushRevealer").SetValue(true));
            _menu.AddItem(new MenuItem("AutoBushEnabled", "Enabled").SetValue(true));

            var useWardsMenu = _menu.AddSubMenu(new Menu("Use Wards: ", "AutoBushUseWards"));

            foreach (var ward in Wards)
            {
                useWardsMenu.AddItem(new MenuItem("AutoBush" + ward.Key, ward.Value).SetValue(true));
            }

            //Game.OnUpdate += Game_OnUpdate; // TODO: wait for update
        }

        private InventorySlot GetWardSlot()
        {
            return
                Wards.Select(x => x.Key)
                    .Where(id => _menu.Item("AutoBush" + id).GetValue<bool>() && Items.CanUseItem(id))
                    .Select(
                        wardId => ObjectManager.Player.InventoryItems.FirstOrDefault(slot => slot.Id == (ItemId) wardId))
                    .FirstOrDefault();
        }

        private Obj_AI_Base GetNearObject(String name, Vector3 pos, int maxDistance)
        {
            return ObjectManager.Get<Obj_AI_Base>()
                .FirstOrDefault(x => x.Name == name && x.Distance(pos) <= maxDistance);
        }

        private void Game_OnUpdate(EventArgs args)
        {
            var time = Environment.TickCount;

            foreach (var enemyInfo in _enemyInfo.Where(x => x.Player.IsVisible))
            {
                enemyInfo.LastSeen = time;
            }

            if (PluginBase.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            if (_menu.Item("AutoBushEnabled").GetValue<bool>())
            {
                foreach (var enemy in
                    _enemyInfo.Where(
                        x =>
                            x.Player.IsValid && !x.Player.IsVisible && !x.Player.IsDead &&
                            x.Player.Distance(ObjectManager.Player.ServerPosition) < 1000 && time - x.LastSeen < 2500)
                        .Select(x => x.Player))
                {
                    var bestWardPos = GetWardPos(enemy.ServerPosition, 165, 2);

                    if (bestWardPos != enemy.ServerPosition && bestWardPos != Vector3.Zero &&
                        bestWardPos.Distance(ObjectManager.Player.ServerPosition) <= 600)
                    {
                        var timedif = Environment.TickCount - _lastTimeWarded;

                        if (timedif > 1250 && !(timedif < 2500 && GetNearObject("SightWard", bestWardPos, 200) != null))
                            //no near wards
                        {
                            var wardSlot = GetWardSlot();

                            if (wardSlot != null && wardSlot.Id != ItemId.Unknown)
                            {
                                //wardSlot.UseItem(bestWardPos); // TODO fix for 4.21
                                _lastTimeWarded = Environment.TickCount;
                            }
                        }
                    }
                }
            }
        }

        private Vector3 GetWardPos(Vector3 lastPos, int radius = 165, int precision = 3)
        {
            return new Vector3();
            //    var count = precision;

            //    while (count > 0)
            //    {
            //        var vertices = radius;

            //        var wardLocations = new WardLocation[vertices];
            //        var angle = 2*Math.PI/vertices;

            //        for (var i = 0; i < vertices; i++)
            //        {
            //            var th = angle*i;
            //            var pos = new Vector3((float) (lastPos.X + radius*Math.Cos(th)),
            //                (float) (lastPos.Y + radius*Math.Sin(th)), 0);
            //            wardLocations[i] = new WardLocation(pos, NavMesh.IsWallOfGrass(pos, 50)); // TODO: check later
            //        }

            //        var grassLocations = new List<GrassLocation>();

            //        for (var i = 0; i < wardLocations.Length; i++)
            //        {
            //            if (!wardLocations[i].Grass) continue;
            //            if (i != 0 && wardLocations[i - 1].Grass)
            //                grassLocations.Last().Count++;
            //            else
            //                grassLocations.Add(new GrassLocation(i, 1));
            //        }

            //        var grassLocation = grassLocations.OrderByDescending(x => x.Count).FirstOrDefault();

            //        if (grassLocation != null) //else: no pos found. increase/decrease radius?
            //        {
            //            var midelement = (int) Math.Ceiling(grassLocation.Count/2f);
            //            lastPos = wardLocations[grassLocation.Index + midelement - 1].Pos;
            //            radius = (int) Math.Floor(radius/2f);
            //        }

            //        count--;
            //    }

            //    return lastPos;
        }

        private class EnemyInfo
        {
            public EnemyInfo(Obj_AI_Hero player)
            {
                Player = player;
            }

            public Obj_AI_Hero Player { get; set; }
            public int LastSeen { get; set; }
        }

        private class GrassLocation
        {
            public readonly int Index;
            public int Count;

            public GrassLocation(int index, int count)
            {
                Index = index;
                Count = count;
            }
        }

        private class WardLocation
        {
            public readonly bool Grass;
            public readonly Vector3 Pos;

            public WardLocation(Vector3 pos, bool grass)
            {
                Pos = pos;
                Grass = grass;
            }
        }
    }
}
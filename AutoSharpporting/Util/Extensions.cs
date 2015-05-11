#region LICENSE

// Copyright 2014-2015 Support
// Extensions.cs is part of Support.
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
// Filename: Support/Support/Extensions.cs
// Created:  26/11/2014
// Date:     20/01/2015/11:20
// Author:   h3h3

#endregion

namespace AutoSharpporting.Util
{
    #region

    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    public enum HeroType
    {
        Ap,
        Support,
        Tank,
        Ad,
        Bruiser
    }

    public static class Extensions
    {
        private static readonly string[] Ap =
        {
            "ahri", "akali", "anivia", "annie", "brand", "cassiopeia", "diana",
            "fiddlesticks", "fizz", "gragas", "heimerdinger", "karthus", "kassadin", "katarina", "kayle", "kennen",
            "leblanc", "lissandra", "lux", "malzahar", "mordekaiser", "morgana", "nidalee", "orianna", "ryze", "sion",
            "swain", "syndra", "teemo", "twistedfate", "veigar", "viktor", "vladimir", "xerath", "ziggs", "zyra",
            "velkoz"
        };

        private static readonly string[] Sup =
        {
            "blitzcrank", "janna", "karma", "leona", "lulu", "nami", "sona",
            "soraka", "thresh", "zilean"
        };

        private static readonly string[] Tank =
        {
            "amumu", "chogath", "drmundo", "galio", "hecarim", "malphite",
            "maokai", "nasus", "rammus", "sejuani", "shen", "singed", "skarner", "volibear", "warwick", "yorick", "zac",
            "nunu", "taric", "alistar", "garen", "nautilus", "braum"
        };

        private static readonly string[] Ad =
        {
            "ashe", "caitlyn", "corki", "draven", "ezreal", "graves", "kogmaw",
            "missfortune", "quinn", "sivir", "talon", "tristana", "twitch", "urgot", "varus", "vayne", "zed", "jinx",
            "yasuo", "lucian"
        };

        private static readonly string[] Bruiser =
        {
            "darius", "elise", "evelynn", "fiora", "gangplank", "gnar", "jayce",
            "pantheon", "irelia", "jarvaniv", "jax", "khazix", "leesin", "nocturne", "olaf", "poppy", "renekton",
            "rengar", "riven", "shyvana", "trundle", "tryndamere", "udyr", "vi", "monkeyking", "xinzhao", "aatrox",
            "rumble", "shaco", "masteryi"
        };

        public static bool IsHeroType(this Obj_AI_Base obj, HeroType type)
        {
            switch (type)
            {
                case HeroType.Ad:
                    return obj.IsValid<Obj_AI_Hero>() && Ad.Contains(obj.BaseSkinName.ToLowerInvariant());

                case HeroType.Ap:
                    return obj.IsValid<Obj_AI_Hero>() && Ap.Contains(obj.BaseSkinName.ToLowerInvariant());

                case HeroType.Bruiser:
                    return obj.IsValid<Obj_AI_Hero>() && Bruiser.Contains(obj.BaseSkinName.ToLowerInvariant());

                case HeroType.Support:
                    return obj.IsValid<Obj_AI_Hero>() && Sup.Contains(obj.BaseSkinName.ToLowerInvariant());

                case HeroType.Tank:
                    return obj.IsValid<Obj_AI_Hero>() && Tank.Contains(obj.BaseSkinName.ToLowerInvariant());
            }

            return false;
        }

        public static bool IsValidAlly(this Obj_AI_Base unit, float range = float.MaxValue)
        {
            return unit.Distance(ObjectManager.Player) < range && unit.IsValid<Obj_AI_Hero>() && unit.IsAlly &&
                   !unit.IsDead && unit.IsTargetable;
        }

        public static double HealthBuffer(this Obj_AI_Base hero, int buffer)
        {
            return hero.Health - (hero.MaxHealth * buffer / 100);
        }

        public static bool CastCheck(this Items.Item item, Obj_AI_Base target)
        {
            return item.IsReady() && target.IsValidTarget(item.Range);
        }

        public static bool CastCheck(this Spell spell,
            Obj_AI_Base target,
            string menu,
            bool range = true,
            bool team = true)
        {
            return spell.IsReady() && target.IsValidTarget(range ? spell.Range : float.MaxValue, team) &&
                   PluginBase.Config.Item(menu + ObjectManager.Player.ChampionName).GetValue<bool>();
        }

        public static bool CastCheck(this Spell spell, Obj_AI_Base target, bool range = true, bool team = true)
        {
            return spell.IsReady() && target.IsValidTarget(range ? spell.Range : float.MaxValue, team) && !ObjectManager.Player.UnderTurret(true);
        }

        public static bool IsInRange(this Spell spell, Obj_AI_Base target)
        {
            return ObjectManager.Player.Distance(target) < spell.Range;
        }

        public static bool IsInRange(this Items.Item item, Obj_AI_Base target)
        {
            return ObjectManager.Player.Distance(target) < item.Range;
        }

        public static bool WillKill(this Obj_AI_Base caster, Obj_AI_Base target, string spell, int buffer = 10)
        {
            return caster.GetSpellDamage(target, spell) >= target.HealthBuffer(buffer);
        }

        public static bool WillKill(this Obj_AI_Base caster, Obj_AI_Base target, SpellData spell, int buffer = 10)
        {
            return caster.GetSpellDamage(target, spell.Name) >= target.HealthBuffer(buffer);
        }

        public static bool WillKill(this Obj_AI_Base caster, Obj_AI_Base target, Evade.SpellData spell, int buffer = 10)
        {
            return caster.GetSpellDamage(target, spell.SpellName) >= target.HealthBuffer(buffer);
        }

        public static void AddList(this Menu menu, string name, string displayName, string[] list)
        {
            menu.AddItem(
                new MenuItem(name + ObjectManager.Player.ChampionName, displayName).SetValue(new StringList(list)));
        }

        public static void AddBool(this Menu menu, string name, string displayName, bool value)
        {
            menu.AddItem(new MenuItem(name + ObjectManager.Player.ChampionName, displayName).SetValue(value));
        }

        public static void AddSlider(this Menu menu, string name, string displayName, int value, int min, int max)
        {
            menu.AddItem(
                new MenuItem(name + ObjectManager.Player.ChampionName, displayName).SetValue(
                    new Slider(value, min, max)));
        }

        public static void AddObject(this Menu menu, string name, string displayName, object value)
        {
            menu.AddItem(new MenuItem(name + ObjectManager.Player.ChampionName, displayName).SetValue(value));
        }
    }
}
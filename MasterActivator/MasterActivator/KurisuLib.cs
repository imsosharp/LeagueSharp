/* KurisuLib borrowed from Kurisu's Morgana
Kurisu is a pretty cool bro and a good developer.
You can find his github here:
https://github.com/Kurisu44/
*/

using System;
using LeagueSharp;
using LeagueSharp.Common;
using System.Collections.Generic;

namespace MasterActivator
{
    public enum Skilltype
    {
        Unknown = 0,
        Line = 1,
        Circle = 2,
        Cone = 3
    }

    public class KurisuLib
    {
        public string HeroName { get; set; }
        public string SpellMenuName { get; set; }
        public SpellSlot Slot { get; set; }
        public Skilltype Type { get; set; }
        public float Radius { get; set; }
        public string SDataName { get; set; }
        public int DangerLevel { get; set; }

        public static List<KurisuLib> GDList = new List<KurisuLib>(); // Generic Dangerous List
        public static List<KurisuLib> CCList = new List<KurisuLib>(); // Crowd Control List
        public static List<KurisuLib> SList = new List<KurisuLib>();  // Silence List

        static KurisuLib()
        {
            #region CCList
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Aatorx",
                    SpellMenuName = "Dark Flight",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    SDataName = "AatroxQ",
                    DangerLevel = 5
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Aatorx",
                    SpellMenuName = "Blades of Torment",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Cone,
                    SDataName = "AatroxE",
                    DangerLevel = 3
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Ahri",
                    SpellMenuName = "Charm",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    SDataName = "AhriSeduce",
                    DangerLevel = 5
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Alistar",
                    SpellMenuName = "Pulverize",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    SDataName = "Pulverize",
                    DangerLevel = 5
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Alistar",
                    SpellMenuName = "Headbutt",
                    Slot = SpellSlot.W,
                    SDataName = "Headbutt",
                    DangerLevel = 3
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Amumu",
                    SpellMenuName = "Bandage Toss",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    SDataName = "BandageToss",
                    DangerLevel = 5
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Amumu",
                    SpellMenuName = "Curse of the Sad Mummy",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    SDataName = "CurseoftheSadMummy",
                    DangerLevel = 5
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Anivia",
                    SpellMenuName = "Flash Frost",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    SDataName = "FlashFrost",
                    DangerLevel = 5
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Anivia",
                    SpellMenuName = "Glacial Storm",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    SDataName = "GlacialStorm",
                    DangerLevel = 3
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Annie",
                    SpellMenuName = "Tibbers",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    SDataName = "InfernalGuardian",
                    DangerLevel = 5
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Ashe",
                    SpellMenuName = "Crystal Arrow",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    SDataName = "EnchantedCrystalArrow",
                    DangerLevel = 5
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Ashe",
                    SpellMenuName = "Volley",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Cone,
                    SDataName = "Volley",
                    DangerLevel = 3
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Azir",
                    SpellMenuName = "ShiftingSands",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    SDataName = "AzirE",
                    DangerLevel = 3
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Azir",
                    SpellMenuName = "Emperor's Divide",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    SDataName = "AzirR",
                    DangerLevel = 5
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Blitzcrank",
                    SpellMenuName = "Rocket Grab",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    SDataName = "RocketGrab",
                    DangerLevel = 5
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Blitzcrank",
                    SpellMenuName = "Power Fist",
                    Slot = SpellSlot.E,
                    SDataName = "PowerFist",
                    DangerLevel = 3
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Brand",
                    SpellMenuName = "Sear",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    SDataName = "BrandBlazeMissile",
                    DangerLevel = 3
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Bruam",
                    SpellMenuName = "Winter's Bite",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    SDataName = "BraumQ",
                    DangerLevel = 3
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Bruam",
                    SpellMenuName = "Glacial Fissure",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    SDataName = "BraumR",
                    DangerLevel = 5
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Caitlyn",
                    SpellMenuName = "90 Caliber Net",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    SDataName = "CaitlynEntrapment",
                    DangerLevel = 3
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Cassiopeia",
                    SpellMenuName = "Petrifying Gaze",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Cone,
                    SDataName = "CassiopeiaPetrifyingGaze",
                    DangerLevel = 5
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Cho'gath",
                    SpellMenuName = "Rupture",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    SDataName = "Rupture",
                    DangerLevel = 5
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Darius",
                    SpellMenuName = "Aprehend",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Cone,
                    SDataName = "DariusAxeGrabCone",
                    DangerLevel = 3
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Diana",
                    SpellMenuName = "Moonfall",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    SDataName = "DianaVortex",
                    DangerLevel = 3
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "DrMundo",
                    SpellMenuName = "Cleaver",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    SDataName = "InfectedCleaverMissileCast",
                    DangerLevel = 3
                });

            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Draven",
                    SpellMenuName = "Stand Aside",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    SDataName = "DravenDoubleShot",
                    DangerLevel = 3
                });

            CCList.Add(
               new KurisuLib
               {
                   HeroName = "Elise",
                   SpellMenuName = "Cocoon",
                   Slot = SpellSlot.E,
                   Type = Skilltype.Line,
                   SDataName = "DravenDoubleShot",
                   DangerLevel = 3
               });

            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Evelynn",
                    SpellMenuName = "Agony's Embrace",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "EvelynnR",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Fizz",
                    SpellMenuName = "Chum the Waters",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "FizzMarinerDoomMissile",
                });
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Fizz",
                     SpellMenuName = "Playful Trickster",
                     Slot = SpellSlot.E,
                     Type = Skilltype.Line,
                     DangerLevel = 3,
                     SDataName = "FizzJump",
                 });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Galio",
                    SpellMenuName = "Resolute Smite",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "GalioResoluteSmite",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Galio",
                    SpellMenuName = "Idol Of Durand",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "GalioIdolOfDurand",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Gnar",
                    SpellMenuName = "Boomerang Throw",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "GnarQ",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Gnar",
                    SpellMenuName = "Bouldar Toss",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "GnarBigQ",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Gnar",
                    SpellMenuName = "Wallop",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "GnarBigW",
                });

            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Gnar",
                    SpellMenuName = "GNAR!",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "GnarR",
                });

            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Gragas",
                    SpellMenuName = "Barrel Roll",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "GragasQ",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Gragas",
                    SpellMenuName = "Body Slam",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "GragasE",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Gragas",
                    SpellMenuName = "Explosive Cask",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "GragasR",
                });

            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Heimerdinger",
                    SpellMenuName = "Electron Storm Grenade",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "HeimerdingerE",
                });
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Hecarim",
                     SpellMenuName = "Onslaught of Shadows",
                     Slot = SpellSlot.R,
                     Type = Skilltype.Circle,
                     DangerLevel = 5,
                     SDataName = "HecarimUlt",
                 });
            CCList.Add(
                  new KurisuLib
                  {
                      HeroName = "Hecarim",
                      SpellMenuName = "Devestating Charge",
                      Slot = SpellSlot.E,
                      Type = Skilltype.Circle,
                      DangerLevel = 3,
                      SDataName = "HecarimRamp",
                  });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Janna",
                    SpellMenuName = "Howling Gale",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "HowlingGale",
                });
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Janna",
                     SpellMenuName = "Zephyr",
                     Slot = SpellSlot.W,
                     DangerLevel = 3,
                     SDataName = "ReapTheWhirlwind",
                 });
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Jax",
                     SpellMenuName = "Counter Strike",
                     Slot = SpellSlot.E,
                     Type = Skilltype.Line,
                     DangerLevel = 5,
                     SDataName = "JaxCounterStrike",
                 });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "JarvanIV",
                    SpellMenuName = "Dragon Strike",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "JarvanIVDragonStrike",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Jayce",
                    SpellMenuName = "Thundering Blow",
                    Slot = SpellSlot.E,
                    DangerLevel = 3,
                    SDataName = "JayceThunderingBlow",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Jinx",
                    SpellMenuName = "Zap!",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "JinxW",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Jinx",
                    SpellMenuName = "Chompers!",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 4,
                    SDataName = "JinxE",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Karma",
                    SpellMenuName = "Inner Flame (Mantra)",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "KarmaQMantra",
                });
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Karma",
                     SpellMenuName = "Sprit Bond",
                     Slot = SpellSlot.W,
                     DangerLevel = 3,
                     SDataName = "KarmaQMantra",
                 });

            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Kassadin",
                    SpellMenuName = "Force Pulse",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Cone,
                    DangerLevel = 3,
                    SDataName = "ForcePulse",
                });

            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Khazix",
                    SpellMenuName = "Void Spikes",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "KhazixW",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Kayle",
                    SpellMenuName = "Reckoning",
                    Slot = SpellSlot.Q,
                    DangerLevel = 3,
                    SDataName = "JudicatorReckoning",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "KogMaw",
                    SpellMenuName = "Void Ooze",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "KogMawVoidOoze",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Leblanc",
                    SpellMenuName = "Soul Shackle",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "LeblancSoulShackle",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Leblanc",
                    SpellMenuName = "Soul Shackle (Mimic)",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "LeblancSoulShackleM",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "LeeSin",
                    SpellMenuName = "Dragon's Rage",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "BlindMonkRKick",
                });
            CCList.Add(
new KurisuLib
{
    HeroName = "Leona",
    SpellMenuName = "Zenith Blade",
    Slot = SpellSlot.E,
    Type = Skilltype.Line,
    DangerLevel = 3,
    SDataName = "LeonaZenithBlade",
});
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Leona",
                     SpellMenuName = "Shield of Daybreak",
                     Slot = SpellSlot.Q,
                     DangerLevel = 3,
                     SDataName = "LeonaShieldOfDaybreak",
                 });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Leona",
                    SpellMenuName = "Solar Flare",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "LeonaSolarFlare",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Lissandra",
                    SpellMenuName = "Ice Shard",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "LissandraQ",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Lissandra",
                    SpellMenuName = "Ring of Frost",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "LissandraW",
                });

            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Lulu",
                    SpellMenuName = "Glitterlance",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "LuluQ"
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Lulu",
                    SpellMenuName = "Glitterlance: Extended",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "LuluQMissileTwo"
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Lux",
                    SpellMenuName = "Light Binding",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "LuxLightBinding",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Lux",
                    SpellMenuName = "Lucent Singularity",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "LuxLightStrikeKugel",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Lux",
                    SpellMenuName = "Final Spark",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "LuxMaliceCannon",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Malphite",
                    SpellMenuName = "Unstoppable Force",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "UFSlash",
                });
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Malphite",
                     SpellMenuName = "Sismic Shard",
                     Slot = SpellSlot.Q,
                     Type = Skilltype.Circle,
                     DangerLevel = 3,
                     SDataName = "SismicShard",
                 });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Malzahar",
                    SpellMenuName = "Nether Grasp",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "AlZaharNetherGrasp",
                });
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Maokai",
                     SpellMenuName = "Twisted Advance",
                     Slot = SpellSlot.W,
                     DangerLevel = 3,
                     SDataName = "MaokaiUnstableGrowth",
                 });
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Maokai",
                     SpellMenuName = "Arcane Smash",
                     Slot = SpellSlot.Q,
                     DangerLevel = 3,
                     SDataName = "MaokaiTrunkLine",
                 });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Morgana",
                    SpellMenuName = "Dark Binding",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "DarkBindingMissile",
                });
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Mordekaiser",
                     SpellMenuName = "Children of the Grave",
                     Slot = SpellSlot.Q,
                     DangerLevel = 5,
                     SDataName = "MordekaiserChildrenOfTheGrave",
                 });
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Wukong",
                     SpellMenuName = "Cyclone",
                     Slot = SpellSlot.R,
                     Type = Skilltype.Circle,
                     DangerLevel = 5,
                     SDataName = "MonkeyKingSpinToWin",
                 });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Nami",
                    SpellMenuName = "Aqua Prision",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "NamiQ",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Nasus",
                    SpellMenuName = "Wither",
                    Slot = SpellSlot.Q,
                    DangerLevel = 3,
                    SDataName = "NasusW",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Karthus",
                    SpellMenuName = "Wall of Pain",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "KarthusWallOfPain",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Nami",
                    SpellMenuName = "Tidal Wave",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "NamiR",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Nautilus",
                    SpellMenuName = "Dredge Line",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "NautilusAnchorDragMissile",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Nautilus",
                    SpellMenuName = "Riptide",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "NautilusSplashZone",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Nautilus",
                    SpellMenuName = "Depth Charge",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "NautilusGrandLine",
                });

            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Nidalee",
                    SpellMenuName = "Javelin Toss",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "JavelinToss",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Olaf",
                    SpellMenuName = "Undertow",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "OlafAxeThrowCast",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Orianna",
                    SpellMenuName = "Command: Dissonance ",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "OrianaDissonanceCommand",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Orianna",
                    SpellMenuName = "OrianaDetonateCommand",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "OrianaDetonateCommand",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Quinn",
                    SpellMenuName = "Blinding Assault",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "QuinnQ",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Rammus",
                    SpellMenuName = "Puncturing Taunt",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "PuncturingTaunt",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Rengar",
                    SpellMenuName = "Bola Strike (Emp)",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "RengarEFinal",
                });

            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Fiddlesticks",
                     SpellMenuName = "Terrify",
                     Slot = SpellSlot.Q,
                     DangerLevel = 3,
                     SDataName = "Terrify",
                 });
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Renekton",
                     SpellMenuName = "Ruthless Predator",
                     Slot = SpellSlot.W,
                     DangerLevel = 3,
                     SDataName = "RenektonPreExecute",
                 });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Riven",
                    SpellMenuName = "Ki Burst",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "RivenMartyr"
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Rumble",
                    SpellMenuName = "RumbleGrenade",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "RumbleGrenade",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Rumble",
                    SpellMenuName = "RumbleCarpetBombM",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 4,
                    SDataName = "RumbleCarpetBombMissile",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Ryze",
                    SpellMenuName = "Rune Prision",
                    Slot = SpellSlot.W,
                    DangerLevel = 3,
                    SDataName = "RunePrison",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Sejuani",
                    SpellMenuName = "Arctic Assault",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "SejuaniArcticAssault",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Sejuani",
                    SpellMenuName = "Glacial Prision",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "SejuaniGlacialPrisonStart",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Singed",
                    SpellMenuName = "Mega Adhesive",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "MegaAdhesive",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Singed",
                    SpellMenuName = "Fling",
                    Slot = SpellSlot.E,
                    DangerLevel = 2,
                    SDataName = "Fling",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Nocturne",
                    SpellMenuName = "Unspeakable Horror",
                    Slot = SpellSlot.E,
                    DangerLevel = 3,
                    SDataName = "NocturneUnspeakableHorror",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Shen",
                    SpellMenuName = "ShenShadowDash",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "ShenShadowDash",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Shyvana",
                    SpellMenuName = "ShyvanaTransformCast",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "ShyvanaTransformCast",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Skarner",
                    SpellMenuName = "Fracture",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "SkarnerFractureMissile",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Skarner",
                    SpellMenuName = "Impale",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "SkarnerFractureMissile",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Pantheon",
                    SpellMenuName = "Aegis of Zeonia",
                    Slot = SpellSlot.W,
                    DangerLevel = 3,
                    SDataName = "PantheonW",
                });
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Pantheon",
                     SpellMenuName = "Heroic Charge",
                     Slot = SpellSlot.W,
                     DangerLevel = 3,
                     SDataName = "PoppyHeroicCharge",
                 });
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Nunu",
                     SpellMenuName = "Ice Blast",
                     Slot = SpellSlot.E,
                     DangerLevel = 3,
                     SDataName = "Ice Blast",
                 });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Sona",
                    SpellMenuName = "Crescendo",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "SonaCrescendo",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Swain",
                    SpellMenuName = "Nevermove",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "SwainShadowGrasp",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Syndra",
                    SpellMenuName = "Scatter the Weak",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Cone,
                    DangerLevel = 5,
                    SDataName = "SyndraE",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Thresh",
                    SpellMenuName = "Death Sentence",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "ThreshQ",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Thresh",
                    SpellMenuName = "Flay",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "ThreshEFlay",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Tristana",
                    SpellMenuName = "Buster Shot",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "BusterShot",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Trundle",
                    SpellMenuName = "Pillar of Ice",
                    Slot = SpellSlot.E,
                    DangerLevel = 3,
                    SDataName = "TrundleCircle",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Trundle",
                    SpellMenuName = "Subjugate",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "TrundlePain",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Tryndamere",
                    SpellMenuName = "Mocking Shout",
                    Slot = SpellSlot.W,
                    DangerLevel = 3,
                    SDataName = "MockingShout",
                });

            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Twitch",
                    SpellMenuName = "Venom Cask",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "TwitchVenomCaskMissile",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Urgot",
                    SpellMenuName = "Corrosive Charge",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "UrgotPlasmaGrenadeBoom",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Varus",
                    SpellMenuName = "Hail of Arrowws",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "VarusE",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Varus",
                    SpellMenuName = "Chain of Corruption",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "VarusR",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Veigar",
                    SpellMenuName = "Event Horizon",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "VeigarEventHorizon",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Velkoz",
                    SpellMenuName = "VelkozQ",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "VelkozQ",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Velkoz",
                    SpellMenuName = "Plasma Fission",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "VelkozQSplit",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Velkoz",
                    SpellMenuName = "Tectonic Disruption",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "VelkozE",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Vi",
                    SpellMenuName = "Vault Breaker",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "ViQ",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Vi",
                    SpellMenuName = "Assault and Battery",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "ViR",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Viktor",
                    SpellMenuName = "Gravity Field",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "ViktorGravitonField",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Vayne",
                    SpellMenuName = "Condemn",
                    Slot = SpellSlot.E,
                    DangerLevel = 3,
                    SDataName = "Vayne Condemn",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Warwick",
                    SpellMenuName = "Infinite Duress",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "InfiniteDuress",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Xerath",
                    SpellMenuName = "Eye of Destruction",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "XerathArcaneBarrage2",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Xerath",
                    SpellMenuName = "Shocking Orb",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "XerathMageSpearMissile",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "XinZhao",
                    SpellMenuName = "Three Talon Strike",
                    Slot = SpellSlot.Q,
                    DangerLevel = 3,
                    SDataName = "XenZhaoComboTarget",
                });
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "XinZhao",
                     SpellMenuName = "Audacious Charge",
                     Slot = SpellSlot.E,
                     DangerLevel = 4,
                     SDataName = "XenZhaoSweep",
                 });
            CCList.Add(
                  new KurisuLib
                  {
                      HeroName = "XinZhao",
                      SpellMenuName = "Crescent Sweep",
                      Slot = SpellSlot.R,
                      Type = Skilltype.Circle,
                      DangerLevel = 5,
                      SDataName = "XenZhaoParry",
                  });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Yasuo",
                    SpellMenuName = "yasuoq2",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "yasuoq2",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Yasuo",
                    SpellMenuName = "yasuoq3w",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "yasuoq3w",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Yasuo",
                    SpellMenuName = "yasuoq",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "yasuoq",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Zac",
                    SpellMenuName = "Stretching Strike",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "ZacQ",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Zac",
                    SpellMenuName = "Lets Bounce!",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "ZacR",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Zed",
                    SpellMenuName = "Death Mark",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "ZedUlt",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Ziggs",
                    SpellMenuName = "Satchel Charge",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "ZiggsW",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Zyra",
                    SpellMenuName = "Grasping Roots",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "ZyraGraspingRoots",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Zyra",
                    SpellMenuName = "Stranglethorns",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "ZyraBrambleZone",
                });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Taric",
                    SpellMenuName = "Dazzle",
                    Slot = SpellSlot.E,
                    SDataName = "Dazzle",
                    DangerLevel = 5
                });
            CCList.Add(
                 new KurisuLib
                 {
                     HeroName = "Yoric",
                     SpellMenuName = "Omen of Pestilence",
                     Slot = SpellSlot.W,
                     DangerLevel = 3,
                     SDataName = "YorickDecayed",
                 });
            CCList.Add(
                new KurisuLib
                {
                    HeroName = "Yasuo",
                    SpellMenuName = "Steel Tempest (3)",
                    Slot = SpellSlot.W,
                    DangerLevel = 3,
                    SDataName = "YasuoQ3",
                });
            #endregion

            #region SList
            SList.Add(
                new KurisuLib
                {
                    HeroName = "Fiddlesticks",
                    SpellMenuName = "Dark Wind",
                    Slot = SpellSlot.E,
                    DangerLevel = 3,
                    SDataName = "FiddlesticksDarkWind",
                });
            SList.Add(
                new KurisuLib
                {
                    HeroName = "Blitzcrank",
                    SpellMenuName = "Static Field",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "StaticField",

                });
            SList.Add(
                new KurisuLib
                {
                    HeroName = "Chogath",
                    SpellMenuName = "Feral Scream",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Cone,
                    DangerLevel = 3,
                    SDataName = "FeralScream",

                });
            SList.Add(
                new KurisuLib
                {
                    HeroName = "Malzahar",
                    SpellMenuName = "Call of the Void",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "AlZaharCalloftheVoid",
                });
            SList.Add(
                new KurisuLib
                {
                    HeroName = "Talon",
                    SpellMenuName = "Cutthroat",
                    Type = Skilltype.Unknown,
                    Slot = SpellSlot.E,
                    DangerLevel = 3,
                    SDataName = "TalonCutthroat",
                });
            SList.Add(
                 new KurisuLib
                 {
                     HeroName = "Garen",
                     SpellMenuName = "Decisive Strike",
                     Slot = SpellSlot.Q,
                     DangerLevel = 3,
                     SDataName = "GarenQ",
                 });
            SList.Add(
                  new KurisuLib
                  {
                      HeroName = "Viktor",
                      SpellMenuName = "Chaos Storm",
                      Type = Skilltype.Circle,
                      Slot = SpellSlot.R,
                      DangerLevel = 3,
                      SDataName = "ViktorChaosStorm",
                  });
            SList.Add(
                   new KurisuLib
                   {
                       HeroName = "Soraka",
                       SpellMenuName = "Equinox",
                       Slot = SpellSlot.E,
                       Type = Skilltype.Circle,
                       DangerLevel = 2,
                       SDataName = "SorakaE",
                   });
            #endregion

            #region GDList

            #endregion
        }
    }
}

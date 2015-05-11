#region LICENSE

// Copyright 2014-2015 Support
// DamageBoostDatabase.cs is part of Support.
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
// Filename: Support/Support/DamageBoostDatabase.cs
// Created:  26/11/2014
// Date:     20/01/2015/11:20
// Author:   h3h3

#endregion

namespace AutoSharpporting.Util
{
    #region

    using System.Collections.Generic;

    #endregion

    internal class DamageBoostSpell
    {
        public string Champion { get; set; }
        public string Spell { get; set; }
        public string Slot { get; set; }
        public int Priority { get; set; }
    }

    internal class DamageBoostDatabase
    {
        public static readonly List<DamageBoostSpell> Spells = new List<DamageBoostSpell>
        {
            new DamageBoostSpell { Champion = "Ashe", Spell = "Volley", Slot = "W", Priority = 1 },
            new DamageBoostSpell { Champion = "Caitlyn", Spell = "CaitlynPiltoverPeacemaker", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "Caitlyn", Spell = "CaitlynAceintheHole", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Corki", Spell = "PhosphorusBomb", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "Corki", Spell = "GGun", Slot = "E", Priority = 1 },
            new DamageBoostSpell { Champion = "Corki", Spell = "MissileBarrage", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Draven", Spell = "DravenSpinning", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "Draven", Spell = "DravenDoubleShot", Slot = "E", Priority = 1 },
            new DamageBoostSpell { Champion = "Draven", Spell = "DravenRCast", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Ezreal", Spell = "EzrealMysticShot", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "Ezreal", Spell = "EzrealTrueshotBarrage", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Graves", Spell = "GravesClusterShot", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "Graves", Spell = "GravesChargeShot", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Jinx", Spell = "JinxW", Slot = "W", Priority = 1 },
            new DamageBoostSpell { Champion = "Jinx", Spell = "JinxRWrapper", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "KogMaw", Spell = "KogMawLivingArtillery", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Lucian", Spell = "LucianQ", Slot = "Q", Priority = 2 },
            new DamageBoostSpell { Champion = "Lucian", Spell = "LucianW", Slot = "W", Priority = 1 },
            new DamageBoostSpell { Champion = "Lucian", Spell = "LucianR", Slot = "R", Priority = 3 },
            new DamageBoostSpell
            {
                Champion = "MissFortune",
                Spell = "MissFortuneRicochetShot",
                Slot = "Q",
                Priority = 2
            },
            new DamageBoostSpell { Champion = "MissFortune", Spell = "MissFortuneBulletTime", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Quinn", Spell = "QuinnQ", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "Quinn", Spell = "QuinnE", Slot = "E", Priority = 2 },
            new DamageBoostSpell { Champion = "Quinn", Spell = "QuinnR", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Sivir", Spell = "SivirQ", Slot = "Q", Priority = 2 },
            new DamageBoostSpell { Champion = "Twitch", Spell = "Expunge", Slot = "E", Priority = 3 },
            new DamageBoostSpell { Champion = "Twitch", Spell = "FullAutomatic", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Urgot", Spell = "UrgotHeatseekingMissile", Slot = "Q", Priority = 2 },
            new DamageBoostSpell { Champion = "Urgot", Spell = "UrgotPlasmaGrenade", Slot = "E", Priority = 1 },
            new DamageBoostSpell { Champion = "Varus", Spell = "VarusQ", Slot = "Q", Priority = 3 },
            new DamageBoostSpell { Champion = "Varus", Spell = "VarusE", Slot = "E", Priority = 1 },
            new DamageBoostSpell { Champion = "Vayne", Spell = "VayneTumble", Slot = "Q", Priority = 2 },
            new DamageBoostSpell { Champion = "Vayne", Spell = "VayneCondemn", Slot = "E", Priority = 1 },
            new DamageBoostSpell { Champion = "Vayne", Spell = "VayneInquisition", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "LeeSin", Spell = "BlindMonkQOne", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "LeeSin", Spell = "BlindMonkRKick", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Nasus", Spell = "NasusQ", Slot = "Q", Priority = 2 },
            new DamageBoostSpell { Champion = "Nocturne", Spell = "NocturneParanoia", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Shaco", Spell = "TwoShivPoison", Slot = "E", Priority = 2 },
            new DamageBoostSpell { Champion = "Trundle", Spell = "TrundleTrollSmash", Slot = "Q", Priority = 2 },
            new DamageBoostSpell { Champion = "Vi", Spell = "ViQ", Slot = "Q", Priority = 2 },
            new DamageBoostSpell { Champion = "Vi", Spell = "ViE", Slot = "E", Priority = 1 },
            new DamageBoostSpell { Champion = "Vi", Spell = "ViR", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "XinZhao", Spell = "XenZhaoComboTarget", Slot = "Q", Priority = 2 },
            new DamageBoostSpell { Champion = "XinZhao", Spell = "XenZhaoParry", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Khazix", Spell = "KhazixQ", Slot = "Q", Priority = 2 },
            new DamageBoostSpell { Champion = "Khazix", Spell = "KhazixW", Slot = "W", Priority = 2 },
            new DamageBoostSpell { Champion = "MasterYi", Spell = "AlphaStrike", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "MasterYi", Spell = "WujuStyle", Slot = "E", Priority = 1 },
            new DamageBoostSpell { Champion = "Talon", Spell = "TalonNoxianDiplomacy", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "Talon", Spell = "TalonShadowAssault", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Pantheon", Spell = "PantheonQ", Slot = "Q", Priority = 2 },
            new DamageBoostSpell { Champion = "Yasuo", Spell = "YasuoQW", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "Yasuo", Spell = "yasuoq2w", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "Yasuo", Spell = "yasuoq3w", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "Yasuo", Spell = "YasuoRKnockUpComboW", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Zed", Spell = "ZedShuriken", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "Zed", Spell = "ZedPBAOEDummy", Slot = "E", Priority = 1 },
            new DamageBoostSpell { Champion = "Zed", Spell = "zedult", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Aatrox", Spell = "AatroxW", Slot = "W", Priority = 2 },
            new DamageBoostSpell { Champion = "Darius", Spell = "DariusExecute", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Gangplank", Spell = "Parley", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "Garen", Spell = "GarenQ", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "Garen", Spell = "GarenE", Slot = "E", Priority = 2 },
            new DamageBoostSpell { Champion = "Jayce", Spell = "JayceToTheSkies", Slot = "R", Priority = 2 },
            new DamageBoostSpell { Champion = "Jayce", Spell = "jayceshockblast", Slot = "Q", Priority = 2 },
            new DamageBoostSpell { Champion = "Renekton", Spell = "RenektonCleave", Slot = "Q", Priority = 2 },
            new DamageBoostSpell { Champion = "Renekton", Spell = "RenektonPreExecute", Slot = "W", Priority = 2 },
            new DamageBoostSpell { Champion = "Renekton", Spell = "RenektonSliceAndDice", Slot = "E", Priority = 2 },
            new DamageBoostSpell { Champion = "Rengar", Spell = "RengarQ", Slot = "Q", Priority = 2 },
            new DamageBoostSpell { Champion = "Rengar", Spell = "RengarE", Slot = "E", Priority = 1 },
            new DamageBoostSpell { Champion = "Rengar", Spell = "RengarR", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Riven", Spell = "RivenFengShuiEngine", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "MonkeyKing", Spell = "MonkeyKingDoubleAttack", Slot = "Q", Priority = 1 },
            new DamageBoostSpell { Champion = "MonkeyKing", Spell = "MonkeyKingNimbus", Slot = "E", Priority = 2 },
            new DamageBoostSpell { Champion = "MonkeyKing", Spell = "MonkeyKingSpinToWin", Slot = "R", Priority = 3 },
            new DamageBoostSpell { Champion = "Kalista", Spell = "KalistaMysticShot", Slot = "Q", Priority = 2 },
            new DamageBoostSpell { Champion = "Kalista", Spell = "KalistaExpungeWrapper", Slot = "E", Priority = 3 }
        };
    }
}
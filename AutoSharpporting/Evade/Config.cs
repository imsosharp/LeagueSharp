#region LICENSE

// Copyright 2014-2015 Support
// Config.cs is part of Support.
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
// Filename: Support/Support/Config.cs
// Created:  05/10/2014
// Date:     20/01/2015/11:20
// Author:   h3h3

#endregion

namespace Support.Evade
{
    internal static class Config
    {
        public const bool PrintSpellData = false;
        public const bool TestOnAllies = false;
        public const int SkillShotsExtraRadius = 9;
        public const int SkillShotsExtraRange = 20;
        public const int GridSize = 10;
        public const int ExtraEvadeDistance = 15;
        public const int DiagonalEvadePointsCount = 7;
        public const int DiagonalEvadePointsStep = 20;
        public const int CrossingTimeOffset = 250;
        public const int EvadingFirstTimeOffset = 250;
        public const int EvadingSecondTimeOffset = 0;
        public const int EvadingRouteChangeTimeOffset = 250;
        public const int EvadePointChangeInterval = 300;
        public static int LastEvadePointChangeT = 0;
    }
}
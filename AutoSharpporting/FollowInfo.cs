/*
 * Hi Adam, I was lazy, cheers for this ly bro :D
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Support
{
    internal class FollowInfo
    {
            // Considered AFK after 20 seconds
        private const int AFK_TIMESPAN = 20 * 1000;
        // Holds previous known position of player
        public Vector3 PreviousPosition;
        // Holds previous time position was recorded
        public int TimeStamp;

        public FollowInfo(Vector3 previousPosition, int timeStamp)
        {
            PreviousPosition = previousPosition;
            TimeStamp = timeStamp;
        }

        public void Update(Vector3 position)
        {
            // If position is not same as previous known,
            // the player has moved
            if (PreviousPosition != position)
            {
                // Update the position and time to now
                PreviousPosition = position;
                TimeStamp = Environment.TickCount;
            }
        }

        public bool IsAfk()
        {
            // If we haven't seen them move for over timespan they are
            // considered AFK
            return (Environment.TickCount > TimeStamp + AFK_TIMESPAN);
        }
    }
}

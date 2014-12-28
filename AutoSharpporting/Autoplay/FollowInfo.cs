using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Support
{
    class FollowInfo
    {
        // Considered AFK after 30 seconds
        private const int AFK_TIMESPAN = 30 * 1000;
        // Holds previous known position of player
        public static Vector3 PreviousPosition;
        // Holds previous time position was recorded
        public static int TimeStamp;

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

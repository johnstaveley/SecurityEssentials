using System;
using System.Threading;

namespace SecurityEssentials.Acceptance.Tests.Utility
{
    public static class Repeater
    {
        public static void DoOrTimeout(Func<bool> condition, TimeSpan timeout)
        {
            DoOrTimeout(condition, timeout, new TimeSpan(0, 0, 5));
        }

        public static void DoOrTimeout(Func<bool> condition, TimeSpan timeout, TimeSpan interval)
        {
            var stopTrying = false;
            var started = DateTime.UtcNow;

            while (!stopTrying)
            {
                Thread.Sleep(interval);

                if (condition())
                    stopTrying = true;

                if (DateTime.UtcNow.Subtract(started).TotalMilliseconds > timeout.TotalMilliseconds)
                    stopTrying = true;
            }
        }
    }
}
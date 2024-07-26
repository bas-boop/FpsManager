using System;
using System.Threading;

namespace FpsManager
{
    /// <summary>
    /// Simple monitor checker that sets the main screen to the desired hz if there is a second monitor connected or not.
    /// </summary>
    internal static class Program
    {
        private const int DETECT_FOR_SCREEN_MINUTES = 3;
        private const int DETECT_SPEED = 10000;
        private const int READ_TIME = 5000;

        private static void Main()
        {
            HzData hz = Saving.GetRefreshRates();
            int monitorCount = Screen.GetMonitorCount();
            
            if (monitorCount > 1)
            {
                Screen.SetRefreshRate(hz.StandaloneHz);
                SendMessage($"Second monitor detected. Setting refresh rate to {hz.StandaloneHz}Hz.");
                return;
            }
            
            Console.WriteLine($"No second monitor detected. Setting refresh rate to preferred hz: {hz.LowestHz}.");
            Screen.SetRefreshRate(hz.LowestHz);
            DateTime endTime = DateTime.Now.AddMinutes(DETECT_FOR_SCREEN_MINUTES);

            while (DateTime.Now < endTime)
            {
                monitorCount = Screen.GetMonitorCount();

                if (monitorCount > 1)
                {
                    Screen.SetRefreshRate(hz.LowestHz);
                    SendMessage($"Second monitor detected within waiting period. Setting refresh rate to {hz.LowestHz}Hz.");
                    return;
                }

                Thread.Sleep(DETECT_SPEED);
            }
        }

        private static void SendMessage(string msg)
        {
            Console.Write(msg);
            Thread.Sleep(READ_TIME);
        }
    }
}
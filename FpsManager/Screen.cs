using System;
using System.Runtime.InteropServices;

namespace FpsManager
{
    /// <summary>
    /// Class for screen related functions. Monitor count and setting the refresh rate.
    /// </summary>
    public static class Screen
    {
        public static int GetMonitorCount()
        {
            int monitorCount = 0;
            OperatingSystemData.DisplayDevice d = new();
            d.cb = Marshal.SizeOf(d);

            for (uint id = 0; OperatingSystemData.EnumDisplayDevices(null, id, ref d, 0); id++)
            {
                if ((d.StateFlags & 0x1) != 0) 
                    monitorCount++;
            }

            return monitorCount;
        }

        public static void SetRefreshRate(int hz)
        {
            OperatingSystemData.DevMode dm = new();
            dm.dmSize = (short)Marshal.SizeOf(dm);

            if (!OperatingSystemData.EnumDisplaySettings(null, OperatingSystemData.ENUM_CURRENT_SETTINGS, ref dm))
                return;
            
            dm.dmDisplayFrequency = hz;
            int iRet = OperatingSystemData.ChangeDisplaySettingsEx(null, ref dm, IntPtr.Zero,
                OperatingSystemData.CDS_UPDATEREGISTRY, IntPtr.Zero);

            if (iRet == OperatingSystemData.DISP_CHANGE_SUCCESSFUL)
                return;
            
            Console.WriteLine("Failed to change display settings. Given refresh rate is not in you machine settings.");
            RetryHandler.Retry();
        }

        public static bool AreCurrentSettingsUsable(int l, int s)
        {
            OperatingSystemData.DevMode dm = new();
            dm.dmSize = (short)Marshal.SizeOf(dm);

            if (!OperatingSystemData.EnumDisplaySettings(null, OperatingSystemData.ENUM_CURRENT_SETTINGS, ref dm))
                return false;
            
            dm.dmDisplayFrequency = l;
            int iRet1 = OperatingSystemData.ChangeDisplaySettingsEx(null, ref dm, IntPtr.Zero,
                OperatingSystemData.CDS_UPDATEREGISTRY, IntPtr.Zero);
            
            dm.dmDisplayFrequency = s;
            int iRet2 = OperatingSystemData.ChangeDisplaySettingsEx(null, ref dm, IntPtr.Zero,
                OperatingSystemData.CDS_UPDATEREGISTRY, IntPtr.Zero);

            return IsSuccess(iRet1)
                   && IsSuccess(iRet2);
        }

        private static bool IsSuccess(int iRet) => iRet != OperatingSystemData.DISP_CHANGE_SUCCESSFUL;
    }
}
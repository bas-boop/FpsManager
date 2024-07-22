using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace FpsManager
{
    /// <summary>
    /// A simple monitor checker that sets the main screen to the desired hz if there is a second monitor connected or not.
    /// </summary>
    internal static class Program
    {
        private const int ENUM_CURRENT_SETTINGS = -1;
        private const int CDS_UPDATEREGISTRY = 0x01;
        private const int DISP_CHANGE_SUCCESSFUL = 0;

        [StructLayout(LayoutKind.Sequential)]
        public struct DevMode
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public short dmLogPixels;
            public short dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DisplayDevice lpDisplayDevice, uint dwFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DisplayDevice
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public int StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DevMode lpDevMode);

        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DevMode lpDevMode, IntPtr hwnd, uint dwflags, IntPtr lParam);

        private static void Main(string[] args)
        {
            int monitorCount = GetMonitorCount();

            if (monitorCount > 1)
            {
                Console.WriteLine("Second monitor detected. Setting refresh rate to 60Hz.");
                SetRefreshRate(60);
                return;
            }

            Console.WriteLine("No second monitor detected. Setting refresh rate to dynamic.");
            SetRefreshRate(120);
            DateTime endTime = DateTime.Now.AddMinutes(3);

            while (DateTime.Now < endTime)
            {
                monitorCount = GetMonitorCount();

                if (monitorCount > 1)
                {
                    Console.WriteLine("Second monitor detected within waiting period. Setting refresh rate to 60Hz.");
                    SetRefreshRate(60);
                    return;
                }

                Thread.Sleep(10000);
            }
        }

        private static int GetMonitorCount()
        {
            int monitorCount = 0;
            DisplayDevice d = new DisplayDevice();
            d.cb = Marshal.SizeOf(d);

            for (uint id = 0; EnumDisplayDevices(null, id, ref d, 0); id++)
            {
                if ((d.StateFlags & 0x1) != 0) 
                    monitorCount++;
            }

            return monitorCount;
        }

        private static void SetRefreshRate(int hz)
        {
            DevMode dm = new DevMode();
            dm.dmSize = (short)Marshal.SizeOf(dm);

            if (!EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm))
                return;
            
            dm.dmDisplayFrequency = hz;
            int iRet = ChangeDisplaySettingsEx(null, ref dm, IntPtr.Zero,
                CDS_UPDATEREGISTRY, IntPtr.Zero);

            if (iRet != DISP_CHANGE_SUCCESSFUL) 
                Console.WriteLine("Failed to change display settings.");
        }
    }
}
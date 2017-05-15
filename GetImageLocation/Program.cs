using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace TestingImageSearchDLL
{
    class Program
    {
        const int ENUM_CURRENT_SETTINGS = -1;

        const int ENUM_REGISTRY_SETTINGS = -2;

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean EnumDisplaySettings(
            [param: MarshalAs(UnmanagedType.LPTStr)]
            string lpszDeviceName,
            [param: MarshalAs(UnmanagedType.U4)]
            int iModeNum,
            [In, Out]
            ref DEVMODE lpDevMode);

        [Flags()]
        public enum ChangeDisplaySettingsFlags : uint
        {
            CDS_NONE = 0,
            CDS_UPDATEREGISTRY = 0x00000001,
            CDS_TEST = 0x00000002,
            CDS_FULLSCREEN = 0x00000004,
            CDS_GLOBAL = 0x00000008,
            CDS_SET_PRIMARY = 0x00000010,
            CDS_VIDEOPARAMETERS = 0x00000020,
            CDS_ENABLE_UNSAFE_MODES = 0x00000100,
            CDS_DISABLE_UNSAFE_MODES = 0x00000200,
            CDS_RESET = 0x40000000,
            CDS_RESET_EX = 0x20000000,
            CDS_NORESET = 0x10000000
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.I8)]
        public static extern long ChangeDisplaySettings(ref DEVMODE devMode, ChangeDisplaySettingsFlags flags);


        [StructLayout(LayoutKind.Sequential)]
        public struct POINTL
        {
            [MarshalAs(UnmanagedType.I4)]
            public int x;
            [MarshalAs(UnmanagedType.I4)]
            public int y;
        }

        [StructLayout(LayoutKind.Sequential,
    CharSet = CharSet.Ansi)]
        public struct DEVMODE
        {
            // You can define the following constant
            // but OUTSIDE the structure because you know
            // that size and layout of the structure
            // is very important
            // CCHDEVICENAME = 32 = 0x50
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            // In addition you can define the last character array
            // as following:
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            //public Char[] dmDeviceName;

            // After the 32-bytes array
            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmSpecVersion;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmDriverVersion;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmSize;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmDriverExtra;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmFields;

            public POINTL dmPosition;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayOrientation;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFixedOutput;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmColor;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmDuplex;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmYResolution;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmTTOption;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmCollate;

            // CCHDEVICENAME = 32 = 0x50
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            // Also can be defined as
            //[MarshalAs(UnmanagedType.ByValArray,
            //    SizeConst = 32, ArraySubType = UnmanagedType.U1)]
            //public Byte[] dmFormName;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmLogPixels;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmBitsPerPel;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPelsWidth;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPelsHeight;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFlags;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFrequency;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmICMMethod;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmICMIntent;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmMediaType;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDitherType;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmReserved1;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmReserved2;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPanningWidth;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPanningHeight;
        }

        [DllImport("ImageSearchDLL.dll")]
        static extern string ImageSearchFile([MarshalAs(UnmanagedType.I4)]int aLeft, 
            [MarshalAs(UnmanagedType.I4)]int aTop, 
            [MarshalAs(UnmanagedType.I4)]int aRight,
            [MarshalAs(UnmanagedType.I4)]int aBottom,
            [MarshalAs(UnmanagedType.LPStr)]string FromImageFile,
            [MarshalAs(UnmanagedType.LPStr)]string aImageFile,
            [MarshalAs(UnmanagedType.I4)]int debug,
            [MarshalAs(UnmanagedType.I4)]int Variation);

        [DllImport("ImageSearchDLL.dll")]
        static extern string ImageSearch([MarshalAs(UnmanagedType.I4)]int aLeft,
            [MarshalAs(UnmanagedType.I4)]int aTop,
            [MarshalAs(UnmanagedType.I4)]int aRight,
            [MarshalAs(UnmanagedType.I4)]int aBottom,
            [MarshalAs(UnmanagedType.LPStr)]string aImageFile,
            [MarshalAs(UnmanagedType.I4)]int debug,
            [MarshalAs(UnmanagedType.I4)]int Variation);

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
            byte[] newline = Encoding.ASCII.GetBytes(Environment.NewLine);
            fs.Write(newline, 0, newline.Length);
        }

        public static int ChangeMyDisplaySettings(int width, int height, int bitCount)
        {
            DEVMODE originalMode = new DEVMODE();
            originalMode.dmSize =
                (ushort)Marshal.SizeOf(originalMode);

            // Retrieving current settings
            // to edit them
            EnumDisplaySettings(null,
                ENUM_CURRENT_SETTINGS,
                ref originalMode);

            // Making a copy of the current settings
            // to allow reseting to the original mode
            DEVMODE newMode = originalMode;

            // Changing the settings
            newMode.dmPelsWidth = (uint)width;
            newMode.dmPelsHeight = (uint)height;
            newMode.dmBitsPerPel = (uint)bitCount;

            // Capturing the operation result
            int result =
               (int)ChangeDisplaySettings(ref newMode, 0);
               
            if (result == 0)
            {
                Console.WriteLine("Call for: "+ width + "x" + height + "color: " + bitCount + "\n");

                // Inspecting the new mode
                GetCurrentSettings();

                Console.WriteLine();
                        
              //  ChangeDisplaySettings((int)width, (int)height, 0);
            }
            else if (result == 1)
                Console.WriteLine("Mode not supported.");
            else if (result == 2)
                Console.WriteLine("Restart required.");
            else
                Console.WriteLine("Failed. Error code = {0}", result);
                Console.WriteLine("Call was: " + width + "x" + height + "color: " + bitCount + "\n");

            return 0;
        }

        public static void GetCurrentSettings()
        {
            DEVMODE mode = new DEVMODE();
            mode.dmSize = (ushort)Marshal.SizeOf(mode);

            if (EnumDisplaySettings(null,
                ENUM_CURRENT_SETTINGS,
                ref mode) == true) // Succeeded
            {
                Console.WriteLine("Current Mode:\n\t" +
                    "{0} by {1}, " +
                    "{2} bit, " +
                    "{3} degrees, " +
                    "{4} hertz",
                    mode.dmPelsWidth,
                    mode.dmPelsHeight,
                    mode.dmBitsPerPel,
                    mode.dmDisplayOrientation * 90,
                    mode.dmDisplayFrequency);
            }
        }

        static int Main(string[] args)
        {
            int iDebug = 0; // 0 means not debug
                            // 1 means Full debug
                            // 2 means Save ScreenShot only if ImageSearch not found

            int aVariation = 0;

            string result;

            if (args.Length < 1)  // Warning : Index was out of the bounds of the array
            {
                Console.Write("Usage:\n");
                Console.Write("GetImageLocation.exe ImagePathToFindOnScreen Flag Variance [ImagePathToUseAsScreen]\n");
                Console.Write("Flag:\n");
                Console.Write("\t 0: Short output, no debug\n");
                Console.Write("\t 1: Full debug\n");
                Console.Write("\t 2: Recognation with screenshot of not found\n");
                Console.Write("\t 3: Get current sreen resolution\n");
                Console.Write("\t 4: Set screen resolution\n");
                Console.Write("\t 5: Means 2\n");
                Console.Write("Variance:\n");
                Console.Write("\t 0: Strict color comparaison\n");
                Console.Write("\t n: (Max: 255) Color derive acceptance\n");
                Console.Write("Ex: GetImageLocation.exe c:\\bob.l.eponge\\MonIcone.bmp 1\n");
                return 1;
            }

            if (args.Length < 3)  // Warning : Index was out of the bounds of the array
            {
                iDebug = 0;
                aVariation = 0;
            } else 
            {
                iDebug = 0;
                if (Convert.ToInt32(args[1]) == 1)
                {
                    iDebug = Convert.ToInt32(args[1]);
                }
                if (Convert.ToInt32(args[1]) == 2)
                {
                    iDebug = Convert.ToInt32(args[1]);
                }
                aVariation = 0;
                if (Convert.ToInt32(args[2]) < 256)
                {
                    aVariation = Convert.ToInt32(args[2]);
                }
            }

            string path = "log\\GetImageLocation.log";
 
            if (!File.Exists("ImageSearchDLL.dll"))
            {
                FileStream fs = File.Open(path, FileMode.Append);
                Console.WriteLine("Could not find \\ImageSearchDLL.dll.");
                AddText(fs, "Could not find \\ImageSearchDLL.dll.");
                fs.Close();
                return 1;
            }

            if (iDebug == 1)
            {
                FileStream fs = File.Open(path, FileMode.Append);
                AddText(fs, "GetImageLocation .NET is in the place....\n");
                fs.Close();
            }

            //   ChangeMyDisplaySettings(1024, 720, 32);
           
            int ResolutionHeight = Screen.PrimaryScreen.Bounds.Height;
            int ResolutionWidth = Screen.PrimaryScreen.Bounds.Width;

            if (File.Exists(args[0]))
            {
                if (iDebug == 1)
                {
                    // Use this version to capture the full extended desktop (i.e. multiple screens)
                    // Note it don't work using WindowsTask with non session user :(
                    //
                    Bitmap screenshot = new Bitmap(SystemInformation.VirtualScreen.Width,
                                                   SystemInformation.VirtualScreen.Height,
                                                   PixelFormat.Format32bppArgb);
                    Graphics screenGraph = Graphics.FromImage(screenshot);
                    screenGraph.CopyFromScreen(SystemInformation.VirtualScreen.X,
                                               SystemInformation.VirtualScreen.Y,
                                               0,
                                               0,
                                               SystemInformation.VirtualScreen.Size,
                                               CopyPixelOperation.SourceCopy);

                    screenshot.Save("log\\Last-Screenshot.png", System.Drawing.Imaging.ImageFormat.Png);
                }

                if (args.Length > 3)
                {
                    result = ImageSearchFile(0, 0, ResolutionWidth, ResolutionHeight, args[2], args[0], iDebug, aVariation);
                }
                else
                {
                    result = ImageSearch(0, 0, ResolutionWidth,ResolutionHeight, args[0], iDebug, aVariation);
                }

                Console.Write(result);
                if (iDebug == 1)
                {
                    FileStream fs = File.Open(path, FileMode.Append);
                    AddText(fs, result + "\n");
                    fs.Close();
                }
                return 0;
            }
            else
            {
                Console.WriteLine("File: " + args[0] + " doesn't exist.");
                if (iDebug == 1)
                {
                    FileStream fs = File.Open(path, FileMode.Append);
                    AddText(fs, "File: " + args[0] + " doesn't exist.");
                    fs.Close();
                }
                return 1;
            }
        }
    }
}
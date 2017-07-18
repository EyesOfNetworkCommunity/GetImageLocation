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
        [DllImport("..\\lib\\ImageSearchDLL.dll")]
        static extern string ImageSearchFile([MarshalAs(UnmanagedType.I4)]int aLeft, 
            [MarshalAs(UnmanagedType.I4)]int aTop, 
            [MarshalAs(UnmanagedType.I4)]int aRight,
            [MarshalAs(UnmanagedType.I4)]int aBottom,
            [MarshalAs(UnmanagedType.LPStr)]string FromImageFile,
            [MarshalAs(UnmanagedType.LPStr)]string aImageFile,
            [MarshalAs(UnmanagedType.I4)]int debug,
            [MarshalAs(UnmanagedType.I4)]int Variation,
            [MarshalAs(UnmanagedType.I4)]int GreenDrift);

        [DllImport("..\\lib\\ImageSearchDLL.dll")]
        static extern string ImageSearch([MarshalAs(UnmanagedType.I4)]int aLeft,
            [MarshalAs(UnmanagedType.I4)]int aTop,
            [MarshalAs(UnmanagedType.I4)]int aRight,
            [MarshalAs(UnmanagedType.I4)]int aBottom,
            [MarshalAs(UnmanagedType.LPStr)]string aImageFile,
            [MarshalAs(UnmanagedType.I4)]int debug,
            [MarshalAs(UnmanagedType.I4)]int Variation,
            [MarshalAs(UnmanagedType.I4)]int GreenDrift);

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
            byte[] newline = Encoding.ASCII.GetBytes(Environment.NewLine);
            fs.Write(newline, 0, newline.Length);
        }

        static int Main(string[] args)
        {
            int iDebug = 0; // 0 means not debug
                            // 1 means Full debug
                            // 2 means Save ScreenShot only if ImageSearch not found

            int aVariation = 0;
            int GreenDrift = 0;

            string result;

            if (args.Length < 1)  // Warning : Index was out of the bounds of the array
            {
                Console.Write("Usage:\n");
                Console.Write("GetImageLocation.exe ImagePathToFindOnScreen Flag Variance GreenDrift\n");
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
                Console.Write("GreenDrift:\n");
                Console.Write("\t 0: Strict color comparaison\n");
                Console.Write("\t 1: Color derive to the green (Black and White fixing)\n");
                Console.Write("Ex: GetImageLocation.exe c:\\bob.l.eponge\\MonIcone.bmp 1\n");
                return 1;
            }

            if (args.Length < 2)  // Warning : Index was out of the bounds of the array
            {
                iDebug = 0;
                aVariation = 0;
                GreenDrift = 0;
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
                GreenDrift = 0;
                if (Convert.ToInt32(args[3]) == 1)
                {
                    GreenDrift = Convert.ToInt32(args[3]);
                }
            }

            string path = "..\\log\\GetImageLocation.log";
 
            if (!File.Exists("..\\lib\\ImageSearchDLL.dll"))
            {
                FileStream fs = File.Open(path, FileMode.Append);
                Console.WriteLine("Could not find ..\\lib\\ImageSearchDLL.dll.");
                AddText(fs, "Could not find ..\\lib\\ImageSearchDLL.dll.");
                fs.Close();
                return 1;
            }

            if (iDebug == 1)
            {
                FileStream fs = File.Open(path, FileMode.Append);
                AddText(fs, "GetImageLocation .NET is in the place....\n");
                fs.Close();
            }
           
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

             /*   if (args.Length > 4)
                {
                    result = ImageSearchFile(0, 0, ResolutionWidth, ResolutionHeight, args[2], args[0], iDebug, aVariation, GreenDrift);
                }
                else
                {*/
                    result = ImageSearch(0, 0, ResolutionWidth,ResolutionHeight, args[0], iDebug, aVariation, GreenDrift);
             /*   } */

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
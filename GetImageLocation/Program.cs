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
        [DllImport("C:\\eon\\APX\\EON4APPS\\ImageSearchDLL.dll")]
        static extern string ImageSearch([MarshalAs(UnmanagedType.I4)]int aLeft, 
            [MarshalAs(UnmanagedType.I4)]int aTop, 
            [MarshalAs(UnmanagedType.I4)]int aRight,
            [MarshalAs(UnmanagedType.I4)]int aBottom,
            [MarshalAs(UnmanagedType.LPStr)]string aImageFile,
            [MarshalAs(UnmanagedType.I4)]int debug);

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

            if (args.Length < 1)  // Warning : Index was out of the bounds of the array
            {
                Console.Write("Usage:\n");
                Console.Write("GetImageLocation.exe ImagePathToFindOnScreen debugflag\n");
                Console.Write("Ex: GetImageLocation.exe c:\\bob.l.eponge\\MonIcone.bmp 1\n");
                return 1;
            }

            if (args.Length < 2)  // Warning : Index was out of the bounds of the array
            {
                iDebug = 0;
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
            }

            string path = @"C:\eon\APX\EON4APPS\log\GetImageLocation.log";
            FileStream fs = File.Open(path,FileMode.Append);

            if (iDebug == 1)
            {
                AddText(fs, "GetImageLocation .NET is in the place....\n");
            }    
            
            if (!File.Exists("C:\\eon\\APX\\EON4APPS\\ImageSearchDLL.dll"))
            {
                Console.WriteLine("Could not find C:\\eon\\APX\\EON4APPS\\ImageSearchDLL.dll.");
                AddText(fs, "Could not find C:\\eon\\APX\\EON4APPS\\ImageSearchDLL.dll.");
                return 1;
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

                    screenshot.Save("C:\\eon\\APX\\EON4APPS\\Last-Screenshot.png", System.Drawing.Imaging.ImageFormat.Png);
                }
                string result = ImageSearch(0, 0, ResolutionWidth,ResolutionHeight, args[0], iDebug);

                Console.Write(result);
                if (iDebug == 1)
                {
                    AddText(fs, result + "\n");
                }
                return 0;
            }
            else
            {
                Console.WriteLine("File: " + args[0] + " doesn't exist.");
                if (iDebug == 1)
                {
                    AddText(fs, "File: " + args[0] + " doesn't exist.");
                }
                return 1;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using IronOcr;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace captcha
{

    
    class Program
    {
        public class ScreenCapture
        {
            [DllImport("user32.dll")]
            private static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern IntPtr GetDesktopWindow();

            [StructLayout(LayoutKind.Sequential)]
            private struct Rect
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }

            [DllImport("user32.dll")]
            private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

            public static Image CaptureDesktop()
            {
                return CaptureWindow(GetDesktopWindow());
            }

            public static Bitmap CaptureActiveWindow()
            {
                return CaptureWindow(GetForegroundWindow());
            }

            public static Bitmap CaptureWindow(IntPtr handle)
            {
                var rect = new Rect();
                GetWindowRect(handle, ref rect);
                var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                var result = new Bitmap(bounds.Width, bounds.Height);

                using (var graphics = Graphics.FromImage(result))
                {
                    graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                }

                return result;
            }
        }
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint Msg);

        private const uint SW_RESTORE = 0x09;
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetFocus(HandleRef hWnd);


        static void Main(string[] args)
        {
            Process process = new Process();
            string program = "CCLauncher_Client";
            process = Process.GetProcessesByName(program)[0];
            ShowWindow(process.MainWindowHandle, SW_RESTORE);
            SetForegroundWindow(process.MainWindowHandle);
            MoveWindow(process.MainWindowHandle, 0, 0, 700, 400, true);

            //var image = ScreenCapture.CaptureActiveWindow();
            //image.Save(@"C:\Users\bjoer\Desktop\test3.png", ImageFormat.Png);



            //var Ocr = new IronOcr.AdvancedOcr();

            //var testDocument = @"C:\Users\bjoer\Desktop\test3.png";
            //var Results = Ocr.Read(testDocument);
            //string hallo = Results.ToString();
            //Console.WriteLine(hallo);
            //string hallo2 = new string(hallo.Where(Char.IsDigit).ToArray());
            //Console.WriteLine(hallo2);
            //Console.Read();


            //string captchaZahl = "1230";
            //string matrixZahl = "7894561230";
            //int j = 0;


            //for (int i = 0; i < matrixZahl.Length; i++)
            //{
            //    if (matrixZahl[i] == captchaZahl[j])
            //    {
            //        switch (i)
            //        {
            //            case 0:
            //                Console.WriteLine("oben links");
            //                break;
            //            case 1:
            //                Console.WriteLine("oben mitte");
            //                break;
            //            case 2:
            //                Console.WriteLine("oben rechts");
            //                break;
            //            case 3:
            //                Console.WriteLine("mitte links");
            //                break;
            //            case 4:
            //                Console.WriteLine("mitte mitte");
            //                break;
            //            case 5:
            //                Console.WriteLine("mitte rechts");
            //                break;
            //            case 6:
            //                Console.WriteLine("unten links");
            //                break;
            //            case 7:
            //                Console.WriteLine("unten mitte");
            //                break;
            //            case 8:
            //                Console.WriteLine("unten rechts");
            //                break;
            //            case 9:
            //                Console.WriteLine("ganz unten ");
            //                break;

            //        }
            //        if(j==3)
            //        {
            //            break;
            //        }
            //        j = j + 1;
            //    }
            //}


            //Console.Read();


        }
    }
}

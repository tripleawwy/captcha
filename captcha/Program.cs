﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        //struct für GetWindowRect
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner  
            public int Top;         // y position of upper-left corner  
            public int Right;       // x position of lower-right corner  
            public int Bottom;      // y position of lower-right corner  
        }

        //user32 API import
        [DllImport("user32", EntryPoint = "mouse_event")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);
        //members
        private const uint MouseMove = 0x0001;
        private const uint MouseEventLeftDown = 0x0002;
        private const uint MouseEventLeftUp = 0x0004;
        private const uint ARSCH = 0x8000;
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("User32.Dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT 
        {
            public int X;
            public int Y;
        }

            public struct CaptchaWindow
        {
            public int Width;
            public int Height;

            public POINT MatrixZahlObenLinks;
            public POINT MatrixZahlObenMitte;
            public POINT MatrixZahlObenRechts;
            public POINT MatrixZahlMitteLinks;
            public POINT MatrixZahlMitteMitte;
            public POINT MatrixZahlMitteRechts;
            public POINT MatrixZahlUntenLinks;
            public POINT MatrixZahlUntenMitte;
            public POINT MatrixZahlUntenRechts;
            public POINT MatrixZahlGanzUnten;

            public POINT Absenden;
        }

        




        static void Main(string[] args)
        {
            int primaryScreenWidth = 1600;
            int primaryScreenHeight = 900;

            Process process = new Process();
            var activeWindow = new RECT();
            string program = "CCLauncher_Client";
            string hallo, hallo2;

            process = Process.GetProcessesByName(program)[0];

            //do
            //{

            ShowWindow(process.MainWindowHandle, SW_RESTORE);
            Thread.Sleep(200);
            SetForegroundWindow(process.MainWindowHandle);

            //GetWindowRect(process.MainWindowHandle, out activeWindow);

            //MoveWindow(process.MainWindowHandle, 100, 100, activeWindow.Right - activeWindow.Left, activeWindow.Bottom - activeWindow.Top, true);
            MoveWindow(process.MainWindowHandle, 0, 0, 700, 390, true);

            //GetWindowRect(process.MainWindowHandle, out activeWindow);

            Thread.Sleep(50);

            var image = ScreenCapture.CaptureActiveWindow();
            Thread.Sleep(100);
            //ToDo: bild beschneiden ???
            image.Save(@"C:\Users\bmarten\Desktop\test4.bmp", ImageFormat.Bmp);
            Thread.Sleep(100);

            var Ocr = new IronOcr.AdvancedOcr();

                var testDocument = @"C:\Users\bmarten\Desktop\test4.bmp";
                var Results = Ocr.Read(testDocument);
                hallo = Results.Text;
                Console.WriteLine(hallo);
                hallo2 = new string(hallo.Where(Char.IsDigit).ToArray());
                Console.WriteLine(hallo2);
            Console.Read();

            //    } while (hallo2.Length < 13);  //ToDo: Endlosschleife verhindern falls OCR nicht alle Zahlen erkennt

            //    string captchaZahl = hallo2.Substring(0, 4); Console.WriteLine(captchaZahl);
            //    string matrixZahl = hallo2.Substring(4, 10); Console.WriteLine(matrixZahl);
            //    int j = 0;

            //    Thread.Sleep(5000);
            //    GetWindowRect(GetForegroundWindow(), out activeWindow);

            //    CaptchaWindow fixedCaptcha = new CaptchaWindow();
            //    fixedCaptcha.Width = 374;
            //    fixedCaptcha.Height = 431;
            //    fixedCaptcha.MatrixZahlObenLinks.X = 154; fixedCaptcha.MatrixZahlObenLinks.Y = 204;
            //    fixedCaptcha.MatrixZahlObenMitte.X = 181; fixedCaptcha.MatrixZahlObenMitte.Y = 204;
            //    fixedCaptcha.MatrixZahlObenRechts.X = 208; fixedCaptcha.MatrixZahlObenRechts.Y = 204;
            //    fixedCaptcha.MatrixZahlMitteLinks.X = 154; fixedCaptcha.MatrixZahlMitteLinks.Y = 233;
            //    fixedCaptcha.MatrixZahlMitteMitte.X = 181; fixedCaptcha.MatrixZahlMitteMitte.Y = 233;
            //    fixedCaptcha.MatrixZahlMitteRechts.X = 208; fixedCaptcha.MatrixZahlMitteRechts.Y = 233;
            //    fixedCaptcha.MatrixZahlUntenLinks.X = 154; fixedCaptcha.MatrixZahlUntenLinks.Y = 262;
            //    fixedCaptcha.MatrixZahlUntenMitte.X = 181; fixedCaptcha.MatrixZahlUntenMitte.Y = 262;
            //    fixedCaptcha.MatrixZahlUntenRechts.X = 208; fixedCaptcha.MatrixZahlUntenRechts.Y = 262;
            //    fixedCaptcha.MatrixZahlGanzUnten.X = 154; fixedCaptcha.MatrixZahlGanzUnten.Y = 291;
            //    fixedCaptcha.Absenden.X = 315; fixedCaptcha.Absenden.Y = 400;


            //    CaptchaWindow curCaptcha = new CaptchaWindow();
            //    //Verhältnis in Pixeln zu (0,0)
            //    curCaptcha.Width = activeWindow.Right - activeWindow.Left;
            //    curCaptcha.Height = activeWindow.Bottom - activeWindow.Top;
            //    //curCaptcha.MatrixZahlObenLinks.X = (int)Math.Round((double)activeWindow.Left + (((double)fixedCaptcha.MatrixZahlObenLinks.X * (double)curCaptcha.Width) / (double)fixedCaptcha.Width), 0);
            //    //curCaptcha.MatrixZahlObenLinks.Y = (int)Math.Round((double)activeWindow.Top + (((double)fixedCaptcha.MatrixZahlObenLinks.Y * (double)curCaptcha.Height) / (double)fixedCaptcha.Height), 0);
            //    curCaptcha.MatrixZahlObenLinks.X = activeWindow.Left + ((fixedCaptcha.MatrixZahlObenLinks.X * curCaptcha.Width) / fixedCaptcha.Width);
            //    curCaptcha.MatrixZahlObenLinks.Y = activeWindow.Top + ((fixedCaptcha.MatrixZahlObenLinks.Y * curCaptcha.Height) / fixedCaptcha.Height);
            //    curCaptcha.MatrixZahlObenMitte.X = activeWindow.Left + ((fixedCaptcha.MatrixZahlObenMitte.X * curCaptcha.Width) / fixedCaptcha.Width);
            //    curCaptcha.MatrixZahlObenMitte.Y = activeWindow.Top + ((fixedCaptcha.MatrixZahlObenMitte.Y * curCaptcha.Height) / fixedCaptcha.Height);
            //    curCaptcha.MatrixZahlObenRechts.X = activeWindow.Left + ((fixedCaptcha.MatrixZahlObenRechts.X * curCaptcha.Width) / fixedCaptcha.Width);
            //    curCaptcha.MatrixZahlObenRechts.Y = activeWindow.Top + ((fixedCaptcha.MatrixZahlObenRechts.Y * curCaptcha.Height) / fixedCaptcha.Height);
            //    curCaptcha.MatrixZahlMitteLinks.X = activeWindow.Left + ((fixedCaptcha.MatrixZahlMitteLinks.X * curCaptcha.Width) / fixedCaptcha.Width);
            //    curCaptcha.MatrixZahlMitteLinks.Y = activeWindow.Top + ((fixedCaptcha.MatrixZahlMitteLinks.Y * curCaptcha.Height) / fixedCaptcha.Height);
            //    curCaptcha.MatrixZahlMitteMitte.X = activeWindow.Left + ((fixedCaptcha.MatrixZahlMitteMitte.X * curCaptcha.Width) / fixedCaptcha.Width);
            //    curCaptcha.MatrixZahlMitteMitte.Y = activeWindow.Top + ((fixedCaptcha.MatrixZahlMitteMitte.Y * curCaptcha.Height) / fixedCaptcha.Height);
            //    curCaptcha.MatrixZahlMitteRechts.X = activeWindow.Left + ((fixedCaptcha.MatrixZahlMitteRechts.X * curCaptcha.Width) / fixedCaptcha.Width);
            //    curCaptcha.MatrixZahlMitteRechts.Y = activeWindow.Top + ((fixedCaptcha.MatrixZahlMitteRechts.Y * curCaptcha.Height) / fixedCaptcha.Height);
            //    curCaptcha.MatrixZahlUntenLinks.X = activeWindow.Left + ((fixedCaptcha.MatrixZahlUntenLinks.X * curCaptcha.Width) / fixedCaptcha.Width);
            //    curCaptcha.MatrixZahlUntenLinks.Y = activeWindow.Top + ((fixedCaptcha.MatrixZahlUntenLinks.Y * curCaptcha.Height) / fixedCaptcha.Height);
            //    curCaptcha.MatrixZahlUntenMitte.X = activeWindow.Left + ((fixedCaptcha.MatrixZahlUntenMitte.X * curCaptcha.Width) / fixedCaptcha.Width);
            //    curCaptcha.MatrixZahlUntenMitte.Y = activeWindow.Top + ((fixedCaptcha.MatrixZahlUntenMitte.Y * curCaptcha.Height) / fixedCaptcha.Height);
            //    curCaptcha.MatrixZahlUntenRechts.X = activeWindow.Left + ((fixedCaptcha.MatrixZahlUntenRechts.X * curCaptcha.Width) / fixedCaptcha.Width);
            //    curCaptcha.MatrixZahlUntenRechts.Y = activeWindow.Top + ((fixedCaptcha.MatrixZahlUntenRechts.Y * curCaptcha.Height) / fixedCaptcha.Height);
            //    curCaptcha.MatrixZahlGanzUnten.X = activeWindow.Left + ((fixedCaptcha.MatrixZahlGanzUnten.X * curCaptcha.Width) / fixedCaptcha.Width);
            //    curCaptcha.MatrixZahlGanzUnten.Y = activeWindow.Top + ((fixedCaptcha.MatrixZahlGanzUnten.Y * curCaptcha.Height) / fixedCaptcha.Height);
            //    curCaptcha.Absenden.X = activeWindow.Left + ((fixedCaptcha.Absenden.X * curCaptcha.Width) / fixedCaptcha.Width);
            //    curCaptcha.Absenden.Y = activeWindow.Top + ((fixedCaptcha.Absenden.Y * curCaptcha.Height) / fixedCaptcha.Height);



            //    //Verhältnis für MouseMove(ABSOLUTE)
            //    //curCaptcha.MatrixZahlObenLinks.X = (ushort)Math.Round((double)curCaptcha.MatrixZahlObenLinks.X * 65535 / 1600, 0);
            //    //curCaptcha.MatrixZahlObenLinks.Y = (ushort)Math.Round((double)curCaptcha.MatrixZahlObenLinks.Y * 65535 / 900, 0);
            //    curCaptcha.MatrixZahlObenLinks.X = (curCaptcha.MatrixZahlObenLinks.X * 65535) / primaryScreenWidth;
            //    curCaptcha.MatrixZahlObenLinks.Y = (curCaptcha.MatrixZahlObenLinks.Y * 65535) / primaryScreenHeight;
            //    curCaptcha.MatrixZahlObenMitte.X = (curCaptcha.MatrixZahlObenMitte.X * 65535) / primaryScreenWidth;
            //    curCaptcha.MatrixZahlObenMitte.Y = (curCaptcha.MatrixZahlObenMitte.Y * 65535) / primaryScreenHeight;
            //    curCaptcha.MatrixZahlObenRechts.X = (curCaptcha.MatrixZahlObenRechts.X * 65535) / primaryScreenWidth;
            //    curCaptcha.MatrixZahlObenRechts.Y = (curCaptcha.MatrixZahlObenRechts.Y * 65535) / primaryScreenHeight;
            //    curCaptcha.MatrixZahlMitteLinks.X = (curCaptcha.MatrixZahlMitteLinks.X * 65535) / primaryScreenWidth;
            //    curCaptcha.MatrixZahlMitteLinks.Y = (curCaptcha.MatrixZahlMitteLinks.Y * 65535) / primaryScreenHeight;
            //    curCaptcha.MatrixZahlMitteMitte.X = (curCaptcha.MatrixZahlMitteMitte.X * 65535) / primaryScreenWidth;
            //    curCaptcha.MatrixZahlMitteMitte.Y = (curCaptcha.MatrixZahlMitteMitte.Y * 65535) / primaryScreenHeight;
            //    curCaptcha.MatrixZahlMitteRechts.X = (curCaptcha.MatrixZahlMitteRechts.X * 65535) / primaryScreenWidth;
            //    curCaptcha.MatrixZahlMitteRechts.Y = (curCaptcha.MatrixZahlMitteRechts.Y * 65535) / primaryScreenHeight;
            //    curCaptcha.MatrixZahlUntenLinks.X = (curCaptcha.MatrixZahlUntenLinks.X * 65535) / primaryScreenWidth;
            //    curCaptcha.MatrixZahlUntenLinks.Y = (curCaptcha.MatrixZahlUntenLinks.Y * 65535) / primaryScreenHeight;
            //    curCaptcha.MatrixZahlUntenMitte.X = (curCaptcha.MatrixZahlUntenMitte.X * 65535) / primaryScreenWidth;
            //    curCaptcha.MatrixZahlUntenMitte.Y = (curCaptcha.MatrixZahlUntenMitte.Y * 65535) / primaryScreenHeight;
            //    curCaptcha.MatrixZahlUntenRechts.X = (curCaptcha.MatrixZahlUntenRechts.X * 65535) / primaryScreenWidth;
            //    curCaptcha.MatrixZahlUntenRechts.Y = (curCaptcha.MatrixZahlUntenRechts.Y * 65535) / primaryScreenHeight;
            //    curCaptcha.MatrixZahlGanzUnten.X = (curCaptcha.MatrixZahlGanzUnten.X * 65535) / primaryScreenWidth;
            //    curCaptcha.MatrixZahlGanzUnten.Y = (curCaptcha.MatrixZahlGanzUnten.Y * 65535) / primaryScreenHeight;
            //    curCaptcha.Absenden.X = (curCaptcha.MatrixZahlObenLinks.X * 65535) / primaryScreenWidth;
            //    curCaptcha.Absenden.Y = (curCaptcha.MatrixZahlObenLinks.Y * 65535) / primaryScreenHeight;


            //    //event call
            //    Thread.Sleep(1000);
            //    //mouse_event((ARSCH|MouseMove), 0x08000, 0x8000, 0, new System.IntPtr());
            //    Thread.Sleep(1000);

            //    //mouse_event(MouseMove, -100*1600/900, -100*1.58, 0, new System.IntPtr());
            //    //Thread.Sleep(1000);
            //    mouse_event((ARSCH | MouseMove), curCaptcha.MatrixZahlObenLinks.X, curCaptcha.MatrixZahlObenLinks.Y, 0, new System.IntPtr());
            //    //mouse_event(0x0001, 0x8000, 0x8000, 0, new System.IntPtr());


            //    //mouse_event(MouseEventLeftDown, 0, 0, 0, new System.IntPtr());
            //    //mouse_event(MouseEventLeftUp, 0, 0,0, new System.IntPtr());



            //    for (int i = 0; i < matrixZahl.Length; i++)
            //    {
            //        if (matrixZahl[i] == captchaZahl[j])
            //        {
            //            switch (i)
            //            {
            //                case 0:
            //                    mouse_event((ARSCH | MouseMove), curCaptcha.MatrixZahlObenLinks.X, curCaptcha.MatrixZahlObenLinks.Y, 0, new System.IntPtr());
            //                    Console.WriteLine("oben links");
            //                    break;
            //                case 1:
            //                    mouse_event((ARSCH | MouseMove), curCaptcha.MatrixZahlObenMitte.X, curCaptcha.MatrixZahlObenMitte.Y, 0, new System.IntPtr());
            //                    Console.WriteLine("oben mitte");
            //                    break;
            //                case 2:
            //                    mouse_event((ARSCH | MouseMove), curCaptcha.MatrixZahlObenRechts.X, curCaptcha.MatrixZahlObenRechts.Y, 0, new System.IntPtr());
            //                    Console.WriteLine("oben rechts");
            //                    break;
            //                case 3:
            //                    mouse_event((ARSCH | MouseMove), curCaptcha.MatrixZahlMitteLinks.X, curCaptcha.MatrixZahlMitteLinks.Y, 0, new System.IntPtr());
            //                    Console.WriteLine("mitte links");
            //                    break;
            //                case 4:
            //                    mouse_event((ARSCH | MouseMove), curCaptcha.MatrixZahlMitteMitte.X, curCaptcha.MatrixZahlMitteMitte.Y, 0, new System.IntPtr());
            //                    Console.WriteLine("mitte mitte");
            //                    break;
            //                case 5:
            //                    mouse_event((ARSCH | MouseMove), curCaptcha.MatrixZahlMitteRechts.X, curCaptcha.MatrixZahlMitteRechts.Y, 0, new System.IntPtr());
            //                    Console.WriteLine("mitte rechts");
            //                    break;
            //                case 6:
            //                    mouse_event((ARSCH | MouseMove), curCaptcha.MatrixZahlUntenLinks.X, curCaptcha.MatrixZahlUntenLinks.Y, 0, new System.IntPtr());
            //                    Console.WriteLine("unten links");
            //                    break;
            //                case 7:
            //                    mouse_event((ARSCH | MouseMove), curCaptcha.MatrixZahlUntenMitte.X, curCaptcha.MatrixZahlUntenMitte.Y, 0, new System.IntPtr());
            //                    Console.WriteLine("unten mitte");
            //                    break;
            //                case 8:
            //                    mouse_event((ARSCH | MouseMove), curCaptcha.MatrixZahlUntenRechts.X, curCaptcha.MatrixZahlUntenRechts.Y, 0, new System.IntPtr());
            //                    Console.WriteLine("unten rechts");

            //                    break;
            //                case 9:
            //                    mouse_event((ARSCH | MouseMove), curCaptcha.MatrixZahlGanzUnten.X, curCaptcha.MatrixZahlGanzUnten.Y, 0, new System.IntPtr());
            //                    Console.WriteLine("ganz unten ");
            //                    break;

            //            }
            //            Thread.Sleep(200);
            //            mouse_event(MouseEventLeftDown, 0, 0, 0, new System.IntPtr());
            //            mouse_event(MouseEventLeftUp, 0, 0, 0, new System.IntPtr());
            //            Thread.Sleep(600);

            //            if (j == 3)
            //            {
            //                break;
            //            }
            //            j = j + 1;
            //            i = -1;

            //        }
            //    }


            //    mouse_event((ARSCH | MouseMove), curCaptcha.Absenden.X, curCaptcha.Absenden.Y, 0, new System.IntPtr());
            //    Thread.Sleep(200);
            //    //mouse_event(MouseEventLeftDown, 0, 0, 0, new System.IntPtr());
            //    //mouse_event(MouseEventLeftUp, 0, 0,0, new System.IntPtr());
            //    Thread.Sleep(600);

            //    Console.Read();


        }
    }
}

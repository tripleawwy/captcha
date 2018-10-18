using System;

//for StringBuilder
using System.Text;

//for DllImport
using System.Runtime.InteropServices;


namespace captcha
{
    internal class User32DLL
    {

        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        internal static extern int ShowWindow(IntPtr hWnd, uint Msg);
        //showwindow
        internal const uint SW_RESTORE = 0x09;
        internal const uint SW_FORCEMINIMIZE = 0x11;
        internal const uint SW_SHOWMAXIMIZED = 0x03;
        internal const uint SW_SHOWNORMAL = 0x01;
        internal const uint SW_SHOWDEFAULT = 0x10;
        internal const uint SW_MINIMIZE = 0x06;
        internal const uint SW_HIDE = 0x00;
        internal const uint SW_SHOWMINIMIZED = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr SetFocus(HandleRef hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        //struct für GetWindowRect
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            internal int Left;        // x position of upper-left corner  
            internal int Top;         // y position of upper-left corner  
            internal int Right;       // x position of lower-right corner  
            internal int Bottom;      // y position of lower-right corner  
        }

        [DllImport("user32", EntryPoint = "mouse_event")]
        internal static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);
        //members
        internal const uint MouseMove = 0x0001;
        internal const uint MouseEventLeftDown = 0x0002;
        internal const uint MouseEventLeftUp = 0x0004;
        internal const uint ARSCH = 0x8000;

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern IntPtr GetActiveWindow();

        [DllImport("User32.Dll")]
        internal static extern long SetCursorPos(int x, int y);

        [DllImport("user32.dll", EntryPoint = "GetWindowInfo", SetLastError = true)]
        internal static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO windowInfo);
        /* WINDOWINFO */
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWINFO
        {
            internal uint cbSize;
            internal RECT rcWindow;
            internal RECT rcClient;
            internal uint dwStyle;
            internal uint dwExStyle;
            internal uint dwWindowStatus;
            internal uint cxWindowBorders;
            internal uint cyWindowBorders;
            internal ushort atomWindowType;
            internal ushort wCreatorVersion;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            internal int X;
            internal int Y;
        }

        internal struct CaptchaWindow
        {
            internal int Width;
            internal int Height;

            internal POINT MatrixZahlObenLinks;
            internal POINT MatrixZahlObenMitte;
            internal POINT MatrixZahlObenRechts;
            internal POINT MatrixZahlMitteLinks;
            internal POINT MatrixZahlMitteMitte;
            internal POINT MatrixZahlMitteRechts;
            internal POINT MatrixZahlUntenLinks;
            internal POINT MatrixZahlUntenMitte;
            internal POINT MatrixZahlUntenRechts;
            internal POINT MatrixZahlGanzUnten;

            internal POINT Absenden;
        }

        internal static string GetActiveWindowTitle()
        {

            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
    }
}

using System;
//for StringBuilder
using System.Text;
//for DllImport
using System.Runtime.InteropServices;


#region preamble
//windows-datatypes : https://docs.microsoft.com/en-us/windows/desktop/WinProg/windows-data-types
//BYTE = unsigned 8 bit
//WORD = unsigned 16 bit
//DWORD = UINT = unsigned 32 bit  //sometimes also signed 32 bit
//QWORD = unsigned 64 bit
//SHORT = signed 16 bit
//LONG = INT = signed 32 bit
//LONGLONG = signed 64 bit
//HWND = void * = pointer to any type
// pointer as parameter use REF with value-datatypes (structs), reference-datatypes (objects, strings) can be used directly
//                      prefer using REF to an existing object, instead of OUT, 
//                      due to uncertainty whether the object is or isn't build in the external method
// pointer as returned value use IntPtr / UIntPtr class (can handle 32/64 bit pointer)
// c# string is reference-type and CharSet is Unicode
#endregion


namespace captcha
{
    internal static class User32DLL
    {
        #region DLL_Imports

        #region GetActiveWindow_done
        /*      
         *      Syntax
         *      HWND GetActiveWindow();
         *      
         *      Return Value
         *      Type: HWND - a handle to the active window or null
         */
        /// <summary>
        /// GetActiveWindow() from winuser.h / user32.dll
        /// </summary>
        /// <returns> a handle to the active window or null </returns>
        [DllImport("user32.dll", EntryPoint = "GetActiveWindow")]
        internal static extern IntPtr GetActiveWindow();
        #endregion

        #region GetDesktopWindow_done
        /*      
         *      Syntax
         *      HWND GetDesktopWindow();
         *      
         *      Return Value
         *      Type: HWND - a handle to the desktop window
         */
        /// <summary>
        /// GetDesktopWindow() from winuser.h / user32.dll
        /// </summary>
        /// <returns> a handle to the desktop window </returns>
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        internal static extern IntPtr GetDesktopWindow();
        #endregion

        #region GetForegroundWindow_done
        /*      
         *      Syntax
         *      HWND GetForegroundWindow();
         *      
         *      Return Value
         *      Type: HWND - a handle to the foreground window or null
         */
        /// <summary>
        /// GetForegroundWindow() from winuser.h / user32.dll
        /// </summary>
        /// <returns> a handle to the foreground window or null </returns>
        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        internal static extern IntPtr GetForegroundWindow();
        #endregion

        #region GetWindowInfo_done
        /*      
         *      Syntax
         *      BOOL GetWindowInfo( HWND hWnd, PWINDOWINFO pwi );
         *      
         *      Parameters
         *      Type: HWND - A handle to the window whose information is to be retrieved
         *      Type: PWINDOWINFO - A pointer to a WINDOWINFO structure to receive the information.
         *                          Note that you must set the cbSize member to sizeof(WINDOWINFO) before calling this function
         *                          
         *      Return Value
         *      Type: BOOL - failed returns zero, succeeded returns nonzero
         */
        /// <summary>
        /// GetWindowInfo() from winuser.h / user32.dll
        /// 
        /// Note that you must set the cbSize member to sizeof(WINDOWINFO) before calling this function
        /// 
        /// </summary>
        /// <param name="hWnd"> a handle to the window whose information is to be retrieved </param>
        /// <param name="pwi"> a pointer to a WINDOWINFO structure to receive the information </param>
        /// <returns> returns zero if fails and nonzero if succeeds </returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowInfo", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowInfo(IntPtr hWnd, ref WINDOWINFO pwi);
        #endregion

        #region GetWindowRect_done
        /*      
         *      Syntax
         *      BOOL GetWindowRect( HWND hWnd, LPRECT lpRect );
         *      
         *      Parameters
         *      Type: HWND - A handle to the window
         *      Type: LPRECT - A pointer to a RECT structure that receives the screen coordinates of the upper-left and lower-right corners of the window.
         *      
         *      Return Value
         *      Type: BOOL - failed returns zero, succeeded returns nonzero
         */
        /// <summary>
        /// GetWindowRect() from winuser.h / user32.dll
        /// </summary>
        /// <param name="hWnd">a handle to the window</param>
        /// <param name="lpRect">a pointer to a RECT structure that receives the screen coordinates </param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowRect", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        #endregion

        #region GetWindowText_done
        /*
         *      there is an internal, an A and a W version of GetWindowText() function
         *      
         *      in case of problems with string interpretation try different CharSet
         *      
         *      Syntax
         *      int GetWindowTextA( HWND hWnd, LPSTR lpString, int nMaxCount );  <- Ansi
         *      int GetWindowTextW( HWND hWnd, LPWSTR lpString, int nMaxCount ); <- Unicode (should be choosen by CharSet.Unicode + ExactSpelling = false)
         *      int InternalGetWindowText( HWND hWnd, LPWSTR pString, int cchMaxCount );
         *      
         *      Parameters
         *      Type: HWND - A handle to the window or control containing the text.
         *      Type: LPTSTR - The buffer that will receive the text. If the string is as long or longer than the buffer, 
         *                     the string is truncated and terminated with a null character.
         *      Type: int - The maximum number of characters to copy to the buffer, including the null character. 
         *                  If the text exceeds this limit, it is truncated.
         *      
         *      Return Value
         *      Type: int - If the function succeeds, the return value is the length, in characters, of the copied string, 
         *                  not including the terminating null character. If the window has no title bar or text, 
         *                  if the title bar is empty, or if the window or control handle is invalid, the return value is zero.
         *      
         */
        /// <summary>
        /// GetWindowText() from winuser.h / user32.dll
        /// </summary>
        /// <param name="hWnd"> A handle to the window or control containing the text. </param>
        /// <param name="lpString"> The buffer that will receive the text. </param>
        /// <param name="nMaxCount"> The maximum number of characters to copy to the buffer, including the null character. </param>
        /// <returns> return value is the length, empty title or invalid handle returns zero </returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Unicode, ExactSpelling = false, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        #endregion

        #region mouse_event_done
        /*
         *      Syntax
         *      void mouse_event( DWORD dwFlags, DWORD dx, DWORD dy, DWORD dwData, ULONG_PTR dwExtraInfo );
         *      
         *      Parameters
         *      dwFlags - Type: DWORD - Controls various aspects of mouse motion and button clicking. This parameter can be certain combinations of the following values.
         *      dx - Type: DWORD - The mouse's absolute position along the x-axis or its amount of motion since the last mouse event was generated, depending on the setting of MOUSEEVENTF_ABSOLUTE. Absolute data is specified as the mouse's actual x-coordinate; relative data is specified as the number of mickeys moved. A mickey is the amount that a mouse has to move for it to report that it has moved.
         *      dy - Type: DWORD - The mouse's absolute position along the y-axis or its amount of motion since the last mouse event was generated, depending on the setting of MOUSEEVENTF_ABSOLUTE. Absolute data is specified as the mouse's actual y-coordinate; relative data is specified as the number of mickeys moved.
         *      dwData - Type: DWORD - If dwFlags contains MOUSEEVENTF_WHEEL, then dwData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated forward, away from the user; a negative value indicates that the wheel was rotated backward, toward the user. One wheel click is defined as WHEEL_DELTA, which is 120.
         *                             If dwFlags contains MOUSEEVENTF_HWHEEL, then dwData specifies the amount of wheel movement. A positive value indicates that the wheel was tilted to the right; a negative value indicates that the wheel was tilted to the left.
         *                             If dwFlags contains MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP, then dwData specifies which X buttons were pressed or released. This value may be any combination of the following flags.
         *                             If dwFlags is not MOUSEEVENTF_WHEEL, MOUSEEVENTF_XDOWN, or MOUSEEVENTF_XUP, then dwData should be zero.
         *      dwExtraInfo - Type: ULONG_PTR - An additional value associated with the mouse event. An application calls GetMessageExtraInfo to obtain this extra information.
         *      
         */
        /// <summary>
        ///  mouse_event() from winuser.h / user32.dll
        /// </summary>
        /// <param name="dwFlags"> Controls various aspects of mouse motion and button clicking. </param>
        /// <param name="dx"> The mouse's position along the x-axis </param>
        /// <param name="dy"> The mouse's position along the y-axis </param>
        /// <param name="dwData"> contains data about the action happend </param>
        /// <param name="dwExtraInfo"> An additional value associated with GetMessageExtraInfo </param>
        [DllImport("user32.dll", EntryPoint = "mouse_event")]
        internal static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);
        // DWORD for dx and dy should be converted to unsigned 32 bit, 
        //       but since mouse can move relatively to last position it must be signed 32 bit
        #endregion

        #region MoveWindow_done
        /*      
         *      Syntax
         *      BOOL MoveWindow( HWND hWnd, int X, int Y, int nWidth, int nHeight, BOOL bRepaint );
         *      
         *      Parameters
         *      hWnd - Type: HWND - A handle to the window.
         *      X - Type: int - The new position of the left side of the window.
         *      Y - Type: int - The new position of the top of the window.
         *      nWidth - Type: int - The new width of the window.
         *      nHeight - Type: int - The new height of the window.
         *      bRepaint - Type: BOOL - Indicates whether the window is to be repainted. If this parameter is TRUE, 
         *                              the window receives a message. If the parameter is FALSE, no repainting of any kind occurs. 
         *                              This applies to the client area, the nonclient area (including the title bar and scroll bars), 
         *                              and any part of the parent window uncovered as a result of moving a child window.
         *      
         *      Return Value
         *      Type: BOOL - If the function succeeds, the return value is nonzero.
         *      
         */
        /// <summary>
        /// MoveWindow() from winuser.h / user32.dll
        /// </summary>
        /// <param name="hWnd"> A handle to the window </param>
        /// <param name="X"> The new position of the left side of the window </param>
        /// <param name="Y"> The new position of the top of the window </param>
        /// <param name="nWidth"> The new width of the window </param>
        /// <param name="nHeight"> The new height of the window </param>
        /// <param name="bRepaint"> Indicates whether the window is to be repainted </param>
        /// <returns> true if the function succeeds </returns>
        [DllImport("user32.dll", EntryPoint = "MoveWindow", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        #endregion

        #region SetCursorPos_done 
        /*      
         *      Syntax
         *      BOOL SetCursorPos( int X, int Y );
         *      
         *      Parameters
         *      X - Type: int - The new x-coordinate of the cursor, in screen coordinates.
         *      Y - Type: int - The new y-coordinate of the cursor, in screen coordinates.
         *      
         *      Return Value
         *      Type: BOOL - Returns nonzero if successful or zero otherwise. 
         *      
         */
        /// <summary>
        /// SetCursorPos() from winuser.h / user32.dll
        /// </summary>
        /// <param name="X"> The new x-coordinate of the cursor, in screen coordinates </param>
        /// <param name="Y"> The new y-coordinate of the cursor, in screen coordinates </param>
        /// <returns> true if successful </returns>
        [DllImport("User32.Dll", EntryPoint = "SetCursorPos", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetCursorPos(int X, int Y);
        #endregion

        #region SetForegroundWindow_done
        /*
         *      Syntax
         *      BOOL SetForegroundWindow( HWND hWnd );
         *      
         *      Parameters
         *      hWnd - Type: HWND - A handle to the window that should be activated and brought to the foreground.
         *      
         *      Return Value
         *      Type: Type: BOOL - If the window was brought to the foreground, the return value is nonzero.
         *                         If the window was not brought to the foreground, the return value is zero.
         *                         
         */
        /// <summary>
        /// SetForegroundWindow() from winuser.h / user32.dll
        /// </summary>
        /// <param name="hWnd"> A handle to the window that should be activated and brought to the foreground </param>
        /// <returns> true if the window was brought to the foreground </returns>
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion

        #region SetFocus_done
        /*
         *      Syntax
         *      HWND SetFocus( HWND hWnd );
         *      
         *      Parameters
         *      hWnd - Type: HWND - A handle to the window that will receive the keyboard input. 
         *                          If this parameter is NULL, keystrokes are ignored.
         *      
         *      Return Value
         *      Type: HWND - If the function succeeds, the return value is the handle to the window that previously 
         *                   had the keyboard focus. If the hWnd parameter is invalid or the window is not attached 
         *                   to the calling thread's message queue, the return value is NULL.
         *                   
         */
        /// <summary>
        /// SetFocus() from winuser.h / user32.dll
        /// </summary>
        /// <param name="hWnd"> A handle to the window that will receive the keyboard input </param>
        /// <returns> if succeds, returns a handle to the window that previously had the keyboard focus or NULL </returns>
        [DllImport("user32.dll", EntryPoint = "SetFocus", SetLastError = true)]
        internal static extern IntPtr SetFocus(IntPtr hWnd);
        #endregion

        #region ShowWindow_done
        /*
         *      Syntax
         *      BOOL ShowWindow( HWND hWnd, int nCmdShow );
         *      
         *      Parameters
         *      hWnd - Type: HWND - A handle to the window.
         *      nCmdShow - Type: int - Controls how the window is to be shown. 
         *                             This parameter is ignored the first time an application calls ShowWindow, 
         *                             if the program that launched the application provides a STARTUPINFO structure. 
         *                             Otherwise, the first time ShowWindow is called, the value should be the value 
         *                             obtained by the WinMain function in its nCmdShow parameter. 
         *                             In subsequent calls, this parameter can be one of the following values.
         *                             
         *      Return Value
         *      Type: Type: BOOL - If the window was previously visible, the return value is nonzero.
         *                         If the window was previously hidden, the return value is zero.
         *      
         */
        /// <summary>
        /// ShowWindow() from winuser.h / user32.dll
        /// </summary>
        /// <param name="hWnd"> A handle to the window </param>
        /// <param name="nCmdShow"> Controls how the window is to be shown </param>
        /// <returns> true if previously visible, false if previously hidden </returns>
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        #endregion

        #endregion

        #region Help_Structures

        #region WINDOWINFO_done
        /*      
         *      a struct for GetWindowInfo()
         *      
         *      typedef struct WINDOWINFO
         *      {
         *          DWORD cbSize;           //the size of the structure, in bytes
         *          RECT rcWindow;          //the coordinates of the window
         *          RECT rcClient;          //the coordinates of the client area
         *          DWORD dwStyle;          //the window styles
         *          DWORD dwExStyle;        //the extended window styles
         *          DWORD dwWindowStatus;   //The window status. If this member is WS_ACTIVECAPTION (0x0001), the window is active. Otherwise, this member is zero.
         *          UINT cxWindowBorders;   //The width of the window border, in pixels.
         *          UINT cyWindowBorders;   //The height of the window border, in pixels.
         *          ATOM atomWindowType;    //The window class atom (see RegisterClass).
         *          WORD wCreatorVersion;   //The Windows version of the application that created the window.
         *      }
         */
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWINFO
        {
            internal uint cbSize;
            [MarshalAs(UnmanagedType.LPStruct)]
            internal RECT rcWindow;
            [MarshalAs(UnmanagedType.LPStruct)]
            internal RECT rcClient;
            internal uint dwStyle;
            internal uint dwExStyle;
            internal uint dwWindowStatus;
            internal uint cxWindowBorders;
            internal uint cyWindowBorders;
            //internal ATOM atomWindowType;       // external register class (16 bit - WORD)
            [MarshalAs(UnmanagedType.U2)]
            internal ushort atomWindowType;
            internal ushort wCreatorVersion;
        }
        #endregion

        #region RECT_done
        /*      
         *      a struct for GetWindowInfo().WINDOWINFO
         *      a struct for GetWindowRect()
         *      
         *      typedef struct RECT
         *      {
         *          LONG left;      // x position of upper-left corner 
         *          LONG top;       // y position of upper-left corner 
         *          LONG right;     // x position of lower-right corner 
         *          LONG bottom;    // y position of lower-right corner
         *      }
         */
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
        }
        #endregion

        #region mouse_event_dwFlags_done
        /*
         *      Flags for dwFlags in mouse_event()
         */
        internal struct DwFlags
        {
            internal static uint MOUSEEVENTF_ABSOLUTE = 0x8000;     //The dx and dy parameters contain normalized absolute coordinates.If not set, those parameters contain relative data: the change in position since the last reported position.This flag can be set, or not set, regardless of what kind of mouse or mouse-like device, if any, is connected to the system. For further information about relative mouse motion, see the following Remarks section.
            internal static uint MOUSEEVENTF_LEFTDOWN = 0x0002;     //The left button is down.
            internal static uint MOUSEEVENTF_LEFTUP = 0x0004;       //The left button is up.
            internal static uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;   //The middle button is down.
            internal static uint MOUSEEVENTF_MIDDLEUP = 0x0040;     //The middle button is up.
            internal static uint MOUSEEVENTF_MOVE = 0x0001;         //Movement occurred.
            internal static uint MOUSEEVENTF_RIGHTDOWN = 0x0008;    //The right button is down.
            internal static uint MOUSEEVENTF_RIGHTUP = 0x0010;      //The right button is up.
            internal static uint MOUSEEVENTF_WHEEL = 0x0800;        //The wheel has been moved, if the mouse has a wheel.The amount of movement is specified in dwData
            internal static uint MOUSEEVENTF_XDOWN = 0x0080;        //An X button was pressed.
            internal static uint MOUSEEVENTF_XUP = 0x0100;          //An X button was released.
//double    internal static uint MOUSEEVENTF_WHEEL = 0x0800;        //The wheel button is rotated.
            internal static uint MOUSEEVENTF_HWHEEL = 0x01000;      //The wheel button is tilted. 
        }
        #endregion

        #region mouse_event_dwData_Flags_done
        /*
         *      Flags for dwData in mouse_event()
         */
        internal struct DwDataFlags
        {
            internal static uint WHEEL_DELTA = 120;     //One wheel click is defined as WHEEL_DELTA, which is 120.
            internal static uint XBUTTON1 = 0x0001;     //Set if the first X button was pressed or released.
            internal static uint XBUTTON2 = 0x0002;     //Set if the second X button was pressed or released. 
        }
        #endregion

        #region ShowWindow_nCmdShow_Flags_done
        /*
         *      Flags for nCmdShow in ShowWindow()
         */
        internal struct NCmdShowFlags
        {
            internal static int SW_FORCEMINIMIZE = 11;      //Minimizes a window, even if the thread that owns the window is not responding.This flag should only be used when minimizing windows from a different thread.
            internal static int SW_HIDE = 0;                //Hides the window and activates another window.
            internal static int SW_MAXIMIZE = 3;            //Maximizes the specified window.
            internal static int SW_MINIMIZE = 6;            //Minimizes the specified window and activates the next top-level window in the Z order.
            internal static int SW_RESTORE = 9;             //Activates and displays the window.If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.
            internal static int SW_SHOW = 5;                //Activates the window and displays it in its current size and position.
            internal static int SW_SHOWDEFAULT = 10;        //Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.
            internal static int SW_SHOWMAXIMIZED = 3;       //Activates the window and displays it as a maximized window.
            internal static int SW_SHOWMINIMIZED = 2;       //Activates the window and displays it as a minimized window.
            internal static int SW_SHOWMINNOACTIVE = 7;     //Displays the window as a minimized window.This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
            internal static int SW_SHOWNA = 8;              //Displays the window in its current size and position.This value is similar to SW_SHOW, except that the window is not activated.
            internal static int SW_SHOWNOACTIVATE = 4;      //Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except that the window is not activated.
            internal static int SW_SHOWNORMAL = 1;          //Activates and displays a window.If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
        }
        #endregion

        #endregion

        #region imports for WindowDictionary
        internal delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("USER32.DLL")]
        internal static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        //[DllImport("USER32.DLL")]
        //internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        internal static extern IntPtr GetShellWindow();
        #endregion
    }
}


 
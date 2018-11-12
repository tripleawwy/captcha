using System;
//for Image, Bitmap, Rectangle, Graphics, Point
using System.Drawing;
//for StringBuilder
using System.Text;
//for Screen.PrimaryScreen.Bounds 
using System.Windows.Forms;
//for Thread.Sleep
using System.Threading;
//for Ocr functions
using IronOcr;

//to address funktions and flags directly
using static captcha.User32DLL;
using static captcha.User32DLL.DwFlags;
using static captcha.User32DLL.DwDataFlags;
using static captcha.User32DLL.NCmdShowFlags;
using System.Collections.Generic;

namespace captcha
{
    internal class ScreenCapture
    {

        #region static variables
        private const double MAX_ABSOLUTE = 65535; //double to force the Math.Round() function to use double

        //uncropped screenshot from captcha popup
        internal static CaptchaWindow FixedCaptcha = new CaptchaWindow(
            374, 432, //Width, Height

            //buttons x-coordinate is relative to center
            -32, 205, //MatrixZahlObenLinks
            -5, 205, //MatrixZahlObenMitte
            21, 205, //MatrixZahlObenRechts
            -32, 234, //MatrixZahlMitteLinks
            -5, 234, //MatrixZahlMitteMitte
            21, 234, //MatrixZahlMitteRechts
            -32, 263, //MatrixZahlUntenLinks
            -5, 263, //MatrixZahlUntenMitte
            21, 263, //MatrixZahlUntenRechts
            -32, 292, //MatrixZahlGanzUnten

            //button send relativ to lower right
            -55, -30  //Absenden
            );
        #endregion

        #region structs

        internal struct POINT
        {
            internal int X { get; set; }
            internal int Y { get; set; }

            //constructor
            internal POINT(int pX, int pY)
            {
                X = pX;
                Y = pY;
            }
        }

        internal struct CaptchaWindow
        {
            internal int Width { get; set; }
            internal int Height { get; set; }

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

            //default constructor
            internal CaptchaWindow(int pWidth, int pHeight
                , int pMatrixZahlObenLinksX, int pMatrixZahlObenLinksY
                , int pMatrixZahlObenMitteX, int pMatrixZahlObenMitteY
                , int pMatrixZahlObenRechtsX, int pMatrixZahlObenRechtsY
                , int pMatrixZahlMitteLinksX, int pMatrixZahlMitteLinksY
                , int pMatrixZahlMitteMitteX, int pMatrixZahlMitteMitteY
                , int pMatrixZahlMitteRechtsX, int pMatrixZahlMitteRechtsY
                , int pMatrixZahlUntenLinksX, int pMatrixZahlUntenLinksY
                , int pMatrixZahlUntenMitteX, int pMatrixZahlUntenMitteY
                , int pMatrixZahlUntenRechtsX, int pMatrixZahlUntenRechtsY
                , int pMatrixZahlGanzUntenX, int pMatrixZahlGanzUntenY
                , int pAbsendenX, int pAbsendenY
                 )
            {
                Width = pWidth;
                Height = pHeight;
                MatrixZahlObenLinks = new POINT(pMatrixZahlObenLinksX, pMatrixZahlObenLinksY);
                MatrixZahlObenMitte = new POINT(pMatrixZahlObenMitteX, pMatrixZahlObenMitteY);
                MatrixZahlObenRechts = new POINT(pMatrixZahlObenRechtsX, pMatrixZahlObenRechtsY);
                MatrixZahlMitteLinks = new POINT(pMatrixZahlMitteLinksX, pMatrixZahlMitteLinksY);
                MatrixZahlMitteMitte = new POINT(pMatrixZahlMitteMitteX, pMatrixZahlMitteMitteY);
                MatrixZahlMitteRechts = new POINT(pMatrixZahlMitteRechtsX, pMatrixZahlMitteRechtsY);
                MatrixZahlUntenLinks = new POINT(pMatrixZahlUntenLinksX, pMatrixZahlUntenLinksY);
                MatrixZahlUntenMitte = new POINT(pMatrixZahlUntenMitteX, pMatrixZahlUntenMitteY);
                MatrixZahlUntenRechts = new POINT(pMatrixZahlUntenRechtsX, pMatrixZahlUntenRechtsY);
                MatrixZahlGanzUnten = new POINT(pMatrixZahlGanzUntenX, pMatrixZahlGanzUntenY);
                Absenden = new POINT(pAbsendenX, pAbsendenY);
            }

            //constructor for relative creation by current WindowRect using static CaptchaWindow FixedCaptcha
            internal CaptchaWindow(RECT activeWindowRect)
            {
                Width = activeWindowRect.Right - activeWindowRect.Left;
                Height = activeWindowRect.Bottom - activeWindowRect.Top;

                // button x-coordinates relative to window center
                MatrixZahlObenLinks = new POINT(
                    (int)Math.Round((activeWindowRect.Right + activeWindowRect.Left) / 2.0 + ((FixedCaptcha.MatrixZahlObenLinks.X * (double)(activeWindowRect.Right - activeWindowRect.Left)) / FixedCaptcha.Width), 0)
                    , (int)Math.Round(activeWindowRect.Top + ((FixedCaptcha.MatrixZahlObenLinks.Y * (double)(activeWindowRect.Bottom - activeWindowRect.Top)) / FixedCaptcha.Height), 0)
                );
                MatrixZahlObenMitte = new POINT(
                    (int)Math.Round((activeWindowRect.Right + activeWindowRect.Left) / 2.0 + ((FixedCaptcha.MatrixZahlObenMitte.X * (double)(activeWindowRect.Right - activeWindowRect.Left)) / FixedCaptcha.Width), 0)
                    , (int)Math.Round(activeWindowRect.Top + ((FixedCaptcha.MatrixZahlObenMitte.Y * (double)(activeWindowRect.Bottom - activeWindowRect.Top)) / FixedCaptcha.Height), 0)
                );
                MatrixZahlObenRechts = new POINT(
                    (int)Math.Round((activeWindowRect.Right + activeWindowRect.Left) / 2.0 + ((FixedCaptcha.MatrixZahlObenRechts.X * (double)(activeWindowRect.Right - activeWindowRect.Left)) / FixedCaptcha.Width), 0)
                    , (int)Math.Round(activeWindowRect.Top + ((FixedCaptcha.MatrixZahlObenRechts.Y * (double)(activeWindowRect.Bottom - activeWindowRect.Top)) / FixedCaptcha.Height), 0)
                );
                MatrixZahlMitteLinks = new POINT(
                    (int)Math.Round((activeWindowRect.Right + activeWindowRect.Left) / 2.0 + ((FixedCaptcha.MatrixZahlMitteLinks.X * (double)(activeWindowRect.Right - activeWindowRect.Left)) / FixedCaptcha.Width), 0)
                    , (int)Math.Round(activeWindowRect.Top + ((FixedCaptcha.MatrixZahlMitteLinks.Y * (double)(activeWindowRect.Bottom - activeWindowRect.Top)) / FixedCaptcha.Height), 0)
                );
                MatrixZahlMitteMitte = new POINT(
                    (int)Math.Round((activeWindowRect.Right + activeWindowRect.Left) / 2.0 + ((FixedCaptcha.MatrixZahlMitteMitte.X * (double)(activeWindowRect.Right - activeWindowRect.Left)) / FixedCaptcha.Width), 0)
                    , (int)Math.Round(activeWindowRect.Top + ((FixedCaptcha.MatrixZahlMitteMitte.Y * (double)(activeWindowRect.Bottom - activeWindowRect.Top)) / FixedCaptcha.Height), 0)
                );
                MatrixZahlMitteRechts = new POINT(
                    (int)Math.Round((activeWindowRect.Right + activeWindowRect.Left) / 2.0 + ((FixedCaptcha.MatrixZahlMitteRechts.X * (double)(activeWindowRect.Right - activeWindowRect.Left)) / FixedCaptcha.Width), 0)
                    , (int)Math.Round(activeWindowRect.Top + ((FixedCaptcha.MatrixZahlMitteRechts.Y * (double)(activeWindowRect.Bottom - activeWindowRect.Top)) / FixedCaptcha.Height), 0)
                );
                MatrixZahlUntenLinks = new POINT(
                    (int)Math.Round((activeWindowRect.Right + activeWindowRect.Left) / 2.0 + ((FixedCaptcha.MatrixZahlUntenLinks.X * (double)(activeWindowRect.Right - activeWindowRect.Left)) / FixedCaptcha.Width), 0)
                    , (int)Math.Round(activeWindowRect.Top + ((FixedCaptcha.MatrixZahlUntenLinks.Y * (double)(activeWindowRect.Bottom - activeWindowRect.Top)) / FixedCaptcha.Height), 0)
                );
                MatrixZahlUntenMitte = new POINT(
                    (int)Math.Round((activeWindowRect.Right + activeWindowRect.Left) / 2.0 + ((FixedCaptcha.MatrixZahlUntenMitte.X * (double)(activeWindowRect.Right - activeWindowRect.Left)) / FixedCaptcha.Width), 0)
                    , (int)Math.Round(activeWindowRect.Top + ((FixedCaptcha.MatrixZahlUntenMitte.Y * (double)(activeWindowRect.Bottom - activeWindowRect.Top)) / FixedCaptcha.Height), 0)
                );
                MatrixZahlUntenRechts = new POINT(
                    (int)Math.Round((activeWindowRect.Right + activeWindowRect.Left) / 2.0 + ((FixedCaptcha.MatrixZahlUntenRechts.X * (double)(activeWindowRect.Right - activeWindowRect.Left)) / FixedCaptcha.Width), 0)
                    , (int)Math.Round(activeWindowRect.Top + ((FixedCaptcha.MatrixZahlUntenRechts.Y * (double)(activeWindowRect.Bottom - activeWindowRect.Top)) / FixedCaptcha.Height), 0)
                );
                MatrixZahlGanzUnten = new POINT(
                    (int)Math.Round((activeWindowRect.Right + activeWindowRect.Left) / 2.0 + ((FixedCaptcha.MatrixZahlGanzUnten.X * (double)(activeWindowRect.Right - activeWindowRect.Left)) / FixedCaptcha.Width), 0)
                    , (int)Math.Round(activeWindowRect.Top + ((FixedCaptcha.MatrixZahlGanzUnten.Y * (double)(activeWindowRect.Bottom - activeWindowRect.Top)) / FixedCaptcha.Height), 0)
                );

                // send button y/x-coordinates relative to lower right position of window
                Absenden = new POINT(
                    (int)Math.Round(activeWindowRect.Right + ((FixedCaptcha.Absenden.X * (double)(activeWindowRect.Right - activeWindowRect.Left)) / FixedCaptcha.Width), 0)
                    , (int)Math.Round(activeWindowRect.Bottom + ((FixedCaptcha.Absenden.Y * (double)(activeWindowRect.Bottom - activeWindowRect.Top)) / FixedCaptcha.Height), 0)
                );
            }
        }

        #endregion

        #region WindowDictionary
        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static IDictionary<IntPtr, string> GetOpenWindows()
        {
            IntPtr shellWindow = GetShellWindow();
            Dictionary<IntPtr, string> windows = new Dictionary<IntPtr, string>();

            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder sb = new StringBuilder(length);
                GetWindowText(hWnd, sb, GetWindowTextLength(hWnd) + 1);

                windows[hWnd] = sb.ToString();
                return true;

            }, 0);

            return windows;
        }
        #endregion

        #region functions

        internal static CaptchaWindow PrepareCaptchaWindowForMouseMoveAbsolute(CaptchaWindow captcha)
        {
            captcha.MatrixZahlObenLinks.X = (int)Math.Round((captcha.MatrixZahlObenLinks.X * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Width, 0);
            captcha.MatrixZahlObenLinks.Y = (int)Math.Round((captcha.MatrixZahlObenLinks.Y * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Height, 0);
            captcha.MatrixZahlObenMitte.X = (int)Math.Round((captcha.MatrixZahlObenMitte.X * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Width, 0);
            captcha.MatrixZahlObenMitte.Y = (int)Math.Round((captcha.MatrixZahlObenMitte.Y * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Height, 0);
            captcha.MatrixZahlObenRechts.X = (int)Math.Round((captcha.MatrixZahlObenRechts.X * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Width, 0);
            captcha.MatrixZahlObenRechts.Y = (int)Math.Round((captcha.MatrixZahlObenRechts.Y * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Height, 0);
            captcha.MatrixZahlMitteLinks.X = (int)Math.Round((captcha.MatrixZahlMitteLinks.X * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Width, 0);
            captcha.MatrixZahlMitteLinks.Y = (int)Math.Round((captcha.MatrixZahlMitteLinks.Y * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Height, 0);
            captcha.MatrixZahlMitteMitte.X = (int)Math.Round((captcha.MatrixZahlMitteMitte.X * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Width, 0);
            captcha.MatrixZahlMitteMitte.Y = (int)Math.Round((captcha.MatrixZahlMitteMitte.Y * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Height, 0);
            captcha.MatrixZahlMitteRechts.X = (int)Math.Round((captcha.MatrixZahlMitteRechts.X * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Width, 0);
            captcha.MatrixZahlMitteRechts.Y = (int)Math.Round((captcha.MatrixZahlMitteRechts.Y * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Height, 0);
            captcha.MatrixZahlUntenLinks.X = (int)Math.Round((captcha.MatrixZahlUntenLinks.X * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Width, 0);
            captcha.MatrixZahlUntenLinks.Y = (int)Math.Round((captcha.MatrixZahlUntenLinks.Y * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Height, 0);
            captcha.MatrixZahlUntenMitte.X = (int)Math.Round((captcha.MatrixZahlUntenMitte.X * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Width, 0);
            captcha.MatrixZahlUntenMitte.Y = (int)Math.Round((captcha.MatrixZahlUntenMitte.Y * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Height, 0);
            captcha.MatrixZahlUntenRechts.X = (int)Math.Round((captcha.MatrixZahlUntenRechts.X * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Width, 0);
            captcha.MatrixZahlUntenRechts.Y = (int)Math.Round((captcha.MatrixZahlUntenRechts.Y * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Height, 0);
            captcha.MatrixZahlGanzUnten.X = (int)Math.Round((captcha.MatrixZahlGanzUnten.X * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Width, 0);
            captcha.MatrixZahlGanzUnten.Y = (int)Math.Round((captcha.MatrixZahlGanzUnten.Y * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Height, 0);
            captcha.Absenden.X = (int)Math.Round((captcha.Absenden.X * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Width, 0);
            captcha.Absenden.Y = (int)Math.Round((captcha.Absenden.Y * MAX_ABSOLUTE) / Screen.PrimaryScreen.Bounds.Height, 0);

            return captcha;
        }

        internal static Bitmap CaptureWindow(IntPtr hWnd)
        {
            RECT rect = new RECT();
            GetWindowRect(hWnd, ref rect);

            Bitmap result = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top);

            using (var graphics = Graphics.FromImage(result))
            {
                graphics.CopyFromScreen(new Point(rect.Left, rect.Top), Point.Empty, new Size(rect.Right - rect.Left, rect.Bottom - rect.Top));
            }

            return result;
        }

        internal static string GetWindowTitle(IntPtr hWnd)
        {
            StringBuilder Buff = new StringBuilder(255);

            if (GetWindowText(hWnd, Buff, Buff.Capacity) > 0)
            {
                return Buff.ToString();
            }

            return null;
        }

        internal static void SetWindowPositionCentred(IntPtr hWnd)
        {
            RECT desktopRect = new RECT();
            RECT windowRect = new RECT();

            GetWindowRect(GetDesktopWindow(), ref desktopRect);
            GetWindowRect(hWnd, ref windowRect);

            int desktopWidth = desktopRect.Right - desktopRect.Left;
            int desktopHeight = desktopRect.Bottom - desktopRect.Top;
            int windowWidth = windowRect.Right - windowRect.Left;
            int windowHeight = windowRect.Bottom - windowRect.Top;

            MoveWindow(hWnd
                , desktopWidth > windowWidth ? (desktopWidth - windowWidth) / 2 : 0
                , desktopHeight > windowHeight ? (desktopHeight - windowHeight) / 2 : 0
                , windowWidth
                , windowHeight
                , true);

            ShowWindow(hWnd, SW_SHOWNORMAL);
            SetForegroundWindow(hWnd);
        }

        internal static Bitmap CropScreenshot(string popupImagePath, RECT rect)
        {
            //create bitmap from original screenshot
            Bitmap image = Image.FromFile(popupImagePath) as Bitmap;

            //prepare size for cropped bitmap
            int curWidth = rect.Right - rect.Left;
            int curHeight = rect.Bottom - rect.Top;

            //rectangle to crop image from original screenshot
            Rectangle croppedImageRect = new Rectangle(
                (int)Math.Round( (curWidth / 2.0) - (160.0 * curWidth / FixedCaptcha.Width),0)  // X relative to middle
               ,(int)Math.Round( 110.0 * curHeight / FixedCaptcha.Height ,0)                    // Y 
               ,(int)Math.Round( 320.0 * curWidth / FixedCaptcha.Width ,0)                      // Width
               ,(int)Math.Round( 210.0 * curHeight / FixedCaptcha.Height ,0)                    // Height
                );

            //create new bitmap for cropped image
            Bitmap croppedImage = new Bitmap(croppedImageRect.Width, croppedImageRect.Height);

            //crop original image and save to croppedImage
            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                //draw new image, cropped from original screenshot
                g.DrawImage( image
                    , new Rectangle(0, 0, croppedImage.Width, croppedImage.Height)
                    , croppedImageRect
                    , GraphicsUnit.Pixel);

                //weißen Rand zeichnen
                g.DrawRectangle(new Pen(Brushes.White, 10 * curWidth / FixedCaptcha.Width)
                    , new Rectangle(0, 0, croppedImage.Width, croppedImage.Height));

                //Button "Löschen" übermalen
                g.DrawRectangle(new Pen(Brushes.White, 26 * curHeight / FixedCaptcha.Height)
                    , new Rectangle(
                        //x = relative to center of croppedImage + relative pos.x + 0.5 BrushSize
                        (int)Math.Round((croppedImage.Width / 2.0)
                                       + (-16.0 * curWidth / FixedCaptcha.Width)
                                       + (0.5 * 26.0 * curHeight / FixedCaptcha.Height)
                                       , 0)
                        // y = relative pos.y - cropped top of the image
                        , (int)Math.Round( (292.5 - 110.0) * curHeight / FixedCaptcha.Height ,0)
                        // width = width of button clear - BrushSize
                        , (int)Math.Round((60.0 * curWidth / FixedCaptcha.Width) - (26.0 * curHeight / FixedCaptcha.Height), 0)
                        // height (only a line)
                        , 1));

                //Textfeld übermalen
                g.DrawRectangle(new Pen(Brushes.White, 26 * curHeight / FixedCaptcha.Height)
                    , new Rectangle( 0                                                          // x (whole line)
                        , (int)Math.Round( (160.0-110.0) * curHeight / FixedCaptcha.Height ,0)  // y
                        , croppedImage.Width                                                    // width (whole line)
                        , 1 ));                                                                 // height (only a line)
            }
           return croppedImage;
        }

        internal static string DoOCR(string croppedImagePath)
        {
            IronOcr.AdvancedOcr Ocr = new IronOcr.AdvancedOcr
            {
                //ocr settings
                AcceptedOcrCharacters = "0123456789",
                Strategy = IronOcr.AdvancedOcr.OcrStrategy.Advanced,
                ReadBarCodes = true,
                CleanBackgroundNoise = true,
                ColorSpace = AdvancedOcr.OcrColorSpace.Color,
                InputImageType = AdvancedOcr.InputTypes.Snippet,
                ColorDepth = 6,
                Language = IronOcr.Languages.German.OcrLanguagePack,
                EnhanceContrast = true,
                EnhanceResolution = true,
                DetectWhiteTextOnDarkBackgrounds = true
            };

            //proceed ocr
            StringBuilder sb;
            try
            {
                sb = new StringBuilder( (Ocr.Read(croppedImagePath)).ToString() );
            }catch(Exception e)
            {
                //ignore ocr exception
                sb = new StringBuilder("42");
            }
            return sb.ToString();
        }

        internal static void resolveCaptcha(string captchaZahl, string matrixZahl, IntPtr popupWindow)
        {
            Random rd = new Random(DateTime.Now.Millisecond);
            int wt = 300; //for randomly Sleep( wait + random.next() % wait ) meaning 450 +- 150 milliseconds

            RECT currentWindowRect = new RECT();
            GetWindowRect(popupWindow, ref currentWindowRect);
            CaptchaWindow curCaptcha = new CaptchaWindow(currentWindowRect);
            
            //Verhältnis für MouseMove(ABSOLUTE)
            curCaptcha = PrepareCaptchaWindowForMouseMoveAbsolute(curCaptcha);

            for (int i = 0, j = 0; i < matrixZahl.Length; i++)
            {
                if (captchaZahl[j] == matrixZahl[i])
                {
                    switch (i)
                    {
                        case 0:
                            mouse_event((MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE), curCaptcha.MatrixZahlObenLinks.X, curCaptcha.MatrixZahlObenLinks.Y, 0, new UIntPtr());
                            Console.WriteLine("oben links");
                            break;
                        case 1:
                            mouse_event((MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE), curCaptcha.MatrixZahlObenMitte.X, curCaptcha.MatrixZahlObenMitte.Y, 0, new UIntPtr());
                            Console.WriteLine("oben mitte");
                            break;
                        case 2:
                            mouse_event((MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE), curCaptcha.MatrixZahlObenRechts.X, curCaptcha.MatrixZahlObenRechts.Y, 0, new UIntPtr());
                            Console.WriteLine("oben rechts");
                            break;
                        case 3:
                            mouse_event((MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE), curCaptcha.MatrixZahlMitteLinks.X, curCaptcha.MatrixZahlMitteLinks.Y, 0, new UIntPtr());
                            Console.WriteLine("mitte links");
                            break;
                        case 4:
                            mouse_event((MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE), curCaptcha.MatrixZahlMitteMitte.X, curCaptcha.MatrixZahlMitteMitte.Y, 0, new UIntPtr());
                            Console.WriteLine("mitte mitte");
                            break;
                        case 5:
                            mouse_event((MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE), curCaptcha.MatrixZahlMitteRechts.X, curCaptcha.MatrixZahlMitteRechts.Y, 0, new UIntPtr());
                            Console.WriteLine("mitte rechts");
                            break;
                        case 6:
                            mouse_event((MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE), curCaptcha.MatrixZahlUntenLinks.X, curCaptcha.MatrixZahlUntenLinks.Y, 0, new UIntPtr());
                            Console.WriteLine("unten links");
                            break;
                        case 7:
                            mouse_event((MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE), curCaptcha.MatrixZahlUntenMitte.X, curCaptcha.MatrixZahlUntenMitte.Y, 0, new UIntPtr());
                            Console.WriteLine("unten mitte");
                            break;
                        case 8:
                            mouse_event((MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE), curCaptcha.MatrixZahlUntenRechts.X, curCaptcha.MatrixZahlUntenRechts.Y, 0, new UIntPtr());
                            Console.WriteLine("unten rechts");

                            break;
                        case 9:
                            mouse_event((MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE), curCaptcha.MatrixZahlGanzUnten.X, curCaptcha.MatrixZahlGanzUnten.Y, 0, new UIntPtr());
                            Console.WriteLine("ganz unten ");
                            break;
                    }
                    Thread.Sleep(wt + (rd.Next() % wt));

                    //click
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, new UIntPtr());
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, new UIntPtr());
                    Thread.Sleep(wt + (rd.Next() % wt));

                    //setup for new loop, only if numbers matched each other
                    j = j + 1;
                    i = -1;

                    if (j >= captchaZahl.Length)
                    {
                        break; //captcha done
                    }
                }
            }

            //click send
            mouse_event((MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE), curCaptcha.Absenden.X, curCaptcha.Absenden.Y, 0, new UIntPtr());
            Thread.Sleep(wt + (rd.Next() % wt));

            //ToDo: click send
            //Console.WriteLine("...press any key to continue...");
            //Console.ReadKey();
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, new UIntPtr());
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, new UIntPtr());
            Thread.Sleep(wt + (rd.Next() % wt));
        }

        #endregion

    }
}


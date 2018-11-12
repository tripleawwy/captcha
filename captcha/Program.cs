using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Imaging;

//to address funktions and flags directly
using static captcha.User32DLL;
using static captcha.ScreenCapture;
using System.Drawing;

namespace captcha
{
    class Program
    {
        static void Main(string[] args)
        {
            #region init
            Random random = new Random(DateTime.Now.Millisecond);
            const int wait = 500; //for randomly Sleep( wait + random.next() % wait ) meaning 750 +- 250 milliseconds
            const string programTitle = "CCLauncher_Client";
            const string popupTitle = "Anwesenheitskontrolle";
            const string popupImagePath = @".\temp\_screeshot.png";
            const string croppedImagePath = @".\temp\_cropped.png";
            const string logFilePath = @".\temp\_logFile.txt";

            if (!Directory.Exists(@".\temp"))
            {
                Directory.CreateDirectory(@".\temp");
            }

            Process ccLauncherProcess = new Process();
            IntPtr popupWindow = new IntPtr();
            RECT currentWindowRect = new RECT();
            string recognizedText = "";
            string captchaZahl = "";
            string matrixZahl = "";
            //override previous log file
            using (FileStream fs = new FileStream(logFilePath, FileMode.Create, FileAccess.Write)) { fs.Close(); }
            #endregion

            // main programm loop, "<= MaxValue" specifies infinite loop, Counter is used for dots "..."
            for (uint loopCounter = 0; loopCounter <= uint.MaxValue; loopCounter++)
            {
                #region user_info
                Console.Clear();
                Console.WriteLine("Initializing Captcha-Solver...");
                Console.WriteLine();
                Console.WriteLine("Captcha-Solver started!");
                Console.WriteLine();
                #endregion

                #region check_if_CC_Launcher_is_running
                do
                {
                    try
                    {
                        Console.WriteLine("Scanning for CC_Launcher process...");
                        Console.WriteLine();

                        //geht auch schöner
                        ccLauncherProcess = Process.GetProcessesByName(programTitle)[0];

                        Console.WriteLine("CC_Launcher found!");
                        Console.WriteLine();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine();
                        Console.WriteLine("An Error occured!");
                        Console.WriteLine();
                        Console.WriteLine("Make sure CC_Launcher is running!!");
                        Console.WriteLine();
                        Console.WriteLine("Press Enter after starting CC_Launcher...");
                        Console.Read();
                    }
                } while (ccLauncherProcess.ProcessName != programTitle);
                #endregion

                #region print_log
                using (FileStream fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        Console.Write(sr.ReadToEnd());
                        Console.WriteLine();
                        sr.Close();
                    }
                    fs.Close();
                }
                #endregion

                #region find_captcha_popup
                // scanning for captcha popup
                Console.Write("Scanning for Captcha-Popup");
                for (int i = 0; i < loopCounter % 21; i++)
                {
                    Console.Write(".");
                }
                Console.WriteLine();

                popupWindow = IntPtr.Zero;
                foreach (KeyValuePair<IntPtr, string> window in GetOpenWindows())
                {
                    if (window.Value.Contains(popupTitle))
                    {
                        popupWindow = window.Key;
                    }
                }

                #endregion

                #region proceed_OCR
                // only if popup found
                if (popupWindow != IntPtr.Zero)
                {
                    Console.WriteLine();
                    Console.WriteLine("Popup recognized!");
                    Console.WriteLine();

                    //proceed ocr
                    Console.WriteLine("Proceeding ocr...");
                    Console.WriteLine();

                    //positioning and set to foreground
                    SetWindowPositionCentred(popupWindow);

                    Thread.Sleep(wait + (random.Next() % wait));


                    //ToDo: IO.Exception if writing too quickly several times in a row...
                    //solution 1. slower looping about 10-15 sec per loop
                    // or 2. forced garbage collection, see ToDo GC.Collect()
                    //screenshot popup window
                    using (FileStream fs = new FileStream(popupImagePath, FileMode.Create, FileAccess.Write))
                    {
                        Bitmap bitmap = CaptureWindow(popupWindow);
                        bitmap.Save(fs, ImageFormat.Png);
                        bitmap.Dispose();
                        fs.Close();
                    }

                    //raises io.exception too
                    //crop screenshot
                    GetWindowRect(popupWindow, ref currentWindowRect);
                    using (FileStream fs = new FileStream(croppedImagePath, FileMode.Create, FileAccess.Write))
                    {
                        Bitmap bitmap = CropScreenshot(popupImagePath, currentWindowRect);
                        bitmap.Save(fs, ImageFormat.Png);
                        bitmap.Dispose();
                        fs.Close();
                    }

                    //proceed ocr
                    recognizedText = DoOCR(croppedImagePath);
                    Console.WriteLine(recognizedText);

                    //write log
                    using (FileStream fs = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                sr.ReadToEnd();
                                sw.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " : ");
                                sw.Write(recognizedText.ToArray());
                                sw.WriteLine();
                                sw.WriteLine();
                                sw.Close();
                            }
                            sr.Close();
                        }
                        fs.Close();
                    }

                    //clear text to digits only
                    recognizedText = new string(recognizedText.Where(Char.IsDigit).ToArray());

                    //splitt recognized string
                    if (recognizedText.Length > 13)
                    {
                        captchaZahl = recognizedText.Substring(0, 4); Console.WriteLine(captchaZahl);
                        matrixZahl = recognizedText.Substring(4, 10); Console.WriteLine(matrixZahl);

                        //ensure captcha popup is at correct position
                        SetWindowPositionCentred(popupWindow);
                        Thread.Sleep(wait + (random.Next() % wait));

                        //resolve captcha
                        resolveCaptcha(captchaZahl, matrixZahl, popupWindow);
                        Thread.Sleep(wait + (random.Next() % wait));
                    }
                    else
                    {
                        Console.WriteLine("couldn't solve captcha, will try again...");
                        Console.WriteLine();
                        Thread.Sleep(wait + (random.Next() % wait));
                    }

                    //ToDo: GC.Collect()
                    // force to collect garbage due to io.exception, see ToDo io.exception
                    GC.Collect();
                }
                //scan delay
                Thread.Sleep(5 * (wait + (random.Next() % wait)));
                #endregion
            }
        }
    }
}
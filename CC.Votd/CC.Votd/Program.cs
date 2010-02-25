﻿using System;
using System.Threading;
using System.Windows.Forms;
using CC.Utilities;

namespace CC.Votd
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;

            if (args.Length > 0)
            {
                string arg = args[0].ToLowerInvariant().Trim().Substring(0, 2);
                switch (arg)
                {
                    case "/c":
                        {
                            Application.Run(new FormOptions());
                            break;
                        }
                    case "/p":
                        {
                            IntPtr previewHandle = IntPtr.Zero;
                            
                            if (args.Length > 1)
                            {
                                long tempLong;
                                if (long.TryParse(args[1], out tempLong))
                                {
                                    previewHandle = new IntPtr(tempLong);
                                    Logging.LogMessage("Preview Handle: " + previewHandle);
                                }
                            }

                            Application.Run(new FormMain(previewHandle));
                            break;
                        }
                    case "/s":
                        {
                            Application.Run(new FormMain(IntPtr.Zero));
                            break;
                        }
                    case "/d":
                        {
                            // Should be debug mode...
                            Application.Run(new FormMain(IntPtr.Zero));
                            break;
                        }
                    default:
                        {
                            MessageBox.Show("Invalid command line argument: " + arg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                }
            }
            else
            {
                Application.Run(new FormMain(IntPtr.Zero));
            }
        }

        #region Private Event Handlers
        // ReSharper disable InconsistentNaming
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show("Unhandled Exception: " + e.Exception, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Logging.LogException(e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Unhandled Exception: " + e.ExceptionObject, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Exception ex = e.ExceptionObject as Exception;

            if (ex != null)
            {
                if (ex.InnerException != null)
                {
                    MessageBox.Show("Inner Exception: \r\n\r\n" + ex.InnerException);
                }

                Logging.LogException(ex);
            }
        }
        // ReSharper restore InconsistentNaming
        #endregion

    }
}

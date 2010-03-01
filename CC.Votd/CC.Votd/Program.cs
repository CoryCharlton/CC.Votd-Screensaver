using System;
using System.Diagnostics;
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

            ArgumentParser argumentParser = new ArgumentParser(new[] {"/", "-"}, true, new[] {new Argument("c", ArgumentValue.Optional, true), new Argument("d", ArgumentValue.None, true), new Argument("p", ArgumentValue.Required, true), new Argument("s", ArgumentValue.None, true)});
            argumentParser.Parse(args);
            ArgumentDictionary validArguments = argumentParser.ParsedArguments.GetValidArguments();

            if (validArguments.Contains("d"))
            {
                Debugger.Launch();
                Settings.IsDebug = true;                
            }

            if (validArguments.Contains("c"))
            {
                Application.Run(new FormOptions());
            }
            else
            {
                IntPtr previewHandle = IntPtr.Zero;

                if (validArguments.Contains("p"))
                {
                    long tempLong;
                    if (long.TryParse(validArguments["p"].Value, out tempLong))
                    {
                        previewHandle = new IntPtr(tempLong);
                        Logging.LogMessage("Preview Handle: " + previewHandle);
                    }
                }

                Application.Run(new FormMain(previewHandle));
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

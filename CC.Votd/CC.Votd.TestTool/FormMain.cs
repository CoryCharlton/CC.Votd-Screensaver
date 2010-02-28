using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace CC.Votd.TestTool
{
    public partial class FormMain : Form
    {
        #region Constructor
        public FormMain()
        {
            InitializeComponent();
        }
        #endregion

        #region Private Fields
        private bool _Closing;
        private Process _ScreenSaverProcess;
        #endregion

        #region Private Event Handlers
        private void _ButtonPreview_Click(object sender, EventArgs e)
        {
            StartProcess(true);
        }

        private void _ButtonSettings_Click(object sender, EventArgs e)
        {
            StartProcess("/c", true);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _Closing = true;

            KillProcess();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Show();
            StartMiniPreview();
        }
        #endregion

        #region Private Methods
        private void KillProcess()
        {
            if (_ScreenSaverProcess != null)
            {
                if (!_ScreenSaverProcess.HasExited)
                {
                    _ScreenSaverProcess.Kill();
                }

                _ScreenSaverProcess.Dispose();
                _ScreenSaverProcess = null;
            }
        }

        private void StartMiniPreview()
        {
            StartProcess("/p " + _PanelMiniPreview.Handle, false);
        }

        private void StartProcess(bool waitForExit)
        {
            StartProcess(null, waitForExit);
        }

        private void StartProcess(string arguments, bool waitForExit)
        {
            KillProcess();

            if (_Closing)
            {
                return;
            }

            _ScreenSaverProcess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(Application.StartupPath, @"..\..\..\CC.Votd\bin\Debug\CC.Votd.exe"))
                                             {
                                                 WorkingDirectory = Path.Combine(Application.StartupPath, @"..\..\..\CC.Votd\bin\Debug\")
                                             };

            if (_CheckBoxDebug.Checked)
            {
                startInfo.Arguments = "/d" + (!string.IsNullOrEmpty(arguments) ? " " + arguments : string.Empty);
            }
            else if (!string.IsNullOrEmpty(arguments))
            {
                startInfo.Arguments = arguments;
            }

            _ScreenSaverProcess.StartInfo = startInfo;
            _ScreenSaverProcess.Start();

            if (waitForExit)
            {
                UseWaitCursor = true;
                
                _ButtonPreview.Enabled = false;
                _ButtonSettings.Enabled = false;
                _CheckBoxDebug.Enabled = false;

                while (!_Closing && !_ScreenSaverProcess.WaitForExit(250))
                {
                    Application.DoEvents();
                }

                if (!_Closing)
                {
                    StartMiniPreview();
                }

                UseWaitCursor = false;

                _ButtonPreview.Enabled = true;
                _ButtonSettings.Enabled = true;
                _CheckBoxDebug.Enabled = true;
            }
        }
        #endregion
    }
}

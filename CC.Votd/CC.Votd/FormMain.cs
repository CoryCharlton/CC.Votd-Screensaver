using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CC.Utilities.Interop;

namespace CC.Votd
{
    public partial class FormMain : FormScreenSaver
    {
        // TODO: Move these into CC.Utilities, verifiy their accuracy and that they are the correct choice
        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        #region Constructor
        public FormMain() : this(IntPtr.Zero)
        {
            // Empty constructor
        }

        public FormMain(IntPtr previewHandle)
        {
            InitializeComponent();

            if (previewHandle != IntPtr.Zero)
            {
                Program.Settings.IsPreview = true;
                _PreviewHandle = previewHandle;
            }

            SetupScreenSaver();
        }
        #endregion

        #region Private Constants
        #endregion

        #region Private Fields
        private bool _IsActive;
        private Point _MouseLocation;
        private readonly IntPtr _PreviewHandle = IntPtr.Zero;
        private readonly List<FormScreenSaver> _SecondaryScreensSavers = new List<FormScreenSaver>();
        #endregion

        #region Protected Properties
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;

                if (!DesignMode && Program.Settings.IsPreview)
                {
                    createParams.Style |= 0x40000000; // Add the WS_CHILD style to preview mode...
                }

                return createParams;
            }
        }
        #endregion

        #region Public Event Handlers
        public void FormScreenSaver_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Program.Settings.IsPreview)
            {
                Close();
            }
        }

        public void FormScreenSaver_MouseDown(object sender, MouseEventArgs e)
        {
            if (!Program.Settings.IsPreview)
            {
                Close();
            }
        }

        public void FormScreenSaver_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Program.Settings.IsPreview)
            {
                if (!_IsActive)
                {
                    _IsActive = true;
                    _MouseLocation = MousePosition;
                }
                else
                {
                    if ((Math.Abs(MousePosition.X - _MouseLocation.X) > 10) || (Math.Abs(MousePosition.Y - _MouseLocation.Y) > 10))
                    {
                        Close();
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        private void CreateSecondaryScreenSavers()
        {
            if (Screen.AllScreens.Length > 1)
            {
                foreach (Screen screen in Screen.AllScreens)
                {
                    if (screen.Primary == false)
                    {
                        FormScreenSaver formScreenSaver = new FormScreenSaver
                                                              {
                                                                  Capture = true,
                                                                  ShowInTaskbar = false,
                                                                  StartPosition = FormStartPosition.Manual,
                                                                  Location = screen.WorkingArea.Location,
                                                                  Size = new Size(screen.WorkingArea.Width, screen.WorkingArea.Height),
                                                                  TopMost = true
                                                              };

                        formScreenSaver.KeyDown += FormScreenSaver_KeyDown;
                        formScreenSaver.MouseDown += FormScreenSaver_MouseDown;
                        formScreenSaver.MouseMove += FormScreenSaver_MouseMove;
                        formScreenSaver.Initialize();
                        formScreenSaver.Show();

                        _SecondaryScreensSavers.Add(formScreenSaver);
                    }
                }
            }
        }

        private void SetupScreenSaver()
        {
            if (!string.IsNullOrEmpty(Program.Settings.BackgroundImage) && File.Exists(Program.Settings.BackgroundImage))
            {
                try
                {
                    BackgroundImage = Image.FromFile(Program.Settings.BackgroundImage);
                }
                catch (Exception)
                {
                    BackgroundImage = Properties.Resources.Cross;
                }
            }

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            BackgroundImageLayout = ImageLayout.Stretch;
            Capture = true;
            DoubleBuffered = true;
            ShowInTaskbar = false;

            if (!Program.Settings.IsPreview)
            {
                Cursor.Hide();
                Bounds = Screen.PrimaryScreen.Bounds;
                WindowState = FormWindowState.Maximized;
#if DEBUG
                TopMost = false;
#else
                TopMost = true;
#endif
            }
            else
            {
                User32.SetParent(Handle, _PreviewHandle);

                Rectangle parentRectangle;
                GetClientRect(_PreviewHandle, out parentRectangle);
                Size = parentRectangle.Size;

                Location = new Point(0, 0);
            }

            if (!Program.Settings.IsPreview)
            {
                CreateSecondaryScreenSavers();
            }

            Initialize();
        }
        #endregion
    }
}

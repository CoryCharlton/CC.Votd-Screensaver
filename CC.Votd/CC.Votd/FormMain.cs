using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CC.Utilities.Interop;
using CC.Utilities.Rss;

namespace CC.Votd
{
    public partial class FormMain : FormScreenSaver
    {
        #region Constructor
        public FormMain() : this(IntPtr.Zero)
        {
            // Empty constructor
        }

        public FormMain(IntPtr previewHandle)
        {
            if (previewHandle != IntPtr.Zero)
            {
                Settings.IsPreview = true;
                _PreviewHandle = previewHandle;
            }

            InitializeComponent();

            /*
            RssItem rssItem = RssItemCache.GetRandomItem();

            if (rssItem != null)
            {
                RssItem rssItem2 = new RssItem
                                       {
                                           Author = rssItem.Author, 
                                           Category = rssItem.Category, 
                                           Comments = rssItem.Comments,
                                           Description = rssItem.Description,
                                           Enclosure = rssItem.Enclosure,
                                           Guid = rssItem.Guid,
                                           Link = rssItem.Link,
                                           Title = rssItem.Title
                                       };

                RssItemCache.Add(rssItem2);
            }
            */

            SetupScreenSaver();
            Initialize();
        }
        #endregion

        #region Private Fields
        private bool _IsActive;
        private Point _MouseLocation;
        private readonly IntPtr _PreviewHandle = IntPtr.Zero;
        private readonly List<FormScreenSaver> _SecondaryScreens = new List<FormScreenSaver>();
        #endregion

        #region Protected Properties
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;

                if (!DesignMode && Settings.IsPreview)
                {
                    createParams.Style |= 0x40000000; // Add the WS_CHILD style to preview mode...
                }

                return createParams;
            }
        }
        #endregion

        #region Public Event Handlers
        public void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Settings.IsPreview)
            {
                if (!Settings.IsDebug)
                {
                    Close();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    Close();
                }
            }
        }

        public void FormMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (!Settings.IsPreview && !Settings.IsDebug)
            {
                Close();
            }
        }

        public void FormMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Settings.IsPreview && !Settings.IsDebug)
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
                                                                  StartPosition = FormStartPosition.Manual,
                                                                  Location = screen.WorkingArea.Location,
                                                                  Size = new Size(screen.WorkingArea.Width, screen.WorkingArea.Height),
                                                                  TopMost = true
                                                              };

                        formScreenSaver.KeyDown += FormMain_KeyDown;
                        formScreenSaver.MouseDown += FormMain_MouseDown;
                        formScreenSaver.MouseMove += FormMain_MouseMove;
                        formScreenSaver.Initialize();
                        formScreenSaver.Show();

                        _SecondaryScreens.Add(formScreenSaver);
                    }
                }
            }
        }

        private void SetupScreenSaver()
        {
            if (!Settings.IsPreview)
            {
                if (!Settings.IsDebug)
                {
                    Cursor.Hide();
                }

                Bounds = Screen.PrimaryScreen.Bounds;
                WindowState = FormWindowState.Maximized;
                TopMost = !Settings.IsDebug;
            }
            else
            {
                Cursor = Cursors.Default;
                User32.SetParent(Handle, _PreviewHandle);

                RECT parentRectangle = new RECT();
                if (User32.GetClientRect(_PreviewHandle, parentRectangle))
                {
                    Size = new Size(parentRectangle.right - parentRectangle.left, parentRectangle.bottom - parentRectangle.top);
                }

                Location = new Point(0, 0);
            }

            if (!Settings.IsPreview)
            {
                CreateSecondaryScreenSavers();
            }
        }
        #endregion
    }
}

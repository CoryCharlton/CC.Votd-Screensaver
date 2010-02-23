using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CC.Utilities.Interop;
using CC.Utilities.Rss;

namespace CC.Votd
{
    public partial class FormScreenSaver : Form
    {
        // TODO: Move these into CC.Utilities, verifiy their accuracy and that they are the correct choice
        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        #region Constructor
        public FormScreenSaver() : this(IntPtr.Zero)
        {
            // Empty constructor
        }

        public FormScreenSaver(IntPtr previewHandle)
        {
            InitializeComponent();
            Icon = Properties.Resources.Bible;

            //_Settings = settings;
            if (previewHandle != IntPtr.Zero)
            {
                _IsPreview = true;
                _PreviewHandle = previewHandle;
            }

            if (!_IsPreview)
            {
                CreateSecondaryScreenSavers();
            }

            SetupScreenSaver();

            _RssFeed = new RssFeed(_Settings.RandomVerse ? VERSE_RANDOM : VERSE_DAILY);
            _RssItemView = new RssItemView(_RssFeed.Channels[0].Items[0]);

            InitializeRssItemView();
        }
        #endregion

        #region Private Constants
        // ReSharper disable InconsistentNaming
        private const string VERSE_DAILY = "http://www.gnpcb.org/esv/share/rss2.0/daily/";
        private const string VERSE_RANDOM = "http://www.gnpcb.org/esv/share/rss2.0/verse/";
        // ReSharper restore InconsistentNaming
        #endregion

        #region Private Fields
        private bool _IsActive;
        private readonly bool _IsPreview;
        private Point _MouseLocation;
        private readonly IntPtr _PreviewHandle = IntPtr.Zero;
        private readonly Random _Random = new Random();
        private readonly RssFeed _RssFeed;
        private readonly RssItemView _RssItemView;
        private readonly List<FormSecondaryScreenSaver> _SecondaryScreensSavers = new List<FormSecondaryScreenSaver>();
        private readonly Settings _Settings = new Settings(true);
        #endregion

        #region Protected Properties
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;

                if (!DesignMode && _IsPreview)
                {
                    createParams.Style |= 0x40000000; // Add the WS_CHILD style to preview mode...
                }

                return createParams;
            }
        }
        #endregion

        #region Public Properties
        public RssItemView RssItemView
        {
            get { return _RssItemView; }
        }
        #endregion

        #region Private Event Handlers
        // ReSharper disable InconsistentNaming
        private void _RssItemView_FadeComplete(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            if (_RssFeed.LastUpdated == DateTime.MinValue || (_Settings.RandomVerse && (now - _RssFeed.LastUpdated).TotalMinutes >= 10) || _RssFeed.LastUpdated.Day != now.Day)
            {
                RefreshRssFeed();
            }

            _RssItemView.Location = PositionRssItemView();
            
            Refresh();
            Activate();
        }

        private void _RssItemView_FadeTick(object sender, EventArgs e)
        {
            Invalidate(new Rectangle(_RssItemView.Location, _RssItemView.Size));
        }
        // ReSharper restore InconsistentNaming

        protected override void OnPaint(PaintEventArgs e)
        {
            _RssItemView.Paint(e);

            foreach (FormSecondaryScreenSaver secondaryScreenSaver in _SecondaryScreensSavers)
            {
                secondaryScreenSaver.Draw();
            }
        }
        #endregion

        #region Public Event Handlers
        public void FormScreenSaver_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_IsPreview)
            {
                Close();
            }
        }

        public void FormScreenSaver_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_IsPreview)
            {
                Close();
            }
        }

        public void FormScreenSaver_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_IsPreview)
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
                        FormSecondaryScreenSaver formSecondaryScreenSaver = new FormSecondaryScreenSaver
                                                                                {
                                                                                    StartPosition = FormStartPosition.Manual,
                                                                                    Location = screen.WorkingArea.Location,
                                                                                    ScreenSaver = this,
                                                                                    Size = new Size(screen.WorkingArea.Width, screen.WorkingArea.Height)
                                                                                };

                        //formSecondaryScreenSaver.Size = new Size(screen.WorkingArea.Width/2, screen.WorkingArea.Height/2);
                        formSecondaryScreenSaver.Show();

                        _SecondaryScreensSavers.Add(formSecondaryScreenSaver);
                    }
                }
            }
        }

        private void InitializeRssItemView()
        {
            _RssItemView.BackColor = Color.FromArgb(100, _Settings.BackgroundColor);
            _RssItemView.BorderColor = _Settings.BorderColor;
            _RssItemView.ForeColor = _Settings.ForegroundColor;

            _RssItemView.FadeDelay = _Settings.FadeDelay;
            _RssItemView.FadeSpeed = _Settings.FadeSpeed;

            _RssItemView.TitleFont = _Settings.TitleFont;
            _RssItemView.TextFont = _Settings.TextFont;
            _RssItemView.FadingTick += _RssItemView_FadeTick;
            _RssItemView.FadingComplete += _RssItemView_FadeComplete;

            _RssItemView.MaxWidth = Width/2;
            _RssItemView.SetSize(CreateGraphics());
            _RssItemView.Location = PositionRssItemView();
        }

        private Point PositionRssItemView()
        {
            int x = (Width >= _RssItemView.Size.Width) ? _Random.Next(0, Width - _RssItemView.Size.Width) : 0;
            int y = (Height >= _RssItemView.Size.Height) ? _Random.Next(0, Height - _RssItemView.Size.Height) : 0;
            return new Point(x, y);
        }

        private void RefreshRssFeed()
        {
            _RssFeed.Refresh();
            _RssItemView.Item = _RssFeed.Channels[0].Items[0];
            _RssItemView.SetSize(CreateGraphics());
        }

        // ReSharper disable SuggestBaseTypeForParameter
        private void SetupSecondaryScreenSaver(FormSecondaryScreenSaver formSecondaryScreenSaver)
        // ReSharper restore SuggestBaseTypeForParameter
        {
            formSecondaryScreenSaver.BackgroundImage = BackgroundImage;
            formSecondaryScreenSaver.BackgroundImageLayout = ImageLayout.Stretch;
            formSecondaryScreenSaver.Capture = true;
            formSecondaryScreenSaver.ShowInTaskbar = false;
            formSecondaryScreenSaver.TopMost = true;
        }

        private void SetupScreenSaver()
        {
            if (!string.IsNullOrEmpty(_Settings.BackgroundImage) && File.Exists(_Settings.BackgroundImage))
            {
                BackgroundImage = Image.FromFile(_Settings.BackgroundImage);
            }

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            BackgroundImageLayout = ImageLayout.Stretch;
            Capture = true;
            DoubleBuffered = true;
            ShowInTaskbar = false;

            if (!_IsPreview)
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
                //User32.SetWindowLong(Handle, -16, new IntPtr(User32.GetWindowLong(Handle, -16).ToInt64() | 0x40000000));
                User32.SetParent(Handle, _PreviewHandle);

                Rectangle parentRectangle;
                GetClientRect(_PreviewHandle, out parentRectangle);
                Size = parentRectangle.Size;

                Location = new Point(0, 0);
            }

            foreach (FormSecondaryScreenSaver formSecondaryScreenSaver in _SecondaryScreensSavers)
            {
                SetupSecondaryScreenSaver(formSecondaryScreenSaver);
            }
        }
        #endregion
    }
}

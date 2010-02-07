﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CC.Utilities.Rss;

namespace CC.Votd
{
    public partial class FormScreenSaver : Form
    {
        #region Contstructor
        public FormScreenSaver() : this(new Settings())
        {
            // Empty constructor
        }

        public FormScreenSaver(Settings settings)
        {
            InitializeComponent();
            Icon = Properties.Resources.Bible;

            _Settings = settings;

            CreateSecondaryScreenSavers();
            SetupScreenSaver();

            _RssFeed = new RssFeed((_Settings.RandomVerse) ? VERSE_RANDOM : VERSE_DAILY);
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
        private Point _MouseLocation;
        private readonly Random _Random = new Random();
        private readonly RssFeed _RssFeed;
        private readonly RssItemView _RssItemView;
        private readonly List<FormSecondaryScreenSaver> _SecondaryScreensSavers = new List<FormSecondaryScreenSaver>();
        private readonly Settings _Settings;
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
            Close();
        }

        public void FormScreenSaver_MouseDown(object sender, MouseEventArgs e)
        {
            Close();
        }

        public void FormScreenSaver_MouseMove(object sender, MouseEventArgs e)
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
            int x = _Random.Next(1, Width - _RssItemView.Size.Width);
            int y = _Random.Next(1, Height - _RssItemView.Size.Height);
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
            Capture = true;

            Cursor.Hide();
            Bounds = Screen.PrimaryScreen.Bounds;
            WindowState = FormWindowState.Maximized;
#if DEBUG
            TopMost = false;
#else
            TopMost = true;
#endif
            ShowInTaskbar = false;
            DoubleBuffered = true;
            BackgroundImageLayout = ImageLayout.Stretch;

            foreach (FormSecondaryScreenSaver formSecondaryScreenSaver in _SecondaryScreensSavers)
            {
                SetupSecondaryScreenSaver(formSecondaryScreenSaver);
            }
        }
        #endregion
    }
}

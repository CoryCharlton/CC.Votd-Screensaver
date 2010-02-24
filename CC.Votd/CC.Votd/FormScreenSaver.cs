using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using CC.Utilities;
using CC.Utilities.Rss;

namespace CC.Votd
{
    public partial class FormScreenSaver : Form
    {
        #region Constructor
        public FormScreenSaver()
        {
            InitializeComponent();
            Icon = Properties.Resources.Bible;

            SetupScreenSaver();
        }
        #endregion

        #region Private Fields
        private readonly Random _Random = new Random();
        private RssFeed _RssFeed;
        private RssItemView _RssItemView;
        #endregion

        #region Public Properties
        #endregion

        #region Private Event Handlers
        // ReSharper disable InconsistentNaming
        private void _RssItemView_FadeComplete(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            if (_RssFeed.LastUpdated == DateTime.MinValue || (Program.Settings.RandomVerse && (now - _RssFeed.LastUpdated).TotalMinutes >= 10) || _RssFeed.LastUpdated.Day != now.Day)
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
        #endregion

        #region Private Methods
        private void InitializeRssItemView()
        {
            _RssItemView.BackColor = Program.Settings.BackgroundColor;
            _RssItemView.BorderColor = Program.Settings.BorderColor;
            _RssItemView.ForeColor = Program.Settings.ForegroundColor;

            _RssItemView.FadeDelay = Program.Settings.FadeDelay;
            _RssItemView.FadeSpeed = Program.Settings.FadeSpeed;

            if (Program.Settings.IsPreview)
            {
                _RssItemView.MaxWidth = (int)(Width * .8);
                _RssItemView.Padding = new Padding(2);
                _RssItemView.TitleFont = new Font(Program.Settings.TitleFont.FontFamily.Name, Program.Settings.TitleFont.Size / 4, Program.Settings.TitleFont.Style, Program.Settings.TitleFont.Unit);
                _RssItemView.TextFont = new Font(Program.Settings.TextFont.FontFamily.Name, Program.Settings.TextFont.Size / 4, Program.Settings.TextFont.Style, Program.Settings.TextFont.Unit);
            }
            else
            {
                _RssItemView.MaxWidth = (int)(Width * .65);
                _RssItemView.Padding = new Padding(8);
                _RssItemView.TitleFont = Program.Settings.TitleFont;
                _RssItemView.TextFont = Program.Settings.TextFont;
            }

            _RssItemView.FadingTick += _RssItemView_FadeTick;
            _RssItemView.FadingComplete += _RssItemView_FadeComplete;

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
            Capture = true;
        }
        #endregion

        #region Protected Methods
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_RssItemView != null)
            {
                _RssItemView.Paint(e);
            }
        }

        protected override void  OnResize(EventArgs e)
        {
            if (_RssItemView != null)
            {
                _RssItemView.SetSize(CreateGraphics());
                _RssItemView.Location = PositionRssItemView();
            }
            base.OnResize(e);
        }
        #endregion

        #region Public Methods
        public void Initialize()
        {
            _RssFeed = new RssFeed(Program.Settings.RandomVerse ? Constants.VERSE_RANDOM : Constants.VERSE_DAILY);
            _RssItemView = new RssItemView(_RssFeed.Channels[0].Items[0]);

            InitializeRssItemView();
        }
        #endregion
    }
}

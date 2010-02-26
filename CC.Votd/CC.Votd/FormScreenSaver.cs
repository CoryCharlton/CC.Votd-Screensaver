using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
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

            if (_RssFeed.LastUpdated == DateTime.MinValue || (Settings.RandomVerse && (now - _RssFeed.LastUpdated).TotalMinutes >= 10) || _RssFeed.LastUpdated.Day != now.Day)
            {
                RefreshRssFeed();
            }

            //_RssItemView.Location = PositionRssItemView();
            _RssItemView.SetLocation(Size);
            Refresh();

            if (!Settings.IsPreview)
            {
                Activate();
            }
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
            _RssItemView.BackColor = Settings.BackgroundColor;
            _RssItemView.BorderColor = Settings.BorderColor;
            _RssItemView.ForeColor = Settings.ForegroundColor;

            _RssItemView.FadeDelay = Settings.FadeDelay;
            _RssItemView.FadeSpeed = Settings.FadeSpeed;

            if (Settings.IsPreview)
            {
                _RssItemView.MaxWidth = (int)(Width * .8);
                _RssItemView.Padding = new Padding(2);
                _RssItemView.TitleFont = new Font(Settings.TitleFont.FontFamily.Name, Settings.TitleFont.Size / 4, Settings.TitleFont.Style, Settings.TitleFont.Unit);
                _RssItemView.Font = new Font(Settings.TextFont.FontFamily.Name, Settings.TextFont.Size / 4, Settings.TextFont.Style, Settings.TextFont.Unit);
            }
            else
            {
                _RssItemView.MaxWidth = (int)(Width * .65);
                _RssItemView.Padding = new Padding(8);
                _RssItemView.TitleFont = Settings.TitleFont;
                _RssItemView.Font = Settings.TextFont;
            }

            _RssItemView.FadingTick += _RssItemView_FadeTick;
            _RssItemView.FadingComplete += _RssItemView_FadeComplete;

            _RssItemView.SetSize(CreateGraphics());
            //_RssItemView.Location = PositionRssItemView();
            _RssItemView.SetLocation(Size);
        }

        //private Point PositionRssItemView()
        //{
        //    int x = (Width >= _RssItemView.Size.Width) ? _Random.Next(0, Width - _RssItemView.Size.Width) : 0;
        //    int y = (Height >= _RssItemView.Size.Height) ? _Random.Next(0, Height - _RssItemView.Size.Height) : 0;
        //    return new Point(x, y);
        //}

        private void RefreshRssFeed()
        {
            _RssFeed.Refresh();
            
            SetRssItem();

            _RssItemView.SetSize(CreateGraphics());
        }

        private void SetRssItem()
        {
            _RssItemView.Item = _RssFeed.Channels[0].Items[0];

            if (!_RssFeed.IsError)
            {
                if (RssItemCache.Add(_RssItemView.Item))
                {
                    RssItemCache.Save();
                }
            }
            else
            {
                RssItem rssItem = RssItemCache.GetRandomItem();

                if (rssItem != null)
                {
                    _RssItemView.Item = rssItem;
                }
            }
        }

        private void SetupScreenSaver()
        {
            if (!string.IsNullOrEmpty(Settings.BackgroundImage) && File.Exists(Settings.BackgroundImage))
            {
                try
                {
                    BackgroundImage = Image.FromFile(Settings.BackgroundImage);
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

        protected override void OnResize(EventArgs e)
        {
            if (_RssItemView != null)
            {
                _RssItemView.SetSize(CreateGraphics());
                //_RssItemView.Location = PositionRssItemView();
                _RssItemView.SetLocation(Size);
            }
            base.OnResize(e);
        }
        #endregion

        #region Public Methods
        public void Initialize()
        {
            _RssFeed = new RssFeed(Settings.RandomVerse ? Constants.VERSE_RANDOM : Constants.VERSE_DAILY);
            _RssItemView = new RssItemView(_RssFeed.Channels[0].Items[0]);

            SetRssItem();
            InitializeRssItemView();
        }
        #endregion
    }
}
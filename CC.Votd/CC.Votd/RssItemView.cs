using System;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Drawing;
using CC.Utilities;
using CC.Utilities.Rss;

namespace CC.Votd
{
    public class RssItemView: IDisposable
    {
        #region Constructor
        public RssItemView(RssItem rssItem)
        {
            Item = rssItem;

            _FadeTimer = new Timer();
            _FadeTimer.Tick += _FadeTimer_Tick;
            _FadeTimer.Start();

            _StringFormat = new StringFormat(StringFormatFlags.LineLimit)
                                {
                                    Alignment = StringAlignment.Near,
                                    LineAlignment = StringAlignment.Near,
                                    Trimming = StringTrimming.EllipsisWord
                                };
        }
        #endregion

        #region Private Constants
        // ReSharper disable InconsistentNaming
        private const int ALPHA_MAX = 200;
        // ReSharper restore InconsistentNaming
        #endregion

        #region Private Fields
        private int _Alpha;
        private int _AlphaDelta = 4;
        private Timer _FadeTimer;
        private StringFormat _StringFormat;
        private Size _TextSize;
        private Size _TitleSize;
        #endregion

        #region Public Properties
        public Color BackColor { get; set; }

        public Color BorderColor { get; set; }

        public int FadeDelay { get; set; }

        public int FadeSpeed { get; set; }

        public Color ForeColor { get; set; }

        public RssItem Item { get; set; }

        public Point Location { get; set; }
        
        public int MaxWidth { get; set; }

        public Padding Padding { get; set; }

        public Size Size { get; private set; }

        public Font TextFont { get; set; }

        public Font TitleFont { get; set; }
        #endregion

        #region Public Events
        public event EventHandler FadingComplete;

        public event EventHandler FadingTick;
        #endregion

        #region Private Event Handlers
        // ReSharper disable InconsistentNaming
        private void _FadeTimer_Tick(object sender, EventArgs e)
        {
            _FadeTimer.Interval = Settings.FadeSpeed;
            _Alpha += _AlphaDelta;

            OnFadingTick();

            if (_Alpha >= ALPHA_MAX)
            {
                _AlphaDelta *= -1;
                _FadeTimer.Interval = Settings.FadeDelay;
            }
            else if (_Alpha <= 0)
            {
                OnFadingComplete();

                _Alpha = 0;
                _AlphaDelta *= -1;
            }
        }
        // ReSharper restore InconsistentNaming
        #endregion

        #region Private Methods
        private static Rectangle CreateShadowRectangle(Rectangle rectangle, int offset)
        {
            return new Rectangle(rectangle.Location.X + offset, rectangle.Location.Y + offset, rectangle.Width, rectangle.Height);
        }

        private void DrawBackground(Graphics graphics)
        {
            using (Brush backBrush = new SolidBrush(Color.FromArgb(_Alpha, BackColor)))
            {
                graphics.FillRectangle(backBrush, new Rectangle(Location.X, Location.Y, Size.Width, Size.Height));
            }
         
            using (Pen borderPen = new Pen(Color.FromArgb(_Alpha, BorderColor), 2))
            {
                graphics.DrawRectangle(borderPen, new Rectangle(Location.X, Location.Y, Size.Width - 1, Size.Height - 1));
            }
        }
        #endregion

        #region Protected Methods
        protected void OnFadingComplete()
        {
            if (FadingComplete != null)
            {
                FadingComplete(this, EventArgs.Empty);
            }
        }

        protected void OnFadingTick()
        {
            if (FadingTick != null)
            {
                FadingTick(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Public Methods
        public void Paint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            DrawBackground(e.Graphics);

            Rectangle textRectangle = new Rectangle(Location.X + Padding.Size.Width, Location.Y + Padding.Size.Height, _TextSize.Width, _TextSize.Height);
            Rectangle titleRectangle = new Rectangle(Location.X + Padding.Size.Width, Location.Y + Padding.Size.Height + _TextSize.Height + Padding.Size.Height, _TitleSize.Width, _TitleSize.Height);

            using (Brush textBrush = new SolidBrush(Color.FromArgb(_Alpha, Color.Black)))
            {
                e.Graphics.DrawString(Item.Title, TitleFont, textBrush, CreateShadowRectangle(titleRectangle, 1), _StringFormat);
                e.Graphics.DrawString(Item.Description, TextFont, textBrush, CreateShadowRectangle(textRectangle, 1), _StringFormat);
            }

            using (Brush textBrush = new SolidBrush(Color.FromArgb(_Alpha, ForeColor)))
            {
                e.Graphics.DrawString(Item.Title, TitleFont, textBrush, titleRectangle, _StringFormat);
                e.Graphics.DrawString(Item.Description, TextFont, textBrush, textRectangle, _StringFormat);
            }
        }

        public void Reset()
        {
            _Alpha = 0;
            _AlphaDelta = Math.Abs(_AlphaDelta);
            _FadeTimer_Tick(this, EventArgs.Empty);
        }

        public void SetSize(Graphics graphics)
        {
            SizeF textSize = graphics.MeasureString(" " + Item.Description + " ", TextFont, MaxWidth - (Padding.Size.Width * 2), _StringFormat);
            SizeF titleSize = graphics.MeasureString(" " + Item.Title + " ", TextFont, MaxWidth - (Padding.Size.Width * 2), _StringFormat);
            SizeF totalSize = new SizeF((textSize.Width > titleSize.Width ? textSize.Width : titleSize.Width) + (Padding.Size.Width * 2), textSize.Height + titleSize.Height + (Padding.Size.Height * 3));

            Logging.LogMessage("Text --- H: " + textSize.Height + " W: " + textSize.Width + " Max W: " + MaxWidth);
            Logging.LogMessage("Title -- H: " + titleSize.Height + " W: " + titleSize.Width + " Max W: " + MaxWidth);
            Logging.LogMessage("Total -- H: " + totalSize.Height + " W: " + totalSize.Width + " Max W: " + MaxWidth);
            Logging.LogMessage(Item.Title + " -- " + Item.Description);

            Size = new Size((int)totalSize.Width, (int)totalSize.Height);

            _TextSize = new Size((int)textSize.Width, (int)textSize.Height);
            _TitleSize = new Size((int)titleSize.Width, (int)titleSize.Height);
        }
        #endregion 

        #region IDisposable Members
        /// <summary>
        /// Dispose all disposable fields
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_FadeTimer != null)
                {
                    _FadeTimer.Dispose();
                    _FadeTimer = null;
                }

                if (_StringFormat != null)
                {
                    _StringFormat.Dispose();
                    _StringFormat = null;
                }
            }
        }
        #endregion
    }
}

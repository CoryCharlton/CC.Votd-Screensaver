using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CC.Utilities;

namespace CC.Votd
{
    public partial class FormSecondaryScreenSaver : Form
    {
        #region Constructor
        public FormSecondaryScreenSaver()
        {
            InitializeComponent();
            Icon = Properties.Resources.Bible;
        }
        #endregion

        #region Public Methods
        public void Draw(Image image)
        {
            using (Graphics graphics = CreateGraphics())
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(image, ClientRectangle, image.GetRectangle(), GraphicsUnit.Pixel);
            }
        }
        #endregion
    }
}

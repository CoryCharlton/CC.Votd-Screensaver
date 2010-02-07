using System.Drawing;
using System.Windows.Forms;

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

        #region Public Properties
        public FormScreenSaver ScreenSaver { get; set; }
        #endregion

        #region Private Event Handlers
        private void FormSecondaryScreenSaver_KeyDown(object sender, KeyEventArgs e)
        {
            ScreenSaver.FormScreenSaver_KeyDown(sender, e);
        }

        private void FormSecondaryScreenSaver_MouseDown(object sender, MouseEventArgs e)
        {
            ScreenSaver.FormScreenSaver_MouseDown(sender, e);
        }

        private void FormSecondaryScreenSaver_MouseMove(object sender, MouseEventArgs e)
        {
            ScreenSaver.FormScreenSaver_MouseMove(sender, e);
        }
        #endregion

        #region Public Methods
        public void Draw()
        {
            using (Graphics graphics = CreateGraphics())
            {
                graphics.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.WorkingArea.Size);
            }
        }
        #endregion
    }
}

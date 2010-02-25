﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace CC.Votd
{
    public partial class FormOptions : Form
    {
        #region Constructor
        public FormOptions()
        {
            InitializeComponent();
            Icon = Properties.Resources.Bible;
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Text = AssemblyProduct + " Settings";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

            _LabelProductName.Text = string.Format("{0} v {1}", AssemblyProduct, AssemblyVersion);
            _LabelCopyright.Text = AssemblyCopyright;
            _LinkLabelHomepage.Text = AssemblyCompany;


            _LinkLabelHomepage.Links.Remove(_LinkLabelHomepage.Links[0]);
            _LinkLabelHomepage.Links.Add(0, _LinkLabelHomepage.Text.Length, "http://www.ccswe.com/");
            _LinkLabelHomepage.Text += " (CodePlex Project)";
            _LinkLabelHomepage.Links.Add(AssemblyCompany.Length + 2, _LinkLabelHomepage.Text.Length - (AssemblyCompany.Length + 3), "http://ccvotd.codeplex.com/");

            _TextBoxDescription.Text = AssemblyDescription;

            ConfigToUI();
        }
        #endregion

        #region Private Fields
        private bool _IsLoaded;
        #endregion

        #region Assembly Attribute Accessors
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        #region Private Event Handlers
        // ReSharper disable InconsistentNaming
        private void _ButtonApply_Click(object sender, EventArgs e)
        {
            UIToConfig();
            UpdateApply();
        }

        private void _ButtonBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.CheckFileExists = true;
                openFileDialog.FileName = _TextBoxBackgroundImage.Text;
                openFileDialog.Filter = "Image Files (*.BMP,*.GIF,*.PNG,*.JPG,*.JPEG)|*.BMP;*.GIF;*.PNG;*.JPG;*.JPEG";
                openFileDialog.Multiselect = false;
                openFileDialog.ShowReadOnly = false;
                openFileDialog.Title = "Browse for background image...";
                openFileDialog.ShowDialog();
                if (File.Exists(openFileDialog.FileName))
                {
                    _TextBoxBackgroundImage.Text = openFileDialog.FileName;
                    _PictureBoxBackgroundImage.Image = Image.FromFile(openFileDialog.FileName);
                    UpdateApply();
                }
            }
        }

        private void _ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _ButtonOk_Click(object sender, EventArgs e)
        {
            UIToConfig();
            Close();
        }

        private void FormOptions_Load(object sender, EventArgs e)
        {
            _IsLoaded = true;
        }

        private void _LabelColor_Click(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            label.BackColor = ShowColorDialog(label.BackColor);
            UpdateApply();
        }

        private void _LabelFont_Click(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            label.Font = ShowFontDialog(label.Font);
            UpdateApply();
        }

        private void _LinkLabelHomepage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(e.Link.LinkData.ToString());
            Process.Start(processStartInfo);
        }

        private void _RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateApply();
        }

        private void _TrackBarFadeDelay_ValueChanged(object sender, EventArgs e)
        {
            string text = _TrackBarFadeDelay.Value > 1 ? "minutes" : "minute";
            _LabelFadeDelayValue.Text = string.Format("{0} {1}", _TrackBarFadeDelay.Value, text);
            UpdateApply();
        }

        private void _TrackBarFadeSpeed_ValueChanged(object sender, EventArgs e)
        {
            _LabelFadeSpeedValue.Text = string.Format("{0} milliseconds", _TrackBarFadeSpeed.Value * 10);
            UpdateApply();
        }
        // ReSharper restore InconsistentNaming
        #endregion

        #region Private Methods
        private void ConfigToUI()
        {
            _LabelBackgroundColorValue.BackColor = Settings.BackgroundColor;
            _LabelBorderColorValue.BackColor = Settings.BorderColor;
            _LabelForegroundColorValue.BackColor = Settings.ForegroundColor;
            
            if (Settings.RandomVerse)
            {
                _RadioButtonRandom.Checked = true;
            }
            else
            {
                _RadioButtonDaily.Checked = true;
            }

            _TrackBarFadeDelay.Value = Settings.FadeDelay / 60000;
            _TrackBarFadeSpeed.Value = Settings.FadeSpeed / 10;

            if (!string.IsNullOrEmpty(Settings.BackgroundImage) && File.Exists(Settings.BackgroundImage))
            {
                _PictureBoxBackgroundImage.Image = Image.FromFile(Settings.BackgroundImage);
                _TextBoxBackgroundImage.Text = Settings.BackgroundImage;
            }
            else
            {
                _PictureBoxBackgroundImage.Image = Properties.Resources.Cross;
            }

            _LabelTextFontValue.Font = Settings.TextFont;
            _LabelTitleFontValue.Font = Settings.TitleFont;
        }

        private bool IsDirty()
        {
            bool returnValue = false;

            if (_IsLoaded)
            {
                //Colors...
                if (Settings.BackgroundColor != _LabelBackgroundColorValue.BackColor)
                {
                    returnValue = true;
                }
                if (!returnValue && Settings.BorderColor != _LabelBorderColorValue.BackColor)
                {
                    returnValue = true;
                }
                if (!returnValue && Settings.ForegroundColor != _LabelForegroundColorValue.BackColor)
                {
                    returnValue = true;
                }

                //General...
                if (!returnValue && (Settings.FadeDelay / 60000) != _TrackBarFadeDelay.Value)
                {
                    returnValue = true;
                }
                if (!returnValue && (Settings.FadeSpeed / 10) != _TrackBarFadeSpeed.Value)
                {
                    returnValue = true;
                }
                if (!returnValue && Settings.RandomVerse != _RadioButtonRandom.Checked)
                {
                    returnValue = true;
                }

                // Background Image ...
                if (!returnValue && Settings.BackgroundImage != _TextBoxBackgroundImage.Text)
                {
                    returnValue = true;
                }

                // Fonts
                if (!returnValue && !Settings.TextFont.Equals(_LabelTextFontValue.Font))
                {
                    returnValue = true;
                }
                if (!returnValue && !Settings.TitleFont.Equals(_LabelTitleFontValue.Font))
                {
                    returnValue = true;
                }
            }

            return returnValue;
        }

        private static Color ShowColorDialog(Color defaultColor)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = defaultColor;
                colorDialog.AnyColor = true;
                colorDialog.FullOpen = true;
                colorDialog.ShowDialog();
                return colorDialog.Color;
            }
        }

        private static Font ShowFontDialog(Font defaultFont)
        {
            using (FontDialog fontDialog = new FontDialog())
            {
                fontDialog.AllowScriptChange = false;
                fontDialog.Font = defaultFont;
                fontDialog.FontMustExist = true;
                fontDialog.ShowApply = false;
                fontDialog.ShowColor = false;
                fontDialog.ShowEffects = true;
                fontDialog.ShowHelp = false;
                fontDialog.ShowDialog();
                return fontDialog.Font;
            }
        }

        private void UIToConfig()
        {

            Settings.BackgroundColor = _LabelBackgroundColorValue.BackColor;
            Settings.BorderColor = _LabelBorderColorValue.BackColor;
            Settings.ForegroundColor = _LabelForegroundColorValue.BackColor;

            Settings.FadeDelay = _TrackBarFadeDelay.Value * 60000;
            Settings.FadeSpeed = _TrackBarFadeSpeed.Value * 10;

            Settings.RandomVerse = _RadioButtonRandom.Checked;

            Settings.BackgroundImage = _TextBoxBackgroundImage.Text;

            Settings.TextFont = _LabelTextFontValue.Font;
            Settings.TitleFont = _LabelTitleFontValue.Font;

            Settings.Save();
        }

        private void UpdateApply()
        {
            _ButtonApply.Enabled = IsDirty();
        }
        #endregion
    }
}

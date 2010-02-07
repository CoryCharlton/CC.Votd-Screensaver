using System;
using System.Drawing;
using CC.Utilities;
using Microsoft.Win32;

namespace CC.Votd
{
    public class Settings
    {
        #region Constructor
        public Settings() : this(true)
        {
            // Empty constructor
        }

        public Settings(bool loadSettings)
        {
            Reset();

            if (loadSettings)
            {
                Load();
            }
        }
        #endregion

        #region Private Constants
        // ReSharper disable InconsistentNaming
        private const string BACKGROUND_COLOR = "BackgroundColor";
        private const string BACKGROUND_IMAGE = "BackgroundImage";
        private const string BORDER_COLOR = "BorderColor";
        private const string FADE_DELAY = "FadeDelay";
        private const string FADESPEED = "FadeSpeed";
        private const string FOREGROUND_COLOR = "ForegroundColor";
        private const string RANDOM_VERSE = "RandomVerse";
        private const string TEXT_FONT = "TextFont";
        private const string TITLE_FONT = "TitleFont";

        private const string REGISTRY_KEY = @"Software\CC.Votd";
        // ReSharper restore InconsistentNaming
        #endregion

        #region Private Fields
        // ReSharper disable InconsistentNaming
        private readonly Color _DefaultBackgroundColor = Color.FromArgb(0, 128, 192);
        private readonly string _DefaultBackgroundImage = string.Empty;
        private readonly Color _DefaultBorderColor = Color.FromArgb(0, 0, 128);
        private readonly Color _DefaultForegroundColor = Color.White;
        private const int _DefaultFadeDelay = 60000;
        private const int _DefaultFadeSpeed = 50;
        private const bool _DefaultRandomVerse = false;
        private readonly Font _DefaultTextFont = new Font("Papyrus", 24, FontStyle.Regular, GraphicsUnit.Pixel);
        private readonly Font _DefaultTitleFont = new Font("Papyrus", 24, FontStyle.Italic, GraphicsUnit.Pixel);
        // ReSharper restore InconsistentNaming
        #endregion

        #region Public Properties
        public Color BackgroundColor { get; set; }

        public string BackgroundImage { get; set; }

        public Color BorderColor { get; set; }

        public Color ForegroundColor { get; set; }

        public int FadeDelay { get; set; }

        public int FadeSpeed { get; set; }

        public bool RandomVerse { get; set; }

        public Font TextFont { get; set; }

        public Font TitleFont { get; set; }
        #endregion

        #region Private Methods
        private static RegistryKey OpenRegistryKey()
        {
            return (Registry.LocalMachine.CreateSubKey(REGISTRY_KEY));
        }
        #endregion

        #region Public Methods
        public bool Load()
        {
            bool returnValue;

            try
            {
                using (RegistryKey registryKey = OpenRegistryKey())
                {
                    BackgroundColor = Color.FromArgb((int)registryKey.GetValue(BACKGROUND_COLOR, _DefaultBackgroundColor.ToArgb()));
                    BackgroundImage = registryKey.GetValue(BACKGROUND_IMAGE, _DefaultBackgroundImage).ToString();
                    BorderColor = Color.FromArgb((int)registryKey.GetValue(BORDER_COLOR, _DefaultBorderColor.ToArgb()));
                    FadeDelay = (int)registryKey.GetValue(FADE_DELAY, _DefaultFadeDelay);
                    FadeSpeed = (int)registryKey.GetValue(FADESPEED, _DefaultFadeSpeed);
                    ForegroundColor = Color.FromArgb((int)registryKey.GetValue(FOREGROUND_COLOR, _DefaultForegroundColor.ToArgb()));
                    RandomVerse = bool.Parse(registryKey.GetValue(RANDOM_VERSE, _DefaultRandomVerse).ToString());
                    TextFont = FontBuilder.FromString(registryKey.GetValue(TEXT_FONT, _DefaultTextFont.ToStringEx()).ToString());
                    TitleFont = FontBuilder.FromString(registryKey.GetValue(TITLE_FONT, _DefaultTitleFont.ToStringEx()).ToString());
                }

                returnValue = true;
            }
            catch (Exception e)
            {
                Logging.LogException(e);
                returnValue = false;
            }

            return returnValue;
        }

        public void Reset()
        {
            BackgroundColor = _DefaultBackgroundColor;
            BackgroundImage = _DefaultBackgroundImage;
            BorderColor = _DefaultBorderColor;
            FadeDelay = _DefaultFadeDelay;
            FadeSpeed = _DefaultFadeSpeed;
            ForegroundColor = _DefaultForegroundColor;
            RandomVerse = _DefaultRandomVerse;
            TextFont = _DefaultTextFont;
            TitleFont = _DefaultTitleFont;
        }

        public bool Save()
        {
            bool returnValue;

            try
            {
                using (RegistryKey registryKey = OpenRegistryKey())
                {
                    registryKey.SetValue(BACKGROUND_COLOR, BackgroundColor.ToArgb(), RegistryValueKind.DWord);
                    registryKey.SetValue(BACKGROUND_IMAGE, BackgroundImage, RegistryValueKind.String);
                    registryKey.SetValue(BORDER_COLOR, BorderColor.ToArgb(), RegistryValueKind.DWord);
                    registryKey.SetValue(FADE_DELAY, FadeDelay, RegistryValueKind.DWord);
                    registryKey.SetValue(FADESPEED, FadeSpeed, RegistryValueKind.DWord);
                    registryKey.SetValue(FOREGROUND_COLOR, ForegroundColor.ToArgb(), RegistryValueKind.DWord);
                    registryKey.SetValue(RANDOM_VERSE, RandomVerse.ToString(), RegistryValueKind.String);
                    registryKey.SetValue(TEXT_FONT, TextFont.ToStringEx());
                    registryKey.SetValue(TITLE_FONT, TitleFont.ToStringEx());
                    registryKey.Close();
                }      
          
                returnValue = true;
            }
            catch (Exception e)
            {
                Logging.LogException(e);
                returnValue = false;
            }

            return returnValue;
        }
        #endregion
    }
}

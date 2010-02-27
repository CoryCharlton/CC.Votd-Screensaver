using System;
using System.Drawing;
using CC.Utilities;
using CC.Utilities.Drawing;
using Microsoft.Win32;

namespace CC.Votd
{
    public static class Settings
    {
        #region Constructor
        static Settings()
        {
            Reset();
            Load();
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
        private const string MAXIMUM_CACHE_ITEMS = "MaximumCacheItems";
        private const string RANDOM_VERSE = "RandomVerse";
        private const string TEXT_FONT = "TextFont";
        private const string TITLE_FONT = "TitleFont";

        private const string REGISTRY_KEY = @"Software\CC.Votd";
        // ReSharper restore InconsistentNaming
        #endregion

        #region Private Fields
        // ReSharper disable InconsistentNaming
        private static readonly Color _DefaultBackgroundColor = Color.FromArgb(0, 128, 192);
        private static readonly string _DefaultBackgroundImage = string.Empty;
        private static readonly Color _DefaultBorderColor = Color.FromArgb(0, 0, 128);
        private static readonly Color _DefaultForegroundColor = Color.White;
        private const int _DefaultFadeDelay = 60000;
        private const int _DefaultFadeSpeed = 20;
        private const int _DefaultMaximumCacheItems = 250;
        private const bool _DefaultRandomVerse = false;

        // NOTE: The only font I like here is Papyrus but not everyone will have it. Need better default fallback choices although the default fonts mostly suck :-)
        private static readonly Font _DefaultTextFont = FontBuilder.FromFontNames("Papyrus, Palatino Linotype, Tahoma", 24, FontStyle.Regular, GraphicsUnit.Pixel);
        private static readonly Font _DefaultTitleFont = FontBuilder.FromFontNames("Papyrus, Palatino Linotype, Tahoma", 24, FontStyle.Italic, GraphicsUnit.Pixel);
        // ReSharper restore InconsistentNaming
        #endregion

        #region Public Properties
        public static Color BackgroundColor { get; set; }

        public static string BackgroundImage { get; set; }

        public static Color BorderColor { get; set; }

        public static Color ForegroundColor { get; set; }

        public static int FadeDelay { get; set; }

        public static int FadeSpeed { get; set; }

        public static bool IsDebug { get; set; }

        public static bool IsPreview { get; set; }

        public static int MaximumCacheItems { get; set; }

        public static bool RandomVerse { get; set; }

        public static Font TextFont { get; set; }

        public static Font TitleFont { get; set; }
        #endregion

        #region Private Methods
        private static RegistryKey OpenRegistryKey()
        {
            return (Registry.LocalMachine.CreateSubKey(REGISTRY_KEY));
        }
        #endregion

        #region Public Methods
        public static bool Load()
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
                    MaximumCacheItems = (int)registryKey.GetValue(MAXIMUM_CACHE_ITEMS, _DefaultMaximumCacheItems);
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

        public static void Reset()
        {
            BackgroundColor = _DefaultBackgroundColor;
            BackgroundImage = _DefaultBackgroundImage;
            BorderColor = _DefaultBorderColor;
            FadeDelay = _DefaultFadeDelay;
            FadeSpeed = _DefaultFadeSpeed;
            ForegroundColor = _DefaultForegroundColor;
            MaximumCacheItems = _DefaultMaximumCacheItems;
            RandomVerse = _DefaultRandomVerse;
            TextFont = _DefaultTextFont;
            TitleFont = _DefaultTitleFont;
        }

        public static bool Save()
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
                    registryKey.SetValue(MAXIMUM_CACHE_ITEMS, MaximumCacheItems, RegistryValueKind.DWord);
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

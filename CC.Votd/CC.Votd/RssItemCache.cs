using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using CC.Utilities;
using CC.Utilities.Rss;

namespace CC.Votd
{
    public static class RssItemCache
    {
        #region Constructor
        static RssItemCache()
        {
            Load();
        }
        #endregion

        #region Private Fields
        private static RssItem _DailyItem;
        private static DateTime _DailyItemUpdated;
        private static readonly string _Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CC.Votd.xml");
        private static List<RssItem> _Items = new List<RssItem>();
        private static readonly object _LockObject = new object();
        private readonly static Random _Random = new Random();
        #endregion

        #region Public Properties
        public static List<RssItem> Items
        {
            get { return _Items; }
            set { _Items = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of items in the cache. If this value is less than or equal to 0 no maximum will be applied.
        /// </summary>
        [XmlIgnore]
        public static int Maximum { get; set; }
        #endregion

        #region Private Methods
        private static void SetDailyItem(RssItem rssItem, DateTime updated)
        {
            if (rssItem != null && (rssItem != _DailyItem || updated.Day != _DailyItemUpdated.Day))
            {
                _DailyItem = rssItem;
                _DailyItemUpdated = updated;
            }
        }
        #endregion

        #region Public Methods
        public static bool Add(RssItem rssItem, bool isDaily)
        {
            bool returnValue = false;

            if (rssItem != null)
            {
                lock (_LockObject)
                {
                    while (Maximum > 0 && _Items.Count >= Maximum)
                    {
                        _Items.RemoveAt(0);
                    }

                    if (isDaily)
                    {
                        SetDailyItem(rssItem, DateTime.Now);
                    }

                    if (!_Items.Contains(rssItem))
                    {
                        _Items.Add(rssItem);

                        returnValue = true;
                    }
                }
            }

            return returnValue;
        }

        public static RssItem GetDailyItem()
        {
            lock (_LockObject)
            {
                if (_DailyItem == null || _DailyItemUpdated.Day != DateTime.Now.Day)
                {
                    SetDailyItem(GetRandomItem(), DateTime.Now);
                }
            }

            return _DailyItem;
        }

        public static RssItem GetRandomItem()
        {
            RssItem returnValue = null;

            if (_Items.Count > 0)
            {
                returnValue = _Items[_Random.Next(0, _Items.Count)];
            }

            return returnValue;
        }

        public static void Load()
        {
            // Load a default verse so the screensaver always works
            _Items = new List<RssItem> {new RssItem {Description = "No temptation has seized you except what is common to man. And God is faithful; he will not let you be tempted beyond what you can bear. But when you are tempted, he will also provide a way out so that you can stand up under it.", Title = "1 Corinthians 10:13"}};
            
            if (File.Exists(_Filename))
            {
                string xmlString = File.ReadAllText(_Filename);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<RssItem>));
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                    {
                        streamWriter.Write(xmlString);
                        streamWriter.Flush();
                        memoryStream.Flush();
                        memoryStream.Position = 0;

                        List<RssItem> tempItems = (xmlSerializer.Deserialize(memoryStream) as List<RssItem>);

                        if (tempItems != null)
                        {
                            _Items = tempItems;
                        }
                    }
                }
            }
        }

        public static void Save()
        {
            File.WriteAllText(_Filename, _Items.ToXml());
        }
        #endregion
    }
}

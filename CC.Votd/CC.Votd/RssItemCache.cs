﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
        private static readonly string _Filename = Path.Combine(Application.StartupPath, "CC.Votd.xml");
        private static List<RssItem> _Items = new List<RssItem>();
        private readonly static Random _Random = new Random();
        #endregion

        #region Public Properties
        public static List<RssItem> Items
        {
            get { return _Items; }
            set { _Items = value; }
        }

        /// <summary>
        /// Controls the maximum number of items in the cache. If this value is less than or equal to 0 no maximum will be applied.
        /// </summary>
        [XmlIgnore]
        public static int MaximumItems { get; set; }
        #endregion

        #region Private Methods
        private static void ManageMaxItems()
        {
            while (MaximumItems > 0 && _Items.Count >= MaximumItems)
            {
                _Items.RemoveAt(0);
            }
        }
        #endregion

        #region Public Methods
        public static bool Add(RssItem rssItem)
        {
            bool returnValue = false;

            ManageMaxItems();

            if (!_Items.Contains(rssItem))
            {
                _Items.Add(rssItem);

                returnValue = true;
            }

            return returnValue;
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
            _Items = new List<RssItem>();

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

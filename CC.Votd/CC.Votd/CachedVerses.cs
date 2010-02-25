using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CC.Utilities;

namespace CC.Votd
{
    [XmlRoot("CachedVerses")]
    public class CachedVerses : SerializableDictionary<string, string>
    {

    }
}

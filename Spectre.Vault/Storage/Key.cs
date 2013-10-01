using System.IO;
using Scylla.XmlEngine;
using Spectre.Common;

namespace Spectre.Vault.Storage
{
    [XmlElement("KeyFile")]
    public class KeyFile : XmlStorage
    {
        private readonly static KeyFile _instance = new KeyFile();

        public static KeyFile Instance { get { return _instance; }}

        public KeyFile()
            : base(Path.Combine(SettingsDirectory, "Key.xml"))
        { }

        [XmlElement("Key")]
        public string Key { get; set; }

        [XmlElement("IV")]
        public string Iv { get; set; }
    }
}

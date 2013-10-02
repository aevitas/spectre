using System.Collections.Generic;
using System.IO;
using Scylla.XmlEngine;
using Spectre.Common;

namespace Spectre.Vault.Storage
{
    [XmlElement("Credentials")]
    internal class Credentials : XmlStorage
    {
        private readonly static Credentials _instance = new Credentials();

        public static Credentials Instance { get { return _instance; } }

        public Credentials()
            : base(Path.Combine(SettingsDirectory, "Credentials.xml"))
        {
            if (Entries == null)
                Entries = new List<EncryptedCredentials>();
        }

        [XmlElement("Entries")]
        public List<EncryptedCredentials> Entries { get; set; }
    }
}

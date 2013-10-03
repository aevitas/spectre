using System.Collections.Generic;
using System.IO;
using System.Xml;
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

        public override void Save()
        {
            //Entries = CredentialManager.Entries.Count > 0 ? new List<EncryptedCredentials>(CredentialManager.Entries)
            //    : new List<EncryptedCredentials>() { new EncryptedCredentials("Delete me", "Some user", "Some password") };

            base.Save();
        }
    }
}

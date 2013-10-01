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
                Entries = new List<CredentialPair>();
        }

        [XmlElement("Entries")]
        public List<CredentialPair> Entries { get; set; }
    }

    [XmlElement("CredentialPair")]
    public class CredentialPair
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Username")]
        public string Username { get; set; }

        [XmlElement("Password")]
        public string Password { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialPair"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        public CredentialPair(string name, string user, string password)
        {
            Name = name;
            Username = user;
            Password = password;
        }

        public override bool Equals(object obj)
        {
            var other = obj as CredentialPair;

            if (other != null)
                return Name == other.Name && Username == other.Username;

            return false;
        }
    }
}

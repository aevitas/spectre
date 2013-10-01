using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class CredentialPair : IEquatable<CredentialPair>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialPair"/> class. Mainly used by the Activator; we don't actually use this.
        /// </summary>
        public CredentialPair()
        {
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CredentialPair) obj);
        }

        public bool Equals(CredentialPair other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && string.Equals(Username, other.Username) && string.Equals(Password, other.Password);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Username != null ? Username.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Password != null ? Password.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}

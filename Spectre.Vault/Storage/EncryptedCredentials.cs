using System;
using Scylla.XmlEngine;

namespace Spectre.Vault.Storage
{
    [XmlElement("EncryptedCredentials")]
    public class EncryptedCredentials : IEquatable<EncryptedCredentials>
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Username")]
        public string Username { get; set; }

        [XmlElement("Password")]
        public string Password { get; set; }

        public EncryptedCredentials(string name, string username, string password)
        {
            Name = name;
            Username = username;
            Password = password;
        }

        public EncryptedCredentials()
        {
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != this.GetType()) return false;
            return Equals((EncryptedCredentials) other);
        }

        public bool Equals(EncryptedCredentials other)
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

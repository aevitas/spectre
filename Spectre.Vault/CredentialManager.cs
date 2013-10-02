using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Common;
using Spectre.Vault.Encryption;
using Spectre.Vault.Storage;

namespace Spectre.Vault
{
    /// <summary>
    /// Manages credentials. Quite simply.
    /// </summary>
    public static class CredentialManager
    {
        private static readonly List<EncryptedCredentials> Entries = new List<EncryptedCredentials>();

        public delegate void UpdateDelegate();

        public static event UpdateDelegate OnReloadCompleted;

        static CredentialManager()
        {
            Reload();
        }

        public static void Reload()
        {
            Entries.Clear();

            var entries = Credentials.Instance.Entries;

            // If there's no credentials in the list, we probably don't have to continue.
            if (entries.Count == 0)
                return;

            foreach (var c in entries)
            {
                Entries.Add(c);
            }

            Logging.WriteDiagnostic("[CredentialManager] Reload completed; {0} items in the list after reloading.", Entries.Count);

            InvokeOnReloadCompleted();
        }

        /// <summary>
        /// Gets the credentials.
        /// </summary>
        /// <param name="pair">The pair.</param>
        /// <returns></returns>
        public static DecryptedCredentials GetCredentials(EncryptedCredentials pair)
        {
            var user = Rijndael.Instance.DecryptFromBase64(pair.Username);
            var pass = Rijndael.Instance.DecryptFromBase64(pair.Password);

            return new DecryptedCredentials(pair.Name, user, pass);
        }

        /// <summary>
        /// Gets the credentials.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static DecryptedCredentials GetCredentials(string name)
        {
            return GetCredentials(Entries.FirstOrDefault(e => e.Name == name));
        }

        /// <summary>
        /// Gets the credentials.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static DecryptedCredentials GetCredentials(int index)
        {
            try
            {
                var entry = Entries[index];

                return GetCredentials(entry.Name);
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets the display entries.
        /// </summary>
        /// <returns></returns>
        public static List<EncryptedCredentials> GetDisplayEntries()
        {
            return (from c in Entries let decrypted = GetCredentials(c.Name) let displayUsername = decrypted.Username.Substring(0, 3) + "***" select new EncryptedCredentials(c.Name, displayUsername, string.Empty)).ToList();
        }

        /// <summary>
        /// Adds the credential.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <exception cref="System.Exception">[CredentialManager] Can't add a credential without valid values!</exception>
        public static void AddCredentials(string name, string username, string password)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new Exception("[CredentialManager] Can't add a credential without valid values!");

            username = Rijndael.Instance.EncryptToBase64(username);
            password = Rijndael.Instance.EncryptToBase64(password);

            var newEntry = new EncryptedCredentials(name, username, password);

            Credentials.Instance.Entries.Add(newEntry);
            Entries.Add(newEntry);

            Logging.WriteDiagnostic("[CredentialManager] Added credentials for user {0} with name {1}. Reloading the CredentialManager.", username, name);

            Credentials.Instance.Save();

            Reload();
        }

        private static void InvokeOnReloadCompleted()
        {
            UpdateDelegate handler = OnReloadCompleted;
            if (handler != null) handler();
        }
    }

    public class DecryptedCredentials
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public DecryptedCredentials(string name, string username, string password)
        {
            Name = name;
            Username = username;
            Password = password;
        }
    }
}

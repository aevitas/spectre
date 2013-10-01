using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Spectre.Common;
using Spectre.Vault.Storage;

namespace Spectre.Vault.Encryption
{
    public class Rijndael
    {
        private static readonly RijndaelManaged RijndaelManaged = new RijndaelManaged();
        private static Rijndael _instance;

        public static Rijndael Instance { get { return _instance ?? (_instance = new Rijndael()); }}

        public Rijndael()
        {
            // If there's no values currently in the keyfile for Key or IV, generate them
            // and store them in the XML file. We'll be using them from this point on.
            if (string.IsNullOrEmpty(KeyFile.Instance.Key))
            {
                RijndaelManaged.GenerateKey();
                KeyFile.Instance.Key = Convert.ToBase64String(RijndaelManaged.Key);
            }

            if (string.IsNullOrEmpty(KeyFile.Instance.Iv))
            {
                RijndaelManaged.GenerateIV();
                KeyFile.Instance.Iv = Convert.ToBase64String(RijndaelManaged.IV);
            }

            // Assign the Key and IV to the Rijndael instance, converting them from the Base64 string we store.
            RijndaelManaged.Key = Convert.FromBase64String(KeyFile.Instance.Key);
            RijndaelManaged.IV = Convert.FromBase64String(KeyFile.Instance.Iv);
        }

        /// <summary>
        /// Encrypts the specified source using the current Rijndael instance.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public byte[] Encrypt(string source)
        {
            var crypto = RijndaelManaged.CreateEncryptor();
            var memStream = new MemoryStream();

            using (var s = new CryptoStream(memStream, crypto, CryptoStreamMode.Write))
            {
                using (var sw = new StreamWriter(s))
                    sw.Write(source);
            }

            return memStream.ToArray();
        }

        /// <summary>
        /// Encrypts to base64.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public string EncryptToBase64(string source)
        {
            return Convert.ToBase64String(Encrypt(source));
        }

        /// <summary>
        /// Decrypts the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public string Decrypt(byte[] source)
        {
            var crypto = RijndaelManaged.CreateDecryptor();
            var memStream = new MemoryStream(source);

            using (var s = new CryptoStream(memStream, crypto, CryptoStreamMode.Read))
            {
                using (var sr = new StreamReader(s))
                    sr.ReadToEnd();
            }

            return Encoding.ASCII.GetString(memStream.ToArray());
        }

        /// <summary>
        /// Decrypts from base64.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public string DecryptFromBase64(string source)
        {
            return Decrypt(Convert.FromBase64String(source));
        }
    }
}

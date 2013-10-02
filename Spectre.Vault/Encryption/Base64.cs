using System;
using System.Text;

namespace Spectre.Vault.Encryption
{
    /// <summary>
    /// Base64 helper.
    /// </summary>
    internal static class Base64
    {
        /// <summary>
        /// Encodes the specified source to a base64 string.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        internal static string Encode(string source)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(source));
        }

        /// <summary>
        /// Decodes the specified source base64 string to an ASCII string.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        internal static string Decode(string source)
        {
            var bytes = Convert.FromBase64String(source);

            return Encoding.ASCII.GetString(bytes);
        }
    }
}

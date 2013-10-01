using System;

namespace Spectre.Common
{
    /// <summary>
    /// Provides support for a Singleton instance of any class that inherits us.
    /// </summary>
    /// <typeparam name="T">The encosing type; this is the type that Lazy will actually construct for us.</typeparam>
    public abstract class Singleton<T> where T : class, new()
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(() => new T());

        /// <summary>
        /// Obtains a singleton instance of this object.
        /// </summary>
        public static T Instance { get { return _instance.Value; } }
    }
}

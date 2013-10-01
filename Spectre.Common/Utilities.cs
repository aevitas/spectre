using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Spectre.Common
{
    /// <summary>
    /// A class full of handy stuff!
    /// </summary>
    public static class Utilities
    {
        /// <summary> Gets the pathname of the assembly directory. </summary>
        /// <value> The pathname of the assembly directory. </value>
        public static string AssemblyDirectory
        {
            get { return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location); }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public static Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

        /// <summary>
        /// Converts a unix timestamp to a <see cref="DateTime"/> .
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns></returns>
        /// <remarks>Created 2012-04-24</remarks>
        public static DateTime ConvertFromUnixTimestamp(ulong timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> to a unix timestamp.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        /// <remarks>Created 2012-04-24</remarks>
        public static int ConvertToUnixTimestamp(DateTime date)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return (int)diff.TotalSeconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="performanceCounter">The performance counter.</param>
        /// <returns></returns>
        /// <remarks>Created 2012-04-24</remarks>
        public static DateTime PerformanceCounterToDateTime(ulong performanceCounter)
        {
            return
                DateTime.Now.Subtract(TimeSpan.FromMilliseconds(Environment.TickCount))
                        .AddMilliseconds(performanceCounter);
        }

        /// <summary> Returns true if the current OS is Windows XP. </summary>
        public static bool IsWindowsXp
        {
            get { return Environment.OSVersion.Version.Major == 5; }
        }

        /// <summary>
        /// Gets a string for an object. If the object is null, it returns <paramref name="nullRet"/>. If it's nonnull, it returns obj.ToString().
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="nullRet"></param>
        /// <returns></returns>
        public static string GetObjectString(object obj, string nullRet)
        {
            return obj != null ? obj.ToString() : nullRet;
        }

        public static string FormatString([Localizable(true)] string format, params object[] args)
        {
            try
            {
                return string.Format(Thread.CurrentThread.CurrentUICulture, format, args);
            }
            catch (Exception e)
            {
                return string.Format("FS_EMPTY {0} {1}", e.GetType().Name, format.ToString());
            }
        }

        internal static string BuildCompilerErrorsString(CompilerResults results)
        {
            var errors = new StringBuilder();
            foreach (CompilerError error in results.Errors)
            {
                if (error.IsWarning)
                    continue;

                errors.AppendLine(
                    string.Format(
                        "File: {0} Line: {1} {3}: {2}",
                        Path.GetFileName(error.FileName),
                        error.Line,
                        error.ErrorText,
                        error.IsWarning ? "Warning" : "Error"));
            }

            return errors.ToString();
        }

        public static int FNV1a(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            return
                (int)
                (bytes[3] ^
                 0x1000193 *
                 (bytes[2] ^ 0x1000193 * (bytes[1] ^ 0x1000193 * (bytes[0] ^ 0x1000193 * ((uint)value ^ 0x811C9DC5)))));
        }
    }
}

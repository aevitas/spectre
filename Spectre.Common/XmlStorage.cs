using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Scylla.XmlEngine;

namespace Spectre.Common
{
    public abstract class XmlStorage
    {
        private readonly XmlEngine _engine = new XmlEngine();

        protected XmlStorage(string path)
        {
            SettingsPath = path;
            _engine = new XmlEngine();

            // First set defaults. This way our settings are "resistant" to new settings - so new settings that doesn't exist in the settings file
            // gets a default value, while the old ones are overwritten. 
            SetDefaults();

            // Blah blah, load the xml file and let the engine parse it out to the class structure.
            if (File.Exists(path))
                _engine.Load(this, XElement.Load(path, LoadOptions.SetLineInfo));
            else
            {
                // Force a save with default values. Ensure we have the file on disk for the next time we load.
                Save();
            }
        }

        private void SetDefaults()
        {
            // Check any properties that are defined in the XML, and have a default value attr.
            const BindingFlags searchFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                             BindingFlags.GetProperty | BindingFlags.SetProperty;
            foreach (var prop in GetType().GetProperties(searchFlags).Where(
                p =>
                p.GetCustomAttributes(typeof(DefaultValueAttribute), true).Length != 0 &&
                p.GetCustomAttributes(typeof(XmlElementAttribute), true).Length != 0))
            {
                var defaultVal = prop.GetCustomAttributes(typeof(DefaultValueAttribute), true)[0] as DefaultValueAttribute;
                if (defaultVal != null)
                {
                    // NOTE: This is throwing up an exception on GlobalSettings.LogLevel (Buddy.Common.LogLevel enum) after obfuscation. 
                    //      Its still looking for the Buddy.Common.dll after merging for some reason.
                    //      Once the settings file is available, this code isn't run, so it doesn't crash the client. 
                    //      Really need to look into this. Or just change the saved setting to a string, 
                    //      and just parse it in the enum property to avoid the issue all together.
                    prop.SetValue(this, defaultVal.Value, null);
                }
            }
        }

        public static string SettingsDirectory
        {
            get
            {
                string ret = Path.Combine(Utilities.AssemblyDirectory, "Storage");
                if (!Directory.Exists(ret))
                {
                    Directory.CreateDirectory(ret);
                }
                return ret;
            }
        }

        public string SettingsPath { get; private set; }

        public virtual void Save()
        {
            if (!File.Exists(SettingsPath))
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));

            _engine.Save(this).Save(SettingsPath);
        }

        ~XmlStorage()
        {
            Save();
        }
    }
}

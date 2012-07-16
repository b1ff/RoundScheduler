using System;
using System.IO;
using System.Xml.Serialization;
using RoundScheduler.Model;

namespace RoundScheduler.Utils
{
    public static class SettingsManager
    {
        static SettingsManager()
        {
            settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml");
            settingsSerializer = new XmlSerializer(typeof(Settings));
        }

        private static readonly string settingsPath;

        private static readonly XmlSerializer settingsSerializer;

        private static Settings settings;

        public static Settings CurrentSettings 
        { 
            get
            {
                if (settings == null)
                {
                    settings = GetSettingsFromFile();
                }

                return settings;
            }
        }

        public static void SaveAsCurrentSettings(Settings newSettings)
        {
            using (var settingsFileStream = File.Open(settingsPath, FileMode.Create))
            {
                settingsSerializer.Serialize(settingsFileStream, newSettings);
                settings = newSettings;
            }
        }

        private static Settings GetSettingsFromFile()
        {
            CreateSettingsFileIfNotExist();

            using (var settingsFile = new FileStream(settingsPath, FileMode.Open))
            {
                settings = (Settings)settingsSerializer.Deserialize(settingsFile);
            }

            return settings;
        }

        private static void CreateSettingsFileIfNotExist()
        {
            if (!File.Exists(settingsPath))
            {
                using (var createdFileStream = File.Create(settingsPath))
                {
                    var settingsToSave = new Settings();
                    settingsToSave.SoundFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ring.wav");
                    settingsSerializer.Serialize(createdFileStream, settingsToSave);
                }
            }
        }
    }
}

using NLog;
using System;
using System.IO;
using System.Xml.Serialization;

namespace WeatherApp.Core.Models.ProgramSettings
{
    public interface IProgramSettings
    {
        string Culture { get; set; }
        int Theme { get; set; }
        bool LoadSettings();
        void SaveSettings();
        void SetDefault();
    }

    public class ProgramSettings : IProgramSettings
    {
        public string Culture { get; set; }

        public int Theme { get; set; }

        private static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private string GetFilePath()
        {
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Apps";
            if(!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var asm = System.Reflection.Assembly.GetEntryAssembly();

            filePath += "\\" + asm.ManifestModule.Name.Remove(asm.ManifestModule.Name.IndexOf("."));

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            filePath += @"\settings.xml";
            return filePath;
        }

        public void SetDefault()
        {
            Theme = 1;
            Culture = "ru";
        }

        public bool LoadSettings()
        {
            if (!File.Exists(GetFilePath()))
            {
                return false;
            }
            else
            {
                var formatter = new XmlSerializer(typeof(UserSettings));

                try
                {
                    using (FileStream fs = new FileStream(GetFilePath(), FileMode.OpenOrCreate))
                    {
                        var settings = (UserSettings)formatter.Deserialize(fs);
                        Culture = settings.Culture;
                        Theme = settings.Theme;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
                return true;
            }
        }

        public void SaveSettings()
        {
            var UserSettings = new UserSettings()
            {
                Culture = Culture,
                Theme = Theme
            };

            var formatter = new XmlSerializer(typeof(UserSettings));

            try
            {
                using (FileStream fs = new FileStream(GetFilePath(), FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, UserSettings);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }

    [Serializable]
    public class UserSettings
    {
        public UserSettings()
        {}
        
        public string Culture { get; set; }

        public int Theme { get; set; }
    }
}

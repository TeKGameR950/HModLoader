using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Launcher
{
    public static class ConfigManager
    {
        public static Config config;
        public static HMLData data;

        public static void Load()
        {
            config = LoadConfig();
            if (config == null)
                CreateConfig();
            data = LoadData();
        }

        public static HMLData LoadData()
        {
            HMLData data = new HMLData();
            if (File.Exists(FileManager.dataFile))
            {
                try
                {
                    return JsonConvert.DeserializeObject<HMLData>(File.ReadAllText(FileManager.dataFile));
                }
                catch { }
            }
            File.WriteAllText(FileManager.dataFile, JsonConvert.SerializeObject(data, Formatting.Indented));
            return data;
        }

        public static void SaveData()
        {
            File.WriteAllText(FileManager.dataFile, JsonConvert.SerializeObject(data, Formatting.Indented));
        }

        public static void CreateConfig()
        {
            UpdateConfig(new Config());
        }

        public static Config LoadConfig()
        {
            if (!File.Exists(FileManager.configFile)) return null;

            try
            {
                var parser = new FileIniDataParser();
                var data = parser.ReadFile(FileManager.configFile);
                config = new Config();
                // General

                // Advanced
                config.UpdateBranch = data.GetSafeString("Advanced", "UpdateBranch", config.UpdateBranch).ToLower();
                config.AllowMultipleGameInstances = data.GetSafeBool("Advanced", "AllowMultipleGameInstances", config.AllowMultipleGameInstances);

                UpdateConfig(config);

                return config;
            }
            catch
            {

            }

            return null;
        }

        public static void UpdateConfig(Config conf)
        {
            string fileContent = Encoding.UTF8.GetString(FileManager.GetEmbeddedFile("Data.config.ini"));
            foreach (FieldInfo field in conf.GetType().GetFields())
            {
                fileContent = fileContent.Replace($":{field.Name}:", field.FieldType == typeof(bool) ? field.GetValue(conf).ToString().ToLower() : field.GetValue(conf).ToString());
            }
            File.WriteAllText(FileManager.configFile, fileContent);
        }

        public static bool GetSafeBool(this IniData data, string section, string name, bool defaultValue)
        {
            bool parsed = bool.TryParse(data[section][name], out bool value);
            if (parsed) return value;
            return defaultValue;
        }

        public static int GetSafeInt(this IniData data, string section, string name, int defaultValue)
        {
            bool parsed = int.TryParse(data[section][name], out int value);
            if (parsed) return value;
            return defaultValue;
        }

        public static string GetSafeString(this IniData data, string section, string name, string defaultValue)
        {
            return data[section][name] ?? defaultValue;
        }
    }

    public class Config
    {
        public string UpdateBranch = "public";
        public bool AllowMultipleGameInstances = false;
    }

    public class HMLData
    {
        public string lastTab = "games";
        public List<string> favoritedGames = new List<string>();
    }
}

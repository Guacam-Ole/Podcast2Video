using Newtonsoft.Json;

using System;
using System.IO;

namespace P2VBL
{
    public class Config
    {
        public static P2VEntities.Config.Config Configuration { get; private set; }

        public Config()
        {
            ReadConfig();
        }

        private void ReadConfig()
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(path);
            var appSettingsContent = File.ReadAllText(Path.Combine(directory, "appsettings.json"));
            Configuration = JsonConvert.DeserializeObject<P2VEntities.Config.Config>(appSettingsContent);
        }

        private void ReplaceSingleElement(string configpair)
        {
            if (!configpair.Contains(" ")) throw new Exception($"Invalid config '{configpair}'");
            string pair = configpair.Substring(0, configpair.IndexOf(" ")).Trim();
            string value = configpair.Substring(configpair.IndexOf(" ")).Trim();

            var pairElements = pair.Split(".");
            if (pairElements.Length < 2 || pairElements[0] != "Podcast") throw new Exception("Unallowed config");

            object currentElement = Configuration;
            for (int i = 0; i < pairElements.Length; i++)
            {
                string propertyName = pairElements[i];
                var matchingProperty = currentElement.GetType().GetProperty(propertyName);
                if (matchingProperty == null) throw new Exception($"Unexpected property '{propertyName}'");
                if (!matchingProperty.CanWrite) throw new Exception($"Property '{propertyName}' isn't writable");

                if (i == pairElements.Length - 1)
                {
                    var typedValue = Convert.ChangeType(value, matchingProperty.PropertyType);
                    matchingProperty.SetValue(currentElement, typedValue);
                }
                currentElement = matchingProperty.GetValue(currentElement);
            }
        }

        public void OverrideConfig(string[] args)
        {
            string fullConfig = string.Join(" ", args);
            if (!fullConfig.Contains("-c ")) return;

            while (fullConfig.Contains("-c "))
            {
                int startOption = fullConfig.IndexOf("-c ");
                int startValue = fullConfig.IndexOf(" ", startOption);
                int endValue = fullConfig.IndexOf("-", startValue);
                if (endValue < 0) endValue = fullConfig.Length;
                string value = fullConfig.Substring(startValue, endValue - startValue).Trim();
                ReplaceSingleElement(value);
                fullConfig = fullConfig.Remove(startOption, endValue - startOption);
            }
        }
    }
}
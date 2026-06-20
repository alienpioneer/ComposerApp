using System;
using System.Collections.Generic;
using System.IO;

namespace AppComposer.Services
{
    public class IniReader
    {
        private readonly Dictionary<string, Dictionary<string, string>> _data;

        public IniReader(string filePath)
        {
            _data = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            Load(filePath);
        }

        private void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("INI file not found.", filePath);

            var currentSection = string.Empty;

            foreach (var line in File.ReadAllLines(filePath))
            {
                var trimmedLine = line.Trim();

                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                    continue;

                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2).Trim();

                    if (!_data.ContainsKey(currentSection))
                    {
                        _data[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    }
                        
                }
                else
                {
                    var parts = trimmedLine.Split('=', 2);

                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();

                        if (!_data[currentSection].ContainsKey(key))
                        {
                            _data[currentSection].Add(key, value);
                        }
                    }
                }
            }
        }

        public string GetValue(string section, string key, string defaultValue = null)
        {
            if (_data.TryGetValue(section, out var sectionData) && sectionData.TryGetValue(key, out var value))
                return value;
            return defaultValue;
        }

        public Dictionary<string, string> GetSection(string section)
        {
            if (_data.TryGetValue(section, out var sectionData))
            {
                return sectionData;
            }
            else
            {
                return new Dictionary<string, string>();
            }
        }
    }
}

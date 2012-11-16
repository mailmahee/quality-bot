namespace QualityBot.Util
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    public static class SettingsUtil
    {
        /// <summary>
        /// Finds the specified json settings file and deserializes it to a dynamic object.
        /// </summary>
        /// <param name="fileName">Name of the json settings file.</param>
        /// <returns>The settings.</returns>
        public static dynamic ReadSettings(string fileName)
        {
            var path = GetPathOfFile(fileName);
            var json = File.ReadAllText(path);
            var settings = JsonConvert.DeserializeObject<dynamic>(json);

            return settings;
        }

        /// <summary>
        /// Deserializes the specified json string to a dynamic object.
        /// </summary>
        /// <param name="value">The json string.</param>
        /// <returns>The settings.</returns>
        public static dynamic ReadSettingsFromString(string value)
        {
            var settings = JsonConvert.DeserializeObject<dynamic>(value);

            return settings;
        }

        /// <summary>
        /// Gets the specified setting from app.config.
        /// </summary>
        /// <param name="setting">
        /// The setting.
        /// </param>
        /// <param name="settings">The settings.</param>
        /// <typeparam name="T">
        /// The type of object to return.
        /// </typeparam>
        /// <returns>
        /// The setting as T.
        /// </returns>
        public static T GetConfig<T>(string setting, dynamic settings)
        {
            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), settings[setting].Value, true);
            }

            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFrom(settings[setting].Value);
        }

        /// <summary>
        /// Gets the specified setting from app.config.
        /// </summary>
        /// <param name="setting">
        /// The setting.
        /// </param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// The setting as a string.
        /// </returns>
        public static string GetConfig(string setting, dynamic settings)
        {
            return settings[setting].Value;
        }

        /// <summary>
        /// Recursively searchs the application folder for a file that looks like the value passed.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The file found that is like the value provided.
        /// </returns>
        public static string GetPathOfFile(string value)
        {
            var files = GetFilesRecursive(AppDomain.CurrentDomain.BaseDirectory);
            return (from file in files let name = Path.GetFileName(file) where name != null && name.ToLower() == value.ToLower() select file).FirstOrDefault();
        }

        /// <summary>
        /// Recursively finds all files in the directory provided.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>
        /// All files found in the directory and its subfolders.
        /// </returns>
        public static IEnumerable<string> GetFilesRecursive(string directory)
        {
            // 1.
            // Store results in the file results list.
            var result = new List<string>();

            // 2.
            // Store a stack of our directories.
            var stack = new Stack<string>();

            // 3.
            // Add initial directory.
            stack.Push(directory);

            // 4.
            // Continue while there are directories to process
            while (stack.Count > 0)
            {
                // A.
                // Get top directory
                var dir = stack.Pop();

                try
                {
                    // B
                    // Add all files at this directory to the result List.
                    result.AddRange(Directory.GetFiles(dir, "*.*"));

                    // C
                    // Add all directories at this directory.
                    foreach (string dn in Directory.GetDirectories(dir))
                    {
                        stack.Push(dn);
                    }
                }
                catch
                {
                    // D
                    // Could not open the directory
                }
            }

            return result;
        }
    }
}
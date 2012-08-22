namespace QualityBot.Util
{
    using System;
    using System.ComponentModel;
    using System.Configuration;

    public static class AppConfigUtil
    {
        /// <summary>
        /// Gets the specified setting from app.config.
        /// </summary>
        /// <param name="setting">
        /// The setting.
        /// </param>
        /// <typeparam name="T">
        /// The type of object to return.
        /// </typeparam>
        /// <returns>
        /// The setting as T.
        /// </returns>
        public static T AppConfig<T>(string setting)
        {
            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), ConfigurationManager.AppSettings[setting], true);
            }

            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFrom(ConfigurationManager.AppSettings[setting]);
        }

        /// <summary>
        /// Gets the specified setting from app.config.
        /// </summary>
        /// <param name="setting">
        /// The setting.
        /// </param>
        /// <returns>
        /// The setting as a string.
        /// </returns>
        public static string AppConfig(string setting)
        {
            return ConfigurationManager.AppSettings[setting];
        }
    }
}
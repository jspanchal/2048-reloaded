/**
 * Copyright (c) 2013 Nokia Corporation. All rights reserved.
 *
 * Nokia, Nokia Connecting People, Nokia Developer, and HERE are trademarks
 * and/or registered trademarks of Nokia Corporation. Other product and company
 * names mentioned herein may be trademarks or trade names of their respective
 * owners.
 *
 * See the license text file delivered with this project for more information.
 */

using Windows.Storage;

namespace RateMyApp.Helpers
{
    /// <summary>
    /// This helper class can be used to create, store, retrieve and
    /// remove settings in IsolatedStorageSettings.
    /// </summary>
    public class StorageHelper
    {
        /// <summary>
        /// Stores the given key value pair.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="overwrite">If true, will overwrite the existing value.</param>
        /// <returns>True if success, false otherwise.</returns>
        public static bool StoreSetting(string key, object value, bool overwrite)
        {
            if (overwrite || !ApplicationData.Current.RoamingSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.RoamingSettings.Values[key] = value;
                return true;
            }

            return false;
        }


        /// <summary>
        /// Retrieves a setting matching the given key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultVal"></param>
        /// <returns>The value for the key or the default value if key is not found.</returns>
        public static T GetSetting<T>(string key, T defaultVal = default(T))
        {
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(key))
            {
                return (T)ApplicationData.Current.RoamingSettings.Values[key];
            }

            return defaultVal;
        }

        /// <summary>
        /// Removes a setting from IsolatedStorageSettings.
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveSetting(string key)
        {
            ApplicationData.Current.RoamingSettings.Values.Remove(key);
        }
    }
}

using Newtonsoft.Json;

namespace Utility
{
    /// <summary>
    /// Utility functions.
    /// </summary>
    public class Utils
    {
        #region Public methods

        /// <summary>
        /// Parses a JSON string to type T.
        /// </summary>
        public static T fromJSON<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Returns the left number of characters from the string specified.
        /// </summary>
        public static string left(string s, int length)
        {
            return s.Substring(0, length);
        }

        /// <summary>
        /// Returns the right number of characters from the string specified.
        /// </summary>
        public static string right(string s, int length)
        {
            return s.Substring(s.Length-length, length);
        }

        #endregion
    }
}

using Newtonsoft.Json;
using System;

namespace Utility
{
    /// <summary>
    /// Utility functions.
    /// </summary>
    public class Utils
    {
        #region Public methods

        /// <summary>
        /// Returns a string with the prefix "a" or "an" to the string specified, depending
        /// on whether it starts with a vowel or not.
        /// </summary>
        public static string prefixAOrAn(string s)
        {
            var firstLetter = left(s, 1);
            firstLetter = firstLetter.ToUpper();
            switch(firstLetter)
            {
                case "A":
                case "E":
                case "I":
                case "O":
                case "U":
                    return $"an {s}";

                default:
                    return $"a {s}";
            }
        }

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

        /// <summary>
        /// Raises an event.
        /// </summary>
        public static void raiseEvent<T>(object sender, EventHandler<T> eventHandler, T args)
        {
            eventHandler?.Invoke(sender, args);
        }

        #endregion
    }
}

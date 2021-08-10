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
        /// Returns a string for n items, eg "an apple", "two dragons" etc.
        /// </summary>
        public static string numberOfItems(int n, string item)
        {
            switch(n)
            {
                case 1:
                    return prefixAOrAn(item);

                case 2:
                    return $"two {getPlural(item)}";

                case 3:
                    return $"three {getPlural(item)}";

                case 4:
                    return $"four {getPlural(item)}";

                case 5:
                    return $"five {getPlural(item)}";

                case 6:
                    return $"six {getPlural(item)}";

                case 7:
                    return $"seven {getPlural(item)}";

                case 8:
                    return $"eight {getPlural(item)}";

                case 9:
                    return $"nine {getPlural(item)}";

                case 10:
                    return $"ten {getPlural(item)}";

                default:
                    return $"{n} {getPlural(item)}";
            }
        }

        /// <summary>
        /// Returns a plural for the item passed in.
        /// </summary>
        public static string getPlural(string item)
        {
            if(item.EndsWith("x"))
            {
                return $"{item}es";

            }
            return $"{item}s";
        }

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

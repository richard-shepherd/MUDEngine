using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Utility
{
    /// <summary>
    /// Utility functions.
    /// </summary>
    public class Utils
    {
        #region Properties

        /// <summary>
        /// Gets the random number generator.
        /// </summary>
        public static Random Rnd => m_rnd;

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the text passed in, adding quotes at the start and end.
        /// </summary>
        public static List<string> addQuotes(List<string> text)
        {
            // We check if we have any text...
            if(text == null)
            {
                return text;
            }
            var numLines = text.Count;
            if (numLines == 0)
            {
                return text;
            }

            // If we have only one line, we quote it...
            if(numLines == 1)
            {
                var line = text[0];
                return new List<string> { $"\"{line}\"" };
            }

            // We have multiple lines, so we add quotes at the start and end, and copy in
            // the middle lines...
            var firstLine = $"\"{text[0]}";
            var lastLine = $" {text[numLines - 1]}\"";
            var quotedText = new List<string>();
            quotedText.Add(firstLine);
            for(var i=1; i<numLines-1; ++i)
            {
                quotedText.Add($" {text[i]}");
            }
            quotedText.Add(lastLine);

            return quotedText;
        }

        /// <summary>
        /// Returns a string for n items, eg "an apple", "two dragons" etc.
        /// </summary>
        public static string numberOfItems(int n, string item)
        {
            switch(n)
            {
                case 1:
                    return prefix_a_an(item);

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
            var uppercaseLastLetter = right(item, 1).ToUpper();
            if (uppercaseLastLetter == "X")
            {
                return $"{item}es";
            }
            if (uppercaseLastLetter == "S")
            {
                return $"{item}es";
            }
            return $"{item}s";
        }

        /// <summary>
        /// Returns a list of possible singular forms of the item passed in.
        /// </summary>
        public static List<string> getSingularForms(string item)
        {
            var results = new List<string>();

            // We try removing "s"...
            if (right(item, 1) == "s")
            {
                results.Add(left(item, item.Length - 1));
            }

            // We try removing "es"...
            if (right(item, 2) == "es")
            {
                results.Add(left(item, item.Length - 2));
            }

            return results;
        }

        /// <summary>
        /// Returns the string prefixed with 'The'.
        /// </summary>
        public static string prefix_The(string s)
        {
            // We check if the first letter is upper case. If so, it looks like the string
            // could be a proper name so we do not add 'the'...
            var firstLetter = left(s, 1);
            var uppercaseFirstLetter = firstLetter.ToUpper();
            if (firstLetter == uppercaseFirstLetter)
            {
                return s;
            }

            // The first letter is not a capital, so we prefix with 'The'...
            return $"The {s}";
        }

        /// <summary>
        /// Returns the string prefixed with 'the'.
        /// </summary>
        public static string prefix_the(string s)
        {
            // We check if the first letter is upper case. If so, it looks like the string
            // could be a proper name so we do not add 'the'...
            var firstLetter = left(s, 1);
            var uppercaseFirstLetter = firstLetter.ToUpper();
            if (firstLetter == uppercaseFirstLetter)
            {
                return s;
            }

            // The first letter is not a capital, so we prefix with 'the'...
            return $"the {s}";
        }

        /// <summary>
        /// Returns a string with the prefix "a" or "an" to the string specified, depending
        /// on whether it starts with a vowel or not.
        /// </summary>
        public static string prefix_a_an(string s)
        {
            // We check if the first letter is upper case. If so, it looks like the string
            // could be a proper name so we do not add 'a' or 'an'...
            var firstLetter = left(s, 1);
            var uppercaseFirstLetter = firstLetter.ToUpper();
            if(firstLetter == uppercaseFirstLetter)
            {
                return s;
            }

            // The first letter is not uppercase, so we check if it is a vowel...
            switch(uppercaseFirstLetter)
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
        public static void raiseEvent<T>(EventHandler<T> eventHandler, object sender, T args)
        {
            eventHandler?.Invoke(sender, args);
        }

        #endregion

        #region Private data

        // Random number generator...
        private static Random m_rnd = new Random();

        #endregion
    }
}

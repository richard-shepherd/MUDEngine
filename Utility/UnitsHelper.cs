using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    /// <summary>
    /// Helps parse strings such as "10cm", "2.5kg" into numeric values.
    /// </summary>
    public class UnitsHelper
    {
        #region Public methods

        /// <summary>
        /// Static constructor.
        /// </summary>
        static UnitsHelper()
        {
            // Lengths...
            m_unitMultipliers["MM"] = 0.001;
            m_unitMultipliers["CM"] = 0.01;
            m_unitMultipliers["M"] = 1.0;
            m_unitMultipliers["KM"] = 1000.0;

            // Weights...
            m_unitMultipliers["G"] = 0.001;
            m_unitMultipliers["KG"] = 1.0;
        }

        /// <summary>
        /// Parses a string holding a value and units into a double for
        /// the standard unit of the relevant type. 
        /// For example "10cm" -> 0.1 (meters).
        /// </summary>
        public static double parse(string s)
        {
            // We get the value and units...
            var parsedValue = getParsedValue(s);

            // We look up the multiplier...
            double multiplier;
            if(!m_unitMultipliers.TryGetValue(parsedValue.Units, out multiplier))
            {
                throw new Exception($"Could not find units from string '{s}'");
            }
            return parsedValue.Value * multiplier;
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Parses a string into its value and units.
        /// Units are returned as upper-case.
        /// </summary>
        private static ParsedValue getParsedValue(string s)
        {
            // We find the location of the units...
            var lastDigitIndex = s.Length - 1;
            var unitLength = 0;
            for(; lastDigitIndex >= 0; --lastDigitIndex)
            {
                if(!char.IsLetter(s[lastDigitIndex]))
                {
                    break;
                }
                unitLength++;
            }

            // We parse the value and the units...
            var parsedValue = new ParsedValue();
            parsedValue.Value = Convert.ToDouble(s.Substring(0, lastDigitIndex + 1));
            parsedValue.Units = s.Substring(lastDigitIndex + 1, unitLength).ToUpper();

            return parsedValue;
        }

        #endregion

        #region Private types

        /// <summary>
        /// Holds a value string such as "10cm" parsed into its value and units parts.
        /// </summary>
        private class ParsedValue
        {
            public double Value { get; set; } = 0.0;
            public string Units { get; set; } = "";
        }

        #endregion

        #region Private data

        // Multipliers for units into the standard unit for their type, eg:
        // "KM" -> 1000.0
        // "CM" -> 0.01
        // "G"  -> 0.001
        private static Dictionary<string, double> m_unitMultipliers = new Dictionary<string, double>();

        #endregion
    }
}

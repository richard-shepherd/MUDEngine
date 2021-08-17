using System.Collections.Generic;

namespace Utility
{
    /// <summary>
    /// Represents multiline text - ie, a list of strings, plus some helper functions.
    /// </summary>
    public class MultilineText : List<string>
    {
        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public MultilineText()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MultilineText(IEnumerable<string> text)
            : base(text)
        {
        }

        #endregion
    }
}

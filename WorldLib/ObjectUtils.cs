using System.Collections.Generic;
using System.Linq;
using Utility;

namespace WorldLib
{
    /// <summary>
    /// Utilit functions for objects.
    /// </summary>
    public class ObjectUtils
    {
        #region Public methods

        /// <summary>
        /// Returns a string listing the number of each object type from the 
        /// collection passed in.
        /// For example: "3 apples, two boxes".
        /// </summary>
        public static string objectNamesAndCounts(IEnumerable<ObjectBase> objects)
        {
            var objectNamesAndCounts = objects.Select(x => x.Name)
                .GroupBy(x => x)
                .Select(x => Utils.numberOfItems(x.Count(), x.Key));
            return string.Join(", ", objectNamesAndCounts);
        }

        #endregion
    }
}

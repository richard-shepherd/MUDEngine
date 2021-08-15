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

        /// <summary>
        /// Finds the first instance of the object with the name specified (or an alias for it) 
        /// from the collection of objects passed in.
        /// Return null if no object with the name is found.
        /// </summary>
        public static ObjectBase findObject(IEnumerable<ObjectBase> objects, string objectName)
        {
            // We check each object in the location...
            foreach (var objectInCollection in objects)
            {
                if(objectInCollection.matchesName(objectName))
                {
                    return objectInCollection;
                }
            }

            // We did not find a matching object...
            return null;
        }

        #endregion
    }
}

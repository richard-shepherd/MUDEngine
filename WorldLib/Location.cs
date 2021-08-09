using System.Collections.Generic;
using System.Linq;

namespace WorldLib
{
    /// <summary>
    /// Represents locations, including the objects in them and their connections
    /// such as doors and exits to other locations.
    /// </summary>
    public class Location : ObjectBase
    {
        #region Public types

        /// <summary>
        /// Informatiopn about a 'simple' exit from the location.
        /// </summary><remarks>
        /// A simple exit is one which you can take without any conditions, 
        /// eg, without having to open or unlock a door.
        /// </remarks>
        public class ExitType
        {
            /// <summary>
            /// Gets or sets the direction of the exit, eg "N", "W", "UP".
            /// </summary>
            public string Direction { get; set; } = "";

            /// <summary>
            /// Gets or sets the ObjectID of the location to which the exit leads.
            /// </summary>
            public string To { get; set; } = "";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the collection of exits from the location.
        /// </summary>
        public List<ExitType> Exits { get; set; } = new List<ExitType>();

        #endregion

        #region Public methods

        /// <summary>
        /// Returns what you see when you look at a location - including when you
        /// first enter a location. This includes the location's description, as 
        /// well its exits and an overview of objects in it.
        /// </summary>
        public List<string> look()
        {
            // Description...
            var results = new List<string>(Description);

            // Exits...
            var exits = "";
            if(Exits.Count == 1)
            {
                exits = $"There is an exit to the {Exits[0].Direction}.";
            }
            if(Exits.Count > 1)
            {
                var directions = Exits.Select(x => x.Direction);
                exits = $"There are exits to the {string.Join(", ", directions)}.";
            }
            results.Add(exits);

            return results;
        }

        #endregion
    }
}

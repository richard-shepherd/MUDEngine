using System.Collections.Generic;

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
    }
}

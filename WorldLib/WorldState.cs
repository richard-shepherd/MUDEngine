using System.Collections.Generic;

namespace WorldLib
{
    /// <summary>
    /// Holds the state of the world - ie, all the locations and the objects they contain.
    /// </summary><remarks>
    /// The world state is held separately from the management of the world, so that the
    /// state can be cleanly serialized to JSON.
    /// </remarks>
    public class WorldState
    {
        #region Properties

        /// <summary>
        /// Gets or sets the collection of locations in the game, keyed by object-ID.
        /// </summary>
        public Dictionary<string, Location> Locations { get; set; } = new Dictionary<string, Location>();

        #endregion
    }
}

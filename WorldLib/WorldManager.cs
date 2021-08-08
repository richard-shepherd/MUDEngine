namespace WorldLib
{
    /// <summary>
    /// Manages the world, including all locations and the objects they contain.
    /// </summary><remarks>
    /// This class manages and updates the WorldState. The state is held separately 
    /// to make it easier to serialize.
    /// </remarks>
    public class WorldManager
    {
        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public WorldManager(ObjectFactory objectFactory, string playerStartingLocationID)
        {
            m_objectFactory = objectFactory;
            m_playerStartingLocationID = playerStartingLocationID;
        }

        /// <summary>
        /// Resets the world to the state at the start of a new game, or when the
        /// reset button is pressed.
        /// </summary>
        public void resetWorld()
        {
            m_worldState = new WorldState();
            m_worldState.Locations = m_objectFactory.createAllLocations();
        }

        #endregion

        #region Private data

        // Construction parameters...
        private readonly ObjectFactory m_objectFactory;
        private readonly string m_playerStartingLocationID;

        // The world state...
        private WorldState m_worldState = null;

        #endregion
    }
}

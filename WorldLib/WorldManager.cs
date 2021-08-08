using System.Collections.Generic;

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

        /// <summary>
        /// Creates a new player.
        /// </summary>
        public Player createPlayer(string name)
        {
            // We set up a new player...
            var player = m_playerFactory.createPlayer(this, name);
            m_players.Add(player);

            // We put the player in the starting location...
            player.setLocation(m_playerStartingLocationID);

            return player;
        }

        #endregion

        #region Private data

        // Construction parameters...
        private readonly ObjectFactory m_objectFactory;
        private readonly string m_playerStartingLocationID;

        // The world state...
        private WorldState m_worldState = new WorldState();

        // Creates players or loads saved players...
        private readonly PlayerFactory m_playerFactory = new PlayerFactory();

        // The collection of active players...
        private readonly List<Player> m_players = new List<Player>();

        #endregion
    }
}

using System;
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
        #region Properties

        /// <summary>
        /// Gets the object-factory.
        /// </summary>
        public ObjectFactory ObjectFactory => m_objectFactory;

        #endregion

        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public WorldManager(string playerStartingLocationID)
        {
            m_objectFactory = new ObjectFactory(this);
            m_playerStartingLocationID = playerStartingLocationID;
        }

        /// <summary>
        /// Updates the world state, based on the time passed since the previous update.
        /// </summary>
        public void update()
        {
            var utcNow = DateTime.UtcNow;

            // We update the locations - which also updates all the items in them...
            var locations = new List<Location>(m_worldState.Locations.Values);
            foreach(var location in locations)
            {
                location.update(utcNow);
            }
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

        /// <summary>
        /// Respawns the payer (after they have died).
        /// </summary>
        public void respawnPlayer(Player player)
        {
            // We reset the player's properties...
            player.setDefaultProperties();

            // We put the player in the starting location...
            player.setLocation(m_playerStartingLocationID);
        }

        /// <summary>
        /// Returns the location for the ID specified.
        /// </summary>
        public Location getLocation(string locationID)
        {
            if(!m_worldState.Locations.TryGetValue(locationID, out var location))
            {
                throw new Exception($"Location not found for ID={locationID}");
            }
            return location;
        }

        #endregion

        #region Private data

        // Construction parameters...
        private readonly string m_playerStartingLocationID;

        // The object factory...
        private readonly ObjectFactory m_objectFactory;

        // The world state...
        private WorldState m_worldState = new WorldState();

        // Creates players or loads saved players...
        private readonly PlayerFactory m_playerFactory = new PlayerFactory();

        // The collection of active players...
        private readonly List<Player> m_players = new List<Player>();

        #endregion
    }
}

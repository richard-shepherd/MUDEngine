namespace WorldLib
{
    /// <summary>
    /// Manages the actions of a player.
    /// </summary><remarks>
    /// The player's state is held separately (in the PlayerState class) to make it
    /// easier to serialize.
    /// </remarks>
    public class Player
    {
        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public Player(WorldManager worldManager, string id, string name)
        {
            m_worldManager = worldManager;
            m_playerState.ObjectID = id;
            m_playerState.Name = name;
        }

        /// <summary>
        /// Sets the player's location.
        /// </summary>
        public void setLocation(string locationID)
        {
            m_playerState.LocationID = locationID;
        }

        #endregion

        #region Private data

        // Construction parameters...
        private readonly WorldManager m_worldManager;

        // The player state...
        private PlayerState m_playerState = new PlayerState();

        #endregion
    }
}

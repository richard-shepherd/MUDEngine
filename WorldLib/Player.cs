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
        public Player()
        {
        }



        #endregion

        #region Private data

        // The player state...
        private PlayerState m_playerState = new PlayerState();

        #endregion
    }
}

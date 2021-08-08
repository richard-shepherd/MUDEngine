using System;

namespace WorldLib
{
    /// <summary>
    /// Creates players and loads saved players.
    /// </summary>
    public class PlayerFactory
    {
        #region Public methods

        /// <summary>
        /// Creates a new player with the name specified.
        /// </summary>
        public Player createPlayer(WorldManager worldManager, string name)
        {
            var id = Guid.NewGuid().ToString();
            var player = new Player(worldManager, id, name);
            return player;
        }

        #endregion
    }
}

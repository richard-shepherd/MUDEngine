using System;
using System.Collections.Generic;
using Utility;

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
        #region Events

        /// <summary>
        /// Data passed with the onUpdate event.
        /// </summary>
        public class Args : EventArgs
        {
            /// <summary>
            /// Gets or sets text sent with the update.
            /// </summary>
            public List<string> Text { get; set; }
        }

        /// <summary>
        /// Raised when there is an update to the player or to the world as seen by the player.
        /// </summary>
        public event EventHandler<Args> onUpdate;

        #endregion

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
            // We note that the player is in the location...
            m_playerState.LocationID = locationID;

            // We get the Location and show its description...
            m_location = m_worldManager.getLocation(locationID);
            sendUpdate(m_location.Description);
        }

        /// <summary>
        /// Looks at the current location.
        /// </summary>
        public void look()
        {
            if(m_location != null)
            {
                sendUpdate(m_location.Description);
            }
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Raises an event sending updated info about the player or about what the 
        /// player can see.
        /// </summary>
        private void sendUpdate(List<string> text)
        {
            var args = new Args { Text = text };
            Utils.raiseEvent(this, onUpdate, args);
        }

        #endregion

        #region Private data

        // Construction parameters...
        private readonly WorldManager m_worldManager;

        // The player state...
        private PlayerState m_playerState = new PlayerState();

        // THe player's current location...
        private Location m_location = null;

        #endregion
    }
}

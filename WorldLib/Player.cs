using System;
using System.Collections.Generic;
using System.Linq;
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
            sendUpdate(m_location.look());
        }

        /// <summary>
        /// Looks at the current location.
        /// </summary>
        public void look()
        {
            if(m_location != null)
            {
                sendUpdate(m_location.look());
            }
        }

        /// <summary>
        /// Parses user input and takes the action specified.
        /// </summary>
        public void parseInput(string input)
        {
            // We parse the input...
            var parsedInput = m_inputParser.parseInput(input);
            if(parsedInput == null)
            {
                Logger.log($"You try to {input} but are not entirely clear on how to do that.");
                return;
            }

            // We perform the action...
            switch(parsedInput.Action)
            {
                case InputParser.ActionEnum.GO_TO_DIRECTION:
                    goToDirection(parsedInput.Direction);
                    break;

                case InputParser.ActionEnum.LOOK:
                    look();
                    break;

                default:
                    throw new Exception($"Action {parsedInput.Action} not handled.");
            }
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Goes to the direction specified.
        /// </summary>
        private void goToDirection(string direction)
        {
            // We find the location in the direction specified...
            var exit = m_location.Exits.FirstOrDefault(x => x.Direction == direction);
            if(exit == null)
            {
                Logger.log($"There is no exit to the {direction}.");
                return;
            }

            // We set this as our new location...
            setLocation(exit.To);
        }

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

        // Parses user input...
        private readonly InputParser m_inputParser = new InputParser();

        #endregion
    }
}

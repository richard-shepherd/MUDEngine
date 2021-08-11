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

            // We set up default player properties...
            m_playerState.HP = 100;
            m_playerState.Dexterity = 20;
            m_playerState.Attacks.Add(new NPC.AttackType { Name = "punch", MinDamage = 0, MaxDamage = 5 });
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
                sendUpdate($"You try to {input} but are not entirely clear on how to do that.");
                return;
            }

            // We perform the action...
            switch(parsedInput.Action)
            {
                case InputParser.ActionEnum.SMOKE_POT:
                    sendUpdate("It is not that sort of pot.");
                    break;

                case InputParser.ActionEnum.GO_TO_DIRECTION:
                    goToDirection(parsedInput.Direction);
                    break;

                case InputParser.ActionEnum.LOOK:
                    look();
                    break;

                case InputParser.ActionEnum.TAKE:
                    takeTargetFromLocation(parsedInput.Target1);
                    break;

                case InputParser.ActionEnum.EXAMINE:
                    examine(parsedInput.Target1);
                    break;

                case InputParser.ActionEnum.INVENTORY:
                    showInventory();
                    break;

                default:
                    throw new Exception($"Action {parsedInput.Action} not handled.");
            }
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Shows the contents of the inventory.
        /// </summary>
        private void showInventory()
        {
            sendUpdate(m_inventory.examine());
        }

        /// <summary>
        /// Takes the target specified from the current location and adds it/them to the inventory.
        /// </summary><remarks>
        /// Note that the target specified by a TAKE action could be a plural, eg "TAKE apples".
        /// </remarks>
        private void takeTargetFromLocation(string target)
        {
            // The target could be expressed as a singular or plural form.
            // We find the singular version of the object name and whether it is a plural...
            var targetInfo = m_worldManager.ObjectFactory.getObjectName(target);
            var item = targetInfo.Name;
            if(targetInfo.IsPlural)
            {
                // We are taking multiple items, so we take as many as we can until we 
                // can't take any more...
                var actionResult = ActionResult.succeeded();
                var itemCount = 0;
                while(actionResult.Status == ActionResult.StatusEnum.SUCCEEDED)
                {
                    actionResult = takeOneItemFromLocation(item);
                    if(actionResult.Status == ActionResult.StatusEnum.SUCCEEDED)
                    {
                        sendUpdate(actionResult.Message);
                        itemCount++;
                    }
                }
                if(itemCount == 0)
                {
                    // No items were successfully taken...
                    sendUpdate(actionResult.Message);
                }
            }
            else
            {
                // We are taking a single item...
                var actionResult = takeOneItemFromLocation(item);
                sendUpdate(actionResult.Message);
            }
        }

        /// <summary>
        /// Takes one item from the current location and adds it to the inventory.
        /// </summary>
        private ActionResult takeOneItemFromLocation(string item)
        {
            // We find the item from the current location...
            var objectFromLocation = m_location.findObject(item);
            if (objectFromLocation == null)
            {
                return ActionResult.failed($"There is no {item} which can be taken.");
            }

            // We add the object to our inventory...
            var actionResult = m_inventory.add(objectFromLocation);
            if (actionResult.Status != ActionResult.StatusEnum.SUCCEEDED)
            {
                return actionResult;
            }

            // The object was successfully added to the inventory, so we remove it 
            // from the location...
            m_location.takeObject(objectFromLocation);
            return ActionResult.succeeded($"You add {Utils.prefix_the(item)} to your inventory.");
        }

        /// <summary>
        /// Examines the object specified.
        /// </summary>
        private void examine(string target)
        {
            // We find the object from the current location...
            var objectFromLocation = m_location.findObject(target);
            if (objectFromLocation == null)
            {
                sendUpdate($"You cannot examine {Utils.prefix_the(target)}.");
                return;
            }

            // We examine the object...
            sendUpdate(objectFromLocation.examine());
        }

        /// <summary>
        /// Goes to the direction specified.
        /// </summary>
        private void goToDirection(string direction)
        {
            // We find the location in the direction specified...
            var exit = m_location.Exits.FirstOrDefault(x => x.Direction == direction);
            if(exit == null)
            {
                sendUpdate($"There is no exit to the {direction}.");
                return;
            }

            // We set this as our new location...
            setLocation(exit.To);
        }

        /// <summary>
        /// Raises an event sending updated info about the player or about what the 
        /// player can see.
        /// </summary>
        private void sendUpdate(string text)
        {
            if(!String.IsNullOrEmpty(text))
            {
                sendUpdate(new List<string> { text });
            }
        }
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

        // The player's inventory...
        private readonly Inventory m_inventory = new Inventory();

        #endregion
    }
}

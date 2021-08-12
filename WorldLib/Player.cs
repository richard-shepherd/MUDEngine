using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace WorldLib
{
    /// <summary>
    /// Manages the actions of a player.
    /// </summary><remarks>
    /// NOTE about properties
    /// ---------------------
    /// This class will be serialized when saving the game state, so make sure that public properties
    /// are only ones which make sense to serialize in this way.
    /// </remarks>
    public class Player : Character
    {
        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public Player(WorldManager worldManager, string id, string name)
        {
            m_worldManager = worldManager;

            // We set up the player's identity...
            ObjectType = ObjectTypeEnum.PLAYER;
            ObjectID = id;
            Name = name;

            // We set up default player properties...
            HP = 100;
            Dexterity = 50;
            AttackIntervalSeconds = 1.0;
            Attacks.Add(new Character.AttackType { Name = "punch", MinDamage = 0, MaxDamage = 5 });
        }

        /// <summary>
        /// Sets the player's location.
        /// </summary>
        public void setLocation(string locationID)
        {
            // We note that the player is in the location...
            LocationID = locationID;

            // We get the Location...
            m_location = m_worldManager.getLocation(locationID);

            // We add ourself to it...
            m_location.ParsedObjects.Add(this);

            // We observe events from the location and characters in it...
            updateObservedObjects();

            // We show the description of the location...
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

                case InputParser.ActionEnum.KILL:
                    kill(parsedInput.Target1);
                    break;

                default:
                    throw new Exception($"Action {parsedInput.Action} not handled.");
            }
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Called when we receive updated information from the current location.
        /// </summary>
        private void onLocationUpdated(object sender, Location.Args args)
        {
            try
            {
                // We show the update to the player...
                sendUpdate(args.Text);
            }
            catch (Exception ex)
            {
                Logger.log(ex);
            }
        }

        /// <summary>
        /// Called when we receive updated information from a character we are observing.
        /// </summary>
        private void onCharacterUpdated(object sender, Character.Args args)
        {
            try
            {
                // We show the update to the player...
                sendUpdate(args.Text);
            }
            catch (Exception ex)
            {
                Logger.log(ex);
            }
        }

        /// <summary>
        /// Starts a fight with the target.
        /// </summary>
        private void kill(string target)
        {
            // We find the item from the current location...
            var objectFromLocation = m_location.findObject(target);
            if (objectFromLocation == null)
            {
                sendUpdate($"There is no {target} to fight.");
                return;
            }

            // We check that the target is an character...
            var opponent = objectFromLocation as Character;
            if(opponent == null)
            {
                sendUpdate($"You cannot fight {Utils.prefix_the(target)}.");
                return;
            }

            // We check if the opponent is the player themself...
            if (opponent == this)
            {
                sendUpdate($"It is probably best not to fight yourself.");
                return;
            }

            // We check if the player is already fighting the opponent...
            if (isFightingOpponent(opponent))
            {
                sendUpdate($"You are already fighting {Utils.prefix_the(target)}.");
                return;
            }

            // We note that the player is fighting the opponent (and that it is fighting the player)...
            addFightOpponent(opponent);
            opponent.addFightOpponent(this);

            // The player takes the first swing at the opponent...
            fight(DateTime.UtcNow);
        }

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
        /// We update the objects we observe, including the current location and the characters in it.
        /// </summary>
        private void updateObservedObjects()
        {
            // We stop observing the previous collection of objects...
            if(m_observedObjects.Location != null)
            {
                m_observedObjects.Location.onUpdate -= onLocationUpdated;
                m_observedObjects.Location = null;
            }
            foreach(var character in m_observedObjects.Characters)
            {
                character.onUpdate -= onCharacterUpdated;
            }
            m_observedObjects.Characters.Clear();

            // We observe the current location...
            if (m_location == null)
            {
                return;
            }
            m_observedObjects.Location = m_location;
            m_location.onUpdate += onLocationUpdated;

            // We observe characters in the location (apart from ourself)...
            foreach(var objectBase in m_location.ParsedObjects)
            {
                var character = objectBase as Character;
                if(character != null && character != this)
                {
                    m_observedObjects.Characters.Add(character);
                    character.onUpdate += onCharacterUpdated;
                }
            }
        }

        #endregion

        #region Private data

        // Construction parameters...
        private readonly WorldManager m_worldManager;

        // THe player's current location...
        private Location m_location = null;

        // Parses user input...
        private readonly InputParser m_inputParser = new InputParser();

        // The player's inventory...
        private readonly Inventory m_inventory = new Inventory();

        // The collection of objects being observed for updates...
        private class ObservedObjects
        {
            public Location Location { get; set; } = null;
            public List<Character> Characters { get; set; } = new List<Character>();
        }
        private readonly ObservedObjects m_observedObjects = new ObservedObjects();

        #endregion
    }
}

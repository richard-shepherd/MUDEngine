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
        #region Events

        /// <summary>
        /// Data passed with the onUIUpdate event.
        /// </summary>
        public class UIUpdateArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets text sent with the update.
            /// </summary>
            public List<string> Text { get; set; }
        }

        /// <summary>
        /// Raised when we have new data to show in the UI for this player.
        /// </summary>
        public event EventHandler<UIUpdateArgs> onUIUpdate;

        #endregion

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
            Dimensions.HeightM = 1.8;
            Dimensions.WidthM = 0.6;
            Dimensions.DepthM = 0.6;
            WeightKG = 90.0;

            // We set up player fighting properties...
            HP = 100;
            Dexterity = 70;
            AttackIntervalSeconds = 1.0;
            Attacks.Add(new Character.AttackType { Name = "punch", MinDamage = 1, MaxDamage = 5 });
        }

        /// <summary>
        /// Sets the player's location.
        /// </summary>
        public void setLocation(string locationID)
        {
            // We remove the player from the previous location...
            if(m_location != null)
            {
                m_location.removeObject(this);
            }

            // We note that the player is in the location...
            LocationID = locationID;

            // We get the Location and add the player to it...
            m_location = m_worldManager.getLocation(locationID);
            m_location.addObject(this);

            // We observe events from the location and characters in it...
            updateObservedObjects();

            // We show the description of the location...
            sendUIUpdate(m_location.look());
        }

        /// <summary>
        /// Looks at the current location.
        /// </summary>
        public void look()
        {
            if(m_location != null)
            {
                sendUIUpdate(m_location.look());
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
                sendUIUpdate($"You try to {input} but are not entirely clear on how to do that.");
                return;
            }

            // We perform the action...
            switch(parsedInput.Action)
            {
                case InputParser.ActionEnum.DROP:
                    drop(parsedInput.Target1);
                    break;

                case InputParser.ActionEnum.EXAMINE:
                    examine(parsedInput.Target1);
                    break;

                case InputParser.ActionEnum.GIVE:
                    give(parsedInput.Target1, parsedInput.Target1);
                    break;

                case InputParser.ActionEnum.GO_TO_DIRECTION:
                    goToDirection(parsedInput.Direction);
                    break;

                case InputParser.ActionEnum.INVENTORY:
                    showInventory();
                    break;

                case InputParser.ActionEnum.KILL:
                    kill(parsedInput.Target1);
                    break;

                case InputParser.ActionEnum.LOOK:
                    look();
                    break;

                case InputParser.ActionEnum.SMOKE_POT:
                    sendUIUpdate("It is not that sort of pot.");
                    break;

                case InputParser.ActionEnum.TAKE:
                    takeTargetFromLocation(parsedInput.Target1);
                    break;

                case InputParser.ActionEnum.TALK:
                    talkTo(parsedInput.Target1);
                    break;

                default:
                    throw new Exception($"Action {parsedInput.Action} not handled.");
            }
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Drops the target object.
        /// </summary>
        private void drop(string target)
        {
        }

        /// <summary>
        /// Gives target1 to target2.
        /// </summary>
        private void give(string target11, string target12)
        {
        }

        /// <summary>
        /// Called when we receive updated information from the current location.
        /// </summary>
        private void onLocationUpdated(object sender, Location.UpdateArgs args)
        {
            try
            {
                // We show the update to the player...
                sendUIUpdate(args.Text);
            }
            catch (Exception ex)
            {
                Logger.log(ex);
            }
        }

        /// <summary>
        /// Called when we receive updated information from a character we are observing.
        /// </summary>
        private void onCharacterUpdated(object sender, Character.GameUpdateArgs args)
        {
            try
            {
                // We show the update to the player...
                sendUIUpdate(args.Text);
            }
            catch (Exception ex)
            {
                Logger.log(ex);
            }
        }

        /// <summary>
        /// Talks to the target.
        /// </summary>
        private void talkTo(string target)
        {
            // We find the character to talk to...
            var character = getCharacter(target, "talk to");
            if(character == null)
            {
                return;
            }

            var results = new List<string>();

            // We find what the character has to say...
            var characterTalk = character.talk();
            if(characterTalk.Count == 0)
            {
                results.Add($"{Utils.prefix_The(character.Name)} doesn't seem to have anything to say.");
            }
            else
            {
                results.Add($"{Utils.prefix_The(character.Name)} says:");
                results.AddRange(Utils.addQuotes(characterTalk));
            }

            sendUIUpdate(results);
        }

        /// <summary>
        /// Returns the Character object for the target specified, or null if
        /// the target is not a valid character.
        /// </summary>
        private Character getCharacter(string target, string verb)
        {
            // We find the item from the current location...
            var objectFromLocation = m_location.findObject(target);
            if (objectFromLocation == null)
            {
                sendUIUpdate($"There is no {target} to {verb}.");
                return null;
            }

            // We check that the target is an character...
            var character = objectFromLocation as Character;
            if (character == null)
            {
                sendUIUpdate($"You cannot {verb} {Utils.prefix_the(target)}.");
                return null;
            }

            // We check if the opponent is the player themself...
            if (character == this)
            {
                sendUIUpdate($"It is probably best not to {verb} yourself.");
                return null;
            }

            return character;
        }

        /// <summary>
        /// Starts a fight with the target.
        /// </summary>
        private void kill(string target)
        {
            // We find the opponent...
            var opponent = getCharacter(target, "fight");
            if(opponent == null)
            {
                return;
            }

            // We check if the player is already fighting the opponent...
            if (isFightingOpponent(opponent))
            {
                sendUIUpdate($"You are already fighting {Utils.prefix_the(target)}.");
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
            sendUIUpdate(ParsedInventory.examine());
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
                        sendUIUpdate(actionResult.Message);
                        itemCount++;
                    }
                }
                if(itemCount == 0)
                {
                    // No items were successfully taken...
                    sendUIUpdate(actionResult.Message);
                }
            }
            else
            {
                // We are taking a single item...
                var actionResult = takeOneItemFromLocation(item);
                sendUIUpdate(actionResult.Message);
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
            var actionResult = ParsedInventory.add(objectFromLocation);
            if (actionResult.Status != ActionResult.StatusEnum.SUCCEEDED)
            {
                return actionResult;
            }

            // The object was successfully added to the inventory, so we remove it 
            // from the location...
            m_location.removeObject(objectFromLocation);
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
                sendUIUpdate($"You cannot examine {Utils.prefix_the(target)}.");
                return;
            }

            // We examine the object...
            sendUIUpdate(objectFromLocation.examine());
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
                sendUIUpdate($"There is no exit to the {direction}.");
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
                m_location.onObjectsUpdated -= onLocationObjectsUpdated;
                m_observedObjects.Location = null;
            }
            foreach(var character in m_observedObjects.Characters)
            {
                character.onGameUpdate -= onCharacterUpdated;
            }
            m_observedObjects.Characters.Clear();

            // We observe the current location...
            if (m_location == null)
            {
                return;
            }
            m_observedObjects.Location = m_location;
            m_location.onUpdate += onLocationUpdated;
            m_location.onObjectsUpdated += onLocationObjectsUpdated;

            // We observe characters in the location...
            var characters = m_location.ParsedObjects
                .Where(x => x is Character)
                .Select(x => x as Character)
                .ToList();
            foreach (var character in characters)
            {
                m_observedObjects.Characters.Add(character);
                character.onGameUpdate += onCharacterUpdated;
            }
        }

        /// <summary>
        /// Called when the collection of objects in the current location has changed.
        /// </summary>
        private void onLocationObjectsUpdated(object sender, Location.ObjectsUpdatedArgs e)
        {
            try
            {
                // We make sure that we are observing the objects...
                updateObservedObjects();
            }
            catch(Exception ex)
            {
                Logger.log(ex);
            }
        }

        /// <summary>
        /// Raises an event sending updated info about the player to the UI.
        /// </summary>
        public void sendUIUpdate(string text)
        {
            sendUIUpdate(new List<string> { text });
        }
        public void sendUIUpdate(List<string> text)
        {
            var args = new UIUpdateArgs { Text = text };
            Utils.raiseEvent(onUIUpdate, this, args);
        }

        #endregion

        #region Private data

        // Construction parameters...
        private readonly WorldManager m_worldManager;

        // THe player's current location...
        private Location m_location = null;

        // Parses user input...
        private readonly InputParser m_inputParser = new InputParser();

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

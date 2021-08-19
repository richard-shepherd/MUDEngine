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
            ID = id;
            Name = name;

            // We set up default player properties...
            setDefaultProperties();
        }

        /// <summary>
        /// Sets up default player properties.
        /// </summary>
        public void setDefaultProperties()
        {
            // We set up default player properties...
            Dimensions.HeightM = 1.8;
            Dimensions.WidthM = 0.6;
            Dimensions.DepthM = 0.6;
            WeightKG = 90.0;

            // We set up player fighting properties...
            HP = 100;
            MaxHP = 150;
            Dexterity = 70;

            // Attacks...
            AttackIntervalSeconds = 1.0;
            Attacks.Clear();
            Attacks.Add(new AttackInfo { Name = "punch", MinDamage = 1, MaxDamage = 5 });
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
            // We check if we are dead...
            if(isDead())
            {
                sendUIUpdate($"Your ghostly form cannot get to grips with the rapidly fading material plane.");
                return;
            }

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

                case InputParser.ActionEnum.EAT:
                    eat(parsedInput.Target1);
                    break;

                case InputParser.ActionEnum.EXAMINE:
                    examine(parsedInput.Target1);
                    break;

                case InputParser.ActionEnum.GIVE:
                    give(parsedInput.Target1, parsedInput.Target2);
                    break;

                case InputParser.ActionEnum.GO_TO_DIRECTION:
                    goToDirection(parsedInput.Direction);
                    break;

                case InputParser.ActionEnum.HELP:
                    showHelp();
                    break;

                case InputParser.ActionEnum.INVENTORY:
                    showInventory();
                    break;

                case InputParser.ActionEnum.KILL:
                    kill(parsedInput.Target1, parsedInput.Target2);
                    break;

                case InputParser.ActionEnum.LOOK:
                    look();
                    break;

                case InputParser.ActionEnum.SMOKE_POT:
                    sendUIUpdate("It is not that sort of pot.");
                    break;

                case InputParser.ActionEnum.STATS:
                    stats(parsedInput.Target1);
                    break;

                case InputParser.ActionEnum.TAKE:
                    takeTargetFromLocation(parsedInput.Target1);
                    break;

                case InputParser.ActionEnum.TALK:
                    talkTo(parsedInput.Target1);
                    break;

                case InputParser.ActionEnum.UNLOCK:
                    unlock(parsedInput.Target1);
                    break;

                case InputParser.ActionEnum.WEAR:
                    wear(parsedInput.Target1);
                    break;

                default:
                    throw new Exception($"Action {parsedInput.Action} not handled.");
            }
        }

        /// <summary>
        /// Called when the player has died.
        /// </summary>
        public void onDeath()
        {
            try
            {
                // We respawn...
                drop_All();
                sendUIUpdate("Your spirit finds itself in a new body.");
                m_worldManager.respawnPlayer(this);
            }
            catch (Exception ex)
            {
                Logger.log(ex);
            }
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Unlocks the target.
        /// </summary>
        private void unlock(string target)
        {
            // We see if the target is in the location...
            var targetInfo = m_location.findObjectFromName(target);
            if (!targetInfo.hasObject())
            {
                sendUIUpdate($"There is no {target} here.");
                return;
            }
            var targetName = targetInfo.getObject().Name;

            // We check that the target is a lockable object...
            var lockable = targetInfo.getObjectAs<ILockable>();
            if(lockable == null)
            {
                sendUIUpdate($"You cannot unlock {Utils.prefix_the(targetName)}.");
                return;
            }

            // We get the ID of the target's key, and check if we have it...
            var keyID = lockable.getKeyID();
            var keyInfo = ParsedInventory.findObjectFromID(keyID);
            if(!keyInfo.hasObject())
            {
                sendUIUpdate($"You do not have the right key to unlock {Utils.prefix_the(targetName)}.");
                return;
            }

            // We unlock the target...
            var actionResult = lockable.unlock(keyInfo.getObject());
            if(actionResult.Status == ActionResult.StatusEnum.SUCCEEDED)
            {
                sendUIUpdate($"You unlock {Utils.prefix_the(targetName)}.");
            }
            else
            {
                sendUIUpdate(actionResult.Message);
            }
        }

        /// <summary>
        /// Wears a piece of armour.
        /// </summary>
        private void wear(string target)
        {
            // We see if the target is in the location...
            var targetInfo = m_location.findObjectFromName(target);
            if (!targetInfo.hasObject())
            {
                // There is no item in the location, so we see if we have one in our inventory...
                targetInfo = ParsedInventory.findObjectFromName(target);
            }
            if (!targetInfo.hasObject())
            {
                sendUIUpdate($"There is no {target} here or in your inventory.");
                return;
            }
            var targetName = targetInfo.getObject().Name;

            // We check that the item is armour...
            var armour = targetInfo.getObjectAs<Armour>();
            if (armour == null)
            {
                sendUIUpdate($"You cannot wear {Utils.prefix_the(targetName)}.");
                return;
            }

            // We wear the armour...
            wear(armour);

            // We remove the item...
            targetInfo.removeFromContainer();

            sendUIUpdate($"You wear {Utils.prefix_the(armour.Name)}. It is a perfect fit.");
        }

        /// <summary>
        /// Eats the target item.
        /// </summary>
        private void eat(string target)
        {
            // We see if the target is in the location...
            var targetInfo = m_location.findObjectFromName(target);
            if(!targetInfo.hasObject())
            {
                // There is no item in the location, so we see if we have one in our inventory...
                targetInfo = ParsedInventory.findObjectFromName(target);
            }
            if (!targetInfo.hasObject())
            {
                sendUIUpdate($"There is no {target} here or in your inventory.");
                return;
            }
            var targetName = targetInfo.getObject().Name;

            // We check that the item is food...
            var food = targetInfo.getObjectAs<Food>();
            if(food == null)
            {
                sendUIUpdate($"You cannot eat {Utils.prefix_the(targetName)}.");
                return;
            }

            // We check whether we are full...
            if(HP >= MaxHP)
            {
                sendUIUpdate($"You are feeling too full to eat {Utils.prefix_the(targetName)}.");
                return;
            }

            // We remove the item...
            targetInfo.removeFromContainer();

            // We eat it...
            sendUIUpdate($"Eating {Utils.prefix_the(targetName)} restores {food.HP} HP.");
            HP += food.HP;
            if(HP > MaxHP)
            {
                HP = MaxHP;
            }
        }

        /// <summary>
        /// Shows help.
        /// </summary>
        private void showHelp()
        {
            sendUIUpdate(m_inputParser.Help);
        }

        /// <summary>
        /// Shows stats for the target.
        /// </summary>
        private void stats(string target)
        {
            // If there is no target, we show our own stats...
            if(String.IsNullOrEmpty(target))
            {
                target = Name;
            }

            // We find the target character and show its stats...
            var character = getCharacter(target, "see stats for", allowSelf: true);
            if(character != null)
            {
                sendUIUpdate(character.getStats());
            }
        }

        /// <summary>
        /// Drops the target object.
        /// </summary>
        private void drop(string target)
        {
            if(target.ToUpper() == "ALL")
            {
                drop_All();
            }
            else
            {
                drop_Target(target);
            }
        }

        /// <summary>
        /// Drops the item specified.
        /// </summary>
        private void drop_Target(string target)
        {
            // We check that we have the item in the inventory...
            var containedObject = ParsedInventory.findObjectFromName(target);
            if(!containedObject.hasObject())
            {
                sendUIUpdate($"You are not carrying {Utils.prefix_a_an(target)}.");
                return;
            }

            // We drop the item...
            m_location.addObject(containedObject);
            containedObject.removeFromContainer();
            sendUIUpdate($"You drop: {Utils.prefix_the(target)}.");
        }

        /// <summary>
        /// Drops all items from the inventory.
        /// </summary>
        private void drop_All()
        {
            var items = ParsedInventory.removeAll();
            foreach(var item in items)
            {
                m_location.addObject(item);
            }
            if(items.Count > 0)
            {
                sendUIUpdate($"You drop: {ObjectUtils.objectNamesAndCounts(items)}.");
            }
        }

        /// <summary>
        /// Gives an item to a character.
        /// </summary>
        private void give(string itemName, string characterName)
        {
            // We check that we have the item in the inventory...
            var itemInfo = ParsedInventory.findObjectFromName(itemName);
            if (!itemInfo.hasObject())
            {
                sendUIUpdate($"You are not carrying {Utils.prefix_a_an(itemName)}.");
                return;
            }
            var item = itemInfo.getObject();

            // We check that the target character is in the current location...
            if(String.IsNullOrEmpty(characterName))
            {
                sendUIUpdate($"To whom do you want to give {Utils.prefix_the(item.Name)}?");
                return;
            }
            var characterAsContainedObject = m_location.findObjectFromName(characterName);
            if(!characterAsContainedObject.hasObject())
            {
                sendUIUpdate($"{Utils.prefix_The(characterName)} is not here.");
                return;
            }

            // We check that the target character is a character...
            var character = characterAsContainedObject.getObjectAs<Character>();
            if(character == null)
            {
                sendUIUpdate($"You cannot give {Utils.prefix_the(itemName)} to {Utils.prefix_the(characterName)}.");
                return;
            }

            var text = new List<string>();
            text.Add($"You give {Utils.prefix_the(character.Name)} {Utils.prefix_the(itemName)}.");

            // We remove the item from our inventory...
            itemInfo.removeFromContainer();

            // We give the item to the character...
            var exchangedObject = character.given(item);
            if(exchangedObject != null)
            {
                // The character has given us something in return...
                text.Add($"{Utils.prefix_The(character.Name)} gives you {Utils.prefix_the(exchangedObject.Name)}.");

                // We add the item to the inventory...
                var actionResult = ParsedInventory.add(exchangedObject);
                if(!String.IsNullOrEmpty(actionResult.Message))
                {
                    text.Add(actionResult.Message);
                }
                if (actionResult.Status != ActionResult.StatusEnum.SUCCEEDED)
                {
                    // Adding the item to the inventory failed, so we drop the item...
                    text.Add($"You drop {Utils.prefix_the(exchangedObject.Name)}.");
                    m_location.addObject(exchangedObject);
                }
            }

            sendUIUpdate(text);
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
        private Character getCharacter(string target, string verb, bool allowSelf=false)
        {
            // We find the item from the current location...
            var objectFromLocation = m_location.findObjectFromName(target);
            if (!objectFromLocation.hasObject())
            {
                sendUIUpdate($"There is no {target} to {verb}.");
                return null;
            }

            // We check that the target is an character...
            var character = objectFromLocation.getObjectAs<Character>();
            if (character == null)
            {
                sendUIUpdate($"You cannot {verb} {Utils.prefix_the(target)}.");
                return null;
            }

            // We check if the opponent is the player themself...
            if (character == this && !allowSelf)
            {
                sendUIUpdate($"It is probably best not to {verb} yourself.");
                return null;
            }

            return character;
        }

        /// <summary>
        /// Starts a fight with the target.
        /// </summary>
        private void kill(string target, string weaponName)
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

            // If a weapon is not specified, we choose the weapon with the best average attack score
            // from the inventory...
            if(String.IsNullOrEmpty(weaponName))
            {
                var bestWeapon = ObjectUtils.getBestWeapon(ParsedInventory.getContents());
                if(bestWeapon != null)
                {
                    weaponName = bestWeapon.Name;
                }
            }

            // We note that the player is fighting the opponent (and that it is fighting the player)...
            addFightOpponent(opponent, weaponName);
            opponent.addFightOpponent(this, "");

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
            var objectFromLocation = m_location.findObjectFromName(item);
            if (!objectFromLocation.hasObject())
            {
                return ActionResult.failed($"There is no {item} which can be taken.");
            }

            // We add the object to our inventory...
            var actionResult = ParsedInventory.add(objectFromLocation.getObject());
            if (actionResult.Status != ActionResult.StatusEnum.SUCCEEDED)
            {
                return actionResult;
            }

            // The object was successfully added to the inventory, so we remove it 
            // from the location...
            objectFromLocation.removeFromContainer();
            return ActionResult.succeeded($"You add {Utils.prefix_the(item)} to your inventory.");
        }

        /// <summary>
        /// Examines the object specified.
        /// </summary>
        private void examine(string target)
        {
            // We find the target from the current location...
            var targetInfo = m_location.findObjectFromName(target);
            if (!targetInfo.hasObject())
            {
                // The target is not in the current location, so we see if we have it in our inventory...
                targetInfo = ParsedInventory.findObjectFromName(target);
            }
            if (!targetInfo.hasObject())
            {
                sendUIUpdate($"You cannot examine {Utils.prefix_the(target)}.");
                return;
            }

            // We examine the object...
            sendUIUpdate(targetInfo.getObject().examine());
        }

        /// <summary>
        /// Goes to the direction specified.
        /// </summary>
        private void goToDirection(string direction)
        {
            // We check if the player can go in the direction requested...
            var actionResult = m_location.canGoInDirection(direction);
            if(actionResult.Status != ActionResult.StatusEnum.SUCCEEDED)
            {
                sendUIUpdate(actionResult.Message);
                return;
            }

            // We find the location in the direction specified and set it as the new location...
            var exit = m_location.Exits.FirstOrDefault(x => x.Direction == direction);
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
            var characters = m_location.LocationContainer.getContents()
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

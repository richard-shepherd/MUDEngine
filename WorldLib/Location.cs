using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;

namespace WorldLib
{
    /// <summary>
    /// Represents locations, including the objects in them and their connections
    /// such as doors and exits to other locations.
    /// </summary>
    public class Location : ObjectBase
    {
        #region Events

        /// <summary>
        /// Data passed with the onUpdate event.
        /// </summary>
        public class UpdateArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets text sent with the update.
            /// </summary>
            public List<string> Text { get; set; }
        }

        /// <summary>
        /// Raised when there is an update to the location.
        /// </summary>
        public event EventHandler<UpdateArgs> onUpdate;

        /// <summary>
        /// Data passed with the onObjectsUpdateEvent.
        /// </summary>
        public class ObjectsUpdatedArgs : EventArgs
        {
        }

        /// <summary>
        /// Raised when objects are added to or removed from the location.
        /// </summary>
        public event EventHandler<ObjectsUpdatedArgs> onObjectsUpdated;

        #endregion

        #region Public types

        /// <summary>
        /// Information about a 'simple' exit from the location.
        /// </summary><remarks>
        /// A simple exit is one which you can take without any conditions, 
        /// eg, without having to open or unlock a door.
        /// </remarks>
        public class ExitInfo
        {
            /// <summary>
            /// Gets or sets the direction of the exit, eg "N", "W", "UP".
            /// </summary>
            public string Direction { get; set; } = "";

            /// <summary>
            /// Gets or sets the ID of the location to which the exit leads.
            /// </summary>
            public string To { get; set; } = "";

            /// <summary>
            /// Gets or sets the object-ID for the (optional) door for this exit.
            /// </summary>
            public string Door { get; set; } = "";

            /// <summary>
            /// Gets or sets the object-ID for the optional object blocking this exit.
            /// </summary>
            public ObjectInfo BlockedBy { get; set; } = new ObjectInfo();
        }

        /// <summary>
        /// Information about an object in the location.
        /// </summary>
        public class ObjectInfo
        {
            /// <summary>
            /// Gets or sets the ID of the object.
            /// </summary>
            public string ID { get; set; } = "";

            /// <summary>
            /// Gets or sets whether the object is locked (if it is a container).
            /// </summary>
            public bool Locked { get; set; } = false;

            /// <summary>
            /// Gets or sets the ID of the key which unlocks the object (if it is a container).
            /// </summary>
            public string Key { get; set; } = "";

            /// <summary>
            /// Gets or sets the ID of armour worn by the object  (if it is a character).
            /// </summary>
            public string Wearing { get; set; } = "";

            /// <summary>
            /// Gets or sets the collection of objects held by the object (if it is a container).
            /// </summary>
            public List<ObjectInfo> Contains { get; set; } = new List<ObjectInfo>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the collection of exits from the location.
        /// </summary>
        public List<ExitInfo> Exits { get; set; } = new List<ExitInfo>();

        /// <summary>
        /// Gets or sets the collection of objects (ObjectInfos) in the location.
        /// </summary>
        public List<ObjectInfo> Objects { get; set; } = new List<ObjectInfo>();

        /// <summary>
        /// Gets or sets the location-container which manages the collection of objects in the location.
        /// </summary><remarks>
        /// This is initialized from the objects from the Objects collection above, but parsed
        /// into concrete ObjectBase-derived objects. Other objects can be dynamically added
        /// or removed as the game is played.
        /// </remarks>
        public LocationContainer LocationContainer { get; set; } = new LocationContainer();

        #endregion

        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public Location()
        {
        }

        /// <summary>
        /// Checks whether it is possible to go in the direction specified.
        /// </summary>
        public ActionResult canGoInDirection(string direction)
        {
            // We find the location in the direction specified...
            var exitInfo = Exits.FirstOrDefault(x => x.Direction == direction);
            if (exitInfo == null)
            {
                return ActionResult.failed($"There is no exit {direction}.");
            }

            // We check if there is a locked door in this direction...
            var doorInfo = findObjectFromID(exitInfo.Door);
            if(doorInfo.hasObject())
            {
                var door = doorInfo.getObjectAs<Door>();
                if(door.Locked)
                {
                    return ActionResult.failed($"Cannot go {direction}. The {door.Name} is locked.");
                }
            }

            // We check if there is an object blocking this direction...
            var blockingObject = getBlockingObject(exitInfo);
            if(blockingObject != null)
            {
                return ActionResult.failed($"The exit {direction} is blocked by {Utils.prefix_the(blockingObject.Name)}.");
            }

            return ActionResult.succeeded();
        }

        /// <summary>
        /// Returns what you see when you look at a location - including when you
        /// first enter a location. This includes the location's description, as 
        /// well its exits and an overview of objects in it.
        /// </summary>
        public List<string> look()
        {
            // Description...
            var results = new List<string>(Description);

            // Exits...
            var exits = look_Exits();
            if(exits.Count > 0)
            {
                results.Add("");
                results.AddRange(exits);
            }

            // Objects...
            var objects = look_Objects();
            if (objects != null)
            {
                results.Add("");
                results.Add(objects);
            }

            // Players...
            var players = look_Players();
            if (players != null)
            {
                results.Add("");
                results.Add(players);
            }

            return results;
        }

        /// <summary>
        /// Adds a collection of objects to the location.
        /// </summary>
        public void addObjects(IEnumerable<ObjectBase> objectBases)
        {
            foreach(var objectBase in objectBases)
            {
                objectBase.LocationID = ID;
                LocationContainer.add(objectBase);
            }
            Utils.raiseEvent(onObjectsUpdated, this, null);
        }

        /// <summary>
        /// Adds an object to the location.
        /// </summary>
        public void addObject(ObjectBase objectBase)
        {
            LocationContainer.add(objectBase);
            objectBase.LocationID = ID;
            Utils.raiseEvent(onObjectsUpdated, this, null);
        }

        /// <summary>
        /// Adds an object to the location.
        /// </summary>
        public void addObject(ContainedObject containedObject)
        {
            addObject(containedObject.getObject());
        }

        /// <summary>
        /// Removes the object from the location.
        /// </summary>
        public void removeObject(ObjectBase objectBase)
        {
            LocationContainer.remove(objectBase);
            Utils.raiseEvent(onObjectsUpdated, this, null);
        }

        /// <summary>
        /// Returns an object from this location for the object-name specified.
        /// If there is more than one object of the requested type, the first one is returned.
        /// Return null if there are no objects of the requested type.
        /// </summary>
        public ContainedObject findObjectFromName(string objectName)
        {
            return LocationContainer.findObjectFromName(objectName);
        }

        /// <summary>
        /// Returns an object from this location for the object-ID specified.
        /// If there is more than one object of the requested type, the first one is returned.
        /// Return null if there are no objects of the requested type.
        /// </summary>
        public ContainedObject findObjectFromID(string objectName)
        {
            return LocationContainer.findObjectFromID(objectName);
        }

        #endregion

        #region ObjectBase implementation

        /// <summary>
        /// Called at regular intervals to update the location and the objects in it.
        /// </summary>
        public override void update(DateTime updateTimeUTC)
        {
            // We update all the objects in the location...
            var objectsInLocation = new List<ObjectBase>(LocationContainer.getContents());
            foreach(var objectBase in objectsInLocation)
            {
                objectBase.update(updateTimeUTC);
            }

            // We clean up any dead characters...
            cleanupDeadCharacters();
        }

        /// <summary>
        /// Parses the location config, in particular the objects in the location.
        /// </summary>
        public override void parseConfig()
        {
            // We parse the ObjectBase...
            base.parseConfig();

            // We parse the objects...
            foreach(var objectInfo in Objects)
            {
                var parsedObject = parseObject(objectInfo);
                LocationContainer.add(parsedObject);
            }

            // We add objects for doors and objects blocking exits...
            foreach(var exit in Exits)
            {
                // Door...
                if(!String.IsNullOrEmpty(exit.Door))
                {
                    var door = getObjectFactory().createObject(exit.Door);
                    door.LocationID = ID;
                    LocationContainer.add(door);
                }

                // Blocking object...
                if (!String.IsNullOrEmpty(exit.BlockedBy.ID))
                {
                    var blockingObject =  parseObject(exit.BlockedBy);
                    blockingObject.LocationID = ID;
                    LocationContainer.add(blockingObject);
                }
            }
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Cleans up dead characters.
        /// </summary>
        private void cleanupDeadCharacters()
        {
            // We clean up dead characters...
            var characters = LocationContainer.getContents()
                .Where(x => x is Character)
                .Select(x => x as Character);
            var deadCharacters = characters
                .Where(x => x.isDead())
                .ToList();
            foreach(var deadCharacter in deadCharacters)
            {
                cleanupDeadCharacter(deadCharacter);
            }

            // We notify any dead players that they are dead...
            var deadPlayers = deadCharacters
                .Where(x => x is Player)
                .Select(x => x as Player)
                .ToList();
            foreach(var deadPlayer in deadPlayers)
            {
                deadPlayer.onDeath();
            }
        }

        /// <summary>
        /// Cleans up a character when they are dead.
        /// </summary>
        private void cleanupDeadCharacter(Character character)
        {
            // The character drops everything from the inventory, and any
            // armour they are wearing...
            var items = character.ParsedInventory.removeAll();
            if(character.Armour != null)
            {
                items.Add(character.Armour);
            }
            addObjects(items);
            if(items.Count > 0)
            {
                sendUpdate($"{Utils.prefix_The(character.Name)} drops: {ObjectUtils.objectNamesAndCounts(items)}.");
            }

            // We remove the character from the location...
            sendUpdate($"A swarm of rats eats the carcass of {Utils.prefix_the(character.Name)}.");
            LocationContainer.remove(character);
        }

        /// <summary>
        /// Returns a string description of the location's exits.
        /// </summary>
        private MultilineText look_Exits()
        {
            var exitsText = new MultilineText();

            // Exit directions...
            if (Exits.Count == 1)
            {
                exitsText.Add($"There is an exit {Exits[0].Direction}.");
            }
            if (Exits.Count > 1)
            {
                var directions = Exits.Select(x => x.Direction);
                exitsText.Add($"There are exits: {string.Join(", ", directions)}.");
            }

            // Doors...
            var exitsWithDoors = Exits.Where(x => !String.IsNullOrEmpty(x.Door));
            foreach (var exitInfo in exitsWithDoors)
            {
                var doorInfo = findObjectFromID(exitInfo.Door);
                if (!doorInfo.hasObject())
                {
                    continue;
                }
                var door = doorInfo.getObjectAs<Door>();
                exitsText.Add($"There is {Utils.prefix_a_an(door.Name)} at the {exitInfo.Direction} exit. The {door.Name} is {door.getLockedText()}.");
            }

            // Blocking objects...
            foreach (var exitInfo in Exits)
            {
                var blockingObject = getBlockingObject(exitInfo);
                if(blockingObject != null)
                {
                    exitsText.Add($"The exit {exitInfo.Direction} is blocked by {Utils.prefix_a_an(blockingObject.Name)}.");
                }
            }

            return exitsText;
        }

        /// <summary>
        /// Return the blocking object if the exit is blocked, or null if the exit is not blocked.
        /// </summary>
        private ObjectBase getBlockingObject(ExitInfo exitInfo)
        {
            // Is there a blocking object for this exit?
            if(String.IsNullOrEmpty(exitInfo.BlockedBy.ID))
            {
                // There is no blocking object...
                return null;
            }

            // There is a blocking object specified.
            // We check if it is in the current location...
            var blockingObjectInfo = findObjectFromID(exitInfo.BlockedBy.ID);
            if (!blockingObjectInfo.hasObject())
            {
                // The object is not in this location...
                return null;
            }

            // We check if the object is in the location itself.
            // If it is in some other container, it will not block the exit.
            if (blockingObjectInfo.getContainer() != LocationContainer)
            {
                // The object is in a container...
                return null;
            }

            // The object is in the location (and not in another conatiner), so we return it...
            return blockingObjectInfo.getObject();
        }

        /// <summary>
        /// Returns a string description of the objects in the location.
        /// </summary>
        private string look_Objects()
        {
            var nonPlayerObjects = LocationContainer.getContents(recursive: false)
                .Where(x => x.ObjectType != ObjectTypeEnum.PLAYER);
            if(nonPlayerObjects.Count() == 0)
            {
                return null;
            }
            return $"You can see: {ObjectUtils.objectNamesAndCounts(nonPlayerObjects)}.";
        }

        /// <summary>
        /// Returns a string description of the players in the location.
        /// </summary>
        private string look_Players()
        {
            if (LocationContainer.ItemCount == 0)
            {
                return null;
            }
            var players = LocationContainer.getContents()
                .Where(x => x.ObjectType == ObjectTypeEnum.PLAYER);
            return $"Players here: {ObjectUtils.objectNamesAndCounts(players)}.";
        }

        /// <summary>
        /// Parses one ObjectInfo into a concrete ObjectBase-derived object.
        /// </summary>
        private ObjectBase parseObject(ObjectInfo objectInfo)
        {
            // We get the object from its ID...
            var objectBase =  getObjectFactory().createObject(objectInfo.ID);

            // We set its location to this location...
            objectBase.LocationID = ID;

            // If the object is a container, we parse its properties and contents...
            parseContainer(objectBase, objectInfo);

            // If the object is a character, we parse its properties...
            parseCharacter(objectBase, objectInfo);

            return objectBase;
        }

        /// <summary>
        /// If the object passed in is a character, we parse additional properties for them.
        /// </summary>
        private void parseCharacter(ObjectBase objectBase, ObjectInfo objectInfo)
        {
            // Is the object a character?
            var character = objectBase as Character;
            if (character == null)
            {
                return;
            }

            // Armour...
            if(!String.IsNullOrEmpty(objectInfo.Wearing))
            {
                var armour = getObjectFactory().createObjectAs<Armour>(objectInfo.Wearing);
                character.wear(armour);
            }
        }

        /// <summary>
        /// If the object passed in is a container, we add any contained objects to it.
        /// </summary>
        private void parseContainer(ObjectBase objectBase, ObjectInfo objectInfo)
        {
            // Is the object a container?
            var container = objectBase as Container;
            if (container == null)
            {
                return;
            }

            // We set if the container is locked, and the ID of the key to unlock it...
            container.Locked = objectInfo.Locked;
            container.Key = objectInfo.Key;

            // The object is a container, so we add any contents to it...
            foreach (var containedObjectInfo in objectInfo.Contains)
            {
                var containedObject = parseObject(containedObjectInfo);
                container.add(containedObject);
            }
        }

        /// <summary>
        /// Raises an event sending updated info about the player or about what the 
        /// player can see.
        /// </summary>
        private void sendUpdate(string text)
        {
            sendUpdate(new List<string> { text });
        }
        private void sendUpdate(List<string> text)
        {
            var args = new UpdateArgs { Text = text };
            Utils.raiseEvent(onUpdate, this, args);
        }

        #endregion
    }
}

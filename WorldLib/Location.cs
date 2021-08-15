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
            /// Gets or sets the ObjectID of the location to which the exit leads.
            /// </summary>
            public string To { get; set; } = "";
        }

        /// <summary>
        /// Information about an object in the location.
        /// </summary>
        public class ObjectInfo
        {
            /// <summary>
            /// Gets or sets the ID of the object.
            /// </summary>
            public string ObjectID { get; set; } = "";

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
            if(exits != null)
            {
                results.Add("");
                results.Add(exits);
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
            Utils.raiseEvent(onObjectsUpdated, this, null);
        }

        /// <summary>
        /// Adds an object to the location.
        /// </summary>
        public void addObject(ContainedObject containedObject)
        {
            LocationContainer.add(containedObject.getObject());
            Utils.raiseEvent(onObjectsUpdated, this, null);
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
        public ContainedObject findObject(string objectName)
        {
            return LocationContainer.findObjectFromName(objectName);
        }

        #endregion

        #region ObjectBase implementation

        /// <summary>
        /// Called at regular intervals to update the location and the objects in it.
        /// </summary>
        public override void update(DateTime updateTimeUTC)
        {
            // We update all the objects in the location...
            foreach(var objectBase in LocationContainer.getContents())
            {
                objectBase.update(updateTimeUTC);
            }

            // We clean up any dead characters...
            cleanupDeadCharacters();
        }

        /// <summary>
        /// Parses the location config, in particular the objects in the location.
        /// </summary>
        public override void parseConfig(ObjectFactory objectFactory)
        {
            // We parse the ObjectBase...
            base.parseConfig(objectFactory);

            // We parse the objects...
            foreach(var objectInfo in Objects)
            {
                var parsedObject = parseObject(objectInfo, objectFactory);
                LocationContainer.add(parsedObject);
            }
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Cleans up dead characters.
        /// </summary>
        private void cleanupDeadCharacters()
        {
            var characters = LocationContainer.getContents()
                .Where(x => x is Character)
                .Select(x => x as Character)
                .ToList();
            foreach(var character in characters)
            {
                if(character.HP <= 0)
                {
                    cleanupDeadCharacter(character);
                }
            }
        }

        /// <summary>
        /// Cleans up a character when they are dead.
        /// </summary>
        private void cleanupDeadCharacter(Character character)
        {
            // The character drops everything...
            var items = character.ParsedInventory.removeAll();
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
        private string look_Exits()
        {
            if (Exits.Count == 1)
            {
                return $"There is an exit to the {Exits[0].Direction}.";
            }

            if (Exits.Count > 1)
            {
                var directions = Exits.Select(x => x.Direction);
                return $"There are exits to the {string.Join(", ", directions)}.";
            }

            return null;
        }

        /// <summary>
        /// Returns a string description of the objects in the location.
        /// </summary>
        private string look_Objects()
        {
            var nonPlayerObjects = LocationContainer.getContents()
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
        private ObjectBase parseObject(ObjectInfo objectInfo, ObjectFactory objectFactory)
        {
            // We get the object from its ID...
            var objectBase = objectFactory.createObject(objectInfo.ObjectID);

            // We set its location to this location...
            objectBase.LocationID = ObjectID;

            // We parse any contents (if it is a container)...
            parseContents(objectBase, objectInfo, objectFactory);

            return objectBase;
        }

        /// <summary>
        /// If the object passed in is a container, we add any contained objects to it.
        /// </summary>
        private void parseContents(ObjectBase objectBase, ObjectInfo objectInfo, ObjectFactory objectFactory)
        {
            // Is the object a container?
            var container = objectBase as Container;
            if(container == null)
            {
                return;
            }

            // The object is a container, so we add any contents to it...
            foreach(var containedObjectInfo in objectInfo.Contains)
            {
                var containedObject = parseObject(containedObjectInfo, objectFactory);
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

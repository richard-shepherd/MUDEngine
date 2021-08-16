﻿using System;
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

            /// <summary>
            /// Gets or sets the object-ID for the optional door for this exit.
            /// </summary>
            public string Door { get; set; } = "";
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
        /// Checks whether it is possible to go in the direction specified.
        /// </summary>
        public ActionResult canGoInDirection(string direction)
        {
            // We find the location in the direction specified...
            var exit = Exits.FirstOrDefault(x => x.Direction == direction);
            if (exit == null)
            {
                return ActionResult.failed($"There is no exit to the {direction}.");
            }

            // We check if there is a locked door in this direction...
            var doorInfo = findObjectFromID(exit.Door);
            if(doorInfo.hasObject())
            {
                var door = doorInfo.getObjectAs<Door>();
                if(door.Locked)
                {
                    return ActionResult.failed($"Cannot go {direction}. The {door.Name} is locked.");
                }
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

            // We add objects for doors...
            foreach(var exit in Exits)
            {
                if(!String.IsNullOrEmpty(exit.Door))
                {
                    var door = objectFactory.createObject(exit.Door);
                    LocationContainer.add(door);
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
            var characters = LocationContainer.getContents()
                .Where(x => x is Character)
                .Select(x => x as Character)
                .ToList();
            foreach(var character in characters)
            {
                if(character.isDead())
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
        private List<string> look_Exits()
        {
            var exits = new List<string>();

            // Exit directions...
            if (Exits.Count == 1)
            {
                exits.Add($"There is an exit {Exits[0].Direction}.");
            }
            if (Exits.Count > 1)
            {
                var directions = Exits.Select(x => x.Direction);
                exits.Add($"There are exits: {string.Join(", ", directions)}.");
            }

            // Doors...
            var exitsWithDoors = Exits.Where(x => !String.IsNullOrEmpty(x.Door));
            foreach(var exit in exitsWithDoors)
            {
                var doorInfo = findObjectFromID(exit.Door);
                if(!doorInfo.hasObject())
                {
                    continue;
                }
                var door = doorInfo.getObjectAs<Door>();
                exits.Add($"There is {Utils.prefix_a_an(door.Name)} at the {exit.Direction} exit. The {door.Name} is {door.getLockedText()}.");
            }

            return exits;
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;
using static WorldLib.ObjectBase;

namespace WorldLib
{
    /// <summary>
    /// Creates game objects, ie objects derived from ObjectBase.
    /// </summary>
    public class ObjectFactory
    {
        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public ObjectFactory()
        {
        }

        /// <summary>
        /// Adds a root folder from which object definitions will be loaded.
        /// All .json files in this folder and sub-folders will be loaded and
        /// can be used to create objects.
        /// </summary>
        public void addRootFolder(string path)
        {
            // We find all .json files in this folder and sub-folders, and load them...
            path = Path.GetFullPath(path);
            var objectfilePaths = Directory.EnumerateFiles(path, "*.json", SearchOption.AllDirectories);
            foreach(var objectFilePath in objectfilePaths)
            {
                loadObjectFile(objectFilePath);
            }
        }

        /// <summary>
        /// Returns a new instance of the object for the ID specified.
        /// The object is returned as ObjectBase.
        /// </summary>
        public ObjectBase createObject(string objectID)
        {
            // We find the definition for the object ID and parse it...
            if(!m_objectDefinitions.TryGetValue(objectID, out var objectDefinition))
            {
                throw new Exception($"Could not find JSON definition for object ID={objectID}");
            }
            return parseObject(objectDefinition);
        }

        /// <summary>
        /// Returns a new instance of the object for the ID specified.
        /// The object is returned as type specified.
        /// </summary>
        public T createObjectAs<T>(string objectID) where T : ObjectBase
        {
            var objectBase = createObject(objectID);
            return (T)objectBase;
        }

        /// <summary>
        /// Returns a collection of all locations in the game, each newly created.
        /// The returned dictionary is keyed by object-ID.
        /// </summary>
        public Dictionary<string, Location> createAllLocations()
        {
            var locations = new Dictionary<string, Location>();

            // We find the object-IDs for all locations, and create them...
            var locationIDs = m_objectDefinitions
                .Where(x => x.Value.ObjectType == ObjectTypeEnum.LOCATION)
                .Select(x => x.Key);
            foreach(var locationID in locationIDs)
            {
                var location = createObjectAs<Location>(locationID);
                locations[locationID] = location;
            }

            return locations;
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Loads JSON config from the path specified.
        /// </summary>
        private void loadObjectFile(string path)
        {
            try
            {
                // We read the file and parse it as an ObjectBase...
                var json = File.ReadAllText(path);
                var objectBase = Utils.fromJSON<ObjectBase>(json);

                // We store the object definition against the object ID...
                var objectDefinition = new ObjectDefinition
                {
                    JSON = json,
                    ObjectType = objectBase.ObjectType
                };
                m_objectDefinitions[objectBase.ObjectID] = objectDefinition;
            }
            catch (Exception ex)
            {
                Logger.log(ex);
            }
        }

        /// <summary>
        /// Parses an object from JSON into a concrete object-type, returning
        /// it as the base ObjectType.
        /// </summary>
        private ObjectBase parseObject(ObjectDefinition objectDefinition)
        {
            // We parse the object depending on its type...
            ObjectBase objectBase = null;
            switch (objectDefinition.ObjectType)
            {
                case ObjectTypeEnum.CONTAINER:
                    objectBase = Utils.fromJSON<Container>(objectDefinition.JSON);
                    break;

                case ObjectTypeEnum.FOOD:
                    objectBase = Utils.fromJSON<Food>(objectDefinition.JSON);
                    break;

                case ObjectTypeEnum.LOCATION:
                    objectBase = Utils.fromJSON<Location>(objectDefinition.JSON);
                    break;

                default:
                    throw new Exception($"Failed to parse object type {objectDefinition.ObjectType}. You may need to add a case statement to ObjectFactory.parseObject().");
            }

            // We parse the string values, eg dimensions into numeric values...
            objectBase.parseValues();

            return objectBase;
        }

        #endregion

        #region Private data

        // JSON object definition and associated info...
        private class ObjectDefinition
        {
            public string JSON { get; set; } = "";
            public ObjectTypeEnum ObjectType { get; set; } = ObjectTypeEnum.NOT_SPECIFIED;
        }

        // Object JSON definitions, keyed by object ID...
        private readonly Dictionary<string, ObjectDefinition> m_objectDefinitions = new Dictionary<string, ObjectDefinition>();

        #endregion
    }
}

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
        public ObjectFactory(WorldManager worldManager)
        {
            m_worldManager = worldManager;
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
        /// Confirms that all objects can be created.
        /// </summary>
        public void validateObjects()
        {
            foreach(var objectID in m_objectDefinitions.Keys)
            {
                createObject(objectID);
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
            foreach (var locationID in locationIDs)
            {
                var location = createObjectAs<Location>(locationID);
                locations[locationID] = location;
            }

            // We find the object-IDs for all reapir-locations, and create them...
            var repairLocationIDs = m_objectDefinitions
                .Where(x => x.Value.ObjectType == ObjectTypeEnum.REPAIR_LOCATION)
                .Select(x => x.Key);
            foreach (var locationID in repairLocationIDs)
            {
                var location = createObjectAs<RepairLocation>(locationID);
                locations[locationID] = location;
            }

            return locations;
        }

        /// <summary>
        /// Returns the object name for the ID specified.
        /// </summary>
        public string getObjectNameFromID(string objectID)
        {
            if(m_objectDefinitions.TryGetValue(objectID, out var objectDefinition))
            {
                return objectDefinition.ObjectName;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Gets the object-name for the object-name provided.
        /// 
        /// The name provided can be singular or plural. The singular form is returned, along
        /// with a bool indicating whether the original name was a plural.
        /// </summary>
        public (string Name, bool IsPlural) getObjectName(string name)
        {
            // We check if we have an object with the name as provided...
            if(m_objectNamesAndAliases.Contains(name))
            {
                return (name, false);
            }

            // We do not have the name, so it could be a plural...
            var singularsForms = Utils.getSingularForms(name);
            foreach(var singularForm in singularsForms)
            {
                if (m_objectNamesAndAliases.Contains(singularForm))
                {
                    return (singularForm, true);
                }
            }

            // We do not have this object, either as a plural or singular.
            // It could be an alias for another object, so we return the original name...
            return (name, false);
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

                // If the ID is not specified, we use the file name...
                if(String.IsNullOrEmpty(objectBase.ID))
                {
                    var filename = Path.GetFileNameWithoutExtension(path);
                    objectBase.ID = filename;
                }

                // We parse the object (as an object-base) to update
                // its properties...
                objectBase.setWorldManager(m_worldManager);
                objectBase.parseConfig();

                // We store the object definition against the object ID...
                var objectDefinition = new ObjectDefinition
                {
                    JSON = json,
                    ObjectType = objectBase.ObjectType,
                    ObjectID = objectBase.ID,
                    ObjectName = objectBase.Name
                };
                m_objectDefinitions[objectBase.ID] = objectDefinition;

                // We store the name and aliases...
                m_objectNamesAndAliases.Add(objectBase.Name);
                foreach (var alias in objectBase.Aliases)
                {
                    m_objectNamesAndAliases.Add(alias);
                }
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
                case ObjectTypeEnum.ARMOUR:
                    objectBase = Utils.fromJSON<Armour>(objectDefinition.JSON);
                    break;

                case ObjectTypeEnum.CHARACTER:
                    objectBase = Utils.fromJSON<Character>(objectDefinition.JSON);
                    break;

                case ObjectTypeEnum.CONTAINER:
                    objectBase = Utils.fromJSON<Container>(objectDefinition.JSON);
                    break;

                case ObjectTypeEnum.DOOR:
                    objectBase = Utils.fromJSON<Door>(objectDefinition.JSON);
                    break;

                case ObjectTypeEnum.FOOD:
                    objectBase = Utils.fromJSON<Food>(objectDefinition.JSON);
                    break;

                case ObjectTypeEnum.LOCATION:
                    objectBase = Utils.fromJSON<Location>(objectDefinition.JSON);
                    break;

                case ObjectTypeEnum.MONSTER:
                    objectBase = Utils.fromJSON<Monster>(objectDefinition.JSON);
                    break;

                case ObjectTypeEnum.OBJECT:
                    objectBase = Utils.fromJSON<ObjectBase>(objectDefinition.JSON);
                    break;

                case ObjectTypeEnum.REPAIR_LOCATION:
                    objectBase = Utils.fromJSON<RepairLocation>(objectDefinition.JSON);
                    break;

                case ObjectTypeEnum.WEAPON:
                    objectBase = Utils.fromJSON<Weapon>(objectDefinition.JSON);
                    break;

                default:
                    throw new Exception($"Failed to parse object type {objectDefinition.ObjectType}. You may need to add a case statement to ObjectFactory.parseObject().");
            }

            // We set the object ID (as it may not be in the json definition)...
            objectBase.ID = objectDefinition.ObjectID;

            // We set the object-factory...
            objectBase.setWorldManager(m_worldManager);

            // We parse the string values, eg dimensions into numeric values...
            objectBase.parseConfig();

            return objectBase;
        }

        #endregion

        #region Private data

        // Construction parameters...
        private readonly WorldManager m_worldManager;

        // JSON object definition and associated info...
        private class ObjectDefinition
        {
            public string JSON { get; set; } = "";
            public ObjectTypeEnum ObjectType { get; set; } = ObjectTypeEnum.NOT_SPECIFIED;
            public string ObjectID { get; set; } = "";
            public string ObjectName { get; set; } = "";
        }

        // Object JSON definitions, keyed by object ID...
        private readonly Dictionary<string, ObjectDefinition> m_objectDefinitions = new Dictionary<string, ObjectDefinition>();

        // Names and aliases of all known objects...
        private readonly HashSet<string> m_objectNamesAndAliases = new HashSet<string>();

        #endregion
    }
}

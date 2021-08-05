using System;
using System.IO;
using Utility;
using static WorldLib.ObjectBase;

namespace WorldLib
{
    /// <summary>
    /// Utilities for WorldLib objects.
    /// </summary>
    public class ObjectUtils
    {
        #region Public methods

        /// <summary>
        /// Loads an object from a JSON config file.
        /// </summary>
        public static ObjectBase loadObject(string path)
        {
            // We read the file, and parse it...
            var json = File.ReadAllText(path);
            return parseObject(json);
        }

        /// <summary>
        /// Parses an object from JSON into a concrete object-type, returning
        /// it as the base ObjectType.
        /// </summary>
        public static ObjectBase parseObject(string json)
        {
            // We initially parse the data as an ObjectBase to find
            // its object-type, then parse it as the more specific type...
            var objectBase = Utils.fromJSON<ObjectBase>(json);
            switch(objectBase.ObjectType)
            {
                case ObjectTypeEnum.CONTAINER:
                    return Utils.fromJSON<Container>(json);

                default:
                    throw new Exception($"Failed to parse object type {objectBase.ObjectType}. Add a case statement to ObjectUtils.parseObject().");
            }
        }

        #endregion
    }
}

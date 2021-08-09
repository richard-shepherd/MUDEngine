using System.Collections.Generic;
using System.Linq;

namespace WorldLib
{
    /// <summary>
    /// Represents locations, including the objects in them and their connections
    /// such as doors and exits to other locations.
    /// </summary>
    public class Location : ObjectBase
    {
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
        /// Gets or sets the collection of parsed-objects in the location.
        /// </summary><remarks>
        /// These are the the same objects as the Objects collection above, but parsed
        /// into concrete ObjectBase-derived objects.
        /// </remarks>
        public List<ObjectBase> ParsedObjects { get; set; } = new List<ObjectBase>();

        #endregion

        #region Public methods

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
            var exits = "";
            if(Exits.Count == 1)
            {
                exits = $"There is an exit to the {Exits[0].Direction}.";
            }
            if(Exits.Count > 1)
            {
                var directions = Exits.Select(x => x.Direction);
                exits = $"There are exits to the {string.Join(", ", directions)}.";
            }
            results.Add(exits);

            return results;
        }

        #endregion

        #region ObjectBase implementation

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
                ParsedObjects.Add(parsedObject);
            }
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Parses one ObjectInfo into a concrete ObjectBase-derived object.
        /// </summary>
        private ObjectBase parseObject(ObjectInfo objectInfo, ObjectFactory objectFactory)
        {
            // We get the object from its ID...
            var objectBase = objectFactory.createObject(objectInfo.ObjectID);

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

        #endregion
    }
}

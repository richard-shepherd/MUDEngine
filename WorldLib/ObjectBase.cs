using System.Collections.Generic;

namespace WorldLib
{
    /// <summary>
    /// Base class for all objects - including players, NPCs, monsters etc.
    /// </summary>
    public class ObjectBase
    {
        #region Public types

        /// <summary>
        /// Enum for the various object types.
        /// </summary>
        public enum ObjectTypeEnum
        {
            NOT_SPECIFIED,
            CONTAINER
        }

        /// <summary>
        /// Holds an object's dimensions.
        /// </summary>
        public class DimensionsType
        {
            /// <summary>
            /// Gets or sets the object's height as a string, eg "30cm".
            /// </summary>
            public string Height { get; set; } = "";

            /// <summary>
            /// Gets or sets the object's width as a string, eg "30cm".
            /// </summary>
            public string Width { get; set; } = "";

            /// <summary>
            /// Gets or sets the object's depth as a string, eg "30cm".
            /// </summary>
            public string Depth { get; set; } = "";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the object type.
        /// </summary><remarks>
        /// Specifies the object class which will be used to parse the object's properties.
        /// </remarks>
        public ObjectTypeEnum ObjectType { get; set; } = ObjectTypeEnum.NOT_SPECIFIED;

        /// <summary>
        /// Gets or sets the object's name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets aliases for the object.
        /// </summary><remarks>
        /// Aliases are other names which can be used when refering to the object.
        /// For example "bag" can be used as an alias for "small bag".
        /// </remarks>
        public List<string> Aliases { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the object's dimensions as strings.
        /// </summary><remarks>
        /// Use ParsedDimensions to access the dimensions parsed into numeric values.
        /// </remarks>
        public DimensionsType Dimensions { get; set; } = new DimensionsType();

        /// <summary>
        /// Gets or sets the object's weight as a string.
        /// </summary><remarks>
        /// Use Parsed weight to access the weight parsed into a numeric value.
        /// </remarks>
        public string Weight { get; set; } = "";

        #endregion
    }
}

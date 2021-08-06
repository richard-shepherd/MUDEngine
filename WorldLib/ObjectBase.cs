using System.Collections.Generic;
using Utility;

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
        /// Holds an object's dimensions as strings, eg "10cm".
        /// </summary>
        public class DimensionsType
        {
            /// <summary>
            /// Gets or sets the object's height.
            /// </summary>
            public string Height { get; set; } = "";

            /// <summary>
            /// Gets or sets the object's width.
            /// </summary>
            public string Width { get; set; } = "";

            /// <summary>
            /// Gets or sets the object's depth.
            /// </summary>
            public string Depth { get; set; } = "";

            /// <summary>
            /// Gets or sets the object's height in meters.
            /// </summary>
            public double HeightM { get; set; } = 0.0;

            /// <summary>
            /// Gets or sets the object's width in meters.
            /// </summary>
            public double WidthM { get; set; } = 0.0;

            /// <summary>
            /// Gets or sets the object's depth in meters.
            /// </summary>
            public double DepthM { get; set; } = 0.0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the object's ID.
        /// </summary>
        public string ObjectID { get; set; } = "";

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
        /// Gets or sets the object's dimensions.
        /// </summary>
        public DimensionsType Dimensions { get; set; } = new DimensionsType();

        /// <summary>
        /// Gets or sets the object's weight as a string.
        /// </summary>
        public string Weight { get; set; } = "";

        /// <summary>
        /// Gets or sets the object's weight in kg.
        /// </summary>
        public double WeightKG { get; set; } = 0.0;

        #endregion

        #region Public methods

        /// <summary>
        /// Parses values such as object dimensions, weight and so on to numeric values.
        /// NOTE: Derived implementations must call this method as well as parsing their
        ///       own values.
        /// </summary>
        public virtual void parseValues()
        {
            // We parse the dimensions...
            Dimensions.HeightM = UnitsHelper.parse(Dimensions.Height);
            Dimensions.WidthM = UnitsHelper.parse(Dimensions.Width);
            Dimensions.DepthM = UnitsHelper.parse(Dimensions.Depth);

            // We parse the weight...
            WeightKG = UnitsHelper.parse(Weight);
        }

        #endregion
    }
}

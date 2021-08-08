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
            CONTAINER,
            FOOD,
            LOCATION
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
        /// Gets or sets the object's description.
        /// </summary><remarks>
        /// This is stored as a list of strings to allow for multi-line descriptions
        /// to be neatly specified in JSON config.
        /// </remarks>
        public List<string> Description { get; set; } = new List<string>();

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

        /// <summary>
        /// Gets the total weight of the object. (For example, including the weight of
        /// objects it holds in the case of a container.)
        /// </summary>
        public double TotalWeightKG => getTotalWeightKG();

        #endregion

        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public ObjectBase()
        {
        }

        /// <summary>
        /// Returns the dimensions of the object in meters, ordered from largest to smallest.
        /// </summary>
        public List<double> getOrderedDimensions()
        {
            var orderedDimensions = new List<double>();
            orderedDimensions.Add(Dimensions.HeightM);
            orderedDimensions.Add(Dimensions.WidthM);
            orderedDimensions.Add(Dimensions.DepthM);
            orderedDimensions.Sort();
            orderedDimensions.Reverse();
            return orderedDimensions;
        }

        #endregion

        #region Virtual functions

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

        /// <summary>
        /// Returns the total weight of the object.
        /// </summary><remarks>
        /// Can be optionally overridden in derived classes for cases where the total
        /// weight may need to be calculated. For example, for containers which hold
        /// other objects which add to their total weight.
        /// </remarks>
        public virtual double getTotalWeightKG()
        {
            return WeightKG;
        }

        #endregion
    }
}

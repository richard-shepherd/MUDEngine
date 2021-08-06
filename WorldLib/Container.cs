using Utility;

namespace WorldLib
{
    /// <summary>
    /// Represents containers such as bags, boxes, chests etc.
    /// </summary>
    public class Container : ObjectBase
    {
        #region Public types

        public class CapacityType
        {
            /// <summary>
            /// Gets or sets the maximum number of items the container can hold.
            /// </summary>
            public int Items { get; set; } = 0;

            /// <summary>
            /// Gets or sets the maximum weight the container can hold as a string, eg "10kg".
            /// </summary>
            public string Weight { get; set; } = "";

            /// <summary>
            /// Gets or sets the maximum weight the container can hold in KG.
            /// </summary>
            public double WeightKG { get; set; } = 0.0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the capacity of the container.
        /// </summary>
        public CapacityType Capacity { get; set; } = new CapacityType();

        #endregion

        #region ObjectBase implementations

        /// <summary>
        /// Parses object properties to numeric equivalents.
        /// </summary>
        public override void parseValues()
        {
            // We parse the base object's values...
            base.parseValues();

            // We parse values for the container...
            Capacity.WeightKG = UnitsHelper.parse(Capacity.Weight);
        }

        #endregion
    }
}

using System.Collections.Generic;
using Utility;
using static WorldLib.ActionResult;

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

        /// <summary>
        /// Gets the number of items held by the container.
        /// </summary>
        public int ItemCount => m_contents.Count;

        #endregion

        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public Container()
            : base()
        {
        }

        /// <summary>
        /// Adds an object to the container.
        /// </summary>
        public ActionResult add(ObjectBase objectToAdd)
        {
            m_contents.Add(objectToAdd);
            return ActionResult.succeeded();
        }

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

        #region Private functions



        #endregion

        #region Private data

        // The container's contents...
        private readonly List<ObjectBase> m_contents = new List<ObjectBase>();

        #endregion
    }
}

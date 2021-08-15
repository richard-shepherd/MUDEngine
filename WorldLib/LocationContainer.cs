namespace WorldLib
{
    /// <summary>
    /// Holds the objects in a location.
    /// </summary>
    public class LocationContainer : Container
    {
        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocationContainer()
        {
            // We set up properties of the inventory...
            Name = "location container";

            // We set up the default dimensions and capacity of the inventory...
            Dimensions.HeightM = 2000.0;
            Dimensions.WidthM = 2000.0;
            Dimensions.DepthM = 2000.0;
            Capacity.Items = 1000;
            Capacity.WeightKG = 1000000.0;
        }

        #endregion
    }
}

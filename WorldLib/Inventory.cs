namespace WorldLib
{
    /// <summary>
    /// A player's inventory.
    /// </summary>
    public class Inventory : Container
    {
        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public Inventory()
        {
            // We set up properties of the inventory...
            Name = "inventory";

            // We set up the default dimensions and capacity of the inventory...
            Dimensions.HeightM = 2.0;
            Dimensions.WidthM = 2.0;
            Dimensions.DepthM = 2.0;
            Capacity.Items = 10;
            Capacity.WeightKG = 50.0;
        }

        #endregion
    }
}

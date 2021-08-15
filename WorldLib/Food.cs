namespace WorldLib
{
    /// <summary>
    /// Represents food objects.
    /// </summary>
    public class Food : ObjectBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the HP you gain when eating the food.
        /// </summary>
        public int HP { get; set; } = 0;

        #endregion

        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public Food()
            : base()
        {
        }

        #endregion
    }
}

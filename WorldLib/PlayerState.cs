namespace WorldLib
{
    /// <summary>
    /// Holds the state for a player.
    /// </summary><remarks>
    /// The state is held separately from the management of the player (done by
    /// the Player class) so that it can be serialized cleanly to JSON.
    /// </remarks>
    public class PlayerState : ObjectBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the ID of the player's location.
        /// </summary>
        public string LocationID { get; set; } = "";

        #endregion
    }
}

namespace WorldLib
{
    /// <summary>
    /// Interface for locakable objects, eg doors and chests.
    /// </summary>
    public interface ILockable
    {
        /// <summary>
        /// Returns the ID of the object which unlocks the object.
        /// </summary>
        string getKeyID();

        /// <summary>
        /// Unlocks the object with the key specified.
        /// </summary>
        ActionResult unlock(ObjectBase key);
    }
}

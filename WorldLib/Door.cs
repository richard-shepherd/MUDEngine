using System;
using System.Collections.Generic;
using Utility;

namespace WorldLib
{
    /// <summary>
    /// Represents doors.
    /// </summary>
    public class Door : ObjectBase, ILockable
    {
        #region Properties

        /// <summary>
        /// Gets or sets the ID of the key which unlocks the door.
        /// </summary>
        public string Key { get; set; } = "";

        /// <summary>
        /// Gets or sets whether the door is locked.
        /// </summary>
        public bool Locked { get; set; } = true;

        #endregion

        #region Public methods
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public Door()
        {
        }

        /// <summary>
        /// Returns 'locked' or 'unlocked'.
        /// </summary>
        public string getLockedText()
        {
            return Locked ? "locked" : "unlocked";
        }

        #endregion

        #region ILockable implementation

        /// <summary>
        /// Returns the ID of the object which unlocks the door.
        /// </summary>
        public string getKeyID()
        {
            return Key;
        }

        /// <summary>
        /// Unlocks the object with the key specified.
        /// </summary>
        public ActionResult unlock(ObjectBase key)
        {
            // We check if the right key was provided...
            if(key.ObjectID != Key)
            {
                return ActionResult.failed($"{Utils.prefix_The(key.Name)} cannot be used to unlock {Utils.prefix_the(Name)}.");
            }

            // The right key was used, so we unlock the door...
            Locked = false;
            return ActionResult.succeeded();
        }

        #endregion

        #region ObjectBase implementation

        /// <summary>
        /// Parses door-specific config.
        /// </summary>
        public override void parseConfig(ObjectFactory objectFactory)
        {
            // We parse the base-object config...
            base.parseConfig(objectFactory);

            // We make the door very heavy, so it cannot be moved...
            if(String.IsNullOrEmpty(Weight))
            {
                WeightKG = 1000.0;
            }

            // We add a "door" alias...
            if(!Aliases.Contains("door"))
            {
                Aliases.Add("door");
            }
        }

        /// <summary>
        /// Returns the text when you examine the door.
        /// </summary>
        public override MultilineText examine()
        {
            var examine = new MultilineText();

            // Description if available...
            if(Description.Count > 0)
            {
                examine.AddRange(Description);
            }

            // Whether the door is locked or unlocked...
            examine.Add($"The {Name} is {getLockedText()}.");

            return examine;
        }

        #endregion
    }
}

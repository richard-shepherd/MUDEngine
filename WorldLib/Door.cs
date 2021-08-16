using System;
using System.Collections.Generic;

namespace WorldLib
{
    /// <summary>
    /// Represents doors.
    /// </summary>
    public class Door : ObjectBase
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
        public override List<string> examine()
        {
            var examine = new List<string>();

            // Description if available...
            if(Description.Count > 0)
            {
                examine.AddRange(Description);
            }

            // Whether the door is locked or unlocked...
            var lockedText = Locked ? "locked" : "unlocked";
            examine.Add($"The {Name} is {lockedText}.");

            return examine;
        }

        #endregion
    }
}

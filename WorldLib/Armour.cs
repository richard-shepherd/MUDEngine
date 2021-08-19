using System.Collections.Generic;
using Utility;

namespace WorldLib
{
    /// <summary>
    /// Represents armour.
    /// </summary>
    public class Armour : ObjectBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the original HP from the config.
        /// </summary>
        public int HP { get; set; } = 0;

        /// <summary>
        /// Gets or sets the armour's current HP.
        /// </summary>
        public int CurrentHP { get; set; } = 0;

        /// <summary>
        /// Gets or sets the damage reduction percentage.
        /// </summary>
        public string DamageReduction { get; set; } = "0%";

        #endregion

        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public Armour()
        {
        }

        #endregion

        #region ObjectBase implementation

        /// <summary>
        /// Returns text when you examine the armour. 
        /// </summary>
        public override MultilineText examine()
        {
            // We start with the base-object info...
            var examine = base.examine();

            // HP...
            examine.Add($"HP: {CurrentHP}/{HP}");

            // Damage reduction...
            examine.Add($"Damage reduction: {DamageReduction}");

            return examine;
        }

        #endregion
    }
}

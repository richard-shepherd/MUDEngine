using System;
using Utility;

namespace WorldLib
{
    /// <summary>
    /// Info about one attack which can be performed by a character or weapon.
    /// </summary>
    public class AttackInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the attack.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the string version of the damage range, eg "50-90".
        /// </summary>
        public string Damage { get; set; } = "";

        /// <summary>
        /// Gets or sets the parsed version of the minimum damage.
        /// </summary>
        public int MinDamage { get; set; } = 0;

        /// <summary>
        /// Gets or sets the parsed version of the maximum damage.
        /// </summary>
        public int MaxDamage { get; set; } = 0;

        #endregion

        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public AttackInfo()
        {
        }

        /// <summary>
        /// Returns info about the attack.
        /// </summary>
        public string getStats()
        {
            return $"{Name}: {MinDamage}-{MaxDamage}";
        }

        /// <summary>
        /// Parses config.
        /// </summary>
        public void parseConfig()
        {
            // We parse the Damage string, eg "60-120", into its min and max values...
            var tokens = Damage.Split('-');
            if (tokens.Length != 2)
            {
                Logger.error($"Invalid damage format '{Damage}' for attack={Name}.");
                return;
            }
            try
            {
                MinDamage = Convert.ToInt32(tokens[0]);
                MaxDamage = Convert.ToInt32(tokens[1]);
            }
            catch (Exception ex)
            {
                Logger.error($"Invalid damage format '{Damage}' for attack={Name}. Message={ex.Message}.");
            }
        }

        #endregion
    }
}

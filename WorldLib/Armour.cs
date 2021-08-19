using System;
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
        /// Parses the config...
        /// </summary>
        public override void parseConfig()
        {
            // We parse the object-base...
            base.parseConfig();

            // We set current HP to the initial HP...
            CurrentHP = HP;

            // Damage reduction...
            parseDamageReduction();
        }

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

        #region Private functions

        /// <summary>
        /// Parses the damage reduction from a reduction string to a decimal factor.
        /// eg. "75%" -> 0.25
        /// </summary>
        private void parseDamageReduction()
        {
            // If no damage reduction is specified, we use a damage factor
            // of 1.0 - ie, we take full damage...
            if(String.IsNullOrEmpty(DamageReduction))
            {
                m_damageFactor = 1.0;
                return;
            }

            // We expect the damage reduction string to end in "%"...
            if(Utils.right(DamageReduction, 1) != "%")
            {
                throw new Exception($"DamageReduction must end in %. Object={ObjectID}.");
            }

            // We get the damage reduction, and convert it to the damage factor...
            try
            {
                var strDamageReductionPercent = DamageReduction.Substring(0, DamageReduction.Length - 1);
                var dDamageReductionPercent = Convert.ToDouble(strDamageReductionPercent);
                m_damageFactor = 1.0 - dDamageReductionPercent / 100.0;
            }
            catch (Exception)
            {
                throw new Exception($"Bad DamageReduction format. Object={ObjectID}.");
            }
        }

        #endregion

        #region Private data

        // Proportion of damage taken, based on the damage reduction percentage...
        private double m_damageFactor = 1.0;

        #endregion
    }
}

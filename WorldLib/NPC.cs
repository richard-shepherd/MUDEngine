using System;
using System.Collections.Generic;
using Utility;

namespace WorldLib
{
    /// <summary>
    /// Represents non-player characters.
    /// </summary>
    public class NPC : ObjectBase
    {
        #region Public types

        /// <summary>
        /// Info about one attack which the NPC can perform.
        /// </summary>
        public class AttackType
        {
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
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the NPC's hit-points.
        /// </summary>
        public int HP { get; set; } = 50;

        /// <summary>
        /// Gets or sets the NPC's dexterity. 
        /// (Value between 0 - 100.)
        /// </summary>
        public int Dexterity { get; set; } = 10;

        /// <summary>
        /// Gets or sets the collection of attacks which the NPC can perform.
        /// </summary>
        public List<AttackType> Attacks { get; set; } = new List<AttackType>();

        /// <summary>
        /// Gets or sets the inteval at which the NPC performs attacks.
        /// </summary>
        public double AttackIntervalSeconds { get; set; } = 0.0;

        #endregion

        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public NPC()
        {
        }

        #endregion

        #region ObjectBase implementation

        /// <summary>
        /// Parses the config.
        /// </summary>
        public override void parseConfig(ObjectFactory objectFactory)
        {
            // We parse the base-object values...
            base.parseConfig(objectFactory);

            // Attacks...
            foreach(var attack in Attacks)
            {
                parseAttack(attack);
            }
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Parses information for the attck.
        /// </summary>
        private void parseAttack(AttackType attack)
        {
            // We parse the damage string, eg "60-120", into its min and max values...
            var tokens = attack.Damage.Split('-');
            if(tokens.Length != 2)
            {
                Logger.error($"Invalid damage format '{attack.Damage}' for attack={attack.Name}, object={Name}.");
                return;
            }
            try
            {
                attack.MinDamage = Convert.ToInt32(tokens[0]);
                attack.MaxDamage = Convert.ToInt32(tokens[1]);
            }
            catch (Exception ex)
            {
                Logger.error($"Invalid damage format '{attack.Damage}' for attack={attack.Name}, object={Name}. Message={ex.Message}.");
            }
        }

        #endregion
    }
}

using System.Collections.Generic;

namespace WorldLib
{
    /// <summary>
    /// Represents weapons.
    /// </summary>
    public class Weapon : ObjectBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the collection of attacks which the weapon can perform.
        /// </summary>
        public List<AttackInfo> Attacks { get; set; } = new List<AttackInfo>();

        #endregion

        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public Weapon()
        {
        }

        /// <summary>
        /// Returns the average attack score.
        /// </summary>
        public double getAverageAttackScore()
        {
            if(Attacks.Count == 0)
            {
                return 0.0;
            }

            var total = 0.0;
            foreach(var attack in Attacks)
            {
                var attackAverage = (attack.MinDamage + attack.MaxDamage) / 2.0;
                total += attackAverage;
            }
            var average = total / Attacks.Count;
            return average;
        }

        #endregion

        #region ObjectBase implementation

        /// <summary>
        /// Parses the config.
        /// </summary>
        public override void parseConfig(ObjectFactory objectFactory)
        {
            // We parse the object-base config...
            base.parseConfig(objectFactory);

            // We parse the attacks...
            foreach(var attack in Attacks)
            {
                attack.parseConfig();
            }
        }

        /// <summary>
        /// Examines the weapon.
        /// </summary>
        public override List<string> examine()
        {
            var examine = base.examine();

            // Attacks...
            examine.Add("Attacks:");
            foreach (var attack in Attacks)
            {
                examine.Add($"- {attack.getStats()}");
            }

            return examine;

        }

        #endregion
    }
}

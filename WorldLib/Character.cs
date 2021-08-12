using System;
using System.Collections.Generic;
using Utility;

namespace WorldLib
{
    /// <summary>
    /// Represents player and non-player characters.
    /// </summary>
    public class Character : ObjectBase
    {
        #region Events

        /// <summary>
        /// Data passed with the onUpdate event.
        /// </summary>
        public class Args : EventArgs
        {
            /// <summary>
            /// Gets or sets text sent with the update.
            /// </summary>
            public List<string> Text { get; set; }
        }

        /// <summary>
        /// Raised when there is an update to the player or to the world as seen by the player.
        /// </summary>
        public event EventHandler<Args> onUpdate;

        #endregion

        #region Public types

        /// <summary>
        /// Info about one attack which the character can perform.
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
        /// Gets or sets the ID of the character's location.
        /// </summary>
        public string LocationID { get; set; } = "";

        /// <summary>
        /// Gets or sets the character's hit-points.
        /// </summary>
        public int HP { get; set; } = 50;

        /// <summary>
        /// Gets or sets the character's dexterity. 
        /// (Value between 0 - 100.)
        /// </summary>
        public int Dexterity { get; set; } = 10;

        /// <summary>
        /// Gets or sets the collection of attacks which the character can perform.
        /// </summary>
        public List<AttackType> Attacks { get; set; } = new List<AttackType>();

        /// <summary>
        /// Gets or sets the inteval at which the character performs attacks.
        /// </summary>
        public double AttackIntervalSeconds { get; set; } = 0.0;

        #endregion

        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public Character()
        {
        }

        /// <summary>
        /// Raises an event sending updated info about the character.
        /// </summary>
        public void sendUpdate(string text)
        {
            sendUpdate(new List<string> { text });
        }
        public void sendUpdate(List<string> text)
        {
            var args = new Args { Text = text };
            Utils.raiseEvent(this, onUpdate, args);
        }

        /// <summary>
        /// Adds an opponent to the collection currently being fought.
        /// </summary>
        public void addFightOpponent(Character opponent)
        {
            m_fightOpponents.Add(opponent);
        }

        /// <summary>
        /// Returns true if the opponent is being fought by the character.
        /// </summary>
        public bool isFightingOpponent(Character opponent)
        {
            return m_fightOpponents.Contains(opponent);
        }

        /// <summary>
        /// Fights one of the active opponents.
        /// </summary>
        public void fight(DateTime updateTimeUTC)
        {
            // We check if we have any opponents we are currently fighting...
            if(m_fightOpponents.Count == 0)
            {
                return;
            }

            // We check if this character has any attacks it can perform...
            if (Attacks.Count == 0)
            {
                return;
            }

            // We check if we can attack now...
            if (!canAttack(updateTimeUTC))
            {
                return;
            }

            // We choose a random attack...
            var attackIndex = Utils.Rnd.Next(0, Attacks.Count);
            var attack = Attacks[attackIndex];

            // We choose a random opponent...
            var opponentIndex = Utils.Rnd.Next(0, m_fightOpponents.Count);
            var opponent = m_fightOpponents[opponentIndex];

            // We check our dexterity...
            var dexterity = Utils.Rnd.Next(0, 100);
            if(dexterity >= Dexterity)
            {
                sendUpdate($"{Utils.prefix_The(Name)} launches a {attack.Name} attack at {Utils.prefix_the(opponent.Name)} but misses.");
                return;
            }
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

        public override void update(DateTime updateTimeUTC)
        {
            fight(updateTimeUTC);
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

        /// <summary>
        /// Checks if we can attack (and sets the next attack time).
        /// </summary>
        private bool canAttack(DateTime updateTimeUTC)
        {
            if (m_nextAttackTime != null
                &&
                m_nextAttackTime.Value > updateTimeUTC)
            {
                // The next time we can attack is in the future, so we cannot attack now...
                return false;
            }

            // We can attack now.
            // We set the next available attack time...
            m_nextAttackTime = updateTimeUTC + TimeSpan.FromSeconds(AttackIntervalSeconds);
            return true;
        }

        #endregion

        #region Private data

        // Opponets currently being fought...
        private readonly List<Character> m_fightOpponents = new List<Character>();

        // The time the next attack can be made...
        private DateTime? m_nextAttackTime = null;

        #endregion
    }
}

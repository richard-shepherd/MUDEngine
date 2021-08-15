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
        /// Data passed with onGameUpdate events.
        /// </summary>
        public class GameUpdateArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets text sent with the update.
            /// </summary>
            public List<string> Text { get; set; }
        }

        /// <summary>
        /// Raised when there is an update to the character or to the world as seen by the character.
        /// </summary><remarks>
        /// This is called a "game update" to distinguish it from UI updates sent by players to the UI.
        /// </remarks>
        public event EventHandler<GameUpdateArgs> onGameUpdate;

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

        /// <summary>
        /// Information about an exchange the character is willing to make.
        /// </summary>
        public class ExchangeInfo
        {
            /// <summary>
            /// Gets or sets the text said by the character, offering the exchange.
            /// </summary>
            public List<string> Talk { get; set; } = new List<string>();

            /// <summary>
            /// Gets or sets the ID of the object the character is willing to give.
            /// </summary>
            public string Give { get; set; } = "";

            /// <summary>
            /// Gets or sets the ID of the object the character wants for the exchange.
            /// </summary>
            public string For { get; set; } = "";
        }

        #endregion

        #region Properties

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

        /// <summary>
        /// Gets or sets the player's inventory as held in the config.
        /// </summary>
        public List<string> Inventory { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the player's inventory.
        /// </summary>
        public Inventory ParsedInventory { get; set; } = new Inventory();

        /// <summary>
        /// Gets ot sets details of an exchange the character is willing to make.
        /// </summary>
        public ExchangeInfo Exchange { get; set; } = new ExchangeInfo();

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
        public void sendGameUpdate(string text)
        {
            sendGameUpdate(new List<string> { text });
        }
        public void sendGameUpdate(List<string> text)
        {
            var args = new GameUpdateArgs { Text = text };
            Utils.raiseEvent(onGameUpdate, this, args);
        }

        /// <summary>
        /// Called when the character has been given an item by another character or player.
        /// If the item is part of an exchange, the exchaged item is returned.
        /// </summary>
        public ObjectBase given(ObjectBase item)
        {
            // We add the item to the inventory...
            ParsedInventory.add(item);

            // We check if this item is part of an exchange...
            if(item.ObjectID != Exchange.For)
            {
                return null;
            }

            // The item is part of an exchange, so we return the exchanged item (if we have it)...
            var objectToGive = ParsedInventory.findObjectFromID(Exchange.Give);
            if(objectToGive.Container != null && objectToGive.ObjectBase != null)
            {
                objectToGive.Container.remove(objectToGive.ObjectBase);
            }
            return objectToGive.ObjectBase;
        }

        /// <summary>
        /// Returns text when you talk to the character.
        /// </summary>
        public List<string> talk()
        {
            return Exchange.Talk;
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
            // We remove any opponents who are dead, or no longer in the same location...
            cleanupFightOppenents();

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
                sendGameUpdate($"{Utils.prefix_The(Name)} launches a {attack.Name} attack at {Utils.prefix_the(opponent.Name)} but misses.");
                return;
            }

            // We work out how much damage we have done to the opponent...
            var damage = Utils.Rnd.Next(attack.MinDamage, attack.MaxDamage + 1);
            opponent.HP -= damage;

            // We create the update...
            var update = new List<string>();

            // Attack description...
            update.Add($"{Utils.prefix_The(Name)} launches a {attack.Name} attack at {Utils.prefix_the(opponent.Name)} doing {damage} damage.");
            
            //// Current HP of each fighter (in alphabetical order)...
            //if(opponent.Name.CompareTo(Name) < 0)
            //{
            //    update.Add($"{opponent.Name} HP={opponent.HP}");
            //    update.Add($"{Name} HP={HP}");
            //}
            //else
            //{
            //    update.Add($"{Name} HP={HP}");
            //    update.Add($"{opponent.Name} HP={opponent.HP}");
            //}

            // We note if this character killed the opponent...
            if (opponent.HP <= 0)
            {
                update.Add($"{Utils.prefix_The(Name)} has killed {Utils.prefix_the(opponent.Name)}.");
            }

            sendGameUpdate(update);
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

            // Inventory...
            foreach(var itemName in Inventory)
            {
                var item = objectFactory.createObject(itemName);
                ParsedInventory.add(item);
            }
        }

        /// <summary>
        /// Updates the character at regular intervals.
        /// </summary>
        public override void update(DateTime updateTimeUTC)
        {
            // We update any ongoing fights...
            fight(updateTimeUTC);
        }

        /// <summary>
        /// Returns what you see when the character is examined.
        /// </summary>
        public override List<string> examine()
        {
            // Base information...
            var text = base.examine();

            // Inventory...
            text.AddRange(ParsedInventory.listContents($"{Utils.prefix_The(Name)} is holding"));

            return text;
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

        /// <summary>
        /// Removes any fight opponents which are dead or no longer in the same 
        /// location as the character.
        /// </summary>
        private void cleanupFightOppenents()
        {
            // If this character is dead, we remove all opponents...
            if(HP <= 0)
            {
                m_fightOpponents.Clear();
            }

            // We check if any opponents are dead or no longer in the same location...
            var opponents = new List<Character>(m_fightOpponents);
            foreach (var opponent in opponents)
            {
                if( opponent.HP <= 0
                    ||
                    opponent.LocationID != LocationID)
                {
                    m_fightOpponents.Remove(opponent);
                }
            }
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

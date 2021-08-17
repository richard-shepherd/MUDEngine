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
        /// Gets or sets the character's maximum HP - ie, the amount to which 
        /// it can be boosted by eating, potions etc.
        /// </summary>
        public int MaxHP { get; set; } = -1;

        /// <summary>
        /// Gets or sets the character's dexterity. 
        /// (Value between 0 - 100.)
        /// </summary>
        public int Dexterity { get; set; } = 10;

        /// <summary>
        /// Gets or sets the character's XP.
        /// </summary>
        public int XP { get; set; } = 0;

        /// <summary>
        /// Gets or sets the collection of attacks which the character can perform.
        /// </summary>
        public List<AttackInfo> Attacks { get; set; } = new List<AttackInfo>();

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
        /// Gets the character's stats.
        /// </summary>
        public List<string> getStats()
        {
            var stats = new List<string>();

            // HP...
            stats.Add($"HP: {HP}/{MaxHP}");

            // XP...
            stats.Add($"XP: {XP}");

            // Dexterity...
            stats.Add($"Dexterity: {Dexterity}");

            // Attacks...
            stats.Add("Attacks:");
            foreach(var attack in Attacks)
            {
                stats.Add($"- {attack.getStats()}");
            }

            return stats;
        }

        /// <summary>
        /// Returns whether the character is alive.
        /// </summary>
        public bool isAlive()
        {
            return HP > 0;
        }

        /// <summary>
        /// Returns whether the character is dead.
        /// </summary>
        public bool isDead()
        {
            return HP <= 0;
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
            objectToGive.removeFromContainer();
            return objectToGive.getObject();
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
            // We remove any opponents who are dead, or no longer in the same location
            // and check that there are still opponents left to fight...
            cleanupFightOppenents();
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

            // We find the attack to perform...
            var attack = chooseAttack();

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

            // We gain XP for damage we inflict...
            XP += damage;

            // We create the update...
            var update = new List<string>();

            // Attack description...
            update.Add($"{Utils.prefix_The(Name)} launches a {attack.Name} attack at {Utils.prefix_the(opponent.Name)} doing {damage} damage.");
            
            // We note if this character killed the opponent...
            if (opponent.isDead())
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

            // If MaxHP is not specified, we set it to the HP...
            if(MaxHP == -1)
            {
                MaxHP = HP;
            }

            // Attacks...
            foreach(var attack in Attacks)
            {
                attack.parseConfig();
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
            var examine = base.examine();

            // Inventory...
            examine.AddRange(ParsedInventory.listContents($"{Utils.prefix_The(Name)} is holding"));

            // Stats...
            examine.AddRange(getStats());

            return examine;
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Returns an attack to use in a round of fighting.
        /// </summary>
        private AttackInfo chooseAttack()
        {
            // We check if we are attacking using a selected weapon...
            //todo

            var attackIndex = Utils.Rnd.Next(0, Attacks.Count);
            var attackInfo = Attacks[attackIndex];
            return attackInfo;
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

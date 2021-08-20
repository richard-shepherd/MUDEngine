using System;
using System.Collections.Generic;
using System.Linq;
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
            public MultilineText Talk { get; set; } = new MultilineText();

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
        public int Dexterity { get; set; } = 50;

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
        public double AttackIntervalSeconds { get; set; } = 2.0;

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

        /// <summary>
        /// Gets or sets the collection of things the character can say when you talk to them.
        /// </summary>
        public List<MultilineText> Talk { get; set; } = new List<MultilineText>();

        /// <summary>
        /// Gets or sets armour worn by the character.
        /// </summary>
        public Armour Armour { get; set; } = null;

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

        /// <summary>
        /// Raises an event sending updated info about the character.
        /// </summary>
        public void sendGameUpdate(List<string> text)
        {
            var args = new GameUpdateArgs { Text = text };
            Utils.raiseEvent(onGameUpdate, this, args);
        }

        /// <summary>
        /// Wears the armour specified.
        /// </summary><remarks>
        /// If we are already wearing armour, the current armour is moved to the inventory if there
        /// is enough room, or dropped if not.
        /// </remarks>
        public void wear(Armour armour)
        {
            // If the character is currently wearing armour, we move it to
            // the inventory if it fits, or drop it if it does not...
            if(Armour != null)
            {
                var actionResult = ParsedInventory.add(Armour);
                if(actionResult.Status == ActionResult.StatusEnum.SUCCEEDED)
                {
                    sendGameUpdate($"{Utils.prefix_The(Name)} adds {Utils.prefix_the(Armour.Name)} to their inventory.");
                }
                else
                {
                    getLocation().addObject(Armour);
                    sendGameUpdate($"{Utils.prefix_The(Name)} drops {Utils.prefix_the(Armour.Name)}.");
                }
            }

            // The character wears the new armour...
            Armour = armour;
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
            foreach(var attack in getAllAttacks())
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
            ObjectBase exchangedObject = null;

            var armour = item as Armour;
            if(armour != null)
            {
                // The object is armour, so the character wears it...
                wear(armour);
            }
            else
            {
                // We add non-armour items to the inventory...
                ParsedInventory.add(item);
            }

            // We check if this item is part of an exchange...
            if (item.ID == Exchange.For)
            {
                // The item is part of an exchange, so we return the exchanged item (if we have it)...
                var objectToGive = ParsedInventory.findObjectFromID(Exchange.Give);
                objectToGive.removeFromContainer();
                exchangedObject = objectToGive.getObject();
            }

            return exchangedObject;
        }

        /// <summary>
        /// Returns text when you talk to the character.
        /// </summary>
        public MultilineText talk()
        {
            var talk = new MultilineText();

            // If Talk items are set up, we say the next one...
            if(Talk.Count > 0)
            {
                talk.AddRange(Talk[m_talkIndex]);
                m_talkIndex++;
                if(m_talkIndex >= Talk.Count)
                {
                    m_talkIndex = 0;
                }
            }

            // If there is an exchange available, the character talks about it...
            if(Exchange.Talk.Count > 0)
            {
                if(talk.Count > 0)
                {
                    talk.Add("");
                }
                talk.AddRange(Exchange.Talk);
            }

            return talk;
        }

        /// <summary>
        /// Adds an opponent to the collection currently being fought.
        /// </summary>
        public void addFightOpponent(Character opponent, string weaponName)
        {
            var fightOpponentInfo = new FightOpponentInfo
            {
                Opponent = opponent,
                WeaponName = weaponName
            };
            m_fightOpponentInfos.Add(fightOpponentInfo);
        }

        /// <summary>
        /// Returns true if the opponent is being fought by the character.
        /// </summary>
        public bool isFightingOpponent(Character opponent)
        {
            return m_fightOpponentInfos.Any(x => x.Opponent == opponent);
        }

        /// <summary>
        /// Fights one of the active opponents.
        /// </summary>
        public void fight(DateTime updateTimeUTC)
        {
            // We remove any opponents who are dead, or no longer in the same location
            // and check that there are still opponents left to fight...
            cleanupFightOppenents();
            if(m_fightOpponentInfos.Count == 0)
            {
                return;
            }

            // We check if we can attack now...
            if (!canAttack(updateTimeUTC))
            {
                return;
            }

            // We choose a random opponent...
            var opponentIndex = Utils.Rnd.Next(0, m_fightOpponentInfos.Count);
            var opponentInfo = m_fightOpponentInfos[opponentIndex];
            var opponent = opponentInfo.Opponent;

            // We find the attack to perform...
            var attack = chooseAttack(opponentInfo.WeaponName);
            if (attack == null)
            {
                return;
            }

            // We check our dexterity...
            var dexterity = Utils.Rnd.Next(0, 100);
            if(dexterity >= Dexterity)
            {
                sendGameUpdate($"{Utils.prefix_The(Name)} launches a {attack.Name} attack at {Utils.prefix_the(opponent.Name)} but misses.");
                return;
            }

            // We work out how much damage we have done to the opponent...
            var damage = Utils.Rnd.Next(attack.MinDamage, attack.MaxDamage + 1);

            // We create the update...
            var update = new List<string>();
            update.Add($"{Utils.prefix_The(Name)} launches a {attack.Name} attack at {Utils.prefix_the(opponent.Name)} doing {damage} damage.");

            // We work out whether any damage is absorbed by the opponent's armour...
            var armour = opponent.Armour;
            if (armour != null
                &&
                armour.CurrentHP > 0)
            {
                // The opponent has workign armour, so we check what damage it absorbs...
                var damageAbsorbed = (int)(damage * armour.getDamageReductionFactor());
                update.Add($"{Utils.prefix_The(opponent.Name)}'s {armour.Name} reduces the damage by {damageAbsorbed} HP.");
                damage -= damageAbsorbed;

                // The armour takes the damage before the opponent...
                if(damage >= armour.CurrentHP)
                {
                    // The armour has absorbed some damage, but has then broken...
                    update.Add($"{Utils.prefix_The(armour.Name)} absorbs {armour.CurrentHP} HP, but is now broken.");
                    damage -= armour.CurrentHP;
                    armour.CurrentHP = 0;
                }
                else
                {
                    // The armour has absorbed all the damage...
                    update.Add($"{Utils.prefix_The(armour.Name)} absorbs all the damage from this attack.");
                    armour.CurrentHP -= damage;
                    damage = 0;
                }
            }

            // We take any remaining damage from the opponent's health...
            opponent.HP -= damage;

            // We gain XP for damage we inflict...
            XP += damage;

            // We note if this character killed the opponent...
            if (opponent.isDead())
            {
                update.Add($"{Utils.prefix_The(Name)} has killed {Utils.prefix_the(opponent.Name)}.");
            }

            // We clean up fight opponents, in case we have killed the opponet...
            cleanupFightOppenents();

            sendGameUpdate(update);
        }

        #endregion

        #region ObjectBase implementation

        /// <summary>
        /// Parses the config.
        /// </summary>
        public override void parseConfig()
        {
            // We parse the base-object values...
            base.parseConfig();

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
                var item = getObjectFactory().createObject(itemName);
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
        public override MultilineText examine()
        {
            // Base information...
            var examine = base.examine();

            // Inventory...
            examine.AddRange(ParsedInventory.listContents($"{Utils.prefix_The(Name)} is holding"));

            // Armour...
            if(Armour != null)
            {
                examine.Add($"{Utils.prefix_The(Name)} is wearing {Utils.prefix_a_an(Armour.Name)}. (HP={Armour.CurrentHP}/{Armour.HP}.)");
            }

            // Stats...
            examine.AddRange(getStats());

            return examine;
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Returns an attack to use in a round of fighting.
        /// </summary>
        private AttackInfo chooseAttack(string selectedWeaponName)
        {
            var attackInfos = new List<AttackInfo>();

            // If we are using a weapon, we find its attacks...
            if(!String.IsNullOrEmpty(selectedWeaponName))
            {
                attackInfos.AddRange(getAttacksForWeapon(selectedWeaponName));
            }

            // If we do not have any attacks, we select a random one...
            if(attackInfos.Count == 0)
            {
                attackInfos.AddRange(getAllAttacks());
            }

            // We choose a random attack...
            if(attackInfos.Count == 0)
            {
                return null;
            }
            var attackIndex = Utils.Rnd.Next(0, attackInfos.Count);
            var attackInfo = attackInfos[attackIndex];
            return attackInfo;
        }

        /// <summary>
        /// Returns attacks to use in a round of fighting using the specified weapon.
        /// </summary>
        private List<AttackInfo> getAttacksForWeapon(string weaponName)
        {
            var attackInfos = new List<AttackInfo>();

            // We check that a weapon is specified...
            if(String.IsNullOrEmpty(weaponName))
            {
                return attackInfos;
            }

            // We check that we have the weapon...
            var weaponInfo = ParsedInventory.findObjectFromName(weaponName);
            if(!weaponInfo.hasObject())
            {
                return attackInfos;
            }

            // We check that it is a weapon...
            var weapon = weaponInfo.getObjectAs<Weapon>();
            if(weapon == null)
            {
                return attackInfos;
            }

            // We have the weapon, so we returns its attacks...
            return weapon.Attacks;
        }

        /// <summary>
        /// Gets all attacks the character can perform, including built-in attacks
        /// and attacks based on weapons they hold.
        /// </summary>
        private List<AttackInfo> getAllAttacks()
        {
            var attackInfos = new List<AttackInfo>(Attacks);
            var weaponAttacks = ParsedInventory.getContents()
                .Where(x => x.ObjectType == ObjectTypeEnum.WEAPON)
                .Select(x => x as Weapon)
                .SelectMany(x => x.Attacks);
            attackInfos.AddRange(weaponAttacks);
            return attackInfos;
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
                m_fightOpponentInfos.Clear();
            }

            // We check if any opponents are dead or no longer in the same location...
            var opponentInfos = new List<FightOpponentInfo>(m_fightOpponentInfos);
            foreach (var opponentInfo in opponentInfos)
            {
                var opponent = opponentInfo.Opponent;
                if( opponent.HP <= 0
                    ||
                    opponent.LocationID != LocationID)
                {
                    m_fightOpponentInfos.Remove(opponentInfo);
                }
            }
        }

        #endregion

        #region Private data

        // Opponets currently being fought...
        private class FightOpponentInfo
        {
            public Character Opponent { get; set; } = null;
            public string WeaponName { get; set; } = "";
        }
        private readonly List<FightOpponentInfo> m_fightOpponentInfos = new List<FightOpponentInfo>();

        // The time the next attack can be made...
        private DateTime? m_nextAttackTime = null;

        // The index of the next Talk text to return...
        private int m_talkIndex = 0;

        #endregion
    }
}

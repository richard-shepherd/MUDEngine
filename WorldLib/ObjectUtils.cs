using System.Collections.Generic;
using System.Linq;
using Utility;

namespace WorldLib
{
    /// <summary>
    /// Utilit functions for objects.
    /// </summary>
    public class ObjectUtils
    {
        #region Public methods

        /// <summary>
        /// Returns a string listing the number of each object type from the 
        /// collection passed in.
        /// For example: "3 apples, two boxes".
        /// </summary>
        public static string objectNamesAndCounts(IEnumerable<ObjectBase> objects)
        {
            var objectNamesAndCounts = objects.Select(x => x.Name)
                .GroupBy(x => x)
                .Select(x => Utils.numberOfItems(x.Count(), x.Key));
            return string.Join(", ", objectNamesAndCounts);
        }

        /// <summary>
        /// Returns the best weapon from the collection of objects passed in.
        /// Returns null if the collection does not contain any weapons.
        /// </summary>
        public static Weapon getBestWeapon(IEnumerable<ObjectBase> objects)
        {
            // We find the weapons...
            var weapons = objects
                .Where(x => x.ObjectType == ObjectBase.ObjectTypeEnum.WEAPON)
                .Select(x => x as Weapon);
            if(weapons.Count() == 0)
            {
                return null;
            }

            // We find the weapon with the best average attack score...
            var bestAttackScore = -1.0;
            Weapon bestWeapon = null;
            foreach(var weapon in weapons)
            {
                var attackScore = weapon.getAverageAttackScore();
                if(attackScore > bestAttackScore)
                {
                    bestAttackScore = attackScore;
                    bestWeapon = weapon;
                }
            }
            return bestWeapon;
        }

        #endregion
    }
}

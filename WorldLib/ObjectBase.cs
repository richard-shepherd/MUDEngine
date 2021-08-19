using System;
using System.Collections.Generic;
using Utility;

namespace WorldLib
{
    /// <summary>
    /// Base class for all objects - including player and non-player characters, monsters etc.
    /// </summary>
    public class ObjectBase
    {
        #region Public types

        /// <summary>
        /// Enum for the various object types.
        /// </summary>
        public enum ObjectTypeEnum
        {
            NOT_SPECIFIED,
            ARMOUR,
            CHARACTER,
            CONTAINER,
            DOOR,
            FOOD,
            LOCATION,
            MONSTER,
            OBJECT,
            PLAYER,
            WEAPON
        }

        /// <summary>
        /// Holds an object's dimensions as strings, eg "10cm".
        /// </summary>
        public class DimensionsType
        {
            /// <summary>
            /// Gets or sets the object's height.
            /// </summary>
            public string Height { get; set; } = "30m";

            /// <summary>
            /// Gets or sets the object's width.
            /// </summary>
            public string Width { get; set; } = "30m";

            /// <summary>
            /// Gets or sets the object's depth.
            /// </summary>
            public string Depth { get; set; } = "30m";

            /// <summary>
            /// Gets or sets the object's height in meters.
            /// </summary>
            public double HeightM { get; set; } = 0.0;

            /// <summary>
            /// Gets or sets the object's width in meters.
            /// </summary>
            public double WidthM { get; set; } = 0.0;

            /// <summary>
            /// Gets or sets the object's depth in meters.
            /// </summary>
            public double DepthM { get; set; } = 0.0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the object's ID.
        /// </summary>
        public string ID { get; set; } = "";

        /// <summary>
        /// Gets or sets the object type.
        /// </summary><remarks>
        /// Specifies the object class which will be used to parse the object's properties.
        /// </remarks>
        public ObjectTypeEnum ObjectType { get; set; } = ObjectTypeEnum.NOT_SPECIFIED;

        /// <summary>
        /// <summary>
        /// Gets or sets the ID of the object's location.
        /// </summary>
        public string LocationID { get; set; } = "";

        /// Gets or sets the object's name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets aliases for the object.
        /// </summary><remarks>
        /// Aliases are other names which can be used when refering to the object.
        /// For example "bag" can be used as an alias for "small bag".
        /// </remarks>
        public List<string> Aliases { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the object's description.
        /// </summary><remarks>
        /// This is stored as a list of strings to allow for multi-line descriptions
        /// to be neatly specified in JSON config.
        /// </remarks>
        public MultilineText Description { get; set; } = new MultilineText();

        /// <summary>
        /// Gets or sets optional text returned when the object is examined.
        /// </summary><remarks>
        /// This is stored as a list of strings to allow for multi-line descriptions
        /// to be neatly specified in JSON config.
        /// </remarks>
        public List<string> Examine { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the object's dimensions.
        /// </summary>
        public DimensionsType Dimensions { get; set; } = new DimensionsType();

        /// <summary>
        /// Gets or sets the object's weight as a string.
        /// </summary>
        public string Weight { get; set; } = "";

        /// <summary>
        /// Gets or sets the object's weight in kg.
        /// </summary>
        public double WeightKG { get; set; } = 0.0;

        /// <summary>
        /// Gets the total weight of the object. (For example, including the weight of
        /// objects it holds in the case of a container.)
        /// </summary>
        public double TotalWeightKG => getTotalWeightKG();

        #endregion

        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public ObjectBase()
        {
        }

        /// <summary>
        /// Returns true if the name specified matches the name or aliases of the object, false otherwise.
        /// </summary>
        public bool matchesName(string name)
        {
            var uppercaseName = name.ToUpper();
            if(Name.ToUpper() == uppercaseName)
            {
                return true;
            }
            foreach(var alias in Aliases)
            {
                if(alias.ToUpper() == uppercaseName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the dimensions of the object in meters, ordered from largest to smallest.
        /// </summary>
        public List<double> getOrderedDimensions()
        {
            var orderedDimensions = new List<double>();
            orderedDimensions.Add(Dimensions.HeightM);
            orderedDimensions.Add(Dimensions.WidthM);
            orderedDimensions.Add(Dimensions.DepthM);
            orderedDimensions.Sort();
            orderedDimensions.Reverse();
            return orderedDimensions;
        }

        /// <summary>
        /// Sets the world-manager.
        /// </summary><remarks>
        /// Note: This is not a propertiy, as we do not want to serialize it.
        /// </remarks>
        public void setWorldManager(WorldManager worldManager)
        {
            m_worldManager = worldManager; ;
        }

        /// <summary>
        /// Gets the world-manager.
        /// </summary>
        public WorldManager getWorldManager()
        {
            return m_worldManager;
        }

        /// <summary>
        /// Gets the object-factory used to create the object.
        /// </summary><remarks>
        /// Note: This is not a propertiy, as we do not want to serialize it.
        /// </remarks>
        public ObjectFactory getObjectFactory()
        {
            return m_worldManager.ObjectFactory;
        }

        /// <summary>
        /// Gets the object's ocation.
        /// </summary>
        public Location getLocation()
        {
            return m_worldManager.getLocation(LocationID);
        }

        #endregion

        #region Virtual functions

        /// <summary>
        /// Parses values such as object dimensions, weight and so on to numeric values.
        /// NOTE: Derived implementations must call this method as well as parsing their
        ///       own values.
        /// </summary>
        public virtual void parseConfig()
        {
            // We add default aliases based on the name...
            addDefaultAliases();

            // We parse the dimensions...
            Dimensions.HeightM = UnitsHelper.parse(Dimensions.Height);
            Dimensions.WidthM = UnitsHelper.parse(Dimensions.Width);
            Dimensions.DepthM = UnitsHelper.parse(Dimensions.Depth);

            // We parse the weight...
            WeightKG = UnitsHelper.parse(Weight);
        }

        /// <summary>
        /// Returns the total weight of the object.
        /// </summary><remarks>
        /// Can be optionally overridden in derived classes for cases where the total
        /// weight may need to be calculated. For example, for containers which hold
        /// other objects which add to their total weight.
        /// </remarks>
        public virtual double getTotalWeightKG()
        {
            return WeightKG;
        }

        /// <summary>
        /// Returns the description of the object.
        /// Can be overridden in derived classes to provide a more detailed description.
        /// </summary>
        public virtual MultilineText examine()
        {
            var result = new MultilineText();
            result.AddRange(Description);
            result.AddRange(Examine);
            if(result.Count == 0)
            {
                result.Add($"You take a close look at {Utils.prefix_the(Name)}. It is {Utils.prefix_a_an(Name)}.");
            }
            return result;
        }

        /// <summary>
        /// Called at regular intervals to update the object, if it has dynamic 
        /// properties or behaviour.
        /// </summary>
        public virtual void update(DateTime updateTimeUTC)
        {
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Adds default aliases for the object name, based on parsing the name.
        /// </summary>
        private void addDefaultAliases()
        {
            var defaultAliases = new List<string>();

            // We split the name by spaces and use the last one as an alias.
            // eg, "oak leaf" -> "leaf" as an alias.
            var tokens = Name.Split(' ');
            var numTokens = tokens.Length;
            if (numTokens > 1)
            {
                defaultAliases.Add(tokens[numTokens - 1]);
            }

            // If there are more than two words we use the last two as an alias,
            // eg "suit of steel armour" -> "steel armour" as an alias.
            if (numTokens > 2)
            {
                defaultAliases.Add($"{tokens[numTokens - 2]} {tokens[numTokens - 1]}");
            }

            // We add the default aliases...
            foreach (var defaultAlias in defaultAliases)
            {
                if(!Aliases.Contains(defaultAlias))
                {
                    Aliases.Add(defaultAlias);
                }
            }
        }


        #endregion

        #region Private data

        // Manages all locations, and the object-factory... 
        private WorldManager m_worldManager = null;

        #endregion
    }
}

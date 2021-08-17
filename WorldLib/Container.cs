﻿using System.Collections.Generic;
using System.Linq;
using Utility;
using static WorldLib.ActionResult;

namespace WorldLib
{
    /// <summary>
    /// Represents containers such as bags, boxes, chests etc.
    /// </summary>
    public class Container : ObjectBase
    {
        #region Public types

        public class CapacityType
        {
            /// <summary>
            /// Gets or sets the maximum number of items the container can hold.
            /// </summary>
            public int Items { get; set; } = 0;

            /// <summary>
            /// Gets or sets the maximum weight the container can hold as a string, eg "10kg".
            /// </summary>
            public string Weight { get; set; } = "";

            /// <summary>
            /// Gets or sets the maximum weight the container can hold in KG.
            /// </summary>
            public double WeightKG { get; set; } = 0.0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the capacity of the container.
        /// </summary>
        public CapacityType Capacity { get; set; } = new CapacityType();

        /// <summary>
        /// Gets the number of items held by the container.
        /// </summary>
        public int ItemCount => m_contents.Count;

        #endregion

        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public Container()
            : base()
        {
        }

        /// <summary>
        /// Finds the named object in this container - including looking in nested containers.
        /// Returns the object itself and the container which holds it.
        /// Returned values are null if the object was not found.
        /// </summary>
        public ContainedObject findObjectFromName(string objectName)
        {
            // We check the contents of the container...
            foreach (var objectBase in m_contents)
            {
                // We check the object itself...
                if (objectBase.matchesName(objectName))
                {
                    return new ContainedObject(this, objectBase);
                }

                // If the object is a container we check its contents...
                var container = objectBase as Container;
                if (container == null)
                {
                    continue;
                }
                var containedObject = container.findObjectFromName(objectName);
                if (containedObject.hasObject())
                {
                    return containedObject;
                }
            }

            // We did not find the object...
            return new ContainedObject(null, null);
        }

        /// <summary>
        /// Finds the specified object in this container - including looking in nested containers.
        /// Returns the object itself and the container which holds it.
        /// Returned values are null if the object was not found.
        /// </summary>
        public ContainedObject findObjectFromID(string objectID)
        {
            // We check the contents of the container...
            foreach (var objectBase in m_contents)
            {
                // We check the object itself...
                if (objectBase.ObjectID == objectID)
                {
                    return new ContainedObject(this, objectBase);
                }

                // If the object is a container we check its contents...
                var container = objectBase as Container;
                if (container == null)
                {
                    continue;
                }
                var containedObject = container.findObjectFromID(objectID);
                if (containedObject.hasObject())
                {
                    return containedObject;
                }
            }

            // We did not find the object...
            return new ContainedObject(null, null);
        }

        /// <summary>
        /// Adds an object to the container.
        /// </summary>
        public ActionResult add(ObjectBase objectToAdd)
        {
            // We check that we have capacity for the new item...
            var result = add_checkCapacity();
            if(result.Status != StatusEnum.SUCCEEDED)
            {
                return result;
            }

            // We check that the container can hold the weight of the new item...
            result = add_checkWeight(objectToAdd);
            if (result.Status != StatusEnum.SUCCEEDED)
            {
                return result;
            }

            // We check that the new item fits into the container...
            result = add_checkSize(objectToAdd);
            if (result.Status != StatusEnum.SUCCEEDED)
            {
                return result;
            }

            // We can add the item to the container...
            m_contents.Add(objectToAdd);
            return ActionResult.succeeded();
        }

        /// <summary>
        /// Removes the object from the container.
        /// Returns the removed object.
        /// </summary>
        public ObjectBase remove(ObjectBase objectBase)
        {
            m_contents.Remove(objectBase);
            return objectBase;
        }

        /// <summary>
        /// Removes all objects from the container.
        /// Returns the collection of removed items.
        /// </summary>
        public List<ObjectBase> removeAll()
        {
            var items = new List<ObjectBase>(m_contents);
            m_contents.Clear();
            return items;
        }

        /// <summary>
        /// Returns the collection of objects in the container, including recursing into 
        /// other containers we hold.
        /// </summary>
        public List<ObjectBase> getContents(bool recursive = true)
        {
            // We include the items held in the container...
            var contents = new List<ObjectBase>(m_contents);

            if(recursive)
            {
                // We add items held in any containers we hold...
                var containers = contents
                    .Where(x => x.ObjectType == ObjectTypeEnum.CONTAINER)
                    .Select(x => x as Container)
                    .ToList();
                foreach (var container in containers)
                {
                    contents.AddRange(container.getContents());
                }
            }

            return contents;
        }

        #endregion

        #region ObjectBase implementations

        /// <summary>
        /// Parses object properties to numeric equivalents.
        /// </summary>
        public override void parseConfig(ObjectFactory objectFactory)
        {
            // We parse the base object's values...
            base.parseConfig(objectFactory);

            // We parse values for the container...
            Capacity.WeightKG = UnitsHelper.parse(Capacity.Weight);
        }

        /// <summary>
        /// Returns the total weight of the container and its contents.
        /// </summary>
        public override double getTotalWeightKG()
        {
            return WeightKG + m_contents.Sum(x => x.TotalWeightKG);
        }

        /// <summary>
        /// Returns the list of items in the container.
        /// </summary>
        public override List<string> examine()
        {
            return listContents($"{Utils.prefix_The(Name)} contains");
        }

        /// <summary>
        /// Returns the list of items in the container.
        /// </summary>
        public List<string> listContents(string messagePrefix = "")
        {
            if (m_contents.Count == 0)
            {
                return new List<string> { $"{messagePrefix} nothing." };
            }

            var results = new List<string>();

            // We list the contents of the container...
            results.Add($"{messagePrefix}: {ObjectUtils.objectNamesAndCounts(m_contents)}.");

            // If any of the contained objects is itself a container, we list what is in it...
            foreach (var containedObject in m_contents)
            {
                var container = containedObject as Container;
                if (container != null)
                {
                    results.AddRange(container.listContents($"{Utils.prefix_The(container.Name)} contains"));
                }
            }
            return results;
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Called when an item is being added to check that we have capacity to
        /// hold the extra item.
        /// </summary>
        private ActionResult add_checkCapacity()
        {
            if (m_contents.Count < Capacity.Items)
            {
                return ActionResult.succeeded();
            }
            else
            {
                return ActionResult.failed($"The {Name} is full.");
            }
        }

        /// <summary>
        /// Called when an item is being added to check that we can hold the
        /// weight of the new item.
        /// </summary>
        private ActionResult add_checkWeight(ObjectBase objectToAdd)
        {
            var totalWeightKG = objectToAdd.TotalWeightKG + getContentsWeight();
            if (totalWeightKG <= Capacity.WeightKG)
            {
                return ActionResult.succeeded();
            }
            else
            {
                return ActionResult.failed($"{Utils.prefix_The(objectToAdd.Name)} is too heavy to be added to the {Name}.");
            }
        }

        /// <summary>
        /// Called when an item is being added to check that the new item fits into
        /// this container.
        /// </summary>
        private ActionResult add_checkSize(ObjectBase objectToAdd)
        {
            // To check that the item fits, we check that each dimension of the
            // object being added fits into the container...
            var objectDimensions = objectToAdd.getOrderedDimensions();
            var containerDimensions = getOrderedDimensions();
            for(var i=0; i<3; ++i)
            {
                if(objectDimensions[i] >= containerDimensions[i])
                {
                    return ActionResult.failed($"{Utils.prefix_The(objectToAdd.Name)} is too large to add to the {Name}.");
                }
            }
            return ActionResult.succeeded();
        }

        /// <summary>
        /// Returns the total weight (in kg) of the items in the container. 
        /// </summary>
        private double getContentsWeight()
        {
            return m_contents.Sum(x => x.TotalWeightKG);
        }

        #endregion

        #region Private data

        // The container's contents...
        private readonly List<ObjectBase> m_contents = new List<ObjectBase>();

        #endregion
    }
}

namespace WorldLib
{
    /// <summary>
    /// Holds an object and the container it is part of, along with some helper functions.
    /// </summary>
    public class ContainedObject
    {
        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public ContainedObject(Container container, ObjectBase objectBase)
        {
            m_container = container;
            m_object = objectBase;
        }

        /// <summary>
        /// Returns true if we hold valid info, false if not.
        /// </summary>
        public bool hasObject()
        {
            if (m_container == null) return false;
            if (m_object == null) return false;
            return true;
        }

        /// <summary>
        /// Removes the object from the container.
        /// </summary>
        public void removeFromContainer()
        {
            if(hasObject())
            {
                m_container.remove(m_object);
            }
        }

        /// <summary>
        /// Transfers the object to the container.
        /// </summary>
        public ActionResult transferTo(Container container)
        {
            // Do we have an object?
            if(!hasObject())
            {
                return ActionResult.failed("There is no object to tranfer");
            }

            // We transfer the object - ie, add it to the new container, and remove it from 
            // its current container...
            var actionResult = container.add(m_object);
            if(actionResult.Status == ActionResult.StatusEnum.SUCCEEDED)
            {
                removeFromContainer();
            }
            return actionResult;
        }

        /// <summary>
        /// Returns the container holding the object.
        /// </summary>
        public Container getContainer()
        {
            return m_container;
        }

        /// <summary>
        /// Returns the object.
        /// </summary>
        public ObjectBase getObject()
        {
            return m_object;
        }

        /// <summary>
        /// Returns the object as type T.
        /// Returns null if the object is not a T.
        /// </summary>
        public T getObjectAs<T>() where T : class
        {
            return m_object as T;
        }

        #endregion

        #region Private data

        // Construction parameters...
        private readonly Container m_container;
        private readonly ObjectBase m_object;

        #endregion
    }
}
